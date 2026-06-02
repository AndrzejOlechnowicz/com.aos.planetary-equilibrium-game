using CodependantModules.Simulation;
using UnityEngine;

public class FaunaModuleCompute : IModuleCompute
{
    private readonly FaunaModule config;
            
    private readonly int faunaIndex;
    private readonly int floraIndex;
    private readonly int temperatureIndex;
    private readonly int waterIndex;
    private readonly int oxygenIndex;


    public FaunaModuleCompute(FaunaModule config, int faunaIndex, int floraIndex, int temperatureIndex, int waterIndex, int oxygenIndex)
    {
        this.config = config;
        this.faunaIndex = faunaIndex;
        this.floraIndex = floraIndex;
        this.temperatureIndex = temperatureIndex;
        this.waterIndex = waterIndex;
        this.oxygenIndex = oxygenIndex;
    }

    public ModuleOutput Compute(in SystemState snapshot, double t)
    {
        float fauna = snapshot.Get(faunaIndex);
        float flora = snapshot.Get(floraIndex);
        float temperature = snapshot.Get(temperatureIndex);
        float oxygen = snapshot.Get(oxygenIndex);

        float liquidWaterFraction = snapshot.Get(waterIndex);

        float temperatureSuitability = Helpers.SmoothStep(config.TemperatureMin, config.TemperatureOpt, temperature);
        float waterSuitability = Helpers.SmoothStep(config.WaterMin, config.WaterOpt, liquidWaterFraction);
        float oxygenSuitability = Helpers.SmoothStep(config.OxygenMin, config.OxygenOpt, oxygen);

        float climateFactor = temperatureSuitability * waterSuitability * oxygenSuitability;

        float faunaDerivative =
            + config.FaunaGrowthRate * fauna * flora * climateFactor
            - config.FaunaDecayRate * fauna * (1 - climateFactor);

        var output = new ModuleOutput(snapshot.Length);
        output.AddDerivative(faunaIndex, faunaDerivative);
        return output;
    }
}
