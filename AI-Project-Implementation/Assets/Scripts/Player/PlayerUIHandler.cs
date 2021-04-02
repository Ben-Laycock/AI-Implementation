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

    private Text mScoreText;
    private Text mWaveText;

    private void Start()
    {
        mScoreText = mScoreObject.GetComponent<Text>();
        mWaveText = mWaveObject.GetComponent<Text>();
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
}