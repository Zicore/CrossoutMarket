using Crossout.AspWeb.Models.Info;
using Crossout.Data;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crossout.AspWeb.Services
{
    public class ResourceService
    {
        public ContributorCollection ContributorCollection = new ContributorCollection();
        public UpdateNoteCollection UpdateNoteCollection = new UpdateNoteCollection();

        public static void Initialize(IWebHostEnvironment webHostEnvironment)
        {
            _instance = new ResourceService();
            _instance.LoadFiles(webHostEnvironment);
        }

        private void LoadFiles(IWebHostEnvironment webHostEnvironment)
        {
            var rootPath = webHostEnvironment.ContentRootPath;

            ContributorCollection.ReadJsonResource(Path.Combine(rootPath, WebSettings.Settings.FileContributors));
            UpdateNoteCollection.ReadJsonResource(Path.Combine(rootPath, WebSettings.Settings.FileUpdateNotes));
        }

        private static ResourceService _instance;

        public static ResourceService Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
