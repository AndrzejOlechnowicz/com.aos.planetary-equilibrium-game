using CodependantModules.Simulation;
using UnityEngine;

public class FloraModuleCompute : IModuleCompute
{
    private readonly FloraModule config;
            
    private readonly int floraIndex;
    private readonly int temperatureIndex;
    private readonly int waterIndex;
    private readonly int faunaIndex;
    private readonly int oxygenIndex;

    public FloraModuleCompute(FloraModule config, int floraIndex, int temperatureIndex, int waterIndex, int faunaIndex, int oxygenIndex)
    {
        this.config = config;
        this.floraIndex = floraIndex;
        this.temperatureIndex = temperatureIndex;
        this.waterIndex = waterIndex;
        this.faunaIndex = faunaIndex;
        this.oxygenIndex = oxygenIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double time)
    {
        float flora = snapshot.Get(floraIndex);
        float temperature = snapshot.Get(temperatureIndex);
        float oxygen = snapshot.Get(oxygenIndex);
        float fauna = snapshot.Get(faunaIndex);

        float liquidWaterFraction = snapshot.Get(waterIndex);

        float temperatureSuitability = Helpers.SmoothStep(config.TemperatureMin, config.TemperatureOpt, temperature);
        float waterSuitability = Helpers.SmoothStep(config.WaterMin, config.WaterOpt, liquidWaterFraction);
        float oxygenSuitability = Helpers.SmoothStep(config.OxygenMin, config.OxygenOptimal, oxygen);

        float growthFactor = temperatureSuitability * waterSuitability * oxygenSuitability;
        float decayFactor = config.TemperatureStress * Mathf.Abs(temperature - config.TemperatureOpt);

        float floraDerivative =
            + config.FloraGrowthRate * flora * growthFactor
            - config.FloraDecayRate * flora * decayFactor
            - config.FloraConsumptionRate * fauna * flora
            + config.SpontaneousGrowthFactor * growthFactor;

        var output = new ModuleOutput(snapshot.Length);
        output.AddDerivative(floraIndex, floraDerivative);
        return output;
    }
}
