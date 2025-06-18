using UnityEngine;

namespace VARLab.CCSIF
{
    public class MapRouteRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineCompletedPath;
        [SerializeField] private LineRenderer lineStartingActivePath;
        [SerializeField] private LineRenderer lineGoodRoadChoice;
        [SerializeField] private LineRenderer lineBadRoadChoice;

        [SerializeField] private Material activeRouteMaterial;
        [SerializeField] private Material optionalRouteMaterial;
        
        private LineRenderer activePath;

        private const float AdjustedCompletedPathHeight = 0.8f;
        
        private void OnEnable()
        {
            if (lineCompletedPath == null || lineStartingActivePath == null || lineGoodRoadChoice == null || lineBadRoadChoice == null)
            {
                Debug.LogWarning("MapRouteRenderer, missing reference to line renderer.");
            }
            if (activeRouteMaterial == null || optionalRouteMaterial == null)
            {
                Debug.LogWarning("MapRouteRenderer, missing reference to line renderer material.");
            }

            activePath = lineStartingActivePath;
        }

        /// <summary>
        /// Extends the completed route line and reduces the active route line.
        /// Called from timeline signals. 
        /// </summary>
        public void RemoveVertexFromActivePath()
        {
            // Get the next position for the completed path off the top of the active route line.
            Vector3 nextPosition = activePath.GetPosition(activePath.positionCount - 2);
            // Adjust height of completed path to avoid overlap.
            nextPosition.y = AdjustedCompletedPathHeight;
            
            activePath.positionCount--;
            lineCompletedPath.positionCount++;
            
            // Add the next position to the top of the completed path.
            lineCompletedPath.SetPosition(lineCompletedPath.positionCount-1, nextPosition);
        }

        public void HighlightHoveredRoadChoice(bool highlightGoodRoadChoice)
        {
            if(highlightGoodRoadChoice)
            {
                lineGoodRoadChoice.material = activeRouteMaterial;
                lineBadRoadChoice.material = optionalRouteMaterial;
                activePath = lineGoodRoadChoice;
            }
            else
            {
                lineGoodRoadChoice.material = optionalRouteMaterial;
                lineBadRoadChoice.material = activeRouteMaterial;
                activePath = lineBadRoadChoice;
            }
        }
    }
}
