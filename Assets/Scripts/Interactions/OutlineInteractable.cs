using EPOOutline;
using UnityEngine;

namespace VARLab.CCSIF
{
    public class OutlineInteractable : MonoBehaviour
    {
        [Tooltip("The outline layer for all basic inspections and special views")]
        public int VisibleLayerOne = 1;
        [Tooltip("The outline layer for advanced inspections with multiple highlights")]
        public int VisibleLayerTwo = 2;
        [Tooltip("A layer not included in the Outliner layer mask. The Outline will be hidden when set to this layer.")]
        public int HiddenLayer = 0;

        private Color inspectionHighlight = new Color(1f, 0.9215f, 0.0157f, 0.6275f);
        private Color damageHighlight = new Color(1f, 0.3098f, 0.0157f, 0.6275f);
        private Color specialViewHighlight = new Color(1f, 1f, 1f, 1f);

        /// <summary>
        /// If the received <paramref name="obj"/> has an <see cref="Outlinable"/> component, the layer of 
        /// the Outline is changed so that the layer is visible
        /// </summary>
        /// <param name="obj">A relevant GameObject</param>
        public void ShowOutline(GameObject obj)
        {
            if (!obj) { return; }
            Outlinable outline;

            // Highlight special view with its child outline component
            if (obj.TryGetComponent<SpecializedView>(out _))
            {
                // Highlight a special view object
                outline = obj.GetComponentInChildren<Outlinable>();
                outline.OutlineParameters.Color = specialViewHighlight;
                outline.OutlineLayer = VisibleLayerOne;
                return;
            }

            outline = obj.GetComponent<Outlinable>();
            // highlight either a regular broken state inspection, a broken state inspection with specific damage coverage, or a fixed inspection
            if (obj.TryGetComponent<BrokenInspectable>(out BrokenInspectable inspectable))
            {
                if (WasInspectableInspected(inspectable)) { return; }
                
                outline.OutlineParameters.Color = inspectionHighlight;
                outline.OutlineLayer = VisibleLayerOne;
            }
            else if (obj.transform.parent.TryGetComponent<BrokenInspectable>(out BrokenInspectable damagedInspectable))
            {
                if (WasInspectableInspected(damagedInspectable)) { return; }

                outline.OutlineParameters.Color = damageHighlight;
                outline.OutlineLayer = VisibleLayerTwo;
            }
            else
            {
                outline.OutlineParameters.Color = inspectionHighlight;
                outline.OutlineLayer = VisibleLayerOne;
            }
        }

        /// <summary>
        /// If the received <paramref name="obj"/> has an <see cref="Outlinable"/> component, the layer of 
        /// the Outline is changed so that the layer is hidden.
        /// </summary>
        /// <param name="obj">A relevant GameObject</param>
        public void HideOutline(GameObject obj, bool wasClicked = false)
        {
            if (!obj) { return; }
            Outlinable outline;

            if (obj.TryGetComponent<SpecializedView>(out _))
            {
                outline = obj.GetComponentInChildren<Outlinable>();
                outline.OutlineLayer = HiddenLayer;
                return;
            }

            if (!wasClicked)
            {
                outline = obj.GetComponent<Outlinable>();
                outline.OutlineLayer = HiddenLayer;
                return;
            }

            if (obj.TryGetComponent<BrokenInspectable>(out BrokenInspectable inspectable))
            {
                if(!inspectable.HasSeparatedDamage)
                {
                    outline = inspectable.GetComponent<Outlinable>();
                    outline.OutlineLayer = HiddenLayer;
                }
            }
            else if (obj.transform.parent.TryGetComponent<BrokenInspectable>(out BrokenInspectable damagedInspectable))
            {
                outline = damagedInspectable.GetComponent<Outlinable>();
                outline.OutlineLayer = HiddenLayer;
                outline = obj.GetComponent<Outlinable>();
                outline.OutlineLayer = HiddenLayer;
            }
        }

        /// <summary>
        /// Helper method to determine if an inspectable has been clicked 
        /// to avoid highlighting an inspectable while it is being fixed
        /// </summary>
        /// <param name="brokenInspectable"> Inspectable to highlight </param>
        /// <returns> True if the inspectable was inspected </returns>
        private bool WasInspectableInspected(BrokenInspectable brokenInspectable)
        {
            if(brokenInspectable.transform.parent.TryGetComponent<Inspectable>(out Inspectable parentInspectable)) 
            {
                if (parentInspectable.WasInspected) 
                { 
                    return true; 
                }
            }
            return false;
        }
    }
}
