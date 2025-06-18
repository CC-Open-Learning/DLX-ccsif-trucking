using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Responsible for the initialization of the inspectables and setting them to start broken base of the module seed.
    /// Handles all inspectable state change and keeps the important data used in the inspection results.
    /// </summary>
    public class InspectableHandler : MonoBehaviour
    {
        /// Invokes <see cref="InspectionResults.UpdateResults(PointOfInterest[])"/>
        public UnityEvent<PointOfInterest[]> OnProgressionComplete;

        //this is a list of ALL inspectables, active or not
        private List<Inspectable> allInspectables = new List<Inspectable>(); 

        //these are lists of only ACTIVE inspectables
        private List<Inspectable> brokenInspectables = new List<Inspectable>();
        private List<Inspectable> fixedInspectables = new List<Inspectable>();

        public int InspectableAmount { get { return fixedInspectables.Count + brokenInspectables.Count; } }
        public int InspectableFixedAmount { get { return fixedInspectables.Count; } }
        public int InspectableBrokenAmount { get { return brokenInspectables.Count; } }

        [SerializeField] private MonoBehaviour seedHandler;
        private ISeedHandler seedHandlerInterface;

        [SerializeField] private Notification notificationInstance;

        private int mechanicalFailures = 0;
        private int lawEnforcementIssues = 0;

        private const string NotificationPrefix = "The ";
        private const string NotificationSuffixSingular = " appears";
        private const string NotificationSuffixPlural = " appear";
        private const string NotificationSuffixFixed = " to be in good condition";
        private const string NotificationSuffixBroken = " intact here, keep looking for the damage";

        private PointOfInterest[] pointsOfInterest;


        public UnityEvent OnInspectionFixedShort;
        public UnityEvent OnInspectionFixedLong;
        public UnityEvent OnInspectionInteracted;
        private void Awake()
        {
            if (OnProgressionComplete == null) { OnProgressionComplete = new UnityEvent<PointOfInterest[]>(); }

            pointsOfInterest = FindObjectsOfType<PointOfInterest>();
            foreach (PointOfInterest poi in pointsOfInterest)
            {
                poi.DisablePOIColliders();
                allInspectables.AddRange(poi.GetInspectables());
            }

            // Obtain reference to ISeedHandler used for both Module A and B module seeds
            seedHandlerInterface = seedHandler as ISeedHandler;
            if(seedHandlerInterface == null) { Debug.LogWarning("Seed Handler provided does not inherit ISeedHandler"); return; }

            // Set up inspectables
            brokenInspectables = seedHandlerInterface.SetInspectableStatus(allInspectables);

            OnInspectionFixedShort ??= new UnityEvent();
            OnInspectionFixedLong ??= new UnityEvent();
            OnInspectionInteracted ??= new UnityEvent();
        }

        /// Invoked by <see cref="InteractableManager.OnInspectionInteracted"/>
        public void InteractableFixState(GameObject obj)
        {
            Inspectable inspectable = obj.GetComponentInParent<Inspectable>();

            if (obj.TryGetComponent<BrokenInspectable>(out BrokenInspectable brokenInspectable))
            {
                if (brokenInspectable.HasSeparatedDamage)
                {
                    // Notification for damaged inspection with separate damage
                    InspectionNotification(inspectable, obj, false);
                    OnInspectionInteracted?.Invoke();
                }
                else
                {
                    // Player clicked on actual damage coverage
                    FixInspection(inspectable);
                    if (inspectable.IsLongInspectionFixedAnimation)
                        OnInspectionFixedLong?.Invoke();
                    else
                        OnInspectionFixedShort?.Invoke();
                }
            } 
            else if (obj.transform.parent.TryGetComponent<BrokenInspectable>(out BrokenInspectable damagedInspectable))
            {
                // Fix a regular broken inspection
                FixInspection(inspectable);
                if (inspectable.IsLongInspectionFixedAnimation)
                    OnInspectionFixedLong?.Invoke();
                else
                    OnInspectionFixedShort?.Invoke();
            }
            else
            {
                // Notification for fixed inspection
                InspectionNotification(inspectable, obj, true);
                OnInspectionInteracted?.Invoke();
            }
        }

        private void InspectionNotification(Inspectable inspectable, GameObject obj, bool isFixed)
        {
            string inspectableName = NotificationPrefix;
            bool hasBrokenState = inspectable.GetBrokenInspectables().Count > 0;
            BrokenStateSO brokenState;

            if (isFixed)
            {
                if (!hasBrokenState)
                {
                    if (obj.name.EndsWith("s"))
                        inspectableName += obj.name + NotificationSuffixPlural;
                    else
                        inspectableName += obj.name + NotificationSuffixSingular;
                }
                else
                {
                    brokenState = obj.GetComponentInParent<Inspectable>().GetBrokenInspectables().ElementAt<GameObject>(0).GetComponent<BrokenInspectable>().BrokenStateSO;
                    if (brokenState.IsNamePlural)
                        inspectableName += brokenState.ComponentName + NotificationSuffixPlural;
                    else
                        inspectableName += brokenState.ComponentName + NotificationSuffixSingular;
                }
                // Add fixed text
                // Send notification fixed
                inspectableName += NotificationSuffixFixed;
                notificationInstance.HandleDisplayUI(NotificationType.Info, inspectableName, FontSize.Large, Align.FlexStart);
            } 
            else
            {
                brokenState = obj.GetComponent<BrokenInspectable>().BrokenStateSO;
                if(brokenState.IsNamePlural)
                    inspectableName += brokenState.ComponentName + NotificationSuffixPlural;
                else
                    inspectableName += brokenState.ComponentName + NotificationSuffixSingular;

                // Add warning notification text
                // send notification warning
                inspectableName += NotificationSuffixBroken;
                notificationInstance.HandleDisplayUI(NotificationType.Error, inspectableName, FontSize.Large, Align.FlexStart);
            }
        }

        private void FixInspection(Inspectable inspectable)
        {
            if (brokenInspectables.Contains(inspectable))
            {
                inspectable.FixInspectable();
                brokenInspectables.Remove(inspectable);
                fixedInspectables.Add(inspectable);
            }
        }

        /// <summary>
        /// Checks every type of remaining active inspectable and counts number of mechanical failures and law enforcement issues
        /// </summary>
        public Endings GetEndingType()
        {
            mechanicalFailures = 0;
            lawEnforcementIssues = 0;
            foreach (Inspectable inspectable in brokenInspectables)
            {
                GameObject brokenState = inspectable.ActiveBrokenState;
                if (brokenState.GetComponent<BrokenInspectable>().BrokenStateSO.inspectableType == InspectableType.MechanicalFailure)
                {
                    mechanicalFailures++;
                }
                else if (brokenState.GetComponent<BrokenInspectable>().BrokenStateSO.inspectableType == InspectableType.LawEnforcementIssue)
                {
                    lawEnforcementIssues++;
                }
                else
                {
                    Debug.LogWarning("Third unknown enum added. Inspectable Handler.cs. InspectableTypes.cs");
                }
            }
            return DetermineEndingType();
        }

        /// <summary>
        /// Given number of mechanical issues and law issues, returns type of ending
        /// If no issues, returns happy ending, otherwise returns type with most issues. Defaults to mechanical issue if tied
        /// </summary>
        private Endings DetermineEndingType()
        {
            if (mechanicalFailures == 0 && lawEnforcementIssues == 0)
            {
                return Endings.HappyEnding;
            }
            else if (mechanicalFailures >= lawEnforcementIssues)
            {
                return Endings.MechanicalFailureEnding;
            }
            else
            {
                return Endings.LawEnforcementEnding;
            }
        }

        /// Invoked from <see cref="ConfirmationDialog.OnDialogConfirmed"/>
        public void PassPointsOfInterest()
        {
            OnProgressionComplete.Invoke(pointsOfInterest);
        }

        /// Invoked from <see cref="InspectionResults.OnPlayAgain"/>
        public void ResetInspectionsToPlayAgain()
        {
            //reset variables
            brokenInspectables.Clear();
            fixedInspectables.Clear();

            foreach (Inspectable inspectable in allInspectables)
            {
                inspectable.WasInspected = false;
                inspectable.IsStartingBroken = false;
            }
            
            brokenInspectables = seedHandlerInterface.SetInspectableStatus(allInspectables);
        }
    }
}
