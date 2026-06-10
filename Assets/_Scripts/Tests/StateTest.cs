using CodependantModules.Simulation;
using NUnit.Framework;
using System;
using UnityEngine;

[TestFixture]
public class StateTest
{
    [Test]
    public void Constructor_WhenStateCountIsZero_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new SystemState(0));
    }

    [Test]
    public void Constructor_ShouldCopyInputArray()
    {
        var arr = new[] { 1f, 2f };

        var state = new SystemState(arr);

        arr[0] = 999f;

        Assert.AreEqual(1f, state.Get(0));
    }

    [Test]
    public void Get_OutOfBounds_ShouldReturnZero()
    {
        var state = new SystemState(new[] { 1f });

        Assert.AreEqual(0f, state.Get(-1));
        Assert.AreEqual(0f, state.Get(10));
    }

    [Test]
    public void Clamped_ShouldClampAllValues()
    {
        var state = new SystemState(new[] { -1f, 0.5f, 2f });

        var result = state.Clamped();

        Assert.AreEqual(0f, result.Get(0));
        Assert.AreEqual(0.5f, result.Get(1));
        Assert.AreEqual(1f, result.Get(2));
    }

    [Test]
    public void EqualObjects_ShouldHaveSameHashCode()
    {
        var a = new SystemState(new[] { 1f, 2f });
        var b = new SystemState(new[] { 1f, 2f });

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Test]
    public void Step_ShouldNotMutateSnapshot()
    {
        var initialValue = 0f;

        var state = new SystemState(new[] { initialValue });

        var integrator = new SemiImplicitIntegrator();

        var output = new ModuleOutput(1);
        output.Derivatives[0] = 1f;

        var stateStep = integrator.Step(state, output, 0.1f);

        Assert.AreEqual(initialValue, state.Get(0));
        Assert.AreNotEqual(state, stateStep);
    }
}
