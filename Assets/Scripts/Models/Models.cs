using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Models : MonoBehaviour
{
    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }

    [Serializable]

    public class PlayerSettingsModel
    {
        [Header ("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement Settings")]
        public float walkingForwardSpeed;
        public float walkingStrafeSpeed;
        public float walkingBackwardsSpeed;
        public float runningForwardSpeed;
        public float runningStrafeSpeed;
        public float movementSmoothing;

        [Header("Jumping")]
        public float jumpingHeight;
        public float jumpingFalloff;
        public float fallingSmooting;

        [Header("SpeedEffecter")]
        public float speedEffecter = 1f;
        public float crouchSpeedEffecter;
        public float proneSpeedEffecter;
        public float fallingSpeedEffecter;
    }

    [Serializable]

    public class CharacterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }

    #endregion

    #region - Weapons -

    [Serializable]
    public class WeaponSettingsModel
    {
        [Header("Sway")]
        public float swayAmount;
        public bool swayYInverted;
        public bool swayXInverted;
        public float swaySmoothing;
        public float swayResetSmoothing;
        public float swayClampX;
        public float swayClampY;

    }





    #endregion

}