using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Flora")]
public class FloraModule : SimulationModule
{
    public float TemperatureMin => temperatureMin;
    public float TemperatureOpt => temperatureOpt;

    public float WaterMin => waterMin;
    public float WaterOpt => waterOpt;

    public float OxygenMin => oxygenMin;
    public float OxygenOptimal => oxygenOptimal;

    public float FloraGrowthRate => floraGrowthRate;
    public float FloraDecayRate => floraDecayRate;
    public float TemperatureStress => temperatureStress;
    public float FloraConsumptionRate => floraConsumptionRate;

    public float SpontaneousGrowthFactor => spontaneousGrowthFactor;

    [Header("Climate Windows")]
    [SerializeField] private float temperatureMin = 0.2f;
    [SerializeField] private float temperatureOpt = 0.5f;

    [SerializeField] private float waterMin = 0.2f;
    [SerializeField] private float waterOpt = 0.5f;

    [SerializeField] private float oxygenMin = 0.2f;
    [SerializeField] private float oxygenOptimal = 0.5f;

    [Header("Flora Coefficients")]
    [SerializeField] private float floraGrowthRate = 0.3f;
    [SerializeField] private float floraDecayRate = 0.1f;
    [SerializeField] private float temperatureStress = 1.0f;
    [SerializeField] private float floraConsumptionRate = 0.3f;

    [Header("Spontaneous Growth")]
    [SerializeField] private float spontaneousGrowthFactor = 0.01f;

    [Header("Required Modules")]
    [SerializeField] private TemperatureModule temperatureModule;
    [SerializeField] private WaterModule waterModule;
    [SerializeField] private FaunaModule faunaModule;
    [SerializeField] private OxygenModule oxygenModule;

    private int floraIndex;
    private int temperatureIndex;
    private int waterIndex;
    private int faunaIndex;
    private int oxygenIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        floraIndex = registry.GetIndex(Metadata.Id);
        temperatureIndex = registry.GetIndex(temperatureModule.Metadata.Id);
        waterIndex = registry.GetIndex(waterModule.Metadata.Id);
        faunaIndex = registry.GetIndex(faunaModule.Metadata.Id);
        oxygenIndex = registry.GetIndex(oxygenModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new FloraModuleCompute(this, floraIndex, temperatureIndex, waterIndex, faunaIndex, oxygenIndex);
    }
}
