using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NCM", EntityName = "NCM", Name = "AdminMultisoftware.Dominio.Entidades.Produtos.NCM", NameType = typeof(NCM))]
    public class NCM : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Produtos.NCM>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NCM_NUMERO", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "NCM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        public virtual bool Equals(NCM other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
