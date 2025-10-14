using Assets.Code.InputActions;
using Assets.Code.Tools;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Movement
{
    public class InputReader : IInputService
    {
        private readonly InputControls _inputControls;
        private readonly Joystick _joystick;
        private readonly ITimeService _timeService;

        private Vector2 _previousDirection;

        public InputReader(Joystick joystick, ITimeService timeService)
        {
            _inputControls = new InputControls();
            _joystick = joystick.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _joystick.SetActive(false);

            _inputControls.Player.Move.performed += OnMovePerformed;
            _inputControls.Player.Move.canceled += OnMoveCanceled;
            _inputControls.Ui.Back.performed += OnPausePerformed;

            _joystick.DirectionChanged += OnJoystickMove;
            _timeService.TimeChanging += ToggleJoystick;
        }

        ~InputReader()
        {
            _joystick.DirectionChanged -= OnJoystickMove;
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

        private void ToggleJoystick(bool isStopping)
        {
            _joystick.SetActive(isStopping);
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            BackPressed?.Invoke();
        }

        public void Enable()
        {
            _inputControls.Enable();
            _joystick.SetActive(true);
        }

        public void Disable()
        {
            _inputControls.Disable();
            _joystick.SetActive(false);
        }
    }
}
