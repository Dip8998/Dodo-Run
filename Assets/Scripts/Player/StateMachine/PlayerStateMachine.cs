using System.Collections.Generic;
using DodoRun.Interfaces;

namespace DodoRun.Player
{
    public sealed class PlayerStateMachine
    {
        private readonly PlayerController owner;
        public IPlayerState CurrentState { get; private set; }

        private readonly Dictionary<PlayerState, IPlayerState> states = new();

        public PlayerStateMachine(PlayerController owner)
        {
            this.owner = owner;

            states[PlayerState.RUNNING] = new PlayerRunningState(this);
            states[PlayerState.LEFT_SWIPE] = new PlayerLeftSwipeState(this);
            states[PlayerState.RIGHT_SWIPE] = new PlayerRightSwipeState(this);
            states[PlayerState.JUMP] = new PlayerJumpState(this);
            states[PlayerState.ROLLING] = new PlayerRollingState(this);
            states[PlayerState.DEAD] = new PlayerDeadState(this);

            foreach (var s in states.Values)
                s.Owner = owner;

            ChangeState(PlayerState.RUNNING);
        }

        public void Update() => CurrentState?.Update();

        public void ChangeState(PlayerState state)
        {
            CurrentState?.OnStateExit();
            CurrentState = states[state];
            CurrentState.OnStateEnter();
        }

        public PlayerState GetCurrentStateEnum()
        {
            foreach (var entry in states)
            {
                if (entry.Value == CurrentState) return entry.Key;
            }
            return PlayerState.RUNNING;
        }
    }
}
