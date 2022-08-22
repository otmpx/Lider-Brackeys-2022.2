using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rb;
    public float radius = 5.0F;
    public float power = 10.0F;

    async void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        await Task.Delay(500);
        
        rb.AddExplosionForce(power, transform.position + Vector3.down + Vector3.left, radius);
        
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
