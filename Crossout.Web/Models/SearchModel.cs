﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.Web.Models.Filter;
using Crossout.Web.Models.Pagination;

namespace Crossout.Web.Models
{
    public class SearchModel
    {
        public List<Item> SearchResult { get; set; }
        public PagerModel Pager { get; set; } = new PagerModel();
        public FilterModel FilterModel { get; set; } = new FilterModel();
        public String CurrentQuery { get; set; } = "";
        public StatusModel Status { get; set; } = new StatusModel();

        public string UriFaction(string faction)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={faction}";
        }

        public string UriCategory(string category)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={category}&faction={FilterModel.CurrentFaction.NameUri}";
        }

        public string UriRarity(string rarity)
        {
            return $"{Pager.CurrentPage}/?query={CurrentQuery}&rarity={rarity}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}";
        }

        public string UriSearch(string search)
        {
            return $"{Pager.CurrentPage}/?query={search}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}";
        }

        public string UriPage(int page)
        {
            return $"{page}/?query={CurrentQuery}&rarity={FilterModel.CurrentRarity.NameUri}&category={FilterModel.CurrentCategory.NameUri}&faction={FilterModel.CurrentFaction.NameUri}";
        }
    }
}