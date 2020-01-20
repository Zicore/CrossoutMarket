using Crossout.Model.Recipes;
using Crossout.AspWeb.Models.Items;

namespace Crossout.AspWeb.Models.EditRecipe
{
    public class EditRecipeModel
    {
        public ItemModel ItemModel { get; set; }
        public RecipeItem RecipeItem { get; set; }
        public int Index { get; set; }
    }
}
