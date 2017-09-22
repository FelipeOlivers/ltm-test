using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTM.Test.BLL
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
                return new DAL.Usuarios().Validar(user, Util.Crypto.Encrypt(pass));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

    }
}
