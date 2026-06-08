using CodependantModules.Simulation;

public static class WaterPhasesMath
{
    public static TransitionSuitability GetSuitability(float temperature, float transitionPoint, float transitionSharpness)
    {
        float forwardTransition = 1f - Helpers.Sigmoid(transitionSharpness * (temperature - transitionPoint));

        return new TransitionSuitability(forwardTransition);
    }
}
