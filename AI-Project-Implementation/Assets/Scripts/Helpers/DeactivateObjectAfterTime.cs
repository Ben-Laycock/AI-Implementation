using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObjectAfterTime : MonoBehaviour
{

    private float mDeactivationTime = 0f;
    public float DeactivationTime
    {

        set
        {
            mDeactivationTime = value;
        }
        get
        {
            return mDeactivationTime;
        }

    }


    private float mTimer = 0f;
    private bool mTimerActive = false;


    // Update is called once per frame
    void Update()
    {

        if (mTimerActive)
        {
            mTimer -= Time.deltaTime;

            if (0f >= mTimer)
                ManageDeactivation();
        }

    }


    private void ManageDeactivation()
    {

        DeactivateTimer();
        ResetTimer();

        this.gameObject.SetActive(false);

    }


    /*
     * Resets the timer to the Deactivation Time current stored
     */
    public void ResetTimer()
    {

        mTimer = mDeactivationTime;

    }


    /*
     * Starts the timer countdown until object deactivation
     */
    public void ActivateTimer()
    {

        mTimerActive = true;

    }


    /*
     * Stops the timer countdown
     */
    public void DeactivateTimer()
    {

        mTimerActive = false;

    }

}
