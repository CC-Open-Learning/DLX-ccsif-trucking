using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    public class HowToPlayMenu : MonoBehaviour
    {
        private VisualElement root;

        //events
        public UnityEvent OnDialogShown;
        public UnityEvent OnNextDialog;
        public UnityEvent OnPreviousDialog;
        public UnityEvent OnUnderstoodDialog;

        //required visual elements
        private Button nextButton, previousButton, understoodButton;
        private VisualElement nextButtonContainer, previousButtonContainer, understoodButtonContainer;
        private VisualElement itemGroupOne, itemGroupTwo, itemGroupThree, itemGroupFour, itemGroupFive;
        private VisualElement circleOneHighlight, circleTwoHighlight;

        //lists
        private List<VisualElement> pageOneItems;
        private List<VisualElement> pageTwoItems;

        private void Start()
        {
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            OnDialogShown ??= new UnityEvent();
            OnNextDialog ??= new UnityEvent();
            OnPreviousDialog ??= new UnityEvent();
            OnUnderstoodDialog ??= new UnityEvent();

            //add events to button clicks
            nextButton = root.Q<Button>("NextButton");
            nextButton.clicked += () => { OnNextDialog?.Invoke(); };

            previousButton = root.Q<Button>("PreviousButton");
            previousButton.clicked += () => { OnPreviousDialog?.Invoke(); };

            understoodButton = root.Q<Button>("UnderstandButton");
            understoodButton.clicked += () => { OnUnderstoodDialog?.Invoke(); };


            //add references for different visual elements that need to be hidden or shown
            nextButtonContainer = root.Q("ButtonOne");
            previousButtonContainer = root.Q("ButtonTwo");
            understoodButtonContainer = root.Q("ButtonThree");

            itemGroupOne = root.Q<VisualElement>("GroupOne");
            itemGroupTwo = root.Q<VisualElement>("GroupTwo");
            itemGroupThree = root.Q<VisualElement>("GroupThree");
            itemGroupFour = root.Q<VisualElement>("GroupFour");
            itemGroupFive = root.Q<VisualElement>("GroupFive");

            circleOneHighlight = root.Q<VisualElement>("LeftHighlight");
            circleTwoHighlight = root.Q<VisualElement>("RightHighlight");



            //adding items to lists
            pageOneItems = new List<VisualElement>();
            pageTwoItems = new List<VisualElement>();

            pageOneItems.Add(itemGroupOne);
            pageOneItems.Add(itemGroupTwo);
            pageOneItems.Add(itemGroupThree);
            pageOneItems.Add(nextButtonContainer);
            pageOneItems.Add(circleOneHighlight);

            pageTwoItems.Add(itemGroupFour);
            pageTwoItems.Add(itemGroupFive);
            pageTwoItems.Add(previousButtonContainer);
            pageTwoItems.Add(understoodButtonContainer);
            pageTwoItems.Add(circleTwoHighlight);
            UIHelper.Hide(root);
        }

        public void LoadPageOne()
        {
            foreach (VisualElement v in pageOneItems) { UIHelper.Show(v); }
            foreach (VisualElement v in pageTwoItems) { UIHelper.Hide(v); }
        }

        public void LoadPageTwo()
        {
            foreach (VisualElement v in pageOneItems) { UIHelper.Hide(v); }
            foreach (VisualElement v in pageTwoItems) { UIHelper.Show(v); }
        }

        public void Show()
        {
            LoadPageOne();
            UIHelper.Show(root);
            OnDialogShown.Invoke();
        }

        public void Hide()
        {
            UIHelper.Hide(root);
        }
    }
}
