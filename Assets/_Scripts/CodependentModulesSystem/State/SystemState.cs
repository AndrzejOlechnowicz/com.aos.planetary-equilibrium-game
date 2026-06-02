using System;
using UnityEngine;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// PlanetState: authoritative snapshot stored as an array of floats.
    /// - All state variables are stored in Values[] so adding new variables is a schema operation only.
    /// - SimulationCore and modules operate by index (use a separate StateSchema to map names to indices).
    /// </summary>
    [Serializable]
    public class SystemState
    {
        public float DeltaTime => dT;
        public double Time => time;
        public int Step => step;

        private float[] Values; // length == stateCount

        // meta
        private double time;
        private float dT;
        private int step;

        public SystemState(int stateCount, double time = 0.0, float dt = 0.02f, int step = 0)
        {
            if (stateCount <= 0) throw new ArgumentException("stateCount must be > 0");
            Values = new float[stateCount];
            this.time = time;
            dT = dt;
            this.step = step;
        }

        public SystemState(float[] values, double time = 0.0, float dt = 0.02f, int step = 0)
        {
            Values = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                Values[i] = values[i];
            }
            this.time = time;
            dT = dt;
            this.step = step;
        }

        public int Length => Values?.Length ?? 0;
        public float Get(int index) => Values[index];
        public void SetValues(float[] values) => Values = values;

        /// <summary>
        /// Apply an additive impulse to a single state index. The modified state variable is clamped to [0,1]
        /// </summary>
        public SystemState WithImpulse(int index, float magnitude)
        {
            var copy = this;
            var arr = (float[])Values.Clone();

            var valueWithImpulse = arr[index] + magnitude;
            if (valueWithImpulse > 1 || valueWithImpulse < 0)
                valueWithImpulse = Mathf.Clamp01(valueWithImpulse);

            arr[index] = valueWithImpulse;
            copy.Values = arr;
            return copy;
        }

        /// <summary>
        /// Clamp all core variables to [0,1].
        /// </summary>
        public SystemState Clamped()
        {
            var copy = this;
            var arr = (float[])Values.Clone();
            for (int i = 0; i < arr.Length; ++i) 
                arr[i] = Mathf.Clamp01(arr[i]);
            copy.Values = arr;
            return copy;
        }

        public SystemState WithTime(double time, int step, float dt)
        {
            var s = this;
            s.time = time;
            s.step = step;
            s.dT = dt;
            return s;
        }
    }
}
