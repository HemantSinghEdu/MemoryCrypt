using Microsoft.Azure.Cosmos;

public class Program
{
    private static readonly string EndpointUri = "https://memcryptcosmosdb.documents.azure.com:443/";
    private static readonly string PrimaryKey = "YeUaALJJXyHokbFZ3Cp70EVX93pdDxZBTjLktL8xAZuU5Pt6vNe6HmYbQkAa8baOpgF6MpD5389cACDbUnzrIQ==";

    private CosmosClient cosmosClient;
    private Database database;
    private Container container;
    //the database and container we will create
    private string databaseId = "myCosmosDatabase";
    private string containerId = "myCosmosContainer";

    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Beginning operations...\n");
            Program p = new Program();
            await p.CosmosAsync();
        }
        catch(CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
        }
        catch(Exception e)
        {
            Console.WriteLine("Error: {0}",e);
        }
        finally
        {
            Console.WriteLine("End of program, press any key to exit.");
            Console.ReadKey();
        }
    }   

    public async Task CosmosAsync()
    {
        this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        await this.CreateDatabaseAsync();
        await this.CreateContainerAsync();
    }

    private async Task CreateDatabaseAsync()
    {
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine("Created Database: {0}\n", this.database.Id);
    }

    private async Task CreateContainerAsync()
    {
        this.container = await this.database.CreateContainerIfNotExistsAsync(containerId,"/LastName");
        Console.WriteLine("Created Container: {0}\n", this.container.Id);
    }
}