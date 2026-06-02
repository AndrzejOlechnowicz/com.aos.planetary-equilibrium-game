namespace CodependantModules.Simulation
{
    /// <summary>
    /// Integrator contract used by SimulationCore.
    /// Implementations should be deterministic and thread-safe if used from worker threads.
    /// </summary>
    public interface IIntegrator
    {
        /// <summary>
        /// Advance the state by dt using aggregated derivatives and linear coefficients.
        /// - snapshot: current state (array-backed)
        /// - aggregated: ModuleOutput containing summed derivatives and linear coeffs (not scaled by dt)
        /// - dt: timestep
        /// Returns the next PlanetState (not yet clamped).
        /// </summary>
        SystemState Step(in SystemState snapshot, ModuleOutput aggregated, float dt);
    }
}
