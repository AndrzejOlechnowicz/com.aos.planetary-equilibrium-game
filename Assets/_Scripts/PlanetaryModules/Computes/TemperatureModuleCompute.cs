using CodependantModules.Simulation;
using UnityEngine;

public class TemperatureModuleCompute : IModuleCompute
{
    private readonly TemperatureModule config;
    private readonly int temperatureIndex;
    private readonly int waterIndex;
    private readonly int floraIndex;
    private readonly int tectonicsIndex;
    private readonly int greenhouseIndex;
    private readonly int iceIndex;
    private readonly int vaporIndex;

    public TemperatureModuleCompute(TemperatureModule config, int temperatureIndex, int waterIndex, int floraIndex, 
        int tectonicsIndex, int greenhouseIndex, int iceIndex, int vaporIndex)
    {
        this.config = config;
        this.temperatureIndex = temperatureIndex;
        this.waterIndex = waterIndex;
        this.floraIndex = floraIndex;
        this.tectonicsIndex = tectonicsIndex;
        this.greenhouseIndex = greenhouseIndex;
        this.iceIndex = iceIndex;
        this.vaporIndex = vaporIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float temperature = snapshot.Get(temperatureIndex);
        float flora = snapshot.Get(floraIndex);
        float tectonics = snapshot.Get(tectonicsIndex);
        float greenhouseGases = snapshot.Get(greenhouseIndex);
        float liquidWater = snapshot.Get(waterIndex);
        float ice = snapshot.Get(iceIndex);
        float vapor = snapshot.Get(vaporIndex);

        float temperatureDerivative =
            + config.SolarHeating
            + config.GreenhouseHeating * greenhouseGases
            - config.WaterCooling * liquidWater
            - config.IceCooling * ice
            - config.PlantCooling * flora
            + config.TectonicHeating * tectonics
            + config.VaporHeating * vapor
            - config.RadiativeCooling * temperature;

        var output = new ModuleOutput(snapshot.Length);
        output.AddDerivative(temperatureIndex, temperatureDerivative);
        return output;
    }
}
