using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MDW_WebService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Status")]
    public class StatusController : ApiController
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

        public class MDWCredentials
        {
            public string host;
            public string db;
            public string user;
            public string password;
            public int port;

            public MDWCredentials()
            {
                host = "";
                db = "";
                user = "";
                password = "";
                port = 27017;
            }
        }
        // GET: api/Status
        [Route("Antennas")]
        public List<Status> GetStatus(string ip)
        {
            string id = "";
            List<Status> statusList = new List<Status>();
            //if (LocalIP.Get() == server)
            //{
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("clients");
            var filter = Builders<BsonDocument>.Filter.Eq("ip", ip);
            var found = _collection.Find(filter).ToList();
            if (found.Count > 0)
            {
                for (int i = 0; i < found.Count; i++)
                {
                    Status status = new Status();
                    id = found[i].GetElement("_id").Value.AsObjectId.ToString();
                    status.code = found[i].GetElement("code").Value.AsInt32;
                    status.comment = found[i].GetElement("comment").Value.AsString;
                    status.ip = found[i].GetElement("ip").Value.AsString;
                    status.timestamp = found[i].GetElement("timestamp").Value.AsString;
                    statusList.Add(status);
                }
                _collection.DeleteMany(filter);
            }

            return statusList;
        }
        [Route("Antennas")]
        public List<Status> GetStatus()
        {
            string id = "";
            List<Status> statusList = new List<Status>();
            //if (LocalIP.Get() == server)
            //{
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("clients");
            var filter = new BsonDocument();
            var found = _collection.Find(filter).ToList();
            if (found.Count > 0)
            {
                for (int i = 0; i < found.Count; i++)
                {
                    Status status = new Status();
                    id = found[i].GetElement("_id").Value.AsObjectId.ToString();
                    status.code = found[i].GetElement("code").Value.AsInt32;
                    status.comment = found[i].GetElement("comment").Value.AsString;
                    status.ip = found[i].GetElement("ip").Value.AsString;
                    status.timestamp = found[i].GetElement("timestamp").Value.AsString;
                    statusList.Add(status);
                }
                _collection.DeleteMany(filter);
            }

            return statusList;
        }

        // GET: api/Status/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Status
        public void Post([FromBody]Status status)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("mdw+service");
            var _collection = _database.GetCollection<BsonDocument>("clients");
            _collection.InsertOne(new BsonDocument
            {
                { "code" , status.code },
                { "comment", status.comment},
                { "ip" , status.ip},
                {"timestamp", status.ip }
            });
        }

        // PUT: api/Status/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Status/5
        public void Delete(int id)
        {
        }

        public class Status
        {
            public string ip { get; set; }
            public int code { get; set; }
            public string comment { get; set; }
            public string timestamp { get; set; }
        }
    }
}
