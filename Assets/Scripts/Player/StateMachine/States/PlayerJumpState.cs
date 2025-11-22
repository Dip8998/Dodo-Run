using DodoRun.Interfaces;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerJumpState : IPlayerState
    {
        public PlayerController Owner { get; set; }
        private PlayerStateMachine stateMachine;

        public PlayerJumpState(PlayerStateMachine stateMachine) => this.stateMachine = stateMachine;

        public void OnStateEnter()
        {
            if(Owner.IsGrounded && !Owner.IsSliding)
            {
                Rigidbody rb = Owner.Rigidbody;
                float jumpSpeed = Owner.PlayerScriptableObject.JumpSpeed;

                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, rb.linearVelocity.y);

                Owner.PlayerAnimator.SetTrigger("Jump");
            }

            stateMachine.ChangeState(PlayerState.RUNNING);
        }

        public void Update() { }

        public void OnStateExit() { }
    }
}