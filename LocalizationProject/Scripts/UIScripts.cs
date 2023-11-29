using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;

public class UIScripts : MonoBehaviour
{
    [NonSerialized] public GameObject settingsUI;
    public Dropdown languageDropdown;
    
    void Awake()
    {
        initialSettings();
    }
    void Start() {
    }
    private void initialSettings(){
        settingsUI = GameObject.Find("UI").transform.Find("SettingsUI").gameObject;
    }
    public void openSettingsPanel(){
        settingsUI.SetActive(true);
    }
    public void closeSettingsPanel(){
        settingsUI.SetActive(false);
    }
    public void quitButton()
    {
        Application.Quit();
    }
    public void selectLanguage(){
        StartCoroutine(changeLocales(languageDropdown.value));
    }
     public IEnumerator changeLocales(int currentLanguageIndex)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
    }
}
