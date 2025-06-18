namespace VARLab.CCSIF
{
    /// <summary>
    /// An enum to define the failure type of inspection 
    /// </summary>
    public enum InspectableType
    {
        MechanicalFailure = 0, // if the inspection contributes to a mechanical failure, ie the fuel tank is leaking
        LawEnforcementIssue = 1 // if the inspection contributes to a law enforcement issue, ie windshield cracked, signal light out
    }
}
