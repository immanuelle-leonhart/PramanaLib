namespace PramanaLib;

/// <summary>
/// Interface that all Pramana-mapped objects implement, providing
/// access to the ontology roles (interfaces) the object participates in.
/// </summary>
public interface PramanaInterface
{
    /// <summary>
    /// Returns the <see cref="PramanaRole"/> instances that this object fulfils
    /// within the Pramana ontology.
    /// </summary>
    IEnumerable<PramanaRole> GetRoles();
}
