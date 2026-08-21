using InputModule;
using Levels;
using Signals;
using System;
using VContainer;
using Vehicle;

namespace Services
{
    public class GameplayService : IDisposable
    {
        private IInputService _inputService;
        private IEventBus _eventBus;
        private CarController _currentCar;
        private GameLevel _currentLevel;
        private bool _isGameEnded;

        [Inject]
        public GameplayService(IEventBus eventBus, IInputService inputService)
        {
            _eventBus = eventBus;
            _inputService = inputService;

            _eventBus.Subscribe<LevelFinishedSignal>(OnCompleteLevel);
            _eventBus.Subscribe<LevelFailedSignal>(OnLevelFail);
            _eventBus.Subscribe<LevelReloadSignal>(OnLevelReload);
        }

        public void Initialize(GameLevel level, CarController car)
        {
            _currentLevel = level;
            _currentCar = car;
        }

        private void Restart()
        {
            if (_currentCar == null)
            {
                return;
            }

            _currentCar.ResetSuspension(_currentLevel.GetSpawnPoint());
            _currentLevel.RestartLevel();
            _isGameEnded = false;
            _inputService.IsLocked = false;
        }

        private void OnCompleteLevel(LevelFinishedSignal signal)
        {
            if (_isGameEnded) return;

            _isGameEnded = true;
            _inputService.IsLocked = true;
        }

        private void OnLevelFail(LevelFailedSignal signal)
        {
            if (_isGameEnded) return;

            _isGameEnded = true;
            _inputService.IsLocked = true;
        }

        private void OnLevelReload(LevelReloadSignal signal)
        {
            _inputService.IsLocked = true;

            Restart();
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<LevelFinishedSignal>(OnCompleteLevel);
            _eventBus.Unsubscribe<LevelFailedSignal>(OnLevelFail);
            _eventBus.Unsubscribe<LevelReloadSignal>(OnLevelReload);
        }
    }
}
