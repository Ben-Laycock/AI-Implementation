using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private GameState mCurrentGameState = GameState.Running;

    private void Start()
    {

    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(mCurrentGameState == GameState.Running)
            {
                mCurrentGameState = GameState.Paused;
                mPauseMenuCanvas.SetActive(true);
            }
            else if(mCurrentGameState == GameState.Paused)
            {
                mCurrentGameState = GameState.Running;
                mPauseMenuCanvas.SetActive(false);
            }
        }

        if(mGameOver) {
            mCurrentGameState = GameState.GameOver;
            if(!mGameOverCanvas.activeSelf)
                mGameOverCanvas.SetActive(true);
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
    }

    public void ReturnToMainMenu()
    {
        PoolSystem.Instance.ResetPools();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        PoolSystem.Instance.ResetPools();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

}