using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAffects : MonoBehaviour
{
    private AudioSource audioS;
    [SerializeField] public AudioClip[] clips;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
        Checker.checkerEvent += SoundPlay;
    }

    private void SoundPlay()
    {
        audioS.PlayOneShot(clips[0],0.8f);        
        audioS.PlayOneShot(clips[2], 0.4f);
        Vibration.Vibrate(100);
        Invoke("LateSoundPlay", 0.13f);
    }
    private void LateSoundPlay()
    {
        audioS.PlayOneShot(clips[1], 0.8f);
    }
    private void OnDestroy()
    {
        Checker.checkerEvent -= SoundPlay;
    }
}
