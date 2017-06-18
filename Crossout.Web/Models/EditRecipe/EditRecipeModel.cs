using Crossout.Model.Recipes;

namespace Crossout.Web.Models.EditRecipe
{
    public class EditRecipeModel
    {
        public ItemModel ItemModel { get; set; }
        public RecipeItem RecipeItem { get; set; }
        public int Index { get; set; }
    }
}
