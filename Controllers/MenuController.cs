using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewOPAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewOPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        // GET: api/<MenuController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MenuController>/string
        [HttpGet("{Name}")]
        public int? Get(string Name)
        {
            return HttpContext.Session.GetInt32(Name).GetValueOrDefault();
        }

        // POST api/<MenuController>
        [HttpPost]
        public void Post([FromBody] Words menu)
        {
            HttpContext.Session.SetInt32(menu.Text, menu.Id);
        }

        // PUT api/<MenuController>/5
        [HttpPut]
        public void Put([FromBody] Words menu)
        {
            HttpContext.Session.SetInt32(menu.Text, menu.Id);
        }

        // DELETE api/<MenuController>/5
        [HttpDelete("{Name,Id}")]
        public void Delete(int Id)
        {
        }
    }
}
