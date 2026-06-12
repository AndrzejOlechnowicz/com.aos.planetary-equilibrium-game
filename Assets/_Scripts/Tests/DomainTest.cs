using CodependantModules.Simulation;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class DomainTest
{
    [Test]
    public void WaterCycle_ConservesTotalMass()
    {
        var waterModule = Resources.Load<ScriptableObject>("Water Module") as WaterModule;
        var iceModule = Resources.Load<ScriptableObject>("Ice Module") as IModuleConfig;
        var vaporModule = Resources.Load<ScriptableObject>("Vapor Module") as IModuleConfig;
        var temperatureModule = Resources.Load<ScriptableObject>("Temperature Module") as IModuleConfig;

        Assert.IsNotNull(waterModule);
        Assert.IsNotNull(iceModule);
        Assert.IsNotNull(vaporModule);
        Assert.IsNotNull(temperatureModule);

        var modules = new IModuleConfig[] { waterModule, iceModule, vaporModule, temperatureModule};

        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(modules);

        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();

        // create the start values for the modules
        float waterStartValue = 0.5f;
        float iceStartValue = 0.2f;
        float vaporStartValue = 0.1f;
        float temperatureStartValue = 0.5f;

        var startValues = new float[]
        {
            waterStartValue,
            iceStartValue,
            vaporStartValue,
            temperatureStartValue
        };

        const int WaterIndex = 0;
        const int IceIndex = 1;
        const int VaporIndex = 2;

        float initialTotalMass = waterStartValue + iceStartValue + vaporStartValue;

        // setup the simulation core
        SimulationCore simulation = new SimulationCore(moduleRegistry, integrator, new SystemState(startValues, 0, 0.02f, 0));

        simulation.StepMultiple(100);

        float finalTotalMass = simulation.CurrentState.Get(WaterIndex) + simulation.CurrentState.Get(IceIndex) + simulation.CurrentState.Get(VaporIndex);
        
        Assert.AreEqual(initialTotalMass, finalTotalMass, 1e-5f);
    }

    [Test]
    public void WaterCycle_DerivativesConserveMass()
    {
        float water = 0.5f;
        float ice = 0.3f;
        float vapor = 0.2f;
        float temperature = 0.4f;

        var waterModule = Resources.Load<ScriptableObject>("Water Module") as WaterModule;
        var iceModule = Resources.Load<ScriptableObject>("Ice Module") as IModuleConfig;
        var vaporModule = Resources.Load<ScriptableObject>("Vapor Module") as IModuleConfig;

        var snapshot = new SystemState(new[]
        {
            water,
            ice,
            vapor,
            temperature
        });

        var outputs = new[]
        {
            waterModule.GetModuleCompute().Compute(snapshot, 0),
            iceModule.GetModuleCompute().Compute(snapshot, 0),
            vaporModule.GetModuleCompute().Compute(snapshot, 0)
        };

        float totalDerivative = 0;

        foreach (var output in outputs)
        {
            totalDerivative += output.Derivatives[0];
            totalDerivative += output.Derivatives[1];
            totalDerivative += output.Derivatives[2];
        }

        Assert.AreEqual(0f, totalDerivative, 1e-6f);
    }

    [Test]
    public void GreenhouseGasses_IncreaseTemperature()
    {
        var temperatureModule = Resources.Load<ScriptableObject>("Temperature Module") as IModuleConfig;
        var ghgModule = Resources.Load<ScriptableObject>("Greenhouse Gas Module") as IModuleConfig;

        var modules = new IModuleConfig[] { temperatureModule, ghgModule };

        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(modules);

        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();

        float temperature = 0.3f;
        float ghg = 0.6f;

        var snapshot = new SystemState(new[]
        {
            temperature,
            ghg
        });

        // setup the simulation core
        SimulationCore simulation = new SimulationCore(moduleRegistry, integrator, snapshot);

        simulation.StepMultiple(100);

        Assert.IsTrue(simulation.CurrentState.Get(0) > temperature);
    }
}
