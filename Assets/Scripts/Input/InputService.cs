using Signals;
using System;
using UnityEngine;
using VContainer;

namespace InputModule
{
    public class InputService : IInputService, IDisposable
    {
        public bool IsLocked { get; set; }

        private GameInputAction _controls;
        private DirectionValue _uiMoveValue;
        private IEventBus _eventBus;

        [Inject]
        public InputService(IEventBus eventBus)
        {
            _controls = new GameInputAction();
            _controls.Gameplay.Enable();
            _eventBus = eventBus;
            _controls.Gameplay.Reload.performed += context => Reload();
        }

        public DirectionValue MoveDirection
        {
            get
            {
                if (IsLocked) return DirectionValue.None;

                float keyboardValue = _controls.Gameplay.Drive.ReadValue<float>();

                if (!Mathf.Approximately(keyboardValue, 0f))
                {
                    return keyboardValue > 0f ? DirectionValue.Right : DirectionValue.Left;
                }

                return _uiMoveValue;
            }
        }

        public void SetUiMoveValue(DirectionValue value)
        {
            _uiMoveValue = value;
        }

        public void Reload()
        {
            _eventBus.Push(new LevelReloadSignal());
        }

        public void Dispose()
        {
            _controls.Gameplay.Reload.performed -= context => Reload();
            _controls?.Gameplay.Disable();
            _controls?.Dispose();
        }
    }
}
