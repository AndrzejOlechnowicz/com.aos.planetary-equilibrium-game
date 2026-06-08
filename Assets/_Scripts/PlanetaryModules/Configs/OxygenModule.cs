using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Oxygen")]
public class OxygenModule : SimulationModule
{
    public float PlantOxygenProduction => plantOxygenProduction;
    public float OxygenLoss => oxygenLoss;
    public float FaunaOxygenConsumption => faunaOxygenConsumption;

    [Header("Atmosphere Coefficients")]
    [SerializeField] private float plantOxygenProduction = 0.03f;
    [SerializeField] private float oxygenLoss = 0.05f;
    [SerializeField] private float faunaOxygenConsumption = 0.01f;

    [Header("Required Modules")]
    [SerializeField] private FloraModule floraModule;
    [SerializeField] private FaunaModule faunaModule;

    private int oxygenIndex;
    private int floraIndex;
    private int faunaIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        oxygenIndex = registry.GetIndex(Metadata.Id);
        floraIndex = registry.GetIndex(floraModule.Metadata.Id);
        faunaIndex = registry.GetIndex(faunaModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new OxygenModuleCompute(this, oxygenIndex, floraIndex, faunaIndex);
    }
}
