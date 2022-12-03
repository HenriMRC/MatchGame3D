using UnityEngine;

namespace ArkadiumTest.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        public void PlayButton()
        {
            SystemManager.Instance.ReceiveRequest(SystemManagerRequest.GoToGame);
        }
    }
}
