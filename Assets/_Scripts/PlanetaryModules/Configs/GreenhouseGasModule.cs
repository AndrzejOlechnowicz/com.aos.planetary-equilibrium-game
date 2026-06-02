using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/GreenhouseGas")]
public class GreenhouseGasModule : SimulationModule
{
    public float GreenhouseLoss => greenhouseLoss;
    public float VolcanicEmission => volcanicEmission;
    public float FaunaEmission => faunaEmission;
    public float FloraAbsorption => floraAbsorption;

    [Header("Atmosphere Coefficients")]
    [SerializeField] private float greenhouseLoss = 0.05f;
    [SerializeField] private float volcanicEmission = 0.005f;
    [SerializeField] private float faunaEmission = 0.01f;
    [SerializeField] private float floraAbsorption = 0.01f;

    [Header("Required Modules")]
    [SerializeField] private FloraModule floraModule;
    [SerializeField] private FaunaModule faunaModule;
    [SerializeField] private TectonicsModule tectonicsModule;

    private int greenhouseIndex;
    private int floraIndex;
    private int faunaIndex;
    private int tectonicsIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);

        greenhouseIndex = registry.GetIndex(Metadata.Id);
        floraIndex = registry.GetIndex(floraModule.Metadata.Id);
        faunaIndex = registry.GetIndex(faunaModule.Metadata.Id);
        tectonicsIndex = registry.GetIndex(tectonicsModule.Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new GreenhouseGasModuleCompute(this, greenhouseIndex, floraIndex, faunaIndex, tectonicsIndex);
    }
}

