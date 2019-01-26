using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PassableCollider : MonoBehaviour
{
    private BoxCollider2D collider;
    [SerializeField]
    private bool playerIsTouching;

    private void Awake()
    {
        collider = this.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        playerIsTouching = this.collider.IsTouching(GameplayManager.Instance.Player.GetComponent<Collider2D>())
            && this.collider.isTrigger;
        if (GameplayManager.Instance == null)
        {
            return;
        }
        if (IsMidAirAndAscending()
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S)
            || playerIsTouching)
        {
            SetPassable(true);
        }
        else if ((GameplayManager.Instance.Player.Grounded
                || GameplayManager.Instance.Player.IsFalling())
            && !playerIsTouching)
        {
            SetPassable(false);
        }
    }

    private bool IsMidAirAndAscending()
    {
        return (!GameplayManager.Instance.Player.Grounded
                || GameplayManager.Instance.Player.transform.position.y < this.transform.position.y)
            && !GameplayManager.Instance.Player.IsFalling()
            && !playerIsTouching;
    }

    private void SetPassable(bool passable)
    {
        if (passable == collider.isTrigger)
        {
            return;
        }
        collider.isTrigger = passable;
        if (passable)
        {
            GameplayManager.Instance.Player.CollisionTurnedIntoTrigger(this.collider);
            //playerIsTouching = true;
        }
    }
}
