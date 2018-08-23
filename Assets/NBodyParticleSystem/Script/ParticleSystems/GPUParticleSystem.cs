using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GPUParticleSystem : MonoBehaviour {

    /// <summary>
    /// The initial velocity of the particle system
    /// </summary>
    public Vector3 initialRelativeVelocity;

    public abstract Particle[] InitMassParticles();

    public abstract Particle[] InitMasslessParticles();

    public abstract int CountMassiveParticles();

    public abstract int CountMasslessParticles();

    public void SetInitialTransformsAndVelocities(Particle[] particles)
    {
        ParticleShapeCreator.SetPosition(particles, this.transform.position);
        ParticleShapeCreator.SetOrientation(particles, this.transform.eulerAngles);
        ParticleShapeCreator.SetVelocity(particles, this.transform.eulerAngles);
    }
}
