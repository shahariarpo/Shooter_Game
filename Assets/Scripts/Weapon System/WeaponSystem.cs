using UnityEngine;
using static Models;


public class WeaponSystem : MonoBehaviour
{
    private PlayerMovement characterController;

    [Header("References")]
    public Animator weaponAnimator;



    [Header("Sway Settings")]
    public WeaponSettingsModel settings;
    bool is_initialized;
    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;
    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;
    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundedTrigger;
    private float fallingDelay;


    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
    }



    public void Initialize(PlayerMovement CharacterController)
    {
        characterController = CharacterController;
        is_initialized = true;
    }

    private void Update()
    {
        if (!is_initialized)
        {
            return;
        }

        CalculateWeaponRotation();
        SetWeaponAnimations();
    }

    public void TriggerJump()
    {
        isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }


    private void CalculateWeaponRotation()
    {

        targetWeaponRotation.y += settings.swayAmount * (settings.swayXInverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += settings.swayAmount * (settings.swayYInverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.swayClampX, settings.swayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.swayClampY, settings.swayClampY);
        targetWeaponRotation.z = -targetWeaponRotation.y;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.swayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.swaySmoothing);

        targetWeaponMovementRotation.z = settings.movementSwayX * (settings.movementSwayXInverted ? -characterController.input_Movement.x : characterController.input_Movement.x);
        targetWeaponMovementRotation.x = settings.movementSwayY * (settings.movementSwayYInverted ? -characterController.input_Movement.y : characterController.input_Movement.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, settings.swayResetSmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, settings.swaySmoothing);


        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    private void SetWeaponAnimations()
    {
        if (isGroundedTrigger)
        {
            fallingDelay = 0f;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (characterController.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f)
        {
            isGroundedTrigger = true;
            weaponAnimator.SetTrigger("Land");
        }

        else if (!characterController.isGrounded && isGroundedTrigger)
        {
            isGroundedTrigger = false;
            weaponAnimator.SetTrigger("Falling");
        }

        weaponAnimator.SetBool("is_Sprinting", characterController.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", characterController.weaponAnimSpeed);
    }
}
