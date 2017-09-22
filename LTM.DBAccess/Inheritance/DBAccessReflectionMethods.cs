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
using LTM.DBAccess.Exceptions;
using LTM.DBAccess.Interfaces;

namespace LTM.DBAccess.Inheritance {

    #region [ DBReflection Methods Class ]
    /// <summary>
    /// DBAccessReflectionMethods class
    /// </summary>
    [Obsolete]
    public class DBAccessReflectionMethods : IDBMethods {

        #region [ Interface Methods ]
        /// <summary>
        /// Load object with primary key
        /// </summary>
        /// <param name="id">primary key value</param>
        public void Load(object id) {
            LTM.DBAccess.Persistence.DBAccess dbExtension = new LTM.DBAccess.Persistence.DBAccess();
            try {
                dbExtension.dbSelectReflection(this, id);
            }
            catch (Exception e) {
                throw new DBAccessException(e);
            }
            finally {
                dbExtension.Close();
            }
        }
        /// <summary>
        /// Get object
        /// </summary>
        /// <typeparam name="T">generic object</typeparam>
        /// <returns>object</returns>
        public T GetDataObject<T>() {
            LTM.DBAccess.Persistence.DBAccess dbExtension = new LTM.DBAccess.Persistence.DBAccess();
            try {
                return dbExtension.dbFillReflection<T>(this);
            }
            catch (Exception e) {
                throw new DBAccessException(e);
            }
            finally {
                dbExtension.Close();
            }
        }
        /// <summary>
        /// Save object in database
        /// </summary>
        /// <returns>primary key value</returns>
        public int Save() {
            LTM.DBAccess.Persistence.DBAccess dbExtension = new LTM.DBAccess.Persistence.DBAccess();
            try {
                return dbExtension.dbInsertReflection(this);
            }
            catch (Exception e) {
                throw new DBAccessException(e);
            }
            finally {
                dbExtension.Close();
            }
        }
        /// <summary>
        /// Update object in database
        /// </summary>
        /// <returns>Success</returns>
        public bool Update() {
            LTM.DBAccess.Persistence.DBAccess dbExtension = new LTM.DBAccess.Persistence.DBAccess();
            try {
                return dbExtension.dbUpdateReflection(this);
            }
            catch (Exception e) {
                throw new DBAccessException(e);
            }
            finally {
                dbExtension.Close();
            }
        }
        /// <summary>
        /// Delete object in database
        /// </summary>
        /// <returns>Success</returns>
        public bool Delete() {
            LTM.DBAccess.Persistence.DBAccess dbExtension = new LTM.DBAccess.Persistence.DBAccess();
            try {
                return dbExtension.dbDeleteReflection(this);
            }
            catch (Exception e) {
                throw new DBAccessException(e);
            }
            finally {
                dbExtension.Close();
            }

        }
        #endregion

    }
    #endregion

}