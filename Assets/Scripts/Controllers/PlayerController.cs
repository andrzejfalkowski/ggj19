//#define DEBUG_JUMPS
//#define DEBUG_INTERACTION

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
    private float longJumpAdditionalPower = 100f;
    [SerializeField]
    private float longJumpTimeThresholdStop = 0.2f;
    [SerializeField]
    private float longJumpTimeThresholdStart = 0.1f;
    [SerializeField]
    private float maxSpeed = 100f;
    [SerializeField]
    private float runAnimationMovementThreshold = 0.1f;

    private float timeOnJump = 0f;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private List<Pickable> carriedPickables = new List<Pickable>();
    const int carriedPickablesLimit = 1;

    private List<Pickable> pickablesInRange = new List<Pickable>();
    private List<WallSlot> wallSlotsInRange = new List<WallSlot>();

    [SerializeField]
    private bool grounded = false;

    public bool Grounded { get { return grounded; } }

    public bool IsFalling()
    {
        return (this.rigidbody != false
            && this.rigidbody.velocity.y < 0f);
    }

    private List<Collider2D> currentlyColliding = new List<Collider2D>();

    private bool jumping = false;
    private bool longJumping = false;
    private int moving = 0;

    private float currentInteractionDuration = 0f;

    private bool interactionLock = false;

    // Start is called before the first frame update
    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        moving = 0;
        moving += (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1 : 0;
        moving += (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1 : 0;

        if (!jumping)
        {
            jumping = (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W));
        }
        if (!grounded
            && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
        {
            timeOnJump += Time.deltaTime;
            if (timeOnJump > longJumpTimeThresholdStart
                && timeOnJump < longJumpTimeThresholdStop)
            {
                longJumping = true;
            }
        }
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
        SetAnimationValues();
    }

    private void SetAnimationValues()
    {
        animator.SetBool("Moving", (this.rigidbody.velocity.magnitude > runAnimationMovementThreshold));
        animator.SetBool("Falling", (this.rigidbody.velocity.y < 0));
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Interacting", IsInteracting());
        animator.SetBool("JumpTriggered", (jumping || longJumping) );
    }

    void FixedUpdate()
    {
        if (jumping || longJumping)
        {
            Jump();
            jumping = false;
            longJumping = false;
        }

        if (moving != 0)
        {
            Move(moving);
        }
    }

    private void DisableLongJump()
    {
        timeOnJump = longJumpTimeThresholdStop;
    }

    private void Move(float side)
    {
        this.rigidbody.velocity = new Vector2(Mathf.Clamp(this.rigidbody.velocity.x, -maxSpeed, maxSpeed), this.rigidbody.velocity.y);
        this.rigidbody.AddForce(new Vector2(side * movementSpeed * (grounded ? 1f : jumpControlModifier), 0));           
    }

    private void Jump()
    {
        if(grounded)
        {
            this.rigidbody.AddForce(new Vector2(0, jumpPower));
            timeOnJump = 0f;
        }
        else if (longJumping)
        {
            this.rigidbody.AddForce(new Vector2(0, longJumpAdditionalPower * Time.deltaTime));
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

    private bool IsInteracting()
    {
        return currentInteractionDuration > 0f;
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
        if (!currentlyColliding.Contains(collision.collider))
        {
            currentlyColliding.Add(collision.collider);
        }
        grounded = true;
        Debug.Log("enter collisions " + currentlyColliding.Count);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
#if DEBUG_JUMPS
        UnityEngine.Debug.Log("Stopped touching ground!");
#endif

        if (currentlyColliding.Contains(collision.collider))
        {
            currentlyColliding.Remove(collision.collider);
        }
        grounded = (currentlyColliding.Count != 0);
        Debug.Log("exit collisions " + currentlyColliding.Count);
    }

    public void CollisionTurnedIntoTrigger(Collider2D collider)
    {
        if (currentlyColliding.Contains(collider))
        {
            currentlyColliding.Remove(collider);
        }
        grounded = (currentlyColliding.Count != 0);
        Debug.Log("trigger collisions " + currentlyColliding.Count);
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
