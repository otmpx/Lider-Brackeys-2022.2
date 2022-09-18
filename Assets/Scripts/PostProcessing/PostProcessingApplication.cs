using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostProcessingApplication : MonoBehaviour
{
    public Shader shader;
    protected Material material;

    Camera cam;
    protected virtual void Awake()
    {
        cam = GetComponent<Camera>();
        if (material == null)
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material, 0);
    }
}
