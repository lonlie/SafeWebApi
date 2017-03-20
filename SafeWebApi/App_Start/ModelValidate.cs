using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace SafeWebApi
{
    /// <summary>
    /// 模型验证
    /// </summary>
    public class ModelValidate : ActionFilterAttribute
    {
        /// <summary>
        /// 重写
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var isValidate = true;
            var result = new Result() { Status = RStatus.S0009 };

            //参数为空
            if (actionContext.ActionArguments.Any(kv => kv.Value == null))
            {
                isValidate = false;
            }
            //参数验证失败
            else if (!actionContext.ModelState.IsValid)
            {
                isValidate = false;
                result.Desc = GetErrors(actionContext.ModelState);
            }

            if (!isValidate)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }

        /// <summary>
        /// 获取ModelValid信息
        /// </summary>
        /// <returns></returns>
        /// <remarks>author:lorne date:2015-12-04</remarks>
        protected string GetErrors(ModelStateDictionary state)
        {
            StringBuilder msg = new StringBuilder();
            foreach (var item in state.Values.Where(v => v.Errors.Count > 0).ToList())
            {
                foreach (var error in item.Errors)
                {
                    msg.AppendLine(error.ErrorMessage);
                }
            }
            return msg.ToString();
        }
    }
}