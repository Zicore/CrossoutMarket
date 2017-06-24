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
        public Item Item { get; set; } = new Item() {};
        public RecipeModel Recipe { get; set; } = new RecipeModel();
        public StatusModel Status { get; set; } = new StatusModel();
        
        public List<Item> AllItems { get; set; } = new List<Item>();
        public List<FactionModel> AllFactions { get; set; } = new List<FactionModel>();
        public FactionModel SelectedFaction { get; set; } = new FactionModel();

        [JsonIgnore]
        public string Title => Item.Name;
    }
}
