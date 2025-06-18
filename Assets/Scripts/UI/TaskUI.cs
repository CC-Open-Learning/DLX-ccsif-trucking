using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Data type that contains UI elements that a progress indicator task is bound to.
    /// </summary>
    public class TaskUI
    {
        public Label TaskNameLabel;
        public VisualElement CheckmarkIcon;
        public VisualElement CrossIcon;
        public Label PointIterationLabel;
    }
}
