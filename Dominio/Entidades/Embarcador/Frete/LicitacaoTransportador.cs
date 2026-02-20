using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICITACAO_TRANSPORTADOR", EntityName = "LicitacaoTransportador", Name = "Dominio.Entidades.Embarcador.Frete.LicitacaoTransportador", NameType = typeof(LicitacaoTransportador))]
    public class LicitacaoTransportador : EntidadeBase, IEquatable<LicitacaoTransportador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licitacao", Column = "LIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Licitacao Licitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        public virtual bool Equals(LicitacaoTransportador other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
