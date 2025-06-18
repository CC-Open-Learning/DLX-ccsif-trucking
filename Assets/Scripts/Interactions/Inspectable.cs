using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VARLab.Interactions;

namespace VARLab.CCSIF
{
    /// <summary>
    /// The Inspectable component will be attached to the parent object containing the fixed and broken game objects.
    /// It is responsible for related functions when switching between the two states.
    /// </summary>
    public class Inspectable : MonoBehaviour
    {
        // isBroken is used based on the seed used in InspectableHandler.cs
        private bool isStartingBroken = false;
        public bool IsStartingBroken { get => isStartingBroken; set => isStartingBroken = value; }
        
        private bool wasInspected;
        public bool WasInspected { get => wasInspected; set => wasInspected = value; }

        private GameObject fixedStateReference;
        private List<GameObject> brokenStateReferences = new List<GameObject>();

        // Uses singular reference to the broken state being used by the enabled seed, so that FixInspectabled() knows which to disable
        private GameObject activeBrokenState;
        public GameObject ActiveBrokenState { get => activeBrokenState; set => activeBrokenState = value; }

        private const float SmokeDelay = 0.5f;
        public bool IsLongInspectionFixedAnimation = false;


        /// <summary>
        /// Loads in the broken and fixed state references for the inspectable
        /// called from InspectableHandler's awake method
        /// </summary>
        public void InitializeReferences()
        {
            if (GetComponentsInChildren<Interactable>().Length == 0)
            {
                Debug.LogWarning("There are no Interactables inside the '" + name + "' Inspectable");
                return;
            }

            foreach (Interactable state in GetComponentsInChildren<Interactable>(includeInactive: true))
            {
                if (state.gameObject.TryGetComponent<BrokenInspectable>(out _))
                {
                    brokenStateReferences.Add(state.gameObject);
                } 
                else if(state.transform.parent.TryGetComponent<BrokenInspectable>(out BrokenInspectable brokenInspectable))
                {
                    brokenInspectable.HasSeparatedDamage = true;
                    continue;
                }
                else
                {
                    fixedStateReference = state.gameObject;
                }
            }
        }

        /// <summary>
        /// Sets the broken and fixed states active based on the module seed
        /// Called from InspectableHandler's awake method
        /// </summary>
        public void SetReferencesActive()
        {
            // If the inspectable starts broken ->
            // the fixed state is set inactive, all broken states are set inactive, and then the one broken state in the module seed is set active
            fixedStateReference.SetActive(!IsStartingBroken);
            foreach (GameObject brokenInspectable in brokenStateReferences)
                brokenInspectable.SetActive(false);

            if (IsStartingBroken)
                activeBrokenState.SetActive(true);
        }

        /// <summary>
        /// Responsible for switching an inspectables state from broken to fixed, relying that it is currently broken.
        /// Called from InspectableHandler
        /// </summary>
        public void FixInspectable()
        {
            if (!WasInspected && IsStartingBroken)
            {
                StartCoroutine(ChangeInspectableState());
            }
        }

        /// <summary>
        /// Changes the inspectable from its active bad state to fixed state with Coroutine delay the transformation to happen during the midst of the smoke effect
        /// </summary>
        private IEnumerator ChangeInspectableState()
        {
            WasInspected = true;
            DeploySmokeParticles();
            yield return new WaitForSeconds(SmokeDelay);
            activeBrokenState.GetComponent<BrokenInspectable>().BrokenStateSO.IsFixed = true;
            activeBrokenState.SetActive(!WasInspected);
            fixedStateReference.SetActive(WasInspected);
        }

        /// <summary>
        /// Plays all particle systems in the child of the inspectable. Allows for multiple 
        /// particle systems because the possibility of fixing an inspectable from different angles.
        /// </summary>
        private void DeploySmokeParticles()
        {
            // To make sure there is a particle system in the child
            if(!GetComponentInChildren<ParticleSystem>()) { 
                Debug.LogWarning("Missing Particle System in Inspectable"); 
                return; 
            }
            // Gets all particle systems and plays them
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }
        }

        public List<GameObject> GetBrokenInspectables() { return brokenStateReferences; }
    }
}
