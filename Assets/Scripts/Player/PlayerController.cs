using System.Collections;
using UnityEngine;
using DodoRun.Main;
using DodoRun.Coin;
using DodoRun.Tutorial;
using DodoRun.Sound;

namespace DodoRun.Player
{
    public sealed class PlayerController
    {
        public PlayerScriptableObject PlayerScriptableObject { get; }
        public Rigidbody Rigidbody { get; }
        public CapsuleCollider CapsuleCollider { get; }
        public Animator PlayerAnimator { get; }
        public PlayerView PlayerView { get; }

        public int CurrentLane { get; set; }
        public bool IsGrounded { get; private set; }
        public bool IsSliding { get; set; }
        public bool CanAcceptInput { get; set; } = true;

        public float OriginalHeight { get; }
        public float OriginalCenterY { get; }
        public float SlideDuration { get; } = 0.75f;

        private readonly PlayerStateMachine stateMachine;
        private readonly GameService game;

        private Transform transform;
        private Transform groundCheck;

        private Vector3 touchStart;
        private Vector3 touchEnd;
        private float touchStartTime;
        private float lastGroundedTime;
        private float laneVelocity;

        private Vector2 bufferedSwipe;
        private float bufferTimer;

        private const float BASE_LANE_SMOOTH = 0.08f;
        private const float FAST_LANE_SMOOTH = 0.035f;

        private const float BASE_JUMP_DURATION = 0.75f;
        private const float FAST_JUMP_DURATION = 0.45f;

        private const float LANE_SNAP_THRESHOLD = 0.05f;
        private const float INPUT_BUFFER_TIME = 0.15f;

        private const float MinSwipeDistance = 80f;
        private const float MinSwipeSpeed = 300f;
        private const float DirectionThreshold = 0.9f;

        private readonly float jumpHeight = 1.7f;

        public PlayerController(PlayerScriptableObject data)
        {
            PlayerScriptableObject = data;
            game = GameService.Instance;

            PlayerView = Object.Instantiate(
                data.Player,
                data.SpawnPosition,
                Quaternion.identity
            );

            PlayerView.SetController(this);

            Rigidbody = PlayerView.GetComponent<Rigidbody>();
            CapsuleCollider = PlayerView.GetComponent<CapsuleCollider>();
            PlayerAnimator = PlayerView.GetComponent<Animator>();

            transform = Rigidbody.transform;
            groundCheck = PlayerView.GroundCheckPosition;

            OriginalHeight = CapsuleCollider.height;
            OriginalCenterY = CapsuleCollider.center.y;

            stateMachine = new PlayerStateMachine(this);

            game.StartCoroutine(NotifySpawn());
        }

        private IEnumerator NotifySpawn()
        {
            yield return null;
            game.EventService.OnPlayerSpawned.InvokeEvent(transform);
        }

        public void UpdatePlayer()
        {
            HandleInput();
            ProcessBufferedInput();
            stateMachine.Update();

            if (game.PowerupService?.IsMagnetActive == true)
                AttractCoins();
        }

        public void FixedUpdatePlayer()
        {
            UpdateGroundedState();
        }

        public void MoveToLane()
        {
            float targetX = CurrentLane * PlayerScriptableObject.LaneOffset;
            Vector3 pos = transform.position;

            float smoothTime = Mathf.Lerp(
                BASE_LANE_SMOOTH,
                FAST_LANE_SMOOTH,
                game.Difficulty.Progress
            );

            pos.x = Mathf.SmoothDamp(
                pos.x,
                targetX,
                ref laneVelocity,
                smoothTime
            );

            if (Mathf.Abs(pos.x - targetX) < LANE_SNAP_THRESHOLD)
                pos.x = targetX;

            transform.position = pos;
        }

        public void Die()
        {
            if (stateMachine.CurrentState is PlayerDeadState) return;

            AudioManager.Instance.SetRunningSoundActive(false);

            AudioManager.Instance.PlayEffect(SoundType.ObstacleHit);
            stateMachine.ChangeState(PlayerState.DEAD);
        }

        public void StopMovement()
        {
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }

