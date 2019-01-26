using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlot : Interactable
{
    private Sprite defaultSprite;

    private void Start()
    {
        GameplayManager.Instance.SubscribeWallSlot(this);
        defaultSprite = this.spriteRenderer.sprite;
    }

    public void BuildWall(Pickable pickable)
    {
        this.spriteRenderer.sprite = pickable.Sprite;
        this.resourceType = pickable.ResourceType;
        this.tightness = pickable.Tightness;
        this.durability = pickable.Durability;
    }

    public void DamageWall(float damage)
    {
        this.durability -= damage;
        if (this.durability <= 0f)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        this.spriteRenderer.sprite = defaultSprite;
        this.tightness = 0f;
        this.durability = 0f;
    }
}
