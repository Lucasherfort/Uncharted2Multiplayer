using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TMP_Text playerNickNameTM;
    public static NetworkPlayer Local { get; set; }

    [Networked]
    public NetworkString<_16> nickName { get; set; }
    bool isPublicJoinMessageSent = false;
    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;
    NetworkInGameMessages networkInGameMessages;

    ChangeDetector changeDetector;

    private void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    private void Start()
    {

    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(nickName):
                    OnNickNameChanged();
                    break;
            }
        }
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (Object.HasInputAuthority)
        {
            Local = this;

            Camera.main.gameObject.SetActive(false);

            // a changer quand on aura la BDD
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));

            //Debug.Log("Spawned local player");
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            localUI.SetActive(false);

            //Debug.Log("Spawned remote player");
        }

        // Set the player as player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"P_{Object.Id}";
        playerNickNameTM.text = nickName.ToString();
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetorkObject))
            {
                if (playerLeftNetorkObject == Object)
                    Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(playerLeftNetorkObject.GetComponent<NetworkPlayer>().nickName.ToString(), "left");  
            }           
        }

        if (player == Object.InputAuthority)
            {
                Runner.Despawn(Object);
            }
    }

    private void OnNickNameChanged()
    {
        //Debug.Log($"Nickname changed for player to {nickName} for player to {gameObject.name}");

        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        //Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameRPCMessage(nickName, "joined");

            isPublicJoinMessageSent = true;
        }
    }
}
