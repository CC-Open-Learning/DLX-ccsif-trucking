using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace VARLab.CCSIF
{
    //this class handles the logic behind the inspection results menu
    //uses functions by ProgressIndicator
    public class InspectionResults : MonoBehaviour
    {
        private VisualElement root;
        private Button playAgainButton;
        private Label fixedLabel, missingLabel, timeLabel;

        /// Invokes <see cref="InspectableHandler.ResetInspectionsToPlayAgain"/>
        /// Invokes <see cref="UIManager.ToggleInspectionResults"/>
        /// Invokes <see cref="NavigationHandler.WalkPlayerToStart"/>
        public UnityEvent OnPlayAgain;

        public UnityEvent OnMenuOpen;

        private ProgressIndicator progIndicator;
        [SerializeField] private InspectableHandler inspectableHandler;

        /// Invokes <see cref="AnalyticsHelper.DLXPlayAgain(List{string})"/>
        public UnityEvent<List<string>> OnUpdateResults;
        private int playAgainCount = 1;

        private const string DriverSideLongForm = "Driver Side";
        private const string DriverSideShortForm = "DS";
        private const string PassengerSideLongForm = "Passenger Side";
        private const string PassengerSideShortForm = "PS";
        private const string BrokenMessage = " Broken", FixedMessage = " Fixed";
        private Timer timer;

        private void Awake()
        {
            timer = new Timer();
            timer.StartTimer();
        }
        void Start()
        {
            if (OnMenuOpen == null) { OnMenuOpen = new UnityEvent(); }
            if (OnPlayAgain == null) { OnPlayAgain = new UnityEvent(); }
            if (OnUpdateResults == null) { OnUpdateResults = new UnityEvent<List<string>>(); }

            progIndicator = gameObject.GetComponent<ProgressIndicator>();

            root = GetComponent<UIDocument>().rootVisualElement;
            playAgainButton = root.Q<Button>("playAgainBtn");
            fixedLabel = root.Q<Label>("FixedLabel");
            missingLabel = root.Q<Label>("MissedLabel");
            timeLabel = root.Q<Label>("ElapsedTimeLabel");

            playAgainButton.clicked += () =>
            {
                RestartTimer();
                OnPlayAgain?.Invoke();
            };

            root.style.display = DisplayStyle.None;
        }

        //remove any existing categories
        public void ResetResults()
        {
            for (int i = progIndicator.CategoryCount-1; i >= 0; i--)
            {
                progIndicator.RemoveCategory(i);
            }
        }

        //Invoked by OnPlayAgain in InspectionResults
        public void RestartTimer()
        {
            timer.RestartTimer();
        }

        public void UpdateResults(PointOfInterest[] pointOfInterests)
        {
            ResetResults();

            fixedLabel.text = inspectableHandler.InspectableFixedAmount + " out of " + inspectableHandler.InspectableAmount;
            missingLabel.text = inspectableHandler.InspectableBrokenAmount + " out of " + inspectableHandler.InspectableAmount;

            timer.StopTimer();

            TimeSpan ts = timer.TimeTaken;
            timeLabel.text = ts.Hours + " hr " + ts.Minutes + " mins";

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            List<PointOfInterest> notEmptyPoints = new List<PointOfInterest>();

            //reverse the order of POI as well as remove empties
            foreach (PointOfInterest poi in pointOfInterests.Reverse())
            {
                int count = 0;

                foreach (Inspectable inspect in poi.GetInspectables())
                    if (inspect.IsStartingBroken)
                        count++;

                if (count != 0)
                    notEmptyPoints.Add(poi);
            }

            //for each point of interest format and add the name of the category
            foreach (PointOfInterest poi in notEmptyPoints)
            {
                string catName = poi.name;

                if (catName.Contains(DriverSideShortForm))
                    catName = catName.Replace(DriverSideShortForm, DriverSideLongForm);

                if (catName.Contains(PassengerSideShortForm))
                    catName = catName.Replace(PassengerSideShortForm, PassengerSideLongForm);

                progIndicator.AddCategory(catName);
            }

            // Data to send to analytics
            List<string> completionData = new List<string>();
            completionData.Add(playAgainCount.ToString());
            playAgainCount++;

            //for each inspectable at every poi, if it was starting broken add it to the list, and if it was fixed progress the task
            int i = 0;
            foreach (PointOfInterest poi in notEmptyPoints)
            {
                int j = 0;
                foreach (Inspectable inspect in poi.GetInspectables())
                {
                    if (inspect.IsStartingBroken)
                    {
                        string compName = inspect.GetComponentInChildren<BrokenInspectable>(includeInactive: true).BrokenStateSO.ComponentName;

                        progIndicator.AddTask(i, textInfo.ToTitleCase(compName));

                        if (inspect.WasInspected)
                        {
                            progIndicator.AddProgressToTask(i, j);
                            completionData.Add(compName + FixedMessage);
                        } 
                        else
                        {
                            completionData.Add(compName + BrokenMessage);
                        }
                        j++;
                    }
                }
                i++;
            }

            // Invoke event with updated results for analytics
            OnUpdateResults.Invoke(completionData);
        }
    }
}
