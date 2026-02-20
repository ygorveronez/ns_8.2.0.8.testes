using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_VALE_PEDAGIO", EntityName = "ConfiguracaoValePedagio", Name = "Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio", NameType = typeof(ConfiguracaoValePedagio))]
    public class ConfiguracaoValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracao), NotNull = false)]
        public virtual TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        #region Integradoras

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoSemParar", Column = "CIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoSemParar IntegracaoSemParar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoTarget", Column = "CIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoTarget IntegracaoTarget { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoRepom", Column = "CIR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoRepom IntegracaoRepom { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoPagbem", Column = "CIP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoPagbem IntegracaoPagbem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoDBTrans", Column = "CID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoDBTrans IntegracaoDBTrans { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoQualP", Column = "CIQ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoQualP IntegracaoQualP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoPamcard", Column = "CIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoPamcard IntegracaoPamcard { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoEFrete", Column = "CIE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoEFrete IntegracaoEFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoExtrattaValePedagio", Column = "CIX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoExtrattaValePedagio IntegracaoExtratta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoDigitalComValePedagio", Column = "CDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoDigitalComValePedagio IntegracaoDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoAmbiparValePedagio", Column = "CAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoAmbiparValePedagio IntegracaoAmbipar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoNDDCargo", Column = "CIN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoNDDCargo IntegracaoNDDCargo { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsultarValorPedagioAntesAutorizarEmissao", Column = "CVP_CONSULTAR_VALOR_PEDAGIO_ANTES_AUTORIZAR_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarValorPedagioAntesAutorizarEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirGerarValePedagioVeiculoProprio", Column = "CVP_GERAR_VALE_PEDAGIO_VEICULO_PROPRIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarValePedagioVeiculoProprio { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao => Codigo.ToString();

        public virtual string DescricaoSituacao => Situacao ? "Ativo" : "Inativo";

        public virtual string DescricaoTipoIntegracao => TipoIntegracao.ObterDescricao();

        public virtual bool Equals(ConfiguracaoValePedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}