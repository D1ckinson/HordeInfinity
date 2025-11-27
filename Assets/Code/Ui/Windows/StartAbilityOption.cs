using Assets.Code.AbilitySystem.Base;
using Assets.Code.Data.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class StartAbilityOption : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text AbilityName { get; private set; }
        [field: SerializeField] public TMP_Text ProgressText { get; private set; }
        [field: SerializeField] public TMP_Text GoalText { get; private set; }
        [field: SerializeField] public Image AbilityIcon { get; private set; }
        [field: SerializeField] public Image BarFilling { get; private set; }
        [field: SerializeField] public RectTransform LockIcon { get; private set; }
        [field: SerializeField] public RectTransform CheckMarkIcon { get; private set; }
        [field: SerializeField] public RectTransform Progress { get; private set; }
        [field: SerializeField] public Button ChoseButton { get; private set; }

        public AbilityType AbilityType { get; private set; }

        public StartAbilityOption Initialize(AbilityType abilityType)
        {
            AbilityType = abilityType;
            AbilityName.SetText(UIText.AbilityName[abilityType]);

            return this;
        }
    }
}
