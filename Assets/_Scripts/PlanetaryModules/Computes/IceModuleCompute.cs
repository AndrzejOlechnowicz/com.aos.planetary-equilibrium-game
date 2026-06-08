using CodependantModules.Simulation;

public class IceModuleCompute : IModuleCompute
{
    private readonly IceModule config;

    private readonly int iceIndex;
    private readonly int waterIndex;
    private readonly int temperatureIndex;

    public IceModuleCompute(IceModule config, int iceIndex, int waterIndex, int temperatureIndex)
    {
        this.config = config;

        this.iceIndex = iceIndex;
        this.waterIndex = waterIndex;
        this.temperatureIndex = temperatureIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float ice = snapshot.Get(iceIndex);
        float water = snapshot.Get(waterIndex);
        float temperature = snapshot.Get(temperatureIndex);

        var freezingSuitability = WaterPhasesMath.GetSuitability(temperature, config.FreezePoint, config.FreezeSharpness);

        float freezing = config.FreezingRate * water * freezingSuitability.Forward;
        
        float melting = config.MeltingRate * ice * freezingSuitability.Reverse;
        
        float iceDerivative =
            + freezing
            - melting;

        var output = new ModuleOutput(snapshot.Length);

        output.AddDerivative(iceIndex, iceDerivative);

        return output;
    }
}