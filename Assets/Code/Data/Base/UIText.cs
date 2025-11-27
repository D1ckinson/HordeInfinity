using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.BuffSystem.Base;
using System.Collections.Generic;
using YG;

namespace Assets.Code.Data.Base
{
    public static class UIText
    {
        private const string Russian = "ru";
        private const string English = "en";
        private const string Turkish = "tr";

        private static Dictionary<AbilityType, string> _abilityName;
        private static Dictionary<BuffType, string> _buffName;
        private static Dictionary<BuffType, string> _buffStatDescription;
        private static Dictionary<FloatStatType, string> _floatStatName;
        private static Dictionary<IntStatType, string> _intStatName;
        private static Dictionary<BoolStatType, string> _boolStatName;

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
        public static string Leaders { get; private set; }
        public static string ChoseAbility { get; private set; }

        public static IReadOnlyDictionary<AbilityType, string> AbilityName => _abilityName;
        public static IReadOnlyDictionary<BuffType, string> BuffName => _buffName;
        public static IReadOnlyDictionary<BuffType, string> BuffStatDescription => _buffStatDescription;
        public static IReadOnlyDictionary<FloatStatType, string> FloatStatName => _floatStatName;
        public static IReadOnlyDictionary<IntStatType, string> IntStatName => _intStatName;
        public static IReadOnlyDictionary<BoolStatType, string> BoolStatName => _boolStatName;

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
                    FillEn();
                    break;
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
            Leaders = "Лидеры";
            ChoseAbility = "Выберите способность";
            Tip = "Нанесите достаточно урона, чтобы разблокировать стартовую способность";

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

            _buffName = new()
            {
                [BuffType.Health] = "Усиление Здоровья",
                [BuffType.Regeneration] = "Усиление Регенерации",
                [BuffType.Damage] = "Усиление Урона",
                [BuffType.Cooldown] = "Усиление Перезарядки",
                [BuffType.Speed] = "Усиление Скорости",
                [BuffType.Extraction] = "Усиление Добычи",
                [BuffType.Knowledge] = "Усиление Знания",
                [BuffType.Collection] = "Усиление Сбора",
                [BuffType.Armor] = "Усиление Брони"
            };

            _buffStatDescription = new()
            {
                [BuffType.Health] = "Здоровье",
                [BuffType.Regeneration] = "Регенерация",
                [BuffType.Damage] = "Урон способностей",
                [BuffType.Cooldown] = "Перезарядка способностей",
                [BuffType.Speed] = "Скорость движения",
                [BuffType.Extraction] = "Получаемое золото",
                [BuffType.Knowledge] = "Получаемый опыт",
                [BuffType.Collection] = "Радиус сбора",
                [BuffType.Armor] = "Сопротивление урону"
            };

            _floatStatName = new()
            {
                [FloatStatType.Cooldown] = "Перезарядка",
                [FloatStatType.Damage] = "Урон",
                [FloatStatType.HealthPercent] = "Процент от здоровья",
                [FloatStatType.PullForce] = "Сила притяжения",
                [FloatStatType.Range] = "Дальность",
                [FloatStatType.ThrowDistance] = "Дистанция броска"
            };

            _intStatName = new()
            {
                [IntStatType.ProjectilesCount] = "Количество снарядов",
                [IntStatType.BouncesQuantity] = "Количество отскоков"
            };

            _boolStatName = new()
            {
                [BoolStatType.IsPiercing] = "Пронзание"
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
            Leaders = "Leaders";
            ChoseAbility = "Choose ability";
            Tip = "Deal enough damage to unlock the starting ability";

            _abilityName = new()
            {
                [AbilityType.SwordStrike] = "Sword strike",
                [AbilityType.GhostSwords] = "Ghost swords",
                [AbilityType.HolyGround] = "Holy ground",
                [AbilityType.MidasHand] = "Midas Hand",
                [AbilityType.Bombard] = "Bombard",
                [AbilityType.BlackHole] = "Black hole",
                [AbilityType.StoneSpikes] = "Stone spikes",
                [AbilityType.IceStaff] = "Ice staff",
                [AbilityType.Shuriken] = "Shuriken",
                [AbilityType.Fireball] = "Fireball",
                [AbilityType.WindFlow] = "Wind Flow"
            };

            _buffName = new()
            {
                [BuffType.Health] = "Health Boost",
                [BuffType.Regeneration] = "Regeneration Boost",
                [BuffType.Damage] = "Damage Boost",
                [BuffType.Cooldown] = "Cooldown Boost",
                [BuffType.Speed] = "Speed Boost",
                [BuffType.Extraction] = "Extraction Boost",
                [BuffType.Knowledge] = "Knowledge Boost",
                [BuffType.Collection] = "Collection Boost",
                [BuffType.Armor] = "Armor Boost"
            };

            _buffStatDescription = new()
            {
                [BuffType.Health] = "Health",
                [BuffType.Regeneration] = "Regeneration",
                [BuffType.Damage] = "Ability damage",
                [BuffType.Cooldown] = "Ability cooldown",
                [BuffType.Speed] = "Movement speed",
                [BuffType.Extraction] = "Gold received",
                [BuffType.Knowledge] = "Experience received",
                [BuffType.Collection] = "Collection radius",
                [BuffType.Armor] = "Damage resistance"
            };


            _floatStatName = new()
            {
                [FloatStatType.Cooldown] = "Cooldown",
                [FloatStatType.Damage] = "Damage",
                [FloatStatType.HealthPercent] = "Percent from health",
                [FloatStatType.PullForce] = "Pull force",
                [FloatStatType.Range] = "Range",
                [FloatStatType.ThrowDistance] = "Throw distance"
            };

            _intStatName = new()
            {
                [IntStatType.ProjectilesCount] = "Projectiles count",
                [IntStatType.BouncesQuantity] = "Bounces quantity"
            };

            _boolStatName = new()
            {
                [BoolStatType.IsPiercing] = "Piercing"
            };
        }

