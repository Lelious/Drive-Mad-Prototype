using Signals;
using System;
using UnityEngine;
using VContainer;

namespace Vehicle
{
    public class CarFlipChecker : MonoBehaviour, IDisposable
    {
        [SerializeField] private float _maxFlipAngle = 75f;
        [SerializeField] private LayerMask _groundMask;

        private IEventBus _eventBus;
        private bool _isLevelEnded;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<LevelFinishedSignal>(OnLevelEnded);
            _eventBus.Subscribe<LevelFailedSignal>(OnLevelEnded);
            _eventBus.Subscribe<LevelReloadSignal>(OnLevelReset);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isLevelEnded) return;

            if ((_groundMask.value & (1 << other.gameObject.layer)) != 0)
            {
                float currentAngle = Vector3.Angle(transform.up, Vector3.up);

                if (currentAngle > _maxFlipAngle)
                {
                    _isLevelEnded = true;
                    _eventBus.Push(new LevelFailedSignal());
                }
            }
        }

        private void OnLevelEnded<T>(T signal) where T : struct
        {
            _isLevelEnded = true;
        }

        private void OnLevelReset(LevelReloadSignal signal)
        {
            _isLevelEnded = false;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<LevelFinishedSignal>(OnLevelEnded);
            _eventBus.Unsubscribe<LevelFailedSignal>(OnLevelEnded);
            _eventBus.Unsubscribe<LevelReloadSignal>(OnLevelReset);
        }
    }
}
