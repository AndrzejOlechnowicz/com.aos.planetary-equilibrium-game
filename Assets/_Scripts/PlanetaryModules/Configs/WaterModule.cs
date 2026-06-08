using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Water")]
public class WaterModule : SimulationModule
{
    [Header("Required Modules")]
    [SerializeField] private TemperatureModule temperatureModule;
    [SerializeField] private IceModule iceModule;
    [SerializeField] private VaporModule vaporModule;

    private int waterIndex;
    private int temperatureIndex;
    private int iceIndex;
    private int vaporIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        waterIndex = registry.GetIndex(Metadata.Id);
        temperatureIndex = registry.GetIndex(temperatureModule.Metadata.Id);
        iceIndex = registry.GetIndex(iceModule.Metadata.Id);
        vaporIndex = registry.GetIndex(vaporModule.Metadata.Id);
    }

    public override string GetReadableData(float value)
    {
        return value.ToString("P0");
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new WaterModuleCompute(this, iceModule, vaporModule, waterIndex, iceIndex, vaporIndex, temperatureIndex);
    }
}