        private static void FillTr()
        {
            LevelMax = "Maks";
            LevelCut = "Sev";
            Level = "Seviye";
            Play = "Oyna";
            Shop = "Dükkan";
            Upgrade = "Yükselt";
            Leaderboard = "Lider tablosu";
            Earned = "Kazanılan";
            YourTime = "Süreniz";
            Minutes = "Dakika";
            Continue = "Devam et";
            MenuText = "Menü";
            PersonalBest = "Kişisel rekor";
            Leaders = "Liderler";
            ChoseAbility = "Yeteneği seçin";
            Tip = "Başlangıç yeteneğini açmak için yeterli hasar verin";

            _buffName = new()
            {
                [BuffType.Health] = "Sağlık Güçlendirme",
                [BuffType.Regeneration] = "Yenilenme Güçlendirme",
                [BuffType.Damage] = "Hasar Güçlendirme",
                [BuffType.Cooldown] = "Bekleme Süresi Güçlendirme",
                [BuffType.Speed] = "Hız Güçlendirme",
                [BuffType.Extraction] = "Çıkarma Güçlendirme",
                [BuffType.Knowledge] = "Bilgi Güçlendirme",
                [BuffType.Collection] = "Toplama Güçlendirme",
                [BuffType.Armor] = "Zırh Güçlendirme"
            };

            _buffStatDescription = new()
            {
                [BuffType.Health] = "Sağlık",
                [BuffType.Regeneration] = "Yenilenme",
                [BuffType.Damage] = "Yetenek hasarı",
                [BuffType.Cooldown] = "Yetenek bekleme süresi",
                [BuffType.Speed] = "Hareket hızı",
                [BuffType.Extraction] = "Alınan altın",
                [BuffType.Knowledge] = "Alınan deneyim",
                [BuffType.Collection] = "Toplama yarıçapı",
                [BuffType.Armor] = "Hasar direnci"
            };


            _abilityName = new()
            {
                [AbilityType.SwordStrike] = "Kılıç darbesi",
                [AbilityType.GhostSwords] = "Hayalet kılıçlar",
                [AbilityType.HolyGround] = "Kutsal zemin",
                [AbilityType.MidasHand] = "Midas'ın eli",
                [AbilityType.Bombard] = "Bombardıman",
                [AbilityType.BlackHole] = "Kara delik",
                [AbilityType.StoneSpikes] = "Taş dikenler",
                [AbilityType.IceStaff] = "Buz asası",
                [AbilityType.Shuriken] = "Şuriken",
                [AbilityType.Fireball] = "Ateş topu",
                [AbilityType.WindFlow] = "Rüzgar akışı"
            };

            _floatStatName = new()
            {
                [FloatStatType.Cooldown] = "Bekleme süresi",
                [FloatStatType.Damage] = "Hasar",
                [FloatStatType.HealthPercent] = "Sağlık yüzdesi",
                [FloatStatType.PullForce] = "Çekme gücü",
                [FloatStatType.Range] = "Menzil",
                [FloatStatType.ThrowDistance] = "Atış mesafesi"
            };

            _intStatName = new()
            {
                [IntStatType.ProjectilesCount] = "Mermi sayısı",
                [IntStatType.BouncesQuantity] = "Sekme sayısı"
            };

            _boolStatName = new()
            {
                [BoolStatType.IsPiercing] = "Kazığa geçme"
            };
        }
    }
}
