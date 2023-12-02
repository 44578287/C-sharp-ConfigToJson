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
        /// 配置文件数据
        /// </summary>
        private Dictionary<string, object> _Data = new();

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
        /// <param name="path">配置文件路径</param>
        public ConfigHelper(string path = "Config.json")
        {
            Path = System.IO.Path.GetFullPath(path);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LondResult">加载配置文件结果</param>
        /// <param name="path">配置文件路径</param>
        public ConfigHelper(out bool LondResult, string path = "Config.json")
        {
            Path = System.IO.Path.GetFullPath(path);
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
                using (StreamReader sr = new StreamReader(Path))
                {
                    string fileContents = await sr.ReadToEndAsync();
                    _Data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(fileContents)!;
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
                using (var sw = new StreamWriter(Path))
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };
                    string json = JsonSerializer.Serialize(_Data, options);
                    await sw.WriteAsync(json);
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
        /// 设置配置文件值
        /// </summary>
        /// <typeparam name="T">可以保存任意变量类型</typeparam>
        /// <param name="section">分区名</param>
        /// <param name="key">目标键</param>
        /// <param name="value">保存值</param>
        public void SetValue<T>(string section, string key, T value)
        {
            ConfigDataType InData = new()
            {
                TypeName = typeof(T).FullName,
                Value = value
            };

            if (!_Data.TryGetValue(section, out object? existingSection))
            {
                _Data.Add(section, new Dictionary<string, ConfigDataType> { { key, InData } });
            }
            else if (existingSection is Dictionary<string, ConfigDataType> Data)
            {
                Data[key] = InData; // 这样可以处理密钥已存在或不存在的两种情况
            }
        }

        /// <summary>
        /// 设置配置文件值
        /// </summary>
        /// <param name="section">分区名</param>
        /// <param name="key">目标键</param>
        /// <param name="value">保存值</param>
        public void SetDynamicValue(string section, string key, dynamic value)
        {
            ConfigDataType InData = new()
            {
                TypeName = value.GetType().FullName,
                Value = value
            };

            if (!_Data.TryGetValue(section, out object? existingSection))
            {
                _Data.Add(section, new Dictionary<string, ConfigDataType> { { key, InData } });
            }
            else if (existingSection is Dictionary<string, ConfigDataType> Data)
            {
                Data[key] = InData; // 这样可以处理密钥已存在或不存在的两种情况
            }
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
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    Data.Remove(key);
                }
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
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    if (Data.TryGetValue(key, out ConfigDataType? existingData))
                    {
                        if (existingData.TypeName == typeof(T).FullName)
                        {
                            return (T?)existingData.Value;
                        }
                    }
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
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    if (Data.TryGetValue(key, out ConfigDataType? existingData))
                    {
                        return existingData.Value;
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// 获取分区下所有数据
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <returns>分区数据</returns>
        public Dictionary<string, object> GetSection(string section)
        {
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    Dictionary<string, object> result = new();
                    foreach (var item in Data)
                    {
                        result.Add(item.Key, item.Value.DynamicValue());
                    }
                    return result;
                }
            }
            return new();
        }

        /// <summary>
        /// 尝试获取分区下所有数据
        /// </summary>
        /// <param name="section">目标分区</param>
        /// <param name="result">输出数据</param>
        /// <returns></returns>
        public bool TryGetSection(string section, out Dictionary<string, object> result)
        {
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    result = new();
                    foreach (var item in Data)
                    {
                        result.Add(item.Key, item.Value.DynamicValue());
                    }
                    return true;
                }
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
            if (_Data.TryGetValue(section, out object? existingSection))
            {
                if (existingSection is Dictionary<string, ConfigDataType> Data)
                {
                    return Data.ContainsKey(key);
                }
            }
            return false;
        }

        /// <summary>
        /// 序列化为JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(_Data);
        }
    }
}
