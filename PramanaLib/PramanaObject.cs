namespace PramanaLib;

/// <summary>
/// Base class for all objects mapped into the Pramana knowledge graph.
/// Implements <see cref="IPramanaLinkable"/> for graph identity and
/// <see cref="PramanaInterface"/> for ontology role participation.
/// </summary>
/// <remarks>
/// <para><b>Friction by design:</b> IDs are never auto-generated. A new
/// <c>PramanaObject</c> starts with <see cref="Guid.Empty"/> and only receives
/// a real UUID v4 when <see cref="GenerateId"/> is explicitly called. This
/// prevents disposable or transient objects from polluting the graph with
/// throw-away identifiers. Once assigned, the ID is immutable — calling
/// <see cref="GenerateId"/> a second time throws <see cref="PramanaException"/>.
/// </para>
/// </remarks>
public class PramanaObject : IPramanaLinkable, PramanaInterface
{
    private Guid _pramanaGuid;

    /// <summary>
    /// The well-known root ID for the PramanaObject class itself in the ontology.
    /// </summary>
    public static readonly Guid RootId = Guid.Parse("10000000-0000-4000-8000-000000000001");

    /// <summary>Gets the class-level ID, which for <c>PramanaObject</c> is <see cref="RootId"/>.</summary>
    public static Guid ClassId => RootId;

    /// <summary>Gets the class-level URL in the Pramana graph.</summary>
    public static string ClassUrl => $"https://pramana.dev/entity/{ClassId}";

    /// <summary>
    /// Creates a new <see cref="PramanaObject"/>.
    /// </summary>
    /// <param name="id">
    /// Optional initial GUID. If <see langword="null"/> (default), the object starts
    /// with <see cref="Guid.Empty"/> and must be explicitly activated via
    /// <see cref="GenerateId"/>.
    /// </param>
    public PramanaObject(Guid? id = null)
    {
        _pramanaGuid = id ?? Guid.Empty;
    }

    /// <inheritdoc />
    public Guid PramanaGuid => _pramanaGuid;

    /// <summary>
    /// Gets the Pramana identifier string. Regular objects do not belong to a
    /// pseudo-class, so this returns <see langword="null"/>.
    /// </summary>
    public virtual string? PramanaId => null;

    /// <inheritdoc />
    public string PramanaHashUrl => $"https://pramana.dev/entity/{PramanaGuid}";

    /// <inheritdoc />
    public string PramanaUrl => PramanaHashUrl;

    /// <summary>
    /// Assigns a new UUID v4 to this object. Throws <see cref="PramanaException"/>
    /// if the object already has a non-empty ID — IDs are write-once by design.
    /// </summary>
    /// <exception cref="PramanaException">Thrown when the ID has already been assigned.</exception>
    public void GenerateId()
    {
        if (_pramanaGuid == Guid.Empty)
        {
            _pramanaGuid = Guid.NewGuid();
        }
        else
        {
            throw new PramanaException("Cannot reassign a PramanaObject ID once it has been set.");
        }
    }

    /// <inheritdoc />
    public virtual IEnumerable<PramanaRole> GetRoles() => Enumerable.Empty<PramanaRole>();
}
