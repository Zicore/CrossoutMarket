using Crossout.AspWeb.Pocos;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Helper.TagHelpers
{
    [HtmlTargetElement("loc")]
    public class LocTagHelper : TagHelper
    {
        [HtmlAttributeName("model")]
        public List<FrontendLocalizationPoco> LocModel { get; set; }

        [HtmlAttributeName("category")]
        public string Category { get; set; }

        [HtmlAttributeName("name")]
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            if (LocModel != null)
            {
                var loc = LocModel.FirstOrDefault(x => x.Category == Category && x.Name == Name);

                if (loc != null && loc.Localization != string.Empty)
                {
                    output.Content.SetContent(loc.Localization);
                    output.AddClass("localized", HtmlEncoder.Default);
                }
                else
                {
                    output.AddClass("needs-localization", HtmlEncoder.Default);
                }
            }

            output.AddClass("localization", HtmlEncoder.Default);
            output.Attributes.Add("data-locname", Category + "." + Name);
        }
    }
}
