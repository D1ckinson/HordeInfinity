using Assets.Code.Data.Base;
using TMPro;
using UnityEngine;

namespace Assets.Code.Ui.Windows
{
    public class LevelUpCanvas : MonoBehaviour
    {
        [SerializeField] private TMP_Text _choseAbilityText;

        private void Awake()
        {
            _choseAbilityText.SetText(UIText.ChoseAbility);
        }
    }
}
