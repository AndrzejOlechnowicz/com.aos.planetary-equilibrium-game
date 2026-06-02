using UnityEngine;

namespace CodependantModules.Simulation
{
    public static class Helpers
    {
        public static float Sigmoid(float x)
        {
            return 1f / (1f + Mathf.Exp(-x));
        }

        public static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Mathf.InverseLerp(edge0, edge1, x);
            return t * t * (3f - 2f * t);
        }
    }
}