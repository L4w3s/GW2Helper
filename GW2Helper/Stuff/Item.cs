using System;
using System.Collections.Generic;
using System.Linq;
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
            Minipet,
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
            SoulbindOnAquire,
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
            Warrior
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
        public List<Flag> Flags { get; set; }
        public List<GameType> GameTypes { get; set; }
        public List<Restriction> Restrictions { get; set; }
        public ItemDetail Details { get; set; }

        public static Item GetItemFromJSON(string json, Main main)
        {
            Item newItem = new Item { };
            return newItem;
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
        public List<ItemStat> StatChoices { get; set; }
    }
    public class BackItemDetail : ItemDetail
    {
        public List<Infusion> Infusions { get; set; }
        public InfixUpgrade InfixUpgrades { get; set; }
        public Item SuffixItem { get; set; }
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
            Food,
            Generic,
            Halloween,
            Immediate,
            Transmutation,
            Unlock,
            UpgradeRemoval,
            Utility,
            TeleportToFriend
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
            RandomUlock
        }
        public ConsumableType ConsType { get; set; }
        public string Description { get; set; }
        public int? DurationInMilliseconds { get; set; }
        public UnlockType UnType { get; set; }
        public Colour ConsumableColour { get; set; }
        public Recipe ConsumableRecipe { get; set; }
        public int? StackCount { get; set; }
        public string Name { get; set; }
        public List<Skin> Skins { get; set; }
    }
    public class ContainerItemDetail : ItemDetail
    {
        public enum ContainerType
        {
            Default,
            GiftBox,
            OpenUI
        }
        public ContainerType ContType { get; set; }
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
        public KeyValuePair<Attribute, double> Attributes { get; set; }
    }
}
