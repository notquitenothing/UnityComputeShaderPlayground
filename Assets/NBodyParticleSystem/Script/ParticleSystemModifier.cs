using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShapeCreator : MonoBehaviour {

    

    /*
    internal static void CreateShape(Particle[] particleArray, Shape shape, float size)
    {
        if (shape == Shape.cube)
        {
            for (int i = 0; i < particleArray.Length; ++i)
            {
                particleArray[i].pos.x = (((Random.value * 2) - 1.0f) * size);
                particleArray[i].pos.y = (((Random.value * 2) - 1.0f) * size);
                particleArray[i].pos.z = (((Random.value * 2) - 1.0f) * size);

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
                r = size * Mathf.Pow(u, 0.333f);

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

                particleArray[i].pos.x = size * Mathf.Sin(theta) * Mathf.Cos(phi);
                particleArray[i].pos.y = size * Mathf.Sin(theta) * Mathf.Sin(phi);
                particleArray[i].pos.z = size * Mathf.Cos(theta);

                particleArray[i].vel.x = 0;
                particleArray[i].vel.y = 0;
                particleArray[i].vel.z = 0;
            }
        }

        else if (shape == Shape.galaxy)
        {
            //float totalMass = this.particlesMass * particleArray.Length;
            for (int i = 0; i < particleArray.Length; i++)
            {
                float radius = size * Mathf.Sqrt(Random.value);
                float phi = Random.value * 2 * Mathf.PI;

                particleArray[i].pos.x = radius * Mathf.Cos(phi);
                particleArray[i].pos.y = radius * Mathf.Sin(phi);
                particleArray[i].pos.z = 0.0f;

                //float velocity = Mathf.Sqrt( (Mathf.Pow(radius, 1.0f) * totalMass) / Mathf.Pow(this.shapeSize, 1.0f) );
                //float velocity = Mathf.Sqrt((totalMass * radius * radius) / Mathf.Pow((radius * radius) + (this.shapeSize * this.shapeSize), 3.0f / 2.0f));
                //float velocity = Mathf.Sqrt(2 * Mathf.PI * this.shapeSize * totalMass * Mathf.Pow(radius / (2*this.shapeSize), 2.0f));
                //velocity *= (1 + ((Random.value * 2 - 1) / 10.0f)); //Add 10% variance to velocities
                //particleArray[i].vel.x = -velocity * Mathf.Sin(phi);
                //particleArray[i].vel.y = velocity * Mathf.Cos(phi);
                particleArray[i].vel.x = 0.0f;
                particleArray[i].vel.y = 0.0f;
                particleArray[i].vel.z = 0.0f;
            }
        }
    }
    */

    internal static void SetPosition(Particle[] particleArray, Vector3 transform)
    {
        //Adjust position
        for (int i = 0; i < particleArray.Length; i++)
        {
            particleArray[i].pos.x += transform.x;
            particleArray[i].pos.y += transform.y;
            particleArray[i].pos.z += transform.z;
        }
    }


    internal static void SetOrientation(Particle[] particleArray, Vector3 rotation)
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


    internal static void SetVelocity(Particle[] particleArray, Vector3 velocityBoost)
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
