using FTools.Community;
using NHibernate;
using NHibernate.Criterion;
using Server.Interface;
using System;
using System.Collections.Generic;
namespace Server.Manager
{
    class UserTable : IOperationTable<User>
    {
        public void Delete(User t)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(t);
                    transaction.Commit();
                }
            }
        }
        public ICollection<User> GetAll()
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                IList<User> users = session.CreateCriteria(typeof(User)).List<User>();
                return users;
            }
        }
        public bool Insert(User t)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(t);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Server.log.Info(e);
                        return false;
                    }
                }
            }
        }
        public User SelectByColumnName(string columnName, object value)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User user = session.CreateCriteria(typeof(User))
                        .Add(Restrictions.Eq(columnName, value))
                        .UniqueResult<User>();
                    transaction.Commit();
                    return user;
                }
            }
        }
        public IList<User> SelectsByColumnName(string columnName, object value)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    IList<User> users = session.CreateCriteria(typeof(User))
                        .Add(Restrictions.Eq(columnName, value))
                        .List<User>();
                    transaction.Commit();
                    return users;
                }
            }
        }
        public User SelectByColumnName1AndColumnName2(string columnName1, string columnName2, string value1, string value2)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                User user = session.CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq(columnName1, value1))
                    .Add(Restrictions.Eq(columnName2, value2))
                    .UniqueResult<User>();
                return user;
            }
        }
        public void Update(User t)
        {
            using (ISession session = NHibernateUtility.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(t);
                    transaction.Commit();
                }
            }
        }
    }
}
