/* ---------------------------------------------------------------------- 
 * DBAccess .NET Framework®
 * Copyright © 2017, Luiz Filipe Miranda de Oliveira.
 * ----------------------------------------------------------------------
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 *     1. Redistributions of source code must retain the above
 *        copyright notice, this list of conditions and the following
 *        disclaimer.
 *     2. Redistributions in binary form must reproduce the above
 *        copyright notice, this list of conditions and the following
 *        disclaimer in the documentation and/or other materials
 *        provided with the distribution.
 *
 *     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 *     CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 *     INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 *     MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 *     DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 *     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 *     SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 *     NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 *     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *     HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 *     CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 *     OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 *     EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * ----------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using LTM.DBAccess.Attributes;
using LTM.DBAccess.Exceptions;

namespace LTM.DBAccess.Persistence
{

    #region [ DBAccess Class ]
    /// <summary>
    /// Mobot.DBAccess Framework .NET to Data Persistence
    /// </summary>
    [Obsolete("The method DBAccessReflectionMethods is obsolete.")]
    public sealed class DBAccess : IDisposable
    {

        #region [ Constant ]
        private const string msgErrorMethodInvalid_ORACLE = "This method has no implementation for Oracle.";
        private const string msgErrorMethodInvalid_SQLSERVER = "This method has no implementation for SQLServer.";
        #endregion

        #region [ Attributes ]
        private string dbvariable;

        // UTILITIES
        Modules.DataBase dataBaseType;

        // SQL SERVER
        private SqlConnection connSS = null;
        private SqlCommand cmdSS = null;
        private SqlTransaction transSS;

        // ORACLE
        OracleConnection connORA;
        OracleCommand cmdORA;
        OracleTransaction transORA;
        #endregion

        #region [ Constructor ]
        /// <summary>
        /// Constructor to DBAccess Class (Include "LTM.DBAccess.ConnectionString" in ConnectionString Tag on Web.Config)
        /// </summary>
        public DBAccess()
        {
            if (Modules.DBModule.GetDataBaseModule == Modules.DataBase.SQLSERVER)
            {
                dbvariable = "@";
                dataBaseType = Modules.DataBase.SQLSERVER;
                connSS = (SqlConnection)GetConnection();
                cmdSS = new SqlCommand();
            }
            else
            {
                dbvariable = ":";
                dataBaseType = Modules.DataBase.ORACLE;
                connORA = (OracleConnection)GetConnection();
                cmdORA = connORA.CreateCommand();
            }
        }
        /// <summary>
        /// Constructor to DBAccess Class with Connection String
        /// </summary>
        /// <param name="connectionStringName">Connection String Name</param>
        public DBAccess(string connectionStringName)
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                Modules.DBModule.ConnectionStringName = connectionStringName;
                if (Modules.DBModule.GetDataBaseModule == Modules.DataBase.SQLSERVER)
                {
                    connSS = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName] + "; Asynchronous Processing=true;");
                    cmdSS = new SqlCommand();
                    dbvariable = "@";
                    dataBaseType = Modules.DataBase.SQLSERVER;
                }
                else
                {
                    connORA = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ToString());
                    cmdORA = connORA.CreateCommand();
                    dbvariable = ":";
                    dataBaseType = Modules.DataBase.ORACLE;
                }
            }
            else
            {
                throw new DBAccessException("No connection string in web.config informed");
            }

        }
        #endregion

        #region [ ConnectionString ]
        /// <summary>
        /// Method to get ConnectionStrings (Web.Config)
        /// </summary>
        /// <returns>SqlConnection Instance</returns>
        private DbConnection GetConnection()
        {
            if (System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"] != null)
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"] + "; Asynchronous Processing=true;");
                else
                    return new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"].ToString());
            else
                throw new DBAccessException("No connection string in web.config informed");
        }
        #endregion

        #region [ Modules ]
        // -----------------------------------------------
        // Modules DBAccess Framework
        // -----------------------------------------------

        #region [ DBAccess Structure Methods ]

        #region [ Startup, Open and Close Methods ]
        /// <summary>
        /// Method to open connection base
        /// </summary>
        private void Open()
        {
            try
            {
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                {
                    if (connSS.State != ConnectionState.Open)
                        connSS.Open();
                }
                else
                {
                    if (connORA.State != ConnectionState.Open)
                        connORA.Open();
                }
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
        }

        /// <summary>
        /// Method to close connection
        /// </summary>
        public void Close()
        {
            Close(false);
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="isDestructor">True or false to destructor</param>
        public void Close(bool isDestructor)
        {
            try
            {
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    cmdSS.Dispose();
                else
                    cmdORA.Dispose();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            finally
            {
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                {
                    if (connSS.State != ConnectionState.Closed && transSS == null)
                        connSS.Close();
                }
                else
                {
                    if (connORA.State != ConnectionState.Closed && transORA == null)
                        connORA.Close();
                }
            }
        }

        /// <summary>
        /// Set Command and clear last parameters
        /// </summary>
        /// <param name="sql">Query to execute</param>
        public DBAccess SetCommand(string sql)
        {
            return SetCommand(sql, CommandType.Text);
        }

        /// <summary>
        /// Set Command and clear last parameters
        /// </summary>
        /// <param name="sql">Query or StoredProcedure name to execute</param>
        /// <param name="cmdType">Command type</param>
        public DBAccess SetCommand(string sql, CommandType cmdType)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
            {
                cmdSS.Parameters.Clear();
                cmdSS.CommandText = sql;
                cmdSS.Connection = connSS;
                cmdSS.CommandType = cmdType;
                if (transSS != null)
                    cmdSS.Transaction = transSS;
            }
            else
            {
                cmdORA.Parameters.Clear();
                cmdORA.CommandText = sql;
                cmdORA.Connection = connORA;
                cmdORA.CommandType = cmdType;
                if (transORA != null)
                    cmdORA.Transaction = transORA;
            }
            return this;
        }
        #endregion

        #region [ Transaction Methods ]
        /// <summary>
        /// Get Transaction enabled
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return (dataBaseType == Modules.DataBase.SQLSERVER
                          ? transSS != null
                          : transORA != null);
            }
        }
        /// <summary>
        /// Begin transaction
        /// </summary>
        public void BeginTransaction()
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER
                    ? transSS != null
                    : transORA != null)
                throw new DBAccessException("Transaction already open.");
            Open();
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                transSS = connSS.BeginTransaction();
            else
                transORA = connORA.BeginTransaction();
        }
        /// <summary>
        /// Commit transaction
        /// </summary>
        public void CommitTransaction()
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER
                    ? transSS == null
                    : transORA == null)
                throw new DBAccessException("There is no transaction to be commited.");
            if (dataBaseType == Modules.DataBase.SQLSERVER)
            {
                transSS.Commit();
                transSS = null;
            }
            else
            {
                transORA.Commit();
                transORA = null;
            }
        }
        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void RollBackTransaction()
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER
                    ? transSS == null
                    : transORA == null)
                throw new DBAccessException("There is no transaction to be undone.");
            if (dataBaseType == Modules.DataBase.SQLSERVER)
            {
                transSS.Rollback();
                transSS = null;
            }
            else
            {
                transORA.Rollback();
                transORA = null;
            }
        }
        #endregion

        #region [ Parameters Methods ]
        /// <summary>
        /// Add parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Type</param>
        public DBAccess AddParameter(string name, object dbType)
        {
            return AddParameter(name, dbType, null, -1);
        }

        /// <summary>
        /// Add parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Type</param>
        /// <param name="value">Value</param>
        public DBAccess AddParameter(string name, object dbType, object value)
        {
            return AddParameter(name, dbType, value, -1);
        }

        /// <summary>
        /// Add parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Type</param>
        /// <param name="value">Value</param>
        /// <param name="size">Size</param>
        public DBAccess AddParameter(string name, object dbType, object value, int size)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
            {
                // SQL Server
                if (value == null)
                {
                    if (size < 0)
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType).Value = DBNull.Value;
                    else
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType, size).Value = DBNull.Value;
                }
                else if (value.GetType().Equals(typeof(DateTime)))
                {
                    DateTime dt = (DateTime)value;
                    if (dt == DateTime.MinValue)
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType).Value = DBNull.Value;
                    else
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType).Value = dt;
                }
                else
                {
                    if (size < 0)
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType).Value = value;
                    else
                        cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (SqlDbType)dbType, size).Value = value;
                }
            }
            else
            {
                // Oracle
                if (value == null)
                {
                    if (size < 0)
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType).Value = DBNull.Value;
                    else
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType, size).Value = DBNull.Value;
                }
                else if (value.GetType().Equals(typeof(DateTime)))
                {
                    DateTime dt = (DateTime)value;
                    if (dt == DateTime.MinValue)
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType).Value = DBNull.Value;
                    else
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType).Value = dt;
                }
                else
                {
                    if (size < 0)
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType).Value = value;
                    else
                        cmdORA.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), (OracleType)dbType, size).Value = value;
                }
            }
            return this;
        }

        /// <summary>
        /// Add output parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="sqlType">Type</param>
        public void AddOutputParameter(string name, SqlDbType sqlType)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), sqlType).Direction = ParameterDirection.Output;
        }

        /// <summary>
        /// Add output parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="sqlType">Type</param>
        /// <param name="size">Size</param>
        public void AddOutputParameter(string name, SqlDbType sqlType, int size)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                cmdSS.Parameters.Add((name.StartsWith(dbvariable) ? name : dbvariable + name), sqlType, size).Direction = ParameterDirection.Output;
        }

        /// <summary>
        /// Add input and output parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Type</param>
        /// <param name="value">Value</param>
        public void AddInputOutputParameter(string name, object dbType, object value)
        {
            AddParameter(name, dbType, value);
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                cmdSS.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Direction = ParameterDirection.InputOutput;
            else
                cmdORA.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Direction = ParameterDirection.InputOutput;
        }

        /// <summary>
        /// Add input and output parameter to SqlCommmand
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Type</param>
        /// <param name="value">Value</param>
        /// <param name="size">Size</param>
        public void AddInputOutputParameter(string name, object dbType, object value, int size)
        {
            AddParameter(name, dbType, value, size);
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                cmdSS.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Direction = ParameterDirection.InputOutput;
            else
                cmdORA.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Direction = ParameterDirection.InputOutput;
        }

        /// <summary>
        /// Get parameter value
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Object value</returns>
        public object GetParameterValue(string name)
        {
            object obj;
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                obj = cmdSS.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Value;
            else
                obj = cmdORA.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Value;
            return (obj.GetType().FullName.Equals("System.DBNull") ? null : obj);
        }

        /// <summary>
        /// Set parameter value
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Value</param>
        public void SetParameterValue(string name, object value)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
                cmdSS.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Value = (value == null ? DBNull.Value : value);
            else
                cmdORA.Parameters[(name.StartsWith(dbvariable) ? name : dbvariable + name)].Value = (value == null ? DBNull.Value : value);
        }

        /// <summary>
        /// Add parameters array
        /// </summary>
        /// <param name="fields">Array fields</param>
        /// <param name="types">Array types</param>
        /// <param name="values">Array values</param>
        public void AddArrayParameters(string[] fields, SqlDbType[] types, object[] values)
        {
            for (int i = 0; i < fields.Length; i++)
                AddParameter(fields[i], types[i], values[i]);
        }
        #endregion

        #region [ Execute Methods ]
        /// <summary>
        /// Execute command and Get Identity
        /// </summary>
        /// <returns>Identity</returns>
        public int ExecuteAndGetIdentity()
        {
            return ExecuteAndGetIdentity(null);
        }

        /// <summary>
        /// Execute command and Get Identity
        /// </summary>
        /// <returns>Identity</returns>
        public int ExecuteAndGetIdentity(string primaryKey)
        {
            int ret = -1;
            try
            {
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                {
                    if (cmdSS.CommandType != CommandType.Text)
                        throw new DBAccessException("Invalid CommandType. Only CommandType.Text is accepted.");
                    if (cmdSS.CommandText.IndexOf("SELECT @@IDENTITY") < 0)
                        cmdSS.CommandText += "; SELECT @@IDENTITY";
                    Open();
                    ret = int.Parse(cmdSS.ExecuteScalar().ToString());
                }
                else
                {
                    OracleParameter lastId = new OracleParameter(":retDBAccess", OracleType.Int32);
                    if (cmdORA.CommandType != CommandType.Text)
                        throw new DBAccessException("Invalid CommandType. Only CommandType.Text is accepted.");
                    if (!cmdORA.CommandText.Contains("RETURNING"))
                    {
                        cmdORA.CommandText += string.Format(" RETURNING {0} INTO :retDBAccess", primaryKey);
                        AddParameter(":retDBAccess", DbType.Int32, 0);
                        lastId = new OracleParameter(":retDBAccess", OracleType.Int32);
                        lastId.Direction = ParameterDirection.ReturnValue;
                        cmdORA.Parameters.Add(lastId);
                    }
                    Open();
                    cmdORA.ExecuteNonQuery();
                    ret = int.Parse(lastId.Value.ToString());
                }
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return ret;
        }

        /// <summary>
        /// ExecuteNonQuery command
        /// </summary>
        /// <returns>True or false</returns>
        public bool ExecuteNonQuery()
        {
            bool ret = false;
            try
            {
                Open();
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                {
                    IAsyncResult r;
                    r = cmdSS.BeginExecuteNonQuery();
                    cmdSS.EndExecuteNonQuery(r);
                }
                else
                {
                    cmdORA.ExecuteNonQuery();
                }
                ret = true;
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return ret;
        }

        /// <summary>
        /// ExecuteReader command
        /// </summary>
        /// <returns>SqlDataReader value</returns>
        public DbDataReader ExecuteReader()
        {
            DbDataReader sdr = null;
            try
            {
                Open();
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    sdr = cmdSS.ExecuteReader(CommandBehavior.CloseConnection);
                else
                    sdr = cmdORA.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return sdr;
        }

        /// <summary>
        /// ExecuteXmlReader command (NOT IMPLEMENTED FOR THE ORACLE DATABASE)
        /// </summary>
        /// <returns>XmlReader value</returns>
        public XmlReader ExecuteXmlReader()
        {
            XmlReader xr = null;
            try
            {
                Open();
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    xr = cmdSS.ExecuteXmlReader();
                else
                    throw new Exception(msgErrorMethodInvalid_ORACLE);
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return xr;
        }

        /// <summary>
        /// ExecuteScalar command
        /// </summary>
        /// <remarks>Teste</remarks>
        /// <returns>Object value</returns>
        public object ExecuteScalar()
        {
            object ret = null;
            try
            {
                Open();
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    ret = cmdSS.ExecuteScalar();
                else
                    ret = cmdORA.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return ret;
        }
        #endregion

        #region [ Execute Returning Data Structure Methods ]
        /// <summary>
        /// ExecuteAndGetDataTable command
        /// </summary>
        /// <returns>DataTable value</returns>
        public DataTable ExecuteAndGetDataTable()
        {
            return ExecuteAndGetDataTable("");
        }

        /// <summary>
        /// ExecuteAndGetDataTable
        /// </summary>
        /// <param name="TableName">Table name</param>
        /// <returns>DataTable value</returns>
        public DataTable ExecuteAndGetDataTable(string TableName)
        {
            DataTable dt;
            if (string.IsNullOrEmpty(TableName))
                dt = new DataTable();
            else
                dt = new DataTable(TableName);
            try
            {
                DbDataAdapter adapter;
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    adapter = new SqlDataAdapter(cmdSS);
                else
                    adapter = new OracleDataAdapter(cmdORA);
                adapter.Fill(dt);
                adapter.Dispose();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return dt;
        }

        /// <summary>
        /// ExecuteAndGetGenericList
        /// </summary>
        /// <returns>List of object value</returns>
        public List<T> ExecuteAndGetGenericList<T>()
        {
            List<T> list = new List<T>();
            DataTable dt = new DataTable();
            try
            {
                DbDataAdapter adapter;
                
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    adapter = new SqlDataAdapter(cmdSS);
                else
                    adapter = new OracleDataAdapter(cmdORA);
                adapter.Fill(dt);
                adapter.Dispose();
                foreach (DataRow row in dt.Rows)
                    list.Add((T)this.SetPropertiesReflection(Activator.CreateInstance<T>(), row));
                dt.Dispose();
            }
            catch (DBAccessException e)
            {
                throw new DBAccessException(e);
            }
            return list;
        }

        /// <summary>
        /// ExecuteAndGetDataSet command
        /// </summary>
        /// <returns>DataSet value</returns>
        public DataSet ExecuteAndGetDataSet()
        {
            DataSet ds = new DataSet();
            try
            {
                DbDataAdapter adapter;
                if (dataBaseType == Modules.DataBase.SQLSERVER)
                    adapter = new SqlDataAdapter(cmdSS);
                else
                    adapter = new OracleDataAdapter(cmdORA);
                adapter.Fill(ds);
                adapter.Dispose();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return ds;
        }

        /// <summary>
        /// ExecuteAndGetDataRow command
        /// </summary>
        /// <returns>DataRow value</returns>
        public DataRow ExecuteAndGetDataRow()
        {
            try
            {
                DataTable dt = ExecuteAndGetDataTable();
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            return null;
        }

        /// <summary>
        /// ExecuteAndGetXmlDocument command
        /// </summary>
        /// <returns>XmlDocument value</returns>
        public XmlDocument ExecuteAndGetXmlDocument()
        {
            XmlReader xmlReader = null;
            try
            {
                xmlReader = ExecuteXmlReader();
                XmlDocument xml = new XmlDocument();
                xml.Load(xmlReader);
                return xml;
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }

        /// <summary>
        /// ExecuteAndGetXPathDocument command
        /// </summary>
        /// <returns>XPathDocument value</returns>
        public XPathDocument ExecuteAndGetXPathDocument()
        {
            XmlReader xmlReader = null;
            try
            {
                xmlReader = ExecuteXmlReader();
                XPathDocument xpath = new XPathDocument(xmlReader, XmlSpace.Preserve);
                return xpath;
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }
        #endregion

        #endregion

        #region [ ObjectReflection Module ]

        #region [ Enum Reflection Operations ]
        private enum DBAccessReflectionOperations
        {
            Insert,
            Select,
            Update,
            Delete
        }
        #endregion

        #region [ Structs ]
        // Structs to Querys Reflection

        #region [ Struct to Where Condition ]
        /// <summary>
        /// Where Condition Class
        /// </summary>        
        private class DBAccessStructCondition
        {
            public string sLogic;
            public string sField;
            public string sComparison;
            public string sValue;
        }
        #endregion

        #region [ Struct to Fields and Values ]
        /// <summary>
        /// Fields and Values Class
        /// </summary>
        private class DBAccessStructParameter
        {
            public string sField;
            public string sValue;
        }
        #endregion

        #endregion

        #region [ Reflection Tools ]
        /// <summary>
        /// Get Generic Type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Generic Type</returns>
        private Type GetGenericType<T>()
        {
            Type t = typeof(T);
            if (typeof(T).GetGenericArguments().Length > 0)
                t = typeof(T).GetGenericArguments()[0];
            return t;
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>Table name</returns>
        private string GetTableName(object obj)
        {
            if (obj.GetType().GetCustomAttributes(true).Length > 0 && ((DBEntity)(obj.GetType().GetCustomAttributes(false)[0])).TableName != null)
                return ((DBEntity)(obj.GetType().GetCustomAttributes(false)[0])).TableName;
            else
                return obj.GetType().ToString().Split('.')[obj.GetType().ToString().Split('.').Length - 1].ToString();
        }

        /// <summary>
        /// Get Primary Key Column Name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetPrimaryKeyName(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            string columnName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttribute")
                    && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore"))
                {

                    // ColumnName
                    columnName = ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null
                                        ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                        : fields[i].Name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(columnName))
            {
                PropertyInfo[] prop = obj.GetType().GetProperties();
                columnName = null;
                for (int i = 0; i < prop.Length; i++)
                {
                    if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                    {
                        // ColumnName
                        columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                            ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                            : prop[i].Name;
                        break;
                    }
                }
            }
            return columnName;
        }

        /// <summary>
        /// Method to Return List of structFieldValue with Reflection
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>List of structFieldValue</returns>
        private List<DBAccessStructParameter> GetStructFieldToReflection(object obj)
        {
            List<DBAccessStructParameter> fieldValues = new List<DBAccessStructParameter>();
            FieldInfo[] fields = obj.GetType().GetFields();
            string columnName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                // ColumnName
                columnName = (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                    ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                    : fields[i].Name;
                // Not Primary Key
                if (fields[i].GetCustomAttributes(true) == null || fields[i].GetCustomAttributes(true).Length == 0 || ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.PrimaryKey)
                    fieldValues.Add(new DBAccessStructParameter() { sField = columnName, sValue = columnName });
            }

            PropertyInfo[] prop = obj.GetType().GetProperties();
            columnName = null;
            for (int i = 0; i < prop.Length; i++)
            {
                if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                {
                    // ColumnName
                    columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                        ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                        : prop[i].Name;
                    // Not Primary Key
                    if (prop[i].GetCustomAttributes(true) == null || prop[i].GetCustomAttributes(true).Length == 0 || ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.PrimaryKey)
                        fieldValues.Add(new DBAccessStructParameter() { sField = columnName, sValue = columnName });
                }
            }

            return fieldValues;
        }

        /// <summary>
        /// Method to Return List of structCondition with Reflection
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>List of structCondition</returns>
        private List<DBAccessStructCondition> GetStructConditionToReflection(object obj)
        {
            List<DBAccessStructCondition> Condition = new List<DBAccessStructCondition>();
            FieldInfo[] fields = obj.GetType().GetFields();
            int countCondition = 0;
            string logic = string.Empty;
            string columnName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                if (countCondition > 0) logic = "AND";
                if (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttribute")
                    && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore"))
                {

                    // ColumnName
                    columnName = ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null
                                        ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                        : fields[i].Name;
                    // Not Primary Key
                    if (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && (((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey || ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey))
                    {
                        Condition.Add(new DBAccessStructCondition() { sComparison = "=", sField = columnName, sLogic = logic, sValue = columnName });
                        countCondition++;
                    }
                }
            }

            PropertyInfo[] prop = obj.GetType().GetProperties();
            countCondition = 0;
            logic = string.Empty;
            columnName = null;
            for (int i = 0; i < prop.Length; i++)
            {
                if (countCondition > 0) logic = "AND";
                if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                {
                    // ColumnName
                    columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                        ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                        : prop[i].Name;
                    // Not Primary Key
                    if (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && (((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey || ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey))
                    {
                        Condition.Add(new DBAccessStructCondition() { sComparison = "=", sField = columnName, sLogic = logic, sValue = columnName });
                        countCondition++;
                    }
                }
            }

            return Condition;
        }

        /// <summary>
        /// Set Identity of Object Instance with Reflection
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <param name="value">Operation type</param>
        private void SetPrimaryKeyReflection(object obj, object value)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            string columnName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttribute")
                    && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore"))
                {

                    // ColumnName
                    columnName = ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null
                                        ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                        : fields[i].Name;

                    // Not Primary Key
                    if (((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey)
                    {
                        switch (value.GetType().Name)
                        {
                            case "Int16":
                                fields[i].SetValue(obj, Int16.Parse(value.ToString()));
                                break;
                            case "Int32":
                                fields[i].SetValue(obj, Int32.Parse(value.ToString()));
                                break;
                            case "Int64":
                                fields[i].SetValue(obj, Int64.Parse(value.ToString()));
                                break;
                            case "UInt16":
                                fields[i].SetValue(obj, UInt16.Parse(value.ToString()));
                                break;
                            case "UInt32":
                                fields[i].SetValue(obj, UInt32.Parse(value.ToString()));
                                break;
                            case "UInt64":
                                fields[i].SetValue(obj, UInt64.Parse(value.ToString()));
                                break;
                            case "Decimal":
                                fields[i].SetValue(obj, decimal.Parse(value.ToString()));
                                break;
                            case "String":
                            case "Char":
                                fields[i].SetValue(obj, value.ToString().Trim());
                                break;
                            case "Byte":
                                fields[i].SetValue(obj, Byte.Parse(value.ToString()));
                                break;
                            case "Boolean":
                                fields[i].SetValue(obj, bool.Parse(value.ToString()) ? 1 : 0);
                                break;
                            case "Float":
                                fields[i].SetValue(obj, float.Parse(value.ToString()));
                                break;
                            case "DateTime":
                                fields[i].SetValue(obj, Convert.ToDateTime(value));
                                break;
                        }
                    }
                }
            }

            PropertyInfo[] prop = obj.GetType().GetProperties();
            columnName = null;
            for (int i = 0; i < prop.Length; i++)
            {
                if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                {

                    // ColumnName
                    columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                        ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                        : prop[i].Name;

                    // Not Primary Key
                    if (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey)
                    {
                        switch (value.GetType().Name)
                        {
                            case "Int16":
                                prop[i].SetValue(obj, Int16.Parse(value.ToString()), new object[] { });
                                break;
                            case "Int32":
                                prop[i].SetValue(obj, Int32.Parse(value.ToString()), new object[] { });
                                break;
                            case "Int64":
                                prop[i].SetValue(obj, Int64.Parse(value.ToString()), new object[] { });
                                break;
                            case "UInt16":
                                prop[i].SetValue(obj, UInt16.Parse(value.ToString()), new object[] { });
                                break;
                            case "UInt32":
                                prop[i].SetValue(obj, UInt32.Parse(value.ToString()), new object[] { });
                                break;
                            case "UInt64":
                                prop[i].SetValue(obj, UInt64.Parse(value.ToString()), new object[] { });
                                break;
                            case "Decimal":
                                prop[i].SetValue(obj, decimal.Parse(value.ToString()), new object[] { });
                                break;
                            case "String":
                            case "Char":
                                prop[i].SetValue(obj, value.ToString().Trim(), new object[] { });
                                break;
                            case "Byte":
                                prop[i].SetValue(obj, Byte.Parse(value.ToString()), new object[] { });
                                break;
                            case "Boolean":
                                prop[i].SetValue(obj, (bool.Parse(value.ToString()) ? 1 : 0), new object[] { });
                                break;
                            case "Float":
                                prop[i].SetValue(obj, float.Parse(value.ToString()), new object[] { });
                                break;
                            case "DateTime":
                                prop[i].SetValue(obj, Convert.ToDateTime(value), new object[] { });
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Parameters of SqlCommand with Reflection
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <param name="reflectionOperation">Operation type</param>
        private void SetParametersReflection(object obj, DBAccessReflectionOperations reflectionOperation)
        {
            this.SetParametersReflection(obj, reflectionOperation, string.Empty);
        }

        /// <summary>
        /// Set Parameters of SqlCommand with Reflection
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <param name="reflectionOperation">Operation type</param>
        /// <param name="includeValue">Include value</param>
        private void SetParametersReflection(object obj, DBAccessReflectionOperations reflectionOperation, string includeValue)
        {
            bool sqlServer = (Modules.DBModule.GetDataBaseModule == Modules.DataBase.SQLSERVER);

            FieldInfo[] fields = obj.GetType().GetFields();
            string columnName = null;
            string typeName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                // ColumnName
                columnName = (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                    ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                    : fields[i].Name;

                columnName += includeValue;

                // Not Primary Key
                if (
                    (
                        (
                            fields[i].GetCustomAttributes(true) == null
                                || fields[i].GetCustomAttributes(true).Length == 0
                                || (
                                    ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.PrimaryKey && 
                                        (
                                            (
                                                ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.SecurityKey && 
                                                    reflectionOperation == DBAccessReflectionOperations.Update
                                             ) ||
                                             reflectionOperation == DBAccessReflectionOperations.Insert
                                        )
                                   )
                        )
                        && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")
                     )
                    )
                {

                    if (sqlServer)
                    {
                        if (fields[i].GetValue(obj) != null)
                        {
                            switch (fields[i].GetValue(obj).GetType().Name)
                            {
                                case "Int16":
                                case "Int32":
                                case "Int64":
                                case "UInt16":
                                case "UInt32":
                                case "UInt64":
                                    this.AddParameter(columnName, SqlDbType.Int, UInt64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Decimal":
                                    this.AddParameter(columnName, SqlDbType.Decimal, decimal.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "String":
                                case "Char":
                                    this.AddParameter(columnName, SqlDbType.VarChar, !string.IsNullOrEmpty(fields[i].GetValue(obj).ToString().Trim()) ? fields[i].GetValue(obj).ToString().Trim() : null);
                                    break;
                                case "Byte":
                                    this.AddParameter(columnName, SqlDbType.Bit, Byte.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Boolean":
                                    this.AddParameter(columnName, SqlDbType.Bit, bool.Parse(fields[i].GetValue(obj).ToString()) ? 1 : 0);
                                    break;
                                case "Float":
                                case "Single":
                                case "Double":
                                    this.AddParameter(columnName, SqlDbType.Decimal, float.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "DateTime":
                                    this.AddParameter(columnName, SqlDbType.DateTime, Convert.ToDateTime(fields[i].GetValue(obj)));
                                    break;
                            }
                        }
                        else
                        {
                            // Default
                            this.AddParameter(columnName, SqlDbType.VarChar, null);
                        }
                    }
                    else
                    {

                        if (fields[i].GetValue(obj) != null)
                        {
                            switch (fields[i].GetValue(obj).GetType().Name)
                            {
                                case "Int16":
                                    this.AddParameter(columnName, DbType.Int16, Int16.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Int32":
                                    this.AddParameter(columnName, DbType.Int32, Int32.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Int64":
                                    this.AddParameter(columnName, DbType.Int64, Int64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt16":
                                    this.AddParameter(columnName, DbType.UInt16, UInt16.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt32":
                                    this.AddParameter(columnName, DbType.UInt32, UInt32.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt64":
                                    this.AddParameter(columnName, DbType.UInt64, UInt64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Decimal":
                                    this.AddParameter(columnName, DbType.Decimal, decimal.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "String":
                                case "Char":
                                    this.AddParameter(columnName, DbType.String, !string.IsNullOrEmpty(fields[i].GetValue(obj).ToString().Trim()) ? fields[i].GetValue(obj).ToString().Trim() : null);
                                    break;
                                case "Byte":
                                    this.AddParameter(columnName, DbType.Byte, Byte.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Boolean":
                                    this.AddParameter(columnName, DbType.Byte, bool.Parse(fields[i].GetValue(obj).ToString()) ? 1 : 0);
                                    break;
                                case "Float":
                                case "Single":
                                case "Double":
                                    this.AddParameter(columnName, DbType.Decimal, float.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "DateTime":
                                    this.AddParameter(columnName, DbType.DateTime, Convert.ToDateTime(fields[i].GetValue(obj)));
                                    break;
                            }
                        }
                        else
                        {
                            // Default
                            this.AddParameter(columnName, DbType.String, null);
                        }
                    }
                }
            }

            PropertyInfo[] prop = obj.GetType().GetProperties();
            columnName = null;
            for (int i = 0; i < prop.Length; i++)
            {
                if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                {
                    // ColumnName
                    columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                        ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                        : prop[i].Name;

                    columnName += includeValue;

                    // Not Primary Key
                    if (
                        (
                            prop[i].GetCustomAttributes(true) == null
                            || prop[i].GetCustomAttributes(true).Length == 0
                            // Diferente de PK
                            || (((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.PrimaryKey && 
                                (
                                    (
                                        // Diferente de SK 
                                        ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType != DBAccessColumnType.SecurityKey && 
                                        // ou não é UPDATE
                                        reflectionOperation == DBAccessReflectionOperations.Update
                                     ) || 
                                    // ou é INSERT
                                    reflectionOperation == DBAccessReflectionOperations.Insert
                                )
                               )
                        )
                       )
                    {
                        if (sqlServer)
                        {
                            if (prop[i].PropertyType.Name != null && prop[i].GetValue(obj, new object[] { }) != null)
                            {

                                typeName = prop[i].PropertyType.Name != "Nullable`1"
                                                ? prop[i].PropertyType.Name
                                                : Nullable.GetUnderlyingType(prop[i].PropertyType).Name;

                                switch (typeName)
                                {
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                    case "UInt16":
                                    case "UInt32":
                                    case "UInt64":
                                        this.AddParameter(columnName, SqlDbType.Int, UInt64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Decimal":
                                        this.AddParameter(columnName, SqlDbType.Decimal, decimal.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "String":
                                    case "Char":
                                        this.AddParameter(columnName, SqlDbType.VarChar, !string.IsNullOrEmpty(prop[i].GetValue(obj, new object[] { }).ToString().Trim()) ? prop[i].GetValue(obj, new object[] { }).ToString().Trim() : null);
                                        break;
                                    case "Byte":
                                        this.AddParameter(columnName, SqlDbType.Bit, Byte.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Boolean":
                                        this.AddParameter(columnName, SqlDbType.Bit, bool.Parse(prop[i].GetValue(obj, new object[] { }).ToString()) ? 1 : 0);
                                        break;
                                    case "Float":
                                    case "Single":
                                    case "Double":
                                        this.AddParameter(columnName, SqlDbType.Decimal, float.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "DateTime":
                                        this.AddParameter(columnName, SqlDbType.DateTime, Convert.ToDateTime(prop[i].GetValue(obj, new object[] { })));
                                        break;
                                    default:
                                        this.AddParameter(columnName, SqlDbType.Int, Convert.ToInt64(prop[i].GetValue(obj, new object[] { })));
                                        break;
                                }
                            }
                            else
                            {
                                // Default
                                this.AddParameter(columnName, SqlDbType.VarChar, null);
                            }
                        }
                        else
                        {
                            if (prop[i].PropertyType.Name != null && prop[i].GetValue(obj, new object[] { }) != null)
                            {

                                typeName = prop[i].PropertyType.Name != "Nullable`1"
                                                ? prop[i].PropertyType.Name
                                                : Nullable.GetUnderlyingType(prop[i].PropertyType).Name;

                                switch (typeName)
                                {
                                    case "Int16":
                                        this.AddParameter(columnName, DbType.Int16, Int16.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Int32":
                                        this.AddParameter(columnName, DbType.Int32, Int32.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Int64":
                                        this.AddParameter(columnName, DbType.Int64, Int64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt16":
                                        this.AddParameter(columnName, DbType.UInt16, UInt16.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt32":
                                        this.AddParameter(columnName, DbType.UInt32, UInt32.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt64":
                                        this.AddParameter(columnName, DbType.UInt64, UInt64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Decimal":
                                        this.AddParameter(columnName, DbType.Decimal, decimal.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "String":
                                    case "Char":
                                        this.AddParameter(columnName, DbType.String, !string.IsNullOrEmpty(prop[i].GetValue(obj, new object[] { }).ToString().Trim()) ? prop[i].GetValue(obj, new object[] { }).ToString().Trim() : null);
                                        break;
                                    case "Byte":
                                        this.AddParameter(columnName, DbType.Byte, Byte.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Boolean":
                                        this.AddParameter(columnName, DbType.Byte, bool.Parse(prop[i].GetValue(obj, new object[] { }).ToString()) ? 1 : 0);
                                        break;
                                    case "Float":
                                    case "Single":
                                    case "Double":
                                        this.AddParameter(columnName, DbType.Decimal, float.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "DateTime":
                                        this.AddParameter(columnName, DbType.DateTime, Convert.ToDateTime(prop[i].GetValue(obj, new object[] { })));
                                        break;
                                }
                            }
                            else
                            {
                                // Default
                                this.AddParameter(columnName, DbType.String, null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Parameters of SqlCommand with Reflection (Primary Key and Foreign Key)
        /// </summary>
        /// <param name="obj">Object Instance</param>
        private void SetPrimaryKeyParametersReflection(object obj)
        {
            bool sqlServer = (Modules.DBModule.GetDataBaseModule == Modules.DataBase.SQLSERVER);

            FieldInfo[] fields = obj.GetType().GetFields();
            string columnName = null;
            string typeName = null;
            for (int i = 0; i < fields.Length; i++)
            {
                // ColumnName
                columnName = (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                    ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                    : fields[i].Name;

                // Not Primary Key
                // OBSOLETO
                if (
                    fields[i] != null
                    && fields[i].GetCustomAttributes(true).Length > 0
                    && (
                        (
                            fields[i].GetCustomAttributes(true) == null
                            || fields[i].GetCustomAttributes(true).Length == 0
                            || ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey
                            || ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey

                        )
                        && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")
                     )
                    && (((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey ||
                        ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey)
                    )
                {

                    if (sqlServer)
                    {
                        if (fields[i].GetValue(obj) != null)
                        {
                            switch (fields[i].GetValue(obj).GetType().Name)
                            {
                                case "Int16":
                                case "Int32":
                                case "Int64":
                                case "UInt16":
                                case "UInt32":
                                case "UInt64":
                                    this.AddParameter(columnName, SqlDbType.Int, UInt64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Decimal":
                                    this.AddParameter(columnName, SqlDbType.Decimal, decimal.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "String":
                                case "Char":
                                    this.AddParameter(columnName, SqlDbType.VarChar, fields[i].GetValue(obj).ToString().Trim());
                                    break;
                                case "Byte":
                                    this.AddParameter(columnName, SqlDbType.Bit, Byte.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Boolean":
                                    this.AddParameter(columnName, SqlDbType.Bit, bool.Parse(fields[i].GetValue(obj).ToString()) ? 1 : 0);
                                    break;
                                case "Float":
                                    this.AddParameter(columnName, SqlDbType.Decimal, float.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "DateTime":
                                    this.AddParameter(columnName, SqlDbType.DateTime, Convert.ToDateTime(fields[i].GetValue(obj)));
                                    break;
                            }
                        }
                        else
                        {
                            // Default
                            this.AddParameter(columnName, SqlDbType.VarChar, null);
                        }
                    }
                    else
                    {
                        if (fields[i].GetValue(obj) != null)
                        {
                            switch (fields[i].GetValue(obj).GetType().Name)
                            {
                                case "Int16":
                                    this.AddParameter(columnName, DbType.Int16, Int16.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Int32":
                                    this.AddParameter(columnName, DbType.Int32, Int32.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Int64":
                                    this.AddParameter(columnName, DbType.Int64, Int64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt16":
                                    this.AddParameter(columnName, DbType.UInt16, UInt16.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt32":
                                    this.AddParameter(columnName, DbType.UInt32, UInt32.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "UInt64":
                                    this.AddParameter(columnName, DbType.UInt64, UInt64.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Decimal":
                                    this.AddParameter(columnName, DbType.Decimal, decimal.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "String":
                                case "Char":
                                    this.AddParameter(columnName, DbType.String, fields[i].GetValue(obj).ToString().Trim());
                                    break;
                                case "Byte":
                                    this.AddParameter(columnName, DbType.Byte, Byte.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "Boolean":
                                    this.AddParameter(columnName, DbType.Byte, bool.Parse(fields[i].GetValue(obj).ToString()) ? 1 : 0);
                                    break;
                                case "Float":
                                    this.AddParameter(columnName, DbType.Decimal, float.Parse(fields[i].GetValue(obj).ToString()));
                                    break;
                                case "DateTime":
                                    this.AddParameter(columnName, DbType.DateTime, Convert.ToDateTime(fields[i].GetValue(obj)));
                                    break;
                            }
                        }
                        else
                        {
                            // Default
                            this.AddParameter(columnName, DbType.String, null);
                        }
                    }
                }
            }

            PropertyInfo[] prop = obj.GetType().GetProperties();
            columnName = null;
            for (int i = 0; i < prop.Length; i++)
            {
                if ((prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length == 0) || (prop[i].GetCustomAttributes(true).Length > 0 && !prop[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                {
                    // ColumnName
                    columnName = (prop[i].GetCustomAttributes(true) != null && prop[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                        ? ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnName
                                        : prop[i].Name;
                    // Not Primary Key
                    if (
                        prop[i] != null
                        && prop[i].GetCustomAttributes(true).Length > 0
                        && (
                            prop[i].GetCustomAttributes(true) == null
                            || prop[i].GetCustomAttributes(true).Length == 0
                            || ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey
                            || ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey
                        )
                        && (((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.PrimaryKey ||
                            ((DBAttribute)(prop[i].GetCustomAttributes(true)[0])).ColumnType == DBAccessColumnType.SecurityKey)
                        )
                    {

                        if (sqlServer)
                        {

                            if (prop[i].PropertyType.Name != null && prop[i].GetValue(obj, new object[] { }) != null)
                            {

                                typeName = prop[i].PropertyType.Name != "Nullable`1"
                                                ? prop[i].PropertyType.Name
                                                : Nullable.GetUnderlyingType(prop[i].PropertyType).Name;

                                switch (typeName)
                                {
                                    case "Int16":
                                    case "Int32":
                                    case "Int64":
                                    case "UInt16":
                                    case "UInt32":
                                    case "UInt64":
                                        this.AddParameter(columnName, SqlDbType.Int, UInt64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Decimal":
                                        this.AddParameter(columnName, SqlDbType.Decimal, decimal.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "String":
                                    case "Char":
                                        this.AddParameter(columnName, SqlDbType.VarChar, prop[i].GetValue(obj, new object[] { }).ToString().Trim());
                                        break;
                                    case "Byte":
                                        this.AddParameter(columnName, SqlDbType.Bit, Byte.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Boolean":
                                        this.AddParameter(columnName, SqlDbType.Bit, bool.Parse(prop[i].GetValue(obj, new object[] { }).ToString()) ? 1 : 0);
                                        break;
                                    case "Float":
                                        this.AddParameter(columnName, SqlDbType.Decimal, float.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "DateTime":
                                        this.AddParameter(columnName, SqlDbType.DateTime, Convert.ToDateTime(prop[i].GetValue(obj, new object[] { })));
                                        break;
                                }
                            }
                            else
                            {
                                // Default
                                this.AddParameter(columnName, SqlDbType.VarChar, null);
                            }
                        }
                        else
                        {
                            if (prop[i].PropertyType.Name != null && prop[i].GetValue(obj, new object[] { }) != null)
                            {

                                typeName = prop[i].PropertyType.Name != "Nullable`1"
                                                ? prop[i].PropertyType.Name
                                                : Nullable.GetUnderlyingType(prop[i].PropertyType).Name;

                                switch (typeName)
                                {
                                    case "Int16":
                                        this.AddParameter(columnName, DbType.Int16, Int16.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Int32":
                                        this.AddParameter(columnName, DbType.Int32, Int32.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Int64":
                                        this.AddParameter(columnName, DbType.Int64, Int64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt16":
                                        this.AddParameter(columnName, DbType.UInt16, UInt16.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt32":
                                        this.AddParameter(columnName, DbType.UInt32, UInt32.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "UInt64":
                                        this.AddParameter(columnName, DbType.UInt64, UInt64.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Decimal":
                                        this.AddParameter(columnName, DbType.Decimal, decimal.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "String":
                                    case "Char":
                                        this.AddParameter(columnName, DbType.String, prop[i].GetValue(obj, new object[] { }).ToString().Trim());
                                        break;
                                    case "Byte":
                                        this.AddParameter(columnName, DbType.Byte, Byte.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "Boolean":
                                        this.AddParameter(columnName, DbType.Byte, bool.Parse(prop[i].GetValue(obj, new object[] { }).ToString()) ? 1 : 0);
                                        break;
                                    case "Float":
                                        this.AddParameter(columnName, DbType.Decimal, float.Parse(prop[i].GetValue(obj, new object[] { }).ToString()));
                                        break;
                                    case "DateTime":
                                        this.AddParameter(columnName, DbType.DateTime, Convert.ToDateTime(prop[i].GetValue(obj, new object[] { })));
                                        break;
                                }
                            }
                            else
                            {
                                // Default
                                this.AddParameter(columnName, DbType.String, null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Properties
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <param name="row">Row to set values</param>
        /// <returns>Object Instance Populated</returns>
        public object SetPropertiesReflection(object obj, DataRow row)
        {
            if (row != null)
            {
                FieldInfo[] fields = obj.GetType().GetFields();
                string columnName = null;
                string underlyingType = null;
                for (int i = 0; i < fields.Length; i++)
                {
                    if ((fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length == 0) || (fields[i].GetCustomAttributes(true).Length > 0 && !fields[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                    {
                        // ColumnName
                        columnName = (fields[i].GetCustomAttributes(true) != null && fields[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                            ? ((DBAttribute)(fields[i].GetCustomAttributes(true)[0])).ColumnName
                                            : fields[i].Name;

                        // Nullable ignore
                        underlyingType = fields[i].FieldType.Name != "Nullable`1"
                                                ? fields[i].FieldType.Name
                                                : Nullable.GetUnderlyingType(fields[i].FieldType).Name;

                        switch (underlyingType)
                        {
                            case "Int16":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int16.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "Int32":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int32.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "Int64":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int64.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "UInt16": obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt16.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "UInt32":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt32.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "UInt64":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt64.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "Decimal":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? decimal.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "String":
                            case "Char":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? row[columnName].ToString() : (object)null);
                                break;
                            case "Byte":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? byte.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "Boolean":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? bool.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "Float":
                            case "Single":
                            case "Double":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? float.Parse(row[columnName].ToString()) : (object)null);
                                break;
                            case "DateTime":
                                obj.GetType().GetField(fields[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? DateTime.Parse(row[columnName].ToString()) : (object)null);
                                break;
                        }
                    }
                }

                PropertyInfo[] props = obj.GetType().GetProperties();
                columnName = null;
                underlyingType = null;
                for (int i = 0; i < props.Length; i++)
                {
                    if ((props[i].GetCustomAttributes(true) != null && props[i].GetCustomAttributes(true).Length == 0) || (props[i].GetCustomAttributes(true).Length > 0 && !props[i].GetCustomAttributes(true)[0].GetType().Name.Equals("DBAttributeIgnore")))
                    {
                        // ColumnName
                        columnName = (props[i].GetCustomAttributes(true) != null && props[i].GetCustomAttributes(true).Length > 0 && ((DBAttribute)(props[i].GetCustomAttributes(true)[0])).ColumnName != null)
                                            ? ((DBAttribute)(props[i].GetCustomAttributes(true)[0])).ColumnName
                                            : props[i].Name;

                        // Nullable ignore
                        underlyingType = props[i].PropertyType.Name != "Nullable`1"
                                                ? props[i].PropertyType.Name
                                                : Nullable.GetUnderlyingType(props[i].PropertyType).Name;

                        switch (underlyingType)
                        {
                            case "Int16":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int16.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "Int32":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int32.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "Int64":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? Int64.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "UInt16":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt16.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "UInt32":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt32.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "UInt64":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? UInt64.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "Decimal":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? decimal.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "String":
                            case "Char":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? row[columnName].ToString() : (object)null, new object[] { });
                                break;
                            case "Byte":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? byte.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "Boolean":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? bool.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "Float":
                            case "Single":
                            case "Double":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? float.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                            case "DateTime":
                                obj.GetType().GetProperty(props[i].Name).SetValue(obj, !Convert.IsDBNull(row[columnName]) ? DateTime.Parse(row[columnName].ToString()) : (object)null, new object[] { });
                                break;
                        }
                    }
                }
            }
            return obj;
        }
        #endregion

        #region [ Select and Populated Object with Reflection ]
        /// <summary>
        /// Method to select object in database (with no lock)
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>Object Instance Populated</returns>
        public object dbSelectReflection(object obj) { return this.dbSelectReflection(obj, null); }

        /// <summary>
        /// Method to select object in database (with no lock to sql server)
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <param name="id">Primary Key Value</param>
        /// <returns>Object Instance Populated</returns>
        public object dbSelectReflection(object obj, object id)
        {
            string sSql = string.Empty;
            string sWhere = string.Empty;
            string sTable = this.GetTableName(obj);
            List<DBAccessStructCondition> aWhere = this.GetStructConditionToReflection(obj);
            DataTable dt = new DataTable();

            // Set Primary Key
            if (id != null)
                this.SetPrimaryKeyReflection(obj, id);

            // Where
            if (aWhere != null)
            {
                foreach (DBAccessStructCondition sStructureKey in aWhere)
                    sWhere += " " + sStructureKey.sLogic + " " + sStructureKey.sField + " " + sStructureKey.sComparison + " " + dbvariable + sStructureKey.sValue;
            }

            if (dataBaseType == Modules.DataBase.SQLSERVER)
                sSql = string.Format("SELECT TOP 1 * FROM {0} (NOLOCK) {1}", sTable, (!string.IsNullOrEmpty(sWhere) ? "WHERE " + sWhere : string.Empty));
            else
                sSql = string.Format("SELECT * FROM {0} {1}", sTable, "WHERE " + (!string.IsNullOrEmpty(sWhere) ? sWhere + " AND ROWNUM = 1" : "ROWNUM = 1"));

            this.SetCommand(sSql, CommandType.Text);
            this.SetPrimaryKeyParametersReflection(obj);
            try
            {
                dt = this.ExecuteAndGetDataTable();
                return this.SetPropertiesReflection(obj, dt.Rows[0]);
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            finally
            {
                dt.Dispose();
            }
        }

        /// <summary>
        /// Method to select object in database (with no lock to sql server)
        /// </summary>
        /// <param name="obj">Object Instance Reference</param>
        public void dbSelectReflection(ref object obj)
        {
            string sSql = string.Empty;
            string sWhere = string.Empty;
            string sTable = this.GetTableName(obj);
            List<DBAccessStructCondition> aWhere = this.GetStructConditionToReflection(obj);
            DataTable dt = new DataTable();
            // Where
            if (aWhere != null)
            {
                foreach (DBAccessStructCondition sStructureKey in aWhere)
                    sWhere += " " + sStructureKey.sLogic + " " + sStructureKey.sField + " " + sStructureKey.sComparison + " " + dbvariable + sStructureKey.sValue;
            }

            if (dataBaseType == Modules.DataBase.SQLSERVER)
                sSql = string.Format("SELECT TOP 1 * FROM {0} (NOLOCK) {1}", sTable, (!string.IsNullOrEmpty(sWhere) ? "WHERE " + sWhere : string.Empty));
            else
                sSql = string.Format("SELECT * FROM {0} {1}", sTable, "WHERE " + (!string.IsNullOrEmpty(sWhere) ? sWhere + " AND ROWNUM = 1" : "ROWNUM = 1"));

            this.SetCommand(sSql, CommandType.Text);
            this.SetParametersReflection(obj, DBAccessReflectionOperations.Select);
            try
            {
                dt = this.ExecuteAndGetDataTable();
                this.SetPropertiesReflection(obj, dt.Rows[0]);
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
            finally
            {
                dt.Dispose();
            }
        }
        #endregion

        #region [ Insert Reflection ]
        /// <summary>
        /// Method to insert in DataBase
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>Identity in database</returns>
        public int dbInsertReflection(object obj)
        {
            string sSql = string.Empty;
            string sTable = this.GetTableName(obj);
            string sFields = string.Empty;
            string sValues = string.Empty;
            int iCountField = 0;

            List<DBAccessStructParameter> aData = this.GetStructFieldToReflection(obj);
            foreach (DBAccessStructParameter sStructureKey in aData)
            {
                if (sStructureKey != null)
                {
                    if (sStructureKey.sField != "" && sStructureKey.sField != null)
                    {
                        if (iCountField > 0)
                        {
                            sFields += ", " + sStructureKey.sField;
                            sValues += ", " + dbvariable + sStructureKey.sValue;
                        }
                        else
                        {
                            sFields += sStructureKey.sField;
                            sValues += dbvariable + sStructureKey.sValue;
                        }
                        iCountField++;
                    }
                }
            }

            sSql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", sTable, sFields, sValues);
            this.SetCommand(sSql, CommandType.Text);
            this.SetParametersReflection(obj, DBAccessReflectionOperations.Insert);
            try
            {
                int identity = 0;
                identity = this.ExecuteAndGetIdentity(GetPrimaryKeyName(obj));
                this.SetPrimaryKeyReflection(obj, identity);
                return identity;
            }
            catch (Exception e) { throw new DBAccessException(e); }
        }

        /// <summary>
        /// Method to insert batch in DataBase
        /// </summary>
        /// <param name="list">List Instance</param>
        /// <returns>Boolean</returns>
        public void dbInsertBatchReflection<T>(List<T> list)
        {
            if (dataBaseType == Modules.DataBase.SQLSERVER)
            {
                throw new DBAccessException(msgErrorMethodInvalid_SQLSERVER);
            }
            else
            {
                if (list.Count > 0)
                {
                    string sSql = string.Empty;
                    string sTable = this.GetTableName(list[0]);
                    string sBatchs = string.Empty;
                    string sFields;
                    string sValues;
                    int iCountField;

                    for (int i = 0; i < list.Count; i++)
                    {
                        sFields = string.Empty;
                        sValues = string.Empty;
                        iCountField = 0;

                        List<DBAccessStructParameter> aData = this.GetStructFieldToReflection(list[i]);
                        foreach (DBAccessStructParameter sStructureKey in aData)
                        {
                            if (sStructureKey != null)
                            {
                                if (sStructureKey.sField != "" && sStructureKey.sField != null)
                                {
                                    if (iCountField > 0)
                                    {
                                        sFields += ", " + sStructureKey.sField;
                                        sValues += ", " + dbvariable + sStructureKey.sValue + i.ToString();
                                    }
                                    else
                                    {
                                        sFields += sStructureKey.sField;
                                        sValues += dbvariable + sStructureKey.sValue + i.ToString();
                                    }
                                    iCountField++;
                                }
                            }
                        }

                        sBatchs += string.Format(@"
INTO {0} ({1}) VALUES ({2})
", sTable, sFields, sValues);
                    }
                    sSql = string.Format(@"
INSERT ALL
    {0}
SELECT * FROM DUAL
", sBatchs);
                    this.SetCommand(sSql, CommandType.Text);
                    for (int i = 0; i < list.Count; i++)
                        this.SetParametersReflection(list[i], DBAccessReflectionOperations.Insert, i.ToString());
                    try
                    {
                        this.ExecuteNonQuery();
                    }
                    catch (Exception e) { throw new DBAccessException(e); }
                }
                else
                {
                    throw new DBAccessException("List passed by parameter is empty.");
                }
            }
        }
        #endregion

        #region [ Update Reflection ]
        /// <summary>
        /// Method to update in database
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>True and false to operation</returns>
        public bool dbUpdateReflection(object obj)
        {
            string sSql = string.Empty;
            string sValues = string.Empty;
            string sWhere = string.Empty;
            string sTable = this.GetTableName(obj);
            List<DBAccessStructParameter> aData = this.GetStructFieldToReflection(obj);
            List<DBAccessStructCondition> aWhere = this.GetStructConditionToReflection(obj);

            // Set
            if (sValues != null)
            {
                foreach (DBAccessStructParameter sStructureKey in aData)
                {
                    if (sStructureKey != null)
                        sValues += sStructureKey.sField + " = " + dbvariable + sStructureKey.sValue + ", ";
                }
                //-->
                sValues += ";";
                sValues = sValues.Replace(", ;", " ");
            }

            // Where
            if (aWhere != null)
            {
                foreach (DBAccessStructCondition sStructureKey in aWhere)
                    sWhere += " " + sStructureKey.sLogic + " " + sStructureKey.sField + " " + sStructureKey.sComparison + " " + dbvariable + sStructureKey.sValue;
            }

            sSql = string.Format("UPDATE {0} SET {1} WHERE {2}", sTable, sValues, sWhere);
            this.SetCommand(sSql, CommandType.Text);
            this.SetParametersReflection(obj, DBAccessReflectionOperations.Update);
            this.SetPrimaryKeyParametersReflection(obj);
            try
            {
                return this.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
        }
        #endregion

        #region [ Delete Reflection ]
        /// <summary>
        /// Method to delete in database
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>True and false to operation</returns>
        public bool dbDeleteReflection(object obj)
        {
            string sSql = string.Empty;
            string sWhere = string.Empty;
            string sTable = this.GetTableName(obj);
            List<DBAccessStructCondition> aWhere = this.GetStructConditionToReflection(obj);

            // Where
            if (aWhere != null)
            {
                foreach (DBAccessStructCondition sStructureKey in aWhere)
                    sWhere += " " + sStructureKey.sLogic + " " + sStructureKey.sField + " " + sStructureKey.sComparison + " " + dbvariable + sStructureKey.sValue;
            }

            sSql = string.Format("DELETE FROM {0} WHERE {1}", sTable, sWhere);
            this.SetCommand(sSql, CommandType.Text);
            this.SetPrimaryKeyParametersReflection(obj);
            try
            {
                return this.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }
        }
        #endregion

        #region [ Return Structures with Reflection ]
        /// <summary>
        /// Method to select object in database
        /// </summary>
        /// <param name="obj">Object Instance</param>
        /// <returns>Object Instance Populated</returns>
        public T dbFillReflection<T>(object obj)
        {
            string sSql = string.Empty;
            string sWhere = string.Empty;
            string sTable = this.GetTableName(obj);
            object o = Activator.CreateInstance<T>();
            List<DBAccessStructCondition> aWhere = this.GetStructConditionToReflection(obj);

            // Where
            if (aWhere != null)
            {
                foreach (DBAccessStructCondition sStructureKey in aWhere)
                    sWhere += " " + sStructureKey.sLogic + " " + sStructureKey.sField + " " + sStructureKey.sComparison + " " + dbvariable + sStructureKey.sValue;
            }

            if (dataBaseType == Modules.DataBase.SQLSERVER)
                sSql = string.Format("SELECT * FROM {0} (NOLOCK) WHERE {1}", sTable, sWhere);
            else
                sSql = string.Format("SELECT * FROM {0} WHERE {1}", sTable, sWhere);

            this.SetCommand(sSql, CommandType.Text);
            try
            {
                switch (typeof(T).Name)
                {
                    case "DataTable":
                        o = this.ExecuteAndGetDataTable();
                        break;
                    case "DataSet":
                        o = this.ExecuteAndGetDataSet();
                        break;
                    case "DataRow":
                        o = this.ExecuteAndGetDataRow();
                        break;
                    case "List`1":
                        o = this.ExecuteAndGetGenericList<T>();
                        break;
                    default:
                        o = this.ExecuteAndGetDataTable();
                        break;
                }
            }
            catch (Exception e)
            {
                throw new DBAccessException(e);
            }

            return (T)o;
        }
        #endregion

        #endregion

        // -----------------------------------------------
        #endregion

        #region [ IDisposable Members ]
        /// <summary>
        /// Disposable Method
        /// </summary>
        public void Dispose() { this.Close(); }
        #endregion

    }
    #endregion

}