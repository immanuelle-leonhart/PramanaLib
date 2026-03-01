internal class PramanaObject : IPramanaLinkable
{
    private Guid pramanaID;

    public PramanaObject() //This is the base constructor, it intentionally does not assign a pramana ID, because we want to add some friction to adding one
    {
        // This sets it to 00000000-0000-0000-0000-000000000000
        this.pramanaID = Guid.Empty;
    }

    public Guid PramanaID
    {
        get { return pramanaID; }
    }

    /// <summary>
    /// Gets the Pramana entity URL using the hashed UUID v5 identifier, formed as
    /// <c>https://pramana.dev/entity/{PramanaId}</c>.
    /// </summary>
    public string PramanaHashUrl => $"https://pramana.dev/entity/{PramanaID}";

    public string PramanaUrl => this.PramanaHashUrl;

    public void generateID()
    {
        if (pramanaID == Guid.Empty) { 
            this.pramanaID = Guid.NewGuid(); //apply an ID if you actually are planning on putting it in the Pramana Network. It is bad data hygiene to give guids to things that are disposable.
        }
        else
        {
            throw new PramanaException();

        }
    }

    public static Guid ClassID => PramanaObject.RootID;

    public static string ClassUrl => $"https://pramana.dev/entity/{ClassID}";
    public static Guid RootID = Guid.Parse("10000000-0000-4000-8000-000000000001");
}