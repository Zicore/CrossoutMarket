using Crossout.Model.Recipes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Crossout.AspWeb.Models.Recipes
{
    public class IngredientUsageModel
    {
        public List<IngredientUsageItem> IngredientUsageList { get; set; } = new List<IngredientUsageItem>();
    }
}
