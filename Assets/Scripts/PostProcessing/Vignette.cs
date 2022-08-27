using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vignette : PostProcessingApplication
{
    public static Vignette i;

    //[Range(0f, 1f)]
    public float falloff = .5f;
    [Range(-0.5f, 1f)]
    public float power = .25f;
    public float maxPower = 0.25f;
    public float minPower = -0.25f;
    protected override void Awake()
    {
        base.Awake();
        i = this;
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_Falloff", falloff);
        material.SetFloat("_Power", power);
        base.OnRenderImage(source, destination);
    }
}