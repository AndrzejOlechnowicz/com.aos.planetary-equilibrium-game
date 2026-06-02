using CodependantModules.Simulation;
using System;
using UnityEngine;

public class ScoreManager
{
    public float EQPoints { get { return equilibriumPoints; } }
    public bool IsEquilibrium { get { return isAtEquilibrium; } }
    public float Score => score;

    private float equilibriumPoints;
    private readonly float equilibriumGainRate = 1f;
    private float equilibriumDerivativesMax = 0.01f;
    private bool isAtEquilibrium;

    private float score = 0;

    private readonly int floraIndex, faunaIndex;
    private readonly float floraScoreDelta = 0.1f;
    private readonly float faunaScoreDelta = 0.5f;
    private readonly float scoringEpsilon = 0.05f;

    private readonly int tectonicsModuleIndex;

    public ScoreManager(GameManager gameManager, SimulationCore simulation)
    {
        gameManager.OnImpulseApplied += ReduceEQPoints;
        simulation.OnStepCompleted += AddEQPoints;

        tectonicsModuleIndex = gameManager.Simulation.ModuleRegistry.GetIndex("tectonics");
        floraIndex = gameManager.Simulation.ModuleRegistry.GetIndex("flora");
        faunaIndex = gameManager.Simulation.ModuleRegistry.GetIndex("fauna");
    }

    private void ReduceEQPoints()
    {
        equilibriumPoints -= GameManager.InputCost;
    }

    private void AddEQPoints(SystemState state, ModuleOutput stepOutput)
    {
        isAtEquilibrium = IsPlanetAtEquilibrium(stepOutput);
        if (isAtEquilibrium)
            equilibriumPoints += equilibriumGainRate * state.DeltaTime;

        if (state.Get(floraIndex) > scoringEpsilon)
            score += floraScoreDelta;

        if (state.Get(faunaIndex) > scoringEpsilon)
            score += faunaScoreDelta;
    }

    private bool IsPlanetAtEquilibrium(ModuleOutput aggregatedOutput)
    {
        bool result = true;
    
        for(int i = 0; i < aggregatedOutput.Derivatives.Length; i++)
        {
            if (i == tectonicsModuleIndex) // do not check tectonics as they are their own module (does not need other to calculate the dX)
                continue;
    
            if (Math.Abs(aggregatedOutput.Derivatives[i]) > equilibriumDerivativesMax)
                return false;
        }
    
        return result;
    }
}
