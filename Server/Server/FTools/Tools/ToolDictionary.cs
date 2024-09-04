using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Tools
{
    /// <summary>
    /// 通过Key,获得Value
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    public static partial class Tool
    {
        public static V GetValue<K, V>(Dictionary<K, V> dict, K k)
        {
            V v;
            bool isHave = dict.TryGetValue(k, out v);
            if (isHave)
            {
                return v;
            }
            else
            {
                return default(V);
            }
        }
    }
}
