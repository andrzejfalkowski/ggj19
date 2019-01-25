using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    [SerializeField]
    private Image interactionBar;

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

        //interactionBar.gameObject.SetActive(value > 0f && value < 1f);
    }
}
