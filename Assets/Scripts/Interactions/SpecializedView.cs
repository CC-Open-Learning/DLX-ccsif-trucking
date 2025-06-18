using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VARLab.Interactions;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Handles the events relating to entering a special view including swapping the cameras and disabling interactables 
    /// </summary>
    public class SpecializedView : MonoBehaviour
    {
        [SerializeField] public Transform SpecialViewLookAt; 
        private bool isOpen;
        private PointOfInterest pointOfInterest;
        [SerializeField] private string specialViewText;
        [SerializeField] private List<Inspectable> exceptions;
        [SerializeField] public int specialViewIndex;

        private void Awake()
        {
            if(transform.parent != null || transform.parent.parent != null)
            {
                if (!transform.parent.parent.TryGetComponent<PointOfInterest>(out pointOfInterest))
                {
                    Debug.LogWarning("Special View couldn't find its point of interest, it may be misplaced in the hierarchy");
                }
            } 
            else
            {
                Debug.LogWarning("Special View couldn't find a parent, it may be misplaced in the hierarchy");
            }
        }

        /// <summary>
        /// Use the CameraManager.cs singleton to toggle between specializedViewCamera and the main navigation camera
        /// Also enables and disables colliders so that interactables outside the special view are disabled
        /// </summary>
        public void ToggleCameras()
        {
            SetSpecialViewComponents(isOpen);

            if (isOpen)
            {
                pointOfInterest.EnablePOIColliders();
            }
            else
            {
                // When entering a special view, no other interactables outside the special view should be interacted with
                // Turn off all colliders at the POI (which includes colliders at entered special view)
                pointOfInterest.DisablePOIColliders();
                // Enable all colliders at current specialized view
                foreach (Collider collider in transform.parent.GetComponentsInChildren<Collider>(true)) { collider.enabled = true; }
                // Enables all exception inspection colliders
                foreach (Inspectable inspectable in exceptions)
                {
                    List<Collider> colliders = inspectable.GetComponentsInChildren<Collider>(includeInactive: true).ToList();
                    foreach (Collider collider in colliders) { collider.enabled = true; }
                }

                GetComponent<Collider>().enabled = false;
            }

            isOpen = !isOpen;
        }


        /// <summary>
        /// Helper method to toggle the Collider and Interactable components on and off 
        /// </summary>
        private void SetSpecialViewComponents(bool state)
        {
            Collider boxCollider = GetComponent<Collider>();
            Interactable interactable = GetComponent<Interactable>();

            if (boxCollider != null)
            {
                boxCollider.enabled = state;
            }

            if(interactable != null)
            {
                interactable.enabled = state;
            }
        }

        public string GetSpecialViewText()
        {
            return specialViewText;
        }
    }
}
