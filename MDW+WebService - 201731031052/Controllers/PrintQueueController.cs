using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTKLibrary.Comunications.Net35;
using HTKLibrary.Classes.MDW;
using MongoDB.Bson;
using MongoDB.Driver;
using HTKLibrary.Comunications;
using System.Threading.Tasks;
using RestSharp;

namespace MDW_WebService.Controllers
{
    [Authorize]
    public class PrintQueueController : ApiController
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


        // GET: api/PrintQueue
        public IEnumerable<PrintTag> Get(string client_id)
        {
            List<PrintTag> printQueue = new List<PrintTag>();
            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("printQueue");
                var filter = Builders<BsonDocument>.Filter.Eq("client_id", client_id);
                var found = _collection.Find(filter).ToList();
                foreach (BsonDocument item in found)
                {
                    PrintTag tag = new PrintTag();
                    if (item.Contains("epc"))
                        tag.epc = item.GetElement("epc").Value.AsString;

                    if (item.Contains("fields"))
                    {
                        var d = item.GetElement("fields").Value.AsBsonDocument.ToDictionary();
                        foreach (var elem in d)
                        {
                            string k = elem.Key.ToString();
                            string v = elem.Value.ToString();
                            tag.fields.Add(k, v);
                        }
                    }

                    printQueue.Add(tag);
                }
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var request = new RestSharp.RestRequest("api/PrintQueue?client_id={client_id}", RestSharp.Method.GET);
            //    request.AddUrlSegment("client_id", client_id);
            //    RestSharp.IRestResponse<List<PrintTag>> responseLicense = client.Execute<List<PrintTag>>(request);
            //    printQueue = responseLicense.Data;
            //    var statusLicense = responseLicense.StatusDescription;
            //    if (statusLicense != "OK")
            //        return null;
            //}
            
            return printQueue;
        }

