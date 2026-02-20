using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Modulos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_FORMULARIO", EntityName = "ClienteFormulario", Name = "AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario", NameType = typeof(ClienteFormulario))]
    public class ClienteFormulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Formulario", Column = "FOR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Modulos.Formulario Formulario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pessoas.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormularioExclusivo", Column = "CFO_FORMULARIO_EXCLUSIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FormularioExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormularioBloqueado", Column = "CFO_FORMULARIO_BLOQUEADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FormularioBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CFO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
        
    }
}
