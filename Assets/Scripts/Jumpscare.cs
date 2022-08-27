using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public Transform child;
    public float distFromPlayer = 2f;
    public float headRotation = 5f;
    public float headRotationSpeed = 50f;

    public float timeBeforeActivation = 3f;
    bool active = false;

    public float timeBeforeNextScene = 2f;
    bool transition = false;
    public AudioClip jumpscareClip;
    private void Update()
    {
        if (LevelDirector.instance.coinsCollected > 0 && !active)
        {
            // Cut music here
            timeBeforeActivation -= Time.deltaTime;
            Player.Instance.disableShooting = true;
        }
        if (timeBeforeActivation < 0 && !active && !Input.GetKey(KeyCode.Mouse0))
        {
            Player.Instance.disableShooting = false;
            active = true;
        }

        if (!active) return;
        transform.position = Player.Instance.vCamFollow.position + Camera.main.transform.forward * distFromPlayer;
        transform.LookAt(Player.Instance.vCamFollow);

        var rot = child.transform.localEulerAngles;
        rot.x = Mathf.Cos(Time.time * headRotationSpeed) * headRotation;
        child.transform.localEulerAngles = rot;

        if (Player.Instance.isShooting && !transition)
        {
            // Play jumpscare sound and effects
            AudioManager.instance.PlaySFX(jumpscareClip);
            Player.Instance.disableShooting = true;
            transition = true;
        }

        if (!transition) return;
        timeBeforeNextScene -= Time.deltaTime;
        if (timeBeforeNextScene < 0)
            LevelDirector.instance.AdvanceLevel();
    }
}
