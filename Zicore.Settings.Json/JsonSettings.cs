using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zicore.Settings.Json
{
    public class JsonSettings
    {
        public JsonSettings()
        {

        }

        private bool _isLoaded;
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

        public void Load(String applicationName, String fileName)
        {
            FilePath = GetFilePath(applicationName, fileName);
            LoadFrom(FilePath);
        }

        public virtual void LoadFrom(string path)
        {
            IsLoaded = false;
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

        public static String GetFilePath(String applicationName, String fileName)
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

        public static String GetFolderPath(String applicationName, String folderName)
        {
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String folderPath = Path.Combine(appDataPath, applicationName, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }
        
        public static bool Exists(String filePath)
        {
            return File.Exists(filePath);
        }

        public virtual void Save()
        {
            String result = JsonConvert.SerializeObject(this);
            var data = Encoding.UTF8.GetBytes(result);
            data = SaveFilter(data);
            File.WriteAllBytes(FilePath, data);
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
