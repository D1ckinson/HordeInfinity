using Assets.Code.Tools;
using Assets.Scripts;
using Newtonsoft.Json;

namespace YG
{
    public partial class SavesYG
    {
        public string PlayerDataJson;

        public void Save(PlayerData playerData)
        {
            PlayerDataJson = JsonConvert.SerializeObject(playerData);
            YG2.SaveProgress();
        }

        public PlayerData Load()
        {
            return PlayerDataJson.IsNull() ? new() : JsonConvert.DeserializeObject<PlayerData>(PlayerDataJson);
        }
    }
}
