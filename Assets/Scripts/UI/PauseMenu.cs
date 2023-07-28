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
    public AudioSource bgm;

    [Header("Audio Variables")]
    public TextMeshProUGUI musicValue;
    public TextMeshProUGUI sfxValue;

    [Header("Gameplay Tab")]
    public GameObject gameplayCanvas;
    public Image gameplayIcon;
    public Sprite gameplaySelected, gameplayNotSelected;
    public CanvasGroup gameplayOverlay;

    [Header("Mobile Tab")]
    public GameObject mobileCanvas;
    public Image mobileIcon;
    public Sprite mobileSelected, mobileNotSelected;
    private const int ARROWS = 0, TOUCH = 0, JOYSTICK = 1;

    private void Awake()
    {
        DisplayTab();

        //load settings
        if (bgm != null) bgm.volume = 0.2f * (PlayerPrefs.GetFloat("musicVolume") / 10f);
        if (gameplayOverlay != null) gameplayOverlay.alpha = 1f - (float) PlayerPrefs.GetInt("disableOverlay");

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

    //audio settings
    public void MusicValueUpdate(float sliderValue)
    {
        musicValue.text = "" + sliderValue;
        PlayerPrefs.SetFloat("musicVolume", sliderValue);
        if(bgm != null) bgm.volume = 0.2f * (sliderValue / 10f);
    }

    public void SFXValueUpdate(float sliderValue)
    {
        sfxValue.text = "" + sliderValue;
        PlayerPrefs.SetFloat("sfxVolume", sliderValue);
    }

    //gameplay settings
    public void InverseToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("inverseControls", toggleValue ? 1 : 0);
    }

    public void DisableOverlayToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("disableOverlay", toggleValue ? 1 : 0);
        if(gameplayOverlay != null) gameplayOverlay.alpha = toggleValue ? 0 : 1;
    }

    public void GlideToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("enableGlide", toggleValue ? 1 : 0);
    }
    public void ArrowToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("enableArrow", toggleValue ? 1 : 0);
    }

    //mobile settings
    public void MobileMovementToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("mobileMovement", toggleValue ? ARROWS : JOYSTICK);
    }
    public void MobileGrappleToggle(bool toggleValue)
    {
        PlayerPrefs.SetInt("mobileGrapple", toggleValue ? TOUCH : JOYSTICK);
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
}
