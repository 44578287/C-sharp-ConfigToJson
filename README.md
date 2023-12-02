# C-sharp-ConfigToJson
C#基于JSON的配置文件库


# 配置文件类

## 引言
ConfigHelper是一个用于读取和保存配置文件的类。它支持获取、设置和删除配置文件中的值，以及读取和保存整个配置文件。该类使用JSON格式来存储配置文件数据，并提供了灵活的接口以适应不同类型的配置值。

## 使用方法
### 示例
```csharp
using Config;

// 初始化
ConfigHelper config = new();

// 添加不同类型变量
config.SetValue("Test", "Data0", new Test());
config.SetValue("Test", "Data1", true);
config.SetValue("Test", "Data2", 1.658);
config.SetValue("Test", "Data3", "哈哈哈");

// 使用引索器
config["Main","1"] = new Test();

// 保存配置文件
config.FileSaveAsync().Wait();

class Test
{
    public int I { get; set; } = 10;
    public string S { get; set; } = "Hello";
    public Test2 T { get; set; } = new();
}
class Test2
{
    public bool B { get; set; } = true;
    public double D { get; set; } = 1.658;
}
```
### 保存内容预览
```json
{
  "Test": {
    "Data0": {
      "TypeName": "Test",
      "Value": {
        "I": 10,
        "S": "Hello",
        "T": {
          "B": true,
          "D": 1.658
        }
      }
    },
    "Data1": {
      "TypeName": "System.Boolean",
      "Value": true
    },
    "Data2": {
      "TypeName": "System.Double",
      "Value": 1.658
    },
    "Data3": {
      "TypeName": "System.String",
      "Value": "\u54C8\u54C8\u54C8"
    }
  },
  "Main": {
    "1": {
      "TypeName": "Test",
      "Value": {
        "I": 10,
        "S": "Hello",
        "T": {
          "B": true,
          "D": 1.658
        }
      }
    }
  }
}
```
### 构造函数
```csharp
public ConfigHelper(string path = "Config.json")
public ConfigHelper(out bool LondResult, string path = "Config.json")
```
- 构造函数用于创建ConfigHelper的实例。可以传递一个可选的配置文件路径，默认为"Config.json"。第二个构造函数还可以返回一个bool类型的值，表示配置文件的加载结果。

### 加载和保存配置文件
```csharp
public async Task<bool> FileLondAsync()
public async Task<bool> FileSaveAsync()
```
- 使用FileLondAsync方法可以加载配置文件。返回值表示加载操作是否成功。
- 使用FileSaveAsync方法可以将更改后的配置文件保存到磁盘。返回值表示保存操作是否成功。

### 设置和获取配置值
```csharp
public void SetValue<T>(string section, string key, T value)
public void SetDynamicValue(string section, string key, dynamic value)
public T? GetValue<T>(string section, string key)
public bool TryGetValue<T>(string section, string key, out T? value)

MyConfig["systems"]["language"] = "zh-cn";
```
- 使用SetValue方法可以设置配置文件中的值。可以传递任意类型的值作为参数。
- 使用GetValue方法可以获取配置文件中的值。可以传递目标分区和键作为参数，返回对应的值。
- 使用TryGetValue方法可以尝试获取配置文件中的值，并返回获取结果。
- 使用引索器对配置进行读写

### 删除分区和键
```csharp
public void RemoveSection(string section)
public void RemoveKey(string section, string key)
```
- 使用RemoveSection方法可以删除指定的配置文件分区。
- 使用RemoveKey方法可以删除指定的配置文件键。

### 获取分区数据
```csharp
public Dictionary<string, object> GetSection(string section)
public bool TryGetSection(string section, out Dictionary<string, object> result)

MyConfig["systems"];
```
- 使用GetSection方法可以获取指定分区下的所有数据，并返回一个键值对的字典。
- 使用TryGetSection方法可以尝试获取指定分区下的所有数据，并返回获取结果。
- 使用引索器对分区进行读操作

### 检查分区和键是否存在
```csharp
public bool CheckSection(string section)
public bool CheckKey(string section, string key)
```
- 使用CheckSection方法可以检查指定的分区是否存在。
- 使用CheckKey方法可以检查指定的键是否存在。

### 序列化为JSON
```csharp
public string ToJson()
```
- 使用ToJson方法可以将整个配置文件序列化为JSON字符串，并返回该字符串。
