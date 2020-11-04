using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour
{
    public float force = 10.0f;

    public GameObject sunLight;
    public Light lampLight;
    public ParticleSystem ps;

    //public ParticleSystem.Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        sunLight = GameObject.Find("Directional Light");
        ps = GetComponent<ParticleSystem>();

        //particles = new ParticleSystem.Particle[ps.maxParticles];

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(sunLight.GetComponent<Transform>().rotation.eulerAngles.x);
        if (sunLight.GetComponent<Transform>().rotation.eulerAngles.x > 250)
        {
            if (!ps.IsAlive())
            {
                ps.Play();
                lampLight.enabled = !lampLight.enabled;
            }
                
        }
        else
        {
            if (ps.IsAlive())
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                lampLight.enabled = !lampLight.enabled;
            }                
        }
        
    }

    
}
