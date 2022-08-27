using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
    public Button menu;
    private void Awake()
    {
        menu.onClick.AddListener(() => SceneManager.LoadScene(1));
        menu.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
    }
}