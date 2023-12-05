using System.Reflection;
using System.Text.Json;
using Config.DataModels;

namespace Config
{
    /// <summary>
    /// 配置文件类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 配置文件完整路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 目标程序集
        /// </summary>
        private Assembly Assembly;

        /// <summary>
        /// 配置文件数据
        /// </summary>
        private Dictionary<string, Dictionary<string, ConfigDataType>> _Data = new();

        /// <summary>
        /// 引索器
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <param name="key">目标键</param>
        /// <returns></returns>
        public dynamic? this[string section, string key]
        {
            get => GetDynamicValue(section, key);
            set => SetDynamicValue(section, key, value);
        }
        /// <summary>
        /// 引索器 分区
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <returns></returns>
        public dynamic? this[string section]
        {
            get => GetSection(section);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_Assembly">目标程序集</param>
        /// <param name="path">配置文件路径</param>
        public ConfigHelper(Assembly _Assembly, string path = "Config.json")
        {
            Path = System.IO.Path.GetFullPath(path);
            Assembly = _Assembly;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_Assembly">目标程序集</param>
        /// <param name="LondResult">加载配置文件结果</param>
        /// <param name="path">配置文件路径</param>
        public ConfigHelper(Assembly _Assembly, out bool LondResult, string path = "Config.json")
            : this(_Assembly,path)
        {
            LondResult = FileLondAsync().Result;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FileLondAsync()
        {
            try
            {
                string fileContents = await File.ReadAllTextAsync(Path);
                using (JsonDocument document = JsonDocument.Parse(fileContents))
                {
                    _Data = document.Deserialize<Dictionary<string, Dictionary<string, ConfigDataType>>>()!;

                    // 将JSON字符串还原为对象
                    foreach (var section in _Data.Values)
                    {
                        foreach (var data in section.Values)
                        {
                            if (data.Value is System.Text.Json.JsonElement json && data.TypeName != null)
                            {
                                Type? objectType = this.Assembly.GetType(data.TypeName);

                                if (objectType != null)
                                {
                                    var deserializedObject = JsonSerializer.Deserialize(json, objectType);
                                    if (deserializedObject != null)
                                    {
                                        data.Value = deserializedObject;
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // 根据需要记录或处理异常
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FileSaveAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                string json = JsonSerializer.Serialize(_Data, options);
                await File.WriteAllTextAsync(Path, json);

                return true;
            }
            catch (Exception ex)
            {
                // 根据需要记录或处理异常
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 设置配置文件值
        /// </summary>
        /// <typeparam name="T">可以保存任意变量类型</typeparam>
        /// <param name="section">分区名</param>
        /// <param name="key">目标键</param>
        /// <param name="value">保存值</param>
        public void SetValue<T>(string section, string key, T value)
        {
            _Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection);
            existingSection ??= new Dictionary<string, ConfigDataType>();
            existingSection[key] = new ConfigDataType
            {
                TypeName = typeof(T).FullName,
                Value = value
            };
            _Data[section] = existingSection;
        }

        /// <summary>
        /// 设置配置文件值
        /// </summary>
        /// <param name="section">分区名</param>
        /// <param name="key">目标键</param>
        /// <param name="value">保存值</param>
        public void SetDynamicValue(string section, string key, dynamic value)
        {
            _Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection);
            existingSection ??= new Dictionary<string, ConfigDataType>();
            existingSection[key] = new ConfigDataType
            {
                TypeName = value.GetType().FullName,
                Value = value
            };
            _Data[section] = existingSection;
        }

        /// <summary>
        /// 删除分区
        /// </summary>
        /// <param name="section">目标分区名</param>
        public void RemoveSection(string section)
        {
            _Data.Remove(section);
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="section">目标分区名</param>
        /// <param name="key">目标键名</param>
        public void RemoveKey(string section, string key)
        {
            if (_Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection))
            {
                existingSection.Remove(key);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">目标值类型</typeparam>
        /// <param name="section">目标分区名</param>
        /// <param name="key">目标键名</param>
        /// <returns></returns>
        public T? GetValue<T>(string section, string key)
        {
            if (_Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection))
            {
                if (existingSection.TryGetValue(key, out ConfigDataType? existingData) && existingData.TypeName == typeof(T).FullName)
                {
                    return (T?)existingData.Value;
                }
            }
            return default;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <typeparam name="T">目标值类型</typeparam>
        /// <param name="section">目标分区名</param>
        /// <param name="key">目标键名</param>
        /// <param name="value">输出值</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetValue<T>(string section, string key, out T? value)
        {
            value = GetValue<T>(section, key);
            return value != null;
        }

        /// <summary>
        /// 获取动态值
        /// </summary>
        /// <param name="section">目标分区名</param>
        /// <param name="key">目标键名</param>
        /// <returns>返回值</returns>
        public dynamic? GetDynamicValue(string section, string key)
        {
            if (_Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection))
            {
                if (existingSection.TryGetValue(key, out ConfigDataType? existingData))
                {
                    return existingData.Value;
                }
            }
            return default;
        }

        /// <summary>
        /// 获取分区下所有数据
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <returns>分区数据</returns>
        public Dictionary<string, Dictionary<string, ConfigDataType>> GetSection(string section)
        {
            if (_Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection))
            {
                Dictionary<string, Dictionary<string, ConfigDataType>> result = new();
                foreach (var item in existingSection)
                {
                    result.Add(item.Key, item.Value.DynamicValue());
                }
                return result;
            }
            return new();
        }

        /// <summary>
        /// 尝试获取分区下所有数据
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <param name="result">输出数据</param>
        /// <returns></returns>
        public bool TryGetSection(string section, out Dictionary<string, Dictionary<string, ConfigDataType>> result)
        {
            if (_Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection))
            {
                result = new();
                foreach (var item in existingSection)
                {
                    result.Add(item.Key, item.Value.DynamicValue());
                }
                return true;
            }
            result = new();
            return false;
        }

        /// <summary>
        /// 检查分区是否存在
        /// </summary>
        /// <param name="section">目标分区名</param>
        /// <returns></returns>
        public bool CheckSection(string section)
        {
            return _Data.ContainsKey(section);
        }

        /// <summary>
        /// 检查键是否存在
        /// </summary>
        /// <param name="section">目标分区名</param>
        /// <param name="key">目标分键</param>
        /// <returns></returns>
        public bool CheckKey(string section, string key)
        {
            return _Data.TryGetValue(section, out Dictionary<string, ConfigDataType>? existingSection) && existingSection.ContainsKey(key);
        }

        /// <summary>
        /// 序列化为JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(_Data);
        }
    }
}