using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Utils
{
    public static partial class Tool
    {
        #region protobuf.net Serialize

        /// <summary>
        /// 序列化，使用protobuf.net将对象序列化成字节数组
        /// </summary>
        /// <typeparam name="T">实例的类型</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] SerializeByProtobufNet<T>(T t)
        {
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, t);
                bytes = new byte[ms.Position];
                var fullBytes = ms.GetBuffer();
                Array.Copy(fullBytes, bytes, bytes.Length);
            }
            return bytes;
        }
        /// <summary>
        /// 反序列化，使用Protobuf-net将字节数组转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T DeSerializeByProtobufNet<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        #endregion

        #region Newtonsoft.Json Serialize
        /// <summary>
        /// 序列化，使用Newtonsoft.json将对象序列化为Json字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeToJsonStrByNewtonsoft(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
        /// <summary>
        /// 反序列化，将Newtonsoft.Json将Json字符串转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T DeSerializeByNewtonsoft<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }


        /// <summary>
        /// 序列化，将对象序列化为字节数组
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] SerializeToByteArrayByNewtonsoft(object o)
        {
            return Encoding.Default.GetBytes(JsonConvert.SerializeObject(o));
        }
        /// <summary>
        /// 反序列化，使用Newtonsoft.Json将byte数组转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T DeSerializeByNewtonsoft<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(bytes));
        }
        #endregion

    }
}
