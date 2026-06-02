using CodependantModules.Simulation;
using UnityEngine;

public class OxygenModuleCompute : IModuleCompute
{
    private readonly OxygenModule config;

    private readonly int oxygenIndex;
    private readonly int floraIndex;
    private readonly int faunaIndex;

    public OxygenModuleCompute(OxygenModule config, int oxygenIndex, int floraIndex, int faunaIndex)
    {
        this.config = config;
        this.oxygenIndex = oxygenIndex;
        this.floraIndex = floraIndex;
        this.faunaIndex = faunaIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float flora = snapshot.Get(floraIndex);
        float fauna = snapshot.Get(faunaIndex);
        float oxygen = snapshot.Get(oxygenIndex);

        float oxygenDerivative =
            + config.PlantOxygenProduction * flora
            - config.FaunaOxygenConsumption * fauna
            - config.OxygenLoss * oxygen;

        var output = new ModuleOutput(snapshot.Length);
        output.AddDerivative(oxygenIndex, oxygenDerivative);
        return output;
    }
}
