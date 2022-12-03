using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ArkadiumTest.Manager.SystemManagerRequest;

namespace ArkadiumTest.Manager
{
    public class SystemManager : MonoBehaviour
    {
        internal static SystemManager Instance => _instance;
        private static SystemManager _instance;

        [SerializeField]
        private LoadingScreen _loadingScreen;

        private enum State : byte
        {
            None = 0,
            MainMenu = 1,
            Game = 2,
            Loading = 3
        }

        private State state
        {
            set
            {
                _previousState = _state;
                _state = value;
            }
        }
        private State _state = State.None;
        private State _previousState = State.None;

        private SystemManagerRequest _processing = None;

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
            state = State.Loading;
            _processing = GoToMainMenu;

            Queue<ILoadProcess> loadProcesses = new Queue<ILoadProcess>();
            loadProcesses.Enqueue(new LoadSceneProcess(1, true));

            _loadingScreen.Show(loadProcesses, OnRequestProcessed, 0);
        }

        internal bool ReceiveRequest(SystemManagerRequest request)
        {
            if (_processing != None)
                return false;

            _processing = request;

            switch (request)
            {
                case GoToMainMenu:
                    if (_state == State.MainMenu)
                        return false;

                    state = State.Loading;

                    Queue<ILoadProcess> goToMainMenu = new Queue<ILoadProcess>();
                    goToMainMenu.Enqueue(new UnloadSceneProcess((int)_previousState));
                    goToMainMenu.Enqueue(new LoadSceneProcess(1, true));
                    _loadingScreen.Show(goToMainMenu, OnRequestProcessed);
                    return true;
                case GoToGame:
                    state = State.Loading;

                    Queue<ILoadProcess> goToGame = new Queue<ILoadProcess>();
                    goToGame.Enqueue(new UnloadSceneProcess((int)_previousState));
                    goToGame.Enqueue(new LoadSceneProcess(2, true));
                    _loadingScreen.Show(goToGame, OnRequestProcessed);
                    return true;
                case None:
                default:
                    Debug.LogWarning("None");
                    return false;
            }
        }


        private void OnRequestProcessed()
        {
            SystemManagerRequest request = _processing;
            _processing = None;

            switch (request)
            {
                case GoToMainMenu:
                    state = State.MainMenu;
                    break;
                case GoToGame:
                    state = State.Game;
                    StartGame();
                    break;
                case None:
                default:
                    Debug.LogWarning("None");
                    break;
            }
        }

        private void StartGame()
        {
            Debug.LogWarning("StartGame");
        }

        private class LoadSceneProcess : ILoadProcess
        {
            public float Progress => _handle?.progress ?? 0;

            private AsyncOperation _handle;

            private readonly int _sceneIndex;
            private readonly bool _allowSceneActivation;

            private bool _started;
            private bool _completed;

            internal LoadSceneProcess(int sceneIndex, bool allowSceneActivation)
            {
                _sceneIndex = sceneIndex;
                _allowSceneActivation = allowSceneActivation;
            }

            public IEnumerator StartProcess()
            {
                if (_started)
                    yield break;

                _started = true;

                _handle = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Additive);
                _handle.allowSceneActivation = _allowSceneActivation;
                _handle.completed += OnCompleted;

                while (!_completed)
                    yield return null;
            }

            private void OnCompleted(AsyncOperation handle)
            {
                handle.completed -= OnCompleted;
                _completed = true;
            }
        }

        private class UnloadSceneProcess : ILoadProcess
        {
            public float Progress => _handle?.progress ?? 0;

            private AsyncOperation _handle;

            private readonly int _sceneIndex;

            private bool _started;
            private bool _completed;

            internal UnloadSceneProcess(int sceneIndex)
            {
                _sceneIndex = sceneIndex;
            }

            public IEnumerator StartProcess()
            {
                if (_started)
                    yield break;

                _started = true;

                _handle = SceneManager.UnloadSceneAsync(_sceneIndex);
                _handle.completed += OnCompleted;

                while (!_completed)
                    yield return null;
            }

            private void OnCompleted(AsyncOperation handle)
            {
                handle.completed -= OnCompleted;
                _completed = true;
            }
        }
    }

    internal enum SystemManagerRequest : byte
    {
        None = 0,
        GoToMainMenu = 1,
        GoToGame = 2
    }
}