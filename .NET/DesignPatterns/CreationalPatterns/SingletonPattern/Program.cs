// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello!");

Singleton.Name = "Eager Singleton";
Console.WriteLine($"Starting Parallel call for : {Singleton.Name}");

Parallel.For(0,10, i =>
{
    var singleton = Singleton.Instance;
    singleton.PrintMessage($"Thread {i}");
});

SingletonLazy.Name = "Lazy Singleton";
Console.WriteLine($"Starting Parallel call for : {SingletonLazy.Name}");

Parallel.For(0,10, i =>
{
    var singletonLazy = SingletonLazy.Instance;
    singletonLazy.PrintMessage($"Thread {i}");
});


Console.WriteLine("Press any key to exit...");
Console.ReadKey();
