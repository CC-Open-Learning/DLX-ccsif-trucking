using System.Text.RegularExpressions;
using UnityEngine;
namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "BrokenStateSO", menuName = "ScriptableObjects/BrokenStateSO")]

    //BrokenStateSO is used for indentifying which BrokenStates are present in a module.
    //It is held by inspectable damaged states, and those states are enabled if they are present in the active module seed
    public class BrokenStateSO : ScriptableObject
    {
        public string ComponentName; //the name of the component, ie "window"
        public bool IsNamePlural; //if the name of the component is plural or not
        public bool IsFixed; // If the inspection has been fixed
        public InspectableType inspectableType; //the inspectable type of the component, ie "lawEnforcementIssue"
        public PointOfInterestType pointOfInterest;
        
        public string PointOfInterestName()  
        {  
            string result = Regex.Replace(pointOfInterest.ToString(), "([A-Z])", " $1").Trim();  
            return result;  
        }
    }
}