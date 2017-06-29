using System.Collections.Generic;

namespace Crossout.Model.Items
{
    public class ItemCollection
    {
        public List<Item> Items { get; set; } = new List<Item>();

        public override string ToString()
        {
            return $"{nameof(Items)}: {Items}";
        }
    }
}
