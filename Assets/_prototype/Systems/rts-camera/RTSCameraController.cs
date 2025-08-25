using Unity.Cinemachine;
using UnityEngine;

namespace RTSCamera
{
    [RequireComponent(typeof(RTSCameraInputProvider))]
    public class RTSCameraController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform trackingPoint;
        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
        [SerializeField] private Camera playerCamera;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 20f;
        [SerializeField] private float watchMergeDistance = 5f;
        [SerializeField] private Transform watchTarget;
        private Vector3 velocity = Vector3.zero;
        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 20f;
        [SerializeField] private float zoomSmoothing = 5f;
        [SerializeField] private AnimationCurve zoomSpeedCurve;
        private float currentZoomSpeed;
        [Header("Orbit")]
        [SerializeField] private float orbitSensitivity = 0.5f;
        [SerializeField] private float orbitSmoothing = 5f;
        private Vector2 orbitVelocity = Vector2.zero;
        [Header("Input")]
        [SerializeField] private RTSCameraInputProvider inputReader;

        public void Initialize(RTSCameraConfiguration configuration)
        {
            playerCamera = configuration.PlayerCamera;
            moveSpeed = configuration.MoveSpeed;
            acceleration = configuration.Acceleration;
            deceleration = configuration.Deceleration;
            watchMergeDistance = configuration.WatchMergeDistance;
            zoomSpeed = configuration.ZoomSpeed;
            zoomSmoothing = configuration.ZoomSmoothing;
            orbitSensitivity = configuration.OrbitSensitivity;
            orbitSmoothing = configuration.OrbitSmoothing;

            if (inputReader == null)
            {
                inputReader = GetComponent<RTSCameraInputProvider>();
            }

            inputReader.Initialize(configuration);
        }
        public void SetTarget(Transform target)
        {
            watchTarget = target;
        }
        void OnEnable()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }
        void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            UpdateMovement(deltaTime, inputReader.MoveInput);
            UpdateOrbit(deltaTime, inputReader.RotateInput);
            UpdateZoom(deltaTime, inputReader.ZoomInput);
        }
        private void UpdateOrbit(float deltaTime, Vector2 rotateInput)
        {
            orbitVelocity = Vector2.MoveTowards(orbitVelocity, orbitSensitivity * rotateInput, deltaTime * orbitSmoothing);

            InputAxis horizontalAxis = orbitalFollow.HorizontalAxis;
            InputAxis verticalAxis = orbitalFollow.VerticalAxis;

            horizontalAxis.Value += orbitVelocity.x;
            verticalAxis.Value -= orbitVelocity.y;

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
                return wrapped - Mathf.Abs(axis.Range.x);
            }


            return Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);
        }

        private void UpdateMovement(float deltaTime, Vector2 moveInput)
        {
            if (watchTarget != null && moveInput.sqrMagnitude < 0.1f)
            {
                if (Vector3.Distance(trackingPoint.position, watchTarget.position) < watchMergeDistance) //TODO imporve follow logic
                {
                    trackingPoint.position = watchTarget.position;
                    return;
                }
                trackingPoint.position = Vector3.Lerp(trackingPoint.position, watchTarget.position, Time.deltaTime);
            }

            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            right.Normalize();


            float zoomFactorNormalized = Mathf.InverseLerp(orbitalFollow.RadialAxis.Range.x, orbitalFollow.RadialAxis.Range.y, orbitalFollow.RadialAxis.Value);
            float zoomSpeedMultiplier = Mathf.Lerp(0.1f, 1f, zoomFactorNormalized);
            Vector3 targetVelocity = moveSpeed * zoomSpeedMultiplier * new Vector3(moveInput.x, 0, moveInput.y);

            if (moveInput.sqrMagnitude > 0.1f)
            {
                watchTarget = null;
                velocity = Vector3.Lerp(velocity, targetVelocity, acceleration * deltaTime);
            }
            else
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * deltaTime);
            }

            Vector3 motion = velocity * deltaTime;
            trackingPoint.position += forward * motion.z + right * motion.x;
        }
        private void UpdateZoom(float deltaTime, float zoomInput)
        {
            InputAxis axis = orbitalFollow.RadialAxis;

            float targetZoomSpeed = 0;
            float zoomMultiplier = zoomSpeedCurve.Evaluate(Mathf.InverseLerp(axis.Range.x, axis.Range.y, axis.Value)) * zoomSpeed;

            if (Mathf.Abs(zoomInput) >= 0.01f)
            {
                targetZoomSpeed = zoomInput * zoomMultiplier;
            }
            currentZoomSpeed = Mathf.Lerp(currentZoomSpeed, targetZoomSpeed, zoomSmoothing * deltaTime);

            axis.Value -= currentZoomSpeed * deltaTime;
            axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);

            orbitalFollow.RadialAxis = axis;
        }
    }
}
