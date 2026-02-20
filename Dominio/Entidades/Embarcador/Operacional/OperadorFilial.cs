using System;

namespace Dominio.Entidades.Embarcador.Operacional
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_FILIAL", EntityName = "OperadorFilial", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorFilial", NameType = typeof(OperadorFilial))]
    public class OperadorFilial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.OperadorFilial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorLogistica", Column = "OPL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Operacional.OperadorLogistica OperadorLogistica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public virtual bool Equals(OperadorFilial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
