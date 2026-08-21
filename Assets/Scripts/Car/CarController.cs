using Configs;
using InputModule;
using Signals;
using System;
using UnityEngine;
using VContainer;

namespace Vehicle
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour, IDisposable
    {
        [SerializeField] private Rigidbody _carRigidbody;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private Wheel[] _wheels = new Wheel[4];

        private IInputService _input;
        private ICarConfig _config;
        private IEventBus _eventBus;
        private bool _isAnyWheelGrounded = false;
        private float _currentAirSpinSpeed = 0f;
        private bool _isRestarted;
        private bool _isDead;

        [Inject]
        public void Construct(IInputService input, IEventBus eventBus)
        {
            _input = input;
            _eventBus = eventBus;

            _eventBus.Subscribe<LevelFailedSignal>(OnLevelFailed);
        }

        public void InitializeCar(ICarConfig config)
        {
            _config = config;

            for (int i = 0; i < 4; i++)
            {
                _wheels[i].TargetLocalY = _config.MaxWheelLocalY;
                _wheels[i].InitialVisualRotation = _wheels[i].Visual.localRotation;

                float dotProduct = Vector3.Dot(_wheels[i].Pivot.right, transform.right);
                _wheels[i].DirectionMultiplier = dotProduct >= 0f ? 1f : -1f;
                _wheels[i].RotationAngle = 0f;
                _wheels[i].IsGrounded = false;
            }
        }

        private void Update()
        {
            if (_config == null) return;
            if (_isDead) return;

            float moveInput = GetMovementMultiplier();
            UpdateAirSpinSpeed(moveInput);

            float forwardSpeed = Vector3.Dot(_carRigidbody.linearVelocity, transform.forward);
            float groundedAngleDelta = (forwardSpeed / _config.WheelRadius) * Mathf.Rad2Deg * Time.deltaTime;
            float airAngleDelta = _currentAirSpinSpeed * Time.deltaTime;

            for (int i = 0; i < _wheels.Length; i++)
            {
                UpdateWheelVisualPosition(ref _wheels[i]);
                UpdateWheelVisualRotation(ref _wheels[i], moveInput, airAngleDelta, groundedAngleDelta);
            }
        }

        private void FixedUpdate()
        {
            ApplyModifiedGravity();

            if (_config == null) return;
            if (HandleRestartState()) return;
            if (_isDead) return;

            float moveInput = GetMovementMultiplier();

            ApplySpeedLimit();

            int groundedWheelsCount = UpdateWheelsGroundedState();
            _isAnyWheelGrounded = groundedWheelsCount > 0;

            float dynamicMotorForce = CalculateMotorForce(moveInput);
            Vector3 driveDirection = GetDriveDirection();

            for (int i = 0; i < _wheels.Length; i++)
            {
                ApplySuspensionAndMotorForces(i, groundedWheelsCount, moveInput, dynamicMotorForce, driveDirection);
            }

            HandleAirControl(moveInput);
        }

        #region CarPhysics

        private void ApplyModifiedGravity()
        {
            _carRigidbody.AddForce(Vector3.down * _config.CustomGravity, ForceMode.Acceleration);
        }

        private void ApplySpeedLimit()
        {
            float currentZVelocity = _carRigidbody.linearVelocity.z;
            if (Mathf.Abs(currentZVelocity) > _config.MaxSpeed)
            {
                float limitedZ = Mathf.Sign(currentZVelocity) * _config.MaxSpeed;
                _carRigidbody.linearVelocity = new Vector3(_carRigidbody.linearVelocity.x, _carRigidbody.linearVelocity.y, limitedZ);
            }
        }

        private int UpdateWheelsGroundedState()
        {
            float rayOffsetUp = 0.5f;
            float totalRayLength = _config.MaxSuspentionLength + rayOffsetUp;
            bool[] currentFrameGrounded = new bool[4];
            int count = 0;

            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].IsGrounded = false;
            }

            for (int i = 0; i < _wheels.Length; i++)
            {
                Vector3 rayStart = _wheels[i].SuspensionPoint.position + _wheels[i].SuspensionPoint.up * rayOffsetUp;
                currentFrameGrounded[i] = Physics.Raycast(rayStart, -_wheels[i].SuspensionPoint.up, out _wheels[i].Hit, totalRayLength, _groundMask);
            }

            LinkWheelAxis(ref currentFrameGrounded[0], ref currentFrameGrounded[1], 0, 1);
            LinkWheelAxis(ref currentFrameGrounded[2], ref currentFrameGrounded[3], 2, 3);

            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].IsGrounded) count++;
            }

            return count;
        }

        private void ApplySuspensionAndMotorForces(int i, int groundedCount, float moveInput, float motorForce, Vector3 driveDir)
        {
            float rayOffsetUp = 0.5f;
            float totalRayLength = _config.MaxSuspentionLength + rayOffsetUp;
            ref Wheel wheel = ref _wheels[i];

            if (!wheel.IsGrounded)
            {
                wheel.TargetLocalY = _config.MaxWheelLocalY;
                return;
            }

            int partnerIndex = (i % 2 == 0) ? i + 1 : i - 1;
            Vector3 rayStart = wheel.SuspensionPoint.position + wheel.SuspensionPoint.up * rayOffsetUp;

            RaycastHit finalHit = Physics.Raycast(rayStart, -wheel.SuspensionPoint.up, out RaycastHit individualHit, totalRayLength, _groundMask, QueryTriggerInteraction.Ignore)
                ? individualHit
                : _wheels[partnerIndex].Hit;

            float actualDistance = finalHit.distance - rayOffsetUp;
            float offset = _config.MaxSuspentionLength - actualDistance;
            float springForce = offset * _config.SuspentionMultiplier;

            Vector3 velocityAtWheel = _carRigidbody.GetPointVelocity(wheel.SuspensionPoint.position);
            float suspensionVelocity = Vector3.Dot(wheel.SuspensionPoint.up, velocityAtWheel);

            float currentDampSensitivity = !Mathf.Approximately(moveInput, 0f) ? _config.DampSensitivity : _config.DampSensitivity * _config.CoastDampingMultiplier;
            float damperForce = suspensionVelocity * currentDampSensitivity;
            float totalSuspensionForce = springForce - damperForce;
            float compressionRatio = actualDistance / _config.WheelRadius;

            if (compressionRatio < 1.0f)
            {
                totalSuspensionForce += (1.0f - compressionRatio) * _config.BumpStopMultiplier;
                if (suspensionVelocity < 0f)
                {
                    _carRigidbody.AddForceAtPosition(wheel.SuspensionPoint.up * (Mathf.Abs(suspensionVelocity) * _carRigidbody.mass * 0.05f), wheel.SuspensionPoint.position, ForceMode.Impulse);
                }
            }

            totalSuspensionForce = (compressionRatio >= 1.0f) ? Mathf.Clamp(totalSuspensionForce, 0f, _config.MaxAntiCapapultForce) : Mathf.Max(0f, totalSuspensionForce);
            _carRigidbody.AddForceAtPosition(wheel.SuspensionPoint.up * totalSuspensionForce, wheel.SuspensionPoint.position - Vector3.up);

            if (!Mathf.Approximately(moveInput, 0f) && groundedCount > 0)
            {
                float appliedForce = moveInput * (motorForce / groundedCount);
                Vector3 wheelCenter = finalHit.point + Vector3.up * _config.WheelRadius;
                _carRigidbody.AddForceAtPosition(driveDir * appliedForce, wheelCenter);
            }

            float localTargetY = -(actualDistance - _config.WheelRadius);
            wheel.TargetLocalY = Mathf.Clamp(localTargetY, _config.MaxWheelLocalY, _config.MinWheelLocalY);
        }

        private void HandleAirControl(float moveInput)
        {
            if (_isAnyWheelGrounded) return;

            if (!Mathf.Approximately(moveInput, 0f))
            {
                _carRigidbody.AddRelativeTorque(Vector3.right * (moveInput * _config.AirPitchForce));
            }
            else
            {
                Vector3 localAngularVel = transform.InverseTransformDirection(_carRigidbody.angularVelocity);
                localAngularVel.x *= Mathf.Clamp01(1f - _config.AirAngularDamping * Time.fixedDeltaTime); _carRigidbody.angularVelocity = transform.TransformDirection(localAngularVel);
            }
        }

        #endregion

        #region CarVisual

        private void UpdateAirSpinSpeed(float moveInput)
        {
            float targetAirSpeed = moveInput * _config.AirSpinSpeed;
            float accelRate = Mathf.Approximately(moveInput, 0f) ? _config.WheelAirBrakeSpeed : _config.AirSpinSpeed * 5f;
            _currentAirSpinSpeed = Mathf.MoveTowards(_currentAirSpinSpeed, targetAirSpeed, accelRate * Time.deltaTime);
        }

        private void UpdateWheelVisualPosition(ref Wheel wheel)
        {
            float currentSmoothSpeed = wheel.Pivot.localPosition.y < wheel.TargetLocalY
                ? _config.WheelSmoothSpeed * 2f
                : _config.WheelSmoothSpeed;

            Vector3 localPos = wheel.Pivot.localPosition;
            localPos.y = Mathf.MoveTowards(localPos.y, wheel.TargetLocalY, currentSmoothSpeed * Time.deltaTime);
            wheel.Pivot.localPosition = localPos;
        }

        private void UpdateWheelVisualRotation(ref Wheel wheel, float moveInput, float airAngleDelta, float groundedAngleDelta)
        {
            float finalAngleDelta = groundedAngleDelta;

            if (wheel.IsGrounded && !Mathf.Approximately(moveInput, 0f))
            {
                finalAngleDelta = (Mathf.Abs(airAngleDelta) > Mathf.Abs(groundedAngleDelta) || Mathf.Sign(airAngleDelta) != Mathf.Sign(groundedAngleDelta))
                    ? airAngleDelta
                    : groundedAngleDelta;
            }
            else if (!wheel.IsGrounded)
            {
                finalAngleDelta = airAngleDelta;
            }

            wheel.RotationAngle = (wheel.RotationAngle + finalAngleDelta * wheel.DirectionMultiplier) % 360f;
            wheel.Visual.localRotation = wheel.InitialVisualRotation * Quaternion.Euler(wheel.RotationAngle, 0f, 0f);
        }

        #endregion

        public void ResetSuspension(Transform resetPoint)
        {
            _isDead = false;

            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].Visual.parent != _wheels[i].Pivot)
                {
                    _wheels[i].Visual.SetParent(_wheels[i].Pivot);
                    _wheels[i].Visual.localPosition = Vector3.zero;
                }

                if (_wheels[i].WheelRigidbody != null) _wheels[i].WheelRigidbody.isKinematic = true;
                if (_wheels[i].WheelCollider != null) _wheels[i].WheelCollider.enabled = false;

                _wheels[i].TargetLocalY = _config.MaxWheelLocalY;
                _wheels[i].IsGrounded = false;
                _wheels[i].Hit = default;
            }

            _currentAirSpinSpeed = 0f;
            _carRigidbody.linearVelocity = Vector3.zero;
            _carRigidbody.angularVelocity = Vector3.zero;
            _carRigidbody.ResetInertiaTensor();
            _carRigidbody.isKinematic = true;
            _carRigidbody.position = resetPoint.position;
            _carRigidbody.rotation = resetPoint.rotation;
            _carRigidbody.Sleep();
            _isRestarted = true;
        }

        private void OnLevelFailed(LevelFailedSignal signal)
        {
            if (_isDead) return;
            _isDead = true;

            DetachWheels();
        }

        private void DetachWheels()
        {
            Vector3 carVelocity = _carRigidbody.linearVelocity;

            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].WheelRigidbody == null || _wheels[i].WheelCollider == null) continue;

                _wheels[i].Visual.SetParent(null);
                _wheels[i].WheelCollider.enabled = true;
                _wheels[i].WheelRigidbody.isKinematic = false;
                _wheels[i].WheelRigidbody.linearVelocity = carVelocity;

                Vector3 explosionExplosionDirection = (_wheels[i].Visual.position - transform.position).normalized;
                explosionExplosionDirection.y += 0.5f;

                _wheels[i].WheelRigidbody.AddForce(explosionExplosionDirection * 5f, ForceMode.Impulse);
                _wheels[i].WheelRigidbody.AddTorque(UnityEngine.Random.onUnitSphere * 10f, ForceMode.Impulse);
            }
        }

        private float GetMovementMultiplier()
        {
            return _input.MoveDirection switch
            {
                DirectionValue.Left => -1f,
                DirectionValue.Right => 1f,
                DirectionValue.None => 0f,
                _ => 0f
            };
        }      

        private void LinkWheelAxis(ref bool w1Grounded, ref bool w2Grounded, int idx1, int idx2)
        {
            if (w1Grounded || w2Grounded)
            {
                w1Grounded = w2Grounded = true;
                _wheels[idx1].IsGrounded = _wheels[idx2].IsGrounded = true;
            }
        }

        private float CalculateMotorForce(float moveInput)
        {
            if (Mathf.Approximately(moveInput, 0f)) return 0f;

            float currentZVelocity = Mathf.Abs(_carRigidbody.linearVelocity.z);
            float speedProgress = Mathf.Clamp01(currentZVelocity / _config.MaxSpeed);
            float quadEaseInOut = speedProgress < 0.5f ? 2f * speedProgress * speedProgress : 1f - Mathf.Pow(-2f * speedProgress + 2f, 2f) / 2f;

            return (_config.MotorForce + _config.MotorKickForce) * (1f - quadEaseInOut);
        }

        private Vector3 GetDriveDirection()
        {
            Vector3 driveDir = transform.forward; driveDir.x = 0f;

            return driveDir.normalized;
        }

        private bool HandleRestartState()
        {
            if (!_isRestarted) return false;

            _carRigidbody.isKinematic = false;
            _isRestarted = false;

            return true;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<LevelFailedSignal>(OnLevelFailed);
        }
    }
}
