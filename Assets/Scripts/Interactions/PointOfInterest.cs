using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VARLab.CCSIF
{
    public class PointOfInterest : MonoBehaviour
    {
        private List<Inspectable> inspectables;
        private List<SpecializedView> specialViews;

        /// A list of all inspectables that should be interactable from a waypoint that belong to another waypoint
        [SerializeField] private List<Inspectable> exceptions;

        private void Start()
        {
            DisablePOIColliders();
        }
        
        // Called from Inspectable Handler, used to gather all inspectables
        public List<Inspectable> GetInspectables() 
        {
            inspectables = GetComponentsInChildren<Inspectable>().ToList();
            return inspectables; 
        }

        public List<SpecializedView> GetSpecialViews()
        {
            specialViews = GetComponentsInChildren<SpecializedView>().ToList();
            return specialViews;
        }

        public void DisablePOIColliders()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>(true)) 
            {
                collider.enabled = false; 
            }
            foreach (Inspectable inspectable in exceptions)
            {
                List<Collider> colliders = inspectable.GetComponentsInChildren<Collider>(includeInactive: true).ToList();
                foreach (Collider collider in colliders) { collider.enabled = false; }
            }
        }

        public void EnablePOIColliders()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>(true)) 
            {
                collider.enabled = true; 
            }
            foreach (Inspectable inspectable in exceptions)
            {
                List<Collider> colliders = inspectable.GetComponentsInChildren<Collider>(includeInactive: true).ToList();
                foreach (Collider collider in colliders) { collider.enabled = true; }
            }
        }
    }
}
