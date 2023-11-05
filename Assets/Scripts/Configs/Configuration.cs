using UnityEngine;

namespace ArkadiumTest.Configurations
{
    [CreateAssetMenu(fileName = "Configuration", menuName = "Configuration", order = 1)]
    public class Configuration : ScriptableObject
    {
        public static Configuration Instance => _instance;
        private static Configuration _instance;

        public void Initialize()
        {
            if (_instance != null)
            {
                Destroy(this);
                throw new System.Exception("Already have an instance!");
            }

            _instance = this;

            int paletteIndex = PlayerPrefs.GetInt(PLAYER_PREFS_PALETTE);
            _palette = _palettes[paletteIndex];
        }

        private const string PLAYER_PREFS_PALETTE = "Palette";
        public Palette Palette => _palette;
        private Palette _palette;
        [SerializeField]
        private Palette[] _palettes;
    }
}