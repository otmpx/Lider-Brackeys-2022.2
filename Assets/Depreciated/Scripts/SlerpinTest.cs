using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpinTest : MonoBehaviour
{
    public Transform p1, p2, center;
    public float t;
    public Mesh debugMesh;
    private void Update()
    {
        transform.position = KongrooUtils.SlerpCenter(p1.position, p2.position, center.position, t);
    }
}