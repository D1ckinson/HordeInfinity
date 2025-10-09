using Assets.Code.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class ShopOption : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text AbilityName { get; private set; }
        [field: SerializeField] public TMP_Text UpgradeText { get; private set; }
        [field: SerializeField] public TMP_Text Cost { get; private set; }
        [field: SerializeField] public TMP_Text LevelText { get; private set; }
        [field: SerializeField] public TMP_Text LevelNumber { get; private set; }
        [field: SerializeField] public TMP_Text LevelMaxText { get; private set; }
        [field: SerializeField] public Image AbilityIcon { get; private set; }
        [field: SerializeField] public RectTransform OfferDescription { get; private set; }
        [field: SerializeField] public Button UpgradeButton { get; private set; }

        public AbilityType AbilityType { get; private set; }

        public ShopOption Initialize(AbilityType abilityType)
        {
            AbilityType = abilityType;

            AbilityName.SetText(UIText.AbilityName[abilityType]);
            UpgradeText.SetText(UIText.Upgrade);
            LevelText.SetText(UIText.LevelCut);
            LevelMaxText.SetText(UIText.LevelMax);

            return this;
        }
    }
}
