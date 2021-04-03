using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mMainMenuCanvas = null;
    [SerializeField] private GameObject mLoadingCanvas = null;
    [SerializeField] private bool mSoundEnabled = true;

    private void Start()
    {
        mMainMenuCanvas.SetActive(true);
        mLoadingCanvas.SetActive(false);
    }

    private void Update()
    {

    }

    public void PlayGame()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        mMainMenuCanvas.SetActive(false);
        mLoadingCanvas.SetActive(true);
        op.allowSceneActivation = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void EnableSound()
    {
        mSoundEnabled = !mSoundEnabled;

        if (mSoundEnabled)
        {
            PlayerPrefs.SetInt("SoundEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SoundEnabled", 0);
        }
    }

    public void ChangeGameVolume(float argVolume)
    {
        PlayerPrefs.SetFloat("SoundSliderValue", argVolume);
    }
}
