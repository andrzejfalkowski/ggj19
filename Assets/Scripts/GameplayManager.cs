using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance = null;

    public PlayerController Player;

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
                CurrentPhase = EGameplayPhase.Preparation;
                DOVirtual.DelayedCall(30f, ()=> { NextPhase(); }).SetId("NextPhase");
                break;
            case EGameplayPhase.Preparation:
                CurrentWave = (EWaveType)UnityEngine.Random.Range(0, (int)EWaveType.COUNT);
                CurrentPhase = EGameplayPhase.Wave;
                DOVirtual.DelayedCall(30f, () => { NextPhase(); }).SetId("NextPhase"); ;
                break;
        }

        UIManager.Instance.ChangePhaseLabel(CurrentWave, CurrentPhase);
    }
}
