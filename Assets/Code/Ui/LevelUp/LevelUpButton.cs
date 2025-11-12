using Assets.Code.Tools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.LevelUp
{
    public class LevelUpButton : UiButton
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text[] _stats;

        [field: SerializeField] public TMP_Text LevelText { get; private set; }
        [field: SerializeField] public TMP_Text LevelNumber { get; private set; }

        public void SetDescription(string name, Sprite image, List<string> stats)
        {
            _stats.ForEach(text => text.SetActive(false));

            _name.text = name.ThrowIfNullOrEmpty();
            _image.sprite = image.ThrowIfNull();
            stats.ThrowIfNullOrEmpty().Count.ThrowIfMoreThan(_stats.Length);
            _stats.ForEach(text => text.text = string.Empty);

            for (int i = Constants.Zero; i < stats.Count; i++)
            {
                TMP_Text text = _stats[i];

                text.SetText(stats[i]);
                text.SetActive(true);
            }
        }
    }
}
