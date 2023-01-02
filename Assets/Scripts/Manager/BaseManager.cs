using UnityEngine;

namespace ArkadiumTest.Manager
{
    public abstract class BaseManager : MonoBehaviour
    {
        internal static BaseManager Instance => _instance;
        private static BaseManager _instance;

        [SerializeField]
        private AudioSource _mainAudio;

        protected virtual void Awake()
        {
            if (_instance != null && !_instance.Equals(null))
            {
                Debug.LogError("More than one BaseManager instances", _instance);
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void SetMainAudioVolume(float value) => _mainAudio.volume = value;

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
