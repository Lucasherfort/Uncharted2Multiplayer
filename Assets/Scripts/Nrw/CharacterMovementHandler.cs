
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerCustom networkCharacterControllerCustom;
    Camera localCamera;

    private void Awake()
    {
        networkCharacterControllerCustom = GetComponent<NetworkCharacterControllerCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }

    public override void FixedUpdateNetwork()
    {
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
            transform.position = Utils.GetRandomSpawnPoint();
        }
    }
}
