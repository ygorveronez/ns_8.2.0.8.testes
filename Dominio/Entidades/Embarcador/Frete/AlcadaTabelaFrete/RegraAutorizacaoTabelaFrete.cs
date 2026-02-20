using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_TABELA_FRETE", EntityName = "RegraAutorizacaoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete", NameType = typeof(RegraAutorizacaoTabelaFrete))]
    public class RegraAutorizacaoTabelaFrete : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_AD_VALOREM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_VALOR_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAT_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasAdValorem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_AD_VALOREM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaAdValorem", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaAdValorem> AlcadasAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaDestino", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaDestino> AlcadasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaOrigem", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaOrigem> AlcadasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorFrete> AlcadasValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorPedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TABELA_FRETE_VALOR_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasTabelaFrete.AlcadaValorPedagio", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorPedagio> AlcadasValorPedagio { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_TABELA_FRETE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorAdValorem || RegraPorDestino || RegraPorFilial || RegraPorOrigem || RegraPorTipoOperacao || RegraPorTransportador || RegraPorValorFrete || RegraPorValorPedagio;
        }

        public override void LimparAlcadas()
        {
            AlcadasAdValorem?.Clear();
            AlcadasDestino?.Clear();
            AlcadasFilial?.Clear();
            AlcadasOrigem?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasTransportador?.Clear();
            AlcadasValorFrete?.Clear();
            AlcadasValorPedagio?.Clear();
        }

        #endregion
    }
}
