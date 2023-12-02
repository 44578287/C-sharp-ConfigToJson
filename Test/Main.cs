using Config;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

ConfigHelper config = new();

//config.SetValue("Test", "Dara0", new Test());
//config.SetValue("Test", "Dara1", true);
//config.SetValue("Test", "Dara2", 1.658);
//config.SetValue("Test", "Dara3", "哈哈哈");

config["Main","1"] = new Test();

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