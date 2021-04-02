using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField] private GameObject mHealthBarObject;
    [SerializeField] private GameObject mEnergyBarObject;

    [Header("Text Fields")]
    [SerializeField] private GameObject mScoreObject;
    [SerializeField] private GameObject mWaveObject;
    [SerializeField] private GameObject mObjectiveObject;

    [Header("Weapon Images")]
    [SerializeField] private GameObject mSelectedWeaponImage;
    [SerializeField] private GameObject mUnSelectedWeaponImage;
    [SerializeField] private Sprite mSelected;
    [SerializeField] private Sprite mUnSelected;

    private Image mSelectedImage;
    private Image mUnSelectedImage;

    private Text mScoreText;
    private Text mWaveText;
    private Text mObjectiveText;

    private void Start()
    {
        mScoreText = mScoreObject.GetComponent<Text>();
        mWaveText = mWaveObject.GetComponent<Text>();
        mObjectiveText = mObjectiveObject.GetComponent<Text>();

        mSelectedImage = mSelectedWeaponImage.GetComponent<Image>();
        mUnSelectedImage = mUnSelectedWeaponImage.GetComponent<Image>();
    }

    public void UpdateScoreText(int score)
    {
        mScoreText.text = "SCORE: " + score;
    }

    public void UpdateWaveText(int wave)
    {
        mWaveText.text = "WAVE: " + wave;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        mHealthBarObject.GetComponent<RectTransform>().sizeDelta = new Vector2((200 / maxHealth) * currentHealth, 30);
    }

    public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
    {
        mEnergyBarObject.GetComponent<RectTransform>().sizeDelta = new Vector2((200 / maxEnergy) * currentEnergy, 30);
    }

    public void UpdateObjective(int amount)
    {
        mObjectiveText.text = "COLLECT RELICS " + amount + "/3";
    }

    public void SwitchSelectedWeapon()
    {
        if(mSelectedImage.sprite == mSelected)
        {
            mSelectedImage.sprite = mUnSelected;
            mUnSelectedImage.sprite = mSelected;
        }
        else
        {
            mSelectedImage.sprite = mSelected;
            mUnSelectedImage.sprite = mUnSelected;
        }
    }
}