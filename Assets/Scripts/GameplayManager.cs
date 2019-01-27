using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance = null;

    public PlayerController Player;
    public WaveController Waves;

    public EGameplayPhase CurrentPhase = EGameplayPhase.Intro;
    public EWaveType CurrentWave = EWaveType.Flood;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (Player == null)
            {
                Player = FindObjectOfType<PlayerController>();
            }
            if (Waves == null)
            {
                Waves = FindObjectOfType<WaveController>();
            }
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NextPhase();
    }

    void NextPhase()
    {
        switch (CurrentPhase)
        {
            case EGameplayPhase.Intro:
            case EGameplayPhase.Wave:
                CurrentWave = (EWaveType)UnityEngine.Random.Range(0, (int)EWaveType.COUNT);
                CurrentPhase = EGameplayPhase.Preparation;
                DOVirtual.DelayedCall(10f, ()=> { NextPhase(); }).SetId("NextPhase");
                DOTween.To(()=> 10f, (value) => { UIManager.Instance.ChangeTimerLabel(value) ; }, 0f, 10f);
                MusicManager.Instance.PlayAction(false);
                break;
            case EGameplayPhase.Preparation:
                CurrentPhase = EGameplayPhase.Wave;
                DOVirtual.DelayedCall(30f, () => { NextPhase(); }).SetId("NextPhase"); ;
                //DOTween.To(() => 30f, (value) => { UIManager.Instance.ChangeTimerLabel(value); }, 0f, 30f);
                Waves.StartInflow();
                MusicManager.Instance.PlayAction(true);
                break;
        }

        UIManager.Instance.ChangePhaseLabel(CurrentWave, CurrentPhase);
    }

    public void SubscribeWallSlot(WallSlot wallSlot)
    {
        Waves.WallSlots.Add(wallSlot);
    }

    public void GameOver()
    {
        CurrentPhase = EGameplayPhase.GameOver;
        UIManager.Instance.GameOver();
    }
}
