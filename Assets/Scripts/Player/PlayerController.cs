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
        }

        public void FixedUpdatePlayer() => HandleGroundCheck();

        public void MoveToLane()
        {
            float targetX = CurrentLane * PlayerScriptableObject.LaneOffset;
            Vector3 pos = playerView.transform.position;

            if (IsGrounded)
                pos.x = Mathf.SmoothDamp(pos.x, targetX, ref laneVelocity, 0.08f);
            else
                pos.x = Mathf.MoveTowards(pos.x, targetX, 8f * Time.deltaTime);

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

            if (Vector2.Dot(direction, Vector2.right) > directionThreshold)
                playerStateMachine.ChangeState(PlayerState.RIGHT_SWIPE);
            else if (Vector2.Dot(direction, Vector2.left) > directionThreshold)
                playerStateMachine.ChangeState(PlayerState.LEFT_SWIPE);
            else if (Vector2.Dot(direction, Vector2.up) > directionThreshold)
            {
                if (IsGroundedSafe()) playerStateMachine.ChangeState(PlayerState.JUMP);
                else return;
            }
            else if (Vector2.Dot(direction, Vector2.down) > directionThreshold)
                playerStateMachine.ChangeState(PlayerState.ROLLING);
        }

        bool IsGroundedSafe() => Time.time - lastGroundedTime < 0.05f;
    }
}