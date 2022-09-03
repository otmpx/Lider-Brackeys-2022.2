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
    private readonly Transform _gameObject;
    //need this to store the relative pos to the gameobject, TRS mat can be used later
    public Vector3[] points = new Vector3[ParticleManager.MAX_POINTS_IN_CHUNK];

    public DynamicPointChunkHolder(Transform gameObject, Color initColor) : base(initColor)
    {
        _gameObject = gameObject;
        var init = Enumerable.Repeat(0f, ParticleManager.MAX_POINTS_IN_CHUNK).ToArray();
        //Everythign startsout with lifetime of 0 i.e infinite
        propBlock.SetFloatArray(ParticleManager.TIME_SHADER_PROPERTY, init);
    }

}

//Esnure the struct is declared in the same order with the same types EVERYWHERE
public struct StaticPointDef
{
    public Vector4 posScale;
    public Vector4 color;
}

public enum PointType
{
    Static,
    Dynamic,
    Enemy,
    Objective
}

public class ParticleManager : MonoBehaviour
{
    [Header("variability")]
    public Mesh particleMesh;
    public Material instancedMaterial;
    public Material indirectMaterial;
    public int shotsPerInterval = 5000;

    public float particleSize = 1f;
    private static Dictionary<Transform, List<DynamicPointChunkHolder>> dynamicLocations;
    private static List<PointChunkHolder> staticLocations;

    public static ParticleManager instance;

    public static readonly int COLOR_SHADER_PROPERTY = Shader.PropertyToID("_Colours");
    public static readonly int TIME_SHADER_PROPERTY = Shader.PropertyToID("_Times");

    [Header("Testing variables")]
    public int testBunches = 10000;
    public float regionSize = 10f;

    public const int MAX_POINTS_IN_CHUNK = 1023;
    //public const int MAX_POINTS_IN_BUFFER = 10_000_000; //Hehe big number
    //public const int MAX_POINTS_IN_BUFFER = 16777216; //OR 8 388 608
    //public const int MAX_POINTS_IN_BUFFER = 4_193_856;
    //public const int MAX_POINTS_IN_BUFFER = 1_000_000; //Hehe big number

    public const int BASE_POINTS_IN_BUFFER = 4_096;
    //public const int MAX_POINTS_IN_BUFFER = 65536;
    public int MAX_POINTS = 65536;
    public static int MAX_POINTS_IN_BUFFER => instance.MAX_POINTS;
    //public readonly static int MAX_POINTS_IN_BUFFER = (int)Mathf.Pow(BASE_POINTS_IN_BUFFER, 2);

    // Dispatch number of thread groups cant go over 65535, so may need to use array unpacking for indexing


    //https://gamedev.stackexchange.com/questions/194012/difference-between-computebuffer-and-graphicsbuffer-in-unity#:~:text=A%20GraphicsBuffer%20can%20be%20used,mostly%20needed%20for%20compute%20shaders.&text=Don't%20forget%20to%20mark,if%20it%20solved%20your%20problem.

    [Header("Compute shader method vars")]
    public ComputeShader pointsCompute;

    public ComputeBuffer staticPointsBuffer;
    public ComputeBuffer newPointsBuffer;
    public ComputeBuffer argsBuffer;

    const int ADD_POINTS_KERNEL = 0;

    public int currentTotalPoints;

    private static readonly int NUM_POINTS_TO_ADD_KEY = Shader.PropertyToID("numPoints2Add");
    private static readonly int CURRENT_POINTS_KEY = Shader.PropertyToID("currPointCount");
    private static readonly int NEW_ADD_BUFFER_KEY = Shader.PropertyToID("newAdds");
    private static readonly int STATIC_POINTS_BUFFER_KEY = Shader.PropertyToID("particles");
    private static readonly int DIMENSION_SIZE_KEY = Shader.PropertyToID("dimensionSize");

    private void Awake()
    {
        instance = this;
        //Colors would include the alpha which can be interped with time
        staticLocations = new List<PointChunkHolder>();
        staticLocations.Add(new PointChunkHolder(Color.white));

        dynamicLocations = new Dictionary<Transform, List<DynamicPointChunkHolder>>();
        currentTotalPoints = 0;
    }


    void Start()
    {
        //https://forum.unity.com/threads/drawmeshinstancedindirect-not-support-webgl.1285415/ 
        InitComputeShader();

#if UNITY_EDITOR
        //SpawnTest(testBunches);
#endif
    }

    void InitComputeShader()
    {
        staticPointsBuffer = GenerateBuffer<Vector4>(MAX_POINTS_IN_BUFFER);
        newPointsBuffer = GenerateBuffer<Vector4>(shotsPerInterval);

        pointsCompute.SetBuffer(ADD_POINTS_KERNEL, STATIC_POINTS_BUFFER_KEY, staticPointsBuffer);

        pointsCompute.SetBuffer(ADD_POINTS_KERNEL, NEW_ADD_BUFFER_KEY, newPointsBuffer);

        // Create args buffer
        uint[] args = new uint[5];
        args[0] = (uint)particleMesh.GetIndexCount(0);
        args[1] = (uint)MAX_POINTS_IN_BUFFER;
        args[2] = (uint)particleMesh.GetIndexStart(0);
        args[3] = (uint)particleMesh.GetBaseVertex(0);
        args[4] = 0; // offset

        //StaticPointDef[] particles = new StaticPointDef[MAX_POINTS_IN_BUFFER];
        //for (int i = 0; i < MAX_POINTS_IN_BUFFER; i++)
        //{
        //    particles[i] = new StaticPointDef() { posScale = Vector4.zero, color = Color.white };
        //}

        //staticPointsBuffer.SetData(particles);

        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        //indirectMaterial.SetBuffer("particles", staticPointsBuffer);
        indirectMaterial.SetBuffer("points", staticPointsBuffer);
    }

    void Update()
    {

        Vector3 scaleRef = Vector3.one * particleSize;

        DrawStaticPoints();

        foreach (var trans in dynamicLocations.Keys)
        {
            var chunks = dynamicLocations[trans];
            foreach (var chunk in chunks)
            {
                if (chunk.counter == 0) continue;
                //Scale should be initialised as 0
                for (int i = 0; i < chunk.counter; i++)
                {
                    var worldPos = trans.localToWorldMatrix.MultiplyPoint3x4(chunk.points[i]);
                    chunk.pointMats[i] = Matrix4x4.TRS(worldPos, Quaternion.identity, scaleRef);
                }
                Graphics.DrawMeshInstanced(particleMesh, 0, instancedMaterial, chunk.pointMats, chunk.pointMats.Length, chunk.propBlock);
            }
        }
    }

    private void DrawStaticPoints()
    {
        //https://toqoz.fyi/thousands-of-meshes.html
        //Have to use Drawmeshinstanced instead of DrawMeshIndirect because compute shaders not supported in Webgl


        //Vector3 scaleRef = Vector3.one * particleSize;
        //foreach (var staticChunk in staticLocations)
        //{
        //    //DO NOT USE LINQ HERE BECAUSE IT RUNS A LOT OF GC.ALLOC
        //    //var arr = staticChunk.points.Select((point => Matrix4x4.TRS(point, Quaternion.identity, scaleRef))).ToArray();
        //}

        //Graphics.DrawMeshInstancedIndirect(particleMesh, 0, indirectMaterial, new Bounds(Vector3.zero, Vector3.one * 100), argsBuffer);


    }

    private void OnRenderObject()
    {
        indirectMaterial.SetPass(0);
        indirectMaterial.SetBuffer("points", staticPointsBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points, MAX_POINTS_IN_BUFFER, 1);

    }

    private void OnDestroy()
    {
        ReleaseBuffer(staticPointsBuffer, newPointsBuffer, argsBuffer);
    }

    public static Vector3Int GetThreadGroupSizes(ComputeShader cs, int kernelIndex = 0)
    {
        uint x, y, z;
        cs.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }

    public static void DispatchSafe(ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
    {
        Vector3Int threadGroupSizes = GetThreadGroupSizes(cs, kernelIndex);
        var res = threadGroupSizes.x % numIterationsX;
#if UNITY_EDITOR
        if (numIterationsX % threadGroupSizes.x != 0 ||
             numIterationsY % threadGroupSizes.y != 0 ||
             numIterationsZ % threadGroupSizes.z != 0)
            throw new System.Exception("Hey man your iterations isnt divisible by the thread");
#endif

        int numGroupsX = numIterationsX / threadGroupSizes.x;
        int numGroupsY = numIterationsY / threadGroupSizes.y;
        int numGroupsZ = numIterationsZ / threadGroupSizes.z;

        cs.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
    }


    public static void Dispatch(ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
    {
        Vector3Int threadGroupSizes = GetThreadGroupSizes(cs, kernelIndex);
        int numGroupsX = Mathf.CeilToInt(numIterationsX / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(numIterationsY / (float)threadGroupSizes.y);
        int numGroupsZ = Mathf.CeilToInt(numIterationsZ / (float)threadGroupSizes.z);
        cs.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
    }



    public void SpawnTest(int toSpawn)
    {
        //for (int i = 0; i < toSpawn; i++)
        //{
        //    AddParticle(Random.insideUnitSphere * regionSize);
        //}


        //for (int j = 0; j < toSpawn; j++)
        //{
        //    //StaticPointDef[] pointsToAdd = new StaticPointDef[ParticleManager.instance.shotsPerInterval];
        //    Vector4[] pointsToAdd = new Vector4[ParticleManager.instance.shotsPerInterval];
        //    for (int i = 0; i < ParticleManager.instance.shotsPerInterval; i++)
        //    {
        //        pointsToAdd[i] = ParticleManager.GetPointDef(Random.insideUnitSphere * regionSize, PointType.Static);
        //    }

        //    AddParticleGroup(pointsToAdd);

        //}


        Vector4[] pointsToAdd = new Vector4[ParticleManager.instance.MAX_POINTS];
        for (int i = 0; i < ParticleManager.instance.MAX_POINTS; i++)
        {
            pointsToAdd[i] = ParticleManager.GetPointDef(Random.insideUnitSphere * regionSize, PointType.Static);
        }

        staticPointsBuffer.SetData(pointsToAdd);


    }


    /// <summary>
    /// This is using the new compute shader version and is far faster
    /// </summary>
    /// <param name="locs"></param>
    public static void AddParticleGroup(Vector4[] newPoints)
    {
        var cs = instance.pointsCompute;

        cs.SetInt(CURRENT_POINTS_KEY, instance.currentTotalPoints);
        cs.SetInt(NUM_POINTS_TO_ADD_KEY, newPoints.Length);
        //cs.SetInt(DIMENSION_SIZE_KEY, BASE_POINTS_IN_BUFFER);
        instance.newPointsBuffer.SetData(newPoints);

        DispatchSafe(cs, MAX_POINTS_IN_BUFFER);

        //DispatchSafe(cs, BASE_POINTS_IN_BUFFER, BASE_POINTS_IN_BUFFER, 1, ADD_POINTS_KERNEL);

        //int 

        //Vector3Int threadGroupSizes = GetThreadGroupSizes(cs, ADD_POINTS_KERNEL);
        ////#if UNITY_EDITOR
        ////        if (numIterationsX % threadGroupSizes.x != 0 ||
        ////             numIterationsY % threadGroupSizes.y != 0 ||
        ////             numIterationsZ % threadGroupSizes.z != 0)
        ////            throw new System.Exception("Hey man your iterations isnt divisible by the thread");
        ////#endif

        //int numGroupsX = numIterationsX / threadGroupSizes.x;
        //int numGroupsY = numIterationsY / threadGroupSizes.y;
        //int numGroupsZ = numIterationsZ / threadGroupSizes.y;

        //cs.Dispatch(ADD_POINTS_KERNEL, numGroupsX, numGroupsY, numGroupsZ);


        //Ensure that this doesnt just randomly get offset (might just skip points if it does) (compute buffer doesnt run)
        instance.currentTotalPoints += newPoints.Length;
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

    public static void AddParticleToGameObject(Vector3 loc, Transform parent, PointType type)
    {
        if (!dynamicLocations.ContainsKey(parent))
        {
            dynamicLocations[parent] = new List<DynamicPointChunkHolder>();
            dynamicLocations[parent].Add(new DynamicPointChunkHolder(parent, type.ToColor()));
        }

        if (dynamicLocations[parent].Last().counter >= MAX_POINTS_IN_CHUNK - 1)
        {
            dynamicLocations[parent].Add(new DynamicPointChunkHolder(parent, type.ToColor()));
        }
        var lastChunk = dynamicLocations[parent].Last();
        lastChunk.points[lastChunk.counter] = loc;
        lastChunk.counter++;
    }

    public static void RemoveDynamicGO(Transform parent)
    {
        var recursiveChildren = parent.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in recursiveChildren)
        {
            dynamicLocations.Remove(child);
        }
    }
    public static int GetTotalPointsOnObjects(Transform[] objs)
    {

        int sumPoints = 0;
        foreach (var currentTrans in objs)
        {
            if (dynamicLocations.TryGetValue(currentTrans, out var chunkHolderList))
            {
                sumPoints += chunkHolderList.Sum(chunkHolder => chunkHolder.counter);
            }
        }
        return sumPoints;
    }

    //public static StaticPointDef GetPointDef(Vector3 loc, PointType type) =>
    //    new StaticPointDef { posScale = new Vector4(loc.x, loc.y, loc.z, instance.particleSize), color = (Vector4)type.ToColor() };

    public static Vector4 GetPointDef(Vector3 loc, PointType type) => new Vector4(loc.x, loc.y, loc.z, instance.particleSize);

    static ComputeBuffer GenerateBuffer<T>(int size)
    {
        var floatsize = sizeof(float);
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        return new ComputeBuffer(size, stride);
    }

    public static void ReleaseBuffer(params ComputeBuffer[] buffers)
    {
        for (int i = 0; i < buffers.Length; i++)
        {
            if (buffers[i] != null)
            {
                buffers[i].Release();
            }
        }
    }

}

public struct TimedVector
{
    public Vector3 position;
    public System.DateTime startTime;
}
