namespace PramanaLib;

/// <summary>
/// A minimal subclass of <see cref="PramanaObject"/> used for testing
/// the Pramana OGM class hierarchy.
/// </summary>
public class PramanaParticular : PramanaObject
{
    /// <summary>
    /// The well-known class ID for PramanaParticular in the ontology.
    /// </summary>
    public static new readonly Guid ClassId = Guid.Parse("13000000-0000-4000-8000-000000000004");

    /// <summary>Gets the class-level URL in the Pramana graph.</summary>
    public static new string ClassUrl => $"https://pramana.dev/entity/{ClassId}";

    /// <summary>
    /// Creates a new <see cref="PramanaParticular"/>.
    /// </summary>
    /// <param name="id">Optional initial GUID.</param>
    public PramanaParticular(Guid? id = null) : base(id)
    {
    }
}
