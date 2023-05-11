using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageSelectionManager : MonoBehaviour
{
    public void SelectEnglish()
    {
        PlayerPrefs.SetString("SelectedLanguage", "English");
        SceneManager.LoadScene("SampleScene");
    }

    public void SelectPortuguese()
    {
        PlayerPrefs.SetString("SelectedLanguage", "Portuguese");
        SceneManager.LoadScene("SampleScene");
    }
}