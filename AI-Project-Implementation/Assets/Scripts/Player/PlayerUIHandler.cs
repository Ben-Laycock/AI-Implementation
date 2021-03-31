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

    private Text mScoreText;

    private void Start()
    {
        mScoreText = mScoreObject.GetComponent<Text>();
    }

    public void UpdateScoreText(int score)
    {
        mScoreText.text = "SCORE: " + score;
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