using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class Player : Actor
{
    public static Player Instance;
    public float moveSpeed;
    public float mouseSens;
    public float verticalLookRotation;
    public Vector2 input;
    public bool isShooting;
    public CinemachineVirtualCamera vCam;
    CinemachinePOV povController;
    public Camera cam;

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
    }
    public void SetSensitivity()
    {
        povController.m_HorizontalAxis.m_MaxSpeed = mouseSens;
        povController.m_VerticalAxis.m_MaxSpeed = mouseSens;
    }
}


