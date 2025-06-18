using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    /// <summary>
    /// UI element for holding a dynamic dialogue between the player and NPC
    /// </summary>
    public class Conversation : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset conversationBodyUXML;
        [SerializeField] private VisualTreeAsset branchingBodyUXML;
        [SerializeField] private ConversationSO conversation;

        [SerializeField] private Sprite ManagerIcon;
        [SerializeField] private Sprite PlayerIcon;

        public UnityEvent<UIDocument> OnConversationClosed;
        public UnityEvent OnButtonsCreated;
        
        // UI elements on the main conversation window
        private VisualElement root;
        private Button continueBtn;
        private VisualElement contentArea;
        // Variables to control the flow of the conversation 
        private int conversationIndex;
        private bool isSpeakerTalking, skipContent, awaitingResponse;
        private List<VisualElement> activeSegments = new List<VisualElement>();
        // Holds the active branching dialog decision variables
        // (Made class variables because parameters can not be passed through assigned click events)
        private BranchingDialogSegmentSO activeBranchingDialog;
        private Button activeOptionOne;
        private Button activeOptionTwo;
        // Constants
        private const int ContentAreaLimit = 3;
        private const int BorderRadius = 25;
        private const string ManagerSpeakerName = "Truck Yard Manager", PlayerSpeakerName = "You";


        private void Start()
        {
            OnConversationClosed ??= new UnityEvent<UIDocument>();
            OnButtonsCreated ??= new UnityEvent();

            root = GetComponent<UIDocument>().rootVisualElement;
            continueBtn = root.Q<Button>("continueBtn");
            contentArea = root.Q<VisualElement>("Content");

            continueBtn.clicked += () => ContinueDialog();
        }

        /// <summary>
        /// Displays the next piece of dialog in the provided conversation.
        /// The next piece of dialog could be a branching decision or a regular text bubble
        /// </summary>
        private void ContinueDialog()
        {
            // Return if a player option is up, or a message is being written on screen
            if(awaitingResponse) { return; }
            if (isSpeakerTalking) { skipContent = true; return; }

            if (conversationIndex == conversation.DialogSegments.Count)
            {
                HandleHideConversation();
                OnConversationClosed.Invoke(GetComponent<UIDocument>());
            }
            else
            {
                if (conversation.DialogSegments[conversationIndex] is BasicDialogSegmentSO basicDialog)
                {
                    ShowBasicDialog(basicDialog.DialogMessage, basicDialog.AlignLeft);
                }
                else if (conversation.DialogSegments[conversationIndex] is BranchingDialogSegmentSO branchingDialog)
                {
                    activeBranchingDialog = branchingDialog;
                    ShowBranchingDialog();
                }
            }
            conversationIndex++;
        }

        private void ShowBranchingDialog()
        {
            awaitingResponse = true;
            // Clone the branching dialog UXML tree
            VisualElement branchingBody = branchingBodyUXML.CloneTree();
            activeOptionOne = branchingBody.Q<Button>("OptionOneBtn");
            activeOptionTwo = branchingBody.Q<Button>("OptionTwoBtn");
            // Set the dialog text
            activeOptionOne.text = activeBranchingDialog.PlayerResponses[0];
            activeOptionTwo.text = activeBranchingDialog.PlayerResponses[1];
            
            continueBtn.SetEnabled(false);

            activeOptionOne.clicked += ButtonOptionOneClick;
            activeOptionTwo.clicked += ButtonOptionTwoClick;

            contentArea.Add(branchingBody);
            activeSegments.Add(branchingBody);

            OnButtonsCreated?.Invoke();

            LimitContentArea();
        }

        private void ShowBasicDialog(string dialogMessage, bool alignLeft)
        {
            isSpeakerTalking = true;
            // Clone the conversation segment UXML tree
            VisualElement messageBody = conversationBodyUXML.CloneTree();
            VisualElement body = messageBody.Q<VisualElement>("MessageBody");
            VisualElement bodyText = messageBody.Q<VisualElement>("Right");
            VisualElement bodyImage = messageBody.Q<VisualElement>("Left");
            Label bodyTextMessage = messageBody.Q<Label>("Message");
            VisualElement messageIcon = messageBody.Q<VisualElement>("speakerIcon");
            Label speakerName = messageBody.Q<Label>("Speaker");
            
            bodyTextMessage.text = "";

            // Adjust dialog segment to show on the managers or players side
            if (alignLeft)
            {
                body.style.flexDirection = FlexDirection.Row;
                bodyText.style.alignItems = Align.FlexStart;
                bodyImage.style.alignItems = Align.FlexEnd;
                bodyTextMessage.style.borderTopLeftRadius = 0;
                bodyTextMessage.style.borderTopRightRadius = BorderRadius;
                messageIcon.style.backgroundImage = new StyleBackground(ManagerIcon);
                speakerName.text = ManagerSpeakerName;
            }
            else
            {
                body.style.flexDirection = FlexDirection.RowReverse;
                bodyText.style.alignItems = Align.FlexEnd;
                bodyImage.style.alignItems = Align.FlexStart;
                bodyTextMessage.style.borderTopLeftRadius = BorderRadius;
                bodyTextMessage.style.borderTopRightRadius = 0;
                messageIcon.style.backgroundImage = new StyleBackground(PlayerIcon);
                speakerName.text = PlayerSpeakerName;
            }

            contentArea.Add(messageBody);
            activeSegments.Add(messageBody);
            
            StartCoroutine(DisplayTextSlowly(bodyTextMessage, dialogMessage));
        }

        private IEnumerator DisplayTextSlowly(Label targetTextArea, string displayText)
        {
            LimitContentArea();

            foreach (char character in displayText)
            {
                if (skipContent)
                {
                    targetTextArea.text = displayText;
                    skipContent = false;
                    break;
                }
                targetTextArea.text += character;
                yield return new WaitForSeconds(0.05f);
            }
            isSpeakerTalking = false;
        }

        /// <summary>
        /// Click event for first choice of active dialog
        /// </summary>
        private void ButtonOptionOneClick()
        {
            activeOptionTwo.style.display = DisplayStyle.None;

            ConvertBranchToDialog(activeOptionOne);

            ShowBasicDialog(activeBranchingDialog.ManagerReplies[0], true);
            awaitingResponse = false;
            continueBtn.SetEnabled(true);
            activeOptionOne.clicked -= ButtonOptionOneClick;
        }

        /// <summary>
        /// Click event for second choice of active dialog
        /// </summary>
        private void ButtonOptionTwoClick()
        {
            activeOptionOne.style.display = DisplayStyle.None;

            ConvertBranchToDialog(activeOptionTwo);

            ShowBasicDialog(activeBranchingDialog.ManagerReplies[1], true);
            awaitingResponse = false;
            continueBtn.SetEnabled(true);
            activeOptionTwo.clicked -= ButtonOptionTwoClick;
        }

        /// <summary>
        /// Changes the style from a branching dialog button to a message bubble
        /// </summary>
        /// <param name="button"> The button where styles will be applied </param>
        private void ConvertBranchToDialog(Button button)
        {
            button.RemoveFromClassList("button-primary");
            button.AddToClassList("general-10");
            button.AddToClassList("right-dialog-border");
        }

        /// <summary>
        /// Limits the amount of dialog pieces showing at once
        /// </summary>
        private void LimitContentArea()
        {
            if (activeSegments.Count > ContentAreaLimit)
            {
                contentArea.Remove(activeSegments[0]);
                activeSegments.RemoveAt(0);
            }
        }

        private void HandleShowConversation()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.Flex;
                // Show the first piece of dialog on show
                ContinueDialog();
            }
        }

        private void HandleHideConversation()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.None;
            }
        }
    }
}
