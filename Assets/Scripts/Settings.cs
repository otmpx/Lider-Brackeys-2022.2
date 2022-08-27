using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;
public class Settings : MonoBehaviour
{
    public static float musicVol = 0.5f;
    public static float soundVol = 0.5f;
    public static float aimSensitivity = 3f;
    public static Settings instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
