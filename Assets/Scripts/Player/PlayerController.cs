using DodoRun.Coin;
using DodoRun.Main;
using DodoRun.Tutorial;
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
        public PlayerView PlayerView { get; private set; }

        public int CurrentLane { get; set; } = 0;
        public bool IsGrounded { get; private set; }
        public bool IsSliding { get; set; } = false;
        public bool CanAcceptInput { get; set; } = true;

        public float OriginalHeight { get; private set; }
        public float OriginalCenterY { get; private set; }
        public float SlideDuration { get; private set; } = 0.75f;

        private PlayerStateMachine stateMachine;
        private Transform playerTransform;
        private Transform groundCheckTransform;

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

        private GameService gameService;

        public PlayerController(PlayerScriptableObject playerScriptableObject)
        {
            PlayerScriptableObject = playerScriptableObject;

            gameService = GameService.Instance;
            SetupView();

            stateMachine = new PlayerStateMachine(this);
        }

        private void SetupView()
        {
            PlayerView = Object.Instantiate(PlayerScriptableObject.Player, PlayerScriptableObject.SpawnPosition, Quaternion.identity);
            PlayerView.SetController(this);

            Rigidbody = PlayerView.GetComponent<Rigidbody>();
            CapsuleCollider = PlayerView.GetComponent<CapsuleCollider>();
            PlayerAnimator = PlayerView.GetComponent<Animator>();

            playerTransform = Rigidbody.transform;
            groundCheckTransform = PlayerView.GroundCheckPosition;

            OriginalHeight = CapsuleCollider.height;
            OriginalCenterY = CapsuleCollider.center.y;

            gameService.StartCoroutine(InvokeSpawn());
        }

        private IEnumerator InvokeSpawn()
        {
            yield return null;
            gameService.EventService.OnPlayerSpawned.InvokeEvent(playerTransform);
        }

        public void UpdatePlayer()
        {
            HandleSwipeInputs();
            stateMachine.Update();

            if (GameService.Instance.PowerupService != null && GameService.Instance.PowerupService.IsMagnetActive)
                AttractCoins();

            float speedMultiplier = Mathf.Lerp(1f, 1.75f, gameService.Difficulty.Progress);

            if (PlayerAnimator != null)
                PlayerAnimator.speed = speedMultiplier;
        }

        public void FixedUpdatePlayer()
        {
            HandleGroundCheck();
        }

        public void Die()
        {
            if (stateMachine.CurrentState is PlayerDeadState) return;

            stateMachine.ChangeState(PlayerState.DEAD);
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
            Vector3 pos = playerTransform.position;

            if (IsGrounded && !isJumping)
            {
                pos.x = Mathf.SmoothDamp(pos.x, targetX, ref laneVelocity, 0.08f);
            }
            else
            {
                pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * 7f);
            }

            playerTransform.position = pos;
        }

        private void HandleGroundCheck()
        {
            if (IsSliding) return;

            IsGrounded = Physics.CheckSphere(
                groundCheckTransform.position,
                PlayerScriptableObject.GroundCheckRadius,
                PlayerScriptableObject.GroundLayer
            );

            PlayerAnimator.SetBool("IsGrounded", IsGrounded);

            if (IsGrounded)
                lastGroundedTime = Time.time;
        }

        private void HandleSwipeInputs()
        {
            if (!CanAcceptInput || Input.touchCount != 1) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartTime = Time.time;
                    firstTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Ended:
                    lastTouchPosition = touch.position;
                    if (touch.phase == TouchPhase.Ended)
                        DetectSwipeInputs();
                    break;
            }
        }

        private void DetectSwipeInputs()
        {
            Vector2 diff = lastTouchPosition - firstTouchPosition;
            float distance = diff.magnitude;
            if (distance < minSwipeDistance) return;

            float elapsed = Time.time - touchStartTime;
            if (elapsed <= 0.01f) return;

            float speed = distance / elapsed;
            if (speed < minSwipeSpeed) return;

            Vector2 direction = diff.normalized;
            var tutorial = gameService.TutorialService;
            if (tutorial != null && tutorial.IsActive)
            {
                if (!tutorial.CanProcessSwipe(direction))
                    return;
            }
            bool inputConsumed = false;

            if (Vector2.Dot(direction, Vector2.right) > directionThreshold)
            {
                stateMachine.ChangeState(PlayerState.RIGHT_SWIPE);
                inputConsumed = true;
            }
            else if (Vector2.Dot(direction, Vector2.left) > directionThreshold)
            {
                stateMachine.ChangeState(PlayerState.LEFT_SWIPE);
                inputConsumed = true;
            }
            else if (Vector2.Dot(direction, Vector2.up) > directionThreshold && IsGroundedSafe())
            {
                stateMachine.ChangeState(PlayerState.JUMP);
                inputConsumed = true;
            }
            else if (Vector2.Dot(direction, Vector2.down) > directionThreshold)
            {
                stateMachine.ChangeState(PlayerState.ROLLING);
                inputConsumed = true;
            }

            if (inputConsumed)
            {
                CanAcceptInput = false;
                gameService.StartCoroutine(InputCooldown(0.1f));
            }
        }

        public void ForceLane(int lane)
        {
            CurrentLane = lane;
        }

        private IEnumerator InputCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (!IsSliding && !(stateMachine.CurrentState is PlayerDeadState))
                CanAcceptInput = true;
        }

        bool IsGroundedSafe() => Time.time - lastGroundedTime < 0.05f;

        private void AttractCoins()
        {
            if (!GameService.Instance.PowerupService.IsMagnetActive)
                return;

            var coins = GameService.Instance.CoinService.ActiveCoins;
            float range = GameService.Instance.PowerupService.MagnetRange;

            Vector3 targetOffset = new Vector3(0f, 0.5f, 1.2f);
            Vector3 magnetTarget = playerTransform.position + targetOffset;

            for (int i = 0; i < coins.Count; i++)
            {
                var c = coins[i];
                if (!c.IsUsed()) continue;

                var view = c.CoinView;
                if (view == null || !view.gameObject.activeSelf) continue;

                float dz = view.transform.position.z - playerTransform.position.z;
                if (dz < -1f || dz > range) continue;

                float dx = Mathf.Abs(view.transform.position.x - playerTransform.position.x);
                if (dx > 4f) continue;

                c.IsBeingPulled = true;

                view.transform.position = Vector3.Lerp(
                    view.transform.position,
                    magnetTarget,
                    Time.deltaTime * 12f
                );

                if (Vector3.Distance(view.transform.position, playerTransform.position) < 0.55f)
                {
                    c.CollectCoin();
                }
            }
        }

        private bool IsCoinInMagnetRange(Transform coin)
        {
            float dz = coin.position.z - playerTransform.position.z;
            float dx = Mathf.Abs(coin.position.x - playerTransform.position.x);

            return dz > -0.5f && dz < GameService.Instance.PowerupService.MagnetRange && dx < 4f;
        }


        public IEnumerator DoSubwayJump()
        {
            isJumping = true;
            float timer = 0f;

            PlayerAnimator.SetTrigger("Jump");

            Vector3 startPos = playerTransform.position;

            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;
                float t = timer / jumpDuration;

                float yOffset = PlayerScriptableObject.JumpCurve.Evaluate(t) * jumpHeight;

                Vector3 pos = playerTransform.position;
                pos.y = startPos.y + yOffset;
                playerTransform.position = pos;

                yield return null;
            }

            isJumping = false;
        }
    }
}
