using ArkadiumTest.Game;
using System.Collections;
using UnityEngine;

namespace ArkadiumTest.Manager
{
    public class GameManager : BaseManager
    {
        [SerializeField]
        private GameUI _gameUI;

        [SerializeField]
        private Grid3D _grid;

        [SerializeField]
        private float _time = 300;

        [SerializeField]
        private float _multiplierInterval = 3;
        private float _lastScoreTime = float.PositiveInfinity;
        private int _multiplier = 1;

        [SerializeField]
        private int _scorePerMatch = 100;
        private int _score = 0;

        private Coroutine _coroutine;

        protected override void Awake()
        {
            base.Awake();

            _gameUI.UpdateUITimer(_time);
            _gameUI.UpdateUIScore(_score, _multiplier);
            _gameUI.OnUnpaused += Unpause;
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

        public void Pause()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _grid.Play(false);
            _gameUI.Pause();
        }

        private void Unpause()
        {
            if (_coroutine == null)
                _coroutine = StartCoroutine(GameUpdate());

            _grid.Play(true);
        }

        private void OnScore()
        {
            if (_lastScoreTime - _time > _multiplierInterval)
                _multiplier = 1;
            else
                _multiplier++;

            _lastScoreTime = _time;

            _score += _multiplier * _scorePerMatch;
            _gameUI.UpdateUIScore(_score, _multiplier);
        }

        private void OnWin()
        {
            StopCoroutine(_coroutine);
            _grid.Play(false);
            _gameUI.GameEnded(true, _score);
        }

        private void OnLose()
        {
            _grid.Play(false);
            _gameUI.GameEnded(false, _score);
        }

        public void PlayAgain()
        {
            SystemManager.Instance.ReceiveRequest(SystemManagerRequest.GoToGame);
        }

        public void BackToMainMenu()
        {
            SystemManager.Instance.ReceiveRequest(SystemManagerRequest.GoToMainMenu);
        }
    }
}