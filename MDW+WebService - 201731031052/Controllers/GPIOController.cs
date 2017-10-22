using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MDW_WebService.Controllers
{

    /// <summary>
    /// Just for Localy installed MDW+
    /// </summary>
    [Authorize]
    public class GPIOController : ApiController
    {
        // GET: api/GPIO
        public bool Get(string ip, int port)
        {
            bool state = false;
            if(port == 0)
                state = HTKLibrary.Readers.CS203.GetGPI0(ip);
            if (port == 1)
                state = HTKLibrary.Readers.CS203.GetGPI1(ip);

            return state;
        }


        // POST: api/GPIO
        public void Post(string ip, int port, bool on)
        {
            if(port == 0)
                HTKLibrary.Readers.CS203.SetGPO0(ip, on);
            if (port == 1)
                HTKLibrary.Readers.CS203.SetGPO0(ip, on);
        }

    }
}
