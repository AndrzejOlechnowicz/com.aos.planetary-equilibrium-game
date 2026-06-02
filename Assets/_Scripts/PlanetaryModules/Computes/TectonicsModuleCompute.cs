using CodependantModules.Simulation;
using UnityEngine;

public class TectonicsModuleCompute : IModuleCompute
{
    private readonly int tectonicsIndex;
    private readonly int seed;

    public TectonicsModuleCompute(int tectonicsIndex, int seed)
    {
        this.tectonicsIndex = tectonicsIndex;
        this.seed = seed;
    }

    public ModuleOutput Compute(in SystemState snapshot, double t)
    {
        float tectonics = snapshot.Get(tectonicsIndex);

        var output = new ModuleOutput(snapshot.Length);

        float internalDrive = 0.01f;
        float damping = 0.05f;
        float noise = Mathf.PerlinNoise(seed * 0.1f + (float)t * 0.1f, 0f) * 0.02f;

        float dTectonics = internalDrive - damping * tectonics + noise;

        output.AddDerivative(tectonicsIndex, dTectonics);
        output.AddLinearCoeff(tectonicsIndex, damping);

        return output;
    }
}
