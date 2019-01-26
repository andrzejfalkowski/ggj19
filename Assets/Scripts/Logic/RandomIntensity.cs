using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Light2D;
using UnityEngine.UI;
using System;

public class RandomIntensity : BaseLight 
{
	[SerializeField]
	float minIntensity = 1f;
	[SerializeField]
	float maxIntensity = 10f;

	[SerializeField]
	float minPhase = 0.5f;
	[SerializeField]
	float maxPhase = 1f;

    [SerializeField]
    bool sharp = false;

    private Image image;

    protected override void Start()
    {
        base.Start();

        image = this.transform.GetComponent<Image>();

        ChangeIntensity();
	}

	void ChangeIntensity()
	{
        float intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
        float phase = UnityEngine.Random.Range(minPhase, maxPhase);

        if (light != null)
		{
            if (!sharp)
            {
                light.DOIntensity(intensity, phase)
                    .OnComplete(() => ChangeIntensity())
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID());
            }
            else
            {
                light.intensity = intensity;
                DOVirtual.DelayedCall(phase, ()=> { ChangeIntensity(); }, false)
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
		}

		if(light2D != null)
		{
            if (!sharp)
            {
                DOTween.ToAlpha(() => light2D.Color, (c) => light2D.Color = c, intensity, phase)
                    .OnComplete(() => ChangeIntensity())
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
            else
            {
                light2D.Color = new Color(light2D.Color.r, light2D.Color.g, light2D.Color.b, intensity);
                DOVirtual.DelayedCall(phase, () => { ChangeIntensity(); }, false)
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
		}

		if(spriteRenderer != null)
		{
            if (!sharp)
            {
                spriteRenderer.DOFade(intensity, phase)
                    .OnComplete(() => ChangeIntensity())
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
            else
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, intensity);
                DOVirtual.DelayedCall(phase, () => { ChangeIntensity(); }, false)
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
		}

        if (image != null)
        {
            if (!sharp)
            {
                image.DOFade(intensity, phase)
                    .OnComplete(() => ChangeIntensity())
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
            else
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, intensity);
                DOVirtual.DelayedCall(phase, () => { ChangeIntensity(); }, false)
                    .SetId("RandomIntensity" + this.gameObject.GetInstanceID()); ;
            }
        }
    }

    public override void Hide(Action callback = null)
    {
        DOTween.Kill("RandomIntensity" + this.gameObject.GetInstanceID(), false);
        base.Hide();
    }

    public override void Show(Action callback = null)
    {
        base.Show(ChangeIntensity);
    }
}
