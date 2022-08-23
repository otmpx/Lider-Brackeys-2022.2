using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Header("variability")]
    public Mesh particleMesh;
    public Material particleMaterial;
    public float particleSize = 1f;
    private static Dictionary<GameObject, List<Vector3>> dynamicLocations;
    private static Dictionary<GameObject, List<TimedVector>> enemyLocations;
    private static List<List<Vector3>> staticLocations;

    public static ParticleManager instance;

    private MaterialPropertyBlock block;
    private int colourShaderProperty;

    [Header("Testing variables")]
    public int testParticles = 10000;
    public float regionSize = 10f;

    private void Awake()
    {
        instance = this;
        //Colors would include the alpha which can be interped with time
        colourShaderProperty = Shader.PropertyToID("_ParticleColours");
        block = new MaterialPropertyBlock();
    }

    void Start()
    {
        //https://forum.unity.com/threads/drawmeshinstancedindirect-not-support-webgl.1285415/ 

    }

    // Update is called once per frame
    void Update()
    {
        //https://toqoz.fyi/thousands-of-meshes.html
        //Have to use Drawmeshinstanced instead of DrawMeshIndirect because compute shaders not supported in Webgl
        //DrawMeshInstanceIndirect is generally better and faster but it uses compute buffers which dont exist for webgl 

        //Draw all the static particles
        Vector3 scaleRef = Vector3.one * particleSize;
        var colourArr = staticLocations.SelectMany(loc => loc).Select(l => Color.white).Cast<Vector4>().ToList();
        block.SetVectorArray(colourShaderProperty, colourArr);
        foreach (var staticChunk in staticLocations)
        {
            //Easier to store and Serialize stuff in vectors than matrix(decide which is more memory and perforamnce efficient)
            var arr = staticChunk.Select((point => Matrix4x4.TRS(point, Quaternion.identity, scaleRef))).ToList();
            Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, arr, block);
        }


        //Draw and remove expired timedParticles on Enemies


    }

    public static void AddParticle(Vector3 loc)
    {


    }

    public static void AddParticleToGameObject(Vector3 loc, GameObject parent)
    {


    }

    public static void AddFadingParticleToGameObject(Vector3 loc, GameObject parent)
    {

    }

}

public struct TimedVector
{
    public Vector3 position;
    public System.DateTime startTime;
}
