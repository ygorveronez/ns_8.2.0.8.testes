using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_WEB_SERVICE", EntityName = "ConfiguracaoWebService", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService", NameType = typeof(ConfiguracaoWebService))]
    public class ConfiguracaoWebService : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CWS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ATUALIZAR_DADOS_VEICULO_INTEGRACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarDadosVeiculoIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ADICIONAR_VEICULO_TIPO_REBOQUE_COMO_REBOQUE_AO_ADICIONAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_QUANTIDADE_HORAS_PREENCHER_DATA_CARREGAMENTO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeHorasPreencherDataCarregamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_IGNORAR_CAMPOS_ESSENCIAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarCamposEssenciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_RETORNAR_IMAGEM_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarImagemCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_ENTREGAS_REJEITADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarEntregasRejeitadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_SELECIONAR_ROTA_FRETE_AO_ADICIONAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SelecionarRotaFreteAoAdicionarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_ROTEIRIZAR_ROTA_NOVAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRoteirizarRotaNovamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_SEMPRE_UTILIZAR_TOMADOR_ENVIADO_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreUtilizarTomadorEnviadoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_UTILIZAR_CODIGO_CADASTRO_CLIENTE_COMO_ENDERECO_SECUNDARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCodigosDeCadastroComoEnredecoSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_TORNAR_CAMPO_INSS_OBRIGATORIO_E_RETER_IMPOSTO_TRAZER_COMO_SIM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TornarCampoINSSeReterImpostoTrazerComoSim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ATIVAR_VALIDACAO_PRODUTOS_NO_ADICIONAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarValidacaoDosProdutosNoAdicionarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_PERMITE_USAR_DESCRICAO_FAIXA_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirUsarDescricaoFaixaTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_OUTROS_MODELOS_DOCUMENTOS_WS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarOutrosModelosDeDocumentosNoWSCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_APENAS_CARREGAMENTOS_PENDENTES_COM_TRANSPORTADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarApenasCarregamentosPendentesComTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_SOBREPOR_INFORMACOES_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSobrePorInformacoesViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_BLOQUEAR_INCLUSAO_CARGA_COM_MESMO_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_CARREGAMENTOS_SOMENTE_COM_CARGAS_EM_AGUARDANDO_NF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCarregamentosSomenteCargasEmAgNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_PERMITIR_SOLICITAR_CANCELAMENTO_CARGA_VIA_INTEGRACAO_VIAGEM_INICIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_RETORNAR_NFSE_VINCULADA_NFS_MANUAL_METODO_BUSCARNFSS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_PERMITE_RECEBER_DATA_CRIACAO_PEDIDO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteReceberDataCriacaoPedidoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_FILTRAR_POR_CODIGO_INTEGRACAO_NA_PESQUISA_NOME_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_CADASTRO_AUTOMATICO_PESSOA_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastroAutomaticoPessoaExterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RECALCULAR_FRETE_AO_ADICIONAR_REMOVER_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRecalcularFreteAoAdicionarRemoverPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ATUALIZAR_TODOS_CADASTROS_MOTORISTAS_MESMO_CODIGO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_SALVAR_REGIAO_NO_CLIENTE_PARA_PREENCHER_REGIAO_DESTINO_DOS_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_PERMITIR_GERAR_NFES_COM_MESMA_NUMERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarNFSeComMesmaNumeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_SEGUIR_CONFIGURACAO_OCORRENCIA_QUANDO_ADICIONADA_METODO_ADICIONAROCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_HABILITAR_FLUXO_PEDIDO_ECOMMERCE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFluxoPedidoEcommerce { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ATUALIZAR_NUMERO_PEDIDO_VINCULADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarNumeroPedidoVinculado { get; set; }

        [Obsolete("Substituido pela RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_UTILIZAR_PROTOCOLO_CARGA_ORIGEM_PARA_CONSULTA_VP_CARGA_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarProtocoloCargaOrigemParaConsultaVPCargaTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_DADOS_REDESPACHO_TRANSBORDO_INFORMACOES_CARGA_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_VALIDAR_TIPO_DE_VEICULO_NO_METODO_INFORMAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_CADASTRAR_VEICULO_AO_INFORMAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarVeiculoAoInformarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_RETORNAR_CARGAS_CANCELADAS_METODO_BUSCAR_PENDETES_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_PERMITIR_CONFIRMAR_ENTREGA_SITUACAO_CARGA_ANDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_CARGAS_EM_QUALQUER_ETAPA_NO_METODO_BUSCAR_CARGA_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_RETORNAR_APENAS_OCORRENCIAS_FINALIZADAS_METODO_BUSCAR_OCORRENCIAS_PENDENTES_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_PERMITIR_REMOVER_DATA_PREVISAO_DATA_PAGAMENTO_METODO_INFORMAR_PREVISAO_PAGAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_DESVINCULAR_PREENCHIMENTO_DATAS_METODOS_INFORMAR_PREVISAO_PAGAMENTO_CONFIRMAR_PAGAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_GERAR_LOG_METODOS_REST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLogMetodosREST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_PERMITIR_ALTERAR_NUMERO_CARGA_QUANDO_FOR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarNumeroCargaQuandoForCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_PERMITIR_REMOVER_VEICULO_NO_METODO_INFORMAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_VINCULAR_REBOQUE_NA_TRACAO_AO_ACIONAR_METODO_GERAR_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_SALVAR_DOCUMENTO_TRANSPORTE_VALIDAR_SITUACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AoSalvarDocumentoTransporteValidarSituacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_NAO_FILTRAR_SEQUENCIAL_CARGA_NO_METODO_ADICIONAR_CARGA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_QUANDO_GERADO_PRE_CTE_RETORNAR_INFORMACAO_FRETE_CTE_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para WebService"; }
        }
    }
}
