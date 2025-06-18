using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    public class InspectionListResultsContent : MonoBehaviour, IResultsContent
    {
        public void PopulateResultsContent(VisualElement root, ScoreDataSO firstInspectionScore, ScoreDataSO secondInspectionScore, ScoreMetricsSO scoreMetrics, bool isFirstPanel)
        {
            // Get label references
            Label inspectionPoints, stopwatchPoints, currentPoints;
            Label inspectionMetrics, stopwatchMetrics;
            inspectionPoints = root.Q<Label>("InspectionPoints");
            stopwatchPoints = root.Q<Label>("StopwatchPoints");
            currentPoints = root.Q<Label>("CurrentScorePoints");
            inspectionMetrics = root.Q<Label>("InspectionMetrics");
            stopwatchMetrics = root.Q<Label>("StopwatchMetrics");
            
            int inspectionPointsNum = 0, stopwatchPointsNum = 0, currentScoreNum = 0, remainingStopwatchNum = 0; 
            List<BrokenStateSO> brokenGroup;
            // Make score Calculations based of which set of inspections the current panel is using
            if (isFirstPanel)
            {
                // Add the first inspection inspections. 
                brokenGroup = firstInspectionScore.FirstInspections;
                brokenGroup.AddRange(firstInspectionScore.StartingLinkedInspections);
                // Calculate score
                inspectionPointsNum = firstInspectionScore.InspectionScore;
                stopwatchPointsNum = scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime);
                currentScoreNum = inspectionPointsNum + stopwatchPointsNum;
                // Stopwatch remaining time
                remainingStopwatchNum = firstInspectionScore.StopwatchTime;
            }
            else
            {
                // Add the second inspection inspections.
                brokenGroup = secondInspectionScore.SecondInspections;
                brokenGroup.AddRange(secondInspectionScore.EndingLinkedInspections);
                brokenGroup.AddRange(secondInspectionScore.MissedFirstInspections);
                brokenGroup.AddRange(secondInspectionScore.BadRoadChoiceInspections);
                // Calculate score
                int fisrtInspectionCalculation = firstInspectionScore.InspectionScore + scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime);
                inspectionPointsNum = secondInspectionScore.InspectionScore;
                stopwatchPointsNum = scoreMetrics.CalculateStopwatchScore(secondInspectionScore.StopwatchTime);
                currentScoreNum = inspectionPointsNum + stopwatchPointsNum + scoreMetrics.CalculateRoadChoiceScore(firstInspectionScore.RoadChoice) + fisrtInspectionCalculation;
                // Stopwatch remaining time
                remainingStopwatchNum = secondInspectionScore.StopwatchTime;
            }
            
            // Display points
            inspectionPoints.text = PointsFormat(inspectionPointsNum);
            stopwatchPoints.text = PointsFormat(stopwatchPointsNum);
            currentPoints.text = currentScoreNum.ToString() + " Points";
            
            int fixedInspectionCount = 0;
            foreach (BrokenStateSO brokenState in brokenGroup)
            {
                if (brokenState.IsFixed)
                {
                    fixedInspectionCount++;
                }
            }
            
            inspectionMetrics.text = $"{fixedInspectionCount} / {brokenGroup.Count}";
            stopwatchMetrics.text = $"{remainingStopwatchNum}";
            
            // Fill out list of inspections
            ProgressIndicator progressIndicator = GetComponent<ProgressIndicator>();
            progressIndicator.AddCategory("Inspections");
            
            int i = 0;
            foreach (BrokenStateSO brokenState in brokenGroup)
            {
                progressIndicator.AddTask(0, brokenState.ComponentName, scoreMetrics.InspectionPointIncrement.ToString());
                if (brokenState.IsFixed)
                {
                    progressIndicator.AddProgressToTask(0, i);
                }
                i++;
            }
        }

        private string PointsFormat(int points)
        {
            if (points > 0)
            {
                return $"+{points} Points";
            }
            else
            {
                return $"{points} Points";
            }
        }
    }
}
