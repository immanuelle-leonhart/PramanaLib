namespace PramanaLib;

/// <summary>
/// Interface for objects that can be linked to entities in the Pramana knowledge graph.
/// Provides identity and URL properties for graph integration.
/// </summary>
public interface IPramanaLinkable
{
    /// <summary>Gets the UUID (v4 or v5) identifying this entity in the Pramana graph.</summary>
    Guid PramanaGuid { get; }

    /// <summary>
    /// Gets the Pramana identifier string (e.g. <c>pra:num:3,1,2,1</c>).
    /// May be <see langword="null"/> for objects that are not pseudo-class instances.
    /// </summary>
    string? PramanaId { get; }

    /// <summary>
    /// Gets the Pramana entity URL using the hashed UUID,
    /// e.g. <c>https://pramana.dev/entity/{PramanaGuid}</c>.
    /// </summary>
    string PramanaHashUrl { get; }

    /// <summary>
    /// Gets the Pramana entity URL. For pseudo-class instances this uses the
    /// <see cref="PramanaId"/> string; otherwise it falls back to <see cref="PramanaHashUrl"/>.
    /// </summary>
    string PramanaUrl { get; }
}
