using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crossout.Model;
using Newtonsoft.Json;

namespace Crossout.AspWeb.Models.Language
{
    public class LanguageModel
    {
        [JsonProperty("availableLanguages")]
        public List<Language> AvailableLanguages { get; set; } = new List<Language>();

        [JsonProperty("defaultLanguages")]
        public Language DefaultLanguage { get { return AvailableLanguages.Find(x => x.Id == 1); } }

        public Language VerifyLanguage(string twoLetterISOName)
        {
            Language result = AvailableLanguages.FirstOrDefault(x => x.TwoLetterISOName == twoLetterISOName);
            if (result != null)
                return result;
            else
                return DefaultLanguage;
        }
        public Language VerifyLanguage(int id)
        {
            Language result = AvailableLanguages.FirstOrDefault(x => x.Id == id);
            if (result != null)
                return result;
            else
                return DefaultLanguage;
        }

        public void Create(List<object[]> ds)
        {
            foreach (var row in ds)
            {
                int i = 0;
                var lang = new Language
                {
                    Id = row[i++].ConvertTo<int>(),
                    Name = row[i++].ConvertTo<string>(),
                    TwoLetterISOName = row[i++].ConvertTo<string>()
                };
                AvailableLanguages.Add(lang);
            }
        }
    }
}
