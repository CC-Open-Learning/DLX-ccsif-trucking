using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    /// <summary>
    /// The interface for the different kinds of inspection results pages controlled by the Inspection Results Content
    /// </summary>
    public interface IResultsContent
    {
        public void PopulateResultsContent(VisualElement root, ScoreDataSO firstInspectionScore, ScoreDataSO secondInspectionScore, ScoreMetricsSO scoreMetrics, bool isFirstPanel);
    }
}
