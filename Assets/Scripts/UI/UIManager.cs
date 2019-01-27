using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    private TextMeshProUGUI phaseLabel;
    [SerializeField]
    private TextMeshProUGUI timerLabel;

    [SerializeField]
    private Image oxygenBar;

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameWinPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            gameOverPanel.SetActive(false);
            gameWinPanel.SetActive(false);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
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

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void GameWin()
    {
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        DOTween.KillAll(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ChangeOxygenLevel(float value)
    {
        oxygenBar.fillAmount = value;

        //oxygenBar.gameObject.SetActive(value < 1f);
    }
}
