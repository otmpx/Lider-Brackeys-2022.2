using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public LevelDirector levelDirectorPrefab;
    public GameObject startScreen, settingsScreen;
    public SettingsUI settingsUI;
    public Button start, settings, back, quit;
    private void Awake()
    {
        start.onClick.AddListener(() => SceneManager.LoadScene(2));
        start.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        settings.onClick.AddListener(() => LoadSettings());
        settings.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        back.onClick.AddListener(() => LoadStart());
        back.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        quit.onClick.AddListener(() => Application.Quit());
        quit.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
    }
    void LoadStart()
    {
        startScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }
    void LoadSettings()
    {
        settingsScreen.SetActive(true);
        startScreen.SetActive(false);
        settingsUI.LoadSettings();
    }
}
