using System.Collections.Generic;
using UnityEngine;

namespace ArkadiumTest.Configurations
{
    [CreateAssetMenu(fileName = "Palette", menuName = "Palette", order = 1)]
    public class Palette : ScriptableObject
    {
        [SerializeField]
        private Color _backgroundColor;
        public Color BackgroundColor => _backgroundColor;

        [SerializeField]
        private Color _firstColor;
        public Color FirstColor => _firstColor;

        [SerializeField]
        private Color _secondColor;
        public Color SecondColor => _secondColor;

        [SerializeField]
        private Color _thirdColor;
        public Color ThirdColor => _thirdColor;

        [SerializeField]
        private Color[] _boardColors;
        public IReadOnlyList<Color> BoardColors => _boardColors;
    }
}