using Signals;
using UnityEngine;
using VContainer;
using Vehicle;

namespace Levels
{
    [RequireComponent(typeof(Collider))]
    public class FinishZone : MonoBehaviour
    {
        private IEventBus _eventBus;
        private bool _levelCompleted;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void ResetZone()
        {
            _levelCompleted = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_levelCompleted && other.TryGetComponent(out CarController _))
            {
                _levelCompleted = true;
                _eventBus.Push(new LevelFinishedSignal());
            }
        }
    }
}
