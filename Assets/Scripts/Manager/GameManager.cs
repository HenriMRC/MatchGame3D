using ArkadiumTest.Game;
using System.Collections;
using UnityEngine;

namespace ArkadiumTest.Manager
{
    public class GameManager : MonoBehaviour
    {
        internal static GameManager Instance => _instance;
        private static GameManager _instance;

        [SerializeField]
        private GameUI _gameUI;

        [SerializeField]
        private Grid3D _grid;

        [SerializeField]
        private float _time = 300;

        [SerializeField]
        private int _scorePerMatch = 100;
        private int _score = 0;

        private Coroutine _coroutine;

        void Awake()
        {
            if (_instance != null && !_instance.Equals(null))
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _gameUI.UpdateUITimer(_time);
            _gameUI.UpdateUITimer(_score);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            else
                Debug.LogError("More than one GameManager instances", _instance);
        }

        public void StartGame()
        {
            if (_coroutine == null)
                _coroutine = StartCoroutine(GameUpdate());

            _grid.StartGame(OnScore, OnWin);
        }

        private IEnumerator GameUpdate()
        {
            while (_time > 0)
            {
                yield return null;

                _time -= Time.deltaTime;

                _time = Mathf.Max(0, _time);
                _gameUI.UpdateUITimer(_time);
            }

            OnLose();
        }

        private void OnScore()
        {
            _score += _scorePerMatch;
            _gameUI.UpdateUIScore(_score);
        }

        private void OnWin()
        {
            Debug.Log("Victory");
            StopCoroutine(_coroutine);
            _grid.Stop();
        }

        private void OnLose()
        {
            Debug.Log("Defeat");
            _grid.Stop();
        }
    }
}
