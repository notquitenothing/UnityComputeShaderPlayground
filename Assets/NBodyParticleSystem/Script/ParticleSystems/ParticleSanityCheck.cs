﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSanityCheck : GPUParticleSystem
{

    /// <summary>
    /// How massive each particle is (in natural units, 1 = Gravitational Constant (G) Kilograms).
    /// </summary>
    public float centralMass = 100.0f;

    /// <summary>
    /// Number of Particles created in the system.
    /// </summary>
    public int planetCount = 10;

    /// <summary>
    /// Initial size of shape of particles
    /// </summary>
    public float size = 50;

    /// <summary>
    /// 
    /// </summary>
    public bool giveOrbit = true;


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

        particles = this.CreateSolar(this.planetCount);

        this.SetInitialTransformsAndVelocities(particles);

        return particles;
    }

    public override int CountMassiveParticles()
    {
        return 1;
    }

    public override int CountMasslessParticles()
    {
        return this.planetCount;
    }

    private Particle[] CreateSolar(int count)
    {
        Particle[] particles = new Particle[count];

        for (int i = 0; i < count; ++i)
        {
            float r = size * Random.value;
            
            particles[i].pos.y = r;

            if (this.giveOrbit)
            {
                particles[i].vel.x = Mathf.Sqrt(centralMass / r);
            }
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