        private void UpdateGroundedState()
        {
            if (IsSliding) return;

            IsGrounded = Physics.CheckSphere(
                groundCheck.position,
                PlayerScriptableObject.GroundCheckRadius,
                PlayerScriptableObject.GroundLayer
            );

            PlayerAnimator.SetBool("IsGrounded", IsGrounded);

            if (IsGrounded)
                lastGroundedTime = Time.time;
        }

        private void HandleInput()
        {
            if (Input.touchCount != 1) return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.y > Screen.height * 0.5f)
                {
                    touchStartTime = -1f;
                    return;
                }

                touchStartTime = Time.time;
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (touchStartTime == -1f) return;

                touchEnd = touch.position;
                Vector2 delta = touchEnd - touchStart;

                if (delta.magnitude < MinSwipeDistance) return;

                float elapsed = Time.time - touchStartTime;
                if (elapsed <= 0f) return;

                if ((delta.magnitude / elapsed) < MinSwipeSpeed) return;

                bufferedSwipe = delta.normalized;
                bufferTimer = INPUT_BUFFER_TIME;
            }
        }

        private void ProcessBufferedInput()
        {
            if (bufferTimer <= 0f) return;

            bufferTimer -= Time.deltaTime;

            if (!CanAcceptInput || stateMachine.CurrentState is PlayerDeadState)
                return;

            Vector2 dir = bufferedSwipe;

            var tutorial = game.TutorialService;
            if (tutorial != null && tutorial.IsActive && !tutorial.CanProcessSwipe(dir))
                return;

            if (Vector2.Dot(dir, Vector2.right) > DirectionThreshold)
            {
                AudioManager.Instance.SetRunningSoundActive(false);
                AudioManager.Instance.PlayEffect(SoundType.Swipe);
                stateMachine.ChangeState(PlayerState.RIGHT_SWIPE);
            }
            else if (Vector2.Dot(dir, Vector2.left) > DirectionThreshold)
            {
                AudioManager.Instance.SetRunningSoundActive(false);
                AudioManager.Instance.PlayEffect(SoundType.Swipe);
                stateMachine.ChangeState(PlayerState.LEFT_SWIPE);
            }
            else if (Vector2.Dot(dir, Vector2.up) > DirectionThreshold && IsJumpAllowed())
            {
                AudioManager.Instance.SetRunningSoundActive(false);
                AudioManager.Instance.PlayEffect(SoundType.Jump);
                stateMachine.ChangeState(PlayerState.JUMP);
            }
            else if (Vector2.Dot(dir, Vector2.down) > DirectionThreshold)
            {
                AudioManager.Instance.SetRunningSoundActive(false);
                AudioManager.Instance.PlayEffect(SoundType.Slide);
                stateMachine.ChangeState(PlayerState.ROLLING);
            }
            bufferTimer = 0f;
        }

        private bool IsJumpAllowed()
        {
            return Time.time - lastGroundedTime < 0.05f;
        }

        private void AttractCoins()
        {
            var coins = game.CoinService.ActiveCoins;
            float range = game.PowerupService.MagnetRange;

            Vector3 target = transform.position + new Vector3(0f, 0.5f, 1.2f);

            for (int i = 0; i < coins.Count; i++)
            {
                var c = coins[i];
                if (!c.IsUsed()) continue;

                Transform t = c.CoinView.transform;

                float dz = t.position.z - transform.position.z;
                if (dz < -1f || dz > range) continue;

                float dx = Mathf.Abs(t.position.x - transform.position.x);
                if (dx > 4f) continue;

                t.position = Vector3.Lerp(t.position, target, Time.deltaTime * 14f);

                if (Vector3.Distance(t.position, transform.position) < 0.55f)
                    c.Collect();
            }
        }

        public IEnumerator DoSubwayJump()
        {
            PlayerAnimator.SetTrigger("Jump");

            float jumpDuration = Mathf.Lerp(
                BASE_JUMP_DURATION,
                FAST_JUMP_DURATION,
                game.Difficulty.Progress
            );

            Vector3 start = transform.position;
            float timer = 0f;

            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;
                float t = timer / jumpDuration;

                Vector3 pos = start;
                pos.y += PlayerScriptableObject.JumpCurve.Evaluate(t) * jumpHeight;
                transform.position = pos;

                yield return null;
            }
        }

        public PlayerState GetState()
        {
            return stateMachine.GetCurrentStateEnum();
        }
    }
}
