using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Vapor")]
public class VaporModule : SimulationModule
{
    public float EvaporationPoint => evaporationPoint;
    public float VaporSharpness => vaporSharpness;
    public float EvaporationRate => evaporationRate;
    public float CondensationRate => condensationRate;

    [SerializeField] private float evaporationPoint = 0.75f;
    [SerializeField] private float vaporSharpness = 10f;
    [SerializeField] private float evaporationRate = 0.04f;
    [SerializeField] private float condensationRate = 0.03f;

    [Header("Required Modules")]
    [SerializeField] private WaterModule waterModule;
    [SerializeField] private TemperatureModule temperatureModule;

    private int vaporIndex;
    private int waterIndex;
    private int temperatureIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        vaporIndex = registry.GetIndex(Metadata.Id);
        waterIndex = registry.GetIndex(waterModule.Metadata.Id);
        temperatureIndex = registry.GetIndex(temperatureModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new VaporModuleCompute(this, vaporIndex, waterIndex, temperatureIndex);
    }
}

