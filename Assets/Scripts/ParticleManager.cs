using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointChunkHolderGeneric<T>
{
    public List<T> points;
    public MaterialPropertyBlock propBlock;

    public PointChunkHolderGeneric()
    {
        points = new List<T>();
        propBlock = new MaterialPropertyBlock();
        //Size of array cant be mutated but the properties can so just ensure the index doesnt go over this
        var init = Enumerable.Repeat((Vector4)Color.white, ParticleManager.MAX_POINTS_IN_CHUNK).ToArray();
        propBlock.SetVectorArray(ParticleManager.COLOR_SHADER_PROPERTY, init);
    }
}
public class PointChunkHolder
{
    public Vector2[] points = new Vector2[ParticleManager.MAX_POINTS_IN_CHUNK];
    public MaterialPropertyBlock propBlock;

    public PointChunkHolder()
    {
        propBlock = new MaterialPropertyBlock();
        //Size of array cant be mutated but the properties can so just ensure the index doesnt go over this
        var init = Enumerable.Repeat((Vector4)Color.white, ParticleManager.MAX_POINTS_IN_CHUNK).ToArray();
        propBlock.SetVectorArray(ParticleManager.COLOR_SHADER_PROPERTY, init);
    }
}

public class ParticleManager : MonoBehaviour
{
    [Header("variability")]
    public Mesh particleMesh;
    public Material particleMaterial;
    public float particleSize = 1f;
    private static Dictionary<GameObject, PointChunkHolderGeneric<Vector3>> dynamicLocations;
    private static Dictionary<GameObject, PointChunkHolderGeneric<TimedVector>> enemyLocations;
    private static List<PointChunkHolderGeneric<Vector3>> staticLocations;

    public static ParticleManager instance;

    //private MaterialPropertyBlock block;
    public static readonly int COLOR_SHADER_PROPERTY = Shader.PropertyToID("_Colours");

    [Header("Testing variables")]
    public int testParticles = 10000;
    public float regionSize = 10f;

    public const int MAX_POINTS_IN_CHUNK = 100;

    private void Awake()
    {
        instance = this;
        //Colors would include the alpha which can be interped with time
        staticLocations = new List<PointChunkHolderGeneric<Vector3>>();
        staticLocations.Add(new PointChunkHolderGeneric<Vector3>());
    }

    void Start()
    {
        //https://forum.unity.com/threads/drawmeshinstancedindirect-not-support-webgl.1285415/ 
        SpawnTest();

    }

    // Update is called once per frame
    void Update()
    {
        //https://toqoz.fyi/thousands-of-meshes.html
        //Have to use Drawmeshinstanced instead of DrawMeshIndirect because compute shaders not supported in Webgl
        //DrawMeshInstanceIndirect is generally better and faster but it uses compute buffers which dont exist for webgl 

        //Draw all the static particles
        Vector3 scaleRef = Vector3.one * particleSize;
        //block.SetVectorArray(colourShaderProperty, colourArr);
        foreach (var staticChunk in staticLocations)
        {
            if (staticChunk.points.Count == 0) continue;
            //Easier to store and Serialize stuff in vectors than matrix(decide which is more memory and perforamnce efficient)
            var arr = staticChunk.points.Select((point => Matrix4x4.TRS(point, Quaternion.identity, scaleRef))).ToList();
            //var blockColorArr = staticChunk.points.Select(l => (Vector4)Color.white).ToArray();
            //staticChunk.propBlock.SetVectorArray(COLOR_SHADER_PROPERTY, blockColorArr);

            Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, arr, staticChunk.propBlock);
        }


        //Draw and remove expired timedParticles on Enemies

    }

    public void SpawnTest()
    {

        for (int i = 0; i < testParticles; i++)
        {
            AddParticle(Random.insideUnitSphere * regionSize);
        }

    }

    public static void AddParticle(Vector3 loc)
    {
        if (staticLocations.Last().points.Count >= MAX_POINTS_IN_CHUNK)
        {
            staticLocations.Add(new PointChunkHolderGeneric<Vector3>());
        }

        staticLocations.Last().points.Add(loc);
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
