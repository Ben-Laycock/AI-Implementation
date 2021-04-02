using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    private static AudioSystem sInstance = null;

    // Get Singleton Instance
    public static AudioSystem Instance
    {
        get
        {
            return sInstance;
        }
    }


    private void Awake()
    {      
        if (null != sInstance && this != sInstance)
            Destroy(this.gameObject);

        sInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }


    private void Start()
    {
        // Get pool system to create a pool of audio objects
        if (null != PoolSystem.Instance)
        {
            PoolSystem.Instance.CreatePool(mAudioObject, 20, false);
        }
    }


    [SerializeField] private Dictionary<string, AudioClip> mSounds = new Dictionary<string, AudioClip>();
    [SerializeField] private GameObject mAudioObject = null;


    /*
     * Function to add a sound the the Audio System
     * 
     * argKey = The name that should be assigned to the AudioClip
     * argSound = The sound that you want to add to the System
     */
    public void AddSoundToSystem(string argKey, AudioClip argSound)
    {
        // Returns due to null audio clip
        if (null == argSound)
        {
            Debug.LogWarning("Audio System: Given AudioClip is null.");
            return;
        }

        // Return due to invalid sound key
        if ("" == argKey)
        {
            Debug.LogWarning("Audio System: Given empty key for sound creation.");
            return;
        }

        // Return due to already existing key
        if (mSounds.ContainsKey(argKey))
        {
            Debug.LogWarning("Audio System: Given key already exists.");
            return;
        }

        mSounds.Add(argKey, argSound);
    }


    /*
     * Function used to play a sound once
     * 
     * argKey = The name of the sound you want to play
     * argVolume = The volume that the sound should be played at
     */
    public void PlaySound(string argKey, float argVolume)
    {
        // Return due to invalid key
        if (!mSounds.ContainsKey(argKey) || "" == argKey)
        {
            Debug.LogWarning("Audio System: Given invalid key.");
            return;
        }

        GameObject audioObject = PoolSystem.Instance.GetObjectFromPool(mAudioObject, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        DeactivateObjectAfterTime deactivationCompononent = audioObject.GetComponent<DeactivateObjectAfterTime>();

        if (null != audioSource)
        {
            deactivationCompononent.DeactivationTime = mSounds[argKey].length;
            deactivationCompononent.ResetTimer();

            audioSource.clip = mSounds[argKey];
            audioSource.volume = argVolume;

            audioObject.SetActive(true);
            audioSource.Play();
            deactivationCompononent.ActivateTimer();
        }
    }


    /*
     * Function to get an audio object form the pool system
     * 
     * This function should be used when you need to play a sound on loop for a specific time 
     * (You will need to disable the object yourself after you are finished with it)
     */
    public GameObject GetAudioObject()
    {
        return PoolSystem.Instance.GetObjectFromPool(mAudioObject, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
    }
}
