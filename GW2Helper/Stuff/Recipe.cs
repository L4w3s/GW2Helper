using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GW2Helper.Stuff
{
    public class Recipe
    {
        public enum Type
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
            Boots,
            Coats,
            Gloves,
            Helm,
            Leggings,
            Shoulders,
            Amulet,
            Earring,
            Ring,
            Dessert,
            Feast,
            IngredientCooking,
            Meal,
            Seasoning,
            Snack,
            Soup,
            Food,
            Component,
            Inscription,
            Insignia,
            LegendaryComponent,
            Refinement,
            RefinementEctoplasm,
            RefinementObsidian,
            GuildConsumable,
            GuildDecoration,
            GuildConsumableWvw,
            Backpack,
            Bag,
            Bulk,
            Consumable,
            Dye,
            Potion,
            UpgradeComponent,
            Coat
        }
        public enum Discipline
        {
            Artificer,
            Armorsmith,
            Chef,
            Huntsman,
            Jeweler,
            Leatherworker,
            Tailor,
            Weaponsmith,
            Scribe
        };
        public enum Flag
        {
            AutoLearned,
            LearnedFromItem
        }
        public int ID { get; set; }
        public Type RecipeType { get; set; }
        public Item ResultItem { get; set; }
        public int? ItemID { get; set; }
        public int ResultCount { get; set; }
        public int MSCraftTime { get; set; }
        public List<Discipline> Disciplines { get; set; }
        public int MinRating { get; set; }
        public List<Flag> Flags { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<GuildIngredient> GuildIngredients { get; set; }
        public GuildUpgrade ResultUpgrade { get; set; }
        public int? UpgradeID { get; set; }
        
        public static void GetRecipesFromJSON(string json, Main main)
        {
            RecipeRAW[] rawRecipes = new RecipeRAW[1];
            try
            {
                rawRecipes = JsonConvert.DeserializeObject<RecipeRAW[]>(json);
            }
            catch (Exception e)
            {
                rawRecipes[0] = JsonConvert.DeserializeObject<RecipeRAW>(json);
            }
            for (int r = 0; r < rawRecipes.Length; r++)
            {
                double cur = r, max = rawRecipes.Length;
                RecipeRAW recipeRAW = rawRecipes[r];
                main.JSON.Add(new KeyValuePair<string, string>("Recipe", JsonConvert.SerializeObject(recipeRAW)));
                Recipe newRecipe = new Recipe
                {
                    ID = recipeRAW.id,
                    RecipeType = (Type)Enum.Parse(typeof(Type), recipeRAW.type),
                    ResultCount = (recipeRAW.output_item_count.HasValue) ? recipeRAW.output_item_count.Value : 0,
                    MSCraftTime = recipeRAW.time_to_craft_ms,
                    MinRating = recipeRAW.min_rating,
                    Disciplines = new List<Discipline>(),
                    Flags = new List<Flag>(),
                    Ingredients = new List<Ingredient>(),
                    GuildIngredients = new List<GuildIngredient>()
                };

                for (int i = 0; i < recipeRAW.disciplines.Length; i++)
                {
                    newRecipe.Disciplines.Add((Discipline)Enum.Parse(typeof(Discipline), recipeRAW.disciplines[i]));
                }
                for (int i = 0; i < recipeRAW.flags.Length; i++)
                {
                    newRecipe.Flags.Add((Flag)Enum.Parse(typeof(Flag), recipeRAW.flags[i]));
                }

                if (recipeRAW.output_item_id.HasValue)
                {
                    newRecipe.ItemID = recipeRAW.output_item_id.Value;
                }
                if (recipeRAW.output_upgrade_id.HasValue)
                {
                    newRecipe.UpgradeID = recipeRAW.output_upgrade_id.Value;
                }

                if (recipeRAW.ingredients != null)
                {
                    for (int i = 0; i < recipeRAW.ingredients.Length; i++)
                    {
                        Ingredient newIngredient = new Ingredient
                        {
                            Count = recipeRAW.ingredients[i].count
                        };

                        newIngredient.ItemID = recipeRAW.ingredients[i].item_id;

                        newRecipe.Ingredients.Add(newIngredient);
                    }
                }

                if (recipeRAW.guild_ingredients != null)
                {
                    for (int i = 0; i < recipeRAW.guild_ingredients.Length; i++)
                    {
                        GuildIngredient newIngredient = new GuildIngredient
                        {
                            Count = recipeRAW.guild_ingredients[i].count
                        };

                        newIngredient.UpgradeID = recipeRAW.guild_ingredients[i].upgrade_id;

                        newRecipe.GuildIngredients.Add(newIngredient);
                    }
                }

                main.Recipes.Add(newRecipe);
                main.OnCharStatusUpdate("Generated Recipe;" + newRecipe.ID + " " + ((cur != 0) ? Math.Round((double)(cur / max), 2) * 100 : 0).ToString() + "%");
            }
        }
    }
    public class Ingredient
    {
        public Item IngredientItem { get; set; }
        public int ItemID { get; set; }
        public int Count { get; set; }
    }
    public class GuildIngredient
    {
        public GuildUpgrade IngredientUpgrade { get; set; }
        public int UpgradeID { get; set; }
        public int Count { get; set; }
    }

    class RecipeRAW
    {
        public int id { get; set; }
        public string type { get; set; }
        public int? output_item_id { get; set; }
        public int? output_item_count { get; set; }
        public int time_to_craft_ms { get; set; }
        public string[] disciplines { get; set; }
        public int min_rating { get; set; }
        public string[] flags { get; set; }
        public RecipeSub1RAW[] ingredients { get; set; }
        public RecipeSub2RAW[] guild_ingredients { get; set; }
        public int? output_upgrade_id { get; set; }
        public string chat_link { get; set; }
    }
    class RecipeSub1RAW
    {
        public int item_id { get; set; }
        public int count { get; set; }
    }
    class RecipeSub2RAW
    {
        public int upgrade_id { get; set; }
        public int count { get; set; }
    }
}