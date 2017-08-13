using System.Collections.Generic;
using Crossout.Model;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Web.Models.General;
using Crossout.Web.Models.Recipes;
using Newtonsoft.Json;

namespace Crossout.Web.Models.Items
{
    public class ItemModel : IViewTitle
    {
        [JsonProperty("item")]
        public Item Item { get; set; } = new Item() {};

        [JsonProperty("recipe")]
        public RecipeModel Recipe { get; set; } = new RecipeModel();

        [JsonIgnore]
        public StatusModel Status { get; set; } = new StatusModel();

        [JsonIgnore]
        public List<Item> AllItems { get; set; } = new List<Item>();

        [JsonIgnore]
        public List<FactionModel> AllFactions { get; set; } = new List<FactionModel>();

        [JsonIgnore]
        public FactionModel SelectedFaction { get; set; } = new FactionModel();

        [JsonIgnore]
        public string Title => Item.Name;
    }
}
