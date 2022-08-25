using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using UnityEngine;


//fadetime
//positions (combined with scale float4)
//colours buffer (float4)
//numparticles

//compute shader that adds new positions based on graphics buffer


//normal dynamic points need to use Draw meshinstanced to keep sending TRS matrices for positions



public class PointChunkHolder
{
    //public Vector3[] points = new Vector3[ParticleManager.MAX_POINTS_IN_CHUNK];
    public Matrix4x4[] pointMats = new Matrix4x4[ParticleManager.MAX_POINTS_IN_CHUNK];
    //Dont know if I need this for colors or if I can just use a seperate material (might need for alpha blending with time)
    public MaterialPropertyBlock propBlock;
    public int counter;
    public readonly Color initColor;

    public PointChunkHolder(Color initColor)
    {
        this.initColor = initColor;
        propBlock = new MaterialPropertyBlock();
        //Size of array cant be mutated but the properties can so just ensure the index doesnt go over this
        var init = Enumerable.Repeat((Vector4)initColor, ParticleManager.MAX_POINTS_IN_CHUNK).ToArray();
        propBlock.SetVectorArray(ParticleManager.COLOR_SHADER_PROPERTY, init);
    }
}

public class DynamicPointChunkHolder : PointChunkHolder
{
    private readonly GameObject _gameObject;
    //need this to store the relative pos to the gameobject, TRS mat can be used later
    public Vector3[] points = new Vector3[ParticleManager.MAX_POINTS_IN_CHUNK];

    public DynamicPointChunkHolder(GameObject gameObject, Color initColor) : base(initColor)
    {
        _gameObject = gameObject;
    }

}


public class ParticleManager : MonoBehaviour
{
    [Header("variability")]
    public Mesh particleMesh;
    public Material particleMaterial;
    public float particleSize = 1f;
    private static Dictionary<GameObject, List<DynamicPointChunkHolder>> dynamicLocations;
    private static List<PointChunkHolder> staticLocations;

    public static ParticleManager instance;

    //private MaterialPropertyBlock block;
    public static readonly int COLOR_SHADER_PROPERTY = Shader.PropertyToID("_Colours");

    [Header("Testing variables")]
    public int testParticles = 10000;
    public float regionSize = 10f;

    public const int MAX_POINTS_IN_CHUNK = 1023;

    private void Awake()
    {
        instance = this;
        //Colors would include the alpha which can be interped with time
        staticLocations = new List<PointChunkHolder>();
        staticLocations.Add(new PointChunkHolder(Color.white));

        dynamicLocations = new Dictionary<GameObject, List<DynamicPointChunkHolder>>();
    }

    void Start()
    {
        //https://forum.unity.com/threads/drawmeshinstancedindirect-not-support-webgl.1285415/ 

#if UNITY_EDITOR
        SpawnTest(testParticles);
#endif

    }

    void Update()
    {
        //https://toqoz.fyi/thousands-of-meshes.html
        //Have to use Drawmeshinstanced instead of DrawMeshIndirect because compute shaders not supported in Webgl

        Vector3 scaleRef = Vector3.one * particleSize;
        foreach (var staticChunk in staticLocations)
        {
            //DO NOT USE LINQ HERE BECAUSE IT RUNS A LOT OF GC.ALLOC
            //var arr = staticChunk.points.Select((point => Matrix4x4.TRS(point, Quaternion.identity, scaleRef))).ToArray();
            Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, staticChunk.pointMats, staticChunk.pointMats.Length, staticChunk.propBlock);
        }


        foreach (var go in dynamicLocations.Keys)
        {
            var chunks = dynamicLocations[go];
            foreach (var chunk in chunks)
            {
                if (chunk.counter == 0) continue;
                //Scale should be initialised as 0
                for (int i = 0; i < chunk.counter; i++)
                {
                    var worldPos = go.transform.localToWorldMatrix.MultiplyPoint3x4(chunk.points[i]);
                    chunk.pointMats[i] = Matrix4x4.TRS(worldPos, Quaternion.identity, scaleRef);
                }
                Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, chunk.pointMats, chunk.pointMats.Length, chunk.propBlock);
            }
        }
    }


    public void SpawnTest(int toSpawn)
    {
        for (int i = 0; i < toSpawn; i++)
        {
            AddParticle(Random.insideUnitSphere * regionSize);
        }
    }

    void RunStaticPosShader()
    {

    }

/// <summary>
/// This is using the new compute shader version and is far faster
/// </summary>
/// <param name="locs"></param>
    public static void AddParticleGroup(Vector3[] locs)
    {

    }


    public static void AddParticle(Vector3 loc)
    {
        if (staticLocations.Last().counter >= MAX_POINTS_IN_CHUNK - 1)
        {
            staticLocations.Add(new PointChunkHolder(Color.white));
        }

        var lastChunk = staticLocations.Last();

        //lastChunk.points[lastChunk.counter] = loc;
        Vector3 scaleRef = Vector3.one * instance.particleSize;
        lastChunk.pointMats[lastChunk.counter] = Matrix4x4.TRS(loc, Quaternion.identity, scaleRef);

        lastChunk.counter++;
    }

    public static void AddParticleToGameObject(Vector3 loc, GameObject parent)
    {
        if (!dynamicLocations.ContainsKey(parent))
        {
            dynamicLocations[parent] = new List<DynamicPointChunkHolder>();
            dynamicLocations[parent].Add(new DynamicPointChunkHolder(parent, Color.blue));
        }

        if (dynamicLocations[parent].Last().counter >= MAX_POINTS_IN_CHUNK - 1)
        {
            dynamicLocations[parent].Add(new DynamicPointChunkHolder(parent, Color.blue));
        }
        var lastChunk = dynamicLocations[parent].Last();
        lastChunk.points[lastChunk.counter] = loc;
        lastChunk.counter++;
    }

}

public struct TimedVector
{
    public Vector3 position;
    public System.DateTime startTime;
}
