using CodependantModules.Simulation;
using UnityEngine;

[CreateAssetMenu(menuName = "Planetary/Modules/Tectonics")]
public class TectonicsModule : SimulationModule
{
    private int tectonicsIndex;

    public override void Init(ModuleRegistry registry, int seed)
    {
        base.Init(registry, seed);
        tectonicsIndex = registry.GetIndex(Metadata.Id);
    }

    public override IModuleCompute GetModuleCompute()
    {
        return new TectonicsModuleCompute(tectonicsIndex, seed);
    }
}
