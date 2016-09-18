using System.Collections.Generic;
using Crossout.Model.Items;

namespace Crossout.Web.Models
{
    public class RecipeModel
    {
        public RecipeModel Parent { get; set; }
        public ItemCollection Items { get; set; } = new ItemCollection();
        public ItemModel ItemModel { get; set; } = new ItemModel();
    }
}
