using DodoRun.Interfaces;

namespace DodoRun.Player
{
    public class PlayerLeftSwipeState : IPlayerState
    {
        public PlayerController Owner { get; set; }
        private PlayerStateMachine stateMachine;

        public PlayerLeftSwipeState(PlayerStateMachine stateMachine) => this.stateMachine = stateMachine;

        public void OnStateEnter()
        {
            if(Owner.CurrentLane > -1)
            {
                Owner.CurrentLane--;
            }
            stateMachine.ChangeState(PlayerState.RUNNING);
        }

        public void Update() { }

        public void OnStateExit() { }
    }
}
