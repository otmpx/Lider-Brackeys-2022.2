using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState currentState;

    protected virtual void Start()
    {
        currentState = new BaseState(this);
    }

    protected virtual void Update()
    {
        currentState.Update();
    }

    protected virtual void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(collision);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    public virtual void ChangeState(BaseState newState)
    {
        currentState.OnExit();
        newState.OnEnter();
        currentState = newState;
    }
    public void Despawn(Object obj = null)
    {
        if (obj == null)
            Destroy(gameObject);
        else
            Destroy(obj);
    }
}
