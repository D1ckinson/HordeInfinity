using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.Ui.Windows
{
    public class BaseWindow : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        protected void Disable()
        {
            this.SetActive(false);
        }
    }
}
