using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFakeGalaxy : GPUParticleSystem {

    /// <summary>
    /// How massive each particle is (in natural units, 1 = Gravitational Constant (G) Kilograms).
    /// </summary>
    public float centralMass = 100.0f;

    /// <summary>
    /// Number of Particles created in the system.
    /// </summary>
    public int starCount = 40000;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float size = 100;


    public override Particle[] InitMassParticles()
    {
        Particle[] particles;

        particles = this.CreateCenter();

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override Particle[] InitMasslessParticles()
    {
        Particle[] particles;

        particles = this.CreateSolar(this.starCount);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override int CountMassiveParticles()
    {
        return 1;
    }

    public override int CountMasslessParticles()
    {
        return this.starCount;
    }

    private Particle[] CreateSolar(int count)
    {
        Particle[] particles = new Particle[count];

        for (int i = 0; i < count; ++i)
        {
            float r = (size * Mathf.Sqrt(Random.value))+1;
            float phi = Random.value * 2 * Mathf.PI;

            particles[i].pos.x = r * Mathf.Cos(phi);
            particles[i].pos.y = r * Mathf.Sin(phi);
            particles[i].pos.z = 0.0f;

            float velocity = Mathf.Sqrt(centralMass / r);

            particles[i].vel.x = -velocity * Mathf.Sin(phi);
            particles[i].vel.y = velocity* Mathf.Cos(phi);

        }

        return particles;
    }

    private Particle[] CreateCenter()
    {
        Particle[] particles = new Particle[1];

        particles[0].pos.x = 0;
        particles[0].pos.y = 0;
        particles[0].pos.z = 0;

        particles[0].vel.x = 0;
        particles[0].vel.y = 0;
        particles[0].vel.z = 0;

        particles[0].mass = centralMass;

        return particles;
    }
}
