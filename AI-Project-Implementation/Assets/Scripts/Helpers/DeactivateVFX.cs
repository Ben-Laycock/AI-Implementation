using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DeactivateVFX : MonoBehaviour
{
    private VisualEffect mVFX = null;
    private bool mDisableWhenComplete = false;

    private void Awake()
    {
        mVFX = this.gameObject.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mVFX.aliveParticleCount > 0) mDisableWhenComplete = true;
        if (mDisableWhenComplete)
        {
            if (mVFX.aliveParticleCount <= 0)
            {
                mVFX.Stop();
                mDisableWhenComplete = false;
                this.gameObject.SetActive(false);
            }
        }
    }

    public void WaitForDeactivation()
    {
        mVFX.Play();
    }
}
