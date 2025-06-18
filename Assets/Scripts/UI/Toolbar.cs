using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    public class Toolbar : MonoBehaviour
    {
        /* There are no open or closed toolbar events, class names, or some buttons from V1 since V2 mostly just reduces functionality and changes design */
        private const string SelectedClass = "itemgroup-v2-selected";
        private const string DisabledClass = "toolbar-disabled";
        private const string ButtonClass = "toolbar-button";

        private Button lastBtn = null;

        [Header("Unity Events for Toolbar Actions")]
        [Tooltip("Add your own UnityEvent here for when a window is opened.")]
        public UnityEvent OnWindowOpened;
        [Tooltip("Add your own UnityEvent here for when an opened window is closed.")]
        public UnityEvent OnWindowClosed;

        public UnityEvent OnOptionOneSelected;
        public UnityEvent OnOptionTwoSelected;
        public UnityEvent OnOptionThreeSelected;

        /// Invokes <see cref="AnimationManager.PlayAnimation"/>
        public UnityEvent OnOptionFourSelected;
        public UnityEvent OnOptionFiveSelected;

        public UIDocument Document;
        public VisualElement Root;
        private VisualElement toolbar;

        [Header("Toolbar Button Icons")]
        [Tooltip("Add your own sprite to be shown on the first button.")]
        [SerializeField] private Sprite Icon1;
        [Tooltip("Add your own sprite to be shown on the second button.")]
        [SerializeField] private Sprite Icon2;
        [Tooltip("Add your own sprite to be shown on the third button.")]
        [SerializeField] private Sprite Icon3;
        [Tooltip("Add your own sprite to be shown on the fourth button.")]
        [SerializeField] private Sprite Icon4;
        [Tooltip("Add your own sprite to be shown on the fifth button")]
        [SerializeField] private Sprite Icon5;

        private List<Button> toolbarBtns;
        private List<UnityEvent> toolbarBtnEvents;
        private List<ButtonToolTip> btnToolTips;

        private bool isToolbarDisabled = false;
        public bool IsToolbarDisabled 
        { 
            get { return isToolbarDisabled; } 

            set 
            { 
                isToolbarDisabled = value;
                foreach (Button btn in toolbarBtns)
                {
                    if (isToolbarDisabled)
                    {
                        btn.EnableInClassList(DisabledClass, true);
                        btn.EnableInClassList(ButtonClass, false);
                    }
                    else
                    {
                        btn.EnableInClassList(DisabledClass, false);
                        btn.EnableInClassList(ButtonClass, true);
                    }
                }
            } 
        }

        // For CCSIF tooltips
        [SerializeField] private TooltipUI tooltip; 
        [SerializeField] private List<String> tooltipMessages; 
        // Button list order consistent's
        private const int EnterViewOne = 1, EnterViewTwo = 2, ExitView = 3;

        private void Start()
        {
            /* Since we don't have window functionality yet, these are unused for now */
            if (OnWindowOpened == null) { OnWindowOpened = new UnityEvent(); }
            if (OnWindowClosed == null) { OnWindowClosed = new UnityEvent(); }
            if (OnOptionOneSelected == null) { OnOptionOneSelected = new UnityEvent(); }
            if (OnOptionTwoSelected == null) { OnOptionTwoSelected = new UnityEvent(); }
            if (OnOptionThreeSelected == null) { OnOptionThreeSelected = new UnityEvent(); }
            if (OnOptionFourSelected == null) { OnOptionFourSelected = new UnityEvent(); }
            if (OnOptionFiveSelected == null) { OnOptionFiveSelected = new UnityEvent(); }

            toolbarBtnEvents = new List<UnityEvent> { OnOptionOneSelected, OnOptionTwoSelected, OnOptionThreeSelected, OnOptionFourSelected, OnOptionFiveSelected };

            Document = gameObject.GetComponent<UIDocument>();
            Root = Document.rootVisualElement;
            toolbar = Root.Q<VisualElement>("Toolbar");

            Root.style.justifyContent = Justify.Center;

            btnToolTips = new List<ButtonToolTip>();

            toolbarBtns = new List<Button> { toolbar.Q<Button>("GroupOne"), toolbar.Q<Button>("GroupTwo"), toolbar.Q<Button>("GroupThree"), toolbar.Q<Button>("GroupFour"), toolbar.Q<Button>("GroupFive") };

            SetIcons();

            for (int i = 0; i < toolbarBtns.Count; i++)
            {
                toolbarBtns[i].RegisterCallback<ClickEvent, int>(OnGroupButtonClick, i);
            }

            for (int i = 0; i < toolbarBtns.Count; i++)
            {
                btnToolTips.Add(new ButtonToolTip(toolbarBtns[i], tooltip, tooltipMessages[i]));
            }

            //on start hide the two buttons that will disappear when it reaches the starting waypoint
            HideButton(EnterViewTwo);
            HideButton(ExitView);
        }

        private void SetIcons()
        {
            VisualElement newIcon1, newIcon2, newIcon3, newIcon4, newIcon5;
            newIcon1 = toolbar.Q<Button>("GroupOne").Q<VisualElement>("Image");
            newIcon2 = toolbar.Q<Button>("GroupTwo").Q<VisualElement>("Image");
            newIcon3 = toolbar.Q<Button>("GroupThree").Q<VisualElement>("Image");
            newIcon4 = toolbar.Q<Button>("GroupFour").Q<VisualElement>("Image");
            newIcon5 = toolbar.Q<Button>("GroupFive").Q<VisualElement>("Image");
            newIcon1.style.backgroundImage = new StyleBackground(Icon1);
            newIcon2.style.backgroundImage = new StyleBackground(Icon2);
            newIcon3.style.backgroundImage = new StyleBackground(Icon3);
            newIcon4.style.backgroundImage = new StyleBackground(Icon4);
            newIcon5.style.backgroundImage = new StyleBackground(Icon5);
        }

        /// <summary>
        /// Used as a general function for all group buttons, which covers the icons and text used for each area of the toolbar.
        /// </summary>
        /// <param name="cl">Event containing target button</param>
        private void OnGroupButtonClick(ClickEvent cl, int index)
        {
            if (!IsToolbarDisabled)
                toolbarBtnEvents[index]?.Invoke();
        }
        
        // Shows the exit special view button
        public void ShowLeaveSpecialViewButton()
        {
            HideDynamicButtons();
            ShowButton(ExitView);
        }

        // Shows either one or two enter special view buttons based off a button count
        public void ShowEnterSpecialViewButtons(int buttonCount)
        {
            HideDynamicButtons();
            switch (buttonCount) 
            {
                case 0:
                    return;
                case 1:
                    ShowButton(EnterViewOne);
                    break;
                case 2:
                    ShowButton(EnterViewOne);
                    ShowButton(EnterViewTwo);
                    break;
                default:
                    Debug.LogWarning("Passing incorrect count of buttons to Toolbar.ShowEnterSpecialViewButtons");
                    break;
            }
        }

        // Hides the enter and exit buttons to get ready for the updated ones
        public void HideDynamicButtons()
        {
            HideButton(EnterViewOne);
            HideButton(EnterViewTwo);
            HideButton(ExitView);
        }

        // Helper method to hide a specific button by its order in the tool bar list
        private void HideButton(int btnNum)
        {
            UIHelper.Hide(toolbarBtns.ElementAt(btnNum - 1));
        }

        // Helper method to show a specific button by its order in the tool bar list
        private void ShowButton(int btnNum)
        {
            UIHelper.Show(toolbarBtns.ElementAt(btnNum - 1));
        }

        /// Invoked by <see cref="PointClickNavigation.WalkCompleted"/>
        public void UpdateTooltipText()
        {
            TruckMovementHandler truckMovementHandler = FindObjectOfType<TruckMovementHandler>();
            List<SpecializedView> specialViews = truckMovementHandler.WaypointSpecialViews;

            switch (specialViews.Count)
            {
                case 0: break;
                case 1: btnToolTips[0].SetMessage(specialViews[0].GetSpecialViewText()); break;
                case 2:
                    btnToolTips[0].SetMessage(specialViews[0].GetSpecialViewText());
                    btnToolTips[1].SetMessage(specialViews[1].GetSpecialViewText()); break;
                default: Debug.LogWarning("Expected 0, 1, or 2 Special View Names."); break;
            }
        }
    }
}
