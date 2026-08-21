using Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Configs/Car Config")]
    public class CarConfig : ScriptableObject, ICarConfig
    {
        [Header("Assets")]
        public AssetReferenceGameObject CarPrefabReference;

        [Header("Suspension Settings")]
        [SerializeField] private float _maxSuspentionLength = 2.0f;
        [SerializeField] private float _suspentionMultiplier = 35000f;
        [SerializeField] private float _dampSensitivity = 2500f;
        [SerializeField] private float _wheelRadius = 0.4f;
        [SerializeField] private float _maxAntiCapapultForce = 50000f;
        [SerializeField] private float _customGravity = 25f;

        [Header("Hard Bump Stop")]
        [SerializeField] private float _bumpStopMultiplier = 150000f;

        [Header("Dumping")]
        [Range(0f, 1f)]
        [SerializeField] private float _coastDampingMultiplier = 0.2f;

        [Header("Wheel Limits (Local Y inside Suspension)")]
        [SerializeField] private float _minWheelLocalY = -0.1f;
        [SerializeField] private float _maxWheelLocalY = -1.5f;
        [SerializeField] private float _wheelSmoothSpeed = 15f;

        [Header("Movement Settings")]
        [SerializeField] private float _motorForce = 15000f;
        [SerializeField] private float _motorKickForce = 25000f;
        [SerializeField] private float _maxSpeed = 25f;

        [Header("Air Control")]
        [SerializeField] private float _airPitchForce = 8000f;
        [SerializeField] private float _airAngularDamping = 2f;
        [SerializeField] private float _airSpinSpeed = 1200f;
        [SerializeField] private float _wheelAirBrakeSpeed = 800f;

        public float MaxSuspentionLength => _maxSuspentionLength;
        public float SuspentionMultiplier => _suspentionMultiplier;
        public float DampSensitivity => _dampSensitivity;
        public float WheelRadius => _wheelRadius;
        public float MaxAntiCapapultForce => _maxAntiCapapultForce;
        public float CustomGravity => _customGravity;
        public float BumpStopMultiplier => _bumpStopMultiplier;
        public float CoastDampingMultiplier => _coastDampingMultiplier;
        public float MinWheelLocalY => _minWheelLocalY;
        public float MaxWheelLocalY => _maxWheelLocalY;
        public float WheelSmoothSpeed => _wheelSmoothSpeed;
        public float MotorForce => _motorForce;
        public float MotorKickForce => _motorKickForce;
        public float MaxSpeed => _maxSpeed;
        public float AirPitchForce => _airPitchForce;
        public float AirAngularDamping => _airAngularDamping;
        public float AirSpinSpeed => _airSpinSpeed;
        public float WheelAirBrakeSpeed => _wheelAirBrakeSpeed;
    }
}
