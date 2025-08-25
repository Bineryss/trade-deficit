using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSCamera
{
    [RequireComponent(typeof(PlayerInput))]
    public class RTSCameraInputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float mouseWheelZoomSpeedMultiplier = 5f;

        public Vector2 MoveInput { get; private set; }
        public Vector2 RotateInput { get; private set; }
        public float ZoomInput { get; private set; }

        private RTSCameraInputs inputActions;
        private bool isGamePad;
        private bool middleClickInput;

        public void Initialize(RTSCameraConfiguration configuration)
        {
            mouseWheelZoomSpeedMultiplier = configuration.MouseWheelZoomSpeedMultiplier;
        }

        void OnEnable()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            inputActions ??= new RTSCameraInputs();
            inputActions.Enable();

            inputActions.camera.move.performed += OnMove;
            inputActions.camera.move.canceled += OnMove;

            inputActions.camera.look.performed += OnRotate;
            inputActions.camera.look.canceled += OnRotate;

            inputActions.camera.togglerotate.started += OnToggleRotate;
            inputActions.camera.togglerotate.canceled += OnToggleRotate;

            inputActions.camera.zoom.performed += OnZoom;
            inputActions.camera.zoom.canceled += OnZoom;

            playerInput.onControlsChanged += HandleControlsChanged;
        }

        void OnDisable()
        {
            inputActions.camera.move.performed -= OnMove;
            inputActions.camera.move.canceled -= OnMove;

            inputActions.camera.look.performed -= OnRotate;
            inputActions.camera.look.canceled -= OnRotate;

            inputActions.camera.togglerotate.started -= OnToggleRotate;
            inputActions.camera.togglerotate.canceled -= OnToggleRotate;

            inputActions.camera.zoom.performed -= OnZoom;
            inputActions.camera.zoom.canceled -= OnZoom;

            playerInput.onControlsChanged -= HandleControlsChanged;
        }

        private void HandleControlsChanged(PlayerInput input)
        {
            isGamePad = inputActions.GamepadScheme.name.Equals(input.currentControlScheme);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }
        private void OnRotate(InputAction.CallbackContext context)
        {
            if (!isGamePad && !middleClickInput)
            {
                RotateInput = Vector2.zero;
                return;
            }
            RotateInput = context.ReadValue<Vector2>();
        }
        private void OnZoom(InputAction.CallbackContext context)
        {
            if (isGamePad)
            {
                ZoomInput = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
            }
            else
            {
                ZoomInput = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f) * mouseWheelZoomSpeedMultiplier;
            }
        }
        private void OnToggleRotate(InputAction.CallbackContext context)
        {
            middleClickInput = context.ReadValueAsButton();
        }

    }
}