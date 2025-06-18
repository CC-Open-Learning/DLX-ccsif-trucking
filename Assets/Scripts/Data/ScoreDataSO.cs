using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// ScoreDataSO is used for maintaining the different score attributes for module B
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreDataSO", menuName = "ScriptableObjects/ScoreDataSO")]
    public class ScoreDataSO : ScriptableObject
    {
        public int StopwatchTime;
        public RoadChoiceType RoadChoice;
        public int InspectionScore;

        public List<BrokenStateSO> FirstInspections;
        public List<BrokenStateSO> StartingLinkedInspections;
        public List<BrokenStateSO> SecondInspections;
        public List<BrokenStateSO> MissedFirstInspections;
        public List<BrokenStateSO> EndingLinkedInspections;
        public List<BrokenStateSO> BadRoadChoiceInspections;

        public void ResetData()
        {
            FirstInspections = new List<BrokenStateSO>();
            SecondInspections = new List<BrokenStateSO>();
            MissedFirstInspections = new List<BrokenStateSO>();
            EndingLinkedInspections = new List<BrokenStateSO>();
            BadRoadChoiceInspections = new List<BrokenStateSO>();
            StartingLinkedInspections = new List<BrokenStateSO>();
        }
    }
}
