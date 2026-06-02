using System;
using UnityEngine;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// Central simulation runner that:
    /// - Aggregates ModuleOutput from registered modules
    /// - Calls the IIntegrator to advance the state
    /// - Applies impulses and clamps the state
    /// </summary>
    public class SimulationCore
    {
        public SystemState CurrentState { get; private set; }
        public ModuleRegistry ModuleRegistry { get { return _moduleRegistry; } }

        // Event hooks
        public event Action<SystemState, ModuleOutput> OnStepCompleted;
        public event Action<Exception> OnError;

        private readonly ModuleRegistry _moduleRegistry;
        private IIntegrator _integrator;

        // Working buffers sized to current schema
        private ModuleOutput _aggregatedOutput;
        private readonly object _lock = new object();

        private int tectonicsModuleIndex;

        public SimulationCore(
            ModuleRegistry moduleRegistry,
            IIntegrator integrator,
            SystemState initialState)
        {
            _moduleRegistry = moduleRegistry ?? throw new ArgumentNullException(nameof(moduleRegistry));
            _integrator = integrator ?? throw new ArgumentNullException(nameof(integrator));
            CurrentState = initialState;

            //equilibriumDerivativesMax = CurrentState.Dt / 2;

            EnsureBuffersMatchState();
        }

        /// <summary>
        /// Ensure internal ModuleOutput buffer matches the current state length.
        /// Call after schema changes or when replacing CurrentState.
        /// </summary>
        private void EnsureBuffersMatchState()
        {
            int n = CurrentState.Length;
            if (_aggregatedOutput == null || _aggregatedOutput.Derivatives.Length != n)
            {
                _aggregatedOutput = new ModuleOutput(n);
            }
        }

        /// <summary>
        /// Replace the integrator at runtime (e.g., switch to semi-implicit).
        /// </summary>
        public void SetIntegrator(IIntegrator integrator)
        {
            _integrator = integrator ?? throw new ArgumentNullException(nameof(integrator));
        }

        /// <summary>
        /// Apply a direct impulse to a single state index (player action).
        /// Magnitude is additive and clamped after application.
        /// </summary>
        public void ApplyImpulse(int stateIndex, float magnitude)
        {
            lock (_lock)
            {
                CurrentState = CurrentState.WithImpulse(stateIndex, magnitude);
            }
        }

        /// <summary>
        /// Single simulation step: aggregate module outputs, call integrator, clamp, and publish events.
        /// This method is deterministic if modules and integrator are deterministic.
        /// </summary>
        public void Step()
        {
            try
            {
                lock (_lock)
                {
                    _aggregatedOutput.Clear();

                    // Snapshot for deterministic module compute calls
                    var snapshot = CurrentState;

                    // Aggregate outputs from all registered modules
                    foreach (var module in _moduleRegistry.Computes)
                    {
                        var outp = module.Compute(in snapshot, snapshot.Time);
                        if (outp == null) continue;

                        _aggregatedOutput.Merge(outp);
                    }

                    // Call integrator to compute next state (not clamped)
                    var next = _integrator.Step(in snapshot, _aggregatedOutput, CurrentState.DeltaTime);

                    // Apply clamping and update time/step
                    next = next.Clamped();
                    next = next.WithTime(CurrentState.Time + CurrentState.DeltaTime, CurrentState.Step + 1, CurrentState.DeltaTime);

                    // Commit
                    CurrentState = next;

                    // Publish diagnostics/event
                    OnStepCompleted?.Invoke(CurrentState, _aggregatedOutput);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Convenience: run multiple steps in a tight loop (useful for accelerated time).
        /// </summary>
        public void StepMultiple(int steps)
        {
            if (steps <= 0) return;
            for (int i = 0; i < steps; ++i) Step();
        }
    }
}
