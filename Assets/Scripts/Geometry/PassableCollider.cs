﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PassableCollider : MonoBehaviour
{
    private BoxCollider2D collider;
    private bool playerIsTouching;

    private void Awake()
    {
        collider = this.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (GameplayManager.Instance == null)
        {
            return;
        }
        if (IsMidAirAndAscending()
            || Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S))
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
        return !GameplayManager.Instance.Player.Grounded
            && !GameplayManager.Instance.Player.IsFalling()
            && !playerIsTouching;
    }

    private void SetPassable(bool passable)
    {
        if (passable == collider.isTrigger)
        {
            return;
        }
        Debug.Log("passable " + passable);
        collider.isTrigger = passable;
        if (passable)
        {
            GameplayManager.Instance.Player.CollisionTurnedIntoTrigger(this.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerController>() != null)
        {
            playerIsTouching = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerController>() != null)
        {
            playerIsTouching = false;
        }
    }
}
