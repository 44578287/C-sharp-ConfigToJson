namespace Test
{
    public class DataClass
    {
        public int I { get; set; } = 10;
        public string S { get; set; } = "Hello";
        public Test2 T { get; set; } = new();
    }
    public class Test2
    {
        public bool B { get; set; } = true;
        public double D { get; set; } = 1.658;
    }
}
