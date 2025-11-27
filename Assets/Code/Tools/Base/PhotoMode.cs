using UnityEngine;

namespace Assets.Code.Tools.Base
{
    public class PhotoMode : MonoBehaviour
    {
        [Header("Camera Settings")]
        public Camera photoCamera;
        public float moveSpeed = 5f;
        public float mouseSensitivity = 2f;
        public float scrollSpeed = 5f;

        [Header("UI Elements")]
        public GameObject photoModeUI; // Панель с инструкциями

        private bool isPhotoMode = false;
        private Vector3 originalCameraPosition;
        private Quaternion originalCameraRotation;

        // Переменные для управления камерой
        private float rotationX = 0f;
        private float rotationY = 0f;

        void Start()
        {
            // Если камера не назначена, используем основную
            if (photoCamera == null)
                photoCamera = Camera.main;

            // Сохраняем исходное положение камеры
            if (photoCamera != null)
            {
                originalCameraPosition = photoCamera.transform.position;
                originalCameraRotation = photoCamera.transform.rotation;
            }

            // Скрываем UI фоторежима
            if (photoModeUI != null)
                photoModeUI.SetActive(false);
        }

        void Update()
        {
            // Переключение фоторежима по клавише P
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePhotoMode();
            }

            // Если фоторежим активен - управляем камерой
            if (isPhotoMode)
            {
                HandleCameraMovement();
                HandleCameraRotation();
            }
        }

        public void TogglePhotoMode()
        {
            isPhotoMode = !isPhotoMode;

            if (isPhotoMode)
            {
                EnterPhotoMode();
            }
            else
            {
                ExitPhotoMode();
            }
        }

        private void EnterPhotoMode()
        {
            // Ставим игру на паузу
            Time.timeScale = 0f;

            // Сохраняем текущее положение камеры
            if (photoCamera != null)
            {
                originalCameraPosition = photoCamera.transform.position;
                originalCameraRotation = photoCamera.transform.rotation;

                // Сбрасываем вращение
                rotationX = photoCamera.transform.eulerAngles.y;
                rotationY = -photoCamera.transform.eulerAngles.x;
            }

            // Показываем UI фоторежима
            if (photoModeUI != null)
                photoModeUI.SetActive(true);

            // Разблокируем и скрываем курсор
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Photo Mode: ON - Game Paused");
        }

        private void ExitPhotoMode()
        {
            // Возвращаем нормальную скорость игры
            Time.timeScale = 1f;

            // Скрываем UI фоторежима
            if (photoModeUI != null)
                photoModeUI.SetActive(false);

            // Блокируем курсор для геймплея
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("Photo Mode: OFF - Game Resumed");
        }

        private void HandleCameraMovement()
        {
            if (photoCamera == null) return;

            Vector3 moveDirection = Vector3.zero;

            // Движение вперед/назад (W/S)
            if (Input.GetKey(KeyCode.W)) moveDirection += photoCamera.transform.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection -= photoCamera.transform.forward;

            // Движение влево/вправо (A/D)
            if (Input.GetKey(KeyCode.D)) moveDirection += photoCamera.transform.right;
            if (Input.GetKey(KeyCode.A)) moveDirection -= photoCamera.transform.right;

            // Движение вверх/вниз (Q/E)
            if (Input.GetKey(KeyCode.E)) moveDirection += photoCamera.transform.up;
            if (Input.GetKey(KeyCode.Q)) moveDirection -= photoCamera.transform.up;

            // Нормализуем вектор если двигаемся в нескольких направлениях
            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                photoCamera.transform.position += moveDirection * moveSpeed * GetDeltaTime();
            }
        }

        private void HandleCameraRotation()
        {
            if (photoCamera == null) return;

            // Вращение камеры только при зажатой правой кнопке мыши
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                rotationX += mouseX * mouseSensitivity;
                rotationY += mouseY * mouseSensitivity;
                rotationY = Mathf.Clamp(rotationY, -90f, 90f);

                photoCamera.transform.rotation = Quaternion.Euler(-rotationY, rotationX, 0f);

                // Скрываем курсор при вращении камеры
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                // Показываем курсор когда не вращаем камеру
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Зум колесиком мыши
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                photoCamera.transform.Translate(0, 0, scroll * scrollSpeed, Space.Self);
            }
        }

        // Специальный метод для получения deltaTime даже на паузе
        private float GetDeltaTime()
        {
            return Time.unscaledDeltaTime;
        }

        // Метод для кнопки UI
        public void OnPhotoModeButtonClicked()
        {
            TogglePhotoMode();
        }
    }
}