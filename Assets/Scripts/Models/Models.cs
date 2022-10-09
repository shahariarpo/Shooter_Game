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

        [Header("Jumping")]
        public float jumpingHeight;
        public float jumpingFalloff;

    }

    #endregion

}