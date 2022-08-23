using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PickupCoin();
    }
    public void PickupCoin()
    {
        LevelDirector.Instance.RegisterCoin();
        Destroy(gameObject);
    }
}
