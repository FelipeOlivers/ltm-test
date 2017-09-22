using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTM.Test.BLL
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
                return new DAL.Telefones().Listar();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

    }
}
