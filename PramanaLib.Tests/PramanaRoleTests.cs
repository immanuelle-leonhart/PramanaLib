namespace PramanaLib.Tests;

public class PramanaRoleTests
{
    [Fact]
    public void Constructor_SetsLabel()
    {
        var role = new PramanaRole("Entity");
        Assert.Equal("Entity", role.Label);
    }

    [Fact]
    public void Constructor_WithId_SetsGuid()
    {
        var id = Guid.NewGuid();
        var role = new PramanaRole("Entity", id);
        Assert.Equal(id, role.PramanaGuid);
    }

    [Fact]
    public void Constructor_WithoutId_HasEmptyGuid()
    {
        var role = new PramanaRole("Entity");
        Assert.Equal(Guid.Empty, role.PramanaGuid);
    }

    [Fact]
    public void IsPramanaObject()
    {
        var role = new PramanaRole("Entity");
        Assert.IsAssignableFrom<PramanaObject>(role);
    }

    [Fact]
    public void GetRoles_ReturnsSelf()
    {
        var role = new PramanaRole("Entity");
        var roles = role.GetRoles().ToList();
        Assert.Single(roles);
        Assert.Same(role, roles[0]);
    }

    [Fact]
    public void ParentRoles_InitiallyEmpty()
    {
        var role = new PramanaRole("Entity");
        Assert.Empty(role.ParentRoles);
    }

    [Fact]
    public void ChildRoles_InitiallyEmpty()
    {
        var role = new PramanaRole("Entity");
        Assert.Empty(role.ChildRoles);
    }

    [Fact]
    public void InstanceOf_DefaultsToNull()
    {
        var role = new PramanaRole("Entity");
        Assert.Null(role.InstanceOf);
    }

    [Fact]
    public void SubclassOf_DefaultsToNull()
    {
        var role = new PramanaRole("Entity");
        Assert.Null(role.SubclassOf);
    }

    [Fact]
    public void CanBuildRoleHierarchy()
    {
        var parent = new PramanaRole("Thing");
        var child = new PramanaRole("Person");
        child.SubclassOf = parent;
        parent.ChildRoles.Add(child);
        child.ParentRoles.Add(parent);

        Assert.Same(parent, child.SubclassOf);
        Assert.Contains(child, parent.ChildRoles);
        Assert.Contains(parent, child.ParentRoles);
    }

    [Fact]
    public void InstanceOf_CanBeSet()
    {
        var classRole = new PramanaRole("Class");
        var instance = new PramanaRole("MyClass");
        instance.InstanceOf = classRole;

        Assert.Same(classRole, instance.InstanceOf);
    }

    [Fact]
    public void GenerateId_WorksOnRole()
    {
        var role = new PramanaRole("Entity");
        role.GenerateId();
        Assert.NotEqual(Guid.Empty, role.PramanaGuid);
    }
}
