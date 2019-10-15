using System;

namespace Crossout.Web.Models.Pagination
{
    public class PagerModel
    {
        public void CalculatePage(int pages)
        {
            MinPage = Math.Max(CurrentPage - pages, 1);
            MaxPage = Math.Max(Math.Min(CurrentPage + pages, MaxPages), MinPage + pages);
        }

        public int CurrentPage { get; set; }
        public int MaxPages { get; set; }
        public int MaxRows { get; set; }
        public int MinPage { get; set; }
        public int MaxPage { get; set; }
    }
}
