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

    public int WinItems = 0;

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

    public const float PREPARATION_DURATION = 30f;
    public const float INFLOW_DURATION = 20f;
    public const float OUTFLOW_DURATION = 5f;
    void NextPhase()
    {
        switch (CurrentPhase)
        {
            case EGameplayPhase.Intro:
            case EGameplayPhase.Wave:
                CurrentWave = (EWaveType)UnityEngine.Random.Range(0, (int)EWaveType.COUNT);
                CurrentPhase = EGameplayPhase.Preparation;

                DOVirtual.DelayedCall(PREPARATION_DURATION, ()=> { NextPhase(); }).SetId("NextPhase");
                DOTween.To(()=> PREPARATION_DURATION, (value) => { UIManager.Instance.ChangeTimerLabel(value) ; }, 0f, PREPARATION_DURATION);
                MusicManager.Instance.PlayAction(false);

                DOVirtual.DelayedCall(PREPARATION_DURATION - 10f, () => { MusicManager.Instance.Siren(); });
                DOVirtual.DelayedCall(PREPARATION_DURATION - 5f, () => { MusicManager.Instance.Siren(); });
                DOVirtual.DelayedCall(PREPARATION_DURATION - 4f, () => { MusicManager.Instance.Siren(); });
                DOVirtual.DelayedCall(PREPARATION_DURATION - 3f, () => { MusicManager.Instance.Siren(); });
                DOVirtual.DelayedCall(PREPARATION_DURATION - 2f, () => { MusicManager.Instance.Siren(); });
                DOVirtual.DelayedCall(PREPARATION_DURATION - 1f, () => { MusicManager.Instance.Siren(); });
                break;
            case EGameplayPhase.Preparation:
                CurrentPhase = EGameplayPhase.Wave;
                DOVirtual.DelayedCall(INFLOW_DURATION + OUTFLOW_DURATION, () => { NextPhase(); }).SetId("NextPhase"); ;
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
        MusicManager.Instance.PlayGameOver(); 
    }

    public void GameWin()
    {
        CurrentPhase = EGameplayPhase.GameOver;
        UIManager.Instance.GameWin();
        //MusicManager.Instance.PlayGameOver();
    }

    public void WinItem()
    {
        WinItems++;
        if (WinItems > 2)
            GameWin();
    }
}
