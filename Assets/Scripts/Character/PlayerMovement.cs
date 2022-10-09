using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private DefaultInput defaultInput;
    public Vector2 input_Movement;
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;

    [Header("References")]
    public Transform cameraHolder;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin;
    public float viewClampYMax;

    [Header("Gravity")]
    public float gravityAmount;
    public float playerGravity;
    public float gravityMin;
    public Vector3 jumpingForce;
    private Vector3 jumpForceVelocity;


    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();

        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CalculateView();
        CalculateMovements();
        CalculateJump();
    }

    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -input_View.x : input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.ViewXSensitivity * (playerSettings.ViewYInverted ? input_View.y: -input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);


        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovements()
    {
        var verticalSpeed = playerSettings.walkingForwardSpeed * input_Movement.y * Time.deltaTime;
        var horizontalSpeed = playerSettings.walkingStrafeSpeed * input_Movement.x * Time.deltaTime;


        var newMovementSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);

        newMovementSpeed = cameraHolder.TransformDirection(newMovementSpeed);

        playerGravity -= gravityAmount * Time.deltaTime;

        if(playerGravity > gravityMin && jumpingForce.y > 0.1f)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }


        if(playerGravity < -1 && characterController.isGrounded)
        {
            playerGravity = -1;
        }

        if(jumpingForce.y > 0.1f)
        {
            playerGravity = 0;
        }

        newMovementSpeed.y = playerGravity;
        newMovementSpeed += jumpingForce * Time.deltaTime;

        characterController.Move(newMovementSpeed);

    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpForceVelocity, playerSettings.jumpingFalloff);
    }

    private void Jump()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        jumpingForce = Vector3.up * playerSettings.jumpingHeight;

    }
}
