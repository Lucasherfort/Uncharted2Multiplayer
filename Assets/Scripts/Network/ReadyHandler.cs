using Fusion;

public class ReadyHandler : NetworkBehaviour
{
    private void StartGame()
    {
        Runner.SessionInfo.IsOpen = false;

        // Update scene for the network
        //Runner.SetActiveScene("World1");
    }
}
