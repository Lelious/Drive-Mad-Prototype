using UnityEngine;

namespace Vehicle
{
    [System.Serializable]
    public struct Wheel
    {
        public Transform Pivot;
        public Transform Visual;
        public Transform SuspensionPoint;

        [Header("Physics (Detachable)")]
        public Rigidbody WheelRigidbody;
        public Collider WheelCollider;

        [HideInInspector] public float TargetLocalY;
        [HideInInspector] public float RotationAngle;
        [HideInInspector] public Quaternion InitialVisualRotation;
        [HideInInspector] public float DirectionMultiplier;
        [HideInInspector] public bool IsGrounded;
        [HideInInspector] public RaycastHit Hit;
    }
}
