using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject levelSelectObject;
    [SerializeField] private GameObject settingsObject;

    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityText;

    [SerializeField] private AudioMixer masterVolume;

    private Resolution[] resolutions;

    private void Start()
    {
        Main();
        SetDefaults();
    }


    private void SetDefaults()
    {

        int quality = PlayerPrefs.GetInt("Quality", 1);
        SetQuality(quality);
        qualityDropdown.value = quality;

        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = fullscreen;
        Fullscreen(fullscreen);

        float master = PlayerPrefs.GetFloat("MasterVolume", 1);
        MasterVolume(master);
        masterVolumeSlider.value = master;

        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);
        SetSensitivity(sensitivity);
        sensitivitySlider.value = sensitivity;

        //Res

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        string currentResolution = PlayerPrefs.GetString("Resolution", "null");

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;

            options.Add(option);

            if (currentResolution == "null" && resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = option;
            }
        }

        resolutionDropdown.AddOptions(options);

        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text == currentResolution)
            {
                resolutionDropdown.value = i;
                //SetResolution(i);
                break;
            }
        }

        resolutionDropdown.RefreshShownValue();
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteKey("Resolution");
        PlayerPrefs.DeleteKey("Sensitivity");
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("Fullscreen");
        PlayerPrefs.DeleteKey("Quality");


        SetDefaults();
    }

    public void Main()
    {
        mainMenuObject.SetActive(true);

        levelSelectObject.SetActive(false);
        settingsObject.SetActive(false);


        /* levelSelectObject.GetComponent<UITween>().ShutWindow();
         creditsObject.GetComponent<UITween>().ShutWindow();
         settingsObject.GetComponent<UITween>().ShutWindow();*/
    }

    public void LevelSelect()
    {
        levelSelectObject.SetActive(true);

        mainMenuObject.SetActive(false);
        settingsObject.SetActive(false);

        /* mainMenuObject.GetComponent<UITween>().ShutWindow();
         creditsObject.GetComponent<UITween>().ShutWindow();
         settingsObject.GetComponent<UITween>().ShutWindow();*/
    }

    public void Settings()
    {
        settingsObject.SetActive(true);
        levelSelectObject.SetActive(false);
        mainMenuObject.SetActive(false);


        /* mainMenuObject.GetComponent<UITween>().ShutWindow();
         levelSelectObject.GetComponent<UITween>().ShutWindow();
         creditsObject.GetComponent<UITween>().ShutWindow();*/
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);

        PlayerPrefs.SetInt("Quality", index);
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];

        Screen.SetResolution(res.width, res.height, PlayerPrefs.GetInt("Fullscreen", 1) == 1);

        PlayerPrefs.SetString("Resolution", res.width + "x" + res.height);
    }

    public void Fullscreen(bool value)
    {
        Screen.fullScreen = value;

        if (value)
            PlayerPrefs.SetInt("Fullscreen", 1);
        else
            PlayerPrefs.SetInt("Fullscreen", 0);
    }

    public void MasterVolume(float value)
    {
        masterVolume.SetFloat("MasterVolume", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("MasterVolume", value);

        masterVolumeText.text = ((int) ((value / 1) * 100)).ToString();
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);


        sensitivityText.text = value.ToString("0");
    }

    public void Quit()
    {
        Application.Quit();
    }
    
}
