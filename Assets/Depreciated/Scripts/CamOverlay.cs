using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamOverlay : MonoBehaviour
{
    // public RenderTexture renderTexture;
    public Material cameraMat;
    public Camera backgroundCam;
    private bool seeThroughEnabled = false;

    private void Awake()
    {
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        // renderTexture.graphicsFormat = 
        renderTexture.useDynamicScale = true;
        renderTexture.Create();
        backgroundCam.targetTexture = renderTexture;
        cameraMat.SetTexture("blendTex", renderTexture);
    }


    //Need to interpolate smoothly therefor use update vs FixedUpdate for guaranteed but not smooth
    private void Update()
    {
        //Shoot raycast to player and enable adn disable based on that
        
        // Physics.Raycast()
    }


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cameraMat != null)
        {
            Graphics.Blit(src, dest, cameraMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}