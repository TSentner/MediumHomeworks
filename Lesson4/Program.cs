using System;
using System.IO;
using System.Reflection;
using System.Text;
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

            var configsLoader = new ConfigsLoader(path + "\\" + file);

            var configuration = new Configurator(configsLoader.ConfigPack);
            configsLoader.Updated += configuration.Configurate;

            configsLoader.Watch();

            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;

            configsLoader.StopWatch();
        }
    }

    public class Configurator
    {
        public Configurator(ConfigPack config)
        {
            Configurate(config);
        }

        public void Configurate(ConfigPack config)
        {
            Console.WriteLine($"{config.Config1} {config.Config2} {config.Config3} {config.Config4}");
        }
    }

    public class ConfigsLoader
    {
        private string _filePath;
        private FileWatcher _fileWatcher;

        public ConfigsLoader(string filePath)
        {
            _filePath = filePath;
            ConfigPack = Parser.Deserialize(filePath) ?? new ConfigPack(1, 10f, 1, "string");
        }

        public event Action<ConfigPack> Updated;

        public ConfigPack ConfigPack { get; private set; }

        public FileWatcher FileWatcher
        {
            get
            {
                if (_fileWatcher == null)
                {
                    _fileWatcher = new FileWatcher(Path.GetDirectoryName(_filePath), Path.GetFileName(_filePath));
                    FileWatcher.FileUpdate += Load;
                }

                return _fileWatcher;
            }
        }

        private void Load()
        {
            var configs = Parser.Deserialize(_filePath);
            if (configs != null)
            {
                ConfigPack = (ConfigPack)configs;
                Updated?.Invoke(ConfigPack);
            }
        }

        public void Watch() => FileWatcher.Start();
        public void StopWatch() => FileWatcher.Stop();
    }

    [DataContract]
    public struct ConfigPack
    {
        public ConfigPack(int config1, float config2, int config3, string config4)
        {
            Config1 = config1;
            Config2 = config2;
            Config3 = config3;
            Config4 = config4;
        }

        [DataMember] public int Config1 { get; private set; }
        [DataMember] public float Config2 { get; private set; }
        [DataMember] public int Config3 { get; private set; }
        [DataMember] public string Config4 { get; private set; }
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

        public static ConfigPack? Deserialize(string filePath)
        {
            ConfigPack? config = null;
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
            }
            finally
            {
                json?.Dispose();
                fileStream?.Dispose();
            }

            return config;
        }
    }

    public class FileWatcher
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

        public event Action FileUpdate;

        public void Start() => _watcher.EnableRaisingEvents = true;

        public void Stop() => _watcher.EnableRaisingEvents = false;

        private void OnChanged(object source, FileSystemEventArgs eventArgs)
        {
            Console.WriteLine("File changed.");
            FileUpdate?.Invoke();
        }
    }
}
