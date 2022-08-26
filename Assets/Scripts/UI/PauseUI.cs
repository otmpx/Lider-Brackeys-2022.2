using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseScreen, settingsScreen;
    public Button resume, restart, settings, menu, back;
    public SettingsUI settingsUI;
    private void Awake()
    {
        resume.onClick.AddListener(() => LevelDirector.instance.PauseUnpause());
        resume.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        restart.onClick.AddListener(() => LevelDirector.instance.ReloadLevel());
        restart.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        settings.onClick.AddListener(() => LoadSettings());
        settings.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        menu.onClick.AddListener(() => QuitToMenu());
        menu.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
        back.onClick.AddListener(() => LoadPause());
        back.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonPressSound));
    }
    void LoadSettings()
    {
        settingsUI.LoadSettings();
        settingsScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }
    void QuitToMenu()
    {
        Time.timeScale = 1;
        LevelDirector.instance.QuitToMenu();
    }
    public void LoadPause()
    {
        settingsScreen.SetActive(false);
        pauseScreen.SetActive(true);
    }
}
