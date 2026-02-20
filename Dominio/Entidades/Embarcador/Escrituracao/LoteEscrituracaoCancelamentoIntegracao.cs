using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_CANCELAMENTO_INTEGRACAO", EntityName = "LoteEscrituracaoCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoIntegracao", NameType = typeof(LoteEscrituracaoCancelamentoIntegracao))]
    public class LoteEscrituracaoCancelamentoIntegracao : Integracao.Integracao, IEquatable<LoteEscrituracaoCancelamentoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteEscrituracaoCancelamento", Column = "LEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteEscrituracaoCancelamento LoteEscrituracaoCancelamento { get; set; }

        public virtual bool Equals(LoteEscrituracaoCancelamentoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
