using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Actor : StateMachine, ISoundAble
{
    public Rigidbody rb;
    public CapsuleCollider col;
    [HideInNormalInspector] public Animator anim;

    [HideInNormalInspector] public Vector3 moveDir;
    [HideInNormalInspector] public Vector3 targetPos;
    [HideInNormalInspector] public Vector3 targetDir;
    
    [field: HideInNormalInspector]
    public AudioSource sound { get; set; }
    [field: HideInNormalInspector]
    public float pitchMultiplier { get; set; } = 1f;

[field: SerializeField]
    #region Animation Keys

    public static readonly int IdleKey = Animator.StringToHash("Idle");
    public static readonly int WalkKey = Animator.StringToHash("Walk");
    //public static readonly int DashKey = Animator.StringToHash("Dash");
    //public static readonly int RollKey = Animator.StringToHash("Roll");
    //public static readonly int SideDirKey = Animator.StringToHash("SideDir");
    //public static readonly int FwdDirKey = Animator.StringToHash("FwdDir");
    //public static readonly int InteractKey = Animator.StringToHash("Interact");
    //public static readonly int DieKey = Animator.StringToHash("Die");
    //public static readonly int DanceKey = Animator.StringToHash("Dance");
    //public static readonly int WakeKey = Animator.StringToHash("Wake");

    //public static readonly int ShootKey = Animator.StringToHash("Shoot");
    //public static readonly int MeleeKey = Animator.StringToHash("Melee");
    //public static readonly int MeleeSpeedKey = Animator.StringToHash("MeleeSpeed");

    //public static readonly int HopKey = Animator.StringToHash("Hop");
    public static readonly int ChargeKey = Animator.StringToHash("Charge");

    //public static readonly int CoomKey = Animator.StringToHash("Coom");
    //public static readonly int CombustKey = Animator.StringToHash("Combust");
    //public static readonly int SummonKey = Animator.StringToHash("Summon");
    public static readonly int BiteKey = Animator.StringToHash("Bite");
    public static readonly int FlattenKey = Animator.StringToHash("Flatten");

    #endregion

    protected virtual void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        //col = GetComponent<CapsuleCollider>();
        sound = GetComponents<AudioSource>()[0];
    }

    protected override void Start()
    {
        base.Start();
        currentState.OnEnter();
    }

    protected override void Update()
    {
        base.Update();
        targetDir = (targetPos - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
    }

}