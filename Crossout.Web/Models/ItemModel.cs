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
    }
}
