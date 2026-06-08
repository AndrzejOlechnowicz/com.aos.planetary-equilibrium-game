using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Ice")]
public class IceModule : SimulationModule
{
    public float FreezePoint => freezePoint;
    public float FreezeSharpness => freezeSharpness;
    public float FreezingRate => freezingRate;
    public float MeltingRate => meltingRate;

    [SerializeField] private float freezePoint = 0.1f;
    [SerializeField] private float freezeSharpness = 8f;
    [SerializeField] private float freezingRate = 0.03f;
    [SerializeField] private float meltingRate = 0.05f;

    [Header("Required Modules")]
    [SerializeField] private WaterModule waterModule;
    [SerializeField] private TemperatureModule temperatureModule;

    private int iceIndex;
    private int waterIndex;
    private int temperatureIndex;


    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        iceIndex = registry.GetIndex(Metadata.Id);
        waterIndex = registry.GetIndex(waterModule.Metadata.Id);
        temperatureIndex = registry.GetIndex(temperatureModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new IceModuleCompute(this, iceIndex, waterIndex, temperatureIndex);
    }
}

