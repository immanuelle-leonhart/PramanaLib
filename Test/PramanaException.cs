
[Serializable]
internal class PramanaException : Exception
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