using NUnit.Framework;
using UnityEngine;
using CodependantModules.Simulation;
using System;

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

    [Test]
    public void Step_WithZeroAlpha_ShouldBehaveLikeEuler()
    {
        var state = new SystemState(new[] { 1f });

        var output = new ModuleOutput(1);
        output.Derivatives[0] = 2f;

        var integrator = new SemiImplicitIntegrator();

        var next = integrator.Step(state, output, 0.5f);

        // 2f, because: 1 + 0.5 * 2 = 2
        Assert.AreEqual(2f, next.Get(0), 0.0001f);
    }

    [Test]
    public void Step_WithPositiveAlpha_ShouldApplyDamping()
    {
        var state = new SystemState(new[] { 1f });

        var output = new ModuleOutput(1);
        output.LinearCoeffs[0] = 1f;

        var integrator = new SemiImplicitIntegrator();

        var next = integrator.Step(state, output, 1f);

        // 0.5f, because: 1 / (1 + 1) = 0.5
        Assert.AreEqual(0.5f, next.Get(0), 0.0001f);
    }

    [Test]
    public void Step_WithInvalidDt_ShouldThrow()
    {
        var state = new SystemState(new[] { 1f });

        var output = new ModuleOutput(1);

        var integrator = new SemiImplicitIntegrator();

        Assert.Throws<ArgumentException>(() =>
            integrator.Step(state, output, 0f));
    }

    [Test]
    public void Step_WithLengthMismatch_ShouldThrow()
    {
        var state = new SystemState(new[] { 1f });

        var output = new ModuleOutput(2);

        var integrator = new SemiImplicitIntegrator();

        Assert.Throws<ArgumentException>(() =>
            integrator.Step(state, output, 1f));
    }


}
