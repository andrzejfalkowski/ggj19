using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float snapDuration = 0.1f;

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
        pos.x = player.transform.position.x;
        this.transform.position = pos;

        this.transform.DOMoveY(player.transform.position.y, snapDuration).SetId("MoveCamera").SetUpdate(UpdateType.Late).SetEase(Ease.Linear);
    }
}
