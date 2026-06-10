using CodependantModules.Simulation;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ImpulseTest
{
    [Test]
    public void Impulse_ModifiesState()
    {
        SystemState state = new SystemState(new float[] {0.5f });

        state = state.WithImpulse(0, 0.1f);

        float expectedValue = 0.6f;
        float value = state.Get(0);
        
        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [Test]
    public void Impulse_ShouldNotMutateOriginal()
    {
        var state = new SystemState(new[] { 0.5f });

        var modified = state.WithImpulse(0, 0.1f);

        Assert.AreEqual(0.5f, state.Get(0));
        Assert.AreEqual(0.6f, modified.Get(0));
    }

    [Test]
    public void Impulse_ShouldClampToOne()
    {
        var state = new SystemState(new[] { 0.9f });

        var modified = state.WithImpulse(0, 1f);

        Assert.AreEqual(1f, modified.Get(0));
    }

}
