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
    [RoutePrefix("api/MDWAccount")]
    public class MDWAccountController : ApiController
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

        // GET: api/MDWAccount
        [Route("Login")]
        public string GetLogin(string name, string password, string key)
        {
            string id = "";

            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("clients");
                var filter = Builders<BsonDocument>.Filter.Eq("name", name)
                    & Builders<BsonDocument>.Filter.Eq("key", key)
                    & Builders<BsonDocument>.Filter.Eq("password", password);
                var found = _collection.Find(filter).ToList();
                if (found.Count > 0)
                    id = found[0].GetElement("_id").Value.AsObjectId.ToString();
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var requestLicense = new RestSharp.RestRequest("api/MDWAccount/Login?name={name}&password={password}&key={key}", RestSharp.Method.GET);
            //    requestLicense.AddUrlSegment("name", name);
            //    requestLicense.AddUrlSegment("password", password);
            //    requestLicense.AddUrlSegment("key", key);
            //    RestSharp.IRestResponse responseLicense = client.Execute(requestLicense);
            //    id = responseLicense.Content;
            //    var statusLicense = responseLicense.StatusDescription;
            //    if (statusLicense != "OK" || id == null || id == "")
            //        return "";
            //    id = id.Replace("\"", "");
            //}
            return id;
        }

        [Route("Authorize")]
        public bool GetAuthorize(string client_id, string key)
        {
            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("clients");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(client_id)) & Builders<BsonDocument>.Filter.Eq("key", key);
                var found = _collection.Find(filter).ToList();
                return found.Count > 0;
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var requestLicense = new RestSharp.RestRequest("api/MDWAccount/Authorize?client_id={client_id}&key={key}", RestSharp.Method.GET);
            //    requestLicense.AddUrlSegment("client_id", client_id);
            //    requestLicense.AddUrlSegment("key", key);
            //    RestSharp.IRestResponse responseLicense = client.Execute(requestLicense);
            //    var r = responseLicense.Content;
            //    var statusLicense = responseLicense.StatusDescription;
            //    if (statusLicense != "OK" || r == null || r == "")
            //        return false;
            //    if (r.Contains("true"))
            //        return true;
            //    else
            //        return false;
            //}
        }

        [Route("DatabaseCredentials")]
        public MDWCredentials GetDatabaseCredentials(string client_id, string key)
        {
            var credentials = new MDWCredentials();
            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("clients");
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(client_id)) & Builders<BsonDocument>.Filter.Eq("key", key);
                var found = _collection.Find(filter).ToList();
                if (found.Count > 0)
                {
                    try
                    {
                        credentials.db = found[0].GetElement("assetsapp_db_credentials").Value.AsBsonDocument.GetElement("name").Value.AsString;
                        credentials.host = found[0].GetElement("assetsapp_db_credentials").Value.AsBsonDocument.GetElement("host").Value.AsString;
                        credentials.password = found[0].GetElement("assetsapp_db_credentials").Value.AsBsonDocument.GetElement("password").Value.AsString;
                        credentials.user = found[0].GetElement("assetsapp_db_credentials").Value.AsBsonDocument.GetElement("name").Value.AsString;
                        credentials.port = 27017;
                    }
                    catch { }
                }
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var request = new RestSharp.RestRequest("api/MDWAccount/DatabaseCredentials?client_id={client_id}&key={key}", RestSharp.Method.GET);
            //    request.AddUrlSegment("client_id", client_id);
            //    request.AddUrlSegment("key", key);
            //    RestSharp.IRestResponse<MDWCredentials> responseLicense = client.Execute<MDWCredentials>(request);
            //    credentials = responseLicense.Data;
            //    var statusLicense = responseLicense.StatusDescription;
            //    if (statusLicense != "OK")
            //        return null;
            //}
            return credentials;
        }


        // POST: api/MDWAccount
        [Route("CreateClient")]
        public void PostCreateClient(string client)
        {
        }

        // PUT: api/MDWAccount/5
        [Route("UpdateClient")]
        public void PutUpdateClient(string client_id, string field, string value)
        {
        }

        // DELETE: api/MDWAccount/5
        [Route("DeleteClient")]
        public void Delete(string client_id)
        {
        }
    }
}
