using CodependantModules.Simulation;
using UnityEngine;

public class WaterModuleCompute : IModuleCompute
{
    private readonly WaterModule config;

    private readonly IceModule iceConfig;
    private readonly VaporModule vaporConfig;

    private readonly int waterIndex;
    private readonly int iceIndex;
    private readonly int vaporIndex;
    private readonly int temperatureIndex;

    public WaterModuleCompute(
        WaterModule config,
        IceModule iceConfig,
        VaporModule vaporConfig,
        int waterIndex,
        int iceIndex,
        int vaporIndex,
        int temperatureIndex)
    {
        this.config = config;
        this.iceConfig = iceConfig;
        this.vaporConfig = vaporConfig;

        this.waterIndex = waterIndex;
        this.iceIndex = iceIndex;
        this.vaporIndex = vaporIndex;
        this.temperatureIndex = temperatureIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float water = snapshot.Get(waterIndex);
        float ice = snapshot.Get(iceIndex);
        float vapor = snapshot.Get(vaporIndex);
        float temperature = snapshot.Get(temperatureIndex);

        //
        // ICE TRANSITIONS
        //

        var freezingSuitability = WaterPhasesMath.GetSuitability(temperature, iceConfig.FreezePoint, iceConfig.FreezeSharpness);
        
        float freezing = iceConfig.FreezingRate * water * freezingSuitability.Forward;
        
        float melting = iceConfig.MeltingRate * ice * freezingSuitability.Reverse;
        
        //
        // VAPOR TRANSITIONS
        //
        var evaporationSuitability = WaterPhasesMath.GetSuitability(temperature, vaporConfig.EvaporationPoint, vaporConfig.VaporSharpness);

        float evaporation = vaporConfig.EvaporationRate * water * evaporationSuitability.Forward;
        
        float condensation = vaporConfig.CondensationRate * vapor * evaporationSuitability.Reverse;
        
        //
        // LIQUID WATER BALANCE
        //

        float waterDerivative =
            // ice exchange
            - freezing
            + melting

            // vapor exchange
            - evaporation
            + condensation;

        var output = new ModuleOutput(snapshot.Length);

        output.AddDerivative(waterIndex, waterDerivative);

        return output;
    }
}
