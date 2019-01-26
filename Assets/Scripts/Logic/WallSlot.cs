using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlot : Interactable
{
    private void Start()
    {
        GameplayManager.Instance.SubscribeWallSlot(this);
    }

    public void BuildWall(Pickable pickable)
    {
        this.spriteRenderer.sprite = pickable.Sprite;
        this.resourceType = pickable.ResourceType;
        this.value = pickable.Value;
    }
}
