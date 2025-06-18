using UnityEngine;

namespace VARLab.CCSIF
{
    public class MovementHandler : MonoBehaviour
    {
        [SerializeField] public Camera cam;
        
        [HideInInspector] public float CameraPanSensitivity = 1f;
        [HideInInspector] public float CameraZoomSensitivity = 25f;
        [HideInInspector] public float MinFieldOfView = 30f;
        [HideInInspector] public float MaxFieldOfView = 60f;

        private bool specialViewClamps;
        [HideInInspector] public float verticalClampMin = -50f;
        [HideInInspector] public float verticalClampMax = 50f;
        [HideInInspector] public float horizontalClampMin = -50f;
        [HideInInspector] public float horizontalClampMax = 50f;

        [HideInInspector] public bool IsMouseHeld;
        private float mouseBtnHoldTime = 0f;
        private const float MouseHoldCutoffTime = 0.17f;
        
        private const float StandardClampSize = 50f;
        
        public float PlatformSensitivityModifier =>
            Application.platform == RuntimePlatform.WebGLPlayer
                ? 0.25f
                : 1f;
        
        private bool focusGainedFrame;

        private bool isPanningEnabled = true;
        private bool isZoomingEnabled = true;

        //property for the tilt of the current camera. Tilt is the movement of the Vertical Axis.
        public float Tilt  
        {
            get { return NormalizeAngle(cam.transform.localEulerAngles.x); } 
            set { PanOrTilt( NormalizeAngle(value), NormalizeAngle(cam.transform.localEulerAngles.y)); } 
        }

        //property for the pan of the current camera. Pan is the movement of the Horizontal Axis.
        public float Pan
        {
            get { return NormalizeAngle(cam.transform.localEulerAngles.y); } 
            set { PanOrTilt(NormalizeAngle(cam.transform.localEulerAngles.x), NormalizeAngle(value)); }
        }

        //property for the zoom of the current camera. 
        public float Zoom
        {
            get { return cam.fieldOfView; }
            set { cam.fieldOfView = value; }
        }
        
        public float Sensitivity
        {
            get { return CameraPanSensitivity; }
            set { CameraPanSensitivity = value; }
        }
        
        private MouseButtonState mouseBtnState;
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseBtnState = MouseButtonState.Down;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseBtnState = MouseButtonState.Up;
            }

            if (mouseBtnHoldTime > MouseHoldCutoffTime)
            {
                IsMouseHeld = true;
            }
            
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f) 
            {
                ZoomCamera();
            }
            
            if (mouseBtnState == MouseButtonState.Down)
            {
                mouseBtnHoldTime += Time.deltaTime;
                MoveCamera();
            }
            else if (mouseBtnState == MouseButtonState.Up)
            {
                IsMouseHeld = false;
                mouseBtnHoldTime = 0f;
                mouseBtnState = MouseButtonState.Neutral;
            }
        }

        private void MoveCamera()
        {
            if(!isPanningEnabled) { return; }
            
            if (focusGainedFrame)
            {
                focusGainedFrame = false;
                return;
            }
            float modifier = CameraPanSensitivity * PlatformSensitivityModifier;
            
            Vector3 currentEulerAngles = cam.transform.localEulerAngles;
            
            float newXRotation = currentEulerAngles.x + Input.GetAxis("Mouse Y") * modifier;
            float newYRotation = currentEulerAngles.y - Input.GetAxis("Mouse X") * modifier;
            
            if (newXRotation > 180f) newXRotation -= 360f;
            if (newYRotation > 180f) newYRotation -= 360f;
            
            PanOrTilt(newXRotation, newYRotation);
        }

        private void PanOrTilt(float newXRotation, float newYRotation)
        {
            // Normalize angles to prevent wrapping
            newXRotation = NormalizeAngle(newXRotation);
            newYRotation = NormalizeAngle(newYRotation);

            newXRotation = Mathf.Clamp(newXRotation, verticalClampMin, verticalClampMax);
            if (specialViewClamps)
            {
                newYRotation = Mathf.Clamp(newYRotation, horizontalClampMin, horizontalClampMax);
            }

            cam.transform.localEulerAngles = new Vector3(newXRotation, newYRotation, 0);
        }
        
        
        private void ZoomCamera()
        {
            if (!isZoomingEnabled) { return; }

            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                float zoomModifier = CameraZoomSensitivity * PlatformSensitivityModifier;
                Camera cam = Camera.main; 
                
                Zoom = Mathf.Clamp(cam.fieldOfView - (scrollInput * zoomModifier), MinFieldOfView, MaxFieldOfView);
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (scrollInput * zoomModifier), MinFieldOfView, MaxFieldOfView);
            }
        }
        
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                focusGainedFrame = true;
            }
        }

        public void EnablePanAndZoom(bool toEnable)
        {
            isPanningEnabled = toEnable;
            isZoomingEnabled = toEnable;
        }

        public void EnableSpecialViewClamps(Transform specialViewRotation)
        {
            Vector3 referenceEuler = specialViewRotation.eulerAngles;
            if (referenceEuler.x > 180f) referenceEuler.x -= 360f;
            if (referenceEuler.y > 180f) referenceEuler.y -= 360f;
            
            verticalClampMin = referenceEuler.x - (MinFieldOfView / 2f);
            verticalClampMax = referenceEuler.x + (MinFieldOfView / 2f);
            
            if (referenceEuler.y < 0)
            {
                horizontalClampMin = (referenceEuler.y - (StandardClampSize / 2f)) + 180;
                horizontalClampMax = (referenceEuler.y + (StandardClampSize / 2f)) + 180;
            }
            else
            {
                horizontalClampMin = (referenceEuler.y - (StandardClampSize / 2f)) - 180;
                horizontalClampMax = (referenceEuler.y + (StandardClampSize / 2f)) - 180;
            }
            
            specialViewClamps = true; 
        }

        public void DisableSpecialViewClamps()
        {
            specialViewClamps = false;
            verticalClampMin = -StandardClampSize;
            verticalClampMax = StandardClampSize;
        }
        
        private float NormalizeAngle(float angle)
        {
            return Mathf.Repeat(angle + 180f, 360f) - 180f;
        }
    }
}
