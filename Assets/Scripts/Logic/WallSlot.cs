using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallSlot : Interactable
{
    [SerializeField]
    private ParticleSystem leakParticle;
    private Sprite defaultSprite;
    private float startDurability;

    [SerializeField]
    private Image HPBar;
    [SerializeField]
    private GameObject Exclamation;

    [SerializeField]
    private List<Sprite> sprites;

    private void Start()
    {
        GameplayManager.Instance.SubscribeWallSlot(this);
        defaultSprite = this.spriteRenderer.sprite;
        this.durability = startDurability = 0f;

        //HPBar.gameObject.SetActive(false);
        Exclamation.SetActive(true);
        SetLeakPower();
    }

    public virtual void BuildWall(Pickable pickable)
    {
        //this.spriteRenderer.sprite = pickable.Sprite;
        this.spriteRenderer.sprite = sprites[(int)pickable.ResourceType];
        this.resourceType = pickable.ResourceType;
        this.tightness = pickable.Tightness;
        this.durability = pickable.Durability;
        startDurability = pickable.Durability;
        this.DestructionSound = pickable.DestructionSound;

        HPBar.fillAmount = this.durability / startDurability;
        Exclamation.gameObject.SetActive(this.durability > 0f);
    }

    public virtual void DamageWall(float damage)
    {
        //UnityEngine.Debug.Log(this.durability + " " + startDurability);
        this.durability -= damage;
        //UnityEngine.Debug.Log(this.durability + " " + startDurability);
        SetLeakPower();

        HPBar.fillAmount = this.durability / startDurability;
        Exclamation.gameObject.SetActive(this.durability > 0f);

        if (this.durability <= 0f)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        if (DestructionSound != null && this.tightness > 0f)
            MusicManager.Instance.PlaySFX(DestructionSound);

        this.spriteRenderer.sprite = defaultSprite;
        this.tightness = 0f;
        this.durability = 0f;


    }

    protected virtual void SetLeakPower()
    {
        leakParticle.gameObject.SetActive(this.durability > 0f);

        var velocity = leakParticle.limitVelocityOverLifetime;
        velocity.dampen = 0.2f + 0.8f * (this.durability / startDurability);

        var emission = leakParticle.emission;
        emission.rate = 100f * (1f - (this.durability / startDurability));
        // UnityEngine.Debug.Log(this.durability + " " + startDurability);
        //UnityEngine.Debug.Log(velocity.dampen + " " + (this.durability / startDurability));
    }

    public void StopLeak()
    {
        leakParticle.gameObject.SetActive(false);
    }
}
