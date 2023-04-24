public class Client
{
    private IAbstractProductA _productA;
    private IAbstractProductB _productB;

    public Client(IAbstractFactory factory)
    {
        _productA = factory.CreateProductA();
        _productB = factory.CreateProductB();
    }

    public void DoSomething()
    {
        _productA.RunA();
        _productB.RunB();
    }
}