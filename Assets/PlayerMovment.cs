using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerMovment : NetworkBehaviour
{
    [SerializeField] CharacterController ch;
    public float playerSpeed;

    public override void FixedUpdateNetwork()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(HorizontalInput, 0, VerticalInput) * playerSpeed * Runner.DeltaTime;
        ch.Move(movement);

        if (movement != Vector3.zero)
        {
            gameObject.transform.forward = movement;
        }
    }
}
