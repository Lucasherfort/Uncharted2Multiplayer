using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    LocalCameraHandler localCameraHandler;

    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // view input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = -Input.GetAxis("Mouse Y");

        // move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
            isJumpButtonPressed = true;

        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        // View Dara
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        // Move data
        networkInputData.movementInput = moveInputVector;

        // Jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;

        // Reset varaibles now that we have read their stats
        isJumpButtonPressed = false;

        return networkInputData;
    }
}
