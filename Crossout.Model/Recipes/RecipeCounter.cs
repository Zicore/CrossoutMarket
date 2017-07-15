namespace Crossout.Model.Recipes
{
    public class RecipeCounter
    {
        public int NextId()
        {
            return uniqueIdCounter++;
        }

        public void ResetId()
        {
            uniqueIdCounter = 0;
        }

        private int uniqueIdCounter = 0;
    }
}
