using Assets.Scripts;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private string _playerDataJson;
        private PlayerData _playerData;

        public void Save()
        {
            _playerDataJson = JsonUtility.ToJson(_playerData);
            YG2.SaveProgress();
        }

        public PlayerData Load()
        {
            _playerData = JsonUtility.FromJson<PlayerData>(_playerDataJson) ?? new();

            return _playerData;
        }
    }
}
