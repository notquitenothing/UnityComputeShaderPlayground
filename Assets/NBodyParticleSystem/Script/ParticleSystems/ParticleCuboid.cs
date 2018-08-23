using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCuboid : GPUParticleSystem {

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    public int particleCount = 10000;

    /// <summary>
    /// How massive each particle is (in natural units, 1 = Gravitational Constant (G) Kilograms).
    /// </summary>
    public float particlesMass = 0.1f;

    /// <summary>
    /// Number of Particles created in the system.
    /// </summary>
    public int masslessParticleCount = 0;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float sizeX = 50;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float sizeY = 50;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float sizeZ = 50;

    public override Particle[] InitMassParticles()
    {
        Particle[] particles;

        particles = this.CreateCuboid(this.particleCount, this.particlesMass);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override Particle[] InitMasslessParticles()
    {
        Particle[] particles;

        particles = this.CreateCuboid(this.masslessParticleCount, 0);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override int CountMassiveParticles()
    {
        return this.particleCount;
    }

    public override int CountMasslessParticles()
    {
        return this.masslessParticleCount;
    }

    private Particle[] CreateCuboid(int count, float mass)
    {
        Particle[] particles = new Particle[count];

        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].pos.x = (((Random.value * 2) - 1.0f) * sizeX);
            particles[i].pos.y = (((Random.value * 2) - 1.0f) * sizeY);
            particles[i].pos.z = (((Random.value * 2) - 1.0f) * sizeZ);

            particles[i].vel.x = 0;
            particles[i].vel.y = 0;
            particles[i].vel.z = 0;

            particles[i].mass = mass;
        }

        return particles;
    }
}
