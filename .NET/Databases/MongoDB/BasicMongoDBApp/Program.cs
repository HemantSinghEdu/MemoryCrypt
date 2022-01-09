using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq;
using System;
using System.Collections.Generic;

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
            try
            {
                //MongoDB properties - please provide valid values
                string username = "user_name";  //ignored if connected to localhost
                string password = "password";   //ignored if connected to localhost
                string hostname = "localhost";  //server name
                int portNumber = 27017;
                string database = "mindcrypt";
                string replicaSet = "mcrypt_01";
                string collectionName = "articles";

                string serverConnectionString = $"mongodb://{username}:{password}@{hostname}:{portNumber}?readPreference=primaryPreferred&replicaSet={replicaSet}&authMechanism=PLAIN&authSource=$external&ssl=true";
                string localConnectionString = $"mongodb://localhost:{portNumber}";
                string connectionString = hostname == "localhost" ? localConnectionString : serverConnectionString;

                //Ignore the MongoDB fields that are currently not added to our model yet
                ConventionRegistry.Register(
                    "IgnoreExtraElements",
                    new ConventionPack { new IgnoreExtraElementsConvention(true) },
                    _ => true);

                //Setup the connection
                IMongoClient mongoClient = new MongoClient(connectionString);
                IMongoDatabase mongoDatabase = mongoClient.GetDatabase(database);
                IMongoCollection<Article> mongoCollection = mongoDatabase.GetCollection<Article>(collectionName);

                //write some test data 
                mongoCollection.InsertMany(testData);

                //fetch the data
                var records = mongoCollection
                            .Find(a => a.Author == "John Doe")
                            .ToEnumerable()
                            .Where(a => a.Views > 1000);

                //print the records
                Console.WriteLine("Showing all articles authored by John Doe with more than 1000 views");
                foreach (var article in records)
                {
                    Console.WriteLine($"Title: {article.Title} | Content: {article.Content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} | {ex.StackTrace}");
            }
        }

        private static List<Article> testData = new List<Article>
        {
            new Article
            {
                Title = "How to Know Who You Really Are and Aren't", 
                Author = "John Doe", 
                Content = "What are your most important personal values? Do you actually value what you say you do, or are you lying to yourself? And just who the hell are you anyway?", 
                Views = 1000,  
                UpVotes = 200
            },

            new Article
            {
                Title = "Why Facts Don't Change Our Opinions", 
                Author = "John Doe", 
                Content = "Why don't facts change our minds? And why would someone continue to believe a false or inaccurate idea anyway? How do such behaviors serve us?", 
                Views = 1500,  
                UpVotes = 250
            },

            new Article
            {
                Title = "Physicists discover Higgs boson",
                Author = "Jane Doe", 
                Content = "The Higgs boson, discovered at the CERN particle physics laboratory near Geneva, Switzerland, is the particle that gives all other fundamental particles mass",
                Views = 1000,  
                UpVotes = 50
            },
        };
    }
}
