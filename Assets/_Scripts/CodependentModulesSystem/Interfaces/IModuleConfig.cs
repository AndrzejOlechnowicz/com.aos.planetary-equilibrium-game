namespace CodependantModules.Simulation
{
    /// <summary>
    /// Module interface for all simulation components.
    /// Implementations must be pure (no mutation of snapshot) and deterministic when possible.
    /// </summary>
    public interface IModuleConfig
    {
        /// <summary>
        /// Called once at startup. Use to read parameters and initialize deterministic RNG/noise.
        /// </summary>
        /// <param name="moduleRegistry">Modules registry</param>
        /// <param name="seed">Seed for deterministic noise</param>
        void Init(ModuleRegistry moduleRegistry, int seed);

        /// <summary>
        /// Provide metadata for editor, ordering, and dependency checks.
        /// </summary>
        ModuleMetadata Metadata { get; }

        string GetReadableData(float value);

        IModuleCompute GetModuleCompute();
    }
}