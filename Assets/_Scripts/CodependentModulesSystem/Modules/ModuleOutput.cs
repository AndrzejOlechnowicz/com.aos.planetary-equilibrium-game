using System.Collections.Generic;
using System;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// ModuleOutput: typed container modules return from Compute.
    /// - Derivatives: dX (not scaled by dt)
    /// - LinearCoeffs: summed linear coefficients for semi-implicit integration
    /// Designed for efficient aggregation by SimulationCore/ModuleRegistry.
    /// </summary>
    [Serializable]
    public class ModuleOutput
    {
        public float[] Derivatives;   // length == stateCount
        public float[] LinearCoeffs;  // length == stateCount

        public ModuleOutput(int stateCount)
        {
            if (stateCount <= 0) throw new ArgumentException("stateCount must be > 0");
            Derivatives = new float[stateCount];
            LinearCoeffs = new float[stateCount];
        }

        public int Length => Derivatives.Length;

        public void AddDerivative(int index, float value) => Derivatives[index] += value;
        public void AddLinearCoeff(int index, float value) => LinearCoeffs[index] += value;

        /// <summary>
        /// Merge another ModuleOutput into this one (summing arrays and merging diagnostics).
        /// </summary>
        public void Merge(ModuleOutput other)
        {
            if (other == null) return;
            if (other.Length != Length) throw new ArgumentException("ModuleOutput lengths must match");
            for (int i = 0; i < Length; ++i)
            {
                Derivatives[i] += other.Derivatives[i];
                LinearCoeffs[i] += other.LinearCoeffs[i];
            }
        }

        public void Clear()
        {
            Array.Clear(Derivatives, 0, Derivatives.Length);
            Array.Clear(LinearCoeffs, 0, LinearCoeffs.Length);
        }
    }
}
