using System;

public interface IPramanaLinkable
{
    Guid PramanaID { get; set; }
    string PramanaHashUrl { get; set; }
}