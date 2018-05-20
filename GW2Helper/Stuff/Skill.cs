using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Skill
    {
        public enum Type
        {
            Bundle,
            Elite,
            Heal,
            Profession,
            Utility,
            Weapon,
            Toolbelt,
            Monster,
            Transform,
            Pet
        }
        public enum SkillSlot
        {
            Downed_1,
            Downed_2,
            Downed_3,
            Downed_4,
            Pet,
            Profession_1,
            Profession_2,
            Profession_3,
            Profession_4,
            Profession_5,
            Utility,
            Weapon_1,
            Weapon_2,
            Weapon_3,
            Weapon_4,
            Weapon_5,
            Heal,
            Elite,
            Toolbelt,
            Transform_1,
            Transform_2,
            Transform_3,
            Transform_4,
            Transform_5
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Type SkillType { get; set; }
        public string WeaponType { get; set; }
        public List<Character.Profession> Professions { get; set; }
        public SkillSlot Slot { get; set; }
        public List<SkillFact> Facts { get; set; }
        public List<SkillFact> TraitedFacts { get; set; }
        public List<string> Categories { get; set; }
        public string Attunement { get; set; }
        public int? Cost { get; set; }
        public string DualWieldRequirement { get; set; }
        public int? FlipTo { get; set; }
        public int? Initiative { get; set; }
        public int? NextChain { get; set; }
        public int? PrevChain { get; set; }
        public List<Skill> TransformSkills { get; set; }
        public List<int> TransformSkillID { get; set; }
        public List<Skill> BundleSkills { get; set; }
        public List<int> BundleSkillID { get; set; }
        public Skill ToolbeltSkill { get; set; }
        public int? ToolbeltSkillID { get; set; }

        public static void GetSkillsFromJSON(string json, Main main)
        {
            SkillRAW[] rawSkills = new SkillRAW[1];
            try
            {
                rawSkills = JsonConvert.DeserializeObject<SkillRAW[]>(json);
            }
            catch (Exception e)
            {
                rawSkills[0] = JsonConvert.DeserializeObject<SkillRAW>(json);
            }
            for (int a = 0; a < rawSkills.Length; a++)
            {
                double cur = a, max = rawSkills.Length;
                SkillRAW skillRAW = rawSkills[a];
                main.JSON.Add(new KeyValuePair<string, string>("Skill", JsonConvert.SerializeObject(skillRAW)));
                Skill newSkill = new Skill
                {
                    ID = skillRAW.id,
                    Name = skillRAW.name,
                    Description = skillRAW.description,
                    Professions = new List<Character.Profession>(),
                    Facts = new List<SkillFact>(),
                    TraitedFacts = new List<SkillFact>(),
                    Categories = new List<string>(),
                    Cost = (skillRAW.cost.HasValue) ? skillRAW.cost.Value : 0,
                    FlipTo = (skillRAW.flip_skill.HasValue) ? skillRAW.flip_skill.Value : 0,
                    Initiative = (skillRAW.initiative.HasValue) ? skillRAW.initiative.Value : 0,
                    NextChain = (skillRAW.next_chain.HasValue) ? skillRAW.next_chain.Value : 0,
                    PrevChain = (skillRAW.prev_chain.HasValue) ? skillRAW.prev_chain.Value : 0,
                    TransformSkills = new List<Skill>(),
                    TransformSkillID = new List<int>(),
                    BundleSkills = new List<Skill>(),
                    BundleSkillID = new List<int>()
                };
                if (skillRAW.type != null) newSkill.SkillType = (Type)Enum.Parse(typeof(Type), skillRAW.type);
                if (skillRAW.weapon_type != null) newSkill.WeaponType = skillRAW.weapon_type;
                if (skillRAW.slot != null) newSkill.Slot = (SkillSlot)Enum.Parse(typeof(SkillSlot), skillRAW.slot);
                if (skillRAW.professions != null)
                {
                    for (int i = 0; i < skillRAW.professions.Length; i++)
                    {
                        newSkill.Professions.Add((Character.Profession)Enum.Parse(typeof(Character.Profession), skillRAW.professions[i]));
                    }
                }
                if (skillRAW.categories != null)
                {
                    for (int i = 0; i < skillRAW.categories.Length; i++)
                    {
                        newSkill.Categories.Add(skillRAW.categories[i]);
                    }
                }
                if (skillRAW.attunement != null) newSkill.Attunement = skillRAW.attunement;
                if (skillRAW.dual_wield != null) newSkill.DualWieldRequirement = skillRAW.dual_wield;

                if (skillRAW.facts != null)
                {
                    for (int i = 0; i < skillRAW.facts.Length; i++)
                    {
                        if (skillRAW.facts[i].type == "AttributeAdjust")
                        {
                            AttriubuteAdjustSkillFact newFact = new AttriubuteAdjustSkillFact
                            {
                                Type = SkillFact.FactType.AttributeAdjust,
                                Text = skillRAW.facts[i].text,
                                Target = skillRAW.facts[i].target,
                                Value = (!string.IsNullOrEmpty(skillRAW.facts[i].value)) ? int.Parse(skillRAW.facts[i].value) : 0
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Buff")
                        {
                            BuffSkillFact newFact = new BuffSkillFact
                            {
                                Type = SkillFact.FactType.Buff,
                                Text = skillRAW.facts[i].text,
                                Status = skillRAW.facts[i].status,
                                Description = skillRAW.facts[i].description,
                                Count = (skillRAW.facts[i].apply_count.HasValue) ? skillRAW.facts[i].apply_count.Value : 0,
                                Duration = (skillRAW.facts[i].duration.HasValue) ? skillRAW.facts[i].duration.Value : 0
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "ComboField")
                        {
                            ComboFieldSkillFact newFact = new ComboFieldSkillFact
                            {
                                Type = SkillFact.FactType.ComboField,
                                Text = skillRAW.facts[i].text,
                                ComboFieldType = (ComboFieldSkillFact.FieldType)Enum.Parse(typeof(ComboFieldSkillFact.FieldType), skillRAW.facts[i].field_type)
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "ComboFinisher")
                        {
                            ComboFinisherSkillFact newFact = new ComboFinisherSkillFact
                            {
                                Type = SkillFact.FactType.ComboFinisher,
                                Text = skillRAW.facts[i].text,
                                ComboFinisherType = (ComboFinisherSkillFact.FinisherType)Enum.Parse(typeof(ComboFinisherSkillFact.FinisherType), skillRAW.facts[i].finisher_type),
                                Percent = skillRAW.facts[i].percent.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Damage")
                        {
                            DamageSkillFact newFact = new DamageSkillFact
                            {
                                Type = SkillFact.FactType.Damage,
                                Text = skillRAW.facts[i].text,
                                HitCount = skillRAW.facts[i].hit_count.Value,
                                Multiplier = skillRAW.facts[i].dmg_multiplier.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Distance")
                        {
                            DistanceSkillFact newFact = new DistanceSkillFact
                            {
                                Type = SkillFact.FactType.Distance,
                                Text = skillRAW.facts[i].text,
                                Distance = skillRAW.facts[i].distance.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Duration")
                        {
                            DurationSkillFact newFact = new DurationSkillFact
                            {
                                Type = SkillFact.FactType.Duration,
                                Text = skillRAW.facts[i].text,
                                Duration = skillRAW.facts[i].duration.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Heal")
                        {
                            HealSkillFact newFact = new HealSkillFact
                            {
                                Type = SkillFact.FactType.Heal,
                                Text = skillRAW.facts[i].text,
                                HitCount = skillRAW.facts[i].hit_count.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "HealingAdjust")
                        {
                            HealingAdjustSkillFact newFact = new HealingAdjustSkillFact
                            {
                                Type = SkillFact.FactType.HealingAdjust,
                                Text = skillRAW.facts[i].text,
                                HitCount = skillRAW.facts[i].hit_count.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "NoData")
                        {
                            NoDataSkillFact newFact = new NoDataSkillFact
                            {
                                Type = SkillFact.FactType.NoData,
                                Text = skillRAW.facts[i].text
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Number")
                        {
                            NumberSkillFact newFact = new NumberSkillFact
                            {
                                Type = SkillFact.FactType.Number,
                                Text = skillRAW.facts[i].text,
                                Value = (!string.IsNullOrEmpty(skillRAW.facts[i].value)) ? int.Parse(skillRAW.facts[i].value) : 0
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Percent")
                        {
                            PercentSkillFact newFact = new PercentSkillFact
                            {
                                Type = SkillFact.FactType.Percent,
                                Text = skillRAW.facts[i].text
                            };
                            if (skillRAW.facts[i].percent.HasValue) newFact.Percent = skillRAW.facts[i].percent.Value;
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "PrefixedBuff")
                        {
                            PrefixedBuffSkillFact newFact = new PrefixedBuffSkillFact
                            {
                                Type = SkillFact.FactType.PrefixedBuff,
                                Text = skillRAW.facts[i].text,
                                Status = skillRAW.facts[i].status,
                                Description = skillRAW.facts[i].description,
                                Count = (skillRAW.facts[i].apply_count.HasValue) ? skillRAW.facts[i].apply_count.Value : 0,
                                Duration = (skillRAW.facts[i].duration.HasValue) ? skillRAW.facts[i].duration.Value : 0
                            };
                            if (skillRAW.facts[i].prefix != null)
                            {
                                Prefix newPrefix = new Prefix
                                {
                                    Text = skillRAW.facts[i].prefix.text,
                                    Description = skillRAW.facts[i].prefix.text,
                                    Status = skillRAW.facts[i].prefix.text
                                };
                                if (skillRAW.facts[i].prefix.icon != null)
                                {
                                    string fileName = string.Empty;
                                    using (WebClient client = new WebClient())
                                    {
                                        fileName = skillRAW.facts[i].prefix.icon.Substring(skillRAW.facts[i].prefix.icon.LastIndexOf("/") + 1);
                                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].prefix.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName);
                                    }
                                    newPrefix.Icon = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName;
                                }
                                newFact.BuffPrefix = newPrefix;
                            }
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Radius")
                        {
                            RadiusSkillFact newFact = new RadiusSkillFact
                            {
                                Type = SkillFact.FactType.Radius,
                                Text = skillRAW.facts[i].text,
                                Distance = skillRAW.facts[i].distance.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Range")
                        {
                            RangeSkillFact newFact = new RangeSkillFact
                            {
                                Type = SkillFact.FactType.Range,
                                Text = skillRAW.facts[i].text,
                                Range = (!string.IsNullOrEmpty(skillRAW.facts[i].value)) ? int.Parse(skillRAW.facts[i].value) : 0
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Recharge")
                        {
                            RechargeSkillFact newFact = new RechargeSkillFact
                            {
                                Type = SkillFact.FactType.Recharge,
                                Text = skillRAW.facts[i].text,
                                Value = (!string.IsNullOrEmpty(skillRAW.facts[i].value)) ? double.Parse(skillRAW.facts[i].value) : 0
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Time")
                        {
                            TimeSkillFact newFact = new TimeSkillFact
                            {
                                Type = SkillFact.FactType.Time,
                                Text = skillRAW.facts[i].text,
                                Duration = skillRAW.facts[i].duration.Value
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                        if (skillRAW.facts[i].type == "Unblockable")
                        {
                            UnblockableSkillFact newFact = new UnblockableSkillFact
                            {
                                Type = SkillFact.FactType.Unblockable,
                                Text = skillRAW.facts[i].text,
                                Value = true
                            };
                            if (skillRAW.facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.facts[i].icon.Substring(skillRAW.facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.Facts.Add(newFact);
                        }
                    }
                }

                if (skillRAW.traited_facts != null)
                {
                    for (int i = 0; i < skillRAW.traited_facts.Length; i++)
                    {
                        if (skillRAW.traited_facts[i].type == "AttributeAdjust")
                        {
                            AttriubuteAdjustSkillFact newFact = new AttriubuteAdjustSkillFact
                            {
                                Type = SkillFact.FactType.AttributeAdjust,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Target = skillRAW.traited_facts[i].target,
                                Value = (!string.IsNullOrEmpty(skillRAW.traited_facts[i].value)) ? int.Parse(skillRAW.traited_facts[i].value) : 0
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Buff")
                        {
                            BuffSkillFact newFact = new BuffSkillFact
                            {
                                Type = SkillFact.FactType.Buff,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Status = skillRAW.traited_facts[i].status,
                                Description = skillRAW.traited_facts[i].description,
                                Count = (skillRAW.traited_facts[i].apply_count.HasValue) ? skillRAW.traited_facts[i].apply_count.Value : 0,
                                Duration = (skillRAW.traited_facts[i].duration.HasValue) ? skillRAW.traited_facts[i].duration.Value : 0
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "ComboField")
                        {
                            ComboFieldSkillFact newFact = new ComboFieldSkillFact
                            {
                                Type = SkillFact.FactType.ComboField,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                ComboFieldType = (ComboFieldSkillFact.FieldType)Enum.Parse(typeof(ComboFieldSkillFact.FieldType), skillRAW.traited_facts[i].field_type)
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "ComboFinisher")
                        {
                            ComboFinisherSkillFact newFact = new ComboFinisherSkillFact
                            {
                                Type = SkillFact.FactType.ComboFinisher,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                ComboFinisherType = (ComboFinisherSkillFact.FinisherType)Enum.Parse(typeof(ComboFinisherSkillFact.FinisherType), skillRAW.traited_facts[i].finisher_type),
                                Percent = skillRAW.traited_facts[i].percent.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Damage")
                        {
                            DamageSkillFact newFact = new DamageSkillFact
                            {
                                Type = SkillFact.FactType.Damage,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                HitCount = skillRAW.traited_facts[i].hit_count.Value,
                                Multiplier = skillRAW.traited_facts[i].dmg_multiplier.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Distance")
                        {
                            DistanceSkillFact newFact = new DistanceSkillFact
                            {
                                Type = SkillFact.FactType.Distance,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Distance = skillRAW.traited_facts[i].distance.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Duration")
                        {
                            DurationSkillFact newFact = new DurationSkillFact
                            {
                                Type = SkillFact.FactType.Duration,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Duration = skillRAW.traited_facts[i].duration.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Heal")
                        {
                            HealSkillFact newFact = new HealSkillFact
                            {
                                Type = SkillFact.FactType.Heal,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                HitCount = skillRAW.traited_facts[i].hit_count.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "HealingAdjust")
                        {
                            HealingAdjustSkillFact newFact = new HealingAdjustSkillFact
                            {
                                Type = SkillFact.FactType.HealingAdjust,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                HitCount = skillRAW.traited_facts[i].hit_count.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "NoData")
                        {
                            NoDataSkillFact newFact = new NoDataSkillFact
                            {
                                Type = SkillFact.FactType.NoData,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Number")
                        {
                            NumberSkillFact newFact = new NumberSkillFact
                            {
                                Type = SkillFact.FactType.Number,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Value = (!string.IsNullOrEmpty(skillRAW.traited_facts[i].value)) ? int.Parse(skillRAW.traited_facts[i].value) : 0
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Percent")
                        {
                            PercentSkillFact newFact = new PercentSkillFact
                            {
                                Type = SkillFact.FactType.Percent,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait
                            };
                            if (skillRAW.traited_facts[i].percent.HasValue) newFact.Percent = skillRAW.traited_facts[i].percent.Value;
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "PrefixedBuff")
                        {
                            PrefixedBuffSkillFact newFact = new PrefixedBuffSkillFact
                            {
                                Type = SkillFact.FactType.PrefixedBuff,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Status = skillRAW.traited_facts[i].status,
                                Description = skillRAW.traited_facts[i].description,
                                Count = (skillRAW.traited_facts[i].apply_count.HasValue) ? skillRAW.traited_facts[i].apply_count.Value : 0,
                                Duration = (skillRAW.traited_facts[i].duration.HasValue) ? skillRAW.traited_facts[i].duration.Value : 0
                            };
                            if (skillRAW.traited_facts[i].prefix != null)
                            {
                                Prefix newPrefix = new Prefix
                                {
                                    Text = skillRAW.traited_facts[i].prefix.text,
                                    Description = skillRAW.traited_facts[i].prefix.text,
                                    Status = skillRAW.traited_facts[i].prefix.text
                                };
                                if (skillRAW.traited_facts[i].prefix.icon != null)
                                {
                                    string fileName = string.Empty;
                                    using (WebClient client = new WebClient())
                                    {
                                        fileName = skillRAW.traited_facts[i].prefix.icon.Substring(skillRAW.traited_facts[i].prefix.icon.LastIndexOf("/") + 1);
                                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].prefix.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName);
                                    }
                                    newPrefix.Icon = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\buffprefix\" + fileName;
                                }
                                newFact.BuffPrefix = newPrefix;
                            }
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Radius")
                        {
                            RadiusSkillFact newFact = new RadiusSkillFact
                            {
                                Type = SkillFact.FactType.Radius,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Distance = skillRAW.traited_facts[i].distance.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Range")
                        {
                            RangeSkillFact newFact = new RangeSkillFact
                            {
                                Type = SkillFact.FactType.Range,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Range = (!string.IsNullOrEmpty(skillRAW.traited_facts[i].value)) ? int.Parse(skillRAW.traited_facts[i].value) : 0
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Recharge")
                        {
                            RechargeSkillFact newFact = new RechargeSkillFact
                            {
                                Type = SkillFact.FactType.Recharge,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Value = (!string.IsNullOrEmpty(skillRAW.traited_facts[i].value)) ? double.Parse(skillRAW.traited_facts[i].value) : 0
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Time")
                        {
                            TimeSkillFact newFact = new TimeSkillFact
                            {
                                Type = SkillFact.FactType.Time,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Duration = skillRAW.traited_facts[i].duration.Value
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                        if (skillRAW.traited_facts[i].type == "Unblockable")
                        {
                            UnblockableSkillFact newFact = new UnblockableSkillFact
                            {
                                Type = SkillFact.FactType.Unblockable,
                                Text = skillRAW.traited_facts[i].text,
                                Override = (skillRAW.traited_facts[i].overrides.HasValue) ? skillRAW.traited_facts[i].overrides.Value : -1,
                                RequiredTrait = skillRAW.traited_facts[i].requires_trait,
                                Value = true
                            };
                            if (skillRAW.traited_facts[i].icon != null)
                            {
                                string fileName = string.Empty;
                                using (WebClient client = new WebClient())
                                {
                                    fileName = skillRAW.traited_facts[i].icon.Substring(skillRAW.traited_facts[i].icon.LastIndexOf("/") + 1);
                                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.traited_facts[i].icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName);
                                }
                                newFact.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\facts\" + fileName;
                            }
                            newSkill.TraitedFacts.Add(newFact);
                        }
                    }
                }

                if (skillRAW.icon != null)
                {
                    string fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = skillRAW.icon.Substring(skillRAW.icon.LastIndexOf("/") + 1);
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\" + fileName)) client.DownloadFileAsync(new Uri(skillRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\" + fileName);
                    }
                    newSkill.Image = AppDomain.CurrentDomain.BaseDirectory + @"\images\skills\" + fileName;
                }

                if (skillRAW.transform_skills != null)
                {
                    for (int i = 0; i < skillRAW.transform_skills.Length; i++)
                    {
                        newSkill.TransformSkillID.Add(skillRAW.transform_skills[i].Value);
                    }
                }
                if (skillRAW.bundle_skills != null)
                {
                    for (int i = 0; i < skillRAW.bundle_skills.Length; i++)
                    {
                        newSkill.BundleSkillID.Add(skillRAW.bundle_skills[i].Value);
                    }
                }
                if (skillRAW.toolbelt_skill.HasValue)
                {
                    newSkill.ToolbeltSkillID = skillRAW.toolbelt_skill.Value;
                }

                main.Skills.Add(newSkill);
                main.OnCharStatusUpdate("Generated Skill " + newSkill.Name + ";" + newSkill.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    public abstract class SkillFact
    {
        public enum FactType
        {
            AttributeAdjust,
            Buff,
            ComboField,
            ComboFinisher,
            Damage,
            Distance,
            Duration,
            Heal,
            HealingAdjust,
            NoData,
            Number,
            Percent,
            PrefixedBuff,
            Radius,
            Range,
            Recharge,
            Time,
            Unblockable
        }
        public string Text { get; set; }
        public string Image { get; set; }
        public FactType Type { get; set; }
        public int? RequiredTrait { get; set; }
        public int? Override { get; set; }
    }
    public class AttriubuteAdjustSkillFact : SkillFact
    {
        public int Value { get; set; }
        public string Target { get; set; }
    }
    public class BuffSkillFact : SkillFact
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public int? Duration { get; set; }
    }
    public class ComboFieldSkillFact : SkillFact
    {
        public enum FieldType
        {
            Air,
            Dark,
            Fire,
            Ice,
            Light,
            Lightning,
            Poison,
            Smoke,
            Ethereal,
            Water
        }
        public FieldType ComboFieldType { get; set; }
    }
    public class ComboFinisherSkillFact : SkillFact
    {
        public enum FinisherType
        {
            Blast,
            Leap,
            Projectile,
            Whirl
        }
        public FinisherType ComboFinisherType { get; set; }
        public double Percent { get; set; }
    }
    public class DamageSkillFact : SkillFact
    {
        public int HitCount { get; set; }
        public double Multiplier { get; set; }
    }
    public class DistanceSkillFact : SkillFact
    {
        public int Distance { get; set; }
    }
    public class DurationSkillFact : SkillFact
    {
        public int Duration { get; set; }
    }
    public class HealSkillFact : SkillFact
    {
        public int HitCount { get; set; }
    }
    public class HealingAdjustSkillFact : SkillFact
    {
        public int HitCount { get; set; }
    }
    public class NoDataSkillFact : SkillFact
    {
    }
    public class NumberSkillFact : SkillFact
    {
        public int Value { get; set; }
    }
    public class PercentSkillFact : SkillFact
    {
        public double Percent { get; set; }
    }
    public class PrefixedBuffSkillFact : SkillFact
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public int? Duration { get; set; }
        public Prefix BuffPrefix { get; set; }
    }
    public class RadiusSkillFact : SkillFact
    {
        public int Distance { get; set; }
    }
    public class RangeSkillFact : SkillFact
    {
        public int Range { get; set; }
    }
    public class RechargeSkillFact : SkillFact
    {
        public double Value { get; set; }
    }
    public class TimeSkillFact : SkillFact
    {
        public int Duration { get; set; }
    }
    public class UnblockableSkillFact : SkillFact
    {
        public bool Value { get; set; }
    }
    public class Prefix
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    class SkillRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string chat_link { get; set; }
        public string type { get; set; }
        public string weapon_type { get; set; }
        public string[] professions { get; set; }
        public string slot { get; set; }
        public SkillSub1RAW[] facts { get; set; }
        public SkillSub1RAW[] traited_facts { get; set; }
        public string[] categories { get; set; }
        public string attunement { get; set; }
        public int? cost { get; set; }
        public string dual_wield { get; set; }
        public int? flip_skill { get; set; }
        public int? initiative { get; set; }
        public int? next_chain { get; set; }
        public int? prev_chain { get; set; }
        public int?[] transform_skills { get; set; }
        public int?[] bundle_skills { get; set; }
        public int? toolbelt_skill { get; set; }
    }
    class SkillSub1RAW
    {
        public int? requires_trait { get; set; }
        public int? overrides { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string target { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public int? apply_count { get; set; }
        public int? duration { get; set; }
        public string field_type { get; set; }
        public string finisher_type { get; set; }
        public double? percent { get; set; }
        public int? hit_count { get; set; }
        public double? dmg_multiplier { get; set; }
        public int? distance { get; set; }
        public SkillSub3RAW prefix { get; set; }
    }
    class SkillSub3RAW
    {
        public string text { get; set; }
        public string icon { get; set; }
        public string status { get; set; }
        public string description { get; set; }
    }
}
