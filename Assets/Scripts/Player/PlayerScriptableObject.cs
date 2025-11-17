using UnityEngine;

namespace DodoRun.Player
{
    [CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObject/PlayerSO")]

    public class PlayerScriptableObject : ScriptableObject
    {
        public PlayerView Player;
        public Vector3 SpawnPosition;
        public float SwipeSpeed;
        public float JumpSpeed;
        public float LaneOffset;
    }
}
