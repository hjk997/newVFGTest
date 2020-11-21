using UnityEngine;

namespace Assets.Scripts
{
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
            if (sunLight.GetComponent<Transform>().rotation.eulerAngles.x > 250)
            {
                if (!ps.IsAlive())
                {
                    // Music: 토치 켜기 
                    ps.Play();
                    lampLight.enabled = !lampLight.enabled;
                }
            }
            else
            {
                if (ps.IsAlive())
                {
                    // Music: 토치 끄기 (위에랑 똑같은 소리라도 괜찮아요) 
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    lampLight.enabled = !lampLight.enabled;
                }
            }
        }
    }
}