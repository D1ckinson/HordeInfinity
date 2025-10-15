using System;
using System.Collections.Generic;
using YG;

namespace Assets.Code.Data
{
    public static class UIText
    {
        private const string Russian = "ru";
        private const string English = "en";
        private const string Turkish = "tr";

        private static Dictionary<AbilityType, string> _abilityName;

        public static string LevelMax { get; private set; }
        public static string LevelCut { get; private set; }
        public static string Level { get; private set; }
        public static string Play { get; private set; }
        public static string Shop { get; private set; }
        public static string Upgrade { get; private set; }
        public static string Leaderboard { get; private set; }
        public static string Earned { get; private set; }
        public static string YourTime { get; private set; }
        public static string Minutes { get; private set; }
        public static string Continue { get; private set; }
        public static string MenuText { get; private set; }
        public static string PersonalBest { get; private set; }
        public static string Tip { get; private set; }

        public static string Range { get; private set; }
        public static string Damage { get; private set; }
        public static string Cooldown { get; private set; }
        public static string ProjectilesCount { get; private set; }
        public static string HealthPercent { get; private set; }
        public static string PullForce { get; private set; }
        public static string BouncesQuantity { get; private set; }
        public static string ThrowDistance { get; private set; }

        public static IReadOnlyDictionary<AbilityType, string> AbilityName => _abilityName;

        static UIText()
        {
            switch (YG2.envir.language)
            {
                case Russian:
                    FillRu();
                    break;
                case English:
                    FillEn();
                    break;
                case Turkish:
                    FillTr();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void FillRu()
        {
            LevelMax = "Макс";
            LevelCut = "Ур";
            Level = "Уровень";
            Play = "Играть";
            Shop = "Магазин";
            Upgrade = "Улучшить";
            Leaderboard = "Таблица лидеров";
            Earned = "Заработано";
            YourTime = "Ваше время";
            Minutes = "Минут";
            Continue = "Продолжить";
            MenuText = "Меню";
            PersonalBest = "Личный рекорд";
            Tip = "Нанесите достаточно урона, чтобы разблокировать стартовую способность";

            Damage = "Урон";
            Cooldown = "Перезарядка";
            Range = "Дальность";
            ProjectilesCount = "Количество снарядов";
            HealthPercent = "Процент от здоровья";
            PullForce = "Сила притяжения";
            BouncesQuantity = "Количество отскоков";
            ThrowDistance = "Дистанция броска";

            _abilityName = new()
            {
                [AbilityType.SwordStrike] = "Удар мечом",
                [AbilityType.GhostSwords] = "Призрачные мечи",
                [AbilityType.HolyGround] = "Святая земля",
                [AbilityType.MidasHand] = "Рука Мидаса",
                [AbilityType.Bombard] = "Бомбарда",
                [AbilityType.BlackHole] = "Черная дыра",
                [AbilityType.StoneSpikes] = "Каменные иглы",
                [AbilityType.IceStaff] = "Ледяной посох",
                [AbilityType.Shuriken] = "Сюрикен",
                [AbilityType.Fireball] = "Огненный шар",
                [AbilityType.WindFlow] = "Поток ветра"
            };
        }

        private static void FillEn()
        {
            LevelMax = "Max";
            LevelCut = "Lvl";
            Level = "Level";
            Play = "Play";
            Shop = "Shop";
            Upgrade = "Upgrade";
            Leaderboard = "Leaderboard";
            Earned = "Earned";
            YourTime = "Your time";
            Minutes = "Minutes";
            Continue = "Continue";
            MenuText = "Menu";
            PersonalBest = "Personal best";
            Tip = "Deal enough damage to unlock the starting ability";

            Damage = "Damage";
            Cooldown = "Cooldown";
            Range = "Range";
            ProjectilesCount = "Projectiles count";
            HealthPercent = "Percent from health";
            PullForce = "Pull force";
            BouncesQuantity = "Bounces quantity";
            ThrowDistance = "Throw distance";

            _abilityName = new()
            {
                [AbilityType.SwordStrike] = "Sword strike",
                [AbilityType.GhostSwords] = "Ghost swords",
                [AbilityType.HolyGround] = "Holy ground",
                [AbilityType.MidasHand] = "Midas Hand",
                [AbilityType.MidasHand] = "Bombard",
                [AbilityType.BlackHole] = "Black hole",
                [AbilityType.StoneSpikes] = "Stone spikes",
                [AbilityType.IceStaff] = "Ice staff",
                [AbilityType.Shuriken] = "Shuriken",
                [AbilityType.Fireball] = "Fireball",
                [AbilityType.WindFlow] = "Wind Flow"
            };
        }

        private static void FillTr()
        {

        }
    }
}
