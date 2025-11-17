using UnityEngine;

namespace DodoRun.Platform
{
    [CreateAssetMenu(fileName = "PlatformSO", menuName = "ScriptableObject/PlatformSO")]
    public class PlatformScriptableObject : ScriptableObject
    {
        public PlatformView Platform;
        public float MoveSpeed;
        public float PlatformLength;
        public Vector3 spawnPosition;
    }
}
