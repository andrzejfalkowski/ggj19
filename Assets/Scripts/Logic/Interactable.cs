﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

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
    protected float tightness = 100f;
    public float Tightness { get { return tightness; } }

    [SerializeField]
    protected float durability = 100f;
    public float Durability { get { return durability; } }

    [SerializeField]
    private SpriteRenderer aura;

    [SerializeField]
    private GameObject label;

    private Spawner spawner;

    public AudioClip BuildSound;
    public AudioClip DestructionSound;

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
        if (label != null)
        {
            label.SetActive(true);
            label.transform.GetComponentInChildren<TextMeshProUGUI>().text = name;
        }
            
    }

    public void Unhighlight()
    {
        DOTween.Kill("Pulsate" + this.gameObject.GetInstanceID());
        aura.color = new Color(aura.color.r, aura.color.g, aura.color.b, 0f);
        if (label != null)
            label.SetActive(false);
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.HandleSpawnDestruction();
        }
    }
}
