using System.Collections.Generic;
using Crossout.Model;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.Changes;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Recipes;
using Crossout.AspWeb.Models.View;
using Newtonsoft.Json;

namespace Crossout.AspWeb.Models.Items
{
    public class ItemModel : BaseViewModel, IViewTitle
    {
        [JsonProperty("item")]
        public Item Item { get; set; } = new Item() {};

        [JsonProperty("recipe")]
        public RecipeModel Recipe { get; set; } = new RecipeModel();

        [JsonIgnore]
        public StatusModel Status { get; set; } = new StatusModel();

        [JsonIgnore]
        public ChangesModel Changes { get; set; } = new ChangesModel();

        [JsonIgnore]
        public IngredientUsageModel IngredientUsage { get; set; } = new IngredientUsageModel();

        [JsonIgnore]
        public List<Item> AllItems { get; set; } = new List<Item>();

        [JsonIgnore]
        public List<FactionModel> AllFactions { get; set; } = new List<FactionModel>();

        [JsonIgnore]
        public Dictionary<int, string> AllRarities { get; set; } = new Dictionary<int, string>();

        [JsonIgnore]
        public Dictionary<int, string> AllCategories { get; set; } = new Dictionary<int, string>();

        [JsonIgnore]
        public Dictionary<int, string> AllTypes { get; set; } = new Dictionary<int, string>();

        [JsonIgnore]
        public FactionModel SelectedFaction { get; set; } = new FactionModel();

        [JsonIgnore]
        public string Title => Item.Name;
    }
}
