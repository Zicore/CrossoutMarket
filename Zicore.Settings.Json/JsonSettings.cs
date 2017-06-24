using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zicore.Settings.Json
{
    public class JsonSerializable
    {
        public JsonSerializable()
        {

        }

        private bool _isLoaded;

        [JsonIgnore]
        public bool IsLoaded
        {
            get { return _isLoaded; }
            protected set { _isLoaded = value; }
        }

        private String _filePath;

        [JsonIgnore]
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        [JsonIgnore]
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        [JsonIgnore]
        public bool CreateSubdirectoryIfItNotExists { get; set; } = false;

        public void LoadFromAppData(String fileName, String applicationName)
        {
            FilePath = GetAppDataFilePath(fileName, applicationName);
            LoadFrom(FilePath);
        }

        public void LoadFromApplicationDirectory(String filename, String subdirectory = "")
        {
            FilePath = GetApplicationDirectoryFilePath(filename, subdirectory);
            LoadFrom(FilePath);
        }

        public static T Load<T>(String path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            byte[] data = File.ReadAllBytes(path);

            using (var sr = new StreamReader(new MemoryStream(data)))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    var jObject = serializer.Deserialize(jsonTextReader, typeof(T));
                    return (T)jObject;
                }
            }
        }

        public virtual void LoadFrom(string path)
        {
            IsLoaded = false;

            CreateSubDirectorie(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            byte[] data = File.ReadAllBytes(path);
            data = LoadFilter(data);

            using (var sr = new StreamReader(new MemoryStream(data)))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    var jObject = (JObject)serializer.Deserialize(jsonTextReader);
                    Type type = this.GetType();
                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            var value = jObject.GetValue(property.Name);
                            if (value != null)
                            {
                                var result = value.ToObject(property.PropertyType);
                                property.SetValue(this, result, null);
                            }
                        }
                        catch
                        {
                            Debug.Write("Deserialization failed");
                        }
                    }
                }
            }

            IsLoaded = true;
        }

        private void CreateSubDirectorie(String filePath)
        {
            if (CreateSubdirectoryIfItNotExists)
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.DirectoryName != null && !Directory.Exists(fi.DirectoryName))
                {
                    Directory.CreateDirectory(fi.DirectoryName);
                }
            }
        }

        public static String GetAppDataFilePath(String fileName, String applicationName)
        {
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String folderPath = Path.Combine(appDataPath, applicationName);
            String filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return filePath;
        }

        public static String GetApplicationDirectoryFilePath(String fileName, String subdirectory = "")
        {
            var applicationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subdirectory, fileName);
            return applicationFolder;
        }

        public static bool Exists(String filePath)
        {
            return File.Exists(filePath);
        }

        public virtual void SaveToAppData(String fileName, String applicationName)
        {
            FilePath = GetAppDataFilePath(fileName, applicationName);
            Save(FilePath);
        }

        public void SaveToApplicationDirectory(String fileName, String subdirectory = "")
        {
            FilePath = GetApplicationDirectoryFilePath(fileName, subdirectory);
            Save(FilePath);
        }

        public virtual void Save(String path)
        {
            CreateSubDirectorie(path);
            String result = JsonConvert.SerializeObject(this, JsonSerializerSettings);
            var data = Encoding.UTF8.GetBytes(result);
            data = SaveFilter(data);
            File.WriteAllBytes(path, data);
        }

        protected virtual byte[] LoadFilter(byte[] data)
        {
            return data; // No filter in base class
        }

        protected virtual byte[] SaveFilter(byte[] data)
        {
            return data; // No filter in base class
        }
    }
}
