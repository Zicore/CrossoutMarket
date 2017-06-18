using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;

namespace Crossout.Web.Models
{
    public class ItemModel
    {
        public Item Item { get; set; } = new Item() {};
        public RecipeModel Recipe { get; set; } = new RecipeModel();
        public StatusModel Status { get; set; } = new StatusModel();
        
        public List<Item> AllItems { get; set; } = new List<Item>();
        public List<FactionModel> AllFactions { get; set; } = new List<FactionModel>();
        public FactionModel SelectedFaction { get; set; } = new FactionModel();
    }
}
