using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_OPERACAO", EntityName = "RegraTipoOperacao", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacao", NameType = typeof(RegraTipoOperacao))]
    public class RegraTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDocumentoTransporte", Column = "TDT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.TipoDocumentoTransporte TipoDocumentoTransporte { get; set; }

        [Obsolete("Migrado para uma lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEtapas", Column = "RTO_QUANTIDADE_ETAPAS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SimNao QuantidadeEtapas { get; set; }

        [Obsolete("Migrado para uma lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaPessoa", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.CategoriaPessoa Categoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "RTO_TIPO_MODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CteGlobalizado", Column = "RTO_CTE_GLOBALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CteGlobalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RTO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao { get { return this.TipoDocumentoTransporte?.Descricao ?? string.Empty; } }
    }
}