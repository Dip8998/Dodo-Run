using DodoRun.Interfaces;
using DodoRun.Main;
using System.Collections;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerRollingState : IPlayerState
    {
        public PlayerController Owner { get; set; }
        private PlayerStateMachine stateMachine;

        public PlayerRollingState(PlayerStateMachine stateMachine) => this.stateMachine = stateMachine;

        public void OnStateEnter()
        {
            if (Owner.IsGrounded && !Owner.IsSliding)
            {
                GameService.Instance.StartCoroutine(SlideRoutine());
            }
            else
            {
                stateMachine.ChangeState(PlayerState.RUNNING);
            }
        }

        public void Update() { }

        public void OnStateExit() { }

        private IEnumerator SlideRoutine()
        {
            Owner.IsSliding = true;
            Owner.CanAcceptInput = false;

            float slideDuration = Owner.SlideDuration;
            CapsuleCollider capsuleCollider = Owner.CapsuleCollider;
            float originalHeight = Owner.OriginalHeight;
            float originalCenterY = Owner.OriginalCenterY;

            Owner.PlayerAnimator.SetTrigger("Slide");

            capsuleCollider.height = originalHeight * 0.5f;
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                0.5f,
                capsuleCollider.center.z
            );

            yield return new WaitForSeconds(slideDuration);

            capsuleCollider.height = originalHeight;
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                originalCenterY,
                capsuleCollider.center.z
            );

            Owner.IsSliding = false;
            Owner.CanAcceptInput = true;

            stateMachine.ChangeState(PlayerState.RUNNING);
        }
    }
}
