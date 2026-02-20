using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_USUARIO", EntityName = "Usuario", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario", NameType = typeof(AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario))]
    public class Usuario : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "USU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "USU_NOME", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "USU_USUARIO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "USU_SENHA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Senha { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual string Descricao
        {
            get
            {
                return !string.IsNullOrEmpty(Nome) ? Nome : string.Empty;
            }
        }

        #endregion Métodos Públicos
    }
}
