using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class NHibernateUtility
    {
        private static ISessionFactory sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();//解析hibernate.cfg.xml配置文件，进行配置
                    configuration.AddAssembly("FServer");//解析各映射文件 如：User.hbm.xml...
                    sessionFactory = configuration.BuildSessionFactory();//建造一个会话工厂
                }
                return sessionFactory;
            }
        }

        /// <summary>
        /// 打开一个会话，与数据库进行通信
        /// </summary>
        /// <returns></returns>
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
