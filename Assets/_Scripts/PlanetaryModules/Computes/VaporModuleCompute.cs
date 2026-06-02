using CodependantModules.Simulation;

public class VaporModuleCompute : IModuleCompute
{
    private readonly VaporModule config;

    private readonly int vaporIndex;
    private readonly int waterIndex;
    private readonly int temperatureIndex;

    public VaporModuleCompute(VaporModule config, int vaporIndex, int waterIndex, int temperatureIndex)
    {
        this.config = config;

        this.vaporIndex = vaporIndex;
        this.waterIndex = waterIndex;
        this.temperatureIndex = temperatureIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float vapor = snapshot.Get(vaporIndex);
        float water = snapshot.Get(waterIndex);
        float temperature = snapshot.Get(temperatureIndex);

        float evaporationSuitability = Helpers.Sigmoid(config.VaporSharpness * (temperature - config.EvaporationPoint));

        float condensationSuitability =
            1f - evaporationSuitability;

        float evaporation = config.EvaporationRate * water * evaporationSuitability;

        float condensation = config.CondensationRate * vapor * condensationSuitability;

        float vaporDerivative =
            + evaporation
            - condensation;

        var output = new ModuleOutput(snapshot.Length);

        output.AddDerivative(vaporIndex, vaporDerivative);

        return output;
    }
}