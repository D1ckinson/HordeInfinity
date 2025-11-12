using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//https://i.rusvideos.art/rolik/bratec-nakazal-svodnuyu-sestru-oralno-i-zastavil-proglotit-lipkuyu-spermu.html
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
