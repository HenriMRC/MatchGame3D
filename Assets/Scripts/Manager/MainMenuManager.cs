namespace ArkadiumTest.Manager
{
    public class MainMenuManager : BaseManager
    {
        public void PlayButton()
        {
            SystemManager.Instance.ReceiveRequest(SystemManagerRequest.GoToGame);
        }
    }
}