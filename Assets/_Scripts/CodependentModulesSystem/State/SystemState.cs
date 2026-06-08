using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodependantModules.Simulation
{
    /// <summary>
    /// PlanetState: authoritative snapshot stored as an array of floats.
    /// - All state variables are stored in Values[] so adding new variables is a schema operation only.
    /// - SimulationCore and modules operate by index (use a separate StateSchema to map names to indices).
    /// </summary>
    [Serializable]
    public class SystemState : IEquatable<SystemState>
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
        public float Get(int index)
        {
            if (index < 0 || index >= Values.Length)
                return 0;

            return Values[index];
        }
        public SystemState WithValues(float[] values)
        {
            return Copy(values);
        }

        public bool Equals(SystemState other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (time != other.time ||
                dT != other.dT ||
                step != other.step)
                return false;

            if (Values == null || other.Values == null)
                return Values == other.Values;

            if (Values.Length != other.Values.Length)
                return false;

            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] != other.Values[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SystemState);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(time);
            hash.Add(dT);
            hash.Add(step);

            if (Values != null)
            {
                foreach (var value in Values)
                    hash.Add(value);
            }

            return hash.ToHashCode();
        }

        public static bool operator ==(SystemState left, SystemState right)
        {
            return EqualityComparer<SystemState>.Default.Equals(left, right);
        }

        public static bool operator !=(SystemState left, SystemState right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Apply an additive impulse to a single state index. The modified state variable is clamped to [0,1]
        /// </summary>
        public SystemState WithImpulse(int index, float magnitude)
        {
            float[] values = new float[Values.Length];

            for (int i = 0; i < Values.Length; i++)
                values[i] = Values[i];

            values[index] = Mathf.Clamp01(values[index] + magnitude);

            return Copy(values: values);
        }

        /// <summary>
        /// Clamp all core variables to [0,1].
        /// </summary>
        public SystemState Clamped()
        {
            float[] values = new float[Values.Length];

            for (int i = 0; i < Values.Length; i++)
                values[i] = Mathf.Clamp01(Values[i]);

            return Copy(values: values);
        }

        public SystemState WithTime(double time, int step, float dt)
        {
            return Copy(time: time, step: step, dt: dt);
        }

        private SystemState Copy(float[] values = null, double? time = null, float? dt = null, int? step = null)
        {
            return new SystemState(
                values ?? (float[])Values.Clone(),
                time ?? this.time,
                dt ?? dT,
                step ?? this.step);
        }
    }
}
