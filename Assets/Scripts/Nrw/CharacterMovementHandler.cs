
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnRequested = false;

    NetworkCharacterControllerCustom networkCharacterControllerCustom;
    HPHandler hpHandler;

    private void Awake()
    {
        networkCharacterControllerCustom = GetComponent<NetworkCharacterControllerCustom>();
        hpHandler = GetComponent<HPHandler>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            if (isRespawnRequested)
            {
                Respawn();
                return;
            }

            if (hpHandler.isDead)
                    return;
        }


        if (GetInput(out NetworkInputData networkInputData))
            {
                // Rotate the transform according to the client aim vector
                transform.forward = networkInputData.aimForwardVector;

                Quaternion rotation = transform.rotation;
                rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
                transform.rotation = rotation;

                // Move
                Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
                moveDirection.Normalize();

                networkCharacterControllerCustom.Move(moveDirection);

                // Jump
                if (networkInputData.isJumpPressed)
                {
                    networkCharacterControllerCustom.Jump();
                }

                // Check if we've fallen off the world
                CheckFallRespawn();
            }
    }

    private void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasInputAuthority)
            {
                Respawn();
            }
        }
    }

    public void RequestRespawn()
    {
        isRespawnRequested = true;
    }

    private void Respawn()
    {
        networkCharacterControllerCustom.Teleport(Utils.GetRandomSpawnPoint());
        hpHandler.OnRespawned();
        isRespawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerCustom.enabled = isEnabled;
    }
}
