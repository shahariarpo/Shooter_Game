using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Models : MonoBehaviour
{
    #region - Player -
    [Serializable]

    public class PlayerSettingsModel
    {
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted;
        public bool ViewYInverted;
    }

    #endregion

}