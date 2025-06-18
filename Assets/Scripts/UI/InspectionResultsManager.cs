using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    public class InspectionResultsManager : MonoBehaviour
    {
        private UIDocument activeDocument;

        [SerializeField] private List<UIDocument> inspectionResultPanels;
        private int activeDocumentIndex;

        private const float PanelMoveDuration = 0.45f, PanelMoveDistance = 4f, PanelMoveRotation = -55.86f;

        private void Awake()
        {
            activeDocumentIndex = 0;
            
            for (int i = 1; i < inspectionResultPanels.Count; i++)
            {
                RemovePanelSettings(inspectionResultPanels[i]);
                inspectionResultPanels[i].transform.parent.GetComponent<Collider>().enabled = false;
                inspectionResultPanels[i].GetComponent<InspectionResultsContainer>().DisableButtons();
            }
            
            SetupPanelSettings(inspectionResultPanels[activeDocumentIndex]);
            inspectionResultPanels[activeDocumentIndex].transform.parent.GetComponent<Collider>().enabled = true;
            inspectionResultPanels[activeDocumentIndex].GetComponent<InspectionResultsContainer>().EnableButtons();
        }
        
        private void RemovePanelSettings(UIDocument panel)
        {
            panel.panelSettings.SetScreenToPanelSpaceFunction(null);
        }

        /// <summary>
        /// Method that converts UV maping coordinate into UI Toolkit UI coordinate
        /// </summary>
        /// <param name="uiDoc"></param>
        private void SetupPanelSettings(UIDocument uiDoc)
        {
            uiDoc.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) =>
            { 
                var invalidPosition = new Vector2(float.NaN, float.NaN);
                var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("UI")))
                {
                    return invalidPosition;
                }

                Vector2 pixelUI = hit.textureCoord;

                pixelUI.y = 1 - pixelUI.y;
                pixelUI.x *= uiDoc.panelSettings.targetTexture.width;
                pixelUI.y *= uiDoc.panelSettings.targetTexture.height;

                return pixelUI;
            });
        }

        public void ChangeResultsPanel(bool toNextPanel)
        {
            RemovePanelSettings(inspectionResultPanels[activeDocumentIndex]);
            inspectionResultPanels[activeDocumentIndex].transform.parent.GetComponent<Collider>().enabled = false;
            inspectionResultPanels[activeDocumentIndex].GetComponent<InspectionResultsContainer>().DisableButtons();
            
            if (toNextPanel)
            {
                SlidePanelPosition(inspectionResultPanels[activeDocumentIndex].transform.parent, true);
                activeDocumentIndex++;
            }
            else
            {
                activeDocumentIndex--;
                SlidePanelPosition(inspectionResultPanels[activeDocumentIndex].transform.parent, false);
            }
            
            SetupPanelSettings(inspectionResultPanels[activeDocumentIndex]);
            inspectionResultPanels[activeDocumentIndex].transform.parent.GetComponent<Collider>().enabled = true;
            inspectionResultPanels[activeDocumentIndex].GetComponent<InspectionResultsContainer>().EnableButtons();
        }

        private void SlidePanelPosition(Transform target, bool moveLeft)
        {
            StartCoroutine(SlidePanelPositionCoroutine(target, moveLeft));
        }
        
        /// <summary>
        /// Controls the movement of the world space UI panels.
        /// TODO Remove magic numbers, turn them into dynamic variables dependent on the type of ending
        /// </summary>
        /// <param name="target"></param>
        /// <param name="moveLeft"></param>
        /// <returns></returns>
        private IEnumerator SlidePanelPositionCoroutine(Transform target, bool moveLeft)
        {
            float panelMoveDuration = PanelMoveDuration;
            float elapsedTime = 0f;

            float panelMoveDistance = PanelMoveDistance, panelMoveRotation = PanelMoveRotation;
            
            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;
            
            if (!moveLeft)
            {
                panelMoveDistance = -panelMoveDistance;
                panelMoveRotation = -panelMoveRotation;
            }
            
            Vector3 endPosition = startPosition + new Vector3(0, 0, panelMoveDistance);
            Quaternion endRotation = startRotation * Quaternion.Euler(0, panelMoveRotation, 0);

            while (elapsedTime < PanelMoveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / PanelMoveDuration);

                target.position = Vector3.Lerp(startPosition, endPosition, t);
                target.rotation = Quaternion.Lerp(startRotation, endRotation, t);

                yield return null;
            }

            target.position = endPosition;
            target.rotation = endRotation;
        }
    }
}
