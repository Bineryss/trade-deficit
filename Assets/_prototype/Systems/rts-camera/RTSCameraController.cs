using System;
using RTSCamera;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RTSCameraController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform trackingPoint;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 20f;
    private Vector3 velocity = Vector3.zero;
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private AnimationCurve zoomSpeedCurve;
    [Header("Orbit")]
    [SerializeField] private float orbitSensitivity = 0.5f;
    [SerializeField] private float orbitSmoothing = 5f;

    private RTSCameraInputs inputActions;
    private Vector2 moveInput;
    [SerializeField] private Vector2 rotateInput;
    [SerializeField] private bool middleClickInput;
    private float zoomInput;
    [SerializeField] private bool isGamePad;


    void OnEnable()
    {
        inputActions.camera.move.performed += OnMove;
        inputActions.camera.move.canceled += OnMove;

        inputActions.camera.look.performed += OnRotate;
        inputActions.camera.look.canceled += OnRotate;

        inputActions.camera.togglerotate.started += ctx => middleClickInput = true;
        inputActions.camera.togglerotate.canceled += ctx => middleClickInput = false;

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

        inputActions.camera.togglerotate.started -= ctx => middleClickInput = true;
        inputActions.camera.togglerotate.canceled -= ctx => middleClickInput = false;

        inputActions.camera.zoom.performed -= OnZoom;
        inputActions.camera.zoom.canceled -= OnZoom;

        playerInput.onControlsChanged -= HandleControlsChanged;
    }

    void Awake()
    {
        inputActions = new RTSCameraInputs();
        inputActions.Enable();
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        UpdateMovement(deltaTime);
        UpdateOrbit(deltaTime);
        UpdateZoom(deltaTime);
    }

    private void HandleControlsChanged(PlayerInput input)
    {
        isGamePad = inputActions.GamepadScheme.name.Equals(input.currentControlScheme);
    }
    private void UpdateOrbit(float deltaTime)
    {
        if (!isGamePad && !middleClickInput) return;

        Vector2 orbitInput = orbitSensitivity * rotateInput;

        InputAxis horizontalAxis = orbitalFollow.HorizontalAxis;
        InputAxis verticalAxis = orbitalFollow.VerticalAxis;

        horizontalAxis.Value = Mathf.Lerp(horizontalAxis.Value, horizontalAxis.Value + orbitInput.x, deltaTime * orbitSmoothing);
        verticalAxis.Value = Mathf.Lerp(verticalAxis.Value, verticalAxis.Value - orbitInput.y, deltaTime * orbitSmoothing);

        horizontalAxis.Value = ShiftValueInBounds(horizontalAxis);
        verticalAxis.Value = ShiftValueInBounds(verticalAxis);

        orbitalFollow.HorizontalAxis = horizontalAxis;
        orbitalFollow.VerticalAxis = verticalAxis;
    }

    private float ShiftValueInBounds(InputAxis axis)
    {
        if (axis.Wrap)
        {
            float normalized = axis.Value + Mathf.Abs(axis.Range.x);
            float wrapped = Mathf.Repeat(normalized, Mathf.Abs(axis.Range.x - axis.Range.y));
            return wrapped - Math.Abs(axis.Range.x);
        }


        return Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();


        Vector3 targetVelocity = moveSpeed * new Vector3(moveInput.x, 0, moveInput.y);

        if (moveInput.sqrMagnitude > 0.1f)
        {
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * deltaTime);
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * deltaTime);
        }

        Vector3 motion = velocity * deltaTime;
        trackingPoint.position += forward * motion.z + right * motion.x;
    }
    private void UpdateZoom(float deltaTime)
    {
        InputAxis axis = orbitalFollow.RadialAxis;
        axis.Value -= zoomSpeed * zoomInput;
        axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);

        orbitalFollow.RadialAxis = axis;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnRotate(InputAction.CallbackContext context)
    {
        rotateInput = context.ReadValue<Vector2>();
    }
    private void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }
}
