using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIDocument> persistentUI = new List<UIDocument>();
        [SerializeField] private List<UIDocument> disruptiveUI = new List<UIDocument>();
        [SerializeField] private UIDocument mainMenu;

        // In scene UI element references
        [SerializeField] private ConfirmationDialog inspectionResultsDialog;
        [SerializeField] private ConfirmationDialogSO dialogContent;

        public UnityEvent OnButtonClicked;

        private List<UIDocument> allUIElements = new List<UIDocument>();

        private TooltipObject[] tooltipObjects;


        // Tracks if cursor is hovered over a UI Element
        private bool overUIComponent;
        public bool OverUIComponent { get { return overUIComponent; } set { overUIComponent = value; } }

        // Tracks if a disruptiveUIElement (i.e. settings menu or inspection results) is open
        private bool inUserInterface;
        public bool InUserInterface { get { return inUserInterface; } set { inUserInterface = value; } }

        // Prevents any disruptiveUIElement from being opened
        private bool blockMenuOpen;
        public bool BlockMenuOpen { get => blockMenuOpen; set => blockMenuOpen = value; }

        // Tracks if user is inside of the main menu
        private bool inMainMenu = true;
        public bool InMainMenu { get { return inMainMenu; } set { inMainMenu = value; } }

        private void Awake()
        {
            OnButtonClicked ??= new UnityEvent();


            tooltipObjects = (TooltipObject[])FindObjectsOfType(typeof(TooltipObject));

            allUIElements.AddRange(disruptiveUI);
            allUIElements.AddRange(persistentUI);

            // Register the mouse hover and exit call backs
            foreach (UIDocument UIElement in  allUIElements) 
            {
                RegisterMouseHoverCallBack(UIElement);
            }

            // Show the scenes persistent UI elements on start (toolbar, camera controls)
            HideAllUI();
        }
        
        /// <summary>
        /// Shows the confirmation dialog for the inspection results
        /// </summary>
        /// Invoked from <see cref="Toolbar.OnOptionFourSelected"/>
        public void ShowInspectionResultConfirmation()
        {
            if (BlockMenuOpen) { return; }

            HideAllUI();
            inspectionResultsDialog.HandleDisplayUI(dialogContent);
            InUserInterface = true;
        }

        /// Invoked from <see cref="Toolbar.OnOptionFiveSelected"/>
        /// Invoked from <see cref="ConfirmationDialog.OnDialogConfirmed"/>
        public void ShowUIDocument(UIDocument uiDocument)
        {
            if (disruptiveUI.Contains(uiDocument))
            {
                if (blockMenuOpen) { return; }
                HideAllUI();
                InUserInterface = true;
            }
            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }


        /// Invoked from <see cref="SettingsMenu.OnSettingsMenuHidden"/>
        /// Invoked from <see cref="InspectionResults.OnPlayAgain"/>
        /// Invoked from <see cref="StartingPage.OnDialogConfirmed"/>
        public void HideUIDocument(UIDocument uiDocument)
        {
            if (disruptiveUI.Contains(uiDocument))
            {
                InUserInterface = false;
                ShowPersistentUI();
            }

            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Hides the settings menu for both inside of the main menu and during the inspection game. 
        /// </summary>
        /// Invoked from <see cref="SettingsMenu.OnSettingsMenuHidden"/>
        public void HideSettingsMenu(UIDocument settingsMenu)
        {
            if (!InMainMenu)
            {
                InUserInterface = false;
                ShowPersistentUI(); 
            }
            else
            {
                HideUIDocument(settingsMenu);
                ShowUIDocument(mainMenu);
            }
        }

        /// <summary>
        /// Hides the confirmation dialog for inspection results and returns player to inspection state.
        /// </summary>
        /// Invoked from <see cref="ConfirmationDialog.OnDialogClosed"/>
        /// Invoked from <see cref="ConfirmationDialog.OnDialogCancelled"/>
        public void ShowInspectionResultCancel()
        {
            HideAllUI();
            ShowPersistentUI();
            InUserInterface = false;
        }

        /// <summary>
        /// Helper method to hide all UI elements, also used on play again
        /// </summary>
        private void HideAllUI()
        {
            InUserInterface = false;
            foreach (UIDocument element in allUIElements)
            {
                element.rootVisualElement.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// Helper method to show the always on screen game player UI (Toolbar, Camera Controls)
        /// </summary>
        private void ShowPersistentUI()
        {
            foreach (UIDocument element in persistentUI)
            {
                element.rootVisualElement.style.display = DisplayStyle.Flex;
            }
        }


        //hide every tooltip object
        public void HideTooltipObjects()
        {
            foreach (TooltipObject tooltip in tooltipObjects)
                tooltip.IsTooltipEnabled = false;
        }

        //show every tooltip object
        public void ShowTooltipObjects()
        {
            foreach (TooltipObject tooltip in tooltipObjects)
                tooltip.IsTooltipEnabled = true;
        }

        /// <summary>
        /// Registers mouse enter and exit call backs for a UI Component that will update the OverUIComponent variable.
        /// </summary>
        /// <param name="UIComponent"> UI Component to register callbacks </param>
        private void RegisterMouseHoverCallBack(UIDocument UIComponent)
        {
            UIComponent.rootVisualElement.RegisterCallback<MouseEnterEvent>(e => { OverUIComponent = true; HideTooltipObjects(); });
            UIComponent.rootVisualElement.RegisterCallback<MouseLeaveEvent>(e => { OverUIComponent = false; ShowTooltipObjects(); });
            List<Button> buttons = UIComponent.rootVisualElement.Query<Button>().ToList();
            foreach (Button button in buttons) { button.clicked += () => { OnButtonClicked?.Invoke(); }; }
        }

        /// <summary>
        /// This function is needed for dynamically UI, as they don't exist upon initalization
        /// Used by Conversation and ConfirmationDialgoue
        /// </summary>
        /// <param name="UIComponent"> UI Component to register callbacks </param>
        public void RegisterButton(UIDocument UIComponent)
        {
            RegisterMouseHoverCallBack(UIComponent);
        }
    }
}