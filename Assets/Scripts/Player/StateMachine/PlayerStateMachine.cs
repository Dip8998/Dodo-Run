using DodoRun.Interfaces;
using System.Collections.Generic;

namespace DodoRun.Player
{
    public class PlayerStateMachine
    {
        private PlayerController Owner;
        public IPlayerState CurrentState { get; private set; }

        private readonly Dictionary<PlayerState, IPlayerState> States = new();

        public PlayerStateMachine(PlayerController owner)
        {
            Owner = owner;
            CreateState();
            AssignOwner();

            ChangeState(PlayerState.RUNNING);
        }

        private void AssignOwner()
        {
            foreach (var state in States.Values)
                state.Owner = Owner;
        }

        private void CreateState()
        {
            States.Add(PlayerState.RUNNING, new PlayerRunningState(this));
            States.Add(PlayerState.LEFT_SWIPE, new PlayerLeftSwipeState(this));
            States.Add(PlayerState.RIGHT_SWIPE, new PlayerRightSwipeState(this));
            States.Add(PlayerState.JUMP, new PlayerJumpState(this));
            States.Add(PlayerState.ROLLING, new PlayerRollingState(this));
            States.Add(PlayerState.DEAD, new PlayerDeadState(this));
        }

        public void Update() => CurrentState?.Update();

        protected void ChangeState(IPlayerState newState)
        {
            CurrentState?.OnStateExit();
            CurrentState = newState;
            CurrentState?.OnStateEnter();
        }

        public void ChangeState(PlayerState state) => ChangeState(States[state]);
    }
}
