using GW2Helper.Stuff.CharacterStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Main
    {
        public event EventHandler<string> CharStatusUpdate;
        public int TotalLoaded = 0, SleepTimeInMS = 3000, SingularSleepTimeInMS = 1000, MultipleSleepCheck = 100, MultiPullLimit = 200;

        public List<string> ObjectTypes = new List<string>();
        public List<CharacterListObject> SavedCharacters = new List<CharacterListObject>();

        public List<Achievement> Achievements = new List<Achievement>();
        public List<BackstoryAnswer> BackstoryAnswers = new List<BackstoryAnswer>();
        public List<BackstoryQuestion> BackstoryQuestions = new List<BackstoryQuestion>();
        public List<Colour> Colours = new List<Colour>();
        public List<Emblem> GuildEmblemBackgrounds = new List<Emblem>();
        public List<Emblem> GuildEmblemForegrounds = new List<Emblem>();
        public List<GuildUpgrade> GuildUpgrades = new List<GuildUpgrade>();
        public List<Item> Items = new List<Item>();
        public List<ItemStat> ItemStats = new List<ItemStat>();
        public List<Mastery> Masteries = new List<Mastery>();
        public List<MiniPet> Minis = new List<MiniPet>();
        public List<Recipe> Recipes = new List<Recipe>();
        public List<Skill> Skills = new List<Skill>();
        public List<Skin> Skins = new List<Skin>();
        public List<Title> Titles = new List<Title>();

        public List<KeyValuePair<string, string>> JSON = new List<KeyValuePair<string, string>>();
        public List<string> ReadyToExportJSON = new List<string>();

        public virtual void OnCharStatusUpdate(string e)
        {
            CharStatusUpdate?.Invoke(this, e);
        }

        public void FinalizeImport(Main main)
        {
            for (int i = 0; i < main.Achievements.Count; i++)
            {
                double cur = i, max = main.Achievements.Count;
                Achievement achievement = main.Achievements[i];
                if (achievement.PrerequisiteID.Count > 0)
                {
                    for (int a = 0; a < achievement.PrerequisiteID.Count; a++)
                    {
                        int achiID = achievement.PrerequisiteID[a];
                        achievement.Prerequisites.Add(main.Achievements.FirstOrDefault(ac => ac.ID == achiID));
                    }
                }
                if (achievement.Rewards.Count > 0)
                {
                    for (int a = 0; a < achievement.Rewards.Count; a++)
                    {
                        if (achievement.Rewards[a].RewardType == Reward.Type.Item)
                        {
                            ItemReward reward = (ItemReward)achievement.Rewards[a];
                            int itemID = reward.ItemID;
                            reward.RewardItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            achievement.Rewards[a] = reward;
                        }
                        if (achievement.Rewards[a].RewardType == Reward.Type.Title)
                        {
                            TitleReward reward = (TitleReward)achievement.Rewards[a];
                            int titleID = reward.TitleID;
                            reward.RewardTitle = main.Titles.FirstOrDefault(it => it.ID == titleID);
                            achievement.Rewards[a] = reward;
                        }
                    }
                }
                if (achievement.Bits.Count > 0)
                {
                    for (int a = 0; a < achievement.Bits.Count; a++)
                    {
                        if (achievement.Bits[a].BitType == Bit.Type.Item)
                        {
                            ItemBit bit = (ItemBit)achievement.Bits[a];
                            int itemID = bit.ItemID;
                            bit.BitItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            achievement.Bits[a] = bit;
                        }
                        if (achievement.Bits[a].BitType == Bit.Type.Minipet)
                        {
                            MinipetBit bit = (MinipetBit)achievement.Bits[a];
                            int miniID = bit.MiniID;
                            bit.BitMini = main.Minis.FirstOrDefault(mi => mi.ID == miniID);
                            achievement.Bits[a] = bit;
                        }
                        if (achievement.Bits[a].BitType == Bit.Type.Skin)
                        {
                            SkinBit bit = (SkinBit)achievement.Bits[a];
                            int skinID = bit.SkinID;
                            bit.BitSkin = main.Skins.FirstOrDefault(sk => sk.ID == skinID);
                            achievement.Bits[a] = bit;
                        }
                    }
                }
                main.OnCharStatusUpdate("Finalizing Achievements;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.BackstoryAnswers.Count; i++)
            {
                double cur = i, max = main.BackstoryAnswers.Count;
                int questionID = main.BackstoryAnswers[i].QuestionID;
                main.BackstoryAnswers[i].Question = main.BackstoryQuestions.FirstOrDefault(bs => bs.ID == questionID);
                main.OnCharStatusUpdate("Finalizing Backstory Answers;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.BackstoryQuestions.Count; i++)
            {
                double cur = i, max = main.BackstoryQuestions.Count;
                for (int a = 0; a < main.BackstoryQuestions[i].AnswerID.Count; a++)
                {
                    string answerID = main.BackstoryQuestions[i].AnswerID[a];
                    main.BackstoryQuestions[i].Answers.Add(main.BackstoryAnswers.FirstOrDefault(bs => bs.ID == answerID));
                }
                main.OnCharStatusUpdate("Finalizing Backstory Questions;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.GuildUpgrades.Count; i++)
            {
                double cur = i, max = main.GuildUpgrades.Count;
                if (main.GuildUpgrades[i].Costs.Count > 0)
                {
                    for (int a = 0; a < main.GuildUpgrades[i].Costs.Count; a++)
                    {
                        if (main.GuildUpgrades[i].Costs[a].ItemID.HasValue)
                        {
                            int itemID = main.GuildUpgrades[i].Costs[a].ItemID.Value;
                            main.GuildUpgrades[i].Costs[a].RequiredItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                        }
                    }
                }
                if (main.GuildUpgrades[i].PrerequisiteID.Count > 0)
                {
                    for (int a = 0; a < main.GuildUpgrades[i].PrerequisiteID.Count; a++)
                    {
                        int uID = main.GuildUpgrades[i].PrerequisiteID[a];
                        main.GuildUpgrades[i].Prerequisites.Add(main.GuildUpgrades.FirstOrDefault(gu => gu.ID == uID));
                    }
                }
                main.OnCharStatusUpdate("Finalizing Guild Upgrades;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.Items.Count; i++)
            {
                double cur = i, max = main.Items.Count;
                if (main.Items[i].SkinID.HasValue)
                {
                    int skinID = main.Items[i].SkinID.Value;
                    main.Items[i].ItemSkin = main.Skins.FirstOrDefault(sk => sk.ID == skinID);
                }

                if (main.Items[i].ItemType == Item.Type.Armor)
                {
                    ArmorItemDetail detail = (ArmorItemDetail)main.Items[i].Details;
                    if (detail.InfixUpgrades != null)
                    {
                        if (detail.InfixUpgrades.Buff != null)
                        {
                            int skillID = detail.InfixUpgrades.Buff.SkillID.Value;
                            detail.InfixUpgrades.Buff.BuffSkill = main.Skills.FirstOrDefault(sk => sk.ID == skillID);
                        }
                    }
                    if (detail.Infusions.Count > 0)
                    {
                        for (int a = 0; a < detail.Infusions.Count; a++)
                        {
                            if (detail.Infusions[a].ItemID.HasValue)
                            {
                                int itemID = detail.Infusions[a].ItemID.Value;
                                detail.Infusions[a].ExistingItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            }
                        }
                    }
                    if (detail.ItemID.HasValue)
                    {
                        int itemID = detail.ItemID.Value;
                        detail.SuffixItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                    }
                    main.Items[i].Details = detail;
                }
                if (main.Items[i].ItemType == Item.Type.Back)
                {
                    BackItemDetail detail = (BackItemDetail)main.Items[i].Details;
                    if (detail.InfixUpgrades != null)
                    {
                        if (detail.InfixUpgrades.Buff != null)
                        {
                            int skillID = detail.InfixUpgrades.Buff.SkillID.Value;
                            detail.InfixUpgrades.Buff.BuffSkill = main.Skills.FirstOrDefault(sk => sk.ID == skillID);
                        }
                    }
                    if (detail.Infusions.Count > 0)
                    {
                        for (int a = 0; a < detail.Infusions.Count; a++)
                        {
                            if (detail.Infusions[a].ItemID.HasValue)
                            {
                                int itemID = detail.Infusions[a].ItemID.Value;
                                detail.Infusions[a].ExistingItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            }
                        }
                    }
                    if (detail.ItemID.HasValue)
                    {
                        int itemID = detail.ItemID.Value;
                        detail.SuffixItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                    }
                    main.Items[i].Details = detail;
                }
                if (main.Items[i].ItemType == Item.Type.Consumable)
                {
                    ConsumableItemDetail detail = (ConsumableItemDetail)main.Items[i].Details;
                    if (detail.RecipeID.HasValue)
                    {
                        int recipeID = detail.RecipeID.Value;
                        detail.ConsumableRecipe = main.Recipes.FirstOrDefault(re => re.ID == recipeID);
                    }
                    for (int a = 0; a < detail.SkinID.Count; a++)
                    {
                        int skinID = detail.SkinID[a];
                        detail.Skins.Add(main.Skins.FirstOrDefault(sk => sk.ID == skinID));
                    }
                    main.Items[i].Details = detail;
                }
                if (main.Items[i].ItemType == Item.Type.MiniPet)
                {
                    MiniItemDetail detail = (MiniItemDetail)main.Items[i].Details;
                    if (detail.MiniID.HasValue)
                    {
                        int miniID = detail.MiniID.Value;
                        detail.Mini = main.Minis.FirstOrDefault(mi => mi.ID == miniID);
                    }
                    main.Items[i].Details = detail;
                }
                if (main.Items[i].ItemType == Item.Type.Trinket)
                {
                    TrinketItemDetail detail = (TrinketItemDetail)main.Items[i].Details;
                    if (detail.InfixUpgrades != null)
                    {
                        if (detail.InfixUpgrades.Buff != null)
                        {
                            int skillID = detail.InfixUpgrades.Buff.SkillID.Value;
                            detail.InfixUpgrades.Buff.BuffSkill = main.Skills.FirstOrDefault(sk => sk.ID == skillID);
                        }
                    }
                    if (detail.Infusions.Count > 0)
                    {
                        for (int a = 0; a < detail.Infusions.Count; a++)
                        {
                            if (detail.Infusions[a].ItemID.HasValue)
                            {
                                int itemID = detail.Infusions[a].ItemID.Value;
                                detail.Infusions[a].ExistingItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            }
                        }
                    }
                    if (detail.ItemID.HasValue)
                    {
                        int itemID = detail.ItemID.Value;
                        detail.SuffixItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                    }
                    main.Items[i].Details = detail;
                }
                if (main.Items[i].ItemType == Item.Type.Weapon)
                {
                    WeaponItemDetail detail = (WeaponItemDetail)main.Items[i].Details;
                    if (detail.InfixUpgrades != null)
                    {
                        if (detail.InfixUpgrades.Buff != null)
                        {
                            int skillID = detail.InfixUpgrades.Buff.SkillID.Value;
                            detail.InfixUpgrades.Buff.BuffSkill = main.Skills.FirstOrDefault(sk => sk.ID == skillID);
                        }
                    }
                    if (detail.Infusions.Count > 0)
                    {
                        for (int a = 0; a < detail.Infusions.Count; a++)
                        {
                            if (detail.Infusions[a].ItemID.HasValue)
                            {
                                int itemID = detail.Infusions[a].ItemID.Value;
                                detail.Infusions[a].ExistingItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                            }
                        }
                    }
                    if (detail.ItemID.HasValue)
                    {
                        int itemID = detail.ItemID.Value;
                        detail.SuffixItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                    }
                    main.Items[i].Details = detail;
                }
                main.OnCharStatusUpdate("Finalizing Items;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.Minis.Count; i++)
            {
                double cur = i, max = main.Minis.Count;
                int itemID = main.Minis[i].ItemID;
                main.Minis[i].UnlockItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                main.OnCharStatusUpdate("Finalizing Minipets;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.Recipes.Count; i++)
            {
                double cur = i, max = main.Recipes.Count;
                if (main.Recipes[i].ItemID.HasValue)
                {
                    int itemID = main.Recipes[i].ItemID.Value;
                    main.Recipes[i].ResultItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                }
                if (main.Recipes[i].UpgradeID.HasValue)
                {
                    int uID = main.Recipes[i].UpgradeID.Value;
                    main.Recipes[i].ResultUpgrade = main.GuildUpgrades.FirstOrDefault(gu => gu.ID == uID);
                }
                if (main.Recipes[i].Ingredients.Count > 0)
                {
                    for (int a = 0; a < main.Recipes[i].Ingredients.Count; a++)
                    {
                        int itemID = main.Recipes[i].Ingredients[a].ItemID;
                        main.Recipes[i].Ingredients[a].IngredientItem = main.Items.FirstOrDefault(it => it.ID == itemID);
                    }
                }
                if (main.Recipes[i].GuildIngredients.Count > 0)
                {
                    for (int a = 0; a < main.Recipes[i].GuildIngredients.Count; a++)
                    {
                        int uID = main.Recipes[i].GuildIngredients[a].UpgradeID;
                        main.Recipes[i].GuildIngredients[a].IngredientUpgrade = main.GuildUpgrades.FirstOrDefault(gu => gu.ID == uID);
                    }
                }
                main.OnCharStatusUpdate("Finalizing Recipes;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.Skills.Count; i++)
            {
                double cur = i, max = main.Skills.Count;
                for (int a = 0; a < main.Skills[i].TransformSkillID.Count; a++)
                {
                    int skillID = main.Skills[i].TransformSkillID[a];
                    main.Skills[i].TransformSkills.Add(main.Skills.FirstOrDefault(sk => sk.ID == skillID));
                }
                for (int a = 0; a < main.Skills[i].BundleSkillID.Count; a++)
                {
                    int skillID = main.Skills[i].BundleSkillID[a];
                    main.Skills[i].BundleSkills.Add(main.Skills.FirstOrDefault(sk => sk.ID == skillID));
                }
                if (main.Skills[i].ToolbeltSkillID.HasValue)
                {
                    int skillID = main.Skills[i].ToolbeltSkillID.Value;
                    main.Skills[i].ToolbeltSkill = main.Skills.FirstOrDefault(sk => sk.ID == skillID);
                }
                main.OnCharStatusUpdate("Finalizing Skills;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
            for (int i = 0; i < main.Titles.Count; i++)
            {
                double cur = i, max = main.Titles.Count;
                for (int a = 0; a < main.Titles[i].AchievementID.Count; a++)
                {
                    int achieveID = main.Titles[i].AchievementID[a];
                    main.Titles[i].Achievements.Add(main.Achievements.FirstOrDefault(ac => ac.ID == achieveID));
                }
                main.OnCharStatusUpdate("Finalizing Titles;" + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
}
