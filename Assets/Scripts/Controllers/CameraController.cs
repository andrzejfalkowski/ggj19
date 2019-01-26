using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float snapDuration = 0.1f;
    [SerializeField]
    private Vector3 offset = Vector3.zero;

    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameplayManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        DOTween.Kill("MoveCamera");

        Vector3 pos = this.transform.position;
        pos.x = player.transform.position.x + offset.x;
        this.transform.position = pos;

        this.transform.DOMoveY(player.transform.position.y + offset.y, snapDuration).SetId("MoveCamera").SetUpdate(UpdateType.Late).SetEase(Ease.Linear);
    }
}
