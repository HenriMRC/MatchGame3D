using TMPro;
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
            _processing = GoToGame;
            _loadingScreen.Show(DoLoadMainMenu, 0);
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
                    _loadingScreen.Show(LoadMainMenu);
                    return true;
                case GoToGame:
                    if (_state == State.Game)
                        return false;

                    state = State.Loading;
                    _loadingScreen.Show(LoadGame);
                    return true;
                case None:
                default:
                    Debug.LogWarning("None");
                    return false;
            }
        }

        private void LoadMainMenu()
        {
            AsyncOperation handle = SceneManager.UnloadSceneAsync((int)_previousState);
            handle.completed += DoLoadMainMenu;

            _loadingScreen.StartProcess(() => handle.progress / 2);
        }

        private void DoLoadMainMenu() => DoLoadMainMenu(null);
        private void DoLoadMainMenu(AsyncOperation handle)
        {
            _loadingScreen.EndProcess();

            handle = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            handle.allowSceneActivation = true;
            handle.completed += MainMenuLoaded;

            _loadingScreen.StartProcess(() => 0.5f + handle.progress / 2);
        }

        private void MainMenuLoaded(AsyncOperation handle)
        {
            _loadingScreen.EndProcess();
            _loadingScreen.Hide(null);
            _processing = None;
            state = State.MainMenu;
        }

        private void LoadGame()
        {
            AsyncOperation handle = SceneManager.UnloadSceneAsync((int)_previousState);
            handle.completed += DoLoadGame;

            _loadingScreen.StartProcess(() => handle.progress / 2);
        }

        private void DoLoadGame(AsyncOperation handle)
        {
            _loadingScreen.EndProcess();

            handle = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
            handle.allowSceneActivation = true;
            handle.completed += GameLoaded;

            _loadingScreen.StartProcess(() => 0.5f + handle.progress / 2);
        }

        private void GameLoaded(AsyncOperation handle)
        {
            _loadingScreen.EndProcess();
            _loadingScreen.Hide(StartGame);
            _processing = None;
            state = State.Game;
        }

        private void StartGame()
        {
            Debug.LogWarning("StartGame");
        }
    }

    internal enum SystemManagerRequest : byte
    {
        None = 0,
        GoToMainMenu = 1,
        GoToGame = 2
    }
}