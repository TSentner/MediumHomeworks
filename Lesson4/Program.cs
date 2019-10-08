using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace Lesson4
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = "log.txt";

            var configPack1 = Parser.Deserialize(path + "\\" + file);
            if (configPack1 == null)
                configPack1 = new ConfigPack(1, 10f, 1, "string");

            var fileWatcher = new FileWatcher(path, file);
            configPack1.Subscribe(fileWatcher);

            var configuration = new Configurator(configPack1);
            
            fileWatcher.Start();

            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;

            fileWatcher.Stop();
            configPack1.Unsubscribe(fileWatcher);

        }
    }

    public interface IConfigObservable
    {
        void AddListener(Action<ConfigPack> action);
        void RemoveListener(Action<ConfigPack> action);
    }

    public static class Parser
    {
        public static bool Serialize(string filePath, ConfigPack config)
        {
            bool isSerialized = true;
            FileStream fileStream = null;
            XmlDictionaryWriter json = null;

            try
            { 
                var serializer = new DataContractJsonSerializer(typeof(ConfigPack));
                fileStream = new FileStream(filePath, FileMode.Create);
                json = JsonReaderWriterFactory.CreateJsonWriter(fileStream,
                        Encoding.GetEncoding("utf-8"));
                serializer.WriteObject(json, config);
                json.Flush();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                isSerialized = false;
            }
            finally
            {
                json?.Dispose();
                fileStream?.Dispose();
            }

            return isSerialized;
        }

        public static ConfigPack Deserialize(string filePath)
        {
            ConfigPack config;
            FileStream fileStream = null;
            XmlDictionaryReader json = null;

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(ConfigPack));
                fileStream = new FileStream(filePath, FileMode.Open);
                json = JsonReaderWriterFactory.CreateJsonReader(fileStream, 
                        Encoding.GetEncoding("utf-8"), XmlDictionaryReaderQuotas.Max, null);

                config = (ConfigPack)serializer.ReadObject(json);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                config = null;
            }
            finally
            {
                json?.Dispose();
                fileStream?.Dispose();
            }

            return config;
        }
    }

    public class FileWatcher : IConfigObservable
    {
        private string _filePath;
        private FileSystemWatcher _watcher;

        public FileWatcher(string path, string file)
        {
            _filePath = path + "\\" + file;
            if (File.Exists(_filePath) == false)
                throw new FileNotFoundException();

            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Filter = file;
            _watcher.Changed += OnChanged;
        }

        private Action<ConfigPack> _onUpdateConfig;

        public void AddListener(Action<ConfigPack> action) => _onUpdateConfig += action;

        public void RemoveListener(Action<ConfigPack> action) => _onUpdateConfig -= action;

        public void Start() => _watcher.EnableRaisingEvents = true;

        public void Stop() => _watcher.EnableRaisingEvents = false;

        private void OnChanged(object source, FileSystemEventArgs eventArgs)
        {
            Console.WriteLine("File changed.");
            ConfigPack configPack = Parser.Deserialize(_filePath);
            if (configPack != null)
                _onUpdateConfig?.Invoke(configPack);
        }

    }

    public class Configurator
    {
        public Configurator(ConfigPack config)
        {
            config.AddListener(Configurate);
            Configurate(config);
        }

        public void Configurate(ConfigPack config)
        {
            Console.WriteLine($"{config.Config1} {config.Config2} {config.Config3} {config.Config4}");
        }

        public void Unsubscribe(ConfigPack config) => config.RemoveListener(Configurate);
    }

    [DataContract]
    public class ConfigPack : IConfigObservable
    {
        private Action<ConfigPack> _onUpdateConfig;

        [DataMember] public int Config1 { get; private set; }
        [DataMember] public float Config2 { get; private set; }
        [DataMember] public int Config3 { get; private set; }
        [DataMember] public string Config4 { get; private set; }

       
        public ConfigPack(int config1, float config2, int config3, string config4)
        {
            Config1 = config1;
            Config2 = config2;
            Config3 = config3;
            Config4 = config4;
        }

        private void Update(ConfigPack configPack)
        {
            Config1 = configPack.Config1;
            Config2 = configPack.Config2;
            Config3 = configPack.Config3;
            Config4 = configPack.Config4;

            _onUpdateConfig.Invoke(this);
        }

        public void Subscribe(IConfigObservable observable) => observable.AddListener(Update);
        public void Unsubscribe(IConfigObservable observable) => observable.RemoveListener(Update);

        public void AddListener(Action<ConfigPack> action) => _onUpdateConfig += action;
        public void RemoveListener(Action<ConfigPack> action) => _onUpdateConfig -= action;
    }
}
