using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Skeleton : Actor
    {
        [HideInInspector] public NavMeshAgent agent;
        public float chargeRange = 5f;
        public float chargeSpeed = 20f;
        public float chargeDur = 1f;
        public float chargeCooldown = 1f;
        public float cooldownTimer;
        public Color colour;
        public float distToPlayer;
        public float deathDur = 5f;
        public float knockbackDur = 0.2f;
        public int damage = 10;
        public float aggroRange = 10f;
        public bool isAggro = false;

        public GameObject coomParticles;

        public SoundCard hitSound, dodgeSound;

        //public SoundCard deathSound;
        //
        // const float pitchMin = 0.7f;
        // const float pitchMax = 1f;
        public string enemyType;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
            cooldownTimer = chargeCooldown;
            enemyType = tag;
            tag = "Enemy";

            if (!isAggro)
                currentState = new Idle(this);
            else
                currentState = new Chase(this);
            currentState.OnEnter();
        }


        protected override void Update()
        {
            base.Update();
            targetPos = Player.Instance.transform.position;
            distToPlayer = (targetPos - transform.position).magnitude;

            if (currentState is Idle && (distToPlayer < aggroRange || isAggro))
            {
                isAggro = true;
                ChangeState(new Chase(this));
            }

            if (currentState is Chase && distToPlayer < chargeRange && cooldownTimer <= 0)
                ChangeState(new Hop(this));
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
    }

    public class Chase : BaseEnemyState
    {
        public Chase(Skeleton daddy) : base(daddy) { }

        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.anim.CrossFadeInFixedTime(Actor.WalkKey, transitionDur);
            if (skeleton.agent.isStopped)
                skeleton.agent.isStopped = false;
        }

        public override void Update()
        {
            base.Update();
            skeleton.agent.SetDestination(skeleton.targetPos);
            skeleton.cooldownTimer -= Time.deltaTime;
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!skeleton.agent.isStopped)
                skeleton.agent.isStopped = true;
        }
    }

    public class Hop : BaseEnemyState
    {
        const float animDur = 0.8f;
        const float rotationSpeed = 10f;
        const float chargeBackMultiplier = 0.1f;

        public Hop(Skeleton daddy) : base(daddy)
        {
            isTimed = true;
            age = animDur;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.anim.CrossFadeInFixedTime(Actor.HopKey, transitionDur);
            //if (skeleton.enemyType != "Enemy")
            //    skeleton.tag = skeleton.enemyType;
        }

        public override void Update()
        {
            age -= Time.deltaTime;
            if (age <= 0)
                sm.ChangeState(new Charge(skeleton));
            skeleton.transform.rotation = Quaternion.Lerp(skeleton.transform.rotation,
                Quaternion.LookRotation(skeleton.targetDir), Time.deltaTime * rotationSpeed);
        }

        public override void FixedUpdate()
        {
            skeleton.rb.velocity = skeleton.chargeSpeed * chargeBackMultiplier *
                                   (skeleton.transform.rotation * Vector3.back);
        }
        //public override void OnExit()
        //{
        //    base.OnExit();
        //    skeleton.tag = "Enemy";
        //}
    }

    public class Charge : BaseEnemyState
    {
        bool alreadyHit = false;

        //float currentSpeed;
        public Charge(Skeleton daddy) : base(daddy)
        {
            isTimed = true;
            age = skeleton.chargeDur;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            alreadyHit = false;
            skeleton.anim.CrossFadeInFixedTime(Actor.ChargeKey, transitionDur);
            if (skeleton.enemyType != "Enemy")
                skeleton.tag = skeleton.enemyType;
        }

        //public override void Update()
        //{
        //    base.Update();
        //    currentSpeed = Mathf.Lerp(skeleton.chargeSpeed, 0, 1 - age / skeleton.chargeDur);
        //}
        public override void FixedUpdate()
        {
            skeleton.rb.velocity = skeleton.chargeSpeed * (skeleton.transform.rotation * Vector3.forward);
            //skeleton.rb.velocity = currentSpeed * (skeleton.transform.rotation * Vector3.forward);
        }

        public override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            if (alreadyHit) return;
            Physics.IgnoreCollision(collision.collider, skeleton.col);
            Physics.IgnoreCollision(collision.collider, skeleton.col, false);
            if (collision.gameObject.CompareTag("Player"))
            {
                LevelDirector.Instance.UpdateSanity(-skeleton.damage);
                alreadyHit = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            skeleton.cooldownTimer = skeleton.chargeCooldown;
            skeleton.tag = "Enemy";
        }
    }
}