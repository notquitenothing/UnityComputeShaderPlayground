using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisk : GPUParticleSystem
{
    /// <summary>
    /// 
    /// </summary>
    [Range(0,1)]
    public float particleMass = 0.1f;

    /// <summary>
    /// Number of Particles created in the system.
    /// </summary>
    public int massiveCount = 32768;

    /// <summary>
    /// Number of Particles created in the system.
    /// </summary>
    public int masslessCount = 0;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float size = 100;


    public override Particle[] InitMassParticles()
    {
        Particle[] particles;

        particles = this.CreateSolar(this.massiveCount, this.particleMass);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override Particle[] InitMasslessParticles()
    {
        Particle[] particles;

        particles = this.CreateSolar(this.masslessCount, 0);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override int CountMassiveParticles()
    {
        return this.massiveCount;
    }

    public override int CountMasslessParticles()
    {
        return this.masslessCount;
    }

    private Particle[] CreateSolar(int count, float mass)
    {
        Particle[] particles = new Particle[count];

        float totalMass = this.massiveCount * this.particleMass;

        for (int i = 0; i < count; i++)
        {
            //float r = size * Mathf.Sqrt(Random.value);
            float r = size * (1 - Mathf.Exp(-Random.value));
            float phi = Random.value * 2 * Mathf.PI;

            particles[i].pos.x = r * Mathf.Cos(phi);
            particles[i].pos.y = r * Mathf.Sin(phi);
            particles[i].pos.z = 0.0f;
            
            float innerMass = totalMass * (r/size);
            float velocity = Mathf.Sqrt(innerMass / r);

            particles[i].vel.x = -velocity *Mathf.Sin(phi);
            particles[i].vel.y = velocity * Mathf.Cos(phi);

            //particles[i].vel.x = -velocity * Mathf.Sin(phi);
            //particles[i].vel.y = velocity * Mathf.Cos(phi);

            particles[i].mass = mass;

        }

        ////Trying to cheat
        //for(int i = 0; i < count; i++)
        //{
        //    Vector3 sumGrav = new Vector3();
        //    Vector3 dist = new Vector3();
        //    float distMag;
        //    Vector3 pos = particles[i].pos;
        //    for(int j = 0; j < count; j++)
        //    {
        //        if (i != j)
        //        {
        //            dist = particles[j].pos - pos;
        //            distMag = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y) + (dist.z * dist.z));

        //            sumGrav += (dist * particles[j].mass) / (Mathf.Pow((distMag * distMag), 3.0f / 2.0f)); //Gravity
        //        }
        //    }

        //    particles[i].vel.x = -sumGrav.x;
        //    particles[i].vel.y = -sumGrav.y;
        //}

        return particles;
    }
}
