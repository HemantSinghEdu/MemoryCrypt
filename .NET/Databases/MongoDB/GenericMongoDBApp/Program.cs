using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq;
using System;
using System.Collections.Generic;


namespace GenericMongoDBApp
{
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
                //ConventionRegistry.Register(
                //    "IgnoreExtraElements",
                //    new ConventionPack { new IgnoreExtraElementsConvention(true) },
                //    _ => true);

                //Setup the connection
                IMongoClient mongoClient = new MongoClient(connectionString);
                IMongoDatabase mongoDatabase = mongoClient.GetDatabase(database);
                IMongoCollection<BsonDocument> mongoCollection = mongoDatabase.GetCollection<BsonDocument>(collectionName);

                //write some test data 
                mongoCollection.InsertMany(testData);

                //fetch the data
                //var records = mongoCollection
                //            .Find(a => a.Author == "John Doe")
                //            .ToEnumerable()
                //            .Where(a => a.Views > 1000);

                //fetch the data
                var filter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
                    "author", "John Doe",
                    "views", new BsonDocumet{{"$gt",5}}
                    );

                var records = mongoCollection
                            .Find()

                //print the records
                Console.WriteLine("Showing all articles authored by John Doe with more than 1000 views");
                //foreach (var article in records)
                //{
                //    Console.WriteLine($"Title: {article.Title} | Content: {article.Content}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} | {ex.StackTrace}");
            }
        }

        private static List<BsonDocument> testData = new List<BsonDocument>
        {
            new BsonDocument
            {
                {"title", "How to Know Who You Really Are and Aren't"}, 
                {"author", "John Doe"}, 
                {"content", "What are your most important personal values? Do you actually value what you say you do, or are you lying to yourself? And just who the hell are you anyway?"}, 
                {"views", 1000},  
                {"upVotes", 200}
            },

            new BsonDocument
            {
                {"title", "Why Facts Don't Change Our Opinions"}, 
                {"author", "John Doe"}, 
                {"content", "Why don't facts change our minds? And why would someone continue to believe a false or inaccurate idea anyway? How do such behaviors serve us?"}, 
                {"views", 1500},  
                {"upVotes", 250}
            },

            new BsonDocument
            {
                {"title", "Physicists discover Higgs boson"},
                {"author", "Jane Doe"}, 
                {"content", "The Higgs boson, discovered at the CERN particle physics laboratory near Geneva, Switzerland, is the particle that gives all other fundamental particles mass"},
                {"views", 1000},  
                {"upVotes", 50}
            }
        };
    }
}
