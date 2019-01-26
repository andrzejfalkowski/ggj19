#define DEBUG_JUMPS
#define DEBUG_INTERACTION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 10f;
    [SerializeField]
    private float jumpControlModifier = 0.1f;
    [SerializeField]
    private float jumpPower = 400f;
    [SerializeField]
    private float maxSpeed = 100f;

    private Rigidbody2D rigidbody;

    private List<Pickable> carriedPickables = new List<Pickable>();
    const int carriedPickablesLimit = 1;

    private List<Pickable> pickablesInRange = new List<Pickable>();
    private List<WallSlot> wallSlotsInRange = new List<WallSlot>();

    [SerializeField]
    private bool grounded = false;

    private bool jumping = false;
    private int moving = 0;

    private float currentInteractionDuration = 0f;

    private bool interactionLock = false;

    // Start is called before the first frame update
    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moving = 0;
        moving += (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1 : 0;
        moving += (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1 : 0;

        if(!jumping)
            jumping = (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W));

        if (!interactionLock)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Interact();
            }
            else
            {
                StopInteraction();
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                interactionLock = false;
            }
        }
    }


    void FixedUpdate()
    {
        if (jumping)
        {
            Jump();
            jumping = false;
        }

        if (moving != 0)
        {
            Move(moving);
        }
    }

    private void Move(float side)
    {
        if (!IsAtMaxSpeed())
        {
            this.rigidbody.velocity = new Vector2(Mathf.Clamp(this.rigidbody.velocity.x, -maxSpeed, maxSpeed), this.rigidbody.velocity.y);
            this.rigidbody.AddForce(new Vector2(side * movementSpeed * (grounded ? 1f : jumpControlModifier), 0));           
        }
    }

    private bool IsAtMaxSpeed()
    {
        return false;
    }

    private void Jump()
    {
        if(grounded)
        {
            this.rigidbody.AddForce(new Vector2(0, jumpPower));
        }      
    }

    private void Interact()
    {
        if (pickablesInRange.Count > 0)
        {
            PickUp(pickablesInRange[pickablesInRange.Count - 1]);
        }
        else if(carriedPickables.Count > 0)
        {
            if (wallSlotsInRange.Count > 0)
            {
                BuildWall(
                    wallSlotsInRange[wallSlotsInRange.Count - 1],
                    carriedPickables[carriedPickables.Count - 1]
                );
            }
            else
            {
                Drop(carriedPickables[carriedPickables.Count - 1]);
            }
        }
    }

    private void StopInteraction()
    {
        currentInteractionDuration = 0f;
        UIManager.Instance.ChangeInteractionProgress(0f);
    }

    private void FinishInteraction()
    {
        StopInteraction();
        interactionLock = true;
    }

    private void PickUp(Pickable pickable)
    {
        
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Pickup");
#endif
        if (carriedPickables.Count < carriedPickablesLimit)
        {
            currentInteractionDuration += Time.deltaTime;
            UIManager.Instance.ChangeInteractionProgress(currentInteractionDuration / pickable.TimeToInteract);
            UIManager.Instance.ChangeInteractionLabel("Picking up...");

            if (currentInteractionDuration >= pickable.TimeToInteract)
            {
                carriedPickables.Add(pickable);

                if (pickablesInRange.Contains(pickable))
                {
                    pickablesInRange.Remove(pickable);
                    pickable.Unhighlight();
                }

                pickable.gameObject.SetActive(false);

                FinishInteraction();
            }
            //TODO
        }
    }

    private void BuildWall(WallSlot wallSlot, Pickable pickable)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Install in wall");
#endif
        currentInteractionDuration += Time.deltaTime;
        UIManager.Instance.ChangeInteractionProgress(currentInteractionDuration / wallSlot.TimeToInteract);
        UIManager.Instance.ChangeInteractionLabel("BuildingWall...");

        if (currentInteractionDuration >= wallSlot.TimeToInteract)
        {
            carriedPickables.Remove(pickable);

            wallSlot.BuildWall(pickable);

            GameObject.Destroy(pickable.gameObject);

            FinishInteraction();
        }
    }

    private void Drop(Pickable pickable)
    {
        if (carriedPickables.Contains(pickable))
        {
            carriedPickables.Remove(pickable);

            pickable.transform.position = this.transform.position;
            pickable.gameObject.SetActive(true);
        }

        if (!pickablesInRange.Contains(pickable))
        {
            pickablesInRange.Add(pickable);
            pickable.Highlight();
        }

        FinishInteraction();
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
        UnityEngine.Debug.Log("Collider in range");
#endif
        HighlightInteractable(collider);
        AddPickable(collider);
        AddWallSlot(collider);

    }

    private void HighlightInteractable(Collider2D collider)
    {
        Interactable interactable = collider.GetComponentInParent<Interactable>();
        if (interactable != null)
        {
            interactable.Highlight();
        }
    }

    private void UnhighlightInteractable(Collider2D collider)
    {
        Interactable interactable = collider.GetComponentInParent<Interactable>();
        if (interactable != null)
        {
            interactable.Unhighlight();
        }
    }

    private void AddPickable(Collider2D collider)
    {
        Pickable pickable = collider.GetComponentInParent<Pickable>();
        if (pickable != null
            && !pickablesInRange.Contains(pickable))
        {
#if DEBUG_INTERACTION
                UnityEngine.Debug.Log("Add pickable");
#endif
                pickablesInRange.Add(pickable);
        }
    }

    private void AddWallSlot(Collider2D collider)
    {
        WallSlot wallSlot = collider.GetComponentInParent<WallSlot>();
        if (wallSlot != null
            && !wallSlotsInRange.Contains(wallSlot))
        {
#if DEBUG_INTERACTION
            UnityEngine.Debug.Log("Add wall slot");
#endif
            wallSlotsInRange.Add(wallSlot);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Collider out of range");
#endif
        UnhighlightInteractable(collider);
        RemovePickable(collider);
        RemoveWallSlot(collider);
    }
    
    private void RemovePickable(Collider2D collider)
    {
        Pickable pickable = collider.GetComponentInParent<Pickable>();
        if (pickable != null
            && pickablesInRange.Contains(pickable))
        {
            pickablesInRange.Remove(pickable);
        }
    }

    private void RemoveWallSlot(Collider2D collider)
    {
        WallSlot wallSlot = collider.GetComponentInParent<WallSlot>();
        if (wallSlot != null
            && wallSlotsInRange.Contains(wallSlot))
        {
            wallSlotsInRange.Remove(wallSlot);
        }
    }
}
