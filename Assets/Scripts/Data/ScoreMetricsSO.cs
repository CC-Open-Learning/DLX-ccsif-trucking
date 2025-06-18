using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "ScoreMetricsSO", menuName = "ScriptableObjects/ScoreMetricsSO")]
    public class ScoreMetricsSO : ScriptableObject
    {
        public int InspectionPointIncrement;
        public int StartingStopwatchTime;
        public int StopwatchRewardMultiplier;
        public int StopwatchPenaltyIncrement;
        public int StopwatchPenaltyIncrementSize;
        public int RoadChoicePointIncrement;
        public int SuccessEndingMinimum;
        
        public int CalculateStopwatchScore(int stopwatchTime)
        {
            int score = 0;
            if (stopwatchTime >= 0)
            {
                score += (stopwatchTime * StopwatchRewardMultiplier);
            }
            else
            {
                // Calculate the overtime iterations the player went into
                int overtimeCount = (-stopwatchTime) / StopwatchPenaltyIncrementSize;
                if (-stopwatchTime % StopwatchPenaltyIncrementSize != 0)
                {
                    overtimeCount++;
                }
                score -= overtimeCount * StopwatchPenaltyIncrement;
            }
            return score;
        }

        public int CalculateRoadChoiceScore(RoadChoiceType roadChoice)
        {
            if (roadChoice == RoadChoiceType.BadRoadChoice)
            {
                return -RoadChoicePointIncrement;
            } 
            else if (roadChoice == RoadChoiceType.GoodRoadChoice)
            {
                return RoadChoicePointIncrement;
            }
            return 0;
        }
    }
}
