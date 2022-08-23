using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not serializable therefore not able to use custom property drawer
public class BaseState
{
    [HideInInspector] public StateMachine sm;
    [SerializeField] public string stateName = "Base State";
    [HideInInspector] public bool isTimed = false;
    public const float transitionDur = 0.25f;
    public float age = float.MaxValue;

    public BaseState(StateMachine sm)
    {
        this.sm = sm;
        stateName = GetType().Name;
    }
    public virtual void OnEnter()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual void OnCollisionEnter(Collision collision)
    {

    }
    public virtual void OnTriggerEnter(Collider other)
    {

    }
}
