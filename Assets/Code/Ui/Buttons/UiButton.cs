using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Code.Ui
{
    public class UiButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public void Subscribe(UnityAction call)
        {
            _button.onClick.AddListener(call);
        }

        public void UnsubscribeAll()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}
