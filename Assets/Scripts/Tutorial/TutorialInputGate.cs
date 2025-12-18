using DodoRun.Player;

namespace DodoRun.Tutorial
{
    public class TutorialInputGate
    {
        private PlayerController player;

        public bool AllowLeftRight { get; private set; }
        public bool AllowJump { get; private set; }
        public bool AllowSlide { get; private set; }

        public TutorialInputGate(PlayerController player)
        {
            this.player = player;
            LockAll();
        }

        public void LockAll()
        {
            AllowLeftRight = false;
            AllowJump = false;
            AllowSlide = false;
            player.CanAcceptInput = false;
        }

        public void EnableSwipeOnly()
        {
            AllowLeftRight = true;
            AllowJump = false;
            AllowSlide = false;
            player.CanAcceptInput = true;
        }

        public void EnableAll()
        {
            AllowLeftRight = true;
            AllowJump = true;
            AllowSlide = true;
            player.CanAcceptInput = true;
        }
    }
}
