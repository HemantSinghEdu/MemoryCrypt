﻿using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisCache 
{

    public class Program
    {
        // connection string to your Redis Cache    
        static string connectionString = "redis9558.redis.cache.windows.net:6380,password=JOxytvZHFMP2VL3kPWdvEQpUPPUovZ4o6AzCaBn2HCI=,ssl=True,abortConnect=False";

        public static async Task Main(string[] args)
        {
            // The connection to the Azure Cache for Redis is managed by the ConnectionMultiplexer class.
            using(var cache = ConnectionMultiplexer.Connect(connectionString))
            {
                IDatabase db = cache.GetDatabase();

                // Snippet below executes a PING to test the server connection
                var result = await db.ExecuteAsync("ping");
                Console.WriteLine($"PING = {result.Type} : {result}");

                // Call StringSetAsync on the IDatabase object to set the key "test:key" to the value "100"
                bool setValue = await db.StringSetAsync("test:key", "100");
                Console.WriteLine($"SET: {setValue}");

                // StringGetAsync takes the key to retrieve and return the value
                string getValue = await db.StringGetAsync("test:key");
                Console.WriteLine($"GET: {getValue}");

            }
        }   
    }
}