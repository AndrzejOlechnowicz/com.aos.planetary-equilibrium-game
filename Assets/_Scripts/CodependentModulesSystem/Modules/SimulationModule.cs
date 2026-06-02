using UnityEngine;

namespace CodependantModules.Simulation
{
    public abstract class SimulationModule : ScriptableObject, IModuleConfig
    {
        [SerializeField] private ModuleMetadata metadata;

        protected ModuleRegistry moduleRegistry;
        protected int seed;

        public ModuleMetadata Metadata => metadata;

        public virtual void Init(ModuleRegistry moduleRegistry, int seed)
        {
            this.moduleRegistry = moduleRegistry;
            this.seed = seed;
        }

        public virtual string GetReadableData(float value)
        {
            return value.ToString("P0");
        }

        public virtual IModuleCompute GetModuleCompute()
        {
            return null;
        }
    }
}

