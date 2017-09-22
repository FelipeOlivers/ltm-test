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

namespace LTM.DBAccess.Modules
{

    #region [ Enumerator ]
    /// <summary>
    /// Enumerator to Database module
    /// </summary>
    public enum DataBase
    {
        /// <summary>
        /// None
        /// </summary>
        NONE,
        /// <summary>
        /// SqlServer database
        /// </summary>
        SQLSERVER,
        /// <summary>
        /// Oracle database
        /// </summary>
        ORACLE
    }
    #endregion

    #region [ Configuration Class ]
    /// <summary>
    /// DBModule class
    /// </summary>
    public static class DBModule {

        #region [ Constant ]
        private const string _msgErrorConnectionString = "No ConnectionString found in your web.config.";
        private const string _msgErrorAttributeProviderName = "The ProviderName informed the ConnectionString is not valid.";
        #endregion

        #region [ Variables ]
        private static string _connectionStringName = string.Empty;
        #endregion

        #region [ Properties ]
        /// <summary>
        /// ConnectionStringName method
        /// </summary>
        public static string ConnectionStringName {
            set {
                _connectionStringName = value;
            }
        }

        /// <summary>
        /// GetDataBaseModule method
        /// </summary>
        public static DataBase GetDataBaseModule {
            get {
                DataBase db = new DataBase();
                db = DataBase.NONE;
                if (!string.IsNullOrEmpty(_connectionStringName) && System.Configuration.ConfigurationManager.ConnectionStrings[_connectionStringName] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.ConnectionStrings[_connectionStringName].ProviderName))
                    db = ((System.Configuration.ConfigurationManager.ConnectionStrings[_connectionStringName].ProviderName == "System.Data.SqlClient")
                        ? DataBase.SQLSERVER
                        : DataBase.ORACLE);
                else if (System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"].ProviderName))
                    db = ((System.Configuration.ConfigurationManager.ConnectionStrings["LTM.DBAccess.ConnectionString"].ProviderName == "System.Data.SqlClient")
                        ? DataBase.SQLSERVER
                        : DataBase.ORACLE);
                else
                    throw new DBAccess.Exceptions.DBAccessException(new Exception(_msgErrorConnectionString));

                if(db != DataBase.NONE)
                    return db;
                else
                    throw new DBAccess.Exceptions.DBAccessException(new Exception(_msgErrorAttributeProviderName));
            }
        }
        #endregion

    }
    #endregion

}