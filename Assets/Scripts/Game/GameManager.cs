using UnityEngine;

namespace ArkadiumTest.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameUI _gameUI;

        [SerializeField]
        private Grid3D _grid;

        [SerializeField]
        private float _time = 300;

        void Awake()
        {
            _gameUI.UpdateUITimer(_time);
            _grid.RegisterOnWin(OnWin);
        }

        private void Update()
        {
            _time -= Time.deltaTime;

            if (_time <= 0)
                OnLose();

            _time = Mathf.Max(0, _time);
            _gameUI.UpdateUITimer(_time);
        }

        private void OnWin()
        {
            Debug.Log("Victory");
        }

        private void OnLose()
        {
            Debug.Log("Defeat");
        }
    }
}
