using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Skeleton : Actor
    {
        [HideInInspector] public NavMeshAgent agent;
        public float distToPlayer;
        public float killRange;
        public bool isAggro = false;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
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
            targetPos = Player.Instance.transform.position;
            distToPlayer = (targetPos - transform.position).magnitude;
        }
    }

    public class BaseEnemyState : BaseState
    {
        protected readonly Skeleton skeleton;

        public BaseEnemyState(Skeleton daddy) : base(daddy)
        {
            skeleton = daddy;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
            if (!isTimed) return;
            age -= Time.deltaTime;
            if (age <= 0)
            {
                sm.ChangeState(new Chase(skeleton));
            }
        }

        public override void FixedUpdate()
        {
            skeleton.rb.velocity = Vector3.zero;
        }
    }

    public class Idle : BaseEnemyState
    {
        public Idle(Skeleton daddy) : base(daddy) { }

        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.anim.CrossFadeInFixedTime(Actor.IdleKey, transitionDur);
        }
        public override void Update()
        {
            base.Update();
            if (skeleton.isAggro)
                skeleton.ChangeState(new Chase(skeleton));
        }
    }

    public class Chase : BaseEnemyState
    {
        public Chase(Skeleton daddy) : base(daddy) { }

        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.anim.CrossFadeInFixedTime(Actor.ChargeKey, transitionDur);
            if (skeleton.agent.isStopped)
                skeleton.agent.isStopped = false;
        }

        public override void Update()
        {
            base.Update();
            skeleton.agent.SetDestination(skeleton.targetPos);
            if (skeleton.distToPlayer < skeleton.killRange)
                skeleton.ChangeState(new Attack(skeleton));
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!skeleton.agent.isStopped)
                skeleton.agent.isStopped = true;
        }
    }

    public class Attack : BaseEnemyState
    {
        public Attack(Skeleton daddy) : base(daddy)
        {
            isTimed = true;
            age = 0.6f;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            // Face camera towards enemy
            skeleton.anim.CrossFadeInFixedTime(Actor.MeleeKey, transitionDur);
        }
        public override void Update()
        {
            base.Update();
            skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation, Quaternion.LookRotation(skeleton.targetDir), Mathf.Clamp01(1 - age / 0.3f));
            // Do some shit with camera and jumpscares i guess
        }
    }
}