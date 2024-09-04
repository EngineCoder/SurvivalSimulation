using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STools.Tool_Manager
{
    public delegate void Delegate_Message(Message message);

    public class Manager_Message : SingleMode<Manager_Message>, IDispatcher
    {
        private Dictionary<int, Delegate_Message> messages = new Dictionary<int, Delegate_Message>();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void Subscribe(int type, Delegate_Message listener)
        {
            if (listener == null)
            {
                //XMDebug.LogError("AddListener: listener不能为空");
                return;
            }
            Delegate_Message myListener = null;
            messages.TryGetValue(type, out myListener);
            messages[type] = (Delegate_Message)Delegate.Combine(myListener, listener);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void UnSubscribe(int type, Delegate_Message listener)
        {
            if (listener == null)
            {
                //XMDebug.LogError("RemoveListener: listener不能为空");
                return;
            }
            messages[type] = (Delegate_Message)Delegate.Remove(messages[type], listener);
        }

        public void SendMessage(Message evt)
        {
            Delegate_Message listenerDelegate;
            if (messages.TryGetValue(evt.Type, out listenerDelegate))
            {
                try
                {
                    listenerDelegate?.Invoke(evt);
                }
                catch (System.Exception e)
                {
                    //XMDebug.LogError("SendMessage:", evt.Type.ToString(), e.Message, e.StackTrace, e);
                }
            }
        }

        public void SendMessage(int type, params object[] param)
        {
            Delegate_Message listenerDelegate;
            if (messages.TryGetValue(type, out listenerDelegate))
            {
                Message evt = new Message(type, param);
                try
                {
                    listenerDelegate?.Invoke(evt);
                }
                catch (System.Exception e)
                {
                    //XMDebug.LogError("SendMessage:", evt.Type.ToString(), e.Message, e.StackTrace, e);
                }
            }
        }

        public void ClearAllMessage()
        {
            messages.Clear();
        }



        #region AddListener & RemoveListener &SendMessage

        public void Subscribe(MessageType_Common type, Delegate_Message listener)
        {
            Subscribe((int)type, listener);
        }
        public void Subscribe(MessageType_Battle type, Delegate_Message listener)
        {
            Subscribe((int)type, listener);
        }
        public void Subscribe(MessageType_Protocol type, Delegate_Message listener)
        {
            Subscribe((int)type, listener);
        }

        public void UnSubscribe(MessageType_Common type, Delegate_Message listener)
        {
            UnSubscribe((int)type, listener);
        }
        public void UnSubscribe(MessageType_Battle type, Delegate_Message listener)
        {
            UnSubscribe((int)type, listener);
        }
        public void UnSubscribe(MessageType_Protocol type, Delegate_Message listener)
        {
            UnSubscribe((int)type, listener);
        }

        public void SendMessage(MessageType_Common type, params System.Object[] param)
        {
            SendMessage((int)type, param);
        }
        public void SendMessage(MessageType_Battle type, params System.Object[] param)
        {
            SendMessage((int)type, param);
        }
        public void SendMessage(MessageType_Protocol type, params System.Object[] param)
        {
            SendMessage((int)type, param);
        }

        #endregion
    }


    #region Enum
    /// <summary>
    /// 常用消息类型
    /// </summary>
    public enum MessageType_Common
    {
        Common_01 = 1
    }
    /// <summary>
    /// 战斗类型的消息
    /// </summary>
    public enum MessageType_Battle
    {
        Battle_01 = 10001
    }
    /// <summary>
    /// 协议类型的消息
    /// </summary>
    public enum MessageType_Protocol
    {
        Protocol_01 = 20001
    }
    #endregion

    #region Interface

    /// <summary>
    /// Interface Message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        object Sender { get; set; }

        /// <summary>
        /// 参数数组
        /// </summary>
        object[] Params { get; set; }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        string ConvertToString();
    }

    /// <summary>
    /// Concrete Message
    /// </summary>
    public class Message : IMessage
    {
        public int Type { get; set; }
        public object Sender { get; set; }
        public object[] Params { get; set; }

        public Message(int type)
        {
            Type = type;
        }
        public Message(int type, params object[] param)
        {
            Type = type;
            Params = param;
        }
        public Message(int type, object sender, params object[] param)
        {
            Type = type;
            Sender = sender;
            Params = param;
        }

        public Message CreateNewMessage()
        {
            return new Message(Type, Sender, Params);
        }

        public string ConvertToString()
        {
            string arg = null;
            if (Params != null)
            {
                for (int i = 0; i < Params.Length; i++)
                {
                    if ((Params.Length > 1 && Params.Length - 1 == i) || Params.Length == 1)
                    {
                        arg += Params[i];
                    }
                    else
                    {
                        arg += Params[i] + ",";
                    }
                }
            }
            return Type + " [ " + ((Sender == null) ? "null" : Sender.ToString()) + " ] " + " [ " + ((arg == null) ? "null" : arg) + " ] ";
        }
    }

    /// <summary>
    /// 发送者
    /// </summary>
    public interface IDispatcher
    {
        void Subscribe(int type, Delegate_Message listener);

        void UnSubscribe(int type, Delegate_Message listener);

        void SendMessage(Message evt);
        void SendMessage(int type, params object[] param);

        void ClearAllMessage();
    }

    #endregion


}