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
        ParticleSystemModifier.OffsetPosition(particles, this.transform.position);
        ParticleSystemModifier.OffsetOrientation(particles, this.transform.eulerAngles);
        ParticleSystemModifier.OffsetVelocity(particles, this.initialRelativeVelocity);
    }
}
