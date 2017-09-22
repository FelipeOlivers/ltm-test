using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTM.Test.DAL
{
    public class Usuarios
    {

        #region [ Constructor ]

        public Usuarios()
        {

        }

        #endregion

        #region [ Methods ]

        public bool Validar(string user, string pass)
        {
            try
            {
                LTM.DBAccess.Persistence.DBAccess db = new LTM.DBAccess.Persistence.DBAccess();
                db.SetCommand(@"SELECT * FROM Usuarios WHERE Usuario = @Usuario and Senha = @Senha");
                db.AddParameter("@Usuario", System.Data.SqlDbType.VarChar, user);
                db.AddParameter("@Senha", System.Data.SqlDbType.VarChar, pass);

                var ret = db.ExecuteScalar();

                return ret != null ? (int)ret >= 1 : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