        // GET: api/PrintQueue/5
        public IEnumerable<PrintTag> Get(string client_id, string printer_name)
        {
            List<PrintTag> printQueue = new List<PrintTag>();

            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("printQueue");
                var filter = Builders<BsonDocument>.Filter.Eq("client_id", client_id)
                           & Builders<BsonDocument>.Filter.Eq("printer_name", printer_name);
                var found = _collection.Find(filter).ToList();
                foreach (BsonDocument item in found)
                {
                    PrintTag tag = new PrintTag();
                    if (item.Contains("epc"))
                        tag.epc = item.GetElement("epc").Value.AsString;

                    if (item.Contains("fields"))
                    {
                        var d = item.GetElement("fields").Value.AsBsonDocument.ToDictionary();
                        foreach (var elem in d)
                        {
                            string k = elem.Key.ToString();
                            string v = elem.Value.ToString();
                            tag.fields.Add(k, v);
                        }
                    }

                    printQueue.Add(tag);
                }
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var request = new RestSharp.RestRequest("api/PrintQueue?client_id={client_id}&printer_name={printer_name}", RestSharp.Method.GET);
            //    request.AddUrlSegment("client_id", client_id);
            //    request.AddUrlSegment("printer_name", printer_name);
            //    RestSharp.IRestResponse<List<PrintTag>> responseLicense = client.Execute<List<PrintTag>>(request);
            //    printQueue = responseLicense.Data;
            //    var statusLicense = responseLicense.StatusDescription;
            //    if (statusLicense != "OK")
            //        return null;
            //}
            return printQueue;
        }
        class TokenClass
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string userName { get; set; }
        }
        private string LoginServer()
        {
            var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            var requestToken = new RestSharp.RestRequest("Token", RestSharp.Method.POST);
            requestToken.AddParameter("UserName", "middleware@htk-id.com");
            requestToken.AddParameter("Password", "Middleware2016!");
            requestToken.AddParameter("grant_type", "password");
            requestToken.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            RestSharp.IRestResponse responseToken = client.Execute(requestToken);
            var contentToken = responseToken.Content;
            TokenClass responseToken2 = client.Execute<TokenClass>(requestToken).Data;
            var token = responseToken2.access_token;
            var statusToken = responseToken.StatusDescription;
            if (statusToken == "OK" || token != null || token != "")
                return token;
            //Register
            
            var requestRegister = new RestSharp.RestRequest("api/Account/Register", RestSharp.Method.POST);
            requestRegister.AddParameter("UserName", "middleware@htk-id.com");
            requestRegister.AddParameter("Email", "middleware@htk-id.com");
            requestRegister.AddParameter("Password", "Middleware2016!");
            requestRegister.AddParameter("ConfirmPassword", "Middleware00000!");
            requestRegister.AddHeader("Content-Type", "application/json");
            RestSharp.IRestResponse responseRegister = client.Execute(requestRegister);
            var contentRegister = responseRegister.Content;
            var statusRegister = responseRegister.StatusDescription;
            if (contentRegister.Contains("is already taken"))
                statusRegister = "OK";
            if (statusRegister != "OK")
                return "";
            //Token
            responseToken = client.Execute(requestToken);
            contentToken = responseToken.Content;
            responseToken2 = client.Execute<TokenClass>(requestToken).Data;
            token = responseToken2.access_token;
            statusToken = responseToken.StatusDescription;
            return token;
        }
        // POST: api/PrintQueue
        public async Task<IHttpActionResult> Post([FromBody]PrintTag tag, string client_id)
        {
            //if (LocalIP.Get() == server)
            //{
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("printQueue");

                BsonDocument document = new BsonDocument();
                document.Add("client_id", client_id);
                if (!string.IsNullOrEmpty(tag.epc))
                    document.Add("epc", tag.epc);
                //foreach (var element in tag.fields)
                //{
                //    document.Add(element.Key, element.Value);
                //}
                if (tag.fields.Count > 0)
                {
                    document.Add("fields", new BsonDocument(tag.fields));
                    
                }
                    
                _collection.InsertOne(document);
                return Ok();
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var request = new RestSharp.RestRequest("api/PrintQueue?client_id={client_id}", RestSharp.Method.POST);
            //    request.AddUrlSegment("client_id", client_id);
            //    request.AddHeader("Content-Type", "application/json");
            //    request.AddHeader("Authorization", "Bearer " + LoginServer());
            //    request.RequestFormat = DataFormat.Json;
            //    request.AddBody(new { epc = tag.epc, fields = tag.fields });
            //    RestSharp.IRestResponse responseToken = client.Execute(request);
            //    var statusToken = responseToken.StatusDescription;
            //    if (statusToken != "OK")
            //        return BadRequest();
            //    return Ok();
            //}
        }

        public async Task<IHttpActionResult> Post([FromBody]PrintTag tag, string client_id, string printer_name)
        {
            //if (LocalIP.Get() == server)
            //{
                List<PrintTag> printQueue = new List<PrintTag>();
                _client = new MongoClient(connectionString);
                _database = _client.GetDatabase("mdw+service");
                var _collection = _database.GetCollection<BsonDocument>("printQueue");

                BsonDocument document = new BsonDocument();
                document.Add("client_id", client_id);
                document.Add("printer_name", printer_name);
                if (!string.IsNullOrEmpty(tag.epc))
                    document.Add("epc", tag.epc);
                //foreach (var element in tag.fields)
                //{
                //    document.Add(element.Key, element.Value);
                //}
                if (tag.fields.Count > 0)
                    document.Add("fields", new BsonDocument(tag.fields));
                _collection.InsertOne(document);
                return Ok();
            //}
            //else
            //{
            //    var client = new RestSharp.RestClient("http://webservice.assetsapp.com/mdw/");
            //    var request = new RestSharp.RestRequest("api/PrintQueue?client_id={client_id}&printer_name={printer_name}", RestSharp.Method.POST);
            //    request.AddUrlSegment("client_id", client_id);
            //    request.AddUrlSegment("printer_name", printer_name);
            //    request.AddHeader("Content-Type", "application/json");
            //    request.AddHeader("Authorization", "Bearer " + LoginServer());
            //    request.RequestFormat = DataFormat.Json;
            //    request.AddBody(new { tag = tag });
            //    RestSharp.IRestResponse responseToken = client.Execute(request);
            //    var statusToken = responseToken.StatusDescription;
            //    if (statusToken != "OK")
            //        return BadRequest();
            //    return Ok();
            //}
        }
       
    }
}
