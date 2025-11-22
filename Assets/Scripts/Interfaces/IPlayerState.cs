using DodoRun.Player;

namespace DodoRun.Interfaces
{
    public interface IPlayerState
    {
        PlayerController Owner { get; set; }
        void OnStateEnter();
        void Update();
        void OnStateExit();
    }
}
