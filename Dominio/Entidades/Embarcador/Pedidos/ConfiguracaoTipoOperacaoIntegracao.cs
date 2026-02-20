using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_INTEGRACAO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracao", NameType = typeof(ConfiguracaoTipoOperacaoIntegracao))]
    public class ConfiguracaoTipoOperacaoIntegracao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_UTILIZAR_TIPO_INTEGRACAO_GRUPO_PESSOAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTipoIntegracaoGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_ATIVAR_REGRA_CANCELAMENTO_DOS_PEDIDOS_MICHELIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarRegraCancelamentoDosPedidosMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_HORAS_PARA_CALCULO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasParaCalculoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_HABILITAR_INTEGRACAO_PARA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarIntegracaoAvancoParaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_CALCULAR_GERAR_GNRE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularGerarGNRE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_MANTER_INTEGRACOES_REFERENCIANDO_CARGA_DE_ORIGEM_DO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_INTEGRAR_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_INTEGRAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_INTEGRAR_CARGAS_GERADAS_MULTI_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCargasGeradasMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_ENVIAR_APENAS_PRIMEIRO_PEDIDO_NA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarApenasPrimeiroPedidoNaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_ENVIAR_INFORMACOES_TOTAIS_DA_CARGA_NA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarInformacoesTotaisDaCargaNaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_GERAR_INTEGRACAO_KLIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoKlios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_ENVIAR_TAGS_INTEGRACAO_MARFRIG_COM_TOMADOR_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarTagsIntegracaoMarfrigComTomadorServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_VALIDAR_SOMENTE_VEICULO_MOTORISTA_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSomenteVeiculoMotoristaOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_DEFINIR_PARA_NAO_MONITORAR_RETORNO_INTEGRACAO_BOUNNY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirParaNaoMonitorarRetornoIntegracaoBounny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_POSSUI_TEMPO_ENVIO_INTEGRACAO_DOCUMENTOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTempoEnvioIntegracaoDocumentosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_CONSULTAR_TAXAS_KMM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarTaxasKMM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_NAO_GERAR_INTEGRACAO_RETORNO_CONFIRMACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarIntegracaoRetornoConfirmacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTI_NAO_INTEGRAR_ETAPA1_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIntegrarEtapa1Opentech { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIG_TIPO_OPERACAO_INTEGRACAO_TIPO_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerceiro", Column = "TPT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> TiposTerceiros { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações de Integração";
            }
        }
    }
}
