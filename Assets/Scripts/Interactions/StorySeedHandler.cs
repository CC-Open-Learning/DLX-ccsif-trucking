using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    public class StorySeedHandler : MonoBehaviour, ISeedHandler
    {
        [SerializeField] private StorySeedSO moduleSeed;
        [SerializeField] private ScoreDataSO firstInspectionScoreData;
        [SerializeField] private ScoreDataSO secondInspectionScoreData;
        [SerializeField] private bool firstInspection;

        public List<Inspectable> SetInspectableStatus(List<Inspectable> allInspections)
        {
            // Gathering out the broken state scriptable objects that were set in the module seed
            List<BrokenStateSO> activeBrokenStates = new List<BrokenStateSO>();
            if (moduleSeed == null || moduleSeed.BrokenStateSelectorsOne == null || moduleSeed.BrokenStateSelectorsTwo == null)
            {
                return SetAllBroken(allInspections);
            }
            else if (firstInspection)
            {
                if (moduleSeed.ResetPlaythrough)
                {
                    moduleSeed.PlayThroughCount = 1;
                } 
                else
                {
                    moduleSeed.PlayThroughCount++;
                    moduleSeed.ResetPlaythrough = true;
                }

                ResetModuleSeed();

                // Add all BrokenStateSO's from the first inspection list and the damage causers in the linked inspections list
                firstInspectionScoreData.FirstInspections = AddFromBrokenStateGroup(moduleSeed.BrokenStateSelectorsOne);

                foreach (LinkedInspectionsSO linkedInspections in moduleSeed.LinkedInspections)
                {
                    firstInspectionScoreData.StartingLinkedInspections.Add(linkedInspections.DamageSource);
                }
                activeBrokenStates.AddRange(firstInspectionScoreData.FirstInspections);
                activeBrokenStates.AddRange(firstInspectionScoreData.StartingLinkedInspections);
            }
            else 
            {
                // Add all not fixed inspections from the first list
                foreach (BrokenStateSO brokenState in firstInspectionScoreData.FirstInspections)
                {
                    if (!brokenState.IsFixed)
                    {
                        secondInspectionScoreData.MissedFirstInspections.Add(brokenState);
                    }
                }
                // Add all inspections from the second list
                secondInspectionScoreData.SecondInspections.AddRange(AddFromBrokenStateGroup(moduleSeed.BrokenStateSelectorsTwo));
                // Add all effected inspections from the linked list if the causer was missed
                foreach (LinkedInspectionsSO linkedInspections in moduleSeed.LinkedInspections)
                {
                    if (!linkedInspections.DamageSource.IsFixed)
                    {
                        secondInspectionScoreData.StartingLinkedInspections.Add(linkedInspections.DamageSource);
                        foreach (BrokenStateSO brokenState in linkedInspections.DamageEndpoints)
                        {
                            secondInspectionScoreData.EndingLinkedInspections.Add(brokenState);
                        }
                    }
                }
                // Add all inspections that occurred from making a bad road choice
                if (firstInspectionScoreData.RoadChoice == RoadChoiceType.BadRoadChoice)
                {
                    secondInspectionScoreData.BadRoadChoiceInspections.AddRange(AddFromBrokenStateGroup(moduleSeed.BadRoadChoiceConsequences));
                }

                activeBrokenStates.AddRange(secondInspectionScoreData.MissedFirstInspections);
                activeBrokenStates.AddRange(secondInspectionScoreData.SecondInspections);
                activeBrokenStates.AddRange(secondInspectionScoreData.StartingLinkedInspections);
                activeBrokenStates.AddRange(secondInspectionScoreData.EndingLinkedInspections);
                activeBrokenStates.AddRange(secondInspectionScoreData.BadRoadChoiceInspections);
            } 

            // Initialize and activate all inspections based off the gathered broken state scriptable objects
            List<Inspectable> brokenInspections = new List<Inspectable>();
            foreach(Inspectable inspectable in allInspections)
            {
                inspectable.InitializeReferences();
                foreach (GameObject inspect in inspectable.GetBrokenInspectables())
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

        private List<Inspectable> SetAllBroken(List<Inspectable> allInspections)
        {
            List <Inspectable> brokenInspections = new List<Inspectable>();
            foreach(Inspectable inspectable in allInspections)
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

        private List<BrokenStateSO> AddFromBrokenStateGroup(List<BrokenStateSelectorSO> brokenStateSelectorSOs)
        {
            List<BrokenStateSO> activeBrokenStates = new List<BrokenStateSO>();

            foreach (BrokenStateSelectorSO brokenStateGroup in brokenStateSelectorSOs)
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
                        int index = UnityEngine.Random.Range(0, tempList.Count);
                        activeBrokenStates.Add(tempList[index]);
                        tempList.RemoveAt(index);
                    }
                }
            }

            return activeBrokenStates;
        }

        private void ResetModuleSeed()
        {
            // Set the IsFixed property on all the broken states to false.
            ResetBrokenStateSelector(moduleSeed.BrokenStateSelectorsOne, false);

            // Set the IsFixed property on all the broken states to false.
            ResetBrokenStateSelector(moduleSeed.BrokenStateSelectorsTwo, false);

            // Set the IsFixed property on all the broken states to false.
            foreach (LinkedInspectionsSO linkedInspections in moduleSeed.LinkedInspections)
            {
                linkedInspections.DamageSource.IsFixed = false;
                foreach(BrokenStateSO damageEndpoints in linkedInspections.DamageEndpoints)
                {
                    damageEndpoints.IsFixed = false;
                }
            }
            
            firstInspectionScoreData.ResetData();
            secondInspectionScoreData.ResetData();
        }

        /// <summary>
        /// Sets all broken state SO's property 'IsFixed' to false, in a broken state selector list
        /// </summary>
        /// <param name="brokenStateSelectorSO"> Broken state selector list </param>
        private void ResetBrokenStateSelector(List<BrokenStateSelectorSO> brokenStateSelectorSO, bool toEnable)
        {
            foreach (BrokenStateSelectorSO brokenStateGroup in brokenStateSelectorSO)
            {
                foreach (BrokenStateSO brokenState in brokenStateGroup.BrokenStates)
                {
                    brokenState.IsFixed = toEnable;
                }
            }
        }
    }
}
