using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlot : Interactable
{
    public void BuildWall(Pickable pickable)
    {
        this.spriteRenderer.sprite = pickable.Sprite;
        this.resourceType = pickable.ResourceType;
    }
}
