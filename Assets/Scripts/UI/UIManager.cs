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
}
