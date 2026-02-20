using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMOS_PAGAMENTO", EntityName = "TermosPagamento", Name = "Dominio.Entidades.Embarcador.Frete.TermosPagamento", NameType = typeof(TermosPagamento))]
    public class TermosPagamento : EntidadeBase, IEquatable<TermosPagamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TPG_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPG_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        public virtual bool Equals(TermosPagamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
