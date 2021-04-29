using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreaterThanDMTest : MonoBehaviour
{
    [SerializeField] private int mValueToCheck = 0;
    [SerializeField] private int mAmountToTake = 0;

    [SerializeField] private Text mTextValue;

    private void Start()
    {
        mTextValue.text = ">= " + mValueToCheck + "\n Take: " + mAmountToTake;
    }

    public bool TestValue(int valueToTest, DMTestObjectScript testObject)
    {
        if(valueToTest >= mValueToCheck)
            testObject.ChangeValueBy(-mAmountToTake);

        return valueToTest >= mValueToCheck;
    }
}