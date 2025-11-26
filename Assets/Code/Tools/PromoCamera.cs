using UnityEngine;

namespace Assets.Code.Tools
{
    public class PromoCamera : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private PromoCameraMode _mode;
        [SerializeField] private Transform _target;
        [SerializeField, Min(0f)] private float _smoothTime = 0.3f;

        [Header("Rotate Mode")]
        [SerializeField, Min(0f)] private float _rotateSpeed = 30f;
        [SerializeField, Min(0.1f)] private float _rotateRadius = 5f;
        [SerializeField] private float _rotateHeight = 2f;

        [Header("MoveIn Mode")]
        [SerializeField, Min(0f)] private float _moveInSpeed = 1f;
        [SerializeField, Min(0.1f)] private float _targetDistance = 2f;
        [SerializeField] private Vector3 _startPosition = new(0, 5, -10);
        [SerializeField] private Vector3 _startRotation = new(30, 0, 0);

        [Header("FollowView Mode")]
        [SerializeField] private Vector3 _followViewOffset = new(0, 2, -5);

        [Header("Follow Mode")]
        [SerializeField] private Vector3 _followOffset = Vector3.zero;
        [SerializeField] private Vector3 _followRotation = Vector3.zero;

        private Vector3 _currentVelocity;
        private float _currentRotationAngle;
        private bool _isMovingToTarget = true;
        private Vector3 _currentMoveInPosition;
        private Quaternion _currentMoveInRotation;

        private void Start()
        {
            InitializeMoveInMode();
        }

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

        private void InitializeMoveInMode()
        {
            _currentMoveInPosition = _startPosition;
            _currentMoveInRotation = Quaternion.Euler(_startRotation);
            transform.position = _currentMoveInPosition;
            transform.rotation = _currentMoveInRotation;
        }

        private void UpdateMoveInMode()
        {
            if (_isMovingToTarget)
            {
                // Двигаемся к цели
                Vector3 directionToTarget = (_target.position - transform.position).normalized;
                Vector3 targetPosition = _target.position - directionToTarget * _targetDistance;

                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    _moveInSpeed * Time.deltaTime);

                // Плавный поворот к цели
                Quaternion targetRotation = Quaternion.LookRotation(_target.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                    _rotateSpeed * Time.deltaTime);

                // Проверяем достигли ли цели
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    _isMovingToTarget = false;
                }
            }
            else
            {
                // Возвращаемся к стартовой позиции
                transform.position = Vector3.MoveTowards(transform.position, _startPosition,
                    _moveInSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(_startRotation), _rotateSpeed * Time.deltaTime);

                // Проверяем достигли ли стартовой позиции
                if (Vector3.Distance(transform.position, _startPosition) < 0.1f)
                {
                    _isMovingToTarget = true;
                }
            }
        }

        private void UpdateFollowViewMode()
        {
            // В этом режиме позиция камеры не меняется автоматически
            // Только поворот в сторону цели
            Vector3 directionToTarget = _target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                _smoothTime * Time.deltaTime);
        }

        private void UpdateFollowMode()
        {
            // Камера следует за целью как дочерний объект
            Vector3 targetPosition = _target.position + _target.TransformDirection(_followOffset);
            Quaternion targetRotation = _target.rotation * Quaternion.Euler(_followRotation);

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,
                ref _currentVelocity, _smoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                _smoothTime * Time.deltaTime);
        }

        // Public methods for runtime control
        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
            _currentVelocity = Vector3.zero;

            if (_mode == PromoCameraMode.MoveIn)
            {
                InitializeMoveInMode();
            }
        }

        public void SetMode(PromoCameraMode newMode)
        {
            _mode = newMode;
            _currentVelocity = Vector3.zero;

            if (_mode == PromoCameraMode.MoveIn)
            {
                InitializeMoveInMode();
                _isMovingToTarget = true;
            }
        }

        public void SetRotateParameters(float speed, float radius, float height)
        {
            _rotateSpeed = speed;
            _rotateRadius = radius;
            _rotateHeight = height;
        }

        public void SetMoveInParameters(float speed, float distance, Vector3 startPosition, Vector3 startRotation)
        {
            _moveInSpeed = speed;
            _targetDistance = distance;
            _startPosition = startPosition;
            _startRotation = startRotation;
            InitializeMoveInMode();
        }

        public void SetFollowViewOffset(Vector3 offset)
        {
            _followViewOffset = offset;
        }

        public void SetFollowParameters(Vector3 offset, Vector3 rotation)
        {
            _followOffset = offset;
            _followRotation = rotation;
        }

        public void SetSmoothTime(float smoothTime)
        {
            _smoothTime = smoothTime;
        }

        // Для принудительного сброса MoveIn режима
        public void ResetMoveInMode()
        {
            if (_mode == PromoCameraMode.MoveIn)
            {
                InitializeMoveInMode();
                _isMovingToTarget = true;
            }
        }
    }
}