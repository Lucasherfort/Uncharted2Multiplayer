using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Collections;
using System;

public class HPHandler : NetworkBehaviour
{
    [Networked]
    public byte HP { get; set; }

    [Networked]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    public Color uiOnHitColor;
    public Image uiOnHitImage;
    public MeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    public GameObject playerModel;

    public bool skipSettingsStartValues = false;

    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    ChangeDetector changeDetector;

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>(); 
    }

    private void Start()
    {
        if (!skipSettingsStartValues)
        {
            HP = startingHP;
            isDead = false;   
        }

        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HP):
                    var byteReader = GetPropertyReader<byte>(nameof(HP));
                    var (previousByte, currentByte) = byteReader.Read(previousBuffer, currentBuffer);
                    OnHPChanged(previousByte, currentByte);
                    break;
                case nameof(isDead):
                    var boolReader = GetPropertyReader<bool>(nameof(isDead));
                    var (previousBool, currentBool) = boolReader.Read(previousBuffer, currentBuffer);
                    OnStateChanged(previousBool, currentBool);
                    break;
            }
        }
    }
    
    IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
        {
            uiOnHitImage.color = uiOnHitColor;
        }

        yield return new WaitForSeconds(0.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;

        if (Object.HasInputAuthority && !isDead)
        {
            uiOnHitImage.color = new Color(0, 0, 0, 0);
        }
    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        characterMovementHandler.RequestRespawn();
    }

    // Only called on the server
    public void OnTakeDamage(string damageCauseByPlayerNickname)
    {
        if (isDead)
            return;

        HP -= 1;

        // Player died
        if (HP <= 0)
        {
            networkInGameMessages.SendInGameRPCMessage(damageCauseByPlayerNickname, $"killed <b>{networkPlayer.nickName.ToString()}</b>");
            StartCoroutine(ServerReviveCO());
            isDead = true;
        }
    }

    void OnHPChanged(byte previous, byte current)
    {
        if (current < previous)
            OnHPReduced();
    }

    private void OnHPReduced()
    {
        if (!isInitialized)
            return;

        StartCoroutine(OnHitCO());
    }

    void OnStateChanged(bool previous, bool current)
    {
        Debug.Log(previous);
        Debug.Log(current);        

        if (current)
        {
            OnDeath();
        }
        else if (!current && previous)
        {
            OnRevive();
        }
    }

    private void OnDeath()
    {
        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        // Animation de mort 
    }

    private void OnRevive()
    {
        if (Object.HasInputAuthority)
        {
            uiOnHitImage.color = new Color(0, 0, 0, 0);
        }

        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);

    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
}
