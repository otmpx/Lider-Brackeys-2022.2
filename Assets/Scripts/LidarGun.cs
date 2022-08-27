using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarGun : MonoBehaviour
{
    public Material laserMat;
    public Transform gunTransform;
    Camera cam;
    public LayerMask scannable;
    public LayerMask dynamicObjectMask;
    public float shootRange = .25f;
    const float MAX_RAYCAST_DIST = 1000f;

    public static event System.Action fireEvent;

    private Vector3[] lastShotLocs;

    public AudioSource sound;
    public SoundCard scanCard;

    private void Awake()
    {
        cam = LevelDirector.instance.cam;
    }
    // Update is called once per frame
    void Update()
    {
        Sway();
    }

    void Sway()
    {

    }

    public void LaunchPoints()
    {
        //Scale initialised at 0
        StaticPointDef[] pointsToAdd = new StaticPointDef[ParticleManager.instance.shotsPerInterval];
        for (int i = 0; i < ParticleManager.instance.shotsPerInterval; i++)
        {
            var dir = GetRandomTargetDirCircle().normalized;
            if (Physics.Raycast(cam.transform.position, dir, out var hit, MAX_RAYCAST_DIST, scannable))
            {
                var layer = hit.collider.gameObject.layer;
                if ((dynamicObjectMask & (1 << layer)) != 0)
                //if (layer == Mathf.Log(dynamicObjectMask, 2))
                {
                    var localHitPoint = hit.collider.transform.worldToLocalMatrix.MultiplyPoint3x4(hit.point);
                    if (hit.collider.CompareTag("Objective"))
                    {
                        ParticleManager.AddParticleToGameObject(localHitPoint, hit.collider.transform, PointType.Objective);
                    }
                    else if (hit.collider.CompareTag("Enemy"))
                    {
                        ParticleManager.AddParticleToGameObject(localHitPoint, hit.collider.transform, PointType.Enemy);
                    }
                    else
                        ParticleManager.AddParticleToGameObject(localHitPoint, hit.collider.transform, PointType.Dynamic);

                }
                else
                {
                    //pointsToAdd[i] = ParticleManager.GetPointDef(hit.point, PointType.Static);
                    ParticleManager.AddParticle(hit.point);
                }
            }
        }
        //ParticleManager.AddParticleGroup(pointsToAdd);
        fireEvent?.Invoke();

        ShootLasers();
    }

    private void ShootLasers()
    {
        //Graphics.DrawProcedural()
    }
    Vector3 GetRandomTargetDirBox()
    {
        Vector3 randomY = cam.transform.up * Random.Range(-shootRange, shootRange);
        Vector3 randomX = cam.transform.right * Random.Range(-shootRange, shootRange);
        return cam.transform.forward + randomX + randomY;
    }

    Vector3 GetRandomTargetDirCircle()
    {
        var randVec = Random.insideUnitCircle * shootRange;
        var randY = cam.transform.up * randVec.y;
        var randX = cam.transform.right * randVec.x;
        return cam.transform.forward + randX + randY;
    }

}
