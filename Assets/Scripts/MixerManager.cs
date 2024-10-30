using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
     [SerializeField] private AudioMixer mainMixer;
    
    public void MasterVolume(float level)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }
}
