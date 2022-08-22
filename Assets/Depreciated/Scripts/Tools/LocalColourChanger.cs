using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalColourChanger : MonoBehaviour
{
    public MeshRenderer mesh;

    public Color colour;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mesh.material.color = colour;
    }
}
