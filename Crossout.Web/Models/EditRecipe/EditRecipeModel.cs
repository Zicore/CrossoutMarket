using Crossout.Model.Recipes;
using Crossout.Web.Models.Items;

namespace Crossout.Web.Models.EditRecipe
{
    public class EditRecipeModel
    {
        public ItemModel ItemModel { get; set; }
        public RecipeItem RecipeItem { get; set; }
        public int Index { get; set; }
    }
}
