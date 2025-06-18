using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    /// <summary>
    /// UI for the cutscene road choice that will determine the travel path of the truck
    /// </summary>
    public class RoadChoice : MonoBehaviour, IUserInterface
    {
        [SerializeField] private string roadChoiceQuestion;
        [SerializeField] private string responseOne;
        [SerializeField] private string responseTwo;

        public UnityEvent OnResponseOneClicked;
        public UnityEvent OnResponseTwoClicked;
        
        public UnityEvent OnResponseOneEntered;
        public UnityEvent OnResponseTwoEntered;
        
        private VisualElement root;
        private Label labelRoadChoiceQuestion;
        private Button buttonResponseOne, buttonResponseTwo;
        
        private void Start()
        {
            OnResponseOneClicked ??= new UnityEvent();
            OnResponseTwoClicked ??= new UnityEvent();
            
            OnResponseOneEntered ??= new UnityEvent();
            OnResponseTwoEntered ??= new UnityEvent();
            
            // Get UI elements
            root = GetComponent<UIDocument>().rootVisualElement;
            labelRoadChoiceQuestion = root.Q<Label>("Question");
            buttonResponseOne = root.Q<Button>("ResponseOne");
            buttonResponseTwo = root.Q<Button>("ResponseTwo");
            
            // Assign button click events
            buttonResponseOne.clicked += () => { OnResponseOneClicked?.Invoke(); };
            buttonResponseTwo.clicked += () => { OnResponseTwoClicked?.Invoke(); };
            
            buttonResponseOne.RegisterCallback<MouseEnterEvent>(evt => OnResponseOneEntered?.Invoke());
            buttonResponseTwo.RegisterCallback<MouseEnterEvent>(evt => OnResponseTwoEntered?.Invoke());
                
            // Fill in road choice content
            labelRoadChoiceQuestion.text = roadChoiceQuestion;
            buttonResponseOne.text = responseOne;
            buttonResponseTwo.text = responseTwo;

            Hide();
        }

        public void Show()
        {
            root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            root.style.display = DisplayStyle.None;
        }
    }
}
