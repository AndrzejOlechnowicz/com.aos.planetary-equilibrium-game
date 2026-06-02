using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Fauna")]
public class FaunaModule : SimulationModule
{
    public float TemperatureMin => temperatureMin;
    public float TemperatureOpt => temperatureOpt;
    public float WaterMin => waterMin;
    public float WaterOpt => waterOpt;
    public float OxygenMin => oxygenMin;
    public float OxygenOpt => oxygenOpt;
    public float FaunaGrowthRate => faunaGrowthRate;
    public float FaunaDecayRate => faunaDecayRate;


    [Header("Climate Windows")]
    [SerializeField] private float temperatureMin = 0.3f;
    [SerializeField] private float temperatureOpt = 0.5f;

    [SerializeField] private float waterMin = 0.3f;
    [SerializeField] private float waterOpt = 0.5f;

    [SerializeField] private float oxygenMin = 0.3f;
    [SerializeField] private float oxygenOpt = 0.5f;

    [Header("Fauna Coefficients")]
    [SerializeField] private float faunaGrowthRate = 0.4f;
    [SerializeField] private float faunaDecayRate = 0.1f;

    [Header("Required Modules")]
    [SerializeField] private FloraModule floraModule;
    [SerializeField] private TemperatureModule temperatureModule;
    [SerializeField] private WaterModule waterModule;
    [SerializeField] private OxygenModule oxygenModule;

    private int faunaIndex;
    private int floraIndex;
    private int temperatureIndex;
    private int waterIndex;
    private int oxygenIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        faunaIndex = registry.GetIndex(Metadata.Id);
        floraIndex = registry.GetIndex(floraModule.Metadata.Id);
        temperatureIndex = registry.GetIndex(temperatureModule.Metadata.Id);
        waterIndex = registry.GetIndex(waterModule.Metadata.Id);
        oxygenIndex = registry.GetIndex(oxygenModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new FaunaModuleCompute(this, faunaIndex, floraIndex, temperatureIndex, waterIndex, oxygenIndex);
    }
}
