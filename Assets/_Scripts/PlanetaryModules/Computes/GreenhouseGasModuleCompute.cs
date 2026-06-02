using CodependantModules.Simulation;
using UnityEngine;

public class GreenhouseGasModuleCompute : IModuleCompute
{
    private readonly GreenhouseGasModule config;

    private readonly int greenhouseIndex;
    private readonly int floraIndex;
    private readonly int faunaIndex;
    private readonly int tectonicsIndex;

    public GreenhouseGasModuleCompute(GreenhouseGasModule config, int greenhouseIndex, int floraIndex, int faunaIndex, int tectonicsIndex)
    {
        this.config = config;
        this.greenhouseIndex = greenhouseIndex;
        this.floraIndex = floraIndex;
        this.faunaIndex = faunaIndex;
        this.tectonicsIndex = tectonicsIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float fauna = snapshot.Get(faunaIndex);
        float tectonics = snapshot.Get(tectonicsIndex);
        float greenhouseGases = snapshot.Get(greenhouseIndex);
        float flora = snapshot.Get(floraIndex);

        float greenhouseDerivative =
            + config.VolcanicEmission * tectonics
            + config.FaunaEmission * fauna
            - config.GreenhouseLoss * greenhouseGases
            - config.FloraAbsorption * flora;


        var output = new ModuleOutput(snapshot.Length);
        output.AddDerivative(greenhouseIndex, greenhouseDerivative);
        return output;
    }
}
