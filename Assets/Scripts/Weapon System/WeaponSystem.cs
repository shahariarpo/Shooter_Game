using UnityEngine;
using static Models;


public class WeaponSystem : MonoBehaviour
{
    private PlayerMovement characterController;

    [Header("Sway Settings")]
    public WeaponSettingsModel settings;
    bool is_initialized;
    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;
    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;


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

        targetWeaponRotation.y += settings.swayAmount * (settings.swayXInverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += settings.swayAmount * (settings.swayYInverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.swayClampX, settings.swayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.swayClampY, settings.swayClampY);

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.swayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.swaySmoothing);

        transform.localRotation = Quaternion.Euler(newWeaponRotation);
    }

}
