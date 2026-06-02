using UnityEngine;

namespace CodependantModules.Simulation
{
    public interface IModuleCompute
    {
        ModuleOutput Compute(in SystemState snapshot, double t);
    }
}