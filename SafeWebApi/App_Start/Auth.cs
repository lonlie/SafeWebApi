using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SafeWebApi
{
    /// <summary>
    /// 认证过滤器
    /// </summary>
    public class Auth : ActionFilterAttribute
    {
        /// <summary>
        /// 重写
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //允许匿名访问的无需处理
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                var result = new Result();
                IEnumerable<string> values;
                var haveToken = actionContext.Request.Headers.TryGetValues("token", out values);

                //是否存在Token
                if (haveToken)
                {
                    var token = values.ToList()[0];
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        result = TokenTool.CheckToken(values.ToList()[0]);
                        if (result.IsPass)
                        {
                            base.OnActionExecuting(actionContext);
                        }
                    }
                    else
                    {
                        result.Status = RStatus.S0003;
                    }
                }
                else
                {
                    result.Status = RStatus.S0005;
                }

                //返回错误信息
                if (!result.IsPass)
                {
                    var response = HttpContext.Current.Response;
                    response.ContentType = "application/json;charset=utf-8";
                    response.ClearContent();
                    byte[] bytes = null;

                    bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(result, new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
                    response.BinaryWrite(bytes);
                }
            }
        }
    }
}