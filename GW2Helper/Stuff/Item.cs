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
    public class Item
    {
        public enum Type
        {
            Armor,
            Back,
            Bag,
            Consumable,
            Container,
            CraftingMaterial,
            Gathering,
            Gizmo,
            Key,
            MiniPet,
            Tool,
            Trait,
            Trinket,
            Trophy,
            UpgradeComponent,
            Weapon
        }
        public enum Rarity
        {
            Junk,
            Basic,
            Fine,
            Masterwork,
            Rare,
            Exotic,
            Ascended,
            Legendary
        }
        public enum Flag
        {
            AccountBindOnUse,
            AccountBound,
            Attuned,
            BulkConsume,
            DeleteWarning,
            HideSuffix,
            Infused,
            MonsterOnly,
            NoMysticForge,
            NoSalvage,
            NoSell,
            NotUpgradeable,
            NoUnderwater,
            SoulbindOnAcquire,
            SoulBindOnUse,
            Tonic,
            Unique
        }
        public enum GameType
        {
            Activity,
            Dungeon,
            Pve,
            Pvp,
            PvpLobby,
            Wvw
        }
        public enum Restriction
        {
            Asura,
            Charr,
            Human,
            Norn,
            Sylvari,
            Elementalist,
            Engineer,
            Guardian,
            Mesmer,
            Necromancer,
            Ranger,
            Thief,
            Warrior,
            Male,
            Female
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public Type ItemType { get; set; }
        public Rarity ItemRarity { get; set; }
        public int Level { get; set; }
        public int VendorValue { get; set; }
        public Skin ItemSkin { get; set; }
        public int? SkinID { get; set; }
        public List<Flag> Flags { get; set; }
        public List<GameType> GameTypes { get; set; }
        public List<Restriction> Restrictions { get; set; }
        public ItemDetail Details { get; set; }

        public static void GetItemsFromJSON(string json, Main main)
        {
            ItemRAW[] rawItems = new ItemRAW[1];
            try
            {
                rawItems = JsonConvert.DeserializeObject<ItemRAW[]>(json);
            }
            catch (Exception e)
            {
                rawItems[0] = JsonConvert.DeserializeObject<ItemRAW>(json);
            }
            for (int a = 0; a < rawItems.Length; a++)
            {
                double cur = a, max = rawItems.Length;
                ItemRAW itemRAW = rawItems[a];
                main.JSON.Add(new KeyValuePair<string, string>("Item", JsonConvert.SerializeObject(itemRAW)));
                Item newItem = new Item
                {
                    ID = itemRAW.id,
                    Name = itemRAW.name,
                    Description = itemRAW.description,
                    ItemType = (Type)Enum.Parse(typeof(Type), itemRAW.type),
                    ItemRarity = (Rarity)Enum.Parse(typeof(Rarity), itemRAW.rarity),
                    Level = itemRAW.level,
                    VendorValue = itemRAW.vendor_value,
                    Flags = new List<Flag>(),
                    GameTypes = new List<GameType>(),
                    Restrictions = new List<Restriction>()
                };

                string fileName = string.Empty;

                if (!string.IsNullOrEmpty(itemRAW.icon))
                {
                    fileName = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        fileName = itemRAW.icon.Substring(itemRAW.icon.LastIndexOf("/") + 1);
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"images\items\");
                        if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"images\items\" + fileName)) client.DownloadFileAsync(new Uri(itemRAW.icon), AppDomain.CurrentDomain.BaseDirectory + @"\images\items\" + fileName);
                    }
                    newItem.Image = AppDomain.CurrentDomain.BaseDirectory + @"images\items\" + fileName;
                }
                for (int i = 0; i < itemRAW.flags.Length; i++)
                {
                    newItem.Flags.Add((Flag)Enum.Parse(typeof(Flag), itemRAW.flags[i]));
                }
                for (int i = 0; i < itemRAW.game_types.Length; i++)
                {
                    newItem.GameTypes.Add((GameType)Enum.Parse(typeof(GameType), itemRAW.game_types[i]));
                }
                for (int i = 0; i < itemRAW.restrictions.Length; i++)
                {
                    newItem.Restrictions.Add((Restriction)Enum.Parse(typeof(Restriction), itemRAW.restrictions[i]));
                }
                if (itemRAW.default_skin.HasValue)
                {
                    newItem.SkinID = itemRAW.default_skin.Value;
                }

                if (newItem.ItemType == Type.Armor)
                {
                    ArmorItemDetail newItemDetail = new ArmorItemDetail
                    {
                        Type = (ArmorItemDetail.ArmorType)Enum.Parse(typeof(ArmorItemDetail.ArmorType), itemRAW.details.type),
                        Weight = (ArmorItemDetail.WeightClass)Enum.Parse(typeof(ArmorItemDetail.WeightClass), itemRAW.details.weight_class),
                        Defense = itemRAW.details.defense.Value,
                        Infusions = new List<Infusion>(),
                        StatChoices = new List<ItemStat>()
                    };

                    if (itemRAW.details.infix_upgrade != null)
                    {
                        InfixUpgrade newInfixUpgrade = new InfixUpgrade();

                        if (itemRAW.details.infix_upgrade.buff != null)
                        {
                            int skillID = itemRAW.details.infix_upgrade.buff.skill_id;
                            ItemBuff newBuff = new ItemBuff
                            {
                                SkillID = itemRAW.details.infix_upgrade.buff.skill_id,
                                Description = itemRAW.details.infix_upgrade.buff.description
                            };
                            newInfixUpgrade.Buff = newBuff;
                        }
                        if (itemRAW.details.infix_upgrade.attributes != null)
                        {
                            List<ItemAttribute> newAttributes = new List<ItemAttribute>();
                            for (int i = 0; i < itemRAW.details.infix_upgrade.attributes.Length; i++)
                            {
                                ItemAttribute newAttribute = new ItemAttribute
                                {
                                    Att = (ItemAttribute.Attribute)Enum.Parse(typeof(ItemAttribute.Attribute), itemRAW.details.infix_upgrade.attributes[i].attribute),
                                    Modifier = itemRAW.details.infix_upgrade.attributes[i].modifier
                                };
                                newAttributes.Add(newAttribute);
                            }
                            newInfixUpgrade.Attributes = newAttributes;
                        }

                        newItemDetail.InfixUpgrades = newInfixUpgrade;
                    }

                    if (itemRAW.details.infusion_slots.Length > 0)
                    {
                        List<Infusion> newInfusions = new List<Infusion>();
                        for (int i = 0; i < itemRAW.details.infusion_slots.Length; i++)
                        {
                            Infusion newInfusion = new Infusion
                            {
                                Flags = new List<Infusion.Flag>()
                            };
                            for (int j = 0; j < itemRAW.details.infusion_slots[i].flags.Length; j++)
                            {
                                newInfusion.Flags.Add((Infusion.Flag)Enum.Parse(typeof(Infusion.Flag), itemRAW.details.infusion_slots[i].flags[j]));
                            }
                            if (itemRAW.details.infusion_slots[i].item_id.HasValue)
                            {
                                newInfusion.ItemID = itemRAW.details.infusion_slots[i].item_id.Value;
                            }

                            newInfusions.Add(newInfusion);
                        }
                        newItemDetail.Infusions = newInfusions;
                    }

                    if (itemRAW.details.suffix_item_id.HasValue)
                    {
                        newItemDetail.ItemID = itemRAW.details.suffix_item_id.Value;
                    }

                    if (itemRAW.details.stat_choices != null)
                    {
                        for (int i = 0; i < itemRAW.details.stat_choices.Length; i++)
                        {
                            int statID = int.Parse(itemRAW.details.stat_choices[i]);
                            ItemStat newStat = main.ItemStats.FirstOrDefault(st => st.ID == statID);
                            newItemDetail.StatChoices.Add(newStat);
                        }
                    }

                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Back)
                {
                    BackItemDetail newItemDetail = new BackItemDetail
                    {
                        Infusions = new List<Infusion>(),
                        StatChoices = new List<ItemStat>()
                    };

                    if (itemRAW.details.infix_upgrade != null)
                    {
                        InfixUpgrade newInfixUpgrade = new InfixUpgrade();

                        if (itemRAW.details.infix_upgrade.buff != null)
                        {
                            int skillID = itemRAW.details.infix_upgrade.buff.skill_id;
                            ItemBuff newBuff = new ItemBuff
                            {
                                SkillID = itemRAW.details.infix_upgrade.buff.skill_id,
                                Description = itemRAW.details.infix_upgrade.buff.description
                            };
                            newInfixUpgrade.Buff = newBuff;
                        }
                        if (itemRAW.details.infix_upgrade.attributes != null)
                        {
                            List<ItemAttribute> newAttributes = new List<ItemAttribute>();
                            for (int i = 0; i < itemRAW.details.infix_upgrade.attributes.Length; i++)
                            {
                                ItemAttribute newAttribute = new ItemAttribute
                                {
                                    Att = (ItemAttribute.Attribute)Enum.Parse(typeof(ItemAttribute.Attribute), itemRAW.details.infix_upgrade.attributes[i].attribute),
                                    Modifier = itemRAW.details.infix_upgrade.attributes[i].modifier
                                };
                                newAttributes.Add(newAttribute);
                            }
                            newInfixUpgrade.Attributes = newAttributes;
                        }

                        newItemDetail.InfixUpgrades = newInfixUpgrade;
                    }

                    if (itemRAW.details.infusion_slots.Length > 0)
                    {
                        List<Infusion> newInfusions = new List<Infusion>();
                        for (int i = 0; i < itemRAW.details.infusion_slots.Length; i++)
                        {
                            Infusion newInfusion = new Infusion
                            {
                                Flags = new List<Infusion.Flag>()
                            };
                            for (int j = 0; j < itemRAW.details.infusion_slots[i].flags.Length; j++)
                            {
                                newInfusion.Flags.Add((Infusion.Flag)Enum.Parse(typeof(Infusion.Flag), itemRAW.details.infusion_slots[i].flags[j]));
                            }
                            if (itemRAW.details.infusion_slots[i].item_id.HasValue)
                            {
                                newInfusion.ItemID = itemRAW.details.infusion_slots[i].item_id.Value;
                            }

                            newInfusions.Add(newInfusion);
                        }
                        newItemDetail.Infusions = newInfusions;
                    }

                    if (itemRAW.details.suffix_item_id.HasValue)
                    {
                        newItemDetail.ItemID = itemRAW.details.suffix_item_id.Value;
                    }

                    if (itemRAW.details.stat_choices != null)
                    {
                        for (int i = 0; i < itemRAW.details.stat_choices.Length; i++)
                        {
                            int statID = int.Parse(itemRAW.details.stat_choices[i]);
                            ItemStat newStat = main.ItemStats.FirstOrDefault(st => st.ID == statID);
                            newItemDetail.StatChoices.Add(newStat);
                        }
                    }

                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Bag)
                {
                    BagItemDetail newItemDetail = new BagItemDetail
                    {
                        Size = itemRAW.details.size.Value,
                        NoSellSort = itemRAW.details.no_sell_or_sort.Value
                    };
                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Consumable)
                {
                    ConsumableItemDetail newItemDetail = new ConsumableItemDetail
                    {
                        ConsType = (ConsumableItemDetail.ConsumableType)Enum.Parse(typeof(ConsumableItemDetail.ConsumableType), itemRAW.details.type),
                        Description = itemRAW.details.description,
                        DurationInMilliseconds = (itemRAW.details.duration_ms.HasValue) ? itemRAW.details.duration_ms.Value : 0,
                        StackCount = (itemRAW.details.apply_count.HasValue) ? itemRAW.details.apply_count.Value : 0,
                        Name = itemRAW.details.name,
                        Skins = new List<Skin>(),
                        SkinID = new List<int>()
                    };
                    if (itemRAW.details.unlock_type != null) newItemDetail.UnType = (ConsumableItemDetail.UnlockType)Enum.Parse(typeof(ConsumableItemDetail.UnlockType), itemRAW.details.unlock_type);

                    if (itemRAW.details.color_id.HasValue)
                    {
                        int colourID = itemRAW.details.color_id.Value;
                        Colour newColour = main.Colours.FirstOrDefault(co => co.ID == colourID);
                        newItemDetail.ConsumableColour = newColour;
                    }
                    if (itemRAW.details.recipe_id.HasValue)
                    {
                        newItemDetail.RecipeID = itemRAW.details.recipe_id.Value;
                    }
                    if (itemRAW.details.skins != null)
                    {
                        for (int i = 0; i < itemRAW.details.skins.Length; i++)
                        {
                            newItemDetail.SkinID.Add(itemRAW.details.skins[i].Value);
                        }
                    }
                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Container)
                {
                    ContainerItemDetail newItemDetail = new ContainerItemDetail
                    {
                        ContType = (ContainerItemDetail.ContainerType)Enum.Parse(typeof(ContainerItemDetail.ContainerType), itemRAW.details.type)
                    };
                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Gathering)
                {
                    GatheringToolItemDetail newItemDetail = new GatheringToolItemDetail
                    {
                        GatType = (GatheringToolItemDetail.GatheringToolType)Enum.Parse(typeof(GatheringToolItemDetail.GatheringToolType), itemRAW.details.type)
                    };
                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Gizmo)
                {
                    GizmoItemDetail newItemDetail = new GizmoItemDetail
                    {
                        GizType = (GizmoItemDetail.GizmoType)Enum.Parse(typeof(GizmoItemDetail.GizmoType), itemRAW.details.type)
                    };
                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.MiniPet)
                {
                    MiniItemDetail newItemDetail = new MiniItemDetail
                    {
                        MiniID = itemRAW.details.minipet_id.Value
                    };

                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Tool)
                {
                    SalvageKitItemDetail newItemDetail = new SalvageKitItemDetail
                    {
                        SalType = SalvageKitItemDetail.SalvageType.Salvage,
                        Charges = itemRAW.details.charges.Value
                    };
                }
                if (newItem.ItemType == Type.Trinket)
                {
                    TrinketItemDetail newItemDetail = new TrinketItemDetail
                    {
                        TrinkType = (TrinketItemDetail.TrinketType)Enum.Parse(typeof(TrinketItemDetail.TrinketType), itemRAW.details.type),
                        Infusions = new List<Infusion>(),
                        StatChoices = new List<ItemStat>()
                    };

                    if (itemRAW.details.infix_upgrade != null)
                    {
                        InfixUpgrade newInfixUpgrade = new InfixUpgrade();

                        if (itemRAW.details.infix_upgrade.buff != null)
                        {
                            int skillID = itemRAW.details.infix_upgrade.buff.skill_id;
                            ItemBuff newBuff = new ItemBuff
                            {
                                SkillID = itemRAW.details.infix_upgrade.buff.skill_id,
                                Description = itemRAW.details.infix_upgrade.buff.description
                            };
                            newInfixUpgrade.Buff = newBuff;
                        }
                        if (itemRAW.details.infix_upgrade.attributes != null)
                        {
                            List<ItemAttribute> newAttributes = new List<ItemAttribute>();
                            for (int i = 0; i < itemRAW.details.infix_upgrade.attributes.Length; i++)
                            {
                                ItemAttribute newAttribute = new ItemAttribute
                                {
                                    Att = (ItemAttribute.Attribute)Enum.Parse(typeof(ItemAttribute.Attribute), itemRAW.details.infix_upgrade.attributes[i].attribute),
                                    Modifier = itemRAW.details.infix_upgrade.attributes[i].modifier
                                };
                                newAttributes.Add(newAttribute);
                            }
                            newInfixUpgrade.Attributes = newAttributes;
                        }

                        newItemDetail.InfixUpgrades = newInfixUpgrade;
                    }

                    if (itemRAW.details.infusion_slots.Length > 0)
                    {
                        List<Infusion> newInfusions = new List<Infusion>();
                        for (int i = 0; i < itemRAW.details.infusion_slots.Length; i++)
                        {
                            Infusion newInfusion = new Infusion
                            {
                                Flags = new List<Infusion.Flag>()
                            };
                            for (int j = 0; j < itemRAW.details.infusion_slots[i].flags.Length; j++)
                            {
                                newInfusion.Flags.Add((Infusion.Flag)Enum.Parse(typeof(Infusion.Flag), itemRAW.details.infusion_slots[i].flags[j]));
                            }
                            if (itemRAW.details.infusion_slots[i].item_id.HasValue)
                            {
                                newInfusion.ItemID = itemRAW.details.infusion_slots[i].item_id.Value;
                            }

                            newInfusions.Add(newInfusion);
                        }
                        newItemDetail.Infusions = newInfusions;
                    }

                    if (itemRAW.details.suffix_item_id.HasValue)
                    {
                        newItemDetail.ItemID = itemRAW.details.suffix_item_id.Value;
                    }

                    if (itemRAW.details.stat_choices != null)
                    {
                        for (int i = 0; i < itemRAW.details.stat_choices.Length; i++)
                        {
                            int statID = int.Parse(itemRAW.details.stat_choices[i]);
                            ItemStat newStat = main.ItemStats.FirstOrDefault(st => st.ID == statID);
                            newItemDetail.StatChoices.Add(newStat);
                        }
                    }

                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.UpgradeComponent)
                {
                    UpgradeItemDetail newItemDetail = new UpgradeItemDetail
                    {
                        UpType = (UpgradeItemDetail.UpgradeType)Enum.Parse(typeof(UpgradeItemDetail.UpgradeType), itemRAW.details.type),
                        Flags = new List<UpgradeItemDetail.Flag>(),
                        Infusions = new List<UpgradeItemDetail.InfusionFlag>(),
                        Suffix = itemRAW.details.suffix
                    };
                    if (itemRAW.details.bonuses != null) newItemDetail.Bonuses = itemRAW.details.bonuses.ToList();
                    if (itemRAW.details.infusion_upgrade_flags != null)
                    {
                        for (int i = 0; i < itemRAW.details.infusion_upgrade_flags.Length; i++)
                        {
                            newItemDetail.Infusions.Add((UpgradeItemDetail.InfusionFlag)Enum.Parse(typeof(UpgradeItemDetail.InfusionFlag), itemRAW.details.infusion_upgrade_flags[i]));
                        }
                    }
                    if (itemRAW.details.infix_upgrade != null)
                    {
                        InfixUpgrade newInfixUpgrade = new InfixUpgrade();

                        if (itemRAW.details.infix_upgrade.buff != null)
                        {
                            int skillID = itemRAW.details.infix_upgrade.buff.skill_id;
                            ItemBuff newBuff = new ItemBuff
                            {
                                SkillID = itemRAW.details.infix_upgrade.buff.skill_id,
                                Description = itemRAW.details.infix_upgrade.buff.description
                            };
                            newInfixUpgrade.Buff = newBuff;
                        }
                        if (itemRAW.details.infix_upgrade.attributes != null)
                        {
                            List<ItemAttribute> newAttributes = new List<ItemAttribute>();
                            for (int i = 0; i < itemRAW.details.infix_upgrade.attributes.Length; i++)
                            {
                                ItemAttribute newAttribute = new ItemAttribute
                                {
                                    Att = (ItemAttribute.Attribute)Enum.Parse(typeof(ItemAttribute.Attribute), itemRAW.details.infix_upgrade.attributes[i].attribute),
                                    Modifier = itemRAW.details.infix_upgrade.attributes[i].modifier
                                };
                                newAttributes.Add(newAttribute);
                            }
                            newInfixUpgrade.Attributes = newAttributes;
                        }

                        newItemDetail.InfixUpgrades = newInfixUpgrade;
                    }
                    if (itemRAW.details.flags != null)
                    {
                        for (int i = 0; i < itemRAW.details.flags.Length; i++)
                        {
                            newItemDetail.Flags.Add((UpgradeItemDetail.Flag)Enum.Parse(typeof(UpgradeItemDetail.Flag), itemRAW.details.flags[i]));
                        }
                    }

                    newItem.Details = newItemDetail;
                }
                if (newItem.ItemType == Type.Weapon)
                {
                    WeaponItemDetail newItemDetail = new WeaponItemDetail
                    {
                        WeapType = (WeaponItemDetail.WeaponType)Enum.Parse(typeof(WeaponItemDetail.WeaponType), itemRAW.details.type),
                        DamType = (WeaponItemDetail.DamageType)Enum.Parse(typeof(WeaponItemDetail.DamageType), itemRAW.details.damage_type),
                        MIN = itemRAW.details.min_power.Value,
                        MAX = itemRAW.details.max_power.Value,
                        Defense = itemRAW.details.defense.Value,
                        Infusions = new List<Infusion>(),
                        StatChoices = new List<ItemStat>()
                    };

                    if (itemRAW.details.infix_upgrade != null)
                    {
                        InfixUpgrade newInfixUpgrade = new InfixUpgrade();

                        if (itemRAW.details.infix_upgrade.buff != null)
                        {
                            int skillID = itemRAW.details.infix_upgrade.buff.skill_id;
                            ItemBuff newBuff = new ItemBuff
                            {
                                SkillID = itemRAW.details.infix_upgrade.buff.skill_id,
                                Description = itemRAW.details.infix_upgrade.buff.description
                            };
                            newInfixUpgrade.Buff = newBuff;
                        }
                        if (itemRAW.details.infix_upgrade.attributes != null)
                        {
                            List<ItemAttribute> newAttributes = new List<ItemAttribute>();
                            for (int i = 0; i < itemRAW.details.infix_upgrade.attributes.Length; i++)
                            {
                                ItemAttribute newAttribute = new ItemAttribute
                                {
                                    Att = (ItemAttribute.Attribute)Enum.Parse(typeof(ItemAttribute.Attribute), itemRAW.details.infix_upgrade.attributes[i].attribute),
                                    Modifier = itemRAW.details.infix_upgrade.attributes[i].modifier
                                };
                                newAttributes.Add(newAttribute);
                            }
                            newInfixUpgrade.Attributes = newAttributes;
                        }

                        newItemDetail.InfixUpgrades = newInfixUpgrade;
                    }

                    if (itemRAW.details.infusion_slots.Length > 0)
                    {
                        List<Infusion> newInfusions = new List<Infusion>();
                        for (int i = 0; i < itemRAW.details.infusion_slots.Length; i++)
                        {
                            Infusion newInfusion = new Infusion
                            {
                                Flags = new List<Infusion.Flag>()
                            };
                            for (int j = 0; j < itemRAW.details.infusion_slots[i].flags.Length; j++)
                            {
                                newInfusion.Flags.Add((Infusion.Flag)Enum.Parse(typeof(Infusion.Flag), itemRAW.details.infusion_slots[i].flags[j]));
                            }
                            if (itemRAW.details.infusion_slots[i].item_id.HasValue)
                            {
                                newInfusion.ItemID = itemRAW.details.infusion_slots[i].item_id.Value;
                            }

                            newInfusions.Add(newInfusion);
                        }
                        newItemDetail.Infusions = newInfusions;
                    }

                    if (itemRAW.details.suffix_item_id.HasValue)
                    {
                        newItemDetail.ItemID = itemRAW.details.suffix_item_id.Value;
                    }

                    if (itemRAW.details.stat_choices != null)
                    {
                        for (int i = 0; i < itemRAW.details.stat_choices.Length; i++)
                        {
                            int statID = int.Parse(itemRAW.details.stat_choices[i]);
                            ItemStat newStat = main.ItemStats.FirstOrDefault(st => st.ID == statID);
                            newItemDetail.StatChoices.Add(newStat);
                        }
                    }

                    newItem.Details = newItemDetail;
                }

                main.Items.Add(newItem);
                main.OnCharStatusUpdate("Generated Item " + newItem.Name + ";" + newItem.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    public abstract class ItemDetail
    {
    }
    public class ArmorItemDetail : ItemDetail
    {
        public enum ArmorType
        {
            Boots,
            Coat,
            Gloves,
            Helm,
            HelmAquatic,
            Leggings,
            Shoulders
        }
        public enum WeightClass
        {
            Heavy,
            Medium,
            Light,
            Clothing
        }
        public ArmorType Type { get; set; }
        public WeightClass Weight { get; set; }
        public int Defense { get; set; }
        public List<Infusion> Infusions { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public Item SuffixItem { get; set; }
        public int? ItemID { get; set; }
        public List<ItemStat> StatChoices { get; set; }
    }
    public class BackItemDetail : ItemDetail
    {
        public List<Infusion> Infusions { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public Item SuffixItem { get; set; }
        public int? ItemID { get; set; }
        public List<ItemStat> StatChoices { get; set; }
    }
    public class BagItemDetail : ItemDetail
    {
        public int Size { get; set; }
        public bool NoSellSort { get; set; }
    }
    public class ConsumableItemDetail : ItemDetail
    {
        public enum ConsumableType
        {
            AppearanceChange,
            Booze,
            ContractNpc,
            Currency,
            Food,
            Generic,
            Halloween,
            Immediate,
            Transmutation,
            Unlock,
            UpgradeRemoval,
            Utility,
            TeleportToFriend,
            RandomUnlock,
            MountRandomUnlock
        }
        public enum UnlockType
        {
            BagSlot,
            BankTab,
            CollectibleCapacity,
            Content,
            CraftingRecipe,
            Dye,
            Outfit,
            GliderSkin,
            Champion,
            Minipet,
            RandomUnlock,
            SharedSlot,
            Ms
        }
        public ConsumableType ConsType { get; set; }
        public string Description { get; set; }
        public int? DurationInMilliseconds { get; set; }
        public UnlockType UnType { get; set; }
        public Colour ConsumableColour { get; set; }
        public Recipe ConsumableRecipe { get; set; }
        public int? RecipeID { get; set; }
        public int? StackCount { get; set; }
        public string Name { get; set; }
        public List<Skin> Skins { get; set; }
        public List<int> SkinID { get; set; }
    }
    public class ContainerItemDetail : ItemDetail
    {
        public enum ContainerType
        {
            Default,
            GiftBox,
            OpenUI,
            Immediate
        }
        public ContainerType ContType { get; set; }
    }
    public class GatheringToolItemDetail : ItemDetail
    {
        public enum GatheringToolType
        {
            Foraging,
            Logging,
            Mining
        }
        public GatheringToolType GatType { get; set; }
    }
    public class GizmoItemDetail : ItemDetail
    {
        public enum GizmoType
        {
            Default,
            ContainerKey,
            RentableContractNpc,
            UnlimitedConsumable
        }
        public GizmoType GizType { get; set; }
    }
    public class MiniItemDetail : ItemDetail
    {
        public MiniPet Mini { get; set; }
        public int? MiniID { get; set; }
    }
    public class SalvageKitItemDetail : ItemDetail
    {
        public enum SalvageType
        {
            Salvage
        }
        public SalvageType SalType { get; set; }
        public int Charges { get; set; }
    }
    public class TrinketItemDetail : ItemDetail
    {
        public enum TrinketType
        {
            Accessory,
            Amulet,
            Ring
        }
        public TrinketType TrinkType { get; set; }
        public List<Infusion> Infusions { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public Item SuffixItem { get; set; }
        public int? ItemID { get; set; }
        public List<ItemStat> StatChoices { get; set; }
    }
    public class UpgradeItemDetail : ItemDetail
    {
        public enum UpgradeType
        {
            Default,
            Gem,
            Rune,
            Sigil
        }
        public enum Flag
        {
            Axe,
            Dagger,
            Focus,
            Greatsword,
            Hammer,
            Harpoon,
            LongBow,
            Mace,
            Pistol,
            Rifle,
            Scepter,
            Shield,
            ShortBow,
            Speargun,
            Staff,
            Sword,
            Torch,
            Trident,
            Warhorn,
            HeavyArmor,
            MediumArmor,
            LightArmor,
            Trinket
        }
        public enum InfusionFlag
        {
            Defense,
            Offense,
            Utility,
            Agony,
            Infusion,
            Enrichment
        }
        public UpgradeType UpType { get; set; }
        public List<Flag> Flags { get; set; }
        public List<InfusionFlag> Infusions { get; set; }
        public string Suffix { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public List<string> Bonuses { get; set; }
    }
    public class WeaponItemDetail : ItemDetail
    {
        public enum WeaponType
        {
            Axe,
            Dagger,
            Mace,
            Pistol,
            Scepter,
            Sword,
            Focus,
            Shield,
            Torch,
            Warhorn,
            Greatsword,
            Hammer,
            LongBow,
            Rifle,
            ShortBow,
            Staff,
            Harpoon,
            Speargun,
            Trident,
            LargeBundle,
            SmallBundle,
            Toy,
            ToyTwoHanded
        }
        public enum DamageType
        {
            Fire,
            Ice,
            Lightning,
            Physical,
            Choking
        }
        public WeaponType WeapType { get; set; }
        public DamageType DamType { get; set; }
        public int MIN { get; set; }
        public int MAX { get; set; }
        public int Defense { get; set; }
        public List<Infusion> Infusions { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public Item SuffixItem { get; set; }
        public int? ItemID { get; set; }
        public List<ItemStat> StatChoices { get; set; }
    }

    public class Infusion
    {
        public enum Flag
        {
            Enrichment,
            Infusion
        }
        public List<Flag> Flags { get; set; }
        public Item ExistingItem { get; set; }
        public int? ItemID { get; set; }
    }
    public class InfixUpgrade
    {
        public List<ItemAttribute> Attributes { get; set; }
        public ItemBuff Buff { get; set; }
    }
    public class ItemAttribute
    {
        public enum Attribute
        {
            AgonyResistance,
            BoonDuration,
            ConditionDamage,
            ConditionDuration,
            CritDamage,
            Healing,
            Power,
            Precision,
            Toughness,
            Vitality
        }
        public Attribute Att { get; set; }
        public int Modifier { get; set; }
    }
    public class ItemBuff
    {
        public Skill BuffSkill { get; set; }
        public int? SkillID { get; set; }
        public string Description { get; set; }
    }
    public class ItemStat
    {
        public enum Attribute
        {
            AgonyResistance,
            BoonDuration,
            ConditionDamage,
            ConditionDuration,
            CritDamage,
            Healing,
            Power,
            Precision,
            Toughness,
            Vitality
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public List<KeyValuePair<Attribute, double>> Attributes { get; set; }
        
        public static void GetItemStatsFromJSON(string json, Main main)
        {
            ItemStatRAW[] rawItemStats = new ItemStatRAW[1];
            try
            {
                rawItemStats = JsonConvert.DeserializeObject<ItemStatRAW[]>(json);
            }
            catch (Exception e)
            {
                rawItemStats[0] = JsonConvert.DeserializeObject<ItemStatRAW>(json);
            }
            for (int s = 0; s < rawItemStats.Length; s++)
            {
                double cur = s, max = rawItemStats.Length;
                ItemStatRAW itemStatRAW = rawItemStats[s];
                main.JSON.Add(new KeyValuePair<string, string>("ItemStat", JsonConvert.SerializeObject(itemStatRAW)));
                ItemStat newItemStat = new ItemStat
                {
                    ID = itemStatRAW.id,
                    Name = itemStatRAW.name,
                    Attributes = new List<KeyValuePair<Attribute, double>>()
                };
                if (itemStatRAW.attributes != null)
                {
                    if (itemStatRAW.attributes.AgonyResistance.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.AgonyResistance, itemStatRAW.attributes.AgonyResistance.Value));
                    if (itemStatRAW.attributes.BoonDuration.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.BoonDuration, itemStatRAW.attributes.BoonDuration.Value));
                    if (itemStatRAW.attributes.ConditionDamage.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.ConditionDamage, itemStatRAW.attributes.ConditionDamage.Value));
                    if (itemStatRAW.attributes.ConditionDuration.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.ConditionDuration, itemStatRAW.attributes.ConditionDuration.Value));
                    if (itemStatRAW.attributes.CritDamage.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.CritDamage, itemStatRAW.attributes.CritDamage.Value));
                    if (itemStatRAW.attributes.Healing.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.Healing, itemStatRAW.attributes.Healing.Value));
                    if (itemStatRAW.attributes.Power.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.Power, itemStatRAW.attributes.Power.Value));
                    if (itemStatRAW.attributes.Precision.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.Precision, itemStatRAW.attributes.Precision.Value));
                    if (itemStatRAW.attributes.Toughness.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.Toughness, itemStatRAW.attributes.Toughness.Value));
                    if (itemStatRAW.attributes.Vitality.HasValue) newItemStat.Attributes.Add(new KeyValuePair<Attribute, double>(Attribute.Vitality, itemStatRAW.attributes.Vitality.Value));
                }

                main.ItemStats.Add(newItemStat);
                main.OnCharStatusUpdate("Generated ItemStat " + newItemStat.Name + ";" + newItemStat.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    class ItemStatRAW
    {
        public int id { get; set; }
        public string name { get; set; }
        public ItemStatSubRAW attributes { get; set; }
    }
    class ItemStatSubRAW
    {
        public double? AgonyResistance { get; set; }
        public double? BoonDuration { get; set; }
        public double? ConditionDamage { get; set; }
        public double? ConditionDuration { get; set; }
        public double? CritDamage { get; set; }
        public double? Healing { get; set; }
        public double? Power { get; set; }
        public double? Precision { get; set; }
        public double? Toughness { get; set; }
        public double? Vitality { get; set; }
    }

    class ItemRAW
    {
        public int id { get; set; }
        public string chat_link { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string rarity { get; set; }
        public int level { get; set; }
        public int vendor_value { get; set; }
        public int? default_skin { get; set; }
        public string[] flags { get; set; }
        public string[] game_types { get; set; }
        public string[] restrictions { get; set; }
        public ItemSub1RAW details { get; set; }
    }
    class ItemSub1RAW
    {
        public string type { get; set; }
        public string weight_class { get; set; }
        public int? defense { get; set; }
        public ItemSub3RAW[] infusion_slots { get; set; }
        public ItemSub2RAW infix_upgrade { get; set; }
        public int? suffix_item_id { get; set; }
        public string secondary_suffix_item_id { get; set; }
        public string[] stat_choices { get; set; }
        public int? size { get; set; }
        public bool? no_sell_or_sort { get; set; }
        public string description { get; set; }
        public int? duration_ms { get; set; }
        public string unlock_type { get; set; }
        public int? color_id { get; set; }
        public int? recipe_id { get; set; }
        public int? apply_count { get; set; }
        public string name { get; set; }
        public int?[] skins { get; set; }
        public int? minipet_id { get; set; }
        public int? charges { get; set; }
        public string[] flags { get; set; }
        public string[] infusion_upgrade_flags { get; set; }
        public string suffix { get; set; }
        public string[] bonuses { get; set; }
        public string damage_type { get; set; }
        public int? min_power { get; set; }
        public int? max_power { get; set; }
    }
    class ItemSub2RAW
    {
        public ItemSub4RAW[] attributes { get; set; }
        public ItemSub5RAW buff { get; set; }
    }
    class ItemSub3RAW
    {
        public string[] flags { get; set; }
        public int? item_id { get; set; }
    }
    class ItemSub4RAW
    {
        public string attribute { get; set; }
        public int modifier { get; set; }
    }
    class ItemSub5RAW
    {
        public int skill_id { get; set; }
        public string description { get; set; }
    }
}
