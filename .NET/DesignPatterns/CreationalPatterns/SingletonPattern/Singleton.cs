public sealed class Singleton
{
    //private static instance - thread safety ensured by .NET
    private static readonly Singleton _instance = new Singleton();

    //private constructor
    private Singleton()
    {
        Console.WriteLine("INSTANCE CREATED!");
    }

    //public property with only get access
    public static Singleton Instance
    {
        get
        {
            return _instance;
        }
    }

   //other properties and methods
    public static string Name { get; set; }

    public void PrintMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}