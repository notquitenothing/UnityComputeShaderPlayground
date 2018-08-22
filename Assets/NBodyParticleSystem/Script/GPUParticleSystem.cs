using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour {

    public enum Shape { cube, sphere, shell, galaxy };

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    public int particleCount = 1000;

    /// <summary>
    /// How massive each particle is (in natural units, 1 = Gravitational Constant G)
    /// </summary>
    [Range(0, 1.0f)]
    public float particlesMass = 0.01f;

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    public int masslessParticleCount = 1000;

    /// <summary>
    /// The initial velocity of the particle system
    /// </summary>
    public Vector3 initialRelativeVelocity;

    /// <summary>
    /// Shape to initialize the particles in
    /// </summary>
    public Shape initShape = Shape.cube;

    /// <summary>
    /// Initial size of cube of particles
    /// </summary>
    public float shapeSize = 1;





    public GPUParticlesParent.Particle[] InitParticles()
    {
        GPUParticlesParent.Particle[] particleArray = new GPUParticlesParent.Particle[particleCount];
        this.CreateShape(particleArray, this.initShape);
        this.TransformParticles(particleArray, this.transform.position);
        this.RotateParticles(particleArray, this.transform.eulerAngles);
        this.BoostParticleVelocities(particleArray, this.initialRelativeVelocity);
        for (int i = 0; i < particleArray.Length; i++)
        {
            particleArray[i].mass = this.particlesMass;
        }

        return particleArray;
    }

    public GPUParticlesParent.Particle[] InitMasslessParticles()
    {
        GPUParticlesParent.Particle[] particleArray = new GPUParticlesParent.Particle[masslessParticleCount];
        this.CreateShape(particleArray, this.initShape);
        this.TransformParticles(particleArray, this.transform.position);
        this.RotateParticles(particleArray, this.transform.eulerAngles);
        this.BoostParticleVelocities(particleArray, this.initialRelativeVelocity);
        for (int i = 0; i < particleArray.Length; i++)
        {
            particleArray[i].mass = 0.0f;
        }

        return particleArray;
    }


    private void CreateShape(GPUParticlesParent.Particle[] particleArray, Shape shape)
    {
        if (shape == Shape.cube)
        {
            for (int i = 0; i < particleArray.Length; ++i)
            {
                particleArray[i].pos.x = (((Random.value * 2) - 1.0f) * shapeSize);
                particleArray[i].pos.y = (((Random.value * 2) - 1.0f) * shapeSize);
                particleArray[i].pos.z = (((Random.value * 2) - 1.0f) * shapeSize);

                particleArray[i].vel.x = 0;
                particleArray[i].vel.y = 0;
                particleArray[i].vel.z = 0;
            }
        }
        else if (shape == Shape.sphere)
        {
            float phi;
            float costheta;
            float u;
            float theta;
            float r;

            for (int i = 0; i < particleArray.Length; i++)
            {
                phi = Random.value * 2 * Mathf.PI;
                costheta = (Random.value * 2) - 1.0f;
                u = Random.value;

                theta = Mathf.Acos(costheta);
                r = this.shapeSize * Mathf.Pow(u, 0.333f);

                particleArray[i].pos.x = r * Mathf.Sin(theta) * Mathf.Cos(phi);
                particleArray[i].pos.y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
                particleArray[i].pos.z = r * Mathf.Cos(theta);

                particleArray[i].vel.x = 0;
                particleArray[i].vel.y = 0;
                particleArray[i].vel.z = 0;
            }
        }
        else if (shape == Shape.shell)
        {
            float phi;
            float costheta;
            float theta;

            for (int i = 0; i < particleArray.Length; i++)
            {
                phi = Random.value * 2 * Mathf.PI;
                costheta = (Random.value * 2) - 1.0f;

                theta = Mathf.Acos(costheta);

                particleArray[i].pos.x = this.shapeSize * Mathf.Sin(theta) * Mathf.Cos(phi);
                particleArray[i].pos.y = this.shapeSize * Mathf.Sin(theta) * Mathf.Sin(phi);
                particleArray[i].pos.z = this.shapeSize * Mathf.Cos(theta);

                particleArray[i].vel.x = 0;
                particleArray[i].vel.y = 0;
                particleArray[i].vel.z = 0;
            }
        }
        else if (shape == Shape.galaxy)
        {
            float totalMass = this.particlesMass * this.particleCount;
            for (int i = 0; i < particleArray.Length; i++)
            {
                float radius = this.shapeSize * Mathf.Sqrt(Random.value);
                float phi = Random.value * 2 * Mathf.PI;

                particleArray[i].pos.x = radius * Mathf.Cos(phi);
                particleArray[i].pos.y = radius * Mathf.Sin(phi);
                particleArray[i].pos.z = 0.0f;

                //float velocity = Mathf.Sqrt( (Mathf.Pow(radius, 1.0f) * totalMass) / Mathf.Pow(this.shapeSize, 1.0f) );
                float velocity =  Mathf.Sqrt((totalMass * radius * radius) / Mathf.Pow( (radius*radius) + (this.shapeSize*this.shapeSize), 3.0f/2.0f) );
                //float velocity = Mathf.Sqrt(2 * Mathf.PI * this.shapeSize * totalMass * Mathf.Pow(radius / (2*this.shapeSize), 2.0f));
                //velocity *= (1 + ((Random.value * 2 - 1) / 10.0f)); //Add 10% variance to velocities
                particleArray[i].vel.x = -velocity * Mathf.Sin(phi);
                particleArray[i].vel.y = velocity * Mathf.Cos(phi);
                particleArray[i].vel.z = 0.0f;
            }
        }
    }


    private void TransformParticles(GPUParticlesParent.Particle[] particleArray, Vector3 transform)
    {
        //Adjust position
        for (int i = 0; i < particleArray.Length; i++)
        {
            particleArray[i].pos.x += transform.x;
            particleArray[i].pos.y += transform.y;
            particleArray[i].pos.z += transform.z;
        }
    }


    private void RotateParticles(GPUParticlesParent.Particle[] particleArray, Vector3 rotation)
    {
        rotation *= Mathf.PI / 180.0f; //Convert from degrees to radians

        //Adjust rotation
        Vector3 temp = new Vector3();
        for (int i = 0; i < particleArray.Length; i++)
        {

            //Z Axis
            temp.x = particleArray[i].pos.x * Mathf.Cos(rotation.z) + particleArray[i].pos.y * -Mathf.Sin(rotation.z);
            temp.y = particleArray[i].pos.x * Mathf.Sin(rotation.z) + particleArray[i].pos.y * Mathf.Cos(rotation.z);
            particleArray[i].pos.x = temp.x;
            particleArray[i].pos.y = temp.y;
            temp.x = particleArray[i].vel.x * Mathf.Cos(rotation.z) + particleArray[i].vel.y * -Mathf.Sin(rotation.z);
            temp.y = particleArray[i].vel.x * Mathf.Sin(rotation.z) + particleArray[i].vel.y * Mathf.Cos(rotation.z);
            particleArray[i].vel.x = temp.x;
            particleArray[i].vel.y = temp.y;



            //Y Axis
            temp.x = particleArray[i].pos.x * Mathf.Cos(rotation.y) + particleArray[i].pos.z * Mathf.Sin(rotation.y);
            temp.z = particleArray[i].pos.x * -Mathf.Sin(rotation.y) + particleArray[i].pos.z * Mathf.Cos(rotation.y);
            particleArray[i].pos.x = temp.x;
            particleArray[i].pos.z = temp.z;
            temp.x = particleArray[i].vel.x * Mathf.Cos(rotation.y) + particleArray[i].vel.z * Mathf.Sin(rotation.y);
            temp.z = particleArray[i].vel.x * -Mathf.Sin(rotation.y) + particleArray[i].vel.z * Mathf.Cos(rotation.y);
            particleArray[i].vel.x = temp.x;
            particleArray[i].vel.z = temp.z;

            //X Axis
            temp.y = particleArray[i].pos.y * Mathf.Cos(rotation.x) + particleArray[i].pos.z * -Mathf.Sin(rotation.x);
            temp.z = particleArray[i].pos.y * Mathf.Sin(rotation.x) + particleArray[i].pos.z * Mathf.Cos(rotation.x);
            particleArray[i].pos.y = temp.y;
            particleArray[i].pos.z = temp.z;
            temp.y = particleArray[i].vel.y * Mathf.Cos(rotation.x) + particleArray[i].vel.z * -Mathf.Sin(rotation.x);
            temp.z = particleArray[i].vel.y * Mathf.Sin(rotation.x) + particleArray[i].vel.z * Mathf.Cos(rotation.x);
            particleArray[i].vel.y = temp.y;
            particleArray[i].vel.z = temp.z;
        }
    }


    private void BoostParticleVelocities(GPUParticlesParent.Particle[] particleArray, Vector3 velocityBoost)
    {
        //Adjust velocities
        for (int i = 0; i < particleArray.Length; i++)
        {
            particleArray[i].vel.x += velocityBoost.x;
            particleArray[i].vel.y += velocityBoost.y;
            particleArray[i].vel.z += velocityBoost.z;
        }
    }
}
