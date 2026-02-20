using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PEDIDO", EntityName = "ConfiguracaoPedido", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoPedido", NameType = typeof(ConfiguracaoPedido))]
    public class ConfiguracaoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_APAGAR_CAMPO_ROTA_AO_DUPLICAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApagarCampoRotaAoDuplicarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PESSOAS_NAO_OBRIGATORIO_PRODUTO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PessoasNaoObrigatorioProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_APAGAR_CAMPOS_DATAS_AO_DUPLICAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoApagarCamposDatasAoDuplicarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_INFORMAR_ACRESCIMO_DESCONTO_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarAcrescimoDescontoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_FORMATO_DATA_CARREGAMENTO", TypeType = typeof(FormatoData), NotNull = false)]
        public virtual FormatoData? FormatoDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_FORMATO_HORA_CARREGAMENTO", TypeType = typeof(FormatoHora), NotNull = false)]
        public virtual FormatoHora? FormatoHoraCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_MUDAR_STATUS_PEDIDO_PARA_CANCELADO_APOS_VINCULO_COM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_PERMITIR_IMPORTAR_PEDIDOS_EXISTENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirImportarPedidosExistentes { get; set; }

        #region Importação

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_CONCATENAR_NUMERO_PRE_CARGA_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConcatenarNumeroPreCargaNoPedido { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_BUSCAR_VALORES_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBuscarValoresTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial FilialPadraoImportacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_BUSCAR_EMPRESA_PELO_PROPRIETARIO_DO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarEmpresaPeloProprietarioDoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_UTILIZAR_RELATORIO_PEDIDO_COMO_STATUS_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRelatorioPedidoComoStatusEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_CRIAR_PEDIDO_APENAS_MOTORISTA_SITUACAO_TRABALHANDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_EXIBIR_CAMPOS_RECEBIMENTO_PEDIDO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCamposRecebimentoPedidoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_PERMITIR_ALTERAR_CENTRO_RESULTADO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarCentroResultadoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_PERMITIR_INFORMAR_EXPEDIDOR_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInformarExpedidorNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_BLOQUEAR_INSERCAO_NOTA_COM_EMITENTE_DIFERENTE_REMETENTE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_BLOQUEAR_DUPLICAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDuplicarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_UTILIZAR_ENDERECO_EXPEDIDOR_RECEBEDOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEnderecoExpedidorRecebedorPedido { get; set; }

        /// <summary>
        /// Utilizado pela thread que irá limpar pedidos com a situação EM COTAÇÂO de pedidos com data de criação 
        /// inferior a quantidade de dias do parâmentro.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_DIAS_EXCLUIR_PEDIDO_EM_COTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasExcluirPedidoEmCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_EXCLUIR_TAMBEM_COTACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExcluirTambemACotacaoDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_VALIDAR_MESMA_VIAGEM_E_MESMO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarMesmaViagemEMesmoContainer { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_ATIVAR_VALIDACA_CRIACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarValidacaoCriacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_QUANTIDADE_CARGAS_EM_ABERTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargasEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_PREENCHER_ROTA_FRETE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPreencherRotaFreteAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_EXIBIR_PEDIDOS_DO_DIA_AGENDAMENTO_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExibirPedidosDoDiaAgendamentoPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_IMPORTAR_OCORRENCIAS_DE_PEDIDOS_POR_PLANILHAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarOcorrenciasDePedidosPorPlanilhas { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_SELECIONAR_MODELO_VEICULAR_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSelecionarModeloVeicularAutomaticamente { get; set; }     
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_SUBSTITUIR_EMPRESA_NA_GERACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSubstituirEmpresaNaGeracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_MODELO_EMAIL_AGENDAMENTO_PEDIDO", TypeType = typeof(ModeloEmailAgendamentoPedido), NotNull = false)]
        public virtual ModeloEmailAgendamentoPedido? ModeloEmailAgendamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_VALIDAR_CADASTRO_CONTAINER_PELA_FORMULA_GLOBAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarCadastroContainerPelaFormulaGlobal { get; set; }

        /// <summary>
        /// Configuração para bloquear um pedido automaticamente ao recebe-lo por integração
        /// 55993 - FRIMESA
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_BLOQUEAR_PEDIDO_AO_INTEGRAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearPedidoAoIntegrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_ENVIAR_EMAIL_TRANSPORTADOR_ENTREGA_EM_ATRASO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailTransportadorEntregaEmAtraso { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_CONSULTA_MASSIVA_DE_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConsultaMassivaDePedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITE_INFORMAR_PEDIDOS_DE_SUBSTITUICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarPedidoDeSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_IGNORAR_VALIDACOES_DATAS_DE_PREVISAO_AO_EDITAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarValidacoesDatasPrevisaoAoEditarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_HERDAR_NOTAS_IMPORTADAS_AO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HerdarNotasImportadasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_GERAR_CARGA_AUTOMATICAMENTE_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaAutomaticamenteNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_UTILIZAR_BLOQUEIO_PESSOAS_GRUPO_APENAS_PARA_TOMADOR_DO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_ATUALIZAR_CAMPOS_PEDIDO_POR_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarCamposPedidoPorPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_PERMITIR_INFORMAR_VEICULO_DUPLICADO_PEDIDO_CARGA_ABERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_HORA_CARREGAMENTO_EM_PEDIDOS_DE_COLETA_E_EXIBIR_CODIGOS_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_HABILITAR_BID_TRANSPORTE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarBIDTransportePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_USAR_FATOR_CONVERSAO_PRODUTO_EM_PEDIDO_PALETIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarFatorConversaoProdutoEmPedidoPaletizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_PERMITIR_SELECIONAR_CENTRO_CARREGAMENTO_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarCentroDeCarregamentoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_UTILIZAR_CAMPO_DE_MOTIVO_DE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCampoDeMotivoDePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_AJUSTAR_PARTICIPANTES_PEDIDO_CTE_EMITIDO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarParticipantesPedidoCTeEmitidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_NAO_LEVAR_NUMERO_COTACAO_PARA_PEDIDO_GERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoLevarNumeroCotacaoParaPedidoGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_IMPORTAR_PARALELIZANDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarParalelizando { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_SEMPRE_INFORMAR_DESTINATARIO_INFORMADO_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreConsiderarDestinatarioInformadoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_EXIGIR_ROTA_ROTERIZADA_NO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRotaRoteirizadaNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_EXIBIR_AUDITORIA_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirAuditoriaPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_QUANTIDADE_DIAS_DATA_COLETA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasDataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_UTILIZAR_PARAMETROS_BUSCA_AUTOMATICA_CLIENTE_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarParametrosBuscaAutomaticaClienteImportacao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_REMOVER_OBSERVACOES_ENTREGA_AO_REMOVER_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverObservacoesDeEntregaAoRemoverPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_HABILITAR_OPCOES_DUPLICACAO_PEDIDO_PARA_DEVOLUCAO_TOTAL_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COP_ATUALIZAR_CARGA_AO_IMPORTAR_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarCargaAoImportarPlanilha { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para pedidos"; }
        }
    }
}
