using System;
using UnityEngine;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// Semi-implicit integrator (diagonal implicit for linear damping terms).
    /// For each state variable X:
    ///   dX = deriv - alpha * X   (modules should add alpha to LinearCoeffs when they contribute -alpha * X)
    /// Implicit update:
    ///   X_next = (X + dt * deriv) / (1 + dt * alpha)
    /// This is stable for positive alpha and allows larger dt for linear damping.
    /// </summary>
    public class SemiImplicitIntegrator : IIntegrator
    {
        /// <summary>
        /// Safety threshold to treat very small alpha as zero.
        /// </summary>
        public float AlphaEpsilon = 1e-8f;

        /// <summary>
        /// If true, negative linear coefficients are treated as explicit contributions (logged).
        /// </summary>
        public bool TreatNegativeLinearAsExplicit = true;

        public SystemState Step(in SystemState snapshot, ModuleOutput aggregated, float dt)
        {
            if (dt <= 0f) throw new ArgumentException("dt must be > 0", nameof(dt));
            if (aggregated == null) throw new ArgumentNullException(nameof(aggregated));

            int nState = snapshot.Length;
            if (aggregated.Derivatives.Length != nState || aggregated.LinearCoeffs.Length != nState)
                throw new ArgumentException("Aggregated ModuleOutput length must match snapshot length");

            // Prepare next state array
            var nextArr = new float[nState];

            // Per-variable semi-implicit update (diagonal implicit)
            for (int i = 0; i < nState; ++i)
            {
                float x = snapshot.Get(i);
                float deriv = aggregated.Derivatives[i];       // explicit part (may include nonlinear terms)
                float alpha = aggregated.LinearCoeffs[i];      // expected to be >= 0 for -alpha * X contributions

                // Defensive: treat tiny alpha as zero
                if (Mathf.Abs(alpha) < AlphaEpsilon) alpha = 0f;

                if (alpha >= 0f)
                {
                    // Implicit linear damping handled on denominator
                    // X_next = (X + dt * deriv) / (1 + dt * alpha)
                    float denom = 1f + dt * alpha;
                    // Avoid division by zero (shouldn't happen because denom >= 1)
                    if (denom <= 0f) denom = 1e-6f;
                    nextArr[i] = (x + dt * deriv) / denom;
                }
                else
                {
                    // Negative alpha: modules provided a negative linear coeff (i.e., +beta * X).
                    // Option A: treat as explicit (X_next = X + dt*(deriv + (-alpha)*X) )
                    // Option B: treat as implicit with negative denom (unstable). We choose A by default.
                    if (TreatNegativeLinearAsExplicit)
                    {
                        float explicitDeriv = deriv - alpha * x; // since alpha < 0, -alpha*x is positive contribution
                        nextArr[i] = x + dt * explicitDeriv;
                        Debug.LogWarning($"SemiImplicitIntegrator: negative linear coeff for index {i}. Treated as explicit.");
                    }
                    else
                    {
                        // Fallback implicit (risky)
                        float denom = 1f + dt * alpha;
                        if (Mathf.Abs(denom) < 1e-6f) denom = 1e-6f;
                        nextArr[i] = (x + dt * deriv) / denom;
                    }
                }

                // Safety: guard against NaN/Inf
                if (float.IsNaN(nextArr[i]) || float.IsInfinity(nextArr[i]))
                {
                    Debug.LogError($"Integrator produced invalid value at index {i}. Reverting to explicit Euler fallback.");
                    nextArr[i] = x + dt * deriv;
                }
            }

            // Build next SystemState
            return snapshot.WithValues(nextArr);
        }
    }
}
