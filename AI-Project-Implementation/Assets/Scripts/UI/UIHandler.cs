using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mPlayButton;
    private RectTransform mPlayButtonTransform;

    [SerializeField]
    private GameObject mExitButton;
    private RectTransform mExitButtonTransform;

    [SerializeField]
    private GameObject mButtonFlash;
    private RectTransform mButtonFlashTransform;
    private Image mButtonFlashImg;

    [SerializeField]
    private GameObject mSoundEnabledButton;
    private RectTransform mSoundEnabledButtonTransform;
    private Image mSoundEnabledButtonImg;

    [SerializeField]
    private GameObject mWayFinderObject;

    private Renderer mmWayFinderObjectRenderer;
    private Transform mWayFinderObjectTransform;

    [SerializeField]
    private float mWayFinderAnimationSpeed = 0.6f;
    [SerializeField]
    private float mWayFinderAnimationHeight = 3f;
    [SerializeField]
    private float mWayFinderAnimationRotationSpeed = 0.6f;
    [SerializeField]
    private float mWayFinderAnimationRotationAmount = 0.05f;

    [SerializeField] private Slider mSoundSlider;

    private bool mSoundEnabled;
    public bool soundEnabled
    {
        get { return mSoundEnabled; }
    }

    [SerializeField]
    private float soundEnabledBtnFadeSpeed = 8;

    private bool mButtonFlashAnimating = false;
    public bool buttonFlashAnimating
    {
        get { return mButtonFlashAnimating; }
        set { mButtonFlashAnimating = value; }
    }
    private float mHighlightFlashSpeed = 16;

    [SerializeField] private GameObject mMainMenuCanvas = null;
    [SerializeField] private GameObject mLoadingCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        if (mMainMenuCanvas != null) mMainMenuCanvas.SetActive(true);
        if (mLoadingCanvas != null) mLoadingCanvas.SetActive(false);

        mPlayButtonTransform = mPlayButton.GetComponent<RectTransform>();
        if (null == mPlayButtonTransform)
            return;

        mExitButtonTransform = mExitButton.GetComponent<RectTransform>();
        if (null == mExitButtonTransform)
            return;

        mButtonFlashTransform = mButtonFlash.GetComponent<RectTransform>();
        if (null == mButtonFlashTransform)
            return;

        mButtonFlashImg = mButtonFlash.GetComponent<Image>();
        if (null == mButtonFlashImg)
            return;

        mSoundEnabledButtonTransform = mSoundEnabledButton.GetComponent<RectTransform>();
        if (null == mSoundEnabledButtonTransform)
            return;

        mSoundEnabledButtonImg = mSoundEnabledButton.GetComponent<Image>();
        if (null == mSoundEnabledButtonImg)
            return;

        mmWayFinderObjectRenderer = mWayFinderObject.GetComponent<Renderer>();
        if (null == mmWayFinderObjectRenderer)
            return;

        mWayFinderObjectTransform = mWayFinderObject.GetComponent<Transform>();
        if (null == mWayFinderObjectTransform)
            return;

        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            mSoundEnabled = true;
        }
        else
        {
            mSoundEnabled = false;
        }

        mSoundSlider.value = PlayerPrefs.GetFloat("SoundSliderValue", 1.0f);

        mButtonFlashAnimating = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called 60 times per second.
    void FixedUpdate()
    {
        float soundEnabledBtnAlpha = mSoundEnabledButtonImg.color.a;
        if (mSoundEnabled)
        {
            soundEnabledBtnAlpha += 0.05f * soundEnabledBtnFadeSpeed;
        }
        else
        {
            soundEnabledBtnAlpha -= 0.05f * soundEnabledBtnFadeSpeed;
        }
        soundEnabledBtnAlpha = Mathf.Clamp(soundEnabledBtnAlpha, 0, 1);
        mSoundEnabledButtonImg.color = new Color(1, 1, 1, soundEnabledBtnAlpha);

        mWayFinderObjectTransform.position = new Vector3(mWayFinderObjectTransform.position.x, Mathf.Sin(Time.time * mWayFinderAnimationSpeed) * mWayFinderAnimationHeight, mWayFinderObjectTransform.position.z);

        mWayFinderObjectTransform.RotateAround(mmWayFinderObjectRenderer.bounds.center, new Vector3(0, 0, 1), mWayFinderAnimationRotationAmount * Mathf.Sin(Time.time * mWayFinderAnimationRotationSpeed));
        mWayFinderObjectTransform.RotateAround(mmWayFinderObjectRenderer.bounds.center, new Vector3(1, 0, 0), mWayFinderAnimationRotationAmount * Mathf.Sin(Time.time * mWayFinderAnimationRotationSpeed));

        if (!mButtonFlashAnimating)
            return;

        if (mButtonFlashImg.color.a == 0)
        {
            ResetFlashObject();
            return;
        }

        float currentAlphaVal = mButtonFlashImg.color.a;
        currentAlphaVal -= 0.01f * mHighlightFlashSpeed;
        currentAlphaVal = Mathf.Clamp(currentAlphaVal, 0, 1);
        mButtonFlashImg.color = new Color(1, 1, 1, currentAlphaVal);

        mButtonFlashTransform.localScale += new Vector3(0.005f * mHighlightFlashSpeed, 0.005f * mHighlightFlashSpeed, 0) ;
    }

    private void ResetFlashObject()
    {
        mButtonFlashAnimating = false;
        mButtonFlashTransform.position = new Vector3(-10000, -10000, 0);
        mButtonFlashImg.color = new Color(1, 1, 1, 1);
    }

    public void MenuButtonMouseEnter(GameObject triggerObj)
    {
        // Find the RectTransform of the triggerObj.
        RectTransform triggerObjRectTransform = triggerObj.GetComponent<RectTransform>();
        if (null == triggerObjRectTransform)
            return;

        // Perform the white flashing effect here.
        Vector3 triggerObjScale = triggerObjRectTransform.localScale;
        Vector3 triggerObjPos = triggerObjRectTransform.position;

        mButtonFlashTransform.localScale = triggerObjScale;
        mButtonFlashTransform.position = triggerObjPos;
        mButtonFlashImg.color = new Color(1, 1, 1, 1);

        mButtonFlashAnimating = true;
    }

    public void EnableSound()
    {
        mSoundEnabled = !mSoundEnabled;

        if(soundEnabled)
        {
            PlayerPrefs.SetInt("SoundEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SoundEnabled", 0);
        }

        AudioSystem.Instance.PlaySound("Weird", 0.1f);
    }

    public void ChangeGameVolume()
    {
        PlayerPrefs.SetFloat("SoundSliderValue", mSoundSlider.value);

        AudioSystem.Instance.PlaySound("Weird", 0.1f);
    }

    public void PlayGame()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        mMainMenuCanvas.SetActive(false);
        mLoadingCanvas.SetActive(true);
        op.allowSceneActivation = true;
        AudioSystem.Instance.PlaySound("PlayGameSound", 0.1f);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
