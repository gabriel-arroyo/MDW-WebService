using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTKLibrary.Classes.MDW;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace MDW_WebService.Controllers
{
    [Authorize]
    public class TagsController : ApiController
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        //public string connectionString = "mongodb://innovation:IN2016!@192.169.197.44:27017/mdw+service";
        public string server = "localhost";
        public string connectionString
        {
            get
            {
                return "mongodb://innovation:IN2016!@" + server + "/mdw+service";
            }
        }
        // GET: api/Tags
        public IEnumerable<HTKLibrary.Classes.MDW.Tag> Get()
        {
            List<HTKLibrary.Classes.MDW.Tag> TagList = new List<HTKLibrary.Classes.MDW.Tag>();
            //if (LocalIP.Get() == server)
            //{
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("tags");
            var filter = new BsonDocument();
            var found = _collection.Find(filter).Sort(Builders<BsonDocument>.Sort.Ascending("timestamp")).ToList();
            foreach (BsonDocument item in found)
            {
                HTKLibrary.Classes.MDW.Tag tag = new HTKLibrary.Classes.MDW.Tag();
                if (item.Contains("epc"))
                    tag.epc = item.GetElement("epc").Value.AsString;

                if (item.Contains("direction"))
                    tag.direction = item.GetElement("direction").Value.AsDouble;

                if (item.Contains("erasetime"))
                    tag.erasetime = item.GetElement("erasetime").Value.AsDouble;

                if(item.Contains("ip"))
                    tag.ip = item.GetElement("ip").Value.AsString;

                if(item.Contains("rssi"))
                    tag.rssi = item.GetElement("rssi").Value.AsDouble;

                if(item.Contains("timestamp"))
                    tag.timestamp = item.GetElement("timestamp").Value.AsString;
                
                TagList.Add(tag);
            }
            return TagList;
        }

        // GET: api/Tags/5
        public IEnumerable<HTKLibrary.Classes.MDW.Tag> Get(string from)
        {
            List<HTKLibrary.Classes.MDW.Tag> TagList = new List<HTKLibrary.Classes.MDW.Tag>();
            //if (LocalIP.Get() == server)
            //{
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("tags");
            var filter = Builders<BsonDocument>.Filter.Gt("timestamp", from);
            var found = _collection.Find(filter).Sort(Builders<BsonDocument>.Sort.Ascending("timestamp")).ToList();
            foreach (BsonDocument item in found)
            {
                HTKLibrary.Classes.MDW.Tag tag = new HTKLibrary.Classes.MDW.Tag();
                if (item.Contains("epc"))
                    tag.epc = item.GetElement("epc").Value.AsString;

                if (item.Contains("direction"))
                    tag.direction = item.GetElement("direction").Value.AsDouble;

                if (item.Contains("erasetime"))
                    tag.erasetime = item.GetElement("erasetime").Value.AsDouble;

                if (item.Contains("ip"))
                    tag.ip = item.GetElement("ip").Value.AsString;

                if (item.Contains("rssi"))
                    tag.rssi = item.GetElement("rssi").Value.AsDouble;

                if (item.Contains("timestamp"))
                    tag.timestamp = item.GetElement("timestamp").Value.AsString;

                TagList.Add(tag);
            }
            return TagList;
        }

        // POST: api/Tags
        public async Task<IHttpActionResult> Post([FromBody]HTKLibrary.Classes.MDW.Tag value)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<HTKLibrary.Classes.MDW.Tag>("tags");
            _collection.InsertOne(value);
            return Ok();
        }

        // DELETE: api/Tags/5
        public async Task<IHttpActionResult> Delete(string date, bool backwards = true)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("tags");
            var filter = Builders<BsonDocument>.Filter.Lt("timestamp", date);
            if(!backwards)
                filter = Builders<BsonDocument>.Filter.Gt("timestamp", date);
            _collection.DeleteMany(filter);
            return Ok();
        }
    }
}
