using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using HTKLibrary.Readers;
using System.Threading;
using MDW_WebService.Models;

namespace MDW_WebService.Controllers
{
    //[Authorize]
    [RoutePrefix("api/DOIHI")]
    public class DOIHIController : ApiController
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        public string connectionString = "mongodb://IosUser3:IU2015!@192.168.25.123:27017/doihitest";
        public string connectionStringLocal = "mongodb://IosUser3:IU2015!@localhost:27017/doihitest";
        public string connectionStringTest = "mongodb://IosUser3:IU2015!@localhost:27017/doihi";
        public static string database = "doihitest";
        public static string databaseTest = "doihi";
        // GET: api/DOIHI
        [Route("ubicarEpc")]
        public List<ActivoLocalizado> GetbyEPC(string epc)
        {
            List<ActivoLocalizado> localizados = new List<ActivoLocalizado>();
            _client = new MongoClient(connectionStringLocal);
            _database = _client.GetDatabase("doihitest");
            var activos = _database.GetCollection<BsonDocument>("ObjectReal");
            var ubicaciones = _database.GetCollection<BsonDocument>("Locations");
            var lecturas = _database.GetCollection<BsonDocument>("Hardware");
            var antenas = _database.GetCollection<BsonDocument>("HardwareCategories");
            var filterLectura = Builders<BsonDocument>.Filter.Eq("serie", epc);
            var filter = Builders<BsonDocument>.Filter.Eq("EPC", epc);
            var found = activos.Find(filter).ToList();
            foreach (BsonDocument item in found)
            {
                string _nombre = "ND";
                string _marca = "ND";
                string _modelo = "ND";
                string _serie = "ND";
                string _epc = "ND";
                string _ubicacion = "ND";
                string _ultimaLectura = "ND";

                if (item.Contains("name"))
                    _nombre = item.GetElement("name").Value.AsString;
                if (item.Contains("marca"))
                    _marca = item.GetElement("marca").Value.AsString;
                if (item.Contains("modelo"))
                    _modelo = item.GetElement("modelo").Value.AsString;
                if (item.Contains("serie"))
                    _serie = item.GetElement("serie").Value.AsString;
                if (item.Contains("EPC"))
                    _epc = item.GetElement("EPC").Value.AsString;
                if(item.Contains("location"))
                {
                    string _locationId = item.GetElement("location").Value.AsString;
                    string level1 = "";
                    string level2 = "";
                    string level3 = "";
                    var filterLevel1 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(_locationId));
                    FilterDefinition<BsonDocument> filterLevel2 = new BsonDocument();
                    FilterDefinition<BsonDocument> filterLevel3 = new BsonDocument();
                    var foundLevel1 = ubicaciones.Find(filterLevel1).ToList().ToList();
                    if(foundLevel1.Count > 0)
                    {
                        if (foundLevel1[0].Contains("name"))
                            level1 = foundLevel1[0].GetElement("name").Value.AsString;
                        if (foundLevel1[0].Contains("parent"))
                        {
                            filterLevel2 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(foundLevel1[0].GetElement("parent").Value.AsString));
                            var foundLevel2 = ubicaciones.Find(filterLevel2).ToList().ToList();
                            if (foundLevel2.Count > 0)
                            {
                                if (foundLevel2[0].Contains("name"))
                                    level2 = foundLevel2[0].GetElement("name").Value.AsString;
                                if (foundLevel2[0].Contains("parent"))
                                {
                                    filterLevel3 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(foundLevel2[0].GetElement("parent").Value.AsString));
                                    var foundLevel3 = ubicaciones.Find(filterLevel3).ToList().ToList();
                                    if (foundLevel3.Count > 0)
                                    {
                                        if (foundLevel3[0].Contains("name"))
                                            level3 = foundLevel3[0].GetElement("name").Value.AsString;
                                    }
                                }

                            }
                        }
                        
                    }
                    _ubicacion = level3 + "/" + level2 + "/" + level1;
                }
                var sort = Builders<BsonDocument>.Sort.Descending(document => document["CreatedDate"]);
                var foundLectura = lecturas.Find(filterLectura).Sort(sort).Limit(1).ToList();
                if(foundLectura.Count > 0)
                {
                    if(foundLectura[0].Contains("hardware_reference"))
                    {
                        var antennaId = foundLectura[0].GetElement("hardware_reference").Value.AsString;
                        var filterAntenna = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(antennaId));
                        var foundAntena = antenas.Find(filterAntenna).ToList();
                        if(foundAntena.Count > 0)
                        {
                            if (foundAntena[0].Contains("name"))
                                _ultimaLectura = foundAntena[0].GetElement("name").Value.AsString;
                        }
                    }
                }

                ActivoLocalizado activo = new ActivoLocalizado(_nombre, _marca, _modelo, _serie, _epc, _ubicacion, _ultimaLectura);
                localizados.Add(activo);
            }
            return localizados;
        }
        [Route("ubicarSerie")]
        public List<ActivoLocalizado> GetbySerie(string serie)
        {
            string epc = "";
            List<ActivoLocalizado> localizados = new List<ActivoLocalizado>();
            _client = new MongoClient(connectionStringLocal);
            _database = _client.GetDatabase("doihitest");
            var activos = _database.GetCollection<BsonDocument>("ObjectReal");
            var ubicaciones = _database.GetCollection<BsonDocument>("Locations");
            var lecturas = _database.GetCollection<BsonDocument>("Hardware");
            var antenas = _database.GetCollection<BsonDocument>("HardwareCategories");
            
            var filter = Builders<BsonDocument>.Filter.Eq("serie", serie);
            var found = activos.Find(filter).ToList();
            foreach (BsonDocument item in found)
            {
                string _nombre = "ND";
                string _marca = "ND";
                string _modelo = "ND";
                string _serie = "ND";
                string _epc = "ND";
                string _ubicacion = "ND";
                string _ultimaLectura = "ND";

                if (item.Contains("name"))
                    _nombre = item.GetElement("name").Value.AsString;
                if (item.Contains("marca"))
                    _marca = item.GetElement("marca").Value.AsString;
                if (item.Contains("modelo"))
                    _modelo = item.GetElement("modelo").Value.AsString;
                if (item.Contains("serie"))
                    _serie = item.GetElement("serie").Value.AsString;
                if (item.Contains("EPC"))
                {
                    _epc = item.GetElement("EPC").Value.AsString;
                    epc = _epc;
                }
                    
                if (item.Contains("location"))
                {
                    string _locationId = item.GetElement("location").Value.AsString;
                    string level1 = "";
                    string level2 = "";
                    string level3 = "";
                    var filterLevel1 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(_locationId));
                    FilterDefinition<BsonDocument> filterLevel2 = new BsonDocument();
                    FilterDefinition<BsonDocument> filterLevel3 = new BsonDocument();
                    var foundLevel1 = ubicaciones.Find(filterLevel1).ToList().ToList();
                    if (foundLevel1.Count > 0)
                    {
                        if (foundLevel1[0].Contains("name"))
                            level1 = foundLevel1[0].GetElement("name").Value.AsString;
                        if (foundLevel1[0].Contains("parent"))
                        {
                            filterLevel2 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(foundLevel1[0].GetElement("parent").Value.AsString));
                            var foundLevel2 = ubicaciones.Find(filterLevel2).ToList().ToList();
                            if (foundLevel2.Count > 0)
                            {
                                if (foundLevel2[0].Contains("name"))
                                    level2 = foundLevel2[0].GetElement("name").Value.AsString;
                                if (foundLevel2[0].Contains("parent"))
                                {
                                    filterLevel3 = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(foundLevel2[0].GetElement("parent").Value.AsString));
                                    var foundLevel3 = ubicaciones.Find(filterLevel3).ToList().ToList();
                                    if (foundLevel3.Count > 0)
                                    {
                                        if (foundLevel3[0].Contains("name"))
                                            level3 = foundLevel3[0].GetElement("name").Value.AsString;
                                    }
                                }

                            }
                        }

                    }
                    _ubicacion = level3 + "/" + level2 + "/" + level1;
                }
                var sort = Builders<BsonDocument>.Sort.Descending(document => document["CreatedDate"]);
                var filterLectura = Builders<BsonDocument>.Filter.Eq("serie", epc);
                var foundLectura = lecturas.Find(filterLectura).Sort(sort).Limit(1).ToList();
                if (foundLectura.Count > 0)
                {
                    if (foundLectura[0].Contains("hardware_reference"))
                    {
                        var antennaId = foundLectura[0].GetElement("hardware_reference").Value.AsString;
                        var filterAntenna = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(antennaId));
                        var foundAntena = antenas.Find(filterAntenna).ToList();
                        if (foundAntena.Count > 0)
                        {
                            if (foundAntena[0].Contains("name"))
                                _ultimaLectura = foundAntena[0].GetElement("name").Value.AsString;
                        }
                    }
                }

                ActivoLocalizado activo = new ActivoLocalizado(_nombre, _marca, _modelo, _serie, _epc, _ubicacion, _ultimaLectura);
                localizados.Add(activo);
            }
            return localizados;
        }
        public bool GetBelongs(string epc)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("doihitest");
            var _collection = _database.GetCollection<BsonDocument>("ObjectReal");
            var filter = Builders<BsonDocument>.Filter.Eq("EPC", epc);
            var found = _collection.Find(filter).ToList();
            return found.Count > 0;
        }

        public bool GetAlarmPermission(string epc, string timestamp)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("doihitest");
            var _collection = _database.GetCollection<BsonDocument>("Hardware");
            var filter = Builders<BsonDocument>.Filter.Eq("EPC", epc)
                        & Builders<BsonDocument>.Filter.Gt("CreatedTimeStamp", Convert.ToInt64(timestamp) - 500)
                        & Builders<BsonDocument>.Filter.Eq("alarm", true);
            var found = _collection.Find(filter).ToList();
            return found.Count < 3;
        }

        //public async Task<IHttpActionResult> PostAlarm(string epc, string ip)
        //{
        //    var CreatedTimeStamp = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    string datetime = DateTime.Now.ToString();
        //    _client = new MongoClient(connectionString);
        //    _database = _client.GetDatabase("doihitest");
        //    var _collection = _database.GetCollection<BsonDocument>("Alarms");
        //    var filter = Builders<BsonDocument>.Filter.Eq("EPC", epc)
        //                & Builders<BsonDocument>.Filter.Eq("antenna_ip", ip)
        //                & Builders<BsonDocument>.Filter.Gt("CreatedTimeStamp", CreatedTimeStamp - 500)
        //                & Builders<BsonDocument>.Filter.Eq("alarm", true);
        //    var found = _collection.Find(filter).ToList();
        //    if (found.Count > 3) return Content(HttpStatusCode.Forbidden, "La alarma ha sonado más de dos veces en los pasados 5 minutos");

        //    var hardwareCategories = _database.GetCollection<BsonDocument>("HardwareCategories");
        //    var filterIP = Builders<BsonDocument>.Filter.Eq("ip", ip);
        //    var foundAntenna = hardwareCategories.Find(filterIP).ToList();

        //    if (foundAntenna.Count < 1) return Content(HttpStatusCode.Forbidden, "No se encuentra la antena " + ip);



        //    BsonDocument document = new BsonDocument()
        //    {
        //        { "EPC", epc },
        //        { "CreatedDate", datetime},
        //        { "antenna_name", foundAntenna[0].GetElement("name").Value.AsString },
        //        { "antenna_ip", ip},
        //        { "CreatedTimeStamp", CreatedTimeStamp},
        //    };

        //    _collection.InsertOne(document);

        //    //CS203.SetGPO1(ip, true);
        //    //Thread.Sleep(8000);
        //    //CS203.SetGPO1(ip, false);
        //    //CS203.SetGPO1(ip, false);
        //    //Thread.Sleep(100);
        //    //CS203.SetGPO1(ip, false);


        //    return Ok();
        //}

        // POST: api/DOIHI
        public async Task<IHttpActionResult> PostReading(string epc, string ip)
        {
            bool alarm = true;
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(database);

            string name = "";
            string CreatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string hardare_reference = "";
            Int64 CreatedTimeStamp = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));


            var foundObject = _database.GetCollection<BsonDocument>("ObjectReal")
                .Find(Builders<BsonDocument>.Filter.Eq("EPC", epc)).ToList();

            if (foundObject.Count < 1) return Content(HttpStatusCode.Forbidden, "El activo no pertenece a la base de datos");
            
            name = foundObject[0].Contains("name") ? foundObject[0].GetElement("name").Value.AsString : "";

            var foundAntenna = _database.GetCollection<BsonDocument>("HardwareCategories")
                .Find(Builders<BsonDocument>.Filter.Eq("ip", ip)).ToList();
                
            if (foundAntenna.Count < 1) return Content(HttpStatusCode.Forbidden, "No se encuentra la antena " + ip);

            hardare_reference = foundAntenna[0].GetElement("_id").Value.AsObjectId.ToString();


            int timesReaded = foundObject[0].Contains("timesReaded") ? foundObject[0].GetElement("timesReaded").Value.AsInt32 : 0;

            if(foundObject[0].Contains("antenna_ip"))
            {
                if (foundObject[0].GetElement("antenna_ip").Value.AsString != ip)
                    timesReaded = 0;
            }
         
            int wait = timesReaded * timesReaded;
            timesReaded++;
            if(timesReaded > 5)
            {
                alarm = false;
                await _database.GetCollection<BsonDocument>("ObjectReal").UpdateOneAsync(
                Builders<BsonDocument>.Filter.Eq("EPC", epc),
                Builders<BsonDocument>.Update.Set("readTimeStamp", CreatedTimeStamp));
                return Content(HttpStatusCode.Forbidden, "El activo ha sonado más de 5 veces");
            }
            if (foundObject[0].Contains("readTimeStamp"))
            {
                var lastTimeStamp = foundObject[0].GetElement("readTimeStamp").Value.AsInt64.ToString();
                var year = Convert.ToInt32(lastTimeStamp.Substring(0, 4));
                var month = Convert.ToInt32(lastTimeStamp.Substring(4, 2));
                var day = Convert.ToInt32(lastTimeStamp.Substring(6, 2));
                var hour = Convert.ToInt32(lastTimeStamp.Substring(8, 2));
                var minute = Convert.ToInt32(lastTimeStamp.Substring(10, 2));
                var second = Convert.ToInt32(lastTimeStamp.Substring(12, 2));
                var lastDate = new DateTime(year, month, day, hour, minute, second);
                var diff = DateTime.Now.Subtract(lastDate).TotalMinutes;
                int difint = (int)diff;
                if(diff < wait)
                {
                    alarm = false;
                    await _database.GetCollection<BsonDocument>("ObjectReal").UpdateOneAsync(
                    Builders<BsonDocument>.Filter.Eq("EPC", epc),
                    Builders<BsonDocument>.Update.Set("readTimeStamp", CreatedTimeStamp));
                    return Content(HttpStatusCode.Forbidden, "No ha pasado el tiempo de espera " + difint + "/" + wait);

                }
            }
            else
            {
                alarm = true;
            }
            if (alarm && name.ToLower().Contains("silla") && timesReaded == 1)
                new Thread(() => Red(ip)).Start();
            else if (alarm && !name.ToLower().Contains("silla"))
                new Thread(() => Red(ip)).Start();
            else
                alarm = false;

            await _database.GetCollection<BsonDocument>("ObjectReal").UpdateOneAsync(
                    Builders<BsonDocument>.Filter.Eq("EPC", epc),
                    Builders<BsonDocument>.Update.Set("timesReaded", (timesReaded))
                                                 .Set("antenna_ip", ip)
                                                 .Set("readTimeStamp", CreatedTimeStamp)
                );

            BsonDocument document = new BsonDocument()
            {
                { "name", name},
                { "serie", epc },
                { "CreatedDate", CreatedDate},
                { "hardware_reference", hardare_reference},
                { "antenna_name", foundAntenna[0].GetElement("name").Value.AsString },
                { "antenna_ip", ip},
                { "CreatedTimeStamp", CreatedTimeStamp},
                { "date", Convert.ToInt32(CreatedTimeStamp.ToString().Substring(0,8))},
                { "time", Convert.ToInt32(CreatedTimeStamp.ToString().Substring(8,6))},
                { "alarm", alarm }
            };

            
            await _database.GetCollection<BsonDocument>("Hardware").InsertOneAsync(document);
            return Ok();
        }
        [Route("Semaphore")]
        public async Task<IHttpActionResult> PostGreen(string ip, bool on)
        {
            new Thread(() => Green(ip, on)).Start();
            return Ok();
        }
        static readonly object redLock = new object();
        void Red(string ip)
        {
            //lock(redLock)
            //{
                CS203.SetGPO1(ip, true);
                Thread.Sleep(8000);
                CS203.SetGPO1(ip, false);
                CS203.SetGPO1(ip, false);
                Thread.Sleep(100);
                CS203.SetGPO1(ip, false);
            //}
        }
        void Green(string ip, bool on)
        {
            CS203.SetGPO0(ip, on);
        }
    }
}
