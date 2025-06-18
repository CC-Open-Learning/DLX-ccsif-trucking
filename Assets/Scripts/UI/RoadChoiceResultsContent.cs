using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    public class RoadChoiceResultsContent : MonoBehaviour, IResultsContent
    {
        [SerializeField] private RoadChoiceResultsSO goodRoadChoiceContent;
        [SerializeField] private RoadChoiceResultsSO badRoadChoiceContent;
        
        public void PopulateResultsContent(VisualElement root, ScoreDataSO firstInspectionScore, ScoreDataSO secondInspectionScore, ScoreMetricsSO scoreMetrics, bool isFirstPanel)
        {
            Label roadChoiceTitle, currentScore, roadChoiceSubtitle, damagedPartsList;
            VisualElement roadChoiceImage = root.Q<VisualElement>("RoadImage");
            roadChoiceTitle = root.Q<Label>("RoadChoiceTitle");
            roadChoiceSubtitle = root.Q<Label>("RoadChoiceDescription");
            damagedPartsList = root.Q<Label>("DamagedPartsList");
            currentScore = root.Q<Label>("CurrentScore");
            
            string title = "", subtitle = "", damagedParts = "";
            Texture2D image;
            int currentScoreNum = 0;
            
            if (isFirstPanel)
            {
                if (firstInspectionScore.RoadChoice == RoadChoiceType.GoodRoadChoice)
                {
                    title = goodRoadChoiceContent.Title;
                    subtitle = goodRoadChoiceContent.Subtitle;
                    image = goodRoadChoiceContent.RoadChoiceIcon as Texture2D;
                    damagedParts = goodRoadChoiceContent.DamagedPartsList;
                }
                else
                {
                    title = badRoadChoiceContent.Title;
                    subtitle = badRoadChoiceContent.Subtitle;
                    image = badRoadChoiceContent.RoadChoiceIcon as Texture2D;
                    foreach (BrokenStateSO brokenState in secondInspectionScore.BadRoadChoiceInspections)
                    {
                        damagedParts += $" - {brokenState.name} ({brokenState.PointOfInterestName()})\n";
                    }
                }

                currentScoreNum = firstInspectionScore.InspectionScore + scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime) + scoreMetrics.CalculateRoadChoiceScore(firstInspectionScore.RoadChoice);
            }
            else
            {
                if (secondInspectionScore.RoadChoice == RoadChoiceType.GoodRoadChoice)
                {
                    title = goodRoadChoiceContent.Title;
                    subtitle = goodRoadChoiceContent.Subtitle;
                    image = goodRoadChoiceContent.RoadChoiceIcon as Texture2D;
                    damagedParts = goodRoadChoiceContent.DamagedPartsList;
                }
                else
                {
                    title = badRoadChoiceContent.Title;
                    subtitle = badRoadChoiceContent.Subtitle;
                    image = badRoadChoiceContent.RoadChoiceIcon as Texture2D;
                    damagedParts = badRoadChoiceContent.DamagedPartsList;
                }

                currentScoreNum = firstInspectionScore.InspectionScore + scoreMetrics.CalculateStopwatchScore(firstInspectionScore.StopwatchTime) + scoreMetrics.CalculateRoadChoiceScore(firstInspectionScore.RoadChoice);
                currentScoreNum += secondInspectionScore.InspectionScore + scoreMetrics.CalculateStopwatchScore(secondInspectionScore.StopwatchTime) + scoreMetrics.CalculateRoadChoiceScore(secondInspectionScore.RoadChoice);
            }
            
            roadChoiceTitle.text = title;
            roadChoiceSubtitle.text = subtitle;
            roadChoiceImage.style.backgroundImage = new StyleBackground(image);
            
            damagedPartsList.text = damagedParts;
            currentScore.text = "Current Score: " + currentScoreNum;
        }
    }
}
