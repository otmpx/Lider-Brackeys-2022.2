using Enemy;
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
    float lastPlayedFootstep;
    const float walkSoundIntervals = 0.65f;
    const float runSoundIntervals = 0.5f;
    public Move(Player sm) : base(sm) { }
    public override void Update()
    {
        base.Update();
        if (player.moveDir == Vector3.zero)
            player.ChangeState(new Idle(player));

        if (!player.isRunning && Time.time - lastPlayedFootstep < walkSoundIntervals) return;
        if (player.isRunning && Time.time - lastPlayedFootstep < runSoundIntervals) return;
        player.footstepsCard.PlayAfterFinish(player);
        lastPlayedFootstep = Time.time;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.rb.velocity = player.isRunning ? player.runSpeed * player.moveDir : player.walkSpeed * player.moveDir;
    }
}
public class Damaged : PlayerState
{
    float shootAfter = 0.25f;
    bool shot = false;
    readonly Skeleton target;
    public Damaged(Player sm, Skeleton _target) : base(sm)
    {
        target = _target;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        player.disableShooting = true;
        player.deathCam.LookAt = target.lookAt;
        player.deathCam.Priority = 11;
    }
    public override void Update()
    {
        base.Update();
        shootAfter -= Time.deltaTime;
        if (shootAfter < 0 && !shot)
        {
            shot = true;
            player.gun.LaunchPoints();
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.rb.velocity = Vector3.zero;
    }
}
