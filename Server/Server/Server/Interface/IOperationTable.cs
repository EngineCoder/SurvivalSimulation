using System.Collections.Generic;

namespace Server.Interface
{
    interface IOperationTable<T>
    {
        //==============================  Select  ============================================

        #region Select
        T SelectByColumnName(string columnName,object value);
        IList<T> SelectsByColumnName(string columnName,object value);
        T SelectByColumnName1AndColumnName2(string columnName1, string columnName2, string value1, string value2);
        ICollection<T> GetAll();
        #endregion

        //==============================  Insert  ============================================

        #region Insert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="user"></param>
        bool Insert(T t);
        #endregion

        //==============================  Update  ============================================

        #region Update
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="user"></param>
        void Update(T t);

        #endregion

        //==============================  Delete  ============================================

        #region Delete

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="user"></param>
        void Delete(T t);

        #endregion
    }
}
