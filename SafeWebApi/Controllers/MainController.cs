using Newtonsoft.Json.Serialization;
using SafeWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SafeWebApi.Controllers
{
    /// <summary>
    /// Main
    /// </summary>
    [Auth]
    public class MainController : ApiController
    {
        /// <summary>
        /// 测试获取
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        // GET: api/Main
        public Result Get()
        {
            var setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return new ResultSingle<dynamic>() { ReturnObject = new { Token = TokenTool.GetToken(Guid.NewGuid()) }, Status = RStatus.S0001 };
        }

        // GET: api/Main/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Main
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Main/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Main/5
        public void Delete(int id)
        {
        }
    }
}
