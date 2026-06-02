using CodependantModules.Simulation;

public class WaterModuleCompute : IModuleCompute
{
    private readonly WaterModule config;

    private readonly IceModule iceConfig;
    private readonly VaporModule vaporConfig;

    private readonly int waterIndex;
    private readonly int iceIndex;
    private readonly int vaporIndex;
    private readonly int temperatureIndex;
    private readonly int tectonicsIndex;

    public WaterModuleCompute(
        WaterModule config,
        IceModule iceConfig,
        VaporModule vaporConfig,
        int waterIndex,
        int iceIndex,
        int vaporIndex,
        int temperatureIndex,
        int tectonicsIndex)
    {
        this.config = config;
        this.iceConfig = iceConfig;
        this.vaporConfig = vaporConfig;

        this.waterIndex = waterIndex;
        this.iceIndex = iceIndex;
        this.vaporIndex = vaporIndex;
        this.temperatureIndex = temperatureIndex;
        this.tectonicsIndex = tectonicsIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float water = snapshot.Get(waterIndex);
        float ice = snapshot.Get(iceIndex);
        float vapor = snapshot.Get(vaporIndex);
        float temperature = snapshot.Get(temperatureIndex);
        float tectonics = snapshot.Get(tectonicsIndex);

        //
        // ICE TRANSITIONS
        //

        float freezingSuitability = 1f - Helpers.Sigmoid(iceConfig.FreezeSharpness * (temperature - iceConfig.FreezePoint));

        float meltingSuitability = 1f - freezingSuitability;

        float freezing = iceConfig.FreezingRate * water * freezingSuitability;

        float melting = iceConfig.MeltingRate * ice * meltingSuitability;

        //
        // VAPOR TRANSITIONS
        //

        float evaporationSuitability = Helpers.Sigmoid(vaporConfig.VaporSharpness * (temperature - vaporConfig.EvaporationPoint));

        float condensationSuitability = 1f - evaporationSuitability;

        float evaporation = vaporConfig.EvaporationRate * water * evaporationSuitability;

        float condensation = vaporConfig.CondensationRate * vapor * condensationSuitability;

        //
        // LIQUID WATER BALANCE
        //

        float waterDerivative =
            // ice exchange
            -freezing
            + melting

            // vapor exchange
            - evaporation
            + condensation

            // tectonics
            + config.TectonicRelease * tectonics

            // losses
            - config.OutflowRate * water

            // baseline replenishment
            + config.BaselineCondensation;

        var output = new ModuleOutput(snapshot.Length);

        output.AddDerivative(waterIndex, waterDerivative);

        return output;
    }
}
