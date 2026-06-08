using NUnit.Framework;
using UnityEngine;
using CodependantModules.Simulation;

[TestFixture]
public class IntegratorsTest
{
    [Test]
    public void ConstantDerivative_ProducesExpectedValue()
    {
        SemiImplicitIntegrator integrator = new SemiImplicitIntegrator();

        SystemState state = new SystemState(1);

        ModuleOutput aggregatedOutput = new ModuleOutput(1);
        aggregatedOutput.AddDerivative(0, 1);

        int expectedResult = 10;

        for (int i = 0; i < 10; i++)
        {
            state = integrator.Step(in state, aggregatedOutput, 1f);
        }

        var value = state.Get(0);
        
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Is.EqualTo(expectedResult));
    }

}
