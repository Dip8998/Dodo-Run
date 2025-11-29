using DodoRun.Main;
using System.Collections;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerController
    {
        public PlayerScriptableObject PlayerScriptableObject { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public CapsuleCollider CapsuleCollider { get; private set; }
        public Animator PlayerAnimator { get; private set; }

        public int CurrentLane { get; set; } = 0;
        public bool IsGrounded { get; private set; }
        public bool IsSliding { get; set; } = false;
        public bool CanAcceptInput { get; set; } = true;

        public float OriginalHeight { get; private set; }
        public float OriginalCenterY { get; private set; }
        public float SlideDuration { get; private set; } = 0.75f;

        private PlayerStateMachine playerStateMachine;
        private PlayerView playerView;

        private Vector3 firstTouchPosition;
        private Vector3 lastTouchPosition;
        private float touchStartTime;
        private float lastGroundedTime;
        private float laneVelocity;

        private const float minSwipeDistance = 80f;
        private const float minSwipeSpeed = 300f;
        private const float directionThreshold = 0.9f;

        private bool isJumping = false;
        private float jumpDuration = .75f;   
        private float jumpHeight = 1.7f;

        public PlayerController(PlayerScriptableObject playerScriptableObject)
        {
            this.PlayerScriptableObject = playerScriptableObject;
            SetupView();
            playerStateMachine = new PlayerStateMachine(this);
        }

        private void SetupView()
        {
            playerView = Object.Instantiate(PlayerScriptableObject.Player, PlayerScriptableObject.SpawnPosition, Quaternion.identity);
            playerView.SetController(this);
            Rigidbody = playerView.GetComponent<Rigidbody>();
            CapsuleCollider = playerView.GetComponent<CapsuleCollider>();
            PlayerAnimator = playerView.GetComponent<Animator>();

            OriginalHeight = CapsuleCollider.height;
            OriginalCenterY = CapsuleCollider.center.y;

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
            playerStateMachine.Update();

            if (PlayerAnimator != null && GameService.Instance != null)
            {
                PlayerAnimator.speed = Mathf.Lerp(1f, 1.75f, GameService.Instance.Difficulty.Progress);
            }
        }

        public void FixedUpdatePlayer() => HandleGroundCheck();

        public void Die()
        {
            if (playerStateMachine.CurrentState is PlayerDeadState)
                return;

            playerStateMachine.ChangeState(PlayerState.DEAD);
        }

        public void StopMovement()
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }

        public void MoveToLane()
        {
            float targetX = CurrentLane * PlayerScriptableObject.LaneOffset;
            Vector3 pos = playerView.transform.position;

            if (IsGrounded && !isJumping)
            {
                pos.x = Mathf.SmoothDamp(pos.x, targetX, ref laneVelocity, 0.08f);
            }
            else
            {
                pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * 7f);
            }

            playerView.transform.position = pos;
        }


        private void HandleGroundCheck()
        {
            if (IsSliding) return;

            IsGrounded = Physics.CheckSphere(
                playerView.GroundCheckPosition.position,
                PlayerScriptableObject.GroundCheckRadius,
                PlayerScriptableObject.GroundLayer
            );

            if (IsGrounded)
            {
                lastGroundedTime = Time.time;
                PlayerAnimator.SetBool("IsGrounded", true);
            }
            else
            {
                PlayerAnimator.SetBool("IsGrounded", false);
            }
        }

        private void HandleSwipeInputs()
        {
            if (!CanAcceptInput) return;

            if (Input.touchCount != 1) return;

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

            if (distance < minSwipeDistance) return;

            float time = Time.time - touchStartTime;

            if (time <= 0.01f) return;

            float speed = distance / time;

            if (speed < minSwipeSpeed) return;

            Vector2 direction = diff.normalized;

            bool inputConsumed = false;

            if (Vector2.Dot(direction, Vector2.right) > directionThreshold)
            {
                playerStateMachine.ChangeState(PlayerState.RIGHT_SWIPE);
                inputConsumed = true;
            }
            else if (Vector2.Dot(direction, Vector2.left) > directionThreshold)
            {
                playerStateMachine.ChangeState(PlayerState.LEFT_SWIPE);
                inputConsumed = true;
            }
            else if (Vector2.Dot(direction, Vector2.up) > directionThreshold)
            {
                if (IsGroundedSafe())
                {
                    playerStateMachine.ChangeState(PlayerState.JUMP);
                    inputConsumed = true;
                }
            }
            else if (Vector2.Dot(direction, Vector2.down) > directionThreshold)
            {
                playerStateMachine.ChangeState(PlayerState.ROLLING);
                inputConsumed = true;
            }

            if (inputConsumed)
            {
                CanAcceptInput = false;
                GameService.Instance.StartCoroutine(InputCooldown(0.1f));
            }
        }

        private IEnumerator InputCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (!IsSliding && !(playerStateMachine.CurrentState is PlayerDeadState))
            {
                CanAcceptInput = true;
            }
        }

        bool IsGroundedSafe() => Time.time - lastGroundedTime < 0.05f;

        public IEnumerator DoSubwayJump()
        {
            isJumping = true;
            float timer = 0f;

            PlayerAnimator.SetTrigger("Jump");

            Vector3 startPos = playerView.transform.position;

            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;

                float normalizedTime = timer / jumpDuration;
                float yOffset = PlayerScriptableObject.JumpCurve.Evaluate(normalizedTime) * jumpHeight;

                Vector3 pos = playerView.transform.position;
                pos.y = startPos.y + yOffset;
                playerView.transform.position = pos;

                yield return null;
            }

            isJumping = false;
        }
    }
}