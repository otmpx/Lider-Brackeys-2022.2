using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class Player : Actor
{
    public float moveSpeed;
    public float mouseSens;
    public CinemachineVirtualCamera vCam;
    public float shootRange = 2.5f;

    Vector2 input;
    bool isShooting;
    CinemachinePOV povController;
    Camera cam;

    public static Player Instance;

    float lastFired = 0f;
    public float FIRE_RATE_INTERVAL = 0.1f;
    public int TOTAL_SHOTS_PER_INTERVAL = 5;
    const float MAX_RAYCAST_DIST = 1000f;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        rb = GetComponent<Rigidbody>();
        povController = vCam.GetComponentPipeline().First(cb => cb is CinemachinePOV) as CinemachinePOV;

    }
    protected override void Start()
    {
        base.Start();
        currentState = new Idle(this);
        currentState.OnEnter();
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }
    protected override void Update()
    {
        base.Update();
        SetSensitivity();
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (inputDir.magnitude > 1e-7)
            moveDir = transform.TransformDirection(inputDir).normalized;
        else
            moveDir = Vector3.zero;
        isShooting = Input.GetKey(KeyCode.Mouse0);
        transform.localEulerAngles = new Vector3(0, cam.transform.localEulerAngles.y, 0);

        if (Input.GetMouseButton(0))
            Fire();
    }

    private void Fire()
    {
        isShooting = true;
        var dt = Time.time - lastFired;
        if (dt > FIRE_RATE_INTERVAL)
        {
            //Physics.Raycast()
            for (int i = 0; i < TOTAL_SHOTS_PER_INTERVAL; i++)
            {
                var dir = GetRandomTargetDirBox().normalized;
                Debug.DrawRay(cam.transform.position, dir, Color.black, FIRE_RATE_INTERVAL);
                if(Physics.Raycast(cam.transform.position, dir, out var hit, MAX_RAYCAST_DIST))
                {
                    ParticleManager.AddParticle(hit.point);
                }
            }
        }
    }

    Vector3 GetRandomTargetDirBox()
    {
        Vector3 randomY = cam.transform.up * Random.Range(-shootRange, shootRange);
        Vector3 randomX = cam.transform.right * Random.Range(-shootRange, shootRange);

        return cam.transform.forward + randomX + randomY;

    }

    Vector3 GetRandomTargetDirCircle()
    {
        var randVec = Random.insideUnitCircle;
        var randY = cam.transform.up * randVec.y;
        var randX = cam.transform.right * randVec.x;
        return cam.transform.forward + randX + randY;
    }

    public void SetSensitivity()
    {
        povController.m_HorizontalAxis.m_MaxSpeed = mouseSens;
        povController.m_VerticalAxis.m_MaxSpeed = mouseSens;
    }
}


