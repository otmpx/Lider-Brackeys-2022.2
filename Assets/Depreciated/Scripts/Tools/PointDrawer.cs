using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    public Vector3[] roomPoints =
    {
        new Vector3(97500.00f, 34000.00f, 2500.00f),
        new Vector3(85647.67f, 43193.61f, 2500.00f),
        new Vector3(91776.75f, 51095.16f, 2500.00f),
        new Vector3(103629.07f, 41901.55f, 2500.00f)
    };

    public Vector3[] newpoints =
    {
        new Vector3(97056.875f, 37507.656f, 2500f),
        new Vector3(98589.14f, 39483.043f, 2500f),
        new Vector3(100121.41f, 41458.43f, 2500f),
        new Vector3(95081.484f, 39039.926f, 2500f),
        new Vector3(96613.75f, 41015.312f, 2500f),
        new Vector3(98146.016f, 42990.7f, 2500f),
        new Vector3(93106.1f, 40572.19f, 2500f),
        new Vector3(94638.37f, 42547.58f, 2500f),
        new Vector3(96170.64f, 44522.965f, 2500f),
        new Vector3(91130.71f, 42104.46f, 2500f),
        new Vector3(92662.98f, 44079.848f, 2500f),
        new Vector3(94195.25f, 46055.234f, 2500f),
        new Vector3(89155.33f, 43636.727f, 2500f),
        new Vector3(90687.59f, 45612.113f, 2500f),
        new Vector3(92219.86f, 47587.5f, 2500f),
    };


    public float divisor = 1000f;
    public float pointRad = 3f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (var point in roomPoints)
        {
            Gizmos.DrawSphere(point / divisor, pointRad);
        }

        Gizmos.color = Color.red;
        foreach (var point in newpoints)
        {
            Gizmos.DrawSphere(point / divisor, pointRad);
        }
    }
}