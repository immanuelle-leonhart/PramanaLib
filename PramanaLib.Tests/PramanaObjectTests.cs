namespace PramanaLib.Tests;

public class PramanaObjectTests
{
    [Fact]
    public void DefaultConstructor_HasEmptyGuid()
    {
        var obj = new PramanaObject();
        Assert.Equal(Guid.Empty, obj.PramanaGuid);
    }

    [Fact]
    public void Constructor_WithId_SetsGuid()
    {
        var id = Guid.NewGuid();
        var obj = new PramanaObject(id);
        Assert.Equal(id, obj.PramanaGuid);
    }

    [Fact]
    public void GenerateId_AssignsNonEmptyGuid()
    {
        var obj = new PramanaObject();
        obj.GenerateId();
        Assert.NotEqual(Guid.Empty, obj.PramanaGuid);
    }

    [Fact]
    public void GenerateId_ThrowsOnSecondCall()
    {
        var obj = new PramanaObject();
        obj.GenerateId();
        Assert.Throws<PramanaException>(() => obj.GenerateId());
    }

    [Fact]
    public void GenerateId_ThrowsWhenConstructedWithId()
    {
        var obj = new PramanaObject(Guid.NewGuid());
        Assert.Throws<PramanaException>(() => obj.GenerateId());
    }

    [Fact]
    public void PramanaId_IsNull_ForRegularObject()
    {
        var obj = new PramanaObject();
        Assert.Null(obj.PramanaId);
    }

    [Fact]
    public void PramanaHashUrl_ContainsGuid()
    {
        var id = Guid.NewGuid();
        var obj = new PramanaObject(id);
        Assert.Equal($"https://pramana.dev/entity/{id}", obj.PramanaHashUrl);
    }

    [Fact]
    public void PramanaUrl_EqualsHashUrl_ForRegularObject()
    {
        var obj = new PramanaObject(Guid.NewGuid());
        Assert.Equal(obj.PramanaHashUrl, obj.PramanaUrl);
    }

    [Fact]
    public void ClassId_EqualsRootId()
    {
        Assert.Equal(PramanaObject.RootId, PramanaObject.ClassId);
    }

    [Fact]
    public void RootId_HasExpectedValue()
    {
        Assert.Equal(Guid.Parse("10000000-0000-4000-8000-000000000001"), PramanaObject.RootId);
    }

    [Fact]
    public void ClassUrl_UsesClassId()
    {
        Assert.Equal($"https://pramana.dev/entity/{PramanaObject.ClassId}", PramanaObject.ClassUrl);
    }

    [Fact]
    public void GetRoles_ReturnsEmpty_ByDefault()
    {
        var obj = new PramanaObject();
        Assert.Empty(obj.GetRoles());
    }

    [Fact]
    public void ImplementsIPramanaLinkable()
    {
        var obj = new PramanaObject();
        Assert.IsAssignableFrom<IPramanaLinkable>(obj);
    }

    [Fact]
    public void ImplementsPramanaInterface()
    {
        var obj = new PramanaObject();
        Assert.IsAssignableFrom<PramanaInterface>(obj);
    }
}
