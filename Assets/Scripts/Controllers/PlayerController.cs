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
    //private List<Pickable> interactablesInRange = new List<Pickable>();

    [SerializeField]
    private bool grounded = false;

    private float currentInteractionDuration = 0f;

    private bool interactionLock = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Move(-1f);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Move(1f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
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

    }

    private void Move(float side)
    {
        if (!IsAtMaxSpeed())
        {
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
            Drop(carriedPickables[carriedPickables.Count - 1]);
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
            UIManager.Instance.ChangeInteractionProgress(currentInteractionDuration / pickable.TimeToPickup);
            UIManager.Instance.ChangeInteractionLabel("Picking up...");

            if (currentInteractionDuration >= pickable.TimeToPickup)
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
        UnityEngine.Debug.Log("Pickable in range");
#endif
        Pickable pickable = collider.GetComponentInParent<Pickable>();
        if (pickable != null)
        {
            if (!pickablesInRange.Contains(pickable))
            {
#if DEBUG_INTERACTION
                UnityEngine.Debug.Log("Add");
#endif
                pickablesInRange.Add(pickable);
                pickable.Highlight();
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
#if DEBUG_INTERACTION
        UnityEngine.Debug.Log("Pickable out of range");
#endif
        Pickable pickable = collider.GetComponentInParent<Pickable>();
        if (pickable != null)
        {
            if (pickablesInRange.Contains(pickable))
            {
                pickablesInRange.Remove(pickable);
                pickable.Unhighlight();
            }
        }
    }
}
