using System.Reflection;
using System.Text.Json;
using Config;
using Test;

ConfigHelper config = new(Assembly.GetExecutingAssembly());

config.SetValue("Data", "Data0", new DataClass());
//config.SetValue("Test", "Data1", true);
//config.SetValue("Test", "Data2", 1.658);
//config.SetValue("Test", "Data3", "哈哈哈");

//config["Main","1"] = new Test();

config.FileSaveAsync().Wait();

config.FileLondAsync().Wait();
//Console.WriteLine(config.ToJson());
//Console.WriteLine(JsonSerializer.Deserialize<Test>(config["Data", "Data0"]).T.D);
Console.WriteLine(config["Data", "Data0"].T.D);

