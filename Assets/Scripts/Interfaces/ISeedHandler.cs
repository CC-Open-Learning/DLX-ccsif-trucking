using System.Collections.Generic;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Interface for a seed handler that can be referenced in the Inspection Handler
    /// </summary>
    public interface ISeedHandler
    {
        public List<Inspectable> SetInspectableStatus(List<Inspectable> allInspections);
    }
}
