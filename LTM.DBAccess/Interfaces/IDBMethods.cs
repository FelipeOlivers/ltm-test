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

namespace LTM.DBAccess.Interfaces {

    #region [ Interface DBAccess Methods ]
    /// <summary>
    /// IDBMethods interface
    /// </summary>
    public interface IDBMethods {
        
        #region [ Methods ]
        /// <summary>
        /// Load object with Primary Key
        /// </summary>
        /// <param name="id">Primary Key</param>
        void Load(object id);
        /// <summary>
        /// GetDataObject
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <returns></returns>
        T GetDataObject<T>();
        /// <summary>
        /// Save object
        /// </summary>
        /// <returns>Primary Key</returns>
        int Save();
        /// <summary>
        /// Update object
        /// </summary>
        /// <returns>Success</returns>
        bool Update();
        /// <summary>
        /// Delete object in database
        /// </summary>
        /// <returns>Success</returns>
        bool Delete();
        #endregion

    }
    #endregion

}