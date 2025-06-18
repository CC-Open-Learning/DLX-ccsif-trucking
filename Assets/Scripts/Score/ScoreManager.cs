using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Updates the persistent score data used in module B
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ScoreDataSO scoreData;
        [SerializeField] private ScoreMetricsSO scoreMetrics;
        
        public UnityEvent OnRoadChoiceUpdated;

        private void Start()
        {
            OnRoadChoiceUpdated ??= new UnityEvent();
        }

        public void UpdateStopwatchTime(int time)
        {
            scoreData.StopwatchTime = time;
        }
        
        public void UpdateStartingInspections(PointOfInterest[] pointOfInterests)
        {
            List<BrokenStateSO> brokenStates = new List<BrokenStateSO>();
            int inspectionScore = 0;
            foreach (PointOfInterest pointOfInterest in pointOfInterests)
            {
                foreach (Inspectable inspectable in pointOfInterest.GetInspectables())
                {
                    if (inspectable.IsStartingBroken)
                    {
                        brokenStates.Add(inspectable.ActiveBrokenState.GetComponent<BrokenInspectable>().BrokenStateSO);
                        if (inspectable.WasInspected)
                        {
                            inspectionScore += scoreMetrics.InspectionPointIncrement;
                        }
                        else
                        {
                            inspectionScore -= scoreMetrics.InspectionPointIncrement;
                        }
                    }
                }
            }
            scoreData.InspectionScore = inspectionScore;
        }

        /// <summary>
        /// Updates the score data with the users road choice
        /// </summary>
        /// <param name="isRoadChoiceGood"> road choice decision </param>
        public void UpdateRoadChoice(bool isRoadChoiceGood)
        {
            if (isRoadChoiceGood)
            {
                scoreData.RoadChoice = RoadChoiceType.GoodRoadChoice;
            }
            else
            {
                scoreData.RoadChoice = RoadChoiceType.BadRoadChoice;
            }
            
            OnRoadChoiceUpdated?.Invoke();
        }
    }
}
