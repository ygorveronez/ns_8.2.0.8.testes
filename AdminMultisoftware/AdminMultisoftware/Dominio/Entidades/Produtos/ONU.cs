using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ONU", EntityName = "ONU", Name = "AdminMultisoftware.Dominio.Entidades.Produtos.ONU", NameType = typeof(ONU))]
    public class ONU : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Produtos.ONU>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ONU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ONU_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ONU_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sinonimos", Column = "ONU_SINONIMOS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Sinonimos { get; set; }

        public virtual bool Equals(ONU other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
