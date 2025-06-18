using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    //this class controls the on screen camera controls
    public class CameraControls : MonoBehaviour
    {
        private VisualElement root;
        private Slider zoomSlider;
        private Button upBtn, downBtn, leftBtn, rightBtn;
        [SerializeField] private float btnSensitivity = 4;
        private float sensitivityMultiplier = 4;
        [SerializeField] private UIManager UIManager;
        [SerializeField] private MovementHandler movementHandler;
        [SerializeField] private TruckMovementHandler truckMovementHandler;
        
        private const float SliderMax = 1f, SliderMin = 0.5f; 
        
        public void Start()
        {
            //initalize components
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            upBtn = root.Q<Button>("buttonUp");
            downBtn = root.Q<Button>("buttonDown");
            leftBtn = root.Q<Button>("buttonLeft");
            rightBtn = root.Q<Button>("buttonRight");
            zoomSlider = root.Q<Slider>("FillSlider");

            //up and down buttons, clamps are for clamping camera within min/max of boundaries
            upBtn.clicked += () => {
                if (!truckMovementHandler.IsWalking)
                {
                    movementHandler.Tilt -= btnSensitivity;
                }
            };

            downBtn.clicked += () => {
                if (!truckMovementHandler.IsWalking)
                {
                    movementHandler.Tilt += btnSensitivity;
                }
            };

            //left and right buttons, clamps are for clamping camera within min/max of boundaries
            //if not in special view, remove boundaries for horizontal
            leftBtn.clicked += () => {
                if (!truckMovementHandler.IsWalking)
                {
                    movementHandler.Pan -= btnSensitivity;
                }
            };

            rightBtn.clicked += () => {
                if (!truckMovementHandler.IsWalking)
                {
                    movementHandler.Pan += btnSensitivity;
                }
            };

            //change zoom based on slider if not moving and over UI component
            zoomSlider.RegisterValueChangedCallback(e =>
            {
                float newFOV = Mathf.Lerp(movementHandler.MinFieldOfView, movementHandler.MaxFieldOfView, Mathf.InverseLerp(SliderMin, SliderMax, e.newValue));
                
                if (!truckMovementHandler.IsWalking && UIManager.OverUIComponent)
                {
                    movementHandler.Zoom = newFOV;
                } 
            });
        }

        //update zoomSlider to the zoom of the camera manager
        //this is for using mouse wheel to zoom in and out
        private void Update()
        {
            float sliderValue = Mathf.Lerp(SliderMin, SliderMax, Mathf.InverseLerp(movementHandler.MinFieldOfView, movementHandler.MaxFieldOfView, movementHandler.Zoom));
            zoomSlider.value = sliderValue;
            btnSensitivity = movementHandler.Sensitivity * sensitivityMultiplier;
        }
    }
}