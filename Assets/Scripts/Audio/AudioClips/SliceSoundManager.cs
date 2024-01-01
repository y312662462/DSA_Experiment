using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceSoundManager : MonoBehaviour
{
    public AudioClip sliceAudioClip;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySliceSound()
    {
        audioSource.PlayOneShot(sliceAudioClip);
    }
}
