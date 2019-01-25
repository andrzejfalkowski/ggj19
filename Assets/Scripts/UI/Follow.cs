using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField]
    private Transform followed;

    [SerializeField]
    private Vector3 shift;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = this.GetComponentInParent<RectTransform>();
    }

    void OnGUI()
    {
        rectTransform.position = Camera.main.WorldToScreenPoint(followed.position) + shift;
    }
}
