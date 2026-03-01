namespace PramanaLib;

/// <summary>
/// Exception thrown when a Pramana OGM constraint is violated,
/// such as attempting to assign an ID to an object that already has one.
/// </summary>
[Serializable]
public class PramanaException : Exception
{
    public PramanaException()
    {
    }

    public PramanaException(string? message) : base(message)
    {
    }

    public PramanaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
