using DodoRun.Interfaces;
using DodoRun.Main;
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
            if (Owner.IsGrounded && !Owner.IsSliding)
            {
                GameService.Instance.StartCoroutine(Owner.DoSubwayJump());
            }

            stateMachine.ChangeState(PlayerState.RUNNING);
        }

        public void Update() { }

        public void OnStateExit() { }
    }
}
