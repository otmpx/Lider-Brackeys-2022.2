using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : BaseState
{
    protected readonly Player player;
    public PlayerState(Player sm) : base(sm)
    {
        player = sm;
    }
    public override void Update()
    {
        base.Update();
        if (!isTimed) return;
        age -= Time.deltaTime;
        if (age <= 0)
            sm.ChangeState(new Idle(player));
    }
}
public class Idle : PlayerState
{
    public Idle(Player sm) : base(sm) { }
    //public override void OnEnter()
    //{
    //    base.OnEnter();
    //    player.headBob.m_FrequencyGain = 0.5f;
    //}
    public override void Update()
    {
        base.Update();
        if (player.moveDir != Vector3.zero)
            sm.ChangeState(new Move(player));
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.rb.velocity = Vector3.zero;
    }
}
public class Move : PlayerState
{
    public Move(Player sm) : base(sm) { }
    //public override void OnEnter()
    //{
    //    base.OnEnter();
    //    player.headBob.m_FrequencyGain = 2f;
    //}
    public override void Update()
    {
        base.Update();
        if (player.moveDir == Vector3.zero)
            player.ChangeState(new Idle(player));
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.rb.velocity = player.moveSpeed * player.moveDir;
    }
}
public class Damaged : PlayerState
{
    public Damaged(Player sm) : base(sm) { }
    //public override void OnEnter()
    //{
    //    base.OnEnter();
    //    player.headBob.m_FrequencyGain = 0;
    //}
}
