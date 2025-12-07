using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CS_Selenium_SpecFlow.Core.Logging;

namespace CS_Selenium_SpecFlow.Core.Utilities;

/// <summary>
/// Utility class for reading test data from JSON files
/// </summary>
public static class TestDataReader
{
    private static readonly Dictionary<string, JObject> _dataCache = new();
    private static readonly object _lock = new();
    private static string _testDataBasePath = string.Empty;

    public static string TestDataBasePath
    {
        get
        {
            if (string.IsNullOrEmpty(_testDataBasePath))
            {
                _testDataBasePath = FindTestDataPath();
            }
            return _testDataBasePath;
        }
        set => _testDataBasePath = value;
    }

    private static string FindTestDataPath()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var testDataPath = Path.Combine(basePath, "TestData");
        
        if (Directory.Exists(testDataPath))
        {
            return testDataPath;
        }

        var projectRoot = Directory.GetCurrentDirectory();
        testDataPath = Path.Combine(projectRoot, "TestData");
        
        if (Directory.Exists(testDataPath))
        {
            return testDataPath;
        }

        throw new DirectoryNotFoundException("TestData directory not found");
    }

    /// <summary>
    /// Loads JSON test data file
    /// </summary>
    public static JObject LoadTestData(string fileName)
    {
        lock (_lock)
        {
            if (_dataCache.TryGetValue(fileName, out var cached))
            {
                return cached;
            }

            var filePath = Path.Combine(TestDataBasePath, fileName);
            
            if (!filePath.EndsWith(".json"))
            {
                filePath += ".json";
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Test data file not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            var data = JObject.Parse(json);
            
            _dataCache[fileName] = data;
            Logger.Debug($"Loaded test data: {fileName}");

            return data;
        }
    }

    /// <summary>
    /// Gets a specific value from test data
    /// </summary>
    public static T? GetValue<T>(string fileName, string jsonPath)
    {
        var data = LoadTestData(fileName);
        var token = data.SelectToken(jsonPath);
        
        if (token == null)
        {
            Logger.Warning($"Test data path not found: {jsonPath} in {fileName}");
            return default;
        }

        return token.ToObject<T>();
    }

    /// <summary>
    /// Gets an array from test data
    /// </summary>
    public static List<T> GetArray<T>(string fileName, string jsonPath)
    {
        var data = LoadTestData(fileName);
        var token = data.SelectToken(jsonPath) as JArray;
        
        if (token == null)
        {
            Logger.Warning($"Test data array not found: {jsonPath} in {fileName}");
            return new List<T>();
        }

        return token.ToObject<List<T>>() ?? new List<T>();
    }

    /// <summary>
    /// Clears the test data cache
    /// </summary>
    public static void ClearCache()
    {
        lock (_lock)
        {
            _dataCache.Clear();
            Logger.Debug("Test data cache cleared");
        }
    }
}
