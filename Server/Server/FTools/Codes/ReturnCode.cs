using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Codes
{
    public enum ReturnCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 1,

        /// <summary>
        /// The parameter out of range.参数超出范围
        /// </summary>
        ParameterOutOfRange = 51,

        /// <summary>
        /// 不支持的操作
        /// </summary>
        OperationNotSupported,

        /// <summary>
        /// 无效的操作参数
        /// </summary>
        InvalidOperationParameter,

        /// <summary>
        /// 无效的操作
        /// </summary>
        InvalidOperation,

        /// <summary>
        /// The avatar access denied.访问被拒
        /// </summary>
        ItemAccessDenied,

        /// <summary>
        /// interest area not found.
        /// </summary>
        InterestAreaNotFound,

        /// <summary>
        /// The interest area already exists.
        /// </summary>
        InterestAreaAlreadyExists,

        /// <summary>
        /// The world already exists.
        /// </summary>
        WorldAlreadyExists = 101,

        /// <summary>
        /// The world not found.
        /// </summary>
        WorldNotFound,

        /// <summary>
        /// The item already exists.
        /// </summary>
        ItemAlreadyExists,

        /// <summary>
        /// The item not found.
        /// </summary>
        ItemNotFound
    }
}
