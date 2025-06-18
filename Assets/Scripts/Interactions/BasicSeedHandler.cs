using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    public class BasicSeedHandler : MonoBehaviour, ISeedHandler
    {
        private BasicSeedSO moduleSeed;
        [SerializeField] private BasicSeedSO scenarioA;
        [SerializeField] private BasicSeedSO scenarioB;

        private const int ScenarioMin = 1, ScenarioMax = 3;

        /// <summary>
        /// Initializes and activates all inspections based off the module seed being used for a Module A inspection 
        /// </summary>
        /// <param name="allInspections"> All inspections in the scene </param>
        /// <returns> A list of all the inspections that are being set broken </returns>
        public List<Inspectable> SetInspectableStatus(List<Inspectable> allInspections)
        {
            // Choose random between 2 module scenarios
            int randomValue = Random.Range(ScenarioMin, ScenarioMax);
            if (randomValue == ScenarioMin)
            {
                moduleSeed = scenarioA;
            }
            else
            {
                moduleSeed = scenarioB;
            }

            // Gathering out the broken state scriptable objects that were set in the module seed
            List<BrokenStateSO> activeBrokenStates = new List<BrokenStateSO>();
            if (moduleSeed == null || moduleSeed.BrokenStateSelectors.Count == 0)
            {
                return SetAllBroken(allInspections);
            } 
            else
            {
                foreach (BrokenStateSelectorSO brokenStateGroup in moduleSeed.BrokenStateSelectors)
                {
                    if (brokenStateGroup.SelectCount >= brokenStateGroup.BrokenStates.Count)
                    {
                        foreach (BrokenStateSO brokenState in brokenStateGroup.BrokenStates)
                        {
                            activeBrokenStates.Add(brokenState);
                        }
                    } 
                    else
                    {
                        List<BrokenStateSO> tempList = new List<BrokenStateSO>(brokenStateGroup.BrokenStates);
                        for (int i = 0; i < brokenStateGroup.SelectCount; i++)
                        {
                            int index = Random.Range(0, tempList.Count);
                            activeBrokenStates.Add(tempList[index]);
                            tempList.RemoveAt(index);
                        }
                    }
                }
            }

            // Initialize and activate all inspections based off the gathered broken state scriptable objects
            List<Inspectable> brokenInspections = new List<Inspectable>();
            foreach (Inspectable inspectable in allInspections)
            {
                inspectable.InitializeReferences();
                foreach(GameObject inspect in inspectable.GetBrokenInspectables())
                {
                    if (activeBrokenStates.Contains(inspect.GetComponent<BrokenInspectable>().BrokenStateSO))
                    {
                        inspectable.ActiveBrokenState = inspect;
                        inspectable.IsStartingBroken = true;
                        brokenInspections.Add(inspectable);
                        break;
                    }
                }
                inspectable.SetReferencesActive();
            }

            return brokenInspections;
        }

        /// <summary>
        /// Sets all inspections active based on their order in the hierarchy 
        /// </summary>
        /// <param name="allInspections"> All inspections in the scene </param>
        /// <returns> A list of all the inspections that are being set broken </returns>
        private List<Inspectable> SetAllBroken(List<Inspectable> allInspections)
        {
            List<Inspectable> brokenInspections = new List<Inspectable>();
            foreach (Inspectable inspectable in allInspections)
            {
                inspectable.InitializeReferences();

                if (inspectable.GetBrokenInspectables().Count == 0) { continue; }
                inspectable.ActiveBrokenState = inspectable.GetBrokenInspectables()[0];
                inspectable.IsStartingBroken = true;
                brokenInspections.Add(inspectable);

                inspectable.SetReferencesActive();
            }

            return brokenInspections;
        }
    }
}
