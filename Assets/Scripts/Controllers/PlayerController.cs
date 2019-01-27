//#define DEBUG_JUMPS
//#define DEBUG_INTERACTION

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private float jumpingSpeedModifier = 0.7f;
    [SerializeField]
    private float maxSpeed = 100f;
    [SerializeField]
    private float runAnimationMovementThreshold = 0.1f;

    private float timeOnJump = 0f;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private float defaultGravity = 8f;

    private List<Pickable> carriedPickables = new List<Pickable>();
    const int carriedPickablesLimit = 1;

    private List<Pickable> pickablesInRange = new List<Pickable>();
    private List<WallSlot> wallSlotsInRange = new List<WallSlot>();

    [SerializeField]
    private SpriteRenderer inventorySprite;

    [SerializeField]
    private bool grounded = false;
    public bool Grounded { get { return grounded; } }

    [SerializeField]
    private bool inNest = false;
    [SerializeField]
    private bool isUnderwater = false;

    [SerializeField]
    private float drowningSkills = 0.1f;
    [SerializeField]
    private float breathingSkills = 0.1f;
    private float oxygen = 1f;
    public float Oxygen { get { return oxygen; } }

    [SerializeField]
    private Image interactionBar;
    [SerializeField]
    private TextMeshProUGUI interactionLabel;


    [SerializeField]
    private AudioSource interactionLoop;

    public bool IsFalling()
    {
        return (this.rigidbody != false
            && this.rigidbody.velocity.y < 0f);
    }

    private List<Collider2D> currentlyColliding = new List<Collider2D>();

    private bool jumping = false;
    private bool longJumping = false;
    private int moving = 0;

    private bool anyMovement = false;
    public bool AnyMovement { get { return anyMovement; } }

    private int swimming = 0;

    private float currentInteractionDuration = 0f;

    private bool interactionLock = false;
    // Start is called before the first frame update
    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponentInChildren<Animator>();

        defaultGravity = this.rigidbody.gravityScale;
    }

    void Update()
    {
        if (GameplayManager.Instance.CurrentPhase == EGameplayPhase.GameOver)
            return;

        moving = 0;
        moving += (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1 : 0;
        moving += (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1 : 0;

        if (!isUnderwater)
        {
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

            swimming = 0;
        }
        else
        {
            swimming = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ? 1 : 
                (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? -1 : 0;
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

        if (inNest)
        {
            HandleUnderwaterness(this.transform.position.y < GameplayManager.Instance.Waves.InternalWaterLevel());
        }
        else
        {
            HandleUnderwaterness(this.transform.position.y < GameplayManager.Instance.Waves.ExternalWaterLevel());
        }

        anyMovement = moving != 0f || swimming != 0f || jumping || longJumping;
    }

    void HandleUnderwaterness(bool underwater)
    {
        //UnityEngine.Debug.Log("HandleUnderwaterness " + underwater + "" + oxygen);
        isUnderwater = underwater;
        if (underwater)
        {
            oxygen = Mathf.Clamp01(oxygen - Time.deltaTime * drowningSkills);
            this.rigidbody.gravityScale = defaultGravity / 2f;

            if (oxygen <= 0f)
            {
                GameplayManager.Instance.GameOver();
            }
        }
        else
        {
            oxygen = Mathf.Clamp01(oxygen + Time.deltaTime * breathingSkills);
            this.rigidbody.gravityScale = defaultGravity;
        }
        UIManager.Instance.ChangeOxygenLevel(oxygen);       
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
        if (GameplayManager.Instance.CurrentPhase == EGameplayPhase.GameOver)
            return;

        if (jumping || longJumping)
        {
            Jump();
            jumping = false;
            longJumping = false;
        }

        if (swimming != 0)
        {
            Swim(swimming);
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
        var xVelocity = Mathf.Clamp(this.rigidbody.velocity.x, -maxSpeed, maxSpeed);
        if (!grounded)
        {
            xVelocity *= jumpingSpeedModifier;
        }
        this.rigidbody.velocity = new Vector2(xVelocity, this.rigidbody.velocity.y);
        this.rigidbody.AddForce(new Vector2(side * movementSpeed * (grounded ? 1f : jumpControlModifier), 0));           
    }

    private void Swim(float side)
    {
        float limit = maxSpeed;
        //if (Mathf.Abs(this.rigidbody.velocity.y) > Mathf.Abs(limit))
        {
            this.rigidbody.velocity = new Vector2(this.rigidbody.velocity.x, Mathf.Clamp(this.rigidbody.velocity.y, -limit, limit));
        }
        //else
        {
            this.rigidbody.AddForce(new Vector2(0f, side * jumpPower / 4f));
        }      
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
        interactionLoop.Stop();
        interactionLoop.clip = null;
        interactionLoop.volume = 0f;

        currentInteractionDuration = 0f;
        ChangeInteractionProgress(0f);
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
            ChangeInteractionProgress(currentInteractionDuration / pickable.TimeToInteract);
            ChangeInteractionLabel("Picking up...");

            if (currentInteractionDuration >= pickable.TimeToInteract)
            {
                AddToInventory(pickable);


                if (pickable.PickUpSound != null)
                    MusicManager.Instance.PlaySFX(pickable.PickUpSound);

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
        ChangeInteractionProgress(currentInteractionDuration / wallSlot.TimeToInteract);
        ChangeInteractionLabel("Building Wall...");

        //UnityEngine.Debug.Log((pickable.BuildSound != null));
        if (pickable.BuildSound != null)
        {
            interactionLoop.clip = pickable.BuildSound;
            //UnityEngine.Debug.Log(interactionLoop.clip.name);
            interactionLoop.volume = 1f;
            if(!interactionLoop.isPlaying)
                interactionLoop.Play();
        }
        

        if (currentInteractionDuration >= wallSlot.TimeToInteract)
        {
            RemoveFromInventory(pickable);

            wallSlot.BuildWall(pickable);

            GameObject.Destroy(pickable.gameObject);

            FinishInteraction();
        }
    }

    private void Drop(Pickable pickable)
    {
        if (carriedPickables.Contains(pickable))
        {
            RemoveFromInventory(pickable);

            pickable.transform.position = this.transform.position;
            pickable.gameObject.SetActive(true);

            if (pickable.PickUpSound != null)
                MusicManager.Instance.PlaySFX(pickable.DropSound);
        }

        if (!pickablesInRange.Contains(pickable))
        {
            pickablesInRange.Add(pickable);
            pickable.Highlight();
        }

        FinishInteraction();
    }

    void AddToInventory(Pickable pickable)
    {
        carriedPickables.Add(pickable);
        inventorySprite.sprite = pickable.Sprite;
        inventorySprite.gameObject.SetActive(true);
        this.rigidbody.mass += pickable.Weight;
    }

    void RemoveFromInventory(Pickable pickable)
    {
        carriedPickables.Remove(pickable);
        inventorySprite.gameObject.SetActive(false);
        inventorySprite.sprite = null;
        this.rigidbody.mass -= pickable.Weight;
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
    }

    public void CollisionTurnedIntoTrigger(Collider2D collider)
    {
        if (currentlyColliding.Contains(collider))
        {
            currentlyColliding.Remove(collider);
        }
        grounded = (currentlyColliding.Count != 0);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Collider in range");
#endif
        HighlightInteractable(collider);
        AddPickable(collider);
        AddWallSlot(collider);
        SetInNest(collider, true);
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
        SetInNest(collider, false);
    }

    private void SetInNest(Collider2D collider, bool inside)
    {
        WaveController nest = collider.GetComponentInParent<WaveController>();
        if (nest != null)
        {
            inNest = inside;
        }
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



    public void ChangeInteractionProgress(float value)
    {
        interactionBar.fillAmount = value;

        interactionBar.transform.parent.gameObject.SetActive(value > 0f && value < 1f);
    }

    public void ChangeInteractionLabel(string label)
    {
        interactionLabel.text = label;
    }


}
