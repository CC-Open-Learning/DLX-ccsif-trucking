using UnityEngine;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class InteractableManager : MonoBehaviour
    {

        [SerializeField] private UIManager uiManager;
        [SerializeField] private TruckMovementHandler truckMovementHandler;
        [SerializeField] private MovementHandler movementHandler;
        
        /// Invokes <see cref="InspectableHandler.InteractableFixState"/>
        public UnityEvent<GameObject> OnInspectionInteracted;

        /// Invokes <see cref="NavigationHandler.EnterSpecialView(SpecializedView)"/>
        /// Invokes <see cref="UIManager.ShowToolbarLeaveSpecialViewButton"/>
        public UnityEvent<SpecializedView> OnClickSpecializedView;
        
        public UnityEvent<MovementWaypoint> OnClickWaypoint;
        
        private OutlineInteractable outlineInteractable;

        private void Awake()
        {
            outlineInteractable = GetComponentInChildren<OutlineInteractable>();

            if (outlineInteractable == null)
            {
                Debug.LogWarning("InteractableManager unable to find OutlineInteractable component");
            }

            OnInspectionInteracted ??= new UnityEvent<GameObject>(); 
            OnClickSpecializedView ??= new UnityEvent<SpecializedView>(); 
            OnClickWaypoint ??= new UnityEvent<MovementWaypoint>();
        }

        /// Invoked from <see cref="InteractionsHandler.MouseEnter(GameObject)"/>
        public void MouseEnterOnInteractable(GameObject obj)
        {
            if (truckMovementHandler.IsWalking || uiManager.InUserInterface) { return; }

            if (obj.TryGetComponent(out MovementWaypoint waypoint))
            {
                if(truckMovementHandler.DisableExceptionWaypoints(waypoint) || truckMovementHandler.InSpecialView) { return; }
                waypoint.HighlightWaypoint();
            }
            else
            {
                outlineInteractable.ShowOutline(obj);
            }
        }

        /// Invoked from <see cref="InteractionsHandler.MouseExit(GameObject)"/>
        public void MouseExitOnInteractable(GameObject obj)
        {
            if (obj.TryGetComponent(out MovementWaypoint waypoint))
            {
                waypoint.UnhighlightWaypoint();
            }
            else
            {
                outlineInteractable.HideOutline(obj);
            }
        }

        /// Invoked from <see cref="InteractionsHandler.MouseClick(GameObject)"/>
        public void MouseClickOnInteractable(GameObject obj)
        {
            if (movementHandler.IsMouseHeld || truckMovementHandler.IsWalking || uiManager.InUserInterface) { return; }

            outlineInteractable.HideOutline(obj, true);
            
            if (obj.TryGetComponent<SpecializedView>(out SpecializedView specializedView)) 
            {
                OnClickSpecializedView.Invoke(specializedView);
            }
            else if (obj.TryGetComponent<MovementWaypoint>(out MovementWaypoint waypoint))
            {
                if(truckMovementHandler.DisableExceptionWaypoints(waypoint) || truckMovementHandler.InSpecialView) { return; }
                OnClickWaypoint.Invoke(waypoint);
            }
            else
            {
                OnInspectionInteracted?.Invoke(obj);
            }   
        }
    }
}
