using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ik.WebFramework.Api;
using Ik.WebFramework;

namespace Ik.Framework.Web.Controllers
{

    public class ValuesController : ApiController
    {

        public ValuesController()
        {

        }

        [HttpPost]
        // GET api/values
        public IEnumerable<string> Get(GridQuery query)
        {

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
