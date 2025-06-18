using System.Collections;
using UnityEngine;

namespace VARLab.CCSIF
{
    public class LightFlicker : MonoBehaviour
    {
        //burntOutMat and litMat are not required for headlightSpecialCases
        [SerializeField] private Material burntOutMat;
        [SerializeField] private Material litMat;
        
        //This is the actual object that is being switched or enabled and disabled, it emits light
        [SerializeField] private GameObject lightObject;

        //This variable is for the special case of headlights, as instead of switching between two different materials
        //headlights work by enabling and disabling the light source hidden inside. 
        [SerializeField] private bool headlightSpecialCase = false; 

        private const int maxNumber = 4;
        private const float shortTime = 0.2f;
        private const float longTime = 0.8f;

        void Start()
        {
            if (headlightSpecialCase)
                StartCoroutine(enableDisableMaterialsFlicker());
            else
                StartCoroutine(switchMaterialsFlicker());
        }

        //This is the logic for the special case flickering
        private IEnumerator enableDisableMaterialsFlicker()
        {
            while (true)
            {
                int randNum = Random.Range(0, maxNumber + 1); //0-4
                switch (randNum)
                {
                    case 0: //off, on off pattern
                        lightObject.SetActive(false);
                        yield return new WaitForSeconds(shortTime);
                        lightObject.SetActive(true);
                        yield return new WaitForSeconds(shortTime);
                        lightObject.SetActive(false);
                        yield return new WaitForSeconds(shortTime);
                        break;
                    case 1: //off
                        lightObject.SetActive(false);
                        yield return new WaitForSeconds(longTime);
                        break;
                    default: //more likely than not just to be default on
                        lightObject.SetActive(true);
                        yield return new WaitForSeconds(longTime);
                        break;
                }
            }
        }

        //this is the logic for default case flickering
        private IEnumerator switchMaterialsFlicker()
        {
            while (true)
            {
                int randNum = Random.Range(0, maxNumber + 1); //0-4
                switch (randNum)
                {
                    case 0: //off, on off pattern
                        lightObject.GetComponent<MeshRenderer>().material = burntOutMat;
                        yield return new WaitForSeconds(shortTime);
                        lightObject.GetComponent<MeshRenderer>().material = litMat;
                        yield return new WaitForSeconds(shortTime);
                        lightObject.GetComponent<MeshRenderer>().material = burntOutMat;
                        yield return new WaitForSeconds(shortTime);
                        break;
                    case 1: //off
                        lightObject.GetComponent<MeshRenderer>().material = burntOutMat;
                        yield return new WaitForSeconds(longTime);
                        break;
                    default: //more likely than not just to be default on
                        lightObject.GetComponent<MeshRenderer>().material = litMat;
                        yield return new WaitForSeconds(longTime);
                        break;
                }
            }
        }
    }
}
