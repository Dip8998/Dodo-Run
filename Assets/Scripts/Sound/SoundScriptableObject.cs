using UnityEngine;

namespace DodoRun.Sound
{
    [CreateAssetMenu(fileName = "SoundSO", menuName = "ScriptableObject/SoundSO")]
    public class SoundScriptableObject : ScriptableObject
    {
        [Header("Music")]
        public AudioClip Music;

        [Header("UI Sounds")]
        public AudioClip ButtonClick;

        [Header("Player Sounds")]
        public AudioClip RunningSound;
        public AudioClip JumpSound;
        public AudioClip SwipeSound;
        public AudioClip SlideSound;

        [Header("World Sounds")]
        public AudioClip CoinCollect;
        public AudioClip Powerup;
        public AudioClip Powerdown;
        public AudioClip ObstacleHit;
        public AudioClip MagnetCoinCollect; 
    }
}