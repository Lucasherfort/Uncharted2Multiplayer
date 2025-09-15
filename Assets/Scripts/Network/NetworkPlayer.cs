using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TMP_Text playerNickNameTM;
    public static NetworkPlayer Local { get; set; }

    [Networked]
    public NetworkString<_16> nickName { get; set; }

    ChangeDetector changeDetector;

    void Start()
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

            // a changer quand on aura la BDD
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("Spawned local player");
        }
        else
        {
            Debug.Log("Spawned remote player");
        }

        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnNickNameChanged()
    {
        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        this.nickName = nickName;

    }
}
