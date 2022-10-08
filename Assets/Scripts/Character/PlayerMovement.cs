using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Models;

public class PlayerMovement : MonoBehaviour
{
    private DefaultInput defaultInput;
    public Vector2 input_Movement;
    public Vector2 input_View;

    private Vector3 newCameraRotation;

    [Header("References")]
    public Transform cameraHolder;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float viewClampYMin;
    public float viewClampYMax;

    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => input_View = e.ReadValue<Vector2>();

        defaultInput.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovements();
    }

    private void CalculateView()
    {
        newCameraRotation.x += playerSettings.ViewXSensitivity * input_View.y * Time.deltaTime;

        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYMin, viewClampYMax);



        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    }

    private void CalculateMovements()
    {

    }
}
