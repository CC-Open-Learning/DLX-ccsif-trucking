using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class TruckMovementHandler : MonoBehaviour
    {
        [SerializeField] private CameraFade fadeToBlack;
        [SerializeField] MovementWaypoint startingWaypoint;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private UIManager uiManager;
        
        public Animator cameraAnimator;
        [HideInInspector] public bool IsWalking;
        [HideInInspector] public bool InSpecialView; 
        [HideInInspector] public List<SpecializedView> WaypointSpecialViews;
        
        private bool isEnteringSpecialView;
        private bool isExitingSpecialView;
        private MovementHandler movementHandler;
        private bool isMovementEnabled = true;
        private MovementWaypoint currentWaypoint;
        private SpecializedView currentSpecializedView;
        private Transform cameraTransform; 
        private bool hasTransitionStarted;
        private const float SnapLookTime = 0.6f, FadeInTimeNonReset = .8f, FadeInTimeReset = 1f, FadeOutTime = .5f;
        private const float SplitLookDistance = 7.5f, LookAtHeightOffset = 1.6f;
        private const int RightSideWaypointIndex = 4, LeftSideWaypointIndex = 5, FrontSideWaypointIndex = 1, BackSideWaypointIndex = 8;
        
        public UnityEvent OnWalkStarted;
        public UnityEvent<int> OnWalkCompleted;
        public UnityEvent<Transform> OnEnteredSpecialView;
        public UnityEvent<int> OnExitSpecialView;

        [SerializeField] private bool slowWalkingEnabled;
        
        private void Awake()
        {
            OnWalkStarted ??= new UnityEvent();
            OnWalkCompleted ??= new UnityEvent<int>();
            OnEnteredSpecialView ??= new UnityEvent<Transform>();
            OnExitSpecialView ??= new UnityEvent<int>();
            
            movementHandler = GetComponent<MovementHandler>();
            currentWaypoint = startingWaypoint;
               
            cameraTransform = movementHandler.cam.transform;
        }

        private void Start()
        {
            if (slowWalkingEnabled)
            {
                MoveToWaypoint(startingWaypoint);
            }
        }
        
        /// <summary>
        /// The update handle the event calls for navigation related events. It's detecting when the animator stops
        /// playing to know an animation is complete.
        /// </summary>
        private void Update()
        {
            bool isTransitioning = cameraAnimator.IsInTransition(0);

            if (isTransitioning)
            {
                hasTransitionStarted = true; 
                if (isEnteringSpecialView || isExitingSpecialView)
                {
                    InSpecialView = true;
                }
                else if (!IsWalking && !InSpecialView)
                {
                    IsWalking = true;
                    OnWalkStarted?.Invoke();
                }

                movementHandler.EnablePanAndZoom(false);
            }
            else if (hasTransitionStarted) 
            {
                hasTransitionStarted = false; 

                if (isEnteringSpecialView)
                {
                    InSpecialView = true;
                    isEnteringSpecialView = false;
                    OnEnteredSpecialView?.Invoke(currentSpecializedView.SpecialViewLookAt);
                }

                if (isExitingSpecialView)
                {
                    InSpecialView = false;
                    isExitingSpecialView = false;
                    OnExitSpecialView?.Invoke(WaypointSpecialViews.Count);
                }

                if (IsWalking && !isEnteringSpecialView && !isExitingSpecialView)
                {
                    IsWalking = false;
                    OnWalkCompleted?.Invoke(WaypointSpecialViews.Count);
                }
                CheckUserInterface();
            }
            else
            {
                CheckUserInterface();
            }
        }
        
        /// <summary>
        /// Main method that moves the player from one waypoint to another.
        /// </summary>
        /// <param name="waypoint"> Waypoint to move to </param>
        public void MoveToWaypoint(MovementWaypoint waypoint)
        {
            if(!isMovementEnabled || InSpecialView) { return; }
            if(DisableExceptionWaypoints(waypoint)) { return; }
            
            currentWaypoint.pointOfInterest.DisablePOIColliders();
            currentWaypoint = waypoint;
            currentWaypoint.pointOfInterest.EnablePOIColliders();
            
            List<SpecializedView> specialViews = currentWaypoint.pointOfInterest.GetSpecialViews();
            WaypointSpecialViews = specialViews;
            
            if (slowWalkingEnabled)
            {
                cameraAnimator.SetInteger("WaypointIndex", waypoint.waypointIndex);
                StartCoroutine(LookAtWaypoint(waypoint.transform));
            }
            else
            {
                StartCoroutine(FaceWaypoint(waypoint, false));
            }
        }
        
        /// <summary>
        /// Calls the enter special view method from the toolbar
        /// </summary>
        /// <param name="specialView"> Either the top or bottom enter special view button </param>
        public void EnterSpecialView(int specialView)
        {
            if(isExitingSpecialView) { return; }

            EnterSpecialView(WaypointSpecialViews[specialView - 1]);
        }
        
        /// <summary>
        /// Navigates to a selected special view
        /// </summary>
        /// <param name="specialView"> Special view to navigate to </param>
        public void EnterSpecialView(SpecializedView specialView)
        {
            if (isExitingSpecialView) { return; }

            currentSpecializedView = specialView;
            currentSpecializedView.ToggleCameras();
            
            cameraAnimator.SetInteger("WaypointIndex", specialView.specialViewIndex);
            StartCoroutine(LookAtSpecialView(specialView.SpecialViewLookAt));
            isEnteringSpecialView = true;
        }
        
        /// <summary>
        /// Removes player from a special view 
        /// </summary>
        public void ExitSpecialView()
        {
            currentSpecializedView.ToggleCameras();
 
            cameraAnimator.SetInteger("WaypointIndex", currentWaypoint.waypointIndex);
            StartCoroutine(LookAtSpecialView(currentSpecializedView.SpecialViewLookAt));
            isExitingSpecialView = true;
        }

        /// <summary>
        /// Quick restart is called to through the player back at the first waypoint instantly.
        /// </summary>
        /// <remarks>
        /// Module B's 2nd inspection uses quick restart because it does not have a starting menu.
        /// </remarks>
        public void QuickRestart()
        {
            currentWaypoint.pointOfInterest.DisablePOIColliders();
            currentWaypoint = startingWaypoint;
            currentWaypoint.pointOfInterest.EnablePOIColliders();
            
            List<SpecializedView> specialViews = currentWaypoint.pointOfInterest.GetSpecialViews();
            WaypointSpecialViews = specialViews;
            
            OnWalkCompleted?.Invoke(WaypointSpecialViews.Count);
            
            cameraAnimator.SetTrigger("Restart");
            Quaternion targetRotationWaypoint = Quaternion.LookRotation(currentWaypoint.transform.right);
            cameraTransform.rotation = targetRotationWaypoint;
        }
        
        /// <summary>
        /// Teleports the player to the first waypoint. Called after the Module B starting menu and
        /// Module A's play again.
        /// </summary>
        public void RestartNavigation()
        {
            if (InSpecialView)
            {
                currentSpecializedView.ToggleCameras();
                InSpecialView = false;
            }

            currentWaypoint.pointOfInterest.DisablePOIColliders();
            currentWaypoint = startingWaypoint;
            currentWaypoint.pointOfInterest.EnablePOIColliders();
            
            List<SpecializedView> specialViews = currentWaypoint.pointOfInterest.GetSpecialViews();
            WaypointSpecialViews = specialViews;
            
            OnWalkCompleted?.Invoke(WaypointSpecialViews.Count);
            
            StartCoroutine(FaceWaypoint(currentWaypoint, true));
        }
        
        /// <summary>
        /// Teleports the player to a waypoint with a fade to black. Used for Module B's navigation, and all
        /// modules reset.
        /// </summary>
        /// <param name="waypoint"> Waypoint to navigate to </param>
        /// <param name="isRestart"> If the method is being used as a reset, set fade longer </param>
        /// <returns></returns>
        private IEnumerator FaceWaypoint(MovementWaypoint waypoint, bool isRestart)
        {
            fadeToBlack.FadeIn();
            
            if (!isRestart)
            {
                yield return new WaitForSeconds(FadeInTimeNonReset);
                cameraAnimator.SetInteger("WaypointIndex", waypoint.waypointIndex);
            }
            else
            {
                yield return new WaitForSeconds(FadeInTimeReset);
                cameraAnimator.SetTrigger("Restart");
                cameraAnimator.SetInteger("WaypointIndex", 1);
                yield return new WaitForSeconds(FadeOutTime);
            }
            OnWalkCompleted?.Invoke(WaypointSpecialViews.Count);
            Quaternion targetRotationWaypoint = Quaternion.LookRotation(waypoint.transform.right);
            cameraTransform.rotation = targetRotationWaypoint;
            fadeToBlack.FadeOut();
        }
        
        /// <summary>
        /// Rotates the camera to look at a waypoint. If the waypoint is far away, it will split the travel time
        /// into 2 parts. The first part will slowly look at the waypoint destination, the second part will
        /// match the rotation of the waypoint.
        /// </summary>
        /// <param name="waypoint"> Waypoint destination </param>
        /// <returns></returns>
        private IEnumerator LookAtWaypoint(Transform waypoint)
        {
            // Smoothly rotate to look directly above the waypoint
            Vector3 lookAbovePosition = new Vector3(waypoint.position.x, waypoint.position.y + LookAtHeightOffset, waypoint.position.z);
            Quaternion targetRotationAbove = Quaternion.LookRotation(lookAbovePosition - cameraTransform.position); 

            if (Vector3.Distance(cameraTransform.position, lookAbovePosition) > SplitLookDistance)
            {
                 float elapsedTime = 0f;
                 while (elapsedTime < SnapLookTime)
                 {
                     cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotationAbove, rotationSpeed * Time.deltaTime);
                     elapsedTime += Time.deltaTime;
                     yield return null;
                 }
            }
            
            // Calculate the players speed
            float distance1 = Vector3.Distance(cameraTransform.position, lookAbovePosition);
            yield return new WaitForSeconds(.1f);
            float distance2 = Vector3.Distance(cameraTransform.position, lookAbovePosition);
            float speed = (distance1 - distance2) * 10 / 1f;
            float remainingWalkTime = Vector3.Distance(cameraTransform.position, lookAbovePosition) / speed;
            
            // Smoothly transition to the waypoint's forward facing direction
            Quaternion targetRotationWaypoint = Quaternion.LookRotation(waypoint.right);

            // Find the remaining Y axis rotation to complete
            Vector3 currentEuler = cameraTransform.rotation.eulerAngles;
            Vector3 targetEuler = targetRotationWaypoint.eulerAngles;
            float deltaY = Mathf.DeltaAngle(currentEuler.y, targetEuler.y);
            float deltaX = Mathf.DeltaAngle(currentEuler.x, 0f); 
            
            int steps = Mathf.CeilToInt(remainingWalkTime / Time.deltaTime);
            float degreesPerStepY = deltaY / steps;
            float degreesPerStepX = deltaX / steps; 
            
            for (int i = 0; i < steps; i++)
            {
                cameraTransform.rotation = Quaternion.Euler(
                    cameraTransform.rotation.eulerAngles.x + degreesPerStepX, 
                    cameraTransform.rotation.eulerAngles.y + degreesPerStepY, 
                    cameraTransform.rotation.eulerAngles.z 
                );
                yield return null;
            }
            
            cameraTransform.rotation = Quaternion.Euler(0f, targetEuler.y, cameraTransform.rotation.eulerAngles.z);
        } 
        
        /// <summary>
        /// Aligns Cameras rotation with a special views look at point.
        /// </summary>
        /// <param name="specialView"> Special view destination </param>
        /// <returns></returns>
        private IEnumerator LookAtSpecialView(Transform specialView)
        {
            Quaternion initialRotation = cameraTransform.rotation;
            Quaternion targetRotation = specialView.rotation;

            float elapsedTime = 0f;
            float duration = SnapLookTime; 

            while (elapsedTime < duration)
            {
                cameraTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cameraTransform.rotation = targetRotation;
        }

        /// <summary>
        /// Ensures waypoints at the front and end of the truck can't be reached from the middle.
        /// </summary>
        /// <param name="waypoint"> Players current waypoint </param>
        /// <returns></returns>
        public bool DisableExceptionWaypoints(MovementWaypoint waypoint)
        {
            if (currentWaypoint.waypointIndex == RightSideWaypointIndex || currentWaypoint.waypointIndex == LeftSideWaypointIndex)
            {
                waypoint.EnableHighlights(true);
                if (waypoint.waypointIndex == FrontSideWaypointIndex || waypoint.waypointIndex == BackSideWaypointIndex)
                {
                    return true;
                }
            }
            waypoint.EnableHighlights(false);
            return false;
        }

        /// <summary>
        /// Disables pan and zoom if the player is over a user interface. 
        /// </summary>
        private void CheckUserInterface()
        {
            if (uiManager.InUserInterface || uiManager.OverUIComponent)
            {
                movementHandler.EnablePanAndZoom(false);
            }
            else
            {
                movementHandler.EnablePanAndZoom(true);
            }
        }
    }
}
