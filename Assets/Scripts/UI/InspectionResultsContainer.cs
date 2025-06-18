using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class InspectionResultsContainer : MonoBehaviour
    {
        public UnityEvent OnPreviousClicked;
        public UnityEvent OnNextClicked;

        [SerializeField] public string titleText;
        [SerializeField] private VisualTreeAsset resultsContentUXML;
        
        private VisualElement root, content;
        private Button previousButton, nextButton;
        private Label title;
        
        // Booleans to control visual elements based on their order in the inspection results
        [SerializeField] private bool isStartingPanel, isEndingPanel, isFirstResult;
        
        // Scoring data collected by the player that is to be shown
        [SerializeField] private ScoreDataSO firstInspectionScoreData;
        [SerializeField] private ScoreDataSO secondInspectionScoreData;
        [SerializeField] private ScoreMetricsSO scoreMetrics;

        private const float ContentPanelHeight = 393f;
        
        /// <summary>
        /// Using on enabled because the world space panels need to be active one at a time to avoid duplicating move movements
        /// </summary>
        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            content = root.Q<VisualElement>("Content");
            
            if (resultsContentUXML == null)
            {
               Debug.LogWarning("Inspection Results Container is missing a UXML reference.");
               return;
            }
            
            // Clone the provided content depending on the type of inspection results to be displayed
            VisualElement ContentToAdd = resultsContentUXML.CloneTree();
            content.Add(ContentToAdd);
            
            ContentToAdd.style.height = ContentPanelHeight;
            
            title = root.Q<Label>("Name");
            
            OnPreviousClicked ??= new UnityEvent();
            OnNextClicked ??= new UnityEvent();
            
            // Finding and setting up buttons provided by the resultsContentUXML visual tree asset
            previousButton = root.Q<Button>("Previous");
            previousButton.text = "Previous";
            previousButton.clicked += () => { OnPreviousClicked?.Invoke(); };
            if (isStartingPanel) { previousButton.SetEnabled(false); }
            
            nextButton = root.Q<Button>("Next");
            nextButton.text = "Next";
            nextButton.clicked += () => { OnNextClicked?.Invoke(); };
            if (isEndingPanel) { nextButton.text = "Play Again"; }
            
            title.text = titleText;
        }

        public void DisableButtons()
        {
            if(previousButton == null || nextButton == null) { return; }
            
            previousButton.SetEnabled(false);
            nextButton.SetEnabled(false);
        }

        public void EnableButtons()
        {
            if(previousButton == null || nextButton == null) { return; }
            
            if (!isStartingPanel)
            {
                previousButton.SetEnabled(true);
            }
            nextButton.SetEnabled(true);
        }

        /// <summary>
        /// Calculates and displays results depending on what kind of inspection results panel is being used.
        /// The type of inspection results is determined by the UXML assets name and all logic is kept in this class
        /// despite being unused by most panels.
        /// TODO decouple below methods
        /// </summary>
        public void DisplayResults()
        {
            IResultsContent resultsContent;
            
            if (TryGetComponent<InspectionListResultsContent>(out InspectionListResultsContent inspectionResultsContent))
            {
                resultsContent = inspectionResultsContent as IResultsContent;
            } 
            else if (TryGetComponent<RoadChoiceResultsContent>(out RoadChoiceResultsContent roadChoiceResultsContent))
            {
                resultsContent = roadChoiceResultsContent as IResultsContent;
            } 
            else if (TryGetComponent<FinalResultsContent>(out FinalResultsContent finalResultsContent))
            {
                resultsContent = finalResultsContent as IResultsContent;
            }
            else
            {
                Debug.LogWarning("Inspection Results Container is missing a reference to IResultsContent.");
                return;
            }
            
            resultsContent.PopulateResultsContent(root, firstInspectionScoreData, secondInspectionScoreData, scoreMetrics, isFirstResult);
        }
    }
}
