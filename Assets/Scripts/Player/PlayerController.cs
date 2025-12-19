using System.Collections;
using UnityEngine;
using DodoRun.Main;
using DodoRun.Coin;
using DodoRun.Tutorial;

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

        private bool isJumping;

        private const float MinSwipeDistance = 80f;
        private const float MinSwipeSpeed = 300f;
        private const float DirectionThreshold = 0.9f;

        private readonly float jumpDuration = 0.75f;
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
            stateMachine.Update();

            if (game.PowerupService?.IsMagnetActive == true)
                AttractCoins();

            PlayerAnimator.speed = Mathf.Lerp(1f, 1.75f, game.Difficulty.Progress);
        }

        public void FixedUpdatePlayer()
        {
            UpdateGroundedState();
        }

        public void MoveToLane()
        {
            float targetX = CurrentLane * PlayerScriptableObject.LaneOffset;
            Vector3 pos = transform.position;

            pos.x = IsGrounded && !isJumping
                ? Mathf.SmoothDamp(pos.x, targetX, ref laneVelocity, 0.08f)
                : Mathf.Lerp(pos.x, targetX, Time.deltaTime * 7f);

            transform.position = pos;
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
            if (!CanAcceptInput || Input.touchCount != 1) return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEnd = touch.position;
                ProcessSwipe();
            }
        }

        private void ProcessSwipe()
        {
            Vector2 delta = touchEnd - touchStart;
            float distance = delta.magnitude;
            if (distance < MinSwipeDistance) return;

            float elapsed = Time.time - touchStartTime;
            if (elapsed <= 0f) return;

            if (distance / elapsed < MinSwipeSpeed) return;

            Vector2 dir = delta.normalized;

            var tutorial = game.TutorialService;
            if (tutorial != null && tutorial.IsActive && !tutorial.CanProcessSwipe(dir))
                return;

            if (Vector2.Dot(dir, Vector2.right) > DirectionThreshold)
                stateMachine.ChangeState(PlayerState.RIGHT_SWIPE);
            else if (Vector2.Dot(dir, Vector2.left) > DirectionThreshold)
                stateMachine.ChangeState(PlayerState.LEFT_SWIPE);
            else if (Vector2.Dot(dir, Vector2.up) > DirectionThreshold && IsJumpAllowed())
                stateMachine.ChangeState(PlayerState.JUMP);
            else if (Vector2.Dot(dir, Vector2.down) > DirectionThreshold)
                stateMachine.ChangeState(PlayerState.ROLLING);

            CanAcceptInput = false;
            game.StartCoroutine(InputCooldown());
        }

        private IEnumerator InputCooldown()
        {
            yield return new WaitForSeconds(0.1f);
            if (!IsSliding && stateMachine.CurrentState is not PlayerDeadState)
                CanAcceptInput = true;
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

                t.position = Vector3.Lerp(t.position, target, Time.deltaTime * 12f);

                if (Vector3.Distance(t.position, transform.position) < 0.55f)
                    c.Collect();
            }
        }

        public IEnumerator DoSubwayJump()
        {
            isJumping = true;
            PlayerAnimator.SetTrigger("Jump");

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

            isJumping = false;
        }
    }
}
