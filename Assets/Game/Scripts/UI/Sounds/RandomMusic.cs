using UnityEngine;

namespace XposeCraft.UI.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomMusic : MonoBehaviour
    {
        public AudioClip[] Music;
        public GameObject PlayEnabledCallback;
        private AudioSource _audioSource;
        private bool _isPaused;
        private bool _wasPlay;
        private bool _isPlay;

        public bool Play
        {
            get { return _isPlay; }
            set
            {
                _isPlay = value;
                if (value)
                {
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.Pause();
                }
                if (PlayEnabledCallback != null)
                {
                    PlayEnabledCallback.SetActive(Play);
                }
            }
        }

        public void TogglePlay()
        {
            Play = !Play;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _isPaused = !hasFocus;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _isPaused = pauseStatus;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            Play = _wasPlay = PlayerPrefs.GetInt("PlaySound", 1) == 1;
        }

        private void Update()
        {
            if (Play != _wasPlay)
            {
                _wasPlay = Play;
                PlayerPrefs.SetInt("PlaySound", Play ? 1 : 0);
            }
            if (Play && !_isPaused && !_audioSource.isPlaying)
            {
                Next();
            }
        }

        private void Next()
        {
            _audioSource.clip = Music[Random.Range(0, Music.Length)];
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
}
