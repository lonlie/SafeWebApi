using System;
using System.Collections.Generic;
using System.Linq;
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

            //Token认证不通过
            IEnumerable<string> values;
            var haveToken = actionContext.Request.Headers.TryGetValues("token", out values);
            if (haveToken && TokenTool.CheckToken(values.ToList()[0]))
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                var response = HttpContext.Current.Response;
                //response.ContentType = "application/json";
                response.Write("{firstName: \"John\"}");
                response.End();
            }
        }
    }
}