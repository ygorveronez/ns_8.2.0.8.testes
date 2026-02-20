using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_USUARIO_MOBILE_CLIENTE", EntityName = "UsuarioMobileCliente", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente", NameType = typeof(UsuarioMobileCliente))]
    public class UsuarioMobileCliente : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UBC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UsuarioMobile", Column = "UMB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Mobile.UsuarioMobile UsuarioMobile { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseHomologacao", Column = "CLI_BASE_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BaseHomologacao { get; set; }

        public virtual bool Equals(UsuarioMobileCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
