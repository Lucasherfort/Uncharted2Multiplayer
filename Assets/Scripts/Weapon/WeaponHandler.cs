using System.Collections;
using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [Networked]
    public bool isFiring { get; set; }

    ChangeDetector changeDetector;

    public ParticleSystem fireParticlesSystem;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    float lastTimeFired = 0;
    HPHandler hPHandler;

    private void Awake()
    {
        hPHandler = GetComponent<HPHandler>();
    }

    public override void FixedUpdateNetwork()
    {
        if (hPHandler.isDead)
            return;

        if (GetInput(out NetworkInputData networkInputData))
            {
                if (networkInputData.isFireButtonPressed)
                    Fire(networkInputData.aimForwardVector);
            }
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(isFiring):
                    var boolRender = GetPropertyReader<bool>(nameof(isFiring));
                    var (previousBool, curreentBool) = boolRender.Read(previousBuffer, currentBuffer);
                    OnFireChanged(previousBool, curreentBool);
                    break;
            }
        }
    }

    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < 0.15f)
            return;

        StartCoroutine(FireEffectCO());    

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitinfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitinfo.Distance > 0)
            hitDistance = hitinfo.Distance;

        if (hitinfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitinfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority)
            {
                Debug.Log($"{Time.time} {transform.name} call OnTakeDamage {hitinfo.Hitbox.transform.root.name}");
                hitinfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage();
            }

            isHitOtherPlayer = true;

        }
        else if (hitinfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit collider {hitinfo.Collider.transform.name}");
        }

        if (isHitOtherPlayer)
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        else Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

        lastTimeFired = Time.time;
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;
        //fireParticlesSystem.Play();
        yield return new WaitForSeconds(0.009f);
        isFiring = false;
    }

    void OnFireChanged(bool previous, bool current)
    {
        if (current && !previous)
            OnFireRemote();
    }

    void OnFireRemote()
    {
        /*
        if (!Object.HasInputAuthority)
            fireParticlesSystem.Play();
            */
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
}
