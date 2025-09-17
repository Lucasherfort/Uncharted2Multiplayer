using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{
    private InGameMessagesUIHandler inGameMessagesUIHandler;

    private void Start()
    {

    }

    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessages($"<b>{userNickName}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InGameMessages(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");

        if (inGameMessagesUIHandler == null)
        {
            inGameMessagesUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<InGameMessagesUIHandler>();
        }

        if (inGameMessagesUIHandler != null)
                inGameMessagesUIHandler.OnGameMessageReceived(message);
    }
}
