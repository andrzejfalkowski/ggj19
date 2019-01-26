using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOrientation
{
    Left,
    Right,
}

[RequireComponent(typeof(Rigidbody2D))]
public class OrientationSetter : MonoBehaviour
{
    private static Vector3 NormalOrientationScale = new Vector3(1, 1, 1);
    private static Vector3 InverseOrientationScale = new Vector3(-1, 1, 1);

    [SerializeField]
    private EOrientation orientation;
    [SerializeField]
    private float orientationChangeSpeedThreshold = 0.1f;

    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.magnitude == 0)
        {
            return;
        }
        var movesToRight = (rigidbody.velocity.x >= 0);
        var isNormallyOrientedToRight = (orientation == EOrientation.Right);
        if (movesToRight == isNormallyOrientedToRight)
        {
            this.transform.localScale = NormalOrientationScale;
        }
        else
        {
            this.transform.localScale = InverseOrientationScale;
        }
    }
}
