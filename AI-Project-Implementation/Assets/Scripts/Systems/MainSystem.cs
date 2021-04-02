using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Running = 0,
    Paused = 1,
    GameOver = 2
}

public class MainSystem : MonoBehaviour
{
    [Header("Game Values")]
    [SerializeField] private bool mGameOver = false;

    [Header("UI Canvas's")]
    [SerializeField] private GameObject mPauseMenuCanvas;
    [SerializeField] private GameObject mGameOverCanvas;

    [Header("Canvas Elements")]
    [SerializeField] private GameObject mGameOverScoreObject;

    private Text mScoreText;

    private GameState mCurrentGameState = GameState.Running;

    private int mFinalScore = 0;

    private void Start()
    {
        Time.timeScale = 1;
        mScoreText = mGameOverScoreObject.GetComponent<Text>();
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(mCurrentGameState == GameState.Running)
            {
                mCurrentGameState = GameState.Paused;
                mPauseMenuCanvas.SetActive(true);
                Time.timeScale = 0;
            }
            else if(mCurrentGameState == GameState.Paused)
            {
                mCurrentGameState = GameState.Running;
                mPauseMenuCanvas.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if(mGameOver) {
            Time.timeScale = 0;
            mCurrentGameState = GameState.GameOver;
            if(!mGameOverCanvas.activeSelf)
            {
                mScoreText.text = "SCORE: " + mFinalScore;
                mGameOverCanvas.SetActive(true);
            }
        }

        switch(mCurrentGameState)
        {
            case GameState.Running:
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case GameState.Paused:
                Cursor.lockState = CursorLockMode.None;
                break;

            case GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                break;

            default:
                break;
        }

    }

    public void ResumeGame()
    {
        mCurrentGameState = GameState.Running;
        mPauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu()
    {
        PoolSystem.Instance.ResetPools();
        Time.timeScale = 1;
        mCurrentGameState = GameState.Running;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void RestartGame()
    {
        PoolSystem.Instance.ResetPools();
        Time.timeScale = 1;
        mCurrentGameState = GameState.Running;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void SetGameOver(bool value)
    {
        mGameOver = value;
    }

    public void SetFinalScore(int value)
    {
        mFinalScore = value;
    }

}