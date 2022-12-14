using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Transform[] children;
    public int triggerPoint = 1000;

    private void Awake()
    {
        children = GetComponentsInChildren<Transform>();
        LidarGun.fireEvent += DetectTriggerPoints;

    }

    private void OnDestroy()
    {
        LidarGun.fireEvent -= DetectTriggerPoints;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //        PickupCoin();
    //}

    private void DetectTriggerPoints()
    {
        if(ParticleManager.GetTotalPointsOnObjects(children) >= triggerPoint)
            PickupCoin();
    }

    public void PickupCoin()
    {
        // Play pickup sound effect
        LevelDirector.instance.RegisterCoin();
        ParticleManager.RemoveDynamicGO(transform);
        Destroy(gameObject);
    }
}
