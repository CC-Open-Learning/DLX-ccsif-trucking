using UnityEngine;

namespace VARLab.CCSIF
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MovementWaypoint : MonoBehaviour
    {
        [SerializeField] private Color defaultColor = new(1f, 1f, 0f, 0.4f);
        [SerializeField] private Color highlightedColor = new(1f, 1f, 0f, 1f);

        [SerializeField] public int waypointIndex;
        
        [SerializeField] public PointOfInterest pointOfInterest;
        
        private SpriteRenderer spriteRenderer;
        private bool disableHighlights;
        
        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UnhighlightWaypoint();
        }

        public void HighlightWaypoint()
        {
            if (disableHighlights) { return; }
            spriteRenderer.color = highlightedColor;
        }

        public void UnhighlightWaypoint()
        {
            spriteRenderer.color = defaultColor;
        }

        public void EnableHighlights(bool toEnable)
        {
            disableHighlights = toEnable;
        }
    }
}
