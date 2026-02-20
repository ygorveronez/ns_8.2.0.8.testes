using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CEST", EntityName = "CEST", Name = "AdminMultisoftware.Dominio.Entidades.Produtos.CEST", NameType = typeof(CEST))]
    public class CEST : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Produtos.CEST>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CES_NUMERO", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CES_DESCRICAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNCM", Column = "CES_CODIGO_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string CodigoNCM { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NCM", Column = "NCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produtos.NCM NCM { get; set; }

        public virtual bool Equals(CEST other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
