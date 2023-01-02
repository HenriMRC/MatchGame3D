using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace ArkadiumTest.Manager
{
    public class AudioManager : MonoBehaviour
    {
        internal static AudioManager Instance => _instance;
        private static AudioManager _instance;

        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private TMP_Text _mute;

        internal float MasterVolume
        {
            get
            {
                return GetFloat("Master");
            }
            set
            {
                SetFloat("Master", value);
            }
        }
        
        internal float MasterSubVolume
        {
            get
            {
                return GetFloat("MasterSub");
            }
            set
            {
                SetFloat("MasterSub", value);
            }
        }

        internal float MusicVolume
        {
            get
            {
                return GetFloat("Music");
            }
            set
            {
                SetFloat("Music", value);
            }
        }

        internal float SoundsVolume
        {
            get
            {
                return GetFloat("Sounds");
            }
            set
            {
                SetFloat("Sounds", value);
            }
        }

        private void Awake()
        {
            if (_instance != null && !_instance.Equals(null))
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Start()
        {
            UpdateMuteButton();
        }

        private float GetFloat(string name)
        {
            if (_audioMixer.GetFloat(name, out float value))
            {
                value += 80;
                value /= 80;
                return value;
            }
            else
            {
                Debug.LogError($"{name} not found");
                return 1;
            }
        }

        private void SetFloat(string name, float value)
        {
            value *= 80;
            value -= 80;

            if (!_audioMixer.SetFloat(name, value))
                Debug.LogError($"{name} not found");
        }

        public void Mute()
        {
            MasterSubVolume = 1 - MasterSubVolume;
            UpdateMuteButton();
        }

        private void UpdateMuteButton()
        {
            _mute.text = MasterSubVolume == 0 ? "Muted" : "Unmuted";
        }
    }
}
