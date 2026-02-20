using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Modulos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODULO", EntityName = "ClienteModulo", Name = "AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo", NameType = typeof(ClienteModulo))]
    public class ClienteModulo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Modulo", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Modulos.Modulo Modulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModuloExclusivo", Column = "CMO_MODULO_EXCLUSIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ModuloExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModuloBloqueado", Column = "CMO_MODULO_BLOQUEADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ModuloBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CMO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
