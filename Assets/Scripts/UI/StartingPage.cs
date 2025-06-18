using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Class that is responsible for the functionality of the starting page of the sim.
    /// Teaches the user how to use controls of the sim
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class StartingPage : MonoBehaviour, IUserInterface
    {
        private VisualElement root;

        /// <summary>
        /// The root visual element of the starting page dialog.
        /// </summary>
        public VisualElement Root { get => root; }

        private VisualElement startingPageCanvas;
        private Button confirmButton;

        [Header("Events"), Space(4f)]
        [Tooltip("Invoked when the confirmation button is pressed.")]
        public UnityEvent OnDialogConfirmed;

        [Tooltip("Invoked when the confirmation dialog is opened.")]
        public UnityEvent OnDialogShown;

        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            startingPageCanvas = Root.Q("Canvas");
            confirmButton = Root.Q<Button>("Button");

            if (OnDialogConfirmed == null) { OnDialogConfirmed = new UnityEvent(); }

            confirmButton.RegisterCallback<ClickEvent>(ConfirmDialog);

            Root.style.alignItems = Align.Center;
            Root.style.justifyContent = Justify.Center;

            UIHelper.Show(startingPageCanvas);
        }

        /// <summary>
        /// Shows the starting page dialog
        /// </summary>
        public void Show()
        {
            UIHelper.Show(startingPageCanvas);
            OnDialogShown.Invoke();
        }

        /// <summary>
        /// hides the starting page dialog.
        /// </summary>
        public void Hide()
        {
            UIHelper.Hide(startingPageCanvas);
        }

        // Removes any existing buttons in the buttons holder element
        private void ClearButtons()
        {
            confirmButton?.UnregisterCallback<ClickEvent>(ConfirmDialog);
        }

        // Confirmation button event
        private void ConfirmDialog(ClickEvent evt)
        {
            Hide();
            OnDialogConfirmed.Invoke();
        }

        // Cleanup
        private void OnDestroy()
        {
            ClearButtons();
        }
    }
}
