using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    enum SettingsTab {Audio, Gameplay, Mobile};
    SettingsTab currentTab = SettingsTab.Audio;

    [Header("Pause Function")]
    public bool gamePaused = false;
    public GameObject pauseCanvas;

    [Header("Mobile/Desktop")]
    public GameObject desktopTabs, mobileTabs;

    [Header("Audio Tab")]
    public GameObject audioCanvas;
    public Image audioIcon;
    public Sprite audioSelected, audioNotSelected;

    [Header("Audio Variables")]
    public TextMeshProUGUI musicValue;
    public TextMeshProUGUI sfxValue;

    [Header("Gameplay Tab")]
    public GameObject gameplayCanvas;
    public Image gameplayIcon;
    public Sprite gameplaySelected, gameplayNotSelected;

    [Header("Mobile Tab")]
    public GameObject mobileCanvas;
    public Image mobileIcon;
    public Sprite mobileSelected, mobileNotSelected;

    //[Header("Audio")]
    //public GameObject musicSlider, sfxSlider;

    private void Awake()
    {
        DisplayTab();

        if (Input.touchSupported)
        {
            Debug.Log("Touch Supported " + mobileTabs.transform.GetChild(0).GetChildren().Count);
            desktopTabs.SetActive(false);
            mobileTabs.SetActive(true);

            audioIcon = mobileTabs.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            gameplayIcon = mobileTabs.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            mobileIcon = mobileTabs.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        }
        else
        {
            Debug.Log("Touch Not Supported " + desktopTabs.transform.GetChild(0).GetChildren().Count);
            desktopTabs.SetActive(true);
            mobileTabs.SetActive(false);

            audioIcon = desktopTabs.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            gameplayIcon = desktopTabs.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        }
    }

    public void PauseInput()
    {
        if (gamePaused) ResumeGame();
        else PauseGame();
    }

    public void ResumeGame()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void PauseGame()
    {
        currentTab = SettingsTab.Audio;
        DisplayTab();

        pauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void MusicValueUpdate(float sliderValue)
    {
        musicValue.text = "" + sliderValue;
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
    }

    public void SFXValueUpdate(float sliderValue)
    {
        sfxValue.text = "" + sliderValue;
        PlayerPrefs.SetFloat("sfxVolume", sliderValue);
    }

    public void DisplayTab()
    {
        audioCanvas.SetActive(currentTab == SettingsTab.Audio);
        gameplayCanvas.SetActive(currentTab == SettingsTab.Gameplay);
        mobileCanvas.SetActive(currentTab == SettingsTab.Mobile);

        audioIcon.sprite = (currentTab == SettingsTab.Audio) ? audioSelected : audioNotSelected;
        gameplayIcon.sprite = (currentTab == SettingsTab.Gameplay) ? gameplaySelected : gameplayNotSelected;
        mobileIcon.sprite = (currentTab == SettingsTab.Mobile) ? mobileSelected : mobileNotSelected;
    }

    public void DisplayAudioTab()
    {
        currentTab = SettingsTab.Audio;
        DisplayTab();
    }

    public void DisplayGameplayTab()
    {
        currentTab = SettingsTab.Gameplay;
        DisplayTab();
    }
    public void DisplayMobileTab()
    {
        currentTab = SettingsTab.Mobile;
        DisplayTab();
    }
    public void ResetPosition()
    {
        //reset position functionality here
    }

    //add button functions
    //playerprefs for settings
}
