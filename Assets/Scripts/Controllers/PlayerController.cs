#define DEBUG_JUMPS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MOVEMENT_SPEED = 10f;
    const float JUMP_CONTROL_MODIFIER = 0.1f;
    const float JUMP_POWER = 300f;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
}
