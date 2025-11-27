using UnityEngine;

namespace Assets.Code.Ui.Base
{
    public class UiCanvas : MonoBehaviour
    {
        [field: SerializeField] public RectTransform DeathWindowPoint { get; private set; }
        [field: SerializeField] public RectTransform Container { get; private set; }
        [field: SerializeField] public RectTransform FadeContainer { get; private set; }
    }
}
