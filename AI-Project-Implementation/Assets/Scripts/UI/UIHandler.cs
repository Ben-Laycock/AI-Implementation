using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private bool mSoundEnabled;
    public bool soundEnabled
    {
        get { return mSoundEnabled; }
    }

    [SerializeField]
    private float soundEnabledBtnFadeSpeed = 8;

    [SerializeField]
    private GameObject mGameMenuLogo;
    private RectTransform mGameMenuLogoTransform;

    private bool mButtonFlashAnimating = false;
    public bool buttonFlashAnimating
    {
        get { return mButtonFlashAnimating; }
        set { mButtonFlashAnimating = value; }
    }
    private float mHighlightFlashSpeed = 16;

    // Start is called before the first frame update
    void Start()
    {
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

        mGameMenuLogoTransform = mGameMenuLogo.GetComponent<RectTransform>();
        if (null == mGameMenuLogoTransform)
            return;

        mSoundEnabled = true;
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
    }

    public void ChangeGameVolume()
    {

    }

    public void PlayGame(GameObject triggerObj)
    {

    }

    public void ExitGame(GameObject triggerObj)
    {
        Application.Quit();
    }
}
