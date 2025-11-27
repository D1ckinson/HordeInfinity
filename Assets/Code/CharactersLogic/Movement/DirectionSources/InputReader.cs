using Assets.Code.Core;
using Assets.Code.InputActions;
using Assets.Code.Tools.Base;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Code.CharactersLogic.Movement.DirectionSources
{
    public class InputReader : IInputService
    {
        private readonly InputControls _inputControls;
        private readonly Canvas _joystickCanvas;
        private readonly ITimeService _timeService;

        private Vector2 _previousDirection;

        public InputReader(Canvas joystickCanvas, ITimeService timeService)
        {
            _inputControls = new InputControls();
            _joystickCanvas = joystickCanvas.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _joystickCanvas.SetActive(false);

            _inputControls.Player.Move.performed += OnMovePerformed;
            _inputControls.Player.Move.canceled += OnMoveCanceled;
            _inputControls.Ui.Back.performed += OnPausePerformed;

            _joystickCanvas.GetComponentInChildrenOrThrow<Joystick>().DirectionChanged += OnJoystickMove;
            _timeService.TimeChanged += ToggleJoystick;
            _timeService.TimeChanging += ToggleJoystick;
        }

        ~InputReader()
        {
            _joystickCanvas.GetComponentInChildrenOrThrow<Joystick>().DirectionChanged -= OnJoystickMove;
            _timeService.TimeChanged -= ToggleJoystick;
            _timeService.TimeChanging -= ToggleJoystick;

            _inputControls.Player.Move.performed -= OnMovePerformed;
            _inputControls.Player.Move.canceled -= OnMoveCanceled;
            _inputControls.Ui.Back.performed -= OnPausePerformed;
            _inputControls.Disable();
            _inputControls.Dispose();
        }

        public event Action<Vector3> DirectionChanged;
        public event Action BackPressed;

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            if (direction.Compare(_previousDirection, Constants.CompareAccuracy))
            {
                return;
            }

            _previousDirection = direction;
            DirectionChanged?.Invoke(new Vector3(direction.x, Constants.Zero, direction.y));
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _previousDirection = Vector2.zero;
            DirectionChanged?.Invoke(_previousDirection);
        }

        private void OnJoystickMove(Vector2 vector)
        {
            DirectionChanged?.Invoke(new Vector3(vector.x, Constants.Zero, vector.y));
        }

        private void ToggleJoystick()
        {
            bool IsTimeStop = _timeService.TimeScale == Constants.Zero;

            _joystickCanvas.SetActive(IsTimeStop == false);
        }

        private void ToggleJoystick(bool isStopping)
        {
            if (isStopping)
            {
                _joystickCanvas.SetActive(isStopping);
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            BackPressed?.Invoke();
        }

        public void Enable()
        {
            _inputControls.Enable();
            _joystickCanvas.SetActive(true);
        }

        public void Disable()
        {
            _inputControls.Disable();
            _joystickCanvas.SetActive(false);
        }
    }
}
