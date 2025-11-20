using DodoRun.Main;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerController
    {
        private PlayerScriptableObject playerScriptableObject;
        private PlayerView playerView;
        private Vector3 firstTouchPosition;
        private Vector3 lastTouchPosition;
        private float laneVelocity;
        private int currentLane = 0;
        private bool isGrounded;
        private Rigidbody rigidbody;
        private bool canAcceptInput = true;
        private const float minSwipeDistance = 80f;
        private const float minSwipeSpeed = 300f;
        private const float directionThreshold = 0.9f;
        private float touchStartTime;
        private float lastGroundedTime;
        private Animator playerAnimator;
        private bool isSliding = false;
        private float slideDuration = 0.9f; 
        private CapsuleCollider capsuleCollider;
        private float originalHeight;
        private float originalCenterY;


        public PlayerController(PlayerScriptableObject playerScriptableObject)
        {
            this.playerScriptableObject = playerScriptableObject;
            SetupView();
        }

        private void SetupView()
        {
            playerView = Object.Instantiate(playerScriptableObject.Player, playerScriptableObject.SpawnPosition, Quaternion.identity);
            playerView.SetController(this);
            rigidbody = playerView.GetComponent<Rigidbody>();
            capsuleCollider = playerView.GetComponent<CapsuleCollider>();
            originalHeight = capsuleCollider.height;
            originalCenterY = capsuleCollider.center.y;
            playerAnimator = playerView.GetComponent<Animator>();

            GameService.Instance.StartCoroutine(InvokeSpawn());
        }

        private IEnumerator InvokeSpawn()
        {
            yield return null;
            GameService.Instance.EventService.OnPlayerSpawned.InvokeEvent(playerView.transform);
        }

        public void UpdatePlayer()
        {
            HandleSwipeInputs();
            MoveToLane();
        }

        public void FixedUpdatePlayer()
        {
            HandleGroundCheck();
        }

        private void HandleGroundCheck()
        {
            if (isSliding)
                return;

            isGrounded = Physics.CheckSphere(
                playerView.GroundCheckPosition.position,
                playerScriptableObject.GroundCheckRadius,
                playerScriptableObject.GroundLayer
            );

            if (isGrounded)
            {
                Debug.Log("Player on Grounded");
                lastGroundedTime = Time.time;
                playerAnimator.SetBool("IsGrounded", true);
            }
            else
            {
                Debug.Log("Player in Air");
                playerAnimator.SetBool("IsGrounded", false);
            }
        }

        private void MoveToLane()
        {
            float targetX = currentLane * playerScriptableObject.LaneOffset;

            Vector3 pos = playerView.transform.position;

            if (isGrounded)
            {
                pos.x = Mathf.SmoothDamp(pos.x, targetX, ref laneVelocity, 0.08f);
            }
            else
            {
                pos.x = Mathf.MoveTowards(pos.x, targetX, 8f * Time.deltaTime);
            }

            playerView.transform.position = pos;
        }


        private void HandleSwipeInputs()
        {
            if (!canAcceptInput) return;

            if (Input.touchCount != 1)
                return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartTime = Time.time;
                    firstTouchPosition = touch.position;
                    lastTouchPosition = touch.position;
                    break;
                case TouchPhase.Moved:
                    lastTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    lastTouchPosition = touch.position;
                    DetectSwipeInputs();
                    break;
            }
        }

        private void DetectSwipeInputs()
        {
            Vector2 diff = lastTouchPosition - firstTouchPosition;
            float distance = diff.magnitude;

            if (distance < minSwipeDistance)
                return;

            float time = Time.time - touchStartTime;

            if (time <= 0.01f)
                return;

            float speed = distance / time;

            if (speed < minSwipeSpeed)
                return;

            Vector2 direction = diff.normalized;

            if (Vector2.Dot(direction, Vector2.right) > directionThreshold)
                MoveRight();
            else if (Vector2.Dot(direction, Vector2.left) > directionThreshold)
                MoveLeft();
            else if (Vector2.Dot(direction, Vector2.up) > directionThreshold)
            {
                if (IsGroundedSafe()) MoveUp();
                else
                    return;
            }
            else if (Vector2.Dot(direction, Vector2.down) > directionThreshold)
                MoveDown();

            canAcceptInput = false;
            GameService.Instance.StartCoroutine(InputCooldown(0.1f));
        }


        private IEnumerator InputCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            canAcceptInput = true;
        }

        private void MoveRight()
        {
            if (currentLane < 1)
            {
                currentLane++;
            }
        }

        private void MoveLeft()
        {
            if (currentLane > -1)
                currentLane--;
        }

        private void MoveUp()
        {
            if (!isGrounded || isSliding) return;

            rigidbody.linearVelocity = new Vector3(
                rigidbody.linearVelocity.x,
                playerScriptableObject.JumpSpeed,
                rigidbody.linearVelocity.z
            );

            playerAnimator.SetTrigger("Jump");
        }

        private void MoveDown()
        {
            if (!isGrounded) return;        
            if (isSliding) return;       

            GameService.Instance.StartCoroutine(SlideRoutine());
        }

        private IEnumerator SlideRoutine()
        {
            if (isSliding) yield break;
            isSliding = true;
            canAcceptInput = false;

            playerAnimator.SetTrigger("Slide");

            capsuleCollider.height = originalHeight * 0.5f;
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                 0.5f,
                capsuleCollider.center.z
            );

            yield return new WaitForSeconds(.75f);

            capsuleCollider.height = originalHeight;
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                originalCenterY,
                capsuleCollider.center.z
            );

            isGrounded = true;
            lastGroundedTime = Time.time;
            playerAnimator.SetBool("IsGrounded", true);

            isSliding = false;
            canAcceptInput = true;
        }



        bool IsGroundedSafe()
        {
            return Time.time - lastGroundedTime < 0.05f;
        }
    }
}