using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameplayManager Instance = null;

    public PlayerController Player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (Player == null)
            {
                Player = FindObjectOfType<PlayerController>();
            }
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
