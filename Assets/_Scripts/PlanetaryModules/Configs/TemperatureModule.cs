using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Temperature")]
public class TemperatureModule : SimulationModule
{
    public float SolarHeating => solarHeating;
    public float GreenhouseHeating => greenhouseHeating;
    public float WaterCooling => waterCooling;
    public float PlantCooling => plantCooling;
    public float TectonicHeating => tectonicHeating;
    public float RadiativeCooling => radiativeCooling;
    public float IceCooling => iceCooling;
    public float VaporHeating => vaporHeating;


    [Header("Temperature Coefficients")]
    [SerializeField] private float solarHeating = 0.01f;
    [SerializeField] private float greenhouseHeating = 0.1f;
    [SerializeField] private float waterCooling = 0.1f;
    [SerializeField] private float plantCooling = 0.15f;
    [SerializeField] private float tectonicHeating = 0.02f;
    [SerializeField] private float radiativeCooling = 0.05f;
    [SerializeField] private float iceCooling = 0.1f;
    [SerializeField] private float vaporHeating = 0.1f;

    [Header("Modules Required")]
    [SerializeField] private WaterModule waterModule;
    [SerializeField] private OxygenModule oxygenModule;
    [SerializeField] private FloraModule floraModule;
    [SerializeField] private TectonicsModule tectonicsModule;
    [SerializeField] private GreenhouseGasModule greenhouseGasModule;
    [SerializeField] private IceModule iceModule;
    [SerializeField] private VaporModule vaporModule;

    private int temperatureIndex;
    private int atmosphereIndex;
    private int waterIndex;
    private int floraIndex;
    private int tectonicsIndex;
    private int greenhouseIndex;
    private int iceIndex;
    private int vaporIndex;

    private float boilPoint;
    private float freezePoint;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        temperatureIndex = registry.GetIndex(Metadata.Id);
        atmosphereIndex = registry.GetIndex(oxygenModule.Metadata.Id);
        waterIndex = registry.GetIndex(waterModule.Metadata.Id);
        floraIndex = registry.GetIndex(floraModule.Metadata.Id);
        tectonicsIndex = registry.GetIndex(tectonicsModule.Metadata.Id);
        greenhouseIndex = registry.GetIndex(greenhouseGasModule.Metadata.Id);
        iceIndex = registry.GetIndex(iceModule.Metadata.Id);
        vaporIndex = registry.GetIndex(vaporModule.Metadata.Id);

        freezePoint = iceModule.FreezePoint;
        boilPoint = vaporModule.EvaporationPoint;
    }

    public override string GetReadableData(float value)
    {
        return value.ToString("N2");//NormalizedToCelsius(value, freezePoint, boilPoint).ToString("n0") + "°C";
    }

    private float NormalizedToCelsius(float tNorm, float freezeNorm, float boilNorm)
    {
        return (tNorm - freezeNorm) / (boilNorm - freezeNorm) * 100f;
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new TemperatureModuleCompute(this, temperatureIndex, waterIndex, floraIndex, tectonicsIndex,
            greenhouseIndex, iceIndex, vaporIndex);
    }
}
