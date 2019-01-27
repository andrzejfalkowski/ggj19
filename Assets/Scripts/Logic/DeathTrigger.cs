using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameplayManager.Instance.GameOver();
    }
}
