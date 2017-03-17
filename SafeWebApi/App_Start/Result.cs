using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SafeWebApi
{
    /// <summary>
    /// 方法返回结果对象
    /// </summary>
    /// <remarks>author:lorne date:2016-07-19</remarks>
    public class Result
    {
        public Result(RStatus status = RStatus.S0000)
        {
            this.Status = status;
        }

        private RStatus status;
        /// <summary>
        /// 状态
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RStatus Status
        {
            get { return status; }
            set
            {
                status = value;

                //为状态描述赋值
                string name = Enum.GetName(typeof(RStatus), value);
                object[] objs = typeof(RStatus).GetField(name).GetCustomAttributes(false);
                if (objs.Length > 0)
                    Desc = (objs[0] as DescriptionAttribute).Description;
                else
                    Desc = name;
            }
        }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPass { get { return status == RStatus.S0001; } }
    }


    /// <summary>
    /// 方法返回结果对象（包括单个返回数据）
    /// author:lorne
    /// date:2016-07-19
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultSingle<T> : Result
    {
        /// <summary>
        /// 单个返回数据
        /// </summary>
        public T ReturnObject { get; set; }
    }

    /// <summary>
    /// 方法返回结果对象（包括列表返回数据）
    /// author:lorne
    /// date:2016-07-19
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultList<T> : Result
    {
        /// <summary>
        /// 列表返回数据
        /// </summary>
        [JsonProperty]
        public IEnumerable<T> ReturnList { get; set; }

        /// <summary>
        /// 列表数据的总量（用于分页）
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 单页数据量
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Count / Convert.ToDecimal(PageSize)));
            }
            set { PageCount = value; }
        }
    }

    public class ResultList<T, C> : Result
    {
        /// <summary>
        /// 列表返回数据
        /// </summary>
        [JsonProperty]
        public IEnumerable<T> ReturnList { get; set; }

        /// <summary>
        /// 列表数据的总量（用于分页）
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 单页数据量
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(Count / Convert.ToDecimal(PageSize)));
            }
            set { PageCount = value; }
        }

        /// <summary>
        /// 统计信息
        /// </summary>
        public C CountInfo { get; set; }
    }


    /// <summary>
    /// 方法返回结果状态枚举
    /// author:lorne
    /// date:2016-07-19
    /// </summary>
    public enum RStatus
    {
        #region 全局适用状态

        /// <summary>
        /// 初始状态
        /// </summary>
        [Description("初始状态")]
        S0000,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        S0001,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        S0002,
        /// <summary>
        /// Token无效
        /// </summary>
        [Description("Token无效")]
        S0003,
        /// <summary>
        /// Token已失效
        /// </summary>
        [Description("Token已失效")]
        S0004,
        /// <summary>
        /// 未授权
        /// </summary>
        [Description("未授权")]
        S0005,
        /// <summary>
        /// 参数有误
        /// </summary>
        [Description("参数有误")]
        S0009,

        #endregion

        S9999
    }
}