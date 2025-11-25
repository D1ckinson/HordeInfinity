using UnityEngine;

namespace Assets.Code.Tools
{
    public class PromoCamera : MonoBehaviour
    {
        [SerializeField] private PromoCameraMode _mode;
        [SerializeField] private Transform _target;

        // Rotate mode parameters
        [SerializeField] private float _rotateSpeed = 30f;
        [SerializeField] private float _rotateRadius = 5f;
        [SerializeField] private float _rotateHeight = 2f;

        // MoveIn mode parameters
        [SerializeField] private float _moveInSpeed = 1f;
        [SerializeField] private float _targetDistance = 2f;

        // FollowView mode parameters
        [SerializeField] private Vector3 _followViewOffset = new(0, 2, -5);

        // Общие параметры
        [SerializeField] private float _smoothTime = 0.3f;

        private Vector3 _currentVelocity;
        private float _currentRotationAngle;

        private void LateUpdate()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
                return;

            switch (_mode)
            {
                case PromoCameraMode.Rotate:
                    UpdateRotateMode();
                    break;
                case PromoCameraMode.MoveIn:
                    UpdateMoveInMode();
                    break;
                case PromoCameraMode.FollowView:
                    UpdateFollowViewMode();
                    break;
                case PromoCameraMode.Follow:
                    UpdateFollowMode();
                    break;
            }
        }

        private void UpdateRotateMode()
        {
            _currentRotationAngle += _rotateSpeed * Time.deltaTime;

            // Вычисляем позицию на окружности
            float x = Mathf.Cos(_currentRotationAngle * Mathf.Deg2Rad) * _rotateRadius;
            float z = Mathf.Sin(_currentRotationAngle * Mathf.Deg2Rad) * _rotateRadius;

            Vector3 targetPosition = _target.position + new Vector3(x, _rotateHeight, z);

            // Плавное перемещение
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
                ref _currentVelocity, _smoothTime);

            // Смотрим на цель
            transform.LookAt(_target.position + _rotateHeight * 0.2f * Vector3.up);
        }

        private void UpdateMoveInMode()
        {
            // Вычисляем позицию на заданном расстоянии от цели
            Vector3 directionToTarget = (_target.position - transform.position).normalized;
            Vector3 targetPosition = _target.position - directionToTarget * _targetDistance;

            // Плавное приближение
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
                ref _currentVelocity, _smoothTime * 2f);

            // Всегда смотрим на цель
            transform.LookAt(_target.position);
        }

        private void UpdateFollowViewMode()
        {
            // Позиция с offset относительно цели
            Vector3 targetPosition = _target.position + _followViewOffset;

            // Плавное перемещение
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
                ref _currentVelocity, _smoothTime);

            // Смотрим на цель
            transform.LookAt(_target.position);
        }

        private void UpdateFollowMode()
        {
            // В этом режиме просто следуем за целью как дочерний объект
            // но с плавностью для избежания резких движений
            Vector3 targetPosition = _target.position;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
                ref _currentVelocity, _smoothTime);

            // Можно добавить плавное вращение если нужно
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation,
                _smoothTime * Time.deltaTime);
        }

        // Public methods for runtime control
        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
            _currentVelocity = Vector3.zero; // Сбрасываем скорость для плавного перехода
        }

        public void SetMode(PromoCameraMode newMode)
        {
            _mode = newMode;
            _currentVelocity = Vector3.zero; // Сбрасываем скорость при смене режима
        }

        public void SetRotateParameters(float speed, float radius, float height)
        {
            _rotateSpeed = speed;
            _rotateRadius = radius;
            _rotateHeight = height;
        }

        public void SetMoveInParameters(float speed, float distance)
        {
            _moveInSpeed = speed;
            _targetDistance = distance;
        }

        public void SetFollowViewOffset(Vector3 offset)
        {
            _followViewOffset = offset;
        }

        public void SetSmoothTime(float smoothTime)
        {
            _smoothTime = smoothTime;
        }

        // Для отладки в редакторе
        private void OnValidate()
        {
            // Гарантируем положительные значения для критических параметров
            _rotateSpeed = Mathf.Max(0, _rotateSpeed);
            _rotateRadius = Mathf.Max(0.1f, _rotateRadius);
            _moveInSpeed = Mathf.Max(0, _moveInSpeed);
            _targetDistance = Mathf.Max(0.1f, _targetDistance);
            _smoothTime = Mathf.Max(0, _smoothTime);
        }
    }
}