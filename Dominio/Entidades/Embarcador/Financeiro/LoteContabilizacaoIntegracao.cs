using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_CONTABILIZACAO_INTEGRACAO", EntityName = "LoteContabilizacaoIntegracao", Name = "Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracao", NameType = typeof(LoteContabilizacaoIntegracao))]
    public class LoteContabilizacaoIntegracao : Integracao.Integracao, IEquatable<LoteContabilizacaoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteContabilizacao", Column = "LCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteContabilizacao LoteContabilizacao { get; set; }

        public virtual bool Equals(LoteContabilizacaoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
