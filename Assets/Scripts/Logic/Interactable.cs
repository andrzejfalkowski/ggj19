using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private string name;
    [SerializeField]
    private EResourceType resourceType = EResourceType.Metal;
    [SerializeField]
    private float timeToPickUp = 0.3f;
    public float TimeToPickup { get { return timeToPickUp; } }

    [SerializeField]
    private float value = 100f;

    [SerializeField]
    private SpriteRenderer aura;

    private void Start()
    {
        Unhighlight();
    }

    public void Highlight()
    {
        DOTween.Kill("Pulsate" + this.gameObject.GetInstanceID());
        aura.color = new Color(aura.color.r, aura.color.g, aura.color.b, 0f);
        if (aura != null)
        {
            aura.DOFade(1f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetId("Pulsate" + this.gameObject.GetInstanceID());
        }
    }

    public void Unhighlight()
    {
        DOTween.Kill("Pulsate" + this.gameObject.GetInstanceID());
        aura.color = new Color(aura.color.r, aura.color.g, aura.color.b, 0f);
    }
}
