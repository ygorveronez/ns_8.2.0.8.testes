using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA", EntityName = "TabelaComissaoMotorista", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista", NameType = typeof(TabelaComissaoMotorista))]
    public class TabelaComissaoMotorista : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TCM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissaoPadrao", Column = "TCM_PERCENTUAL_COMISSAO_PADRAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCM_DATA_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TCM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarBonificacaoMediaCombustivel", Column = "TCM_ATIVAR_BONIFICACAO_MEDIA_COMBUSTIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarBonificacaoMediaCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarBonificacaoRepresentacaoCombustivel", Column = "TCM_ATIVAR_BONIFICACAO_REPRESENTACAO_COMBUSTIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarBonificacaoRepresentacaoCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarBonificacaoFaturamentoDia", Column = "TCM_ATIVAR_BONIFICACAO_FATURAMENTO_DIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarBonificacaoFaturamentoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarBonificacaoRotaFrete", Column = "TCM_ATIVAR_BONIFICACAO_ROTA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarBonificacaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Segmentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_SEGMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaSegmento", Column = "TCS_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaSegmento> Segmentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Modelos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_MODELO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaModeloVeiculo", Column = "TCV_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaModeloVeiculo> Modelos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaTipoOperacao", Column = "TTO_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaTipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Medias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_MEDIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaMedia", Column = "TMM_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaMedia> Medias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Representacaos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_REPRESENTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaRepresentacao", Column = "TCR_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaRepresentacao> Representacaos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaturamentoDia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_FATURAMENTO_DIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoFaturamentoDia", Column = "TFD_CODIGO")]
        public virtual IList<TabelaComissaoFaturamentoDia> FaturamentoDia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RotasFretes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_COMISSAO_MOTORISTA_ROTA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaRotaFrete", Column = "TRF_CODIGO")]
        public virtual IList<TabelaComissaoMotoristaRotaFrete> RotasFretes { get; set; }


        public virtual bool Equals(TabelaComissaoMotorista other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
