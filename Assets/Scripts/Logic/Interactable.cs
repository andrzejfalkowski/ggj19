using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    protected string name;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    public Sprite Sprite { get { return spriteRenderer.sprite; } }

    [SerializeField]
    protected EResourceType resourceType = EResourceType.Metal;
    public EResourceType ResourceType { get { return resourceType; } }

    [SerializeField]
    protected float timeToInteract = 0.3f;
    public float TimeToInteract { get { return timeToInteract; } }

    [SerializeField]
    protected float value = 100f;
    public float Value { get { return value; } }

    [SerializeField]
    private SpriteRenderer aura;

    private Spawner spawner;

    private void Start()
    {
        Unhighlight();
    }

    public void Init(Spawner spawner)
    {
        this.spawner = spawner;
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

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.HandleSpawnDestruction();
        }
    }
}
