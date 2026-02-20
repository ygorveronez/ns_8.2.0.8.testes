using System;

namespace Dominio.Entidades.Embarcador.Operacional
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_CLIENTE", EntityName = "OperadorCliente", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorCliente", NameType = typeof(OperadorCliente))]
    public class OperadorCliente : EntidadeBase, IEquatable<OperadorCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorLogistica", Column = "OPL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OperadorLogistica OperadorLogistica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        public virtual bool Equals(OperadorCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
