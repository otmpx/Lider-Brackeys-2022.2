using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

public class Player : Actor
{
    public float moveSpeed;
    public Transform vCamFollow;
    public CinemachineVirtualCamera deathCam;
    public float shootRange = 2.5f;
    public LayerMask scannable;
    [Tooltip("This should only be one selection")]
    public LayerMask dynamicObjectMask;

    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool disableShooting = false;
    public static event System.Action fireEvent;

    //[HideInInspector] public CinemachineBasicMultiChannelPerlin headBob;
    Camera cam;

    public static Player Instance;

    [HideInInspector] public float lastFired = 0f;
    public float FIRE_RATE_INTERVAL = 0.1f;
    const float MAX_RAYCAST_DIST = 1000f;

    private Vector3[] lastShotLocs;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        rb = GetComponent<Rigidbody>();
        LevelDirector.instance.vCam.Follow = vCamFollow;
        Cursor.lockState = CursorLockMode.Locked;
        cam = LevelDirector.instance.cam;
    }
    protected override void Start()
    {
        base.Start();
        currentState = new Idle(this);
        currentState.OnEnter();
    }
    protected override void Update()
    {
        base.Update();
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDir.magnitude > 1e-7)
            moveDir = transform.TransformDirection(inputDir).normalized;
        else
            moveDir = Vector3.zero;
        if (disableShooting)
            isShooting = false;
        else
            isShooting = Input.GetKey(KeyCode.Mouse0);

        transform.localEulerAngles = new Vector3(0, cam.transform.localEulerAngles.y, 0);
        if (isShooting && !LevelDirector.instance.paused)
            Fire();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ParticleManager.instance.SpawnTest(100000);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            ParticleManager.instance.SpawnTest(1000000);
#endif
    }
    //protected override void FixedUpdate()
    //{
    //    base.FixedUpdate();
    //    if (isShooting)
    //        FireFrame();
    //}

    private void Fire()
    {
        isShooting = true;
        var dt = Time.time - lastFired;
        if (dt > FIRE_RATE_INTERVAL)
        {
            LaunchPoints();
        }
    }
    public void LaunchPoints()
    {
        //Scale initialised at 0
        StaticPointDef[] pointsToAdd = new StaticPointDef[ParticleManager.instance.shotsPerInterval];
        for (int i = 0; i < ParticleManager.instance.shotsPerInterval; i++)
        {
            var dir = GetRandomTargetDirCircle().normalized;
            Debug.DrawRay(cam.transform.position, dir, Color.green, FIRE_RATE_INTERVAL);
            if (Physics.Raycast(cam.transform.position, dir, out var hit, MAX_RAYCAST_DIST, scannable))
            {
                var layer = hit.collider.gameObject.layer;
                if ((dynamicObjectMask & (1 << layer)) != 0)
                //if (layer == Mathf.Log(dynamicObjectMask, 2))
                {
                    var localHitPoint = hit.collider.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
                    ParticleManager.AddParticleToGameObject(localHitPoint, hit.collider.transform);
                }
                else
                {
                    //Method on ParticleManager that returns types of points with dictionary
                    //pointsToAdd[i].posScale = new Vector4(hit.point.x, hit.point.y, hit.point.z, ParticleManager.instance.particleSize);
                    //pointsToAdd[i].color = (Vector4)Color.white;
                    pointsToAdd[i] = ParticleManager.GetPointDef(hit.point, PointType.Static);
                    //Run check with dictionary
                    //if (hit.collider.CompareTag("CyanStaticSurface"))
                    //    ParticleManager.AddParticle(hit.point, Color.cyan);
                    //if (hit.collider.CompareTag("BlueStaticSurface"))
                    //    ParticleManager.AddParticle(hit.point, Color.blue);
                    //ParticleManager.AddParticle(hit.point);
                }
            }
        }
        lastFired = Time.time;
        ParticleManager.AddParticleGroup(pointsToAdd);
        fireEvent?.Invoke();
    }

    Vector3 GetRandomTargetDirBox()
    {
        Vector3 randomY = cam.transform.up * Random.Range(-shootRange, shootRange);
        Vector3 randomX = cam.transform.right * Random.Range(-shootRange, shootRange);

        return cam.transform.forward + randomX + randomY;

    }

    Vector3 GetRandomTargetDirCircle()
    {
        var randVec = Random.insideUnitCircle * shootRange;
        var randY = cam.transform.up * randVec.y;
        var randX = cam.transform.right * randVec.x;
        return cam.transform.forward + randX + randY;
    }
}


