using CodependantModules.Simulation;
using System;
using UnityEngine;

public class PlanetVisual : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Material material;

    [SerializeField] private AnimationCurve temperatureCurve;
    [SerializeField] private AnimationCurve floraCurve;

    private readonly string waterLevelMatProperty = "_WaterLevel";
    private readonly string temperatureLevelMatProperty = "_Temperature";
    private readonly string floraLevelMatProperty = "_FloraLevel";

    private int waterId;
    private int temperatureId;
    private int floraId;

    private WaterModule waterModule;

    private void Start()
    {
        waterId = gameManager.Simulation.ModuleRegistry.GetIndex("water");
        temperatureId = gameManager.Simulation.ModuleRegistry.GetIndex("temperature");
        floraId = gameManager.Simulation.ModuleRegistry.GetIndex("flora");

        waterModule = gameManager.Simulation.ModuleRegistry.Modules[waterId] as WaterModule;

        gameManager.Simulation.OnStepCompleted += UpdateVisual;
    }

    private void OnDestroy()
    {
        gameManager.Simulation.OnStepCompleted -= UpdateVisual;
    }

    private void UpdateVisual(SystemState state, ModuleOutput output)
    {
        float allWater = gameManager.Simulation.CurrentState.Get(waterId);
        //float liquidWaterFraction = waterModule.LiquidFraction;
        float temperature = gameManager.Simulation.CurrentState.Get(temperatureId);
        float flora = gameManager.Simulation.CurrentState.Get(floraId);

        //float tempCurveEval = temperatureCurve.Evaluate(temperature);
        //float floraCurveEval = floraCurve.Evaluate(flora);

        material.SetFloat(waterLevelMatProperty, allWater);
        material.SetFloat(temperatureLevelMatProperty, temperatureCurve.Evaluate(temperature));
        material.SetFloat(floraLevelMatProperty, floraCurve.Evaluate(flora));

        //Debug.Log($"Temperature value: {temperature}, curved: {tempCurveEval}");
    }
}
