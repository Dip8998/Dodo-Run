using DodoRun.Interfaces;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerDeadState : IPlayerState
    {
        public PlayerController Owner { get; set; }
        private PlayerStateMachine stateMachine;

        public PlayerDeadState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnStateEnter()
        {
            Debug.Log("Player hit an obstacle! Transitioning to Death state.");

            Owner.CanAcceptInput = false;

            Owner.StopMovement();

            Owner.PlayerAnimator.SetTrigger("Dead");
        }

        public void Update()
        {
        }

        public void OnStateExit()
        {
        }
    }
}