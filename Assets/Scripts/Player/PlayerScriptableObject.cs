using System.Collections.Generic;
using UnityEngine;

namespace DodoRun.Player
{
    [CreateAssetMenu(fileName = "PlayerSO", menuName = "ScriptableObject/PlayerSO")]

    public class PlayerScriptableObject : ScriptableObject
    {
        public PlayerView Player;
        public Vector3 SpawnPosition;
        public float GroundCheckRadius;
        public LayerMask GroundLayer;
        public float SwipeSpeed;
        public float JumpSpeed;
        public float LaneOffset;
        public AnimationCurve JumpCurve;
    }
}
