using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialSlot : WallSlot
{
    public EResourceType AcceptedResource;

    public override void DamageWall(float damage)
    {
    }

    public override void BuildWall(Pickable pickable)
    {
        this.GetComponent<Collider2D>().enabled = false;
        GameplayManager.Instance.WinItem();
    }

    protected override void SetLeakPower()
    {
    }
}
