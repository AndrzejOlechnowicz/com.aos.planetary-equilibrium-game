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
}
