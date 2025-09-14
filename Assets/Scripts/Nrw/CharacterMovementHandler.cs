using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class CharacterMovementHandler : NetworkBehaviour
{
    Vector2 viewInput;

    // Rotation
    float cameraRotationX = 0;
    NetworkCharacterControllerCustom networkCharacterControllerCustom;
    Camera localCamera;

    private void Awake()
    {
        networkCharacterControllerCustom = GetComponent<NetworkCharacterControllerCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }

    public void Update()
    {
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Rotate the view
            networkCharacterControllerCustom.Rotate(networkInputData.rotationInput);

            // Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerCustom.Move(moveDirection);

            // Jump
            if (networkInputData.isJumpPressed)
            {
                networkCharacterControllerCustom.Jump();
            }
        }
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
