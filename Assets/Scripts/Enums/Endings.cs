namespace VARLab.CCSIF
{
    /// <summary>
    /// An enum to define the types of possible endings. 
    /// </summary>
    public enum Endings
    {
        MechanicalFailureEnding = 0, // if the truck has a mechanical failure and breaks down
        LawEnforcementEnding = 1, // if the truck has a law enforcement issue and is pulled over by the police
        HappyEnding = 2 // if the truck has no issues, they get the good ending
    }
}
