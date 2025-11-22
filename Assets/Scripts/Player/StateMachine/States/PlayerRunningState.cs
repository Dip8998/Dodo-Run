using DodoRun.Interfaces;

namespace DodoRun.Player
{
    public class PlayerRunningState : IPlayerState
    {
        public PlayerController Owner { get; set; }
        private PlayerStateMachine stateMachine;

        public PlayerRunningState(PlayerStateMachine stateMachine) => this.stateMachine = stateMachine;

        public void OnStateEnter()
        {
            Owner.CanAcceptInput = true;
        }

        public void Update()
        {
            Owner.MoveToLane();
        }

        public void OnStateExit()
        {
            Owner.CanAcceptInput = false;  
        }
    }
}
