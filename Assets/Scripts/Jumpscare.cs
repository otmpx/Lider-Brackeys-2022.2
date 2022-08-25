using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public Transform child;
    public float distFromPlayer = 2f;
    public float headRotation = 10;
    public float headRotationSpeed = 10f;
    bool active = false;
    private void Update()
    {
        if (LevelDirector.Instance.coinsCollected > 0)
            active = true;

        if (!active) return;
        transform.position = Player.Instance.vCamFollow.position + Camera.main.transform.forward * distFromPlayer;
        transform.LookAt(Player.Instance.vCamFollow);

        var rot = child.transform.localEulerAngles;
        rot.x = Mathf.Cos(Time.time * headRotationSpeed) * headRotation;
        child.transform.localEulerAngles = rot;
    }
}
