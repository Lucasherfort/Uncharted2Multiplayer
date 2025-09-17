
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnRequested = false;

    NetworkCharacterController networkCharacterController;
    HPHandler hpHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkCharacterController = GetComponent<NetworkCharacterController>();
        hpHandler = GetComponent<HPHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>(); 
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
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

                networkCharacterController.Move(moveDirection);

                // Jump
                if (networkInputData.isJumpPressed)
                {
                    networkCharacterController.Jump();
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
                networkInGameMessages.SendInGameRPCMessage(networkPlayer.nickName.ToString(), "fell off the world");

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
        networkCharacterController.Teleport(Utils.GetRandomSpawnPoint());
        hpHandler.OnRespawned();
        isRespawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterController.enabled = isEnabled;
    }
}
