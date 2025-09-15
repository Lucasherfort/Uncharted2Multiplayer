using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Collections;
using Fusion.Editor;
using Mono.Cecil.Cil;
using System;

public class HPHandler : NetworkBehaviour
{
    [Networked]
    byte HP { get; set; }

    [Networked]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    public Color uiOnHitColor;
    public Image uiOnHitImage;
    public MeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    public GameObject playerModel;
    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;

    ChangeDetector changeDetector;

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
    }

    private void Start()
    {
        HP = startingHP;
        isDead = false;

        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnTakeDamage();
        }
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HP):
                    var byteReader = GetPropertyReader<Byte>(nameof(HP));
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
    public void OnTakeDamage()
    {
        if (isDead)
            return;

        HP -= 1;

        Debug.Log("Vie : " + HP);

        // Player died
        if (HP <= 0)
        {
            Debug.Log("Je suis mort");

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

        Debug.Log("OOOOOKKKKKKKK");
        StartCoroutine(OnHitCO());
    }

    void OnStateChanged(bool previous, bool current)
    {
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
