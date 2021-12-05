using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq;
using System;

namespace BasicMongoDBApp
{
    public class Article
    {

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("views")]
        public int Views { get; set; }

        [BsonElement("upvotes")]
        public int UpVotes { get; set; }

    }


    class Program
    {
        static void Main(string[] args)
        {
            //MongoDB properties - please provide valid values
            string username = "user_name";
            string password = "password";
            string hostname = "host_name";
            int portNumber = 12345;
            string database = "database_name";
            string replicaSet = "replica_set_name";
            string collectionName = "collection_name";

            string connectionString = $"mongodb://{username}:{password}@{hostname}:{portNumber}?readPreference=primaryPreferred&replicaSet={replicaSet}&authMechanism=PLAIN&authSource=$external&ssl=true";

            //Ignore the MongoDB fields that are currently not added to our model yet
            ConventionRegistry.Register(
                "IgnoreExtraElements",
                new ConventionPack { new IgnoreExtraElementsConvention(true) },
                _ => true);

            //Setup the connection
            IMongoClient mongoClient = new MongoClient(connectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(database);
            IMongoCollection<Article> mongoCollection = mongoDatabase.GetCollection<Article>(collectionName);

            //fetch the data
            var records = mongoCollection
                         .Find(a => a.Author == "John Doe")
                         .ToEnumerable()
                         .Where(a => a.Views > 1000);

            //print the records
            Console.WriteLine("Showing all articles authored by John Doe with more than 1000 views");
            foreach(var article in records)
            {
                Console.WriteLine($"Title: {article.Title} | Content: {article.Content}");
            }

        }
    }
}
