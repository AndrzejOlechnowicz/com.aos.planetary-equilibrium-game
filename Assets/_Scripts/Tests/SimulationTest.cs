using CodependantModules.Simulation;
using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class SimulationTest
{
    [Test]
    public void Step_ShouldAdvanceTime()
    {
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };


        SimulationCore simulation = CreateFullSimulation(startValues, addValues, modulesModifiedIndices, 0, 0.1f, 0);

        simulation.Step();

        Assert.AreEqual(0.1f, simulation.CurrentState.Time);
    }

    [Test]
    public void Step_SholdIncrementStep()
    {
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };


        SimulationCore simulation = CreateFullSimulation(startValues, addValues, modulesModifiedIndices, 0, 0.1f, 0);

        simulation.Step();

        Assert.AreEqual(1f, simulation.CurrentState.Step);
    }

    [Test]
    public void Step_ShouldRaiseOnStepCompleted()
    {
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };

        SimulationCore simulation = CreateFullSimulation(startValues, addValues, modulesModifiedIndices, 0, 0.1f, 0);
        
        bool called = false;

        simulation.OnStepCompleted += (_,_) => called = true;

        simulation.Step();

        Assert.IsTrue(called);
    }

    [Test]
    public void Step_WhenModuleThrows_ShouldRaiseOnError()
    {
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };

        SimulationCore simulation = CreateFullSimulation(startValues, addValues, modulesModifiedIndices, 0, 0.1f, 0);

        MockIntegrator mockIntegrator = new MockIntegrator(new InvalidOperationException("Integrator failed"));
        simulation.SetIntegrator(mockIntegrator);

        bool called = false;

        simulation.OnError += (_) => called = true;

        LogAssert.Expect(LogType.Exception, "InvalidOperationException: Integrator failed");

        simulation.Step();

        Assert.IsTrue(called);
    }


    [Test]
    public void OutputsFromModules_AreSummed()
    {
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };


        SimulationCore simulation = CreateFullSimulation(startValues, addValues, modulesModifiedIndices, 0, 1, 0);
        
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
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };

        // setup the simulations
        SimulationCore simulationA = CreateFullSimulation(startValues, addValues, modulesModifiedIndices);
        SimulationCore simulationB = CreateFullSimulation(startValues, addValues, modulesModifiedIndices);

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
        float[] startValues = { 0, 0 };
        float[] addValues = { 0.1f, 0.2f };
        int[] modulesModifiedIndices = { 0, 0 };

        // setup the simulations
        SimulationCore simulationA = CreateFullSimulation(startValues, addValues, modulesModifiedIndices);
        SimulationCore simulationB = CreateFullSimulation(startValues, addValues, modulesModifiedIndices);

        for (int i = 0; i < 100; i++)
        {
            simulationA.Step();
        }

        simulationB.StepMultiple(100);

        Assert.AreEqual(simulationA.CurrentState, simulationB.CurrentState);
        Assert.AreEqual(simulationA.CurrentState.Step, simulationB.CurrentState.Step);
    }

    [Test]
    public void LongSimulation_ShouldRemainFiniteAndClamped()
    {
        float[] startValues = { 0.5f, 0.3f, 0.1f, 0.8f };
        float[] addValues = { 0.1f, 0.5f, -0.25f, 0.05f };
        int[] modulesModifiedIndices = { 1, 0, 2, 3 };

        var sim = CreateFullSimulation(startValues, addValues, modulesModifiedIndices);
    
        sim.StepMultiple(100000);
    
        for (int i = 0; i < sim.CurrentState.Length; i++)
        {
            float v = sim.CurrentState.Get(i);
    
            Assert.IsFalse(float.IsNaN(v));
            Assert.IsFalse(float.IsInfinity(v));
    
            Assert.That(v, Is.InRange(0f, 1f));
        }
    }

    private SimulationCore CreateFullSimulation(float[] startValues, float[] addValues, int[] modulesModifiedIndices, double time = 0.0, float dt = 0.02f, int step = 0)
    {
        Assert.AreEqual(startValues.Length, addValues.Length);
        Assert.AreEqual(addValues.Length, modulesModifiedIndices.Length);

        IModuleConfig[] modules = new IModuleConfig[addValues.Length];

        for (int i = 0; i < modules.Length; i++)
        {
            modules[i] = new ModuleConfig(addValues[i], modulesModifiedIndices[i]);
        }

        // initiate the modules registry
        ModuleRegistry registry = new ModuleRegistry(modules);
        // create the integrator
        IIntegrator integrator = new SemiImplicitIntegrator();
        // create the entry state
        SystemState state = new SystemState(startValues, time, dt, step);

        return new SimulationCore(registry, integrator, state);
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

    public sealed class MockIntegrator : IIntegrator
    {
        private readonly Exception _exceptionToThrow;

        public MockIntegrator(Exception exceptionToThrow = null)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        public SystemState Step(
            in SystemState current,
            ModuleOutput aggregatedOutput,
            float deltaTime)
        {
            if (_exceptionToThrow != null)
            {
                throw _exceptionToThrow;
            }

            // Return something valid for non-error tests
            return current;
        }
    }
}
