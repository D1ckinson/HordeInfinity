using Assets.Code.Tools.Base;
using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.BuffView
{
    public class BuffView : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private SerializedDictionary<BuffType, Sprite> _sprites;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            Vector3 lookPoint = new(transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
            transform.LookAt(lookPoint);
        }

        public void AddBuff(BuffType type, float time)
        {
            time.ThrowIfNegative();
            type.ThrowIfNull();

            Image icon = _icons.FirstOrDefault(icon => icon.IsActive() == false);

            if (icon.IsNull())
            {
                return;
            }

            icon.sprite = _sprites[type];
            icon.fillAmount = Constants.One;
            icon.SetActive(true);

            TimerService.StartTimerWithUpdate(time, () => icon.SetActive(false), progress => icon.fillAmount = Constants.One - progress, this);
        }

        public void Clear()
        {
            _icons.ForEach(icon => icon.SetActive(false));
            TimerService.StopAllTimersForOwner(this);
        }
    }
}
