using UnityEngine;

namespace RTSCamera
{
    public struct RTSCameraConfiguration
    {
        public Camera PlayerCamera;

        public float MoveSpeed;
        public float Acceleration;
        public float Deceleration;
        public float WatchMergeDistance;
        public float ZoomSpeed;
        public float ZoomSmoothing;
        public float MouseWheelZoomSpeedMultiplier;
        public float OrbitSensitivity;
        public float OrbitSmoothing;
    }
}