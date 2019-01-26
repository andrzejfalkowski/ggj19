using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Light2D;
using UnityEngine;

public class BaseLight : MonoBehaviour
{
    [SerializeField]
    private float defaultIntensity = 0.5f;

    protected Light light;
    protected LightSprite light2D;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        light = this.transform.GetComponent<Light>();
        light2D = this.transform.GetComponent<LightSprite>();
        spriteRenderer = this.transform.GetComponent<SpriteRenderer>();

        ChangeIntensity(defaultIntensity);
    }

    public void ChangeIntensity(float intensity)
    {
        if (light2D != null)
        {
            light2D.Color = new Color(light2D.Color.r, light2D.Color.g, light2D.Color.b, intensity);
        }
    }

    public virtual void Hide(Action callback = null)
    {
        DOTween.ToAlpha(() => light2D.Color, (c) => light2D.Color = c, 0, 0.5f)
                    .SetId("Hide" + this.gameObject.GetInstanceID())
                    .OnComplete(()=> { if (callback != null) callback(); });
    }

    public virtual void Show(Action callback = null)
    {
        DOTween.ToAlpha(() => light2D.Color, (c) => light2D.Color = c, defaultIntensity, 0.5f)
                    .SetId("Show" + this.gameObject.GetInstanceID())
                    .OnComplete(() => { if (callback != null) callback(); });
    }
}