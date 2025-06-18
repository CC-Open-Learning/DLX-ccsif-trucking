using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class FinalResultsContent : MonoBehaviour, IResultsContent
    {
        private ScoreDataSO scoreData;
        private ScoreMetricsSO finalScoreMetrics;
        private int overallScore;
        private Button playAgainButton;

        [SerializeField] private StorySeedSO moduleSeed;
        [SerializeField] private FinalResultsSO successEnding, lawEnforcementEnding, mechanicalFailureEnding;

        public UnityEvent OnLawEnforcementEnding;
        public UnityEvent OnMechanicalFailureEnding;
        public UnityEvent OnSuccessEnding;
        public UnityEvent<List<string>> OnPlayAgain;
        
        private const string BrokenMessage = " Broken", FixedMessage = " Fixed";

        private Endings endingType;
        private Label tripOutcome, finalScore, endingDescription;

        private void Start()
        {
            OnLawEnforcementEnding ??= new UnityEvent();
            OnMechanicalFailureEnding ??= new UnityEvent();
            OnSuccessEnding ??= new UnityEvent();
            OnPlayAgain ??= new UnityEvent<List<string>>();
        }
        
        public void PopulateResultsContent(VisualElement root, ScoreDataSO firstInspectionScore, ScoreDataSO secondInspectionScore, ScoreMetricsSO scoreMetrics, bool isFirstPanel)
        {
            scoreData = secondInspectionScore;
            finalScoreMetrics = scoreMetrics;
            
            tripOutcome = root.Q<Label>("TripOutcome");
            finalScore = root.Q<Label>("FinalScore");
            endingDescription = root.Q<Label>("EndingDescription");
            
            playAgainButton = root.Q<Button>("Next");
            playAgainButton.clicked += UpdateAnalytics;
            
            overallScore = 0;
            // Add inspection score
            overallScore += firstInspectionScore.InspectionScore;
            overallScore += secondInspectionScore.InspectionScore;
            // Add stopwatch score
            overallScore += scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime);
            overallScore += scoreMetrics.CalculateStopwatchScore(secondInspectionScore.StopwatchTime);
            // Add road choice score
            overallScore += scoreMetrics.CalculateRoadChoiceScore(firstInspectionScore.RoadChoice);
            overallScore += scoreMetrics.CalculateRoadChoiceScore(secondInspectionScore.RoadChoice);
            
            finalScore.text = "Final Score: " + overallScore;
            endingDescription.text = $"First Inspection: {firstInspectionScore.InspectionScore}\t\t\tSecond Inspection: {secondInspectionScore.InspectionScore}\n";
            endingDescription.text += $"First Stopwatch Score: {scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime)}\tSecond Stopwatch Score: {scoreMetrics.CalculateStopwatchScore(secondInspectionScore.StopwatchTime)}\n";
            endingDescription.text += $"First Road Choice Score: {scoreMetrics.CalculateRoadChoiceScore(firstInspectionScore.RoadChoice)}\tSecond Road Choice Score: {scoreMetrics.CalculateRoadChoiceScore(secondInspectionScore.RoadChoice)}\n";
            endingDescription.text += $"Final Score = {overallScore}";
        }

        private void UpdateTextResults()
        {
            FinalResultsSO finalResults = successEnding;
            if (endingType == Endings.MechanicalFailureEnding)
            {
                finalResults = mechanicalFailureEnding;
            }
            else if (endingType == Endings.LawEnforcementEnding)
            {
                finalResults = lawEnforcementEnding;
            }
            else if (endingType == Endings.HappyEnding)
            {
                finalResults = successEnding;
            }

            tripOutcome.text = finalResults.TripOutcome;
        }

        public void FindEndingType()
        {
            int mechanicalFailures = 0, lawEnforcementIssues = 0;
            List<BrokenStateSO> brokenStates = new List<BrokenStateSO>();
            
            brokenStates.AddRange(scoreData.StartingLinkedInspections);
            brokenStates.AddRange(scoreData.SecondInspections);
            brokenStates.AddRange(scoreData.MissedFirstInspections);
            brokenStates.AddRange(scoreData.EndingLinkedInspections);
            brokenStates.AddRange(scoreData.BadRoadChoiceInspections);
            
            foreach (BrokenStateSO brokenState in brokenStates)
            {
                if (brokenState.IsFixed == false)
                {
                    if (brokenState.inspectableType == InspectableType.MechanicalFailure)
                    {
                        mechanicalFailures++;
                    } 
                    else if (brokenState.inspectableType == InspectableType.LawEnforcementIssue)
                    {
                        lawEnforcementIssues++;
                    }
                }
            }

            if (overallScore >= finalScoreMetrics.SuccessEndingMinimum)
            {
                endingType = Endings.HappyEnding;
                OnSuccessEnding?.Invoke();
            }
            else if (mechanicalFailures >= lawEnforcementIssues)
            {
                endingType = Endings.MechanicalFailureEnding;
                OnMechanicalFailureEnding?.Invoke();
            }
            else
            {
                endingType = Endings.LawEnforcementEnding;
                OnLawEnforcementEnding?.Invoke();
            }

            UpdateTextResults();
        }

        private void UpdateAnalytics()
        {
            List<string> completionData = new List<string>();
            completionData.Add(moduleSeed.PlayThroughCount.ToString());

            // For each inspection add either broken or fixed message.
            List<BrokenStateSO> brokenStates = new List<BrokenStateSO>();
            brokenStates.AddRange(scoreData.StartingLinkedInspections);
            brokenStates.AddRange(scoreData.SecondInspections);
            brokenStates.AddRange(scoreData.MissedFirstInspections);
            brokenStates.AddRange(scoreData.EndingLinkedInspections);
            brokenStates.AddRange(scoreData.BadRoadChoiceInspections);

            foreach (BrokenStateSO brokenState in brokenStates)
            {
                if (brokenState.IsFixed == false)
                {
                    completionData.Add(brokenState.ComponentName + BrokenMessage);
                }
                else
                {
                    completionData.Add(brokenState.ComponentName + FixedMessage);
                }
            }

            moduleSeed.ResetPlaythrough = false;
            OnPlayAgain?.Invoke(completionData);
        }
    }
}
