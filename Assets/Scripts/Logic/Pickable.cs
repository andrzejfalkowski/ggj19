using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField]
    private string name;
    [SerializeField]
    private EResourceType resourceType = EResourceType.Metal;
    [SerializeField]
    private float timeToPickUp = 0.3f;

    [SerializeField]
    private float value = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
