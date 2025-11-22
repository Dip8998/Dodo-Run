using DodoRun.Interfaces;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace DodoRun.Player
{
    public class PlayerStateMachine
    {
        private PlayerController Owner;
        private IPlayerState currentState;
        protected Dictionary<PlayerState, IPlayerState> States = new();

        public PlayerStateMachine(PlayerController owner)
        {
            Owner = owner;
            CreateState();
            AssignOwner();

            ChangeState(PlayerState.RUNNING);
        }

        private void AssignOwner()
        {
            foreach(var playerState  in States.Values)
            {
                playerState.Owner = Owner;
            }
        }

        private void CreateState()
        {
            States.Add(PlayerState.RUNNING, new PlayerRunningState(this));
            States.Add(PlayerState.LEFT_SWIPE, new PlayerLeftSwipeState(this));
            States.Add(PlayerState.RIGHT_SWIPE, new PlayerRightSwipeState(this));
            States.Add(PlayerState.JUMP, new PlayerJumpState(this));
            States.Add(PlayerState.ROLLING, new PlayerRollingState(this));
        }

        public void Update() => currentState?.Update();

        protected void ChangeState(IPlayerState newState)
        {
            currentState?.OnStateExit();
            currentState = newState;
            currentState?.OnStateEnter();
        }

        public void ChangeState(PlayerState state) => ChangeState(States[state]); 
    }
}
