using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    private Image interactionBar;
    [SerializeField]
    private TextMeshProUGUI interactionLabel;
    [SerializeField]
    private TextMeshProUGUI phaseLabel;
    [SerializeField]
    private TextMeshProUGUI timerLabel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            ChangeInteractionProgress(0f);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void ChangeInteractionProgress(float value)
    {
        interactionBar.fillAmount = value;

        interactionBar.gameObject.SetActive(value > 0f && value < 1f);
    }

    public void ChangeInteractionLabel(string label)
    {
        interactionLabel.text = label;
    }

    public void ChangePhaseLabel(EWaveType type, EGameplayPhase phase)
    {
        switch (type)
        {
            case EWaveType.Flood:
                phaseLabel.text = phase == EGameplayPhase.Wave ? "Flood in progress!" : "Flood incoming!";
                break;
            case EWaveType.SewerDrop:
                phaseLabel.text = phase == EGameplayPhase.Wave ? "Sewer drop in progress!" : "Sewer drop incoming!";
                break;
            case EWaveType.Radiation:
                phaseLabel.text = phase == EGameplayPhase.Wave ? "Radiation in progress!" : "Radiation incoming!";
                break;
            case EWaveType.Swarm:
                phaseLabel.text = phase == EGameplayPhase.Wave ? "Swarm in progress!" : "Swarm incoming!";
                break;
            case EWaveType.ToxicGas:
                phaseLabel.text = phase == EGameplayPhase.Wave ? "Toxic gas in progress!" : "Toxic gas incoming!";
                break;
        }
    }

    public void ChangeTimerLabel(float value)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(value);
        timerLabel.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
    }
}
