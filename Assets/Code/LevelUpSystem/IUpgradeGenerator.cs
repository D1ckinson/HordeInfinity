using System.Collections.Generic;

namespace Assets.Code.LevelUpSystem
{
    public interface IUpgradeGenerator
    {
        public List<UpgradeOption> Generate(int count);
        public void Reset();
    }
}
