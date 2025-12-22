using DodoRun.Main;
using DodoRun.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace DodoRun.Sound
{
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private SoundScriptableObject soundData;
        [SerializeField] private AudioSource soundMusic;
        [SerializeField] private AudioSource effectSource;
        private AudioSource runningSource;
        public AudioSource SoundFX => effectSource;
        public AudioSource SoundMusic => soundMusic;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayMusic(SoundType.Music);
            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            runningSource = gameObject.AddComponent<AudioSource>();
            runningSource.clip = soundData.RunningSound;
            runningSource.loop = true;
            runningSource.playOnAwake = false;
        }

        public void PlayEffect(SoundType type)
        {
            AudioClip clip = GetClipByType(type);
            if (clip != null)
            {
                effectSource.PlayOneShot(clip);
            }
        }

        public void SetRunningSoundActive(bool active)
        {
            if (active && !runningSource.isPlaying)
            {
                runningSource.Play();
            }
            else if (!active && runningSource.isPlaying)
            {
                runningSource.Stop();
            }
        }

        public void PlayMusic(SoundType sound)
        {
            var clip = GetClipByType(sound);
            if (clip == null) return;

            soundMusic.clip = clip;
            soundMusic.Play();
        }

        private AudioClip GetClipByType(SoundType type)
        {
            return type switch
            {
                SoundType.Music => soundData.Music,
                SoundType.ButtonClick => soundData.ButtonClick,
                SoundType.Jump => soundData.JumpSound,
                SoundType.Swipe => soundData.SwipeSound,
                SoundType.Slide => soundData.SlideSound,
                SoundType.CoinCollect => soundData.CoinCollect,
                SoundType.Powerup => soundData.Powerup,
                SoundType.Powerdown => soundData.Powerdown,
                SoundType.ObstacleHit => soundData.ObstacleHit,
                _ => null
            };
        }
    }

    public enum SoundType
    {
        ButtonClick,
        Running,
        Jump,
        Swipe,
        Slide,
        CoinCollect,
        Powerup,
        Powerdown,
        ObstacleHit,
        Music
    }
}