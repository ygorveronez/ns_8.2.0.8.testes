using WebOptimizer;

namespace SGT.WebAdmin.Areas.Relatorios
{
    public static class BundleConfig
    {
        public static void RegisterBundlesRelatorios(this IAssetPipeline pipeline)
        {

            #region Global

            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/businessIntelligenceGlobal", "/Areas/BusinessIntelligence/ViewsScripts/BusinessIntelligence/Global/**/*.js").UseContentRoot();

            #endregion

            #region Cargas

            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadesCarga", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/Quantidade/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/direcionamentoOperador", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/DirecionamentoOperador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/valorMedioFrete", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/ValorMedioFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/indiceAtraso", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/IndiceAtraso/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/faturamentoTransportador", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/FaturamentoTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadePorRota", "/Areas/BusinessIntelligence/ViewsScripts/Cargas/QuantidadePorRota/**/*.js").UseContentRoot();

            #endregion

            #region Chamados

            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/chamado", "/Areas/BusinessIntelligence/ViewsScripts/Chamados/Chamado/**/*.js").UseContentRoot();

            #endregion

            #region Pallets

            pipeline.AddJavaScriptBundle("/businessIntelligence/scripts/quantidadePallets", "/Areas/BusinessIntelligence/ViewsScripts/Pallets/QuantidadePallets/**/*.js").UseContentRoot();

            #endregion

            #region Global
            pipeline.AddJavaScriptBundle("/relatorios/scripts/relatoriosGlobal", "/Areas/Relatorios/ViewsScripts/Relatorios/Global/**/*.js").UseContentRoot();
            #endregion

            #region Auditoria
            pipeline.AddJavaScriptBundle("/relatorios/scripts/auditoria", "/Areas/Relatorios/ViewsScripts/Auditoria/**/*.js").UseContentRoot();
            #endregion

            #region Avarias
            pipeline.AddJavaScriptBundle("/relatorios/scripts/avarias", "/Areas/Relatorios/ViewsScripts/Avarias/Analitico/**/*.js").UseContentRoot();
            #endregion

            #region CRM
            pipeline.AddJavaScriptBundle("/relatorios/scripts/prospeccao", "/Areas/Relatorios/ViewsScripts/CRM/Prospeccao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/agendaTarefas", "/Areas/Relatorios/ViewsScripts/CRM/AgendaTarefas/**/*.js").UseContentRoot();
            #endregion

            #region Compras

            pipeline.AddJavaScriptBundle("/relatorios/scripts/ordemCompra", "/Areas/Relatorios/ViewsScripts/Compras/OrdemCompra/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/notaEntradaOrdemCompra", "/Areas/Relatorios/ViewsScripts/Compras/NotaEntradaOrdemCompra/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pontuacaoComprador", "/Areas/Relatorios/ViewsScripts/Compras/PontuacaoComprador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pontuacaoFornecedor", "/Areas/Relatorios/ViewsScripts/Compras/PontuacaoFornecedor/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/sugestaoCompra", "/Areas/Relatorios/ViewsScripts/Compras/SugestaoCompra/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/requisicaoMercadoria", "/Areas/Relatorios/ViewsScripts/Compras/RequisicaoMercadoria/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cotacaoCompra", "/Areas/Relatorios/ViewsScripts/Compras/CotacaoCompra/**/*.js").UseContentRoot();

            #endregion

            #region Filial
            pipeline.AddJavaScriptBundle("/relatorios/scripts/filial", "/Areas/Relatorios/ViewsScripts/Filiais/Filial/**/*.js").UseContentRoot();
            #endregion

            #region Frota

            pipeline.AddJavaScriptBundle("/relatorios/scripts/pneu", "/Areas/Relatorios/ViewsScripts/Frota/Pneu/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/movimentacaoPneuVeiculo", "/Areas/Relatorios/ViewsScripts/Frota/MovimentacaoPneuVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pneuHistorico", "/Areas/Relatorios/ViewsScripts/Frota/PneuHistorico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ordemServico", "/Areas/Relatorios/ViewsScripts/Frota/OrdemServico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/servicoVeiculo", "/Areas/Relatorios/ViewsScripts/Frota/ServicoVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pneuCustoEstoque", "/Areas/Relatorios/ViewsScripts/Frota/PneuCustoEstoque/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/planejamentoFrotaDia", "/Areas/Relatorios/ViewsScripts/Frota/PlanejamentoFrotaDia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pneuPorVeiculo", "/Areas/Relatorios/ViewsScripts/Frota/PneuPorVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/sinistro", "/Areas/Relatorios/ViewsScripts/Frota/Sinistro/**/*.js").UseContentRoot();

            #endregion

            #region Frotas

            pipeline.AddJavaScriptBundle("/relatorios/scripts/abastecimento", "/Areas/Relatorios/ViewsScripts/Frotas/Abastecimento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/motorista", "/Areas/Relatorios/ViewsScripts/Frotas/Motorista/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedagio", "/Areas/Relatorios/ViewsScripts/Frotas/Pedagio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/multa", "/Areas/Relatorios/ViewsScripts/Frotas/Multa/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/multaParcela", "/Areas/Relatorios/ViewsScripts/Frotas/MultaParcela/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaVeiculo", "/Areas/Relatorios/ViewsScripts/Frotas/DespesaVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaOrdemServico", "/Areas/Relatorios/ViewsScripts/Frotas/DespesaOrdemServico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/abastecimentoNotaEntrada", "/Areas/Relatorios/ViewsScripts/Frotas/AbastecimentoNotaEntrada/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/manutencaoVeiculo", "/Areas/Relatorios/ViewsScripts/Frotas/ManutencaoVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/motoristaExtratoSaldo", "/Areas/Relatorios/ViewsScripts/Frotas/MotoristaExtratoSaldo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaDetalhadaOrdemServico", "/Areas/Relatorios/ViewsScripts/Frotas/DespesaDetalhadaOrdemServico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/retornoAbastecimentoAngellira", "/Areas/Relatorios/ViewsScripts/Frotas/RetornoAbastecimentoAngellira/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ordemServicoPorMecanico", "/Areas/Relatorios/ViewsScripts/Frotas/OrdemServicoPorMecanico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaOrdemServicoProduto", "/Areas/Relatorios/ViewsScripts/Frotas/DespesaOrdemServicoProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/abastecimentoTicketLog", "/Areas/Relatorios/ViewsScripts/Frotas/AbastecimentoTicketLog/**/*.js").UseContentRoot();

            #endregion

            #region Canhotos
            pipeline.AddJavaScriptBundle("/relatorios/scripts/canhotos", "/Areas/Relatorios/ViewsScripts/Canhotos/Canhoto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/movimentacaoCanhoto", "/Areas/Relatorios/ViewsScripts/Canhotos/HistoricoMovimentacaoCanhoto/**/*.js").UseContentRoot();

            #endregion

            #region Chamados
            pipeline.AddJavaScriptBundle("/relatorios/scripts/chamadoOcorrencia", "/Areas/Relatorios/ViewsScripts/Chamados/ChamadoOcorrencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/chamadoDevolucao", "/Areas/Relatorios/ViewsScripts/Chamados/ChamadoDevolucao/**/*.js").UseContentRoot();
            #endregion

            #region CheckListsUsuario
            pipeline.AddJavaScriptBundle("/relatorios/scripts/checkListUsuario", "/Areas/Relatorios/ViewsScripts/CheckListsUsuario/CheckListUsuario/**/*.js").UseContentRoot();
            #endregion


            #region ConfiguracaoContabil
            pipeline.AddJavaScriptBundle("/relatorios/scripts/configuracaoCentroResultado", "/Areas/Relatorios/ViewsScripts/ConfiguracaoContabil/**/*.js").UseContentRoot();
            #endregion

            #region Containers
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleContainer", "/Areas/Relatorios/ViewsScripts/Containers/ControleContainer/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/historicoMovimentacaoContainers", "/Areas/Relatorios/ViewsScripts/Containers/HistoricoMovimentacaoContainers/**/*.js").UseContentRoot();
            #endregion

            #region Contatos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/contatoCliente", "/Areas/Relatorios/ViewsScripts/Contatos/ContatoCliente/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tipoContatoCliente", "/Areas/Relatorios/ViewsScripts/Contatos/TipoContatoCliente/**/*.js").UseContentRoot();

            #endregion

            #region Documentos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/faturaCIOT", "/Areas/Relatorios/ViewsScripts/Documentos/FaturaCIOT/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaCIOT", "/Areas/Relatorios/ViewsScripts/Documentos/CargaCIOT/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/dadosDocsys", "/Areas/Relatorios/ViewsScripts/Documentos/DadosDocsys/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/serieDocumentos", "/Areas/Relatorios/ViewsScripts/Documentos/SerieDocumentos/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaCIOTPedido", "/Areas/Relatorios/ViewsScripts/Documentos/CargaCIOTPedido/**/*.js").UseContentRoot();

            #endregion

            #region Escrituracao

            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteContabil", "/Areas/Relatorios/ViewsScripts/Escrituracao/FreteContabil/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/competencia", "/Areas/Relatorios/ViewsScripts/Escrituracao/Competencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/saldoProvisao", "/Areas/Relatorios/ViewsScripts/Escrituracao/SaldoProvisao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/integracaoLotePagamento", "/Areas/Relatorios/ViewsScripts/Escrituracao/IntegracaoLotePagamento/**/*.js").UseContentRoot();

            #endregion

            #region FaturamentosMensais

            pipeline.AddJavaScriptBundle("/relatorios/scripts/cobrancasMensais", "/Areas/Relatorios/ViewsScripts/FaturamentosMensais/CobrancaMensal/**/*.js").UseContentRoot();

            #endregion

            #region Financeiros

            pipeline.AddJavaScriptBundle("/relatorios/scripts/movimentoFinanceiro", "/Areas/Relatorios/ViewsScripts/Financeiros/MovimentoFinanceiro/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/planoConta", "/Areas/Relatorios/ViewsScripts/Financeiros/PlanoConta/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/extratoConta", "/Areas/Relatorios/ViewsScripts/Financeiros/ExtratoConta/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/titulo", "/Areas/Relatorios/ViewsScripts/Financeiros/Titulo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/extratoMotorista", "/Areas/Relatorios/ViewsScripts/Financeiros/ExtratoMotorista/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/balanceteGerencial", "/Areas/Relatorios/ViewsScripts/Financeiros/BalanceteGerencial/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/posicaoContasReceber", "/Areas/Relatorios/ViewsScripts/Financeiros/PosicaoContasReceber/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/descontoAcrescimoCTe", "/Areas/Relatorios/ViewsScripts/Financeiros/DescontoAcrescimoCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tipoMovimento", "/Areas/Relatorios/ViewsScripts/Financeiros/TipoMovimento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/aliquotaICMSCTe", "/Areas/Relatorios/ViewsScripts/Financeiros/AliquotaICMSCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/descontoAcrescimoFatura", "/Areas/Relatorios/ViewsScripts/Financeiros/DescontoAcrescimoFatura/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/faturamento", "/Areas/Relatorios/ViewsScripts/Financeiros/Faturamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/documentoFaturamento", "/Areas/Relatorios/ViewsScripts/Financeiros/DocumentoFaturamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/posicaoDocumentoReceber", "/Areas/Relatorios/ViewsScripts/Financeiros/PosicaoDocumentoReceber/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/retornoBoleto", "/Areas/Relatorios/ViewsScripts/Financeiros/RetornoBoleto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cteTituloReceber", "/Areas/Relatorios/ViewsScripts/Financeiros/CTeTituloReceber/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/faturamentosMensais", "/Areas/Relatorios/ViewsScripts/Financeiros/FaturamentoMensal/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/perfilClientes", "/Areas/Relatorios/ViewsScripts/Financeiros/PerfilCliente/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tituloAcrescimoDesconto", "/Areas/Relatorios/ViewsScripts/Financeiros/TituloAcrescimoDesconto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/dre", "/Areas/Relatorios/ViewsScripts/Financeiros/DRE/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/dreGerencial", "/Areas/Relatorios/ViewsScripts/Financeiros/DREGerencial/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/planoOrcamentario", "/Areas/Relatorios/ViewsScripts/Financeiros/PlanoOrcamentario/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/fluxoCaixa", "/Areas/Relatorios/ViewsScripts/Financeiros/FluxoCaixa/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tituloSemMovimento", "/Areas/Relatorios/ViewsScripts/Financeiros/TituloSemMovimento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/retornoPagamento", "/Areas/Relatorios/ViewsScripts/Financeiros/RetornoPagamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pagamentoAgregado", "/Areas/Relatorios/ViewsScripts/Financeiros/PagamentoAgregado/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cheque", "/Areas/Relatorios/ViewsScripts/Financeiros/Cheque/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/baixaTitulo", "/Areas/Relatorios/ViewsScripts/Financeiros/BaixaTitulo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaMensal", "/Areas/Relatorios/ViewsScripts/Financeiros/DespesaMensal/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/naturezaDaOperacao", "/Areas/Relatorios/ViewsScripts/Financeiros/NaturezaDaOperacao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/CFOP", "/Areas/Relatorios/ViewsScripts/Financeiros/CFOP/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/extratoBancario", "/Areas/Relatorios/ViewsScripts/Financeiros/ExtratoBancario/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/extratoAcertoViagem", "/Areas/Relatorios/ViewsScripts/Financeiros/ExtratoAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/posicaoContasPagar", "/Areas/Relatorios/ViewsScripts/Financeiros/PosicaoContasPagar/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/rateioDespesaVeiculo", "/Areas/Relatorios/ViewsScripts/Financeiros/RateioDespesaVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/conciliacaoBancaria", "/Areas/Relatorios/ViewsScripts/Financeiros/ConciliacaoBancaria/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/conferenciaFiscal", "/Areas/Relatorios/ViewsScripts/Financeiros/ConferenciaFiscal/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/contratoFinanceiro", "/Areas/Relatorios/ViewsScripts/Financeiros/ContratoFinanceiro/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/condicoesPagamentoTransportador", "/Areas/Relatorios/ViewsScripts/Financeiros/CondicoesPagamentoTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/liberacaoPagamentoProvedor", "/Areas/Relatorios/ViewsScripts/Financeiros/LiberacaoPagamentoProvedor/**/*.js").UseContentRoot();

            #endregion

            #region Gerenciamento de Irregularidades 

            pipeline.AddJavaScriptBundle("/relatorios/scripts/moduloControle", "/Areas/Relatorios/ViewsScripts/GerenciamentoIrregularidades/moduloControle/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/processamentoModuloControle", "/Areas/Relatorios/ViewsScripts/GerenciamentoIrregularidades/processamentoModuloControle/**/*.js").UseContentRoot();

            #endregion

            #region Gestão de Patio

            pipeline.AddJavaScriptBundle("/relatorios/scripts/temposGestaoPatio", "/Areas/Relatorios/ViewsScripts/GestaoPatio/TemposGestaoPatio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/fluxoHorario", "/Areas/Relatorios/ViewsScripts/GestaoPatio/FluxoHorario/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/guaritaCheckList", "/Areas/Relatorios/ViewsScripts/GestaoPatio/GuaritaCheckList/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleVisita", "/Areas/Relatorios/ViewsScripts/GestaoPatio/ControleVisita/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/checkList", "/Areas/Relatorios/ViewsScripts/GestaoPatio/CheckList/**/*.js").UseContentRoot();

            #endregion

            #region ICMS

            pipeline.AddJavaScriptBundle("/relatorios/scripts/regraICMS", "/Areas/Relatorios/ViewsScripts/ICMS/RegraICMS/**/*.js").UseContentRoot();

            #endregion

            #region Integrações

            pipeline.AddJavaScriptBundle("/relatorios/scripts/indicadorIntegracaoCTe", "/Areas/Relatorios/ViewsScripts/Integracoes/IndicadorIntegracaoCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidoAguardandoIntegracao", "/Areas/Relatorios/ViewsScripts/Integracoes/PedidoAguardandoIntegracao/**/*.js").UseContentRoot();

            #endregion

            #region AcertoViagem

            pipeline.AddJavaScriptBundle("/relatorios/scripts/resultadoAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/ResultadoAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/despesaAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/DespesaAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/acertoDeViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/AcertoDeViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/resultadoAnualAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/ResultadoAnualAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaCompartilhada", "/Areas/Relatorios/ViewsScripts/AcertoViagem/CargaCompartilhada/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/diariaAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/DiariaAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comissaoAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/ComissaoAcertoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ultimoAcertoMotorista", "/Areas/Relatorios/ViewsScripts/AcertoViagem/UltimoAcertoMotorista/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/jornadaMotorista", "/Areas/Relatorios/ViewsScripts/AcertoViagem/JornadaMotorista/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tempoDeViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/TempoDeViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/bonificacaoAcertoViagem", "/Areas/Relatorios/ViewsScripts/AcertoViagem/BonificacaoAcertoViagem/**/*.js").UseContentRoot();


            #endregion

            #region Administrativo

            pipeline.AddJavaScriptBundle("/relatorios/scripts/logEnvioEmail", "/Areas/Relatorios/ViewsScripts/Administrativo/LogEnvioEmail/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/licenca", "/Areas/Relatorios/ViewsScripts/Administrativo/Licenca/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/logEnvioSMS", "/Areas/Relatorios/ViewsScripts/Administrativo/LogEnvioSMS/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/licencaVeiculo", "/Areas/Relatorios/ViewsScripts/Administrativo/LicencaVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/logAcesso", "/Areas/Relatorios/ViewsScripts/Administrativo/LogAcesso/**/*.js").UseContentRoot();

            #endregion

            #region Atendimentos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/chamados", "/Areas/Relatorios/ViewsScripts/Atendimentos/Chamado/**/*.js").UseContentRoot();

            #endregion

            #region NFe

            pipeline.AddJavaScriptBundle("/relatorios/scripts/notasEmitidas", "/Areas/Relatorios/ViewsScripts/NFe/NotasEmitidas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/estoqueProdutos", "/Areas/Relatorios/ViewsScripts/NFe/EstoqueProdutos/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/historicoEstoque", "/Areas/Relatorios/ViewsScripts/NFe/HistoricoEstoque/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/notasEmitidasAdministrativo", "/Areas/Relatorios/ViewsScripts/NFe/NotasEmitidasAdministrativo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/girosEstoques", "/Areas/Relatorios/ViewsScripts/NFe/GiroEstoque/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/curvaABCProdutos", "/Areas/Relatorios/ViewsScripts/NFe/CurvaABCProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/curvaABCPessoas", "/Areas/Relatorios/ViewsScripts/NFe/CurvaABCPessoa/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/historicoProdutos", "/Areas/Relatorios/ViewsScripts/NFe/HistoricoProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comprasVendasNCM", "/Areas/Relatorios/ViewsScripts/NFe/CompraVendaNCM/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/produtosSemMovimentacoes", "/Areas/Relatorios/ViewsScripts/NFe/ProdutoSemMovimentacao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/vendasReduzidas", "/Areas/Relatorios/ViewsScripts/NFe/VendasReduzidas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/notasDetalhadas", "/Areas/Relatorios/ViewsScripts/NFe/NotasDetalhadas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/notas", "/Areas/Relatorios/ViewsScripts/NFe/Notas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidosNotas", "/Areas/Relatorios/ViewsScripts/NFe/PedidoNota/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/nfes", "/Areas/Relatorios/ViewsScripts/NFe/NFes/**/*.js").UseContentRoot();

            #endregion

            #region Notas Fiscais

            pipeline.AddJavaScriptBundle("/relatorios/scripts/ItemNaoConformidade", "/Areas/Relatorios/ViewsScripts/NotasFiscais/ItemNaoConformidade/**/*.js").UseContentRoot();

            #endregion Notas Fiscais

            #region CTe

            pipeline.AddJavaScriptBundle("/relatorios/scripts/cteEmitido", "/Areas/Relatorios/ViewsScripts/CTe/CTeEmitido/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ctes", "/Areas/Relatorios/ViewsScripts/CTe/CTes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/faturamentoPorGrupoPessoas", "/Areas/Relatorios/ViewsScripts/CTe/FaturamentoPorGrupoPessoas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/posicaoCTe", "/Areas/Relatorios/ViewsScripts/CTe/PosicaoCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tomador", "/Areas/Relatorios/ViewsScripts/CTe/Tomador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaCTeIntegracao", "/Areas/Relatorios/ViewsScripts/CTe/CargaCTeIntegracao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comparativoMensalFaturamentoGrupoPessoas", "/Areas/Relatorios/ViewsScripts/CTe/ComparativoMensalFaturamentoGrupoPessoas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/componenteFreteCTe", "/Areas/Relatorios/ViewsScripts/CTe/ComponenteFreteCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/afrmmControl", "/Areas/Relatorios/ViewsScripts/CTe/AFRMMControl/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/container", "/Areas/Relatorios/ViewsScripts/CTe/Container/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/apuracaoICMS", "/Areas/Relatorios/ViewsScripts/CTe/ApuracaoICMS/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/subcontratacao", "/Areas/Relatorios/ViewsScripts/CTe/Subcontratacao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/faturamentoPorCTe", "/Areas/Relatorios/ViewsScripts/CTe/FaturamentoPorCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/nfeCTeContainer", "/Areas/Relatorios/ViewsScripts/CTe/NFeCTeContainer/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/afrmmControlMercante", "/Areas/Relatorios/ViewsScripts/CTe/AFRMMControlMercante/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/valePedagio", "/Areas/Relatorios/ViewsScripts/CTe/ValePedagio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/takeOrPay", "/Areas/Relatorios/ViewsScripts/CTe/TakeOrPay/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ctesSubcontratados", "/Areas/Relatorios/ViewsScripts/CTe/CTesSubcontratados/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/auditoriaCTe", "/Areas/Relatorios/ViewsScripts/CTe/AuditoriaCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comissaoVendedorCTe", "/Areas/Relatorios/ViewsScripts/CTe/ComissaoVendedorCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/custoRentabilidadeCteCrt", "/Areas/Relatorios/ViewsScripts/CTe/CustoRentabilidadeCteCrt/**/*.js").UseContentRoot();

            #endregion

            #region Frete

            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteComponentes", "/Areas/Relatorios/ViewsScripts/Fretes/FreteComponentes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comissaoGrupoProduto", "/Areas/Relatorios/ViewsScripts/Fretes/ComissaoGrupoProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/comissaoProduto", "/Areas/Relatorios/ViewsScripts/Fretes/ComissaoProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tabelaFreteRota", "/Areas/Relatorios/ViewsScripts/Fretes/TabelaFreteRota/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteTerceirizado", "/Areas/Relatorios/ViewsScripts/Fretes/FreteTerceirizado/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteTerceirizadoPorCTe", "/Areas/Relatorios/ViewsScripts/Fretes/FreteTerceirizadoPorCTe/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/contratoFreteTransportador", "/Areas/Relatorios/ViewsScripts/Fretes/ContratoFreteTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/transportadoresSemContrato", "/Areas/Relatorios/ViewsScripts/Fretes/TransportadoresSemContrato/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/configuracaoSubcontratacaoTabelaFrete", "/Areas/Relatorios/ViewsScripts/Fretes/ConfiguracaoSubcontratacaoTabelaFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/rotaFrete", "/Areas/Relatorios/ViewsScripts/Fretes/RotaFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/configuracaoTabelaFrete", "/Areas/Relatorios/ViewsScripts/Fretes/ConfiguracaoTabelaFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteTerceirizadoAcrescimoDesconto", "/Areas/Relatorios/ViewsScripts/Fretes/FreteTerceirizadoAcrescimoDesconto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/freteTerceirizadoValePedagio", "/Areas/Relatorios/ViewsScripts/Fretes/FreteTerceirizadoValePedagio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/contratoFreteAcrescimoDesconto", "/Areas/Relatorios/ViewsScripts/Fretes/ContratoFreteAcrescimoDesconto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/provisaoVolumetria", "/Areas/Relatorios/ViewsScripts/Fretes/ProvisaoVolumetria/**/*.js").UseContentRoot();

            #endregion

            #region MDFe

            pipeline.AddJavaScriptBundle("/relatorios/scripts/mdfes", "/Areas/Relatorios/ViewsScripts/MDFe/Mdfes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/mdfesAverbados", "/Areas/Relatorios/ViewsScripts/MDFe/MDFesAverbados/**/*.js").UseContentRoot();

            #endregion

            #region Minuta

            pipeline.AddJavaScriptBundle("/relatorios/scripts/minutas", "/Areas/Relatorios/ViewsScripts/Minutas/Minuta/**/*.js").UseContentRoot();

            #endregion

            #region Logística

            pipeline.AddJavaScriptBundle("/relatorios/scripts/tempoCarregamento", "/Areas/Relatorios/ViewsScripts/Logistica/TempoCarregamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/guaritaTMS", "/Areas/Relatorios/ViewsScripts/Logistica/GuaritaTMS/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/filaCarregamentoHistorico", "/Areas/Relatorios/ViewsScripts/Logistica/FilaCarregamentoHistorico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoNivelServico", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoNivelServico/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoVeiculoAlvo", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoVeiculoAlvo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoVeiculoPosicao", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoVeiculoPosicao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoPosicaoDaFrota", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoPosicaoDaFrota/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoPosicaoFrotaRastreamento", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoPosicaoFrotaRastreamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoTratativaAlerta", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoTratativaAlerta/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoAlerta", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoAlerta/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoHistoricoTemperatura", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoHistoricoTemperatura/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/janelaAgendamento", "/Areas/Relatorios/ViewsScripts/Logistica/JanelaAgendamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/agendaCancelada", "/Areas/Relatorios/ViewsScripts/Logistica/AgendaCancelada/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/consolidacaoGas", "/Areas/Relatorios/ViewsScripts/Logistica/ConsolidacaoGas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/janelaDisponivelAgendamento", "/Areas/Relatorios/ViewsScripts/Logistica/JanelaDisponivelAgendamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoTempoVeiculo", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoTempoVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleTempoViagem", "/Areas/Relatorios/ViewsScripts/Logistica/ControleTempoViagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/monitoramentoControleEntrega", "/Areas/Relatorios/ViewsScripts/Logistica/MonitoramentoControleEntrega/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pracaPedagio", "/Areas/Relatorios/ViewsScripts/Logistica/PracaPedagio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/janelaCarregamentoIntegracao", "/Areas/Relatorios/ViewsScripts/Logistica/JanelaCarregamentoIntegracao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/navio", "/Areas/Relatorios/ViewsScripts/Logistica/Navio/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/historicoJanelaCarregamento", "/Areas/Relatorios/ViewsScripts/Logistica/HistoricoJanelaCarregamento/**/*.js").UseContentRoot();

            #endregion

            #region Cargas

            pipeline.AddJavaScriptBundle("/relatorios/scripts/carga", "/Areas/Relatorios/ViewsScripts/Cargas/Carga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/direcionamentoOperador", "/Areas/Relatorios/ViewsScripts/Cargas/DirecionamentoOperador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/quantidadeCarga", "/Areas/Relatorios/ViewsScripts/Cargas/Quantidade/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/quantidadeDescarga", "/Areas/Relatorios/ViewsScripts/Cargas/QuantidadeDescarga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/taxaOcupacaoVeiculo", "/Areas/Relatorios/ViewsScripts/Cargas/TaxaOcupacaoVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/taxaIncidenciaFrete", "/Areas/Relatorios/ViewsScripts/Cargas/TaxaIncidenciaFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedido", "/Areas/Relatorios/ViewsScripts/Cargas/Pedido/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/encaixes", "/Areas/Relatorios/ViewsScripts/Cargas/Encaixes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/documentoEmissaoNFSManual", "/Areas/Relatorios/ViewsScripts/Cargas/DocumentoEmissaoNFSManual/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaProduto", "/Areas/Relatorios/ViewsScripts/Cargas/CargaProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaProdutoTransportador", "/Areas/Relatorios/ViewsScripts/Cargas/CargaProdutoTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidoProduto", "/Areas/Relatorios/ViewsScripts/Cargas/PedidoProduto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/alteracaoFrete", "/Areas/Relatorios/ViewsScripts/Cargas/AlteracaoFrete/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/preCarga", "/Areas/Relatorios/ViewsScripts/Cargas/PreCarga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/paradas", "/Areas/Relatorios/ViewsScripts/Cargas/Paradas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/avaliacaoEntregaPedido", "/Areas/Relatorios/ViewsScripts/Cargas/AvaliacaoEntregaPedido/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaIntegracao", "/Areas/Relatorios/ViewsScripts/Cargas/CargaIntegracao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaPedidoEmbarcador", "/Areas/Relatorios/ViewsScripts/Cargas/CargaPedidoEmbarcador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaEntregaPedido", "/Areas/Relatorios/ViewsScripts/Cargas/CargaEntregaPedido/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/rotaControleEntrega", "/Areas/Relatorios/ViewsScripts/Cargas/RotaControleEntrega/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/agendamentoEntregaPedido", "/Areas/Relatorios/ViewsScripts/Cargas/AgendamentoEntregaPedido/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/gestaoCarga", "/Areas/Relatorios/ViewsScripts/Cargas/GestaoCarga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleEntrega", "/Areas/Relatorios/ViewsScripts/Cargas/ControleEntrega/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaEntregaChecklist", "/Areas/Relatorios/ViewsScripts/Cargas/CargaEntregaChecklist/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaIntegracaoDadosTransportes", "/Areas/Relatorios/ViewsScripts/Cargas/CargaIntegracaoDadosTransportes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pacotes", "/Areas/Relatorios/ViewsScripts/Cargas/Pacotes/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/historicoVinculo", "/Areas/Relatorios/ViewsScripts/Cargas/HistoricoVinculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaViagemEventos", "/Areas/Relatorios/ViewsScripts/Cargas/CargaViagemEventos/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargasComInteresseTransportadorTerceiro", "/Areas/Relatorios/ViewsScripts/Cargas/CargasComInteresseTransportadorTerceiro/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/modeloVeicularCarga", "/Areas/Relatorios/ViewsScripts/Cargas/ModeloVeicularCarga/**/*.js").UseContentRoot();


            #endregion

            #region Creditos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/cargaComplementoFrete", "/Areas/Relatorios/ViewsScripts/Creditos/CargaComplementoFrete/**/*.js").UseContentRoot();

            #endregion

            #region Expedicao

            pipeline.AddJavaScriptBundle("/relatorios/scripts/expedicaoProdutos", "/Areas/Relatorios/ViewsScripts/Expedicao/ExpedicaoProdutos/**/*.js").UseContentRoot();

            #endregion

            #region Ocorrências

            pipeline.AddJavaScriptBundle("/relatorios/scripts/ocorrencia", "/Areas/Relatorios/ViewsScripts/Ocorrencias/Ocorrencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ocorrenciaEntrega", "/Areas/Relatorios/ViewsScripts/Ocorrencias/OcorrenciaEntrega/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/TipoOcorrencia", "/Areas/Relatorios/ViewsScripts/Ocorrencias/TipoOcorrencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/regrasAutorizacaoOcorrencia", "/Areas/Relatorios/ViewsScripts/Ocorrencias/RegrasAutorizacaoOcorrencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ocorrenciaCentroCusto", "/Areas/Relatorios/ViewsScripts/Ocorrencias/OcorrenciaCentroCusto/**/*.js").UseContentRoot();

            #endregion

            #region Operacional

            pipeline.AddJavaScriptBundle("/relatorios/scripts/configuracaoOperadores", "/Areas/Relatorios/ViewsScripts/Operacional/**/*.js").UseContentRoot();

            #endregion

            #region Patrimonio

            pipeline.AddJavaScriptBundle("/relatorios/scripts/bens", "/Areas/Relatorios/ViewsScripts/Patrimonio/Bem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/mapaDepreciacao", "/Areas/Relatorios/ViewsScripts/Patrimonio/MapaDepreciacao/**/*.js").UseContentRoot();

            #endregion

            #region PedidoVenda

            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidosVendas", "/Areas/Relatorios/ViewsScripts/PedidosVendas/PedidoVenda/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/vendaDireta", "/Areas/Relatorios/ViewsScripts/PedidosVendas/VendaDireta/**/*.js").UseContentRoot();

            #endregion

            #region PreCTes

            pipeline.AddJavaScriptBundle("/relatorios/scripts/preCte", "/Areas/Relatorios/ViewsScripts/PreCTes/PreCTe/**/*.js").UseContentRoot();

            #endregion

            #region Veículos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/classificacaoVeiculo", "/Areas/Relatorios/ViewsScripts/Veiculos/ClassificacaoVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/relatorioVeiculo", "/Areas/Relatorios/ViewsScripts/Veiculos/Veiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/manutencao", "/Areas/Relatorios/ViewsScripts/Veiculos/Manutencao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/veiculoReceitaDespesa", "/Areas/Relatorios/ViewsScripts/Veiculos/ReceitaDespesa/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/responsavelVeiculo", "/Areas/Relatorios/ViewsScripts/Veiculos/ResponsavelVeiculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/equipamento", "/Areas/Relatorios/ViewsScripts/Veiculos/Equipamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/relatorioHistoricoVeiculoVinculo", "/Areas/Relatorios/ViewsScripts/Veiculos/HistoricoVeiculoVinculo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/tacografo", "/Areas/Relatorios/ViewsScripts/Veiculos/Tacografo/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/relatorioHistoricoMotoristaCentro", "/Areas/Relatorios/ViewsScripts/Veiculos/HistoricoMotoristaCentro/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/relatorioHistoricoVeiculo", "/Areas/Relatorios/ViewsScripts/Veiculos/HistoricoVeiculo/**/*.js").UseContentRoot();

            #endregion

            #region Pedidos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidoOcorrencia", "/Areas/Relatorios/ViewsScripts/Pedidos/PedidoOcorrencia/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidoDevolucao", "/Areas/Relatorios/ViewsScripts/Pedidos/PedidoDevolucao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/booking", "/Areas/Relatorios/ViewsScripts/Pedidos/Booking/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pedidoRetornoOcorrencia", "/Areas/Relatorios/ViewsScripts/Pedidos/PedidoRetornoOcorrencia/**/*.js").UseContentRoot();

            #endregion

            #region Pallets

            pipeline.AddJavaScriptBundle("/relatorios/scripts/valorDescarga", "/Areas/Relatorios/ViewsScripts/Pallets/ValorDescarga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/devolucaoPallets", "/Areas/Relatorios/ViewsScripts/Pallets/DevolucaoPallets/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/estoqueFilial", "/Areas/Relatorios/ViewsScripts/Pallets/EstoqueFilial/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/estoqueTransportador", "/Areas/Relatorios/ViewsScripts/Pallets/EstoqueTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/fechamentoTransportador", "/Areas/Relatorios/ViewsScripts/Pallets/FechamentoTransportador/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/estoqueCompraPallet", "/Areas/Relatorios/ViewsScripts/Pallets/EstoqueCompraPallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleReformaPallet", "/Areas/Relatorios/ViewsScripts/Pallets/ControleReformaPallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleTransferenciaPallet", "/Areas/Relatorios/ViewsScripts/Pallets/ControleTransferenciaPallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleValePallet", "/Areas/Relatorios/ViewsScripts/Pallets/ControleValePallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleEntradaSaidaPallet", "/Areas/Relatorios/ViewsScripts/Pallets/ControleEntradaSaidaPallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/controleAvariaPallet", "/Areas/Relatorios/ViewsScripts/Pallets/ControleAvariaPallet/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/estoqueCliente", "/Areas/Relatorios/ViewsScripts/Pallets/EstoqueCliente/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/taxasDescarga", "/Areas/Relatorios/ViewsScripts/Pallets/TaxasDescarga/**/*.js").UseContentRoot();

            #endregion

            #region Pessoas

            pipeline.AddJavaScriptBundle("/relatorios/scripts/perfilAcesso", "/Areas/Relatorios/ViewsScripts/Pessoas/PerfilAcesso/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pessoa", "/Areas/Relatorios/ViewsScripts/Pessoas/Pessoa/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/funcionarioComissao", "/Areas/Relatorios/ViewsScripts/Pessoas/FuncionarioComissao/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/colaboradorSituacaoLancamento", "/Areas/Relatorios/ViewsScripts/Pessoas/ColaboradorSituacaoLancamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pessoaDescarga", "/Areas/Relatorios/ViewsScripts/Pessoas/PessoaDescarga/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/grupoPessoas", "/Areas/Relatorios/ViewsScripts/Pessoas/GrupoPessoas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/enderecoSecundario", "/Areas/Relatorios/ViewsScripts/Pessoas/EnderecoSecundario/**/*.js").UseContentRoot();

            #endregion

            #region Produtos

            pipeline.AddJavaScriptBundle("/relatorios/scripts/produto", "/Areas/Relatorios/ViewsScripts/Produtos/Produto/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/produtoEmbarcador", "/Areas/Relatorios/ViewsScripts/Produtos/ProdutoEmbarcador/**/*.js").UseContentRoot();

            #endregion

            #region Localidades

            pipeline.AddJavaScriptBundle("/relatorios/scripts/localidade", "/Areas/Relatorios/ViewsScripts/Localidades/**/*.js").UseContentRoot();

            #endregion

            #region Seguros

            pipeline.AddJavaScriptBundle("/relatorios/scripts/ctesAverbados", "/Areas/Relatorios/ViewsScripts/Seguros/CTesAverbados/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/apolices", "/Areas/Relatorios/ViewsScripts/Seguros/Apolices/**/*.js").UseContentRoot();

            #endregion

            #region Transportadores

            pipeline.AddJavaScriptBundle("/relatorios/scripts/transportadores", "/Areas/Relatorios/ViewsScripts/Transportadores/Transportadores/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/vencimentoCertificados", "/Areas/Relatorios/ViewsScripts/Transportadores/VencimentoCertificado/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/aceiteContrato", "/Areas/Relatorios/ViewsScripts/Transportadores/AceiteContrato/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/configuracoesNFSe", "/Areas/Relatorios/ViewsScripts/Transportadores/ConfiguracoesNFSe/**/*.js").UseContentRoot();

            #endregion

            #region Usuarios

            pipeline.AddJavaScriptBundle("/relatorios/scripts/usuarios", "/Areas/Relatorios/ViewsScripts/Usuarios/Usuario/**/*.js").UseContentRoot();

            #endregion

            #region WMS

            pipeline.AddJavaScriptBundle("/relatorios/scripts/conferenciaVolume", "/Areas/Relatorios/ViewsScripts/WMS/ConferenciaVolume/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/expedicaoVolume", "/Areas/Relatorios/ViewsScripts/WMS/ExpedicaoVolume/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/saldoArmazenamento", "/Areas/Relatorios/ViewsScripts/WMS/SaldoArmazenamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/armazenagem", "/Areas/Relatorios/ViewsScripts/WMS/Armazenagem/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/rastreabilidadeVolumes", "/Areas/Relatorios/ViewsScripts/WMS/RastreabilidadeVolumes/**/*.js").UseContentRoot();

            #endregion

            #region Pagamentos Motoristas

            pipeline.AddJavaScriptBundle("/relatorios/scripts/pagamentoMotoristaTMS", "/Areas/Relatorios/ViewsScripts/PagamentosMotoristas/PagamentoMotoristaTMS/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/pendenciaMotorista", "/Areas/Relatorios/ViewsScripts/PagamentosMotoristas/PendenciaMotorista/**/*.js").UseContentRoot();

            #endregion

            #region Torre Controle

            pipeline.AddJavaScriptBundle("/relatorios/scripts/consultaPorNotaFiscal", "/Areas/Relatorios/ViewsScripts/TorreControle/ConsultaPorNotaFiscal/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/consolidadoEntregas", "/Areas/Relatorios/ViewsScripts/TorreControle/ConsolidadoEntregas/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/Permanencias", "/Areas/Relatorios/ViewsScripts/TorreControle/Permanencias/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/DevolucaoNotasFiscais", "/Areas/Relatorios/ViewsScripts/TorreControle/DevolucaoNotasFiscais/**/*.js").UseContentRoot();

            #endregion

            #region RH

            pipeline.AddJavaScriptBundle("/relatorios/scripts/folhaLancamento", "/Areas/Relatorios/ViewsScripts/RH/FolhaLancamento/**/*.js").UseContentRoot();
            pipeline.AddJavaScriptBundle("/relatorios/scripts/ComissaoFuncionario", "/Areas/Relatorios/ViewsScripts/RH/ComissaoFuncionario/**/*.js").UseContentRoot();

            #endregion
        }
    }
}