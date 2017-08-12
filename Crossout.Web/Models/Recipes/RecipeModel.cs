using Crossout.Model.Recipes;
using Newtonsoft.Json;

namespace Crossout.Web.Models.Recipes
{
    [JsonObject("recipe")]
    public class RecipeModel
    {
        [JsonProperty("recipe")]
        public RecipeItem Recipe { get; set; }
    }
}
