using System;
using System.Collections.Generic;
using Crossout.Model.Items;
using Crossout.Web.Helper;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.Pagination;

namespace Crossout.Web.Models.General
{
    public class SearchModel : IViewTitle
    {
        public List<Item> SearchResult { get; set; }
        public PagerModel Pager { get; set; } = new PagerModel();
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public String CurrentQuery { get; set; } = "";
        public StatusModel Status { get; set; } = new StatusModel();

        public string UriFaction(string faction)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={faction}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriCategory(string category)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={category}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriRarity(string rarity)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={rarity}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriSearch(string search)
        {
            return $"{Pager.CurrentPage}/?query={search}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriPage(int page)
        {
            return $"{page}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriRemovedItems(bool showRemovedItems)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={!showRemovedItems}&mitems={FilterModel.CurrentShowMetaItems}";
        }

        public string UriMetaItems(bool showMetaItems)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}&rmditems={FilterModel.CurrentShowRemovedItems}&mitems={!showMetaItems}";
        }

        public string Title => null;
    }
}