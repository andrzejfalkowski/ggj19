﻿#define DEBUG_JUMPS
#define DEBUG_INTERACTION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVEMENT_SPEED = 10f;
    const float JUMP_CONTROL_MODIFIER = 0.1f;
    const float JUMP_POWER = 400f;

    private List<Pickable> carriedPickables = new List<Pickable>();
    const int carriedPickablesLimit = 1;

    private List<Pickable> pickablesInRange = new List<Pickable>();

    [SerializeField]
    private bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(-1f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(1f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    private void Move(float side)
    {
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(side * MOVEMENT_SPEED * (grounded ? 1f : JUMP_CONTROL_MODIFIER), 0));
    }

    private void Jump()
    {
        if(grounded)
        {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, JUMP_POWER));
        }      
    }

    private void Interact()
    {
        if (pickablesInRange.Count > 0)
        {
            PickUp(pickablesInRange[pickablesInRange.Count - 1]);
        }
    }

    private void PickUp(Pickable pickable)
    {

#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Pickup");
#endif
        if (carriedPickables.Count < carriedPickablesLimit)
        {
            carriedPickables.Add(pickable);
            //TODO
        }

        if (pickablesInRange.Contains(pickable))
        {
            pickablesInRange.Remove(pickable);
        }

        pickable.gameObject.SetActive(false);
    }

    private void Drop(Pickable pickable)
    {
        if (carriedPickables.Contains(pickable))
        {
            carriedPickables.Remove(pickable);
            //TODO
        }

        if (!pickablesInRange.Contains(pickable))
        {
            pickablesInRange.Add(pickable);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
#if DEBUG_JUMPS
        UnityEngine.Debug.Log("Touched ground!");
#endif
        grounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
#if DEBUG_JUMPS
        UnityEngine.Debug.Log("Stopped touching ground!");
#endif
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Pickable in range");
#endif
        Pickable pickable = collider.GetComponentInChildren<Pickable>();
        if (pickable != null)
        {
            if (!pickablesInRange.Contains(pickable))
            {
#if DEBUG_INTERACTION
                UnityEngine.Debug.Log("Add");
#endif
                pickablesInRange.Add(pickable);
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Pickable out of range");
#endif
        Pickable pickable = collider.GetComponentInChildren<Pickable>();
        if (pickable != null)
        {
            if (!pickablesInRange.Contains(pickable))
            {
                pickablesInRange.Add(pickable);
            }
        }
    }


}