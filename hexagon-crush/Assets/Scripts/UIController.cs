using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject volMute;
    [SerializeField] private GameObject volUnmute;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource.volume = 0.5f;
    }

    public void Mute() 
    {
        volMute.SetActive(false);
        volUnmute.SetActive(true);
        audioSource.volume = 0f;
    }

    public void Unmute() 
    {
        volMute.SetActive(true);
        volUnmute.SetActive(false);
        audioSource.volume = 1f;
    }
}
