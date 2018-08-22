using UnityEngine;

public class GPUParticlesParent : MonoBehaviour {

    /// <summary>
    /// Basic particle definition
    /// </summary>
    public struct Particle
    {
        public Vector3 pos;
        public Vector3 vel;
        public float mass;
    };

    /// <summary>
    /// Size in bytes of the Particle struct.
    /// </summary>
    private const int SIZE_FLOAT = 4;
    private const int SIZE_VECTOR3 = SIZE_FLOAT * 3;
    private const int SIZE_PARTICLE = (SIZE_VECTOR3 * 2) + SIZE_FLOAT;

    private const int WARP_SIZE = 1024;


    ////Public Variables

    /// <summary>
    /// Material used to draw the Particle on screen.
    /// </summary>
    public Material material;

    /// <summary>
    /// Compute shader used to update the Particles.
    /// </summary>
    public ComputeShader computeShader;
    
    ///<summary>
    /// Max size of kernel work sent to GPU
    /// </summary>
    public int maxKernelWorkSize = 1000;

    ///<summary>
    /// Wether to use the beta integrator (Usually doesn't work)
    /// </summary>
    public bool useBetaIntegrator = false;

    /// <summary>
    /// Dark Energy Constant value in simulated universe
    /// </summary>
    [Range(0, 1.0f)]
    public float miliDarkEnergyConstant = 0.0f;

    /// <summary>
    /// Timestep
    /// </summary>
    [Range(0, 0.1f)]
    public float timestep = 0.01f;

    /// <summary>
    /// Should be about the max starting distance two particles are away from each other, divided by 100
    /// </summary>
    public float eps = 200.0f / 100.0f;

    ////Private Variables

    /// <summary>
    /// Id of the n-body gravity kernel used.
    /// </summary>
    private int GravityVelKernelID;

    /// <summary>
    /// Id of the euler position kernel used.
    /// </summary>
    private int EulerPosKernelID;

    /// <summary>
    /// Buffer holding the Particles.
    /// </summary>
    private ComputeBuffer particleBuffer;

    /// <summary>
    /// Ensures Update() is not piled up
    /// </summary>
    private bool isDonePhysics = true;

    /// <summary>
    /// 
    /// </summary>
    private int totalMassiveParticles = 0;

    /// <summary>
    /// 
    /// </summary>
    private int totalMasslessParticles = 0;


    // Use this for initialization
    void Start()
    {
        // Initialize the particle sytems at the start
        foreach (GPUParticleSystem system in this.gameObject.GetComponentsInChildren<GPUParticleSystem>())
        {
            this.totalMassiveParticles += system.particleCount;
            this.totalMasslessParticles += system.masslessParticleCount;
        }

        // Create the ComputeBuffer holding the Particles
        this.particleBuffer = new ComputeBuffer(this.totalMassiveParticles + this.totalMasslessParticles, SIZE_PARTICLE);

        // Put the particles from each system into the buffer.  Place massless particles at the end of the buffer
        int offset = 0;
        Particle[] tempParticleArray;
        foreach (GPUParticleSystem system in this.gameObject.GetComponentsInChildren<GPUParticleSystem>())
        {
            tempParticleArray = system.InitParticles();
            this.particleBuffer.SetData(tempParticleArray, 0, offset, tempParticleArray.Length);
            offset += tempParticleArray.Length;
        }
        foreach (GPUParticleSystem system in this.gameObject.GetComponentsInChildren<GPUParticleSystem>())
        {
            tempParticleArray = system.InitMasslessParticles();
            this.particleBuffer.SetData(tempParticleArray, 0, offset, tempParticleArray.Length);
            offset += tempParticleArray.Length;
        }
        

        // Find the id of the kernels
        if (this.useBetaIntegrator)
            GravityVelKernelID = computeShader.FindKernel("GravityVelBeta");
        else
            GravityVelKernelID = computeShader.FindKernel("GravityVel");
        EulerPosKernelID = computeShader.FindKernel("EulerPos");

        // Bind the ComputeBuffer to the shader and the compute shader
        this.computeShader.SetBuffer(GravityVelKernelID, "particleBuffer", this.particleBuffer);
        this.computeShader.SetBuffer(EulerPosKernelID, "particleBuffer", this.particleBuffer);
        this.material.SetBuffer("particleBuffer", this.particleBuffer);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
        {
            particleBuffer.Release();
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if (this.isDonePhysics)
        {
            // Send constants to the compute shader
            computeShader.SetFloat("dark_energy", this.miliDarkEnergyConstant / 1000.0f);
            computeShader.SetFloat("timestep", timestep);
            computeShader.SetFloat("eps", this.eps);

            this.isDonePhysics = false;
            int end_pos;
            int combinedNumParticles = this.totalMassiveParticles + this.totalMasslessParticles;
            int kernelWorkSize = maxKernelWorkSize <= combinedNumParticles ? maxKernelWorkSize : combinedNumParticles;
            int dispatchCount = combinedNumParticles % kernelWorkSize == 0 ? combinedNumParticles / kernelWorkSize : (combinedNumParticles / kernelWorkSize) + 1;
            int threadGroupsX = combinedNumParticles % WARP_SIZE == 0 ? combinedNumParticles / WARP_SIZE : (combinedNumParticles / WARP_SIZE) + 1;
            int numGroupsMassiveParticles = this.totalMassiveParticles % WARP_SIZE == 0 ? this.totalMassiveParticles / WARP_SIZE : (this.totalMassiveParticles / WARP_SIZE) + 1;

            // Update the Particles
            if (WARP_SIZE > 0 && kernelWorkSize > 0 && combinedNumParticles > 0)
            {

                computeShader.SetInt("last_id", combinedNumParticles); //last valid particle
                computeShader.SetInt("numGroupsMassiveParticles", numGroupsMassiveParticles); //How many groups of massive particles per dispatch

                for (int j = 0; j < dispatchCount; j++)
                {
                    computeShader.SetInt("start_pos", j * kernelWorkSize);
                    end_pos = (j + 1) * kernelWorkSize <= this.totalMassiveParticles ? (j + 1) * kernelWorkSize : this.totalMassiveParticles;
                    computeShader.SetInt("end_pos", end_pos);

                    computeShader.Dispatch(GravityVelKernelID, threadGroupsX, 1, 1);
                }

                computeShader.SetInt("last_id", combinedNumParticles);
                computeShader.Dispatch(EulerPosKernelID, threadGroupsX, 1, 1);
            }

            this.isDonePhysics = true;
        }
    }

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  //0 = OFF, 1 = ScreenRate, 2 = 1/2*ScreenRate
        //Application.targetFrameRate = 50; //VSync must be disabled
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, this.totalMassiveParticles + this.totalMasslessParticles);
    }

    private void OnDisable()
    {
        this.particleBuffer.Dispose();
    }
}
