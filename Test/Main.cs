using Config;

ConfigHelper config = new();

config.SetValue("Test", "Data0", new Test());
config.SetValue("Test", "Data1", true);
config.SetValue("Test", "Data2", 1.658);
config.SetValue("Test", "Data3", "哈哈哈");

//config["Main","1"] = new Test();

config.FileSaveAsync().Wait();

config.FileLondAsync().Wait();
Console.WriteLine(config.ToJson());
Console.WriteLine(config["Test", "Data3"]);



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