using System;
using System.Collections.Generic;

namespace LTM.Test.DAL
{
    public class Telefones
    {

        #region [ Constructor ]

        public Telefones()
        {

        }

        #endregion

        #region [ Methods ]

        public List<DTO.Telefones> Listar()
        {
            try
            {
                LTM.DBAccess.Persistence.DBAccess db = new LTM.DBAccess.Persistence.DBAccess();
                db.SetCommand(@"SELECT * FROM Telefones");

                return db.ExecuteAndGetGenericList<DTO.Telefones>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
