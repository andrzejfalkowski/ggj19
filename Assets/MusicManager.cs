﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance = null;

    [SerializeField]
    List<AudioClip> clips;

    [SerializeField]
    AudioSource musicBase;

    [SerializeField]
    AudioSource musicAction;

    [SerializeField]
    AudioSource gameOverMusic;

    [SerializeField]
    AudioSource siren;

    [SerializeField]
    AudioSource sfx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            musicAction.volume = 0f;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void PlayAction(bool play)
    {
        musicAction.DOFade(play ? 1f : 0f, 3f);
    }

    public void PlayGameOver()
    {
        gameOverMusic.Play();
    }

    public void Siren()
    {
        siren.Play();
    }

    public void PlaySFX(string name)
    {

    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
