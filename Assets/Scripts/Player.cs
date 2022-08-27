using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;

public class Player : Actor
{
    public float walkSpeed;
    public float runSpeed;
    public Transform vCamFollow;
    public CinemachineVirtualCamera deathCam;

    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool disableShooting = false;

    //[HideInInspector] public CinemachineBasicMultiChannelPerlin headBob;

    public static Player Instance;

    [HideInInspector] public float lastFired = 0f;
    public float FIRE_RATE_INTERVAL = 0.1f;

    Camera cam;

    public LidarGun gun;
    public SoundCard footstepsCard;
    public AudioClip deathClip;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        rb = GetComponent<Rigidbody>();
        cam = LevelDirector.instance.cam;
        LevelDirector.instance.vCam.Follow = vCamFollow;
        Cursor.lockState = CursorLockMode.Locked;
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
        isRunning = Input.GetAxisRaw("Vertical") > 0;
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
        var dt = Time.time - lastFired;
        if (dt > FIRE_RATE_INTERVAL)
        {
            gun.LaunchPoints();
            lastFired = Time.time;
            gun.scanCard.PlaySecondary(gun.sound);
        }
    }

}


