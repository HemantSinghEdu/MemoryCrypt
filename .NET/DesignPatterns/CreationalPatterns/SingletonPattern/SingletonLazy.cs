public sealed class SingletonLazy
{
    //private static instance - initially null
    private static SingletonLazy _instance = null;
    //lock object
    private static readonly object _lock = new object();

    //private constructor
    private SingletonLazy()
    {
        Console.WriteLine("INSTANCE CREATED!");
    }

    //public property with only get access
    public static SingletonLazy Instance
    {
        get
        {
            //double checking lock
            if (_instance == null)
            {
                Console.WriteLine("Waiting lock");
                lock (_lock)
                {
                    Console.WriteLine("Lock acquired");
                    if (_instance == null)
                    {
                        _instance = new SingletonLazy();
                    }
                    else{
                        Console.WriteLine("Instance already exists!");
                    }
                }
            }
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