using System;
using LTM.DBAccess.Attributes;
using LTM.DBAccess.Inheritance;

namespace LTM.Test.DTO
{
    
    public class Usuarios : DBAccessReflectionMethods
    {

        #region [ Properties ]

        [DBAttribute(DBAccessColumnType.PrimaryKey)]
        public int Id_Usuario { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }

        #endregion
        
        #region [ Constructor ]

        public Usuarios() { }

        #endregion

    }

}