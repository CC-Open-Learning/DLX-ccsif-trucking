using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    /// <summary>
    /// UI for starting menu for the story version of the inspection
    /// </summary>
    public class StartingMenu : MonoBehaviour
    {
        [SerializeField] private string titleText;
        public UnityEvent OnPlayButtonClicked;
        public UnityEvent OnSettingsButtonClicked;
        public UnityEvent OnHowToPlayButtonClicked;
        private VisualElement root;
        private Button playButton, settingsButton, howToPlayButton;
        private Label titleLabel;
        
        private void Start()
        {
            OnPlayButtonClicked ??= new UnityEvent(); 
            OnSettingsButtonClicked ??= new UnityEvent(); 
            OnHowToPlayButtonClicked ??=  new UnityEvent(); 

            root = GetComponent<UIDocument>().rootVisualElement;
            titleLabel = root.Q<Label>("title");
            titleLabel.text = titleText;

            playButton = root.Q<Button>("playButton");
            playButton.clicked += () => { OnPlayButtonClicked?.Invoke(); };

            settingsButton = root.Q<Button>("settingsButton");
            settingsButton.clicked += () => { OnSettingsButtonClicked?.Invoke(); };

            howToPlayButton = root.Q<Button>("howToPlayButton");
            howToPlayButton.clicked += () => { OnHowToPlayButtonClicked?.Invoke(); };
        }

        public void Show()
        {
            UIHelper.Show(root);
        }

        public void Hide()
        {
            UIHelper.Hide(root);
        }
    }
}
