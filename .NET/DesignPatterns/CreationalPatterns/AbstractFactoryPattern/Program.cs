// See https://aka.ms/new-console-template for more information

while(true)
{
    Console.Write("Press 1 or 2, or else q for quit: ");
    var key = System.Console.ReadKey().KeyChar;
    Console.WriteLine();
    if(key=='q')
        break;

    IAbstractFactory factory = null;

    switch(key)
    {
        case '1': factory = new ConcreteFactory1(); break;
        case '2': factory = new ConcreteFactory2(); break;
    }

    Client client = new Client(factory);
    client.DoSomething();
}
