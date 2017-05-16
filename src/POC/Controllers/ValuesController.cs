using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using POC.Shizzle;

namespace POC.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IMeep _meep;
        public ValuesController(IMeep meep)
        {
            _meep = meep;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            throw new Exception("fdog");
            //return new string[] { "value1", _meep.GetPath() };
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "getbyid")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            return new CreatedAtRouteResult("getbyid", new { Id = "meep" }, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
