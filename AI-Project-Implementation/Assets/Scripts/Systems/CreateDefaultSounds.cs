using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDefaultSounds : MonoBehaviour
{
    [System.Serializable]
    public struct SoundSystemClip
    {
        public string mName;
        public AudioClip mClip;
    }

    public SoundSystemClip[] mSounds;

    // Start is called before the first frame update
    void Start()
    {
        if (mSounds == null) return;
        foreach(SoundSystemClip sound in mSounds)
        {
            AudioSystem.Instance.AddSoundToSystem(sound.mName, sound.mClip);
        }
    }
}
