using CodependantModules.Simulation;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ModuleOutputTest
{
    [Test]
    public void Merge_ShouldSumOutputs()
    {
        var a = new ModuleOutput(2);
        var b = new ModuleOutput(2);

        a.Derivatives[0] = 1f;
        b.Derivatives[0] = 2f;

        a.Merge(b);

        Assert.AreEqual(3f, a.Derivatives[0]);
    }

    [Test]
    public void Merge_WithNull_ShouldDoNothing()
    {
        var output = new ModuleOutput(1);

        output.Merge(null);

        Assert.Pass();
    }

    [Test]
    public void Clear_ShouldResetArrays()
    {
        var output = new ModuleOutput(1);

        output.Derivatives[0] = 5f;
        output.LinearCoeffs[0] = 7f;

        output.Clear();

        Assert.AreEqual(0f, output.Derivatives[0]);
        Assert.AreEqual(0f, output.LinearCoeffs[0]);
    }

}
