using CodependantModules.Simulation;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class SimulationTest
{
    [Test]
    public void OutputsFromModules_AreSummed()
    {
        ModuleConfig moduleA = new ModuleConfig(0.1f, 0);
        ModuleConfig moduleB = new ModuleConfig(0.2f, 0);
        
        var modules = new IModuleConfig[] { moduleA, moduleB };
        
        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(modules);
        
        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();
        
        // setup the simulation core
        SimulationCore simulation = new SimulationCore(moduleRegistry, integrator, new SystemState(modules.Length, 0, 1, 0));
        
        simulation.Step();

        SystemState stateAfterStep = simulation.CurrentState;

        var stepValueA = stateAfterStep.Get(0);
        var stepValueB = stateAfterStep.Get(1);

        var expectedStepValueA = 0.3f;
        var expectedStepValueB = 0;

        Assert.That(stepValueA, Is.EqualTo(expectedStepValueA));
        Assert.That(stepValueB, Is.EqualTo(expectedStepValueB));
    }

    [Test]
    public void Simulation_IsDeterministic()
    {
        ModuleConfig moduleA = new ModuleConfig(0.1f, 1);
        ModuleConfig moduleB = new ModuleConfig(0.2f, 0);

        var modules = new IModuleConfig[] { moduleA, moduleB };

        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(modules);

        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();

        // setup the simulation core
        SimulationCore simulationA = new SimulationCore(moduleRegistry, integrator, new SystemState(modules.Length));
        SimulationCore simulationB = new SimulationCore(moduleRegistry, integrator, new SystemState(modules.Length));

        for (int i = 0; i < 100; i++)
        {
            simulationA.Step();
            simulationB.Step();
        }

        Assert.That(simulationA.CurrentState, Is.EqualTo(simulationB.CurrentState));
    }

    [Test]
    public void Simulation_MultipleStepsProducesSameResult()
    {
        ModuleConfig moduleA = new ModuleConfig(0.1f, 1);
        ModuleConfig moduleB = new ModuleConfig(0.2f, 0);

        var modules = new IModuleConfig[] { moduleA, moduleB };

        // initiate the modules registry
        var moduleRegistry = new ModuleRegistry(modules);

        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();

        // setup the simulation core
        SimulationCore simulationA = new SimulationCore(moduleRegistry, integrator, new SystemState(modules.Length));
        SimulationCore simulationB = new SimulationCore(moduleRegistry, integrator, new SystemState(modules.Length));

        for (int i = 0; i < 100; i++)
        {
            simulationA.Step();
        }

        simulationB.StepMultiple(100);

        Assert.AreEqual(simulationA.CurrentState, simulationB.CurrentState);
        Assert.AreEqual(simulationA.CurrentState.Step, simulationB.CurrentState.Step);
    }

    public class ModuleConfig : IModuleConfig
    {
        public ModuleMetadata Metadata => null;

        private float addValue;
        private int targetIndex;
        public ModuleConfig(float addValue, int targetIndex)
        {
            this.addValue = addValue;
            this.targetIndex = targetIndex;
        }

        public IModuleCompute GetModuleCompute()
        {
            return new ModuleCompute(addValue, targetIndex);
        }

        public string GetReadableData(float value)
        {
            return value.ToString("n2");
        }

        public void Init(ModuleRegistry moduleRegistry, int seed)
        {
            
        }
    }

    public class ModuleCompute : IModuleCompute
    {
        private float addValue;
        private int targetIndex;

        public ModuleCompute(float addValue, int targetIndex)
        {
            this.addValue = addValue;
            this.targetIndex = targetIndex;
        }

        public ModuleOutput Compute(in SystemState snapshot, double t)
        {
            var snapshotValue = snapshot.Get(targetIndex);

            var newValue = snapshotValue + addValue;

            ModuleOutput output = new ModuleOutput(snapshot.Length);
            output.AddDerivative(targetIndex, newValue);

            return output;
        }
    }
}
