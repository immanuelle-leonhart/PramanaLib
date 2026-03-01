namespace PramanaLib;

/// <summary>
/// Represents a role (interface) in the Pramana ontology.
/// Roles form a hierarchy via <see cref="SubclassOf"/> and <see cref="InstanceOf"/>,
/// and track their position in the role graph through
/// <see cref="ParentRoles"/> and <see cref="ChildRoles"/>.
/// </summary>
public class PramanaRole : PramanaObject
{
    /// <summary>
    /// Creates a new <see cref="PramanaRole"/> with the given label.
    /// </summary>
    /// <param name="label">Human-readable name for this role.</param>
    /// <param name="id">Optional initial GUID.</param>
    public PramanaRole(string label, Guid? id = null) : base(id)
    {
        Label = label;
    }

    /// <summary>Gets or sets the human-readable label for this role.</summary>
    public string Label { get; set; }

    /// <summary>Gets or sets the role that this role is an instance of.</summary>
    public PramanaRole? InstanceOf { get; set; }

    /// <summary>Gets or sets the role that this role is a subclass of.</summary>
    public PramanaRole? SubclassOf { get; set; }

    /// <summary>Gets the parent roles of this role in the hierarchy.</summary>
    public List<PramanaRole> ParentRoles { get; } = new();

    /// <summary>Gets the child roles of this role in the hierarchy.</summary>
    public List<PramanaRole> ChildRoles { get; } = new();

    /// <inheritdoc />
    public override IEnumerable<PramanaRole> GetRoles()
    {
        yield return this;
    }
}
