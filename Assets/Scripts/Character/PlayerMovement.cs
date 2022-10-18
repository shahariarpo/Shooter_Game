using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerMovement : MonoBehaviour
{
    #region - Variables and References -

    private CharacterController characterController;
    private DefaultInput defaultInput;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    [HideInInspector]
    public bool isSprinting;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;


    [Header("References")]
    public Transform cameraHolder;
    public Transform feetTransform;


    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin;
    public float viewClampYMax;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float playerGravity;
    public float gravityMin;

    public Vector3 jumpingForce;
    private Vector3 jumpForceVelocity;


    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    private float cameraHeight;
    private float cameraHeightVelocity;
    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    private float stanceCheckErrorMargin = 0.05f;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    [Header("Weapon")]
    public WeaponSystem currentWeapon;

    public float weaponAnimSpeed;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    #endregion

    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crouch.performed += e => Crouch();
        defaultInput.Character.Prone.performed += e => Prone();
        defaultInput.Character.Sprint.performed += e => ToggleSprint();
        defaultInput.Character.SprintReleased.performed += e => StopSprint();
        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = cameraHolder.localPosition.y;

        if (currentWeapon)
        {
            currentWeapon.Initialize(this);
        }
    }

    private void Update()
    {
        SetIsGrounded();
        SetIsFalling();

        CalculateView();
        CalculateMovements();
        CalculateJump();
        CalculateStance();
    }

    #region - Is Falling / Is Grounded - 

    private void SetIsGrounded()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);
    }

    private void SetIsFalling()
    {
        isFalling = (!isGrounded && characterController.velocity.magnitude > playerSettings.isFallingSpeed);
    }

    #endregion

    #region - Player Movement -

    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.ViewXSensitivity * (playerSettings.ViewXInverted ? -input_View.x : input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.ViewXSensitivity * (playerSettings.ViewYInverted ? input_View.y : -input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);


        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovements()
    {

        if (input_Movement.y <= 0.2f)
        {
            isSprinting = false;
        }

        var verticalSpeed = playerSettings.walkingForwardSpeed;
        var horizontalSpeed = playerSettings.walkingStrafeSpeed;

        if (isSprinting)
        {
            verticalSpeed = playerSettings.runningForwardSpeed;
            horizontalSpeed = playerSettings.runningStrafeSpeed;
        }


        if (!isGrounded)
        {
            playerSettings.speedEffecter = playerSettings.fallingSpeedEffecter;
        }
        else if (playerStance == PlayerStance.Crouch)
        {
            playerSettings.speedEffecter = playerSettings.crouchSpeedEffecter;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            playerSettings.speedEffecter = playerSettings.proneSpeedEffecter;
        }
        else
        {
            playerSettings.speedEffecter = 1;
        }

        weaponAnimSpeed = characterController.velocity.magnitude / (playerSettings.walkingForwardSpeed) * playerSettings.speedEffecter;

        if (weaponAnimSpeed > 1)
        {
            weaponAnimSpeed = 1;
        }


        verticalSpeed *= playerSettings.speedEffecter;
        horizontalSpeed *= playerSettings.speedEffecter;



        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_Movement.x * Time.deltaTime, 0, verticalSpeed * input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playerSettings.movementSmoothing : playerSettings.fallingSmooting);
        var MovementSpeed = transform.TransformDirection(newMovementSpeed);

        playerGravity -= gravityAmount * Time.deltaTime;

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }


        if (playerGravity < -0.01f && isGrounded)
        {
            playerGravity = -0.01f;
        }

        MovementSpeed.y = playerGravity;
        MovementSpeed += jumpingForce * Time.deltaTime;

        characterController.Move(MovementSpeed);

    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpForceVelocity, playerSettings.jumpingFalloff);
    }

    private void ToggleSprint()
    {
        if (input_Movement.y <= 0.2f)
        {
            isSprinting = false;
            return;
        }

        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch || playerStance == PlayerStance.Prone)
        {
            isSprinting = false;
            return;
        }

        isSprinting = !isSprinting;
    }

    private void StopSprint()
    {
        isSprinting = false;
    }

    #endregion

    #region - Player Stance - 
    private void CalculateStance()
    {

        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }


        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);

    }

    private void Jump()
    {
        if (!isGrounded || playerStance == PlayerStance.Prone)
        {
            return;
        }

        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }
        jumpingForce = Vector3.up * playerSettings.jumpingHeight;
        playerGravity = 0;
        currentWeapon.TriggerJump();

    }

    private void Crouch()
    {
        if(playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;

        }

        if (StanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }

        playerStance = PlayerStance.Crouch;
    }

    private void Prone()
    {
        playerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        var Start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var End = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);



        return Physics.CheckCapsule(Start, End, characterController.radius, playerMask);
    }

    #endregion

    #region - Gizmos -

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(feetTransform.position, playerSettings.isGroundedRadius);
    }



    #endregion+

}
