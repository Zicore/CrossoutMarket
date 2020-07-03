using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model.Items;
using Crossout.AspWeb.Helper;
using Crossout.AspWeb.Models.View;
using Crossout.AspWeb.Models.General;
using Crossout.AspWeb.Models.Items;
using Crossout.Data.Info;
using Crossout.AspWeb.Models.Changes;

namespace Crossout.AspWeb.Models.Info
{
    public class InfoModel : IViewTitle
    {
        public string Title => "Info";

        public List<LastUpdateTime> LastUpdateTimes = new List<LastUpdateTime>();

        public List<Contributor> Contributors = new List<Contributor>();

        public List<UpdateNote> UpdateNotes = new List<UpdateNote>();

        public ChangesModel ChangesModel = new ChangesModel();

        public StatusModel StatusModel = new StatusModel();
    }
}
