using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Skeleton : Actor
    {
        private Transform[] children;
        public Transform lookAt;
        [HideInInspector] public NavMeshAgent agent;
        public float distToPlayer;
        public float killRange;
        public bool isAggro = false;

        public int soundTriggerPoint = 10;
        public int aggroTriggerPoint = 150;
        public float chaseDur = 10f;

        protected override void Awake()
        {
            base.Awake();
            children = GetComponentsInChildren<Transform>();
            agent = GetComponent<NavMeshAgent>();
            //Player.fireEvent += Activate;
            LidarGun.fireEvent += DetectTriggerPoints;
        }
        private void OnDisable()
        {
            //Player.fireEvent -= Activate;
            LidarGun.fireEvent -= DetectTriggerPoints;
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
        //void Activate()
        //{
        //    if (currentState is null) return;
        //    currentState = new Wake(this);
        //    currentState.OnEnter();
        //}
        void DetectTriggerPoints()
        {
            if (!isAggro && ParticleManager.GetTotalPointsOnObjects(children) >= soundTriggerPoint)
            {
                // Play sound effect
            }
            if (ParticleManager.GetTotalPointsOnObjects(children) >= aggroTriggerPoint)
                isAggro = true;
        }
        public void Respawn()
        {
            SpawnDirector.instance.SpawnEnemy();
            ParticleManager.RemoveDynamicGO(transform);
            Destroy(gameObject);
        }
    }

    public class BaseEnemyState : BaseState
    {
        protected readonly Skeleton skeleton;

        public BaseEnemyState(Skeleton daddy) : base(daddy)
        {
            skeleton = daddy;
        }

        public override void Update()
        {
            base.Update();
            if (!isTimed) return;
            age -= Time.deltaTime;
            if (age <= 0)
            {
                sm.ChangeState(new Idle(skeleton));
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
        float timer;
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
            timer += Time.deltaTime;
            if (timer > skeleton.chaseDur)
                skeleton.ChangeState(new Despawn(skeleton));
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
            age = 2f;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.transform.rotation = Quaternion.LookRotation(skeleton.targetDir);
            if (!(Player.Instance.currentState is Damaged))
                Player.Instance.ChangeState(new Damaged(Player.Instance, skeleton));
            skeleton.anim.CrossFadeInFixedTime(Actor.BiteKey, transitionDur);
        }
        public override void OnExit()
        {
            base.OnExit();
            LevelDirector.instance.ReloadLevel();
        }
    }
    //public class Wake : BaseEnemyState
    //{
    //    public Wake(Skeleton daddy) : base(daddy)
    //    {
    //        isTimed = true;
    //        age = 2f;
    //    }
    //    public override void OnEnter()
    //    {
    //        base.OnEnter();
    //        skeleton.anim.PlayInFixedTime(Actor.WakeKey);
    //    }
    //}
    public class Despawn : BaseEnemyState
    {
        public Despawn(Skeleton daddy) : base(daddy)
        {
            isTimed = true;
            age = 1f;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            skeleton.anim.CrossFadeInFixedTime(Actor.DieKey, transitionDur);
        }
        public override void OnExit()
        {
            base.OnExit();
            skeleton.Respawn();
        }
    }
    //public class Die : BaseEnemyState
    //{
    //    public Die(Skeleton daddy) : base(daddy)
    //    {
    //        isTimed = true;
    //        age = 1f;
    //    }
    //    public override void OnEnter()
    //    {
    //        base.OnEnter();
    //        skeleton.anim.CrossFadeInFixedTime(Actor.DieKey, transitionDur);
    //    }
    //    public override void OnExit()
    //    {
    //        base.OnExit();
    //        ParticleManager.RemoveDynamicGO(skeleton.transform);
    //        Object.Destroy(skeleton);
    //    }
    //}
}