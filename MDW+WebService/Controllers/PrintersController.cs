using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using HTKLibrary.Comunications;

namespace MDW_WebService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Printers")]
    public class PrintersController : ApiController
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        public string server = "localhost";
        public string connectionString
        {
            get
            {
                return "mongodb://innovation:IN2016!@" + server + "/mdw+service";
            }
        }

       

        // GET: api/Printers
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Printers/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Printers
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Printers/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Printers/5
        public void Delete(int id)
        {
        }

        [Route("Printers")]
        public List<string> GetPrinters(string client_id)
        {
            var printers = new List<BsonValue>();
            var printersString = new List<string>();

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("clients");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(client_id));
            var found = _collection.Find(filter).ToList();
            if (found.Count > 0)
            {
                try
                {
                    printers = found[0].GetElement("hardware").Value.AsBsonDocument.GetElement("printers").Value.AsBsonArray.ToList();                  
                }
                catch { }
            }
            foreach (var item in printers)
            {
                printersString.Add(item.AsString);
            }
            return printersString;
        }
    }
}
