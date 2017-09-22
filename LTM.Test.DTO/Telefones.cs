using System;
using LTM.DBAccess.Attributes;
using LTM.DBAccess.Inheritance;

namespace LTM.Test.DTO
{
    
    public class Telefones : DBAccessReflectionMethods
    {

        #region [ Properties ]

        [DBAttribute(DBAccessColumnType.PrimaryKey)]
        public int Id_Telefone { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }

        #endregion
        
        #region [ Constructor ]

        public Telefones() { }

        #endregion

    }

}