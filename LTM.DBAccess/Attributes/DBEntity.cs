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

namespace LTM.DBAccess.Attributes {

    #region [ Attributes ]
    /// <summary>
    /// DBEntity Attribute - DBAccess - Framework DAO.NET with SqlSever
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DBEntity : System.Attribute {

        #region [ Attributes ]
        /// <summary>
        /// Attributes to DBEntity
        /// </summary>
        private string _tableName;
        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Table name</param>
        public DBEntity(string tableName) {
            this._tableName = tableName;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Propertie to Table Name
        /// </summary>
        public string TableName {
            get { return this._tableName; }
            set { this._tableName = value; }
        }
        #endregion

    }
    #endregion

}