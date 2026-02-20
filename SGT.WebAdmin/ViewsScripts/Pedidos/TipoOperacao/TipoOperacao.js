/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/GrupoTipoOperacao.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/TipoRetornoCarga.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../Configuracao/Fatura/Fatura.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressao.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoDiarioBordo.js" />
/// <reference path="../../Enumeradores/EnumTipoObrigacaoUsoTerminal.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumTipoUltimoPontoRoteirizacao.js" />
/// <reference path="../../Enumeradores/EnumEixosSuspenso.js" />
/// <reference path="../../Enumeradores/EnumQuandoProcessarMonitoramento.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumMDFeTipoEmitente.js" />
/// <reference path="../../Enumeradores/EnumTipoCustomEventAppTrizy.js" />
/// <reference path="../../Enumeradores/EnumTipoLiberacaoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCancelamentoCargaDocumento.js" />
/// <reference path="../../Enumeradores/EnumTipoDaEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoEventoSuperApp.js" />
/// <reference path="../../Enumeradores/EnumTipoNotificacaoApp.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoValePedagio.js" />
/// <reference path="../../Enumeradores/EnumTipoParadaEventoSuperApp.js" />
/// <reference path="../../Enumeradores/EnumContatosInformacoesEntregaTrizy.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeVeiculoTipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedadeVeiculoTipoTransportador.js" />
/// <reference path="../../Enumeradores/EnumPesoConsideradoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoDataCalculoParadaNoPrazo.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="MotivoAtendimento.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="ControleEntregaSetor.js" />
/// <reference path="Anexo.js" />
/// <reference path="Vendedores.js" />
/// <reference path="TipoTerceiro.js" />
/// <reference path="CodigoIntegracaoGerenciadoraRisco.js" />

// #region Objetos Globais do Arquivo

var _gridTipoOperacao;
var _tipoOperacao;
var _CRUDTipoOperacao;
var _pesquisaTipoOperacao;
var _tipoOperacaoEmissao;
var _tipoOperacaoFatura;
var _configuracaoLayoutEDI;
var _configuracaoEmissaoCTe;
var _configuracaoFatura;
var _configuracoesTipoOperacaoValorPadrao;
var _gridFilialMotoristaGenerico;
var versaoTrizy = -1;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaTipoOperacao = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), issue: 586 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 50, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), issue: 557 });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Pedidos.TipoOperacao.TipoDePessoa.getFieldDescription(), issue: 306, required: false, eventChange: TipoPessoaChange, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoOperacao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true, maxlength: 500 });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Observacao.getFieldDescription(), issue: 593, maxlength: 400 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 500, issue: 15 });
    this.CodigoIntegracaoGerenciadoraRisco = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CodigoGerenciadoraDeRisco.getFieldDescription(), maxlength: 50 });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Pedidos.TipoOperacao.TipoDePessoa.getFieldDescription(), issue: 306, required: true, eventChange: TipoPessoaChange, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), issue: 58, visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Pessoa.getFieldDescription(), idBtnSearch: guid(), issue: 55, visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    // Tab Geral
    this.TipoOperacaoRedespacho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoOperacaoCargaDeRedespacho.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.GerarRedespachoAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarAutomaticamenteUmaCargaDeRedespachoParaEssaOperacao, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.GerarRedespachoParaOutrasEtapasCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarRedespachoParaOutrasEtapasCarregamento, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.IncluirCargaCanceladaProcessarDT && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.Reentrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteReentrega, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.GerarPedidoAoReceberCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Pedidos.TipoOperacao.GerarPedidoAoReceberIntegracaoDaCarga, visible: ko.observable(true) });

    this.GeraCargaAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoRealizaGeracaoAutomaticaDaCargaAposCriacaoAutorizacaoDoPedid, val: ko.observable(true), def: true, visible: ko.observable(false) });
    this.NaoPermitirGerarComissaoMotorista = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarComissaoAoMotorista, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.OperacaoDeImportacaoExportacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoHabilitaInformarOsDadosDeImportacaoExportacaoNoPedido, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.BloquearEmisssaoComMesmoLocalDeOrigemEDestino = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoDeveBloquearEmissaoQuandoLocalDeOrigemDestinoForemIguais, issue: 607, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeProdutoEmbarcadorPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeQuePedidoTenhaUmProdutoInformado, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeNotaFiscalParaCalcularFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeQueNotaFiscalSejaRecebidaAntesDeCalcularFrete, issue: 607, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TipoOperacaoUtilizaCentroDeCustoPEP = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EsseTipoDeOperacaoUtilizaCentroDeCustoOuElementoPEP, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TipoOperacaoUtilizaContaRazao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EsseTipoDeOperacaoUtilizaContaRazao, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.UtilizarExpedidorComoTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarExpedidorDoPedidoComoTransportadorParaAsCargasDestaOperacao, issue: 608, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NaoExigeVeiculoParaEmissao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEstaOperacaoNaoSeUtilizaVeiculoNaEmissao, issue: 609, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EmitirDocumentosRetroativamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EmitirOsDocumentosRetroativamenteConsiderandoUltimoDiaDoMes, issue: 610, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermiteImportarDocumentosManualmente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarDocumentosManualmenteParaEstaOperacao, issue: 614, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ValidarNotaFiscalPeloDestinatario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ValidarOsXmlsDeNotasFiscaisEnviadosPeloDestintarioNestaOperacao, issue: 615, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NaoGerarTituloGNREAutomatico = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarTituloGNREAutomatico, val: ko.observable(false), def: false });
    this.ExigirValorFreteAvancarCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacxoExigeValorDeFreteNoPedidoNotaFiscalParaAvancarCarga, val: ko.observable(false), def: false });
    this.PermiteGerarPedidoSemDestinatario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteGerarPedidoSemDestinario, val: ko.observable(false), def: false });
    this.NaoIncluirICMSFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoIncluirValorDoICMSNaBaseDeCalculo, val: ko.observable(false), def: false });
    this.ExigirInformarPeso = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarPesoNosSocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarQuantidadeMercadoria = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarQuantidadeNosDocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarValorNota = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarValorDaNotaNosDocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarValorMercadoria = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarValorDaMercadoriaNosDocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarNCM = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarNCMNosDocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarCFOP = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarCFOPNosDocumentos, val: ko.observable(false), def: false });
    this.ExigirInformarM3 = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarMTresNosDocumentos, val: ko.observable(false), def: false });
    this.NaoExigeRotaRoteirizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoExigeUmaRotaDeFretePreCadastradaRoteirizada, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.RoteirizarPorLocalidade = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.RoteirizarPorCidade, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NaoComprarValePedagio = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoFazerCompraDeValePedagioParaEssaOperacao, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigirConfirmacaoDadosTransportadorAvancarCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeConfirmacaoDosDadosDoTransportadorParaAvancarCarga, val: ko.observable(false), def: false });
    this.ObrigarRotaNaMontagemDeCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ObrigarInformarRotaNaMontagemDeCarga, val: ko.observable(false), def: false });
    this.LiberarAutomaticamentePagamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.LiberarAutomaticamentePagamentoDosDocumentoEmitidosParaEsseTipoDeOperacao, def: false, visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado) });
    this.TipoPlanoInfolog = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDePlanoInfolog, maxlength: 100, visible: ko.observable(false) });
    this.NaoGerarCIOT = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarCIOT, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NecessarioAguardarVinculoNotadeRemessaIndustrializador = PropertyEntity({ text: "Necessário aguardar vínculo da nota de remessa industrializador", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ImportarVeiculoMDFEEmbarcador = PropertyEntity({ text: "Importar informações do veículo de acordo com o MDF-e do embarcador", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete = PropertyEntity({ text: "Permite informar Transportador e Veículos na Etapa 1 quando nota fiscal não é recebida antes de calcular o Frete", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete = PropertyEntity({ text: "Permite informar Motoristas somente na etapa 1 quando as notas fiscais não são recebidas antes de calcular o Frete", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.GerarPedidoNoRecebientoNFe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarPedidosAutomaticamenteComOsDadosDaNotaFiscalQuandoNaoTiverDestinatarioInformadoNoPedido, val: ko.observable(false), def: false });

    this.ObrigatorioInformarAnexoSolicitacaoFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarAnexosNaSolicitacaoDeFreteDaCarga, val: ko.observable(false), def: false });

    this.TipoObrigacaoUsoTerminal = PropertyEntity({ val: ko.observable(EnumTipoObrigacaoUsoTerminal.Nenhum), options: EnumTipoObrigacaoUsoTerminal.obterOpcoes(), def: EnumTipoObrigacaoUsoTerminal.Nenhum, text: Localization.Resources.Pedidos.TipoOperacao.TipoObrigacaoDaInformacaoDoTerminal, visible: ko.observable(false) });
    this.UtilizarDataNFeEmissaoDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarCTesUtilizandoDataDeEmissaoDasNotasFiscais, val: ko.observable(false), def: false });

    this.RemarkSped = PropertyEntity({ val: ko.observable(EnumRemarkSped.OutrosServicos), options: EnumRemarkSped.obterOpcoes(), def: EnumRemarkSped.OutrosServicos, text: Localization.Resources.Pedidos.TipoOperacao.RemarkSped, visible: ko.observable(_CONFIGURACAO_TMS.AtivarIntegracaoNFTPEMP) });

    this.TipoCarregamento = PropertyEntity({ val: ko.observable(EnumRetornoCargaTipo.Carregado), options: EnumRetornoCargaTipo.obterOpcoes(), def: EnumRetornoCargaTipo.Carregado, text: Localization.Resources.Pedidos.TipoOperacao.TipoDoCarregamento });
    this.CentroDeResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.CentroDeResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.TipoOperacaoPadraoFerroviario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação Para Modal Ferroviario", required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.RemessaSAP = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Pedidos.TipoOperacao.RemessaSAP, getType: typesKnockout.int, maxlength: 2 });
    this.PermitirQualquerModeloVeicular = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarQualquerModeloDeVeiculoParaEstaOperacao, issue: 617, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeQueVeiculoIgualModeloVeicularDaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigeQueVeiculoSejaDoMesmoModeloSolicitadoNaCarga, issue: 620, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.TipoImpressao = PropertyEntity({ val: ko.observable(EnumTipoImpressao.Nenhum), options: EnumTipoImpressao.obterOpcoes(), def: EnumTipoImpressao.Nenhum, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeImpressao, required: false, visible: ko.observable(false) });
    this.NaoValidarTransportadorImportacaoDocumento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoValidarTransportadoraNaImportacaoDosDocumentos, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.ExigePlacaTracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigeQuePlacaDaTracaoSejaInformadaNaCarga, issue: 621, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EmissaoDocumentosForaDoSistema = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEsseTipoDeOperacaoOsCTesSeraoEmitidosForaDoSistema, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CompraValePedagioDocsEmitidosFora = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ComprarValePedagioMesmoEmitindoForaDoSistema, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.NaoPermiteAgruparCargas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermiteAgruparCargasDesteTipoDeOperaca, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ManterUnicaCargaNoAgrupamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AoAgruparAsCargasManterComoUmaUnicaCargaEmTodoSistema, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.GerarCargaViaMontagemDoTipoPreCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarCargaViaMontagemDoTipoPreCarga, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.UsarRecebedorComoPontoPartidaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UsarRecebedorInformadoComoSendoPontoDePartidaDaCarga, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.NaoGerarFaturamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EstaOperacaoNaoDeveGerarFaturamento, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeRecebedor = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EstaOperacaoExigeQueSejaInformadoRecebedorDiferenteDoDestinatarioParaOsPedidos, issue: 611, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.OperacaoDeRedespacho = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EstaOperacaoUmaOperacaoDeRedespachoExigeUmaCargaCriadaAnteriormente, issue: 613, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.FretePorContadoCliente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NestaOperacaoFretePorContaDoCliente, issue: 613, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.IndicadorGlobalizadoRemetente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarIndicadorGlobalizadoDeRemetenteParaEstaOperacaoNotasFiscaisDeDiferentesEmitentes, issue: 616, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.IndicadorGlobalizadoDestinatario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarIndicadorGlobalizadoDeCTePorDestinatarioParaEstaOperacaoNotasFiscaisComMesmoEmitenteDiferentesDestinatarioDoMesmoEstadoDoEmitente, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NotificarCasoNumeroPedidoForExistente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NotificarCasoNumeroPedidoDoEmbarcadorForExistenteNoTMS, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.IndicadorGlobalizadoDestinatarioNFSe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarIndicadorGlobalizadoDeNFSePorDestinatarioParaEstaOperacaoNotasFiscaisComMesmoEmitenteDiferentesDestinatarios, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.SempreUsarIndicadorGlobalizadoDestinatario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.SempreUtilizarIndicadorGlobalizadoDeCTeParaEstaOperacaoMesmoQueOperacaoNaoSejaNoMesmoEstadoAtencaoPorPadraoSefazIraRejeitarQualquerCTeEmitidoNestaSituacao, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.UtilizarTipoCargaPadrao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoDeCargaPadraoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.UtilizarUmTipoDeCargaPadraoParaEstaOperacao, issue: 604, enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });

    this.EmissaoMDFeManual = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEsseTipoDeOperacaoMDFeSeraEmitidoManualmente, issue: 618, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.UtilizarGrupoTomador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.UtilizarUmGrupoDePessoasParaTomador, enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.PermitirTransbordarNotasDeOutrasCargas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEsseTipoDeOperacaoPossivelTransbordarNotasDeOutrasCargas, issue: 1062, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EmiteCTeFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EmiteCTePelaFilialEmissora, issue: 1063, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeProcImportacaoPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigeQueProcDeImportacaoSejaInformadoParaEmitirCarga, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AverbarDocumentoDaSubcontratacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AverbarDocumentosDaTransportadoraSubcontratada, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CalculaFretePorTabelaFreteFilialEmissora = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEssaOperacaoDeveCalcularViaTabelaDeFreteValorDaFilialEmissora, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.TipoConhecimentoProceda = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoConhecimentoProceda, required: false, maxlength: 1, visible: ko.observable(true) });

    this.HabilitarGestaoPatio = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.HabilitarGestaoDePatio, issue: 1064, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.HabilitarGestaoPatioDestino = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.HabilitarGestaoDePatioDeDestino, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.OperacaoRecolhimentoTroca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.OperacaoDeRecolhimentoTroca, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.ExigePercursoEntreCNPJ = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ParaEsseTipoDeOperacaoNecessarioQueDistanciaDoPercursoSejaCalculadaEntreOsCNPJs, val: ko.observable(false), def: false, issue: 622, visible: ko.observable(false) });

    this.UtilizarTipoOperacaoPreCargaAoGerarCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarEssaOperacaoDePreCargaAoGerarCarga, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExigeConformacaoFreteAntesEmissao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigeQueEtapaDeFreteSejaConfirmadaPeloOperadorAntesDeIniciarEmissao, issue: 1061, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.NaoExigeConformacaoDasNotasEmissao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoExigeQueAsNotasSejamConfirmadasParaAvancar, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.LiberarCargaSemNFesAutomaticamenteAposLiberarFaturamento, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GerarDocumentoPadraoParaCadaPedidoCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarDocumentoPadraoParaCadaPedidoCarga, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ReceberAtualizacaoDeTransporteEmQualquerSituacao, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotado, val: ko.observable(true), def: true, visible: ko.observable(false) });

    this.InformarProdutoPredominanteOperacao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.ProdutoPredominanteOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirUmProdutoPredominantePadraoParaEstaOperacao, issue: 605, required: false, maxlength: 150, enable: ko.observable(false), visible: ko.observable(false) });

    this.ConfiguracaoEmissaoCTe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.UsarConfiguracaoEmissao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.ConfiguracaoLayoutEDI = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.ConfiguracaoFatura = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.UsarConfiguracaoFaturaPorTipoOperacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.UtilizarFatorCubagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarFatorDeCubagem, issue: 1420, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.FatorCubagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FatorDeCubagem, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, visible: ko.observable(false) });
    this.TipoUsoFatorCubagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeUsoDoFatorDeCubagem, val: ko.observable(EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria), options: EnumTupoUsoFatorCubagem.obterOpcoes(), def: EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria, issue: 0, visible: ko.observable(false) });

    this.OperacaoExigeInformarCargaRetorno = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.OperacaoExigeInformarCargaDeRetorno, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GerarCTeComplementarNaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarCTeComplementarNaCargaExigeInformarDocumentoAnterior, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ExclusivaDeSubcontratacaoOuRedespacho = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExclusivaParaGeracaoDeSubcontratacaoOuRedespachoNaoAceitaNFeApenasCTesAnteriores, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.NaoPermitirAlterarValorFreteNaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirAlterarValorDoFreteNaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.UtilizarPaletizacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarPaletizacao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.PesoPorPallet = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PesoPorPallet, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, visible: ko.observable(false) });

    this.ObrigatorioPassagemExpedicao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarCargaPassarPelaEtapaDeExpedicao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.PermiteUtilizarEmContratoFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PodeSerUtilizadaEmContratoDeFrete, issue: 1460, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UtilizarDadosPedidoParaNotasExterior = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarRemetenteDestinatarioDoPedidoParaNotasComParticipantesDoExterior, issue: 1841, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.VincularMotoristaFilaCarregamentoManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.VincularMotoristaDaFilaDeCarregamentoManualmente, def: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizarFilaCarregamento) });
    this.PermitirGerarRedespacho = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirRedespacho, def: false });
    this.PermitirGerarRecorrenciaRedespacho = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirRecorrenciaDeRedespacho, def: false });

    this.ColetaEmProdutorRural = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OperacaoParaColetaEmProdutorRuralNaoGeraraUmaCargaApenasUmRegistroParaFechamentoDeColetaNoProduto, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.RemetenteDoCTeSeraODestinatarioDoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RemetenteDoCTeSeraDestinatarioDoPedido, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.PermitirMultiplosDestinatariosPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirMultiplosDestinatariosNoPedido, val: ko.observable(false), def: false });
    this.PermitirMultiplosRemetentesPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirMultiplosRemetentesNoPedido, val: ko.observable(false), def: false });
    this.UtilizarDeslocamentoPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarDeslocamentoNoPedido, val: ko.observable(false), def: false });
    this.TipoUltimoPontoRoteirizacao = PropertyEntity({ val: ko.observable(EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante), options: EnumTipoUltimoPontoRoteirizacao.obterOpcoes(), def: EnumTipoUltimoPontoRoteirizacao.PontoMaisDistante, text: Localization.Resources.Pedidos.TipoOperacao.UltimoPonto });
    this.EixosSuspenso = PropertyEntity({ val: ko.observable(EnumEixosSuspenso.Volta), options: EnumEixosSuspenso.obterOpcoes(), def: EnumEixosSuspenso.Volta, text: Localization.Resources.Pedidos.TipoOperacao.EixosSuspensos, visible: ko.observable(false) });
    this.BloquearFreteZerado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.BloquearCargaComFreteZerado, visible: ko.observable(false), val: ko.observable(false), def: false });
    this.PermitirTrocaNota = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirTrocaDeNota, val: ko.observable(false), def: false });

    this.OperacaoTrocaNota = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EstaOperacaoUmaOperacaoDeTrocaDeNota, val: ko.observable(false), def: false });
    this.EnviarPesoLiquidoLinkNotas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EnviarPesoLiquidoLinkNotas, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.UtilizarRecebedorApenasComoParticipante = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarRecebedorApenasComoParticipante, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirUtilizarPlacaContrato = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirUtilizarPlacaNoContrato, val: ko.observable(false), def: false });
    this.ConfiguracaoTabelaFretePorPedido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoDaTabelaDeFreteDeveSerUtilizadaPorPedidoNaoPorCarga, val: ko.observable(false), def: false });

    this.NaoGerarControleColetaEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarControleDeEntrega, val: ko.observable(false), def: false });
    this.GerarControleColetaEntregaAposEmissaoDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar Controle Coleta/Entrega após emissão documentos", val: ko.observable(false), visible: ko.observable(!this.NaoGerarControleColetaEntrega.val()), def: false });

    this.GerarControleColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarControleDeColeta, val: ko.observable(false), def: false });
    this.PermiteAdicionarColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteAdicionarColetasCargasQueEstaoEmAndamento, val: ko.observable(false), def: false });
    this.PermiteImpressaoMobile = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteImprimirComprovantesNoMobile, val: ko.observable(false), def: false });
    this.PermiteRetificarMobile = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteRetificarPedidosNoMobile, val: ko.observable(false), def: false });
    this.ExibirCalculadoraMobile = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExibirCalculadoraMobile, val: ko.observable(false), def: false });

    this.NaoPermiteRejeitarEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirRejeitarEntregas, val: ko.observable(false), def: false });

    this.PesoMinimo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PesoMinimo, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(false) });
    this.PesoMaximo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PesoMaximo, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(false) });

    this.GerarComissaoParcialMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarComissaoParcialParaMotorista, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.PercentualComissaoParcialMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ComissaoComissao, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 5 });

    this.AvancarCargaAutomaticaAposMontagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AvançarCargaAutomaticamenteAposGerarSuaMontagem, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.AvancarEtapaFreteAutomaticamente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AvancarEtapaDoFreteTresDeFormaAutomatica, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarTomadorDoPedidoDiferenteDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarSeTomadorDoPedidoEstaDiferenteDoTomadorDaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.ImportarTerminalOrigemComoExpedidor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImportarTerminalDeOrigemComoExpedidor, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.ImportarTerminalDestinoComoRecebedor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImportarTerminalDeDestinoComoRecebedor, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });

    this.NaoEmitirCargaComValorZerado = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEmitirCargaComValorZerado, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.CamposSecundariosObrigatoriosPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CamposSecundariosObrigatoriosNoPedido, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.AtualizarProdutosPorXmlNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtualizarOsProdutosDaNfeAoReceberXmlDaNotaFiscal, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtualizarSaldoDosPedidoProdutosAoReceberXmlDaNotaFiscal, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.DeslocamentoVazio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DeslocamentoVazio, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EncerrarMDFeManualmente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EncerrarMDFeManualmente, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.CargaPropria = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CargaPropriaEmiteApenasMDFe, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });

    this.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CriarNovoPedidoAoVincularUmaNotaFiscalComDiferentesRemetentesDestinatarios, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.TipoDeEmitente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeEmitente, options: EnumMDFeTipoEmitente.obterOpcoesPesquisa(), val: ko.observable(EnumMDFeTipoEmitente.NaoSelecionado), def: EnumMDFeTipoEmitente.NaoSelecionado, visible: ko.observable(true) });
    this.RetornoVazio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RetornoVazio, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirSelecionarNotasCompativeis = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirSelecionarNotasCompativeisNaEtapaDaNFe, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.PermitirTransportadorInformeNotasCompativeis = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirTransportadorInformeNotasCompativeis, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.HabilitarAppTrizy = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.HabilitarAppTrizy, val: ko.observable(false), def: false });

    this.PermitirSelecionarNotasCompativeis.val.subscribe(function (novoValor) {
        _tipoOperacao.PermitirTransportadorInformeNotasCompativeis.visible(novoValor);
    });

    this.TipoOperacaoPadraoParaFretesDentroDoPais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacaoPadraoParaFretesDentroDoPais, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TipoOperacaoPadraoParaFretesForaDoPais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacaoPadraoParaFretesForaDoPais, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });


    this.TipoOperacaoPadraoParaFretesForaDoPais.val.subscribe(function (novoValor) {
        _tipoOperacao.TipoOperacaoPadraoParaFretesDentroDoPais.visible(!novoValor);
    });

    this.TipoOperacaoPadraoParaFretesDentroDoPais.val.subscribe(function (novoValor) {
        _tipoOperacao.TipoOperacaoPadraoParaFretesForaDoPais.visible(!novoValor);
    });

    this.LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento.val.subscribe(function (novoValor) {
        _tipoOperacao.GerarDocumentoPadraoParaCadaPedidoCarga.visible(novoValor);
        _tipoOperacao.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga.visible(novoValor);
    });

    this.UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarNomeDoDestinatarioDaNotaFiscalComoNomeDoDestinatarioDoCTe, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EmissaoAutomaticaCTe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RealizarEmissaoAutomaticaDosCTes, val: ko.observable(false), getType: typesKnockout.bool, def: false, issue: 696, visible: ko.observable(true) });
    this.PermitirAdicionarRemoverPedidosEtapa1 = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAdicionarPedidosNaEtapaUm, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.TomadorCTeSubcontratacaoDeveSerDoCTeOriginal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TomadorDoCTeDeSubcontratacaoDeveSerTomadorDoCTeOriginal, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.FixarValorFreteNegociadoRateioPedidos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FixarValorDoFreteNegociadoDoPedidoNoRateioDoFreteNaoRatearaMaisValorTotalEntreOsPedidos, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.UtilizarValorFreteOriginalSubcontratacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarValorDoFreteDoCTeOriginalParaSubcontratacoes, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirExpedidorRecebedorIgualRemetenteDestinatario = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirEmissoesDeCTeComExpedidorRecebedorIguaisAoRemetenteDestinatario, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.AlterarRemetentePedidoConformeNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AlterarRemetenteDoPedidoConformeNotaFiscalVinculadaNaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RetornarApenasCanhotosDeCTesComTodosCanhotosConfirmadosPagamento, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.RetornarCarregamentoPendenteAposEtapaCTE = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RetornarCarregamentoPendenteAposEtapaCTe, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.NotificarRemetentePorEmailAoSolicitarNotas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NotificarRemetentePorEmailAoSolicitarNotasFiscais, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarMotoristaTeleriscoAoConfirmarTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarMotoristaNaTelerisco, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarMotoristaVeiculoNaBrasilRisk, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarMotoristaVeiculoNaAdagio, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarMotoristaBuonnyAoConfirmarTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarMotoristaVeiculoNaBuonny, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.NaoGerarCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarCanhotos, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.UtilizarMaiorDistanciaPedidoNaMontagemCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarMaiorDistanciaDosPedidosNaMontagemDeCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EmitirNFeRemessa = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EmitirNFeDeRemessaDeMercadoriaAutomaticamente, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.NaoPermitirValorFreteLiquidoZerado = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirValorDeFreteLiquidoZerado, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.UtilizarTipoCargaPedidoCalculoFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarTipoDeCargaDoPedidoParaCalculoDeFreteTabelaComCalculoPorPedido, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermiteDividirPedidoEmCargasDiferentes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteDividirPedidoEmCargasDiferentes, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.AgendamentoGeraApenasPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ApenasGerarPedidosNoAgendamento, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirVeiculoDiferenteMontagemCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirVeiculoDiferenteNaMontagemDeCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.TipoOperacaoAgendamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PedidosComEsseTipoDeOperacaoFazemParteDoAgendamentoDeColeta, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.SolicitarNotasFiscaisAoSalvarDadosTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarAsNotasFiscaisAoSalvarOsDadosDoTransportador, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirValorFreteInformadoPeloEmbarcadorZerado = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirValorDeFreteInformadoPeloEmbarcadorZerado, getType: typesKnockout.bool, val: ko.observable(false) });
    this.SelecionarRetiradaProduto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteSelecionarParaRetiradaDeProduto, getType: typesKnockout.bool, val: ko.observable(false) });
    this.BloquearAlteracaoHorarioCarregamentoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearAlteracaoDeHorarioDeCarregamentoNaCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ImprimirRelatorioRomaneioEtapaImpressaoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImprimirRelatorioDeRomaneioNaEtapaDeImpressaoDeCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoExibirDetalhesDoFretePortalTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoExibirOsDetalhesDoFreteNoPortalDoTransportador, getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermiteAvancarCargaSemDataDePrevisaoDeEntrega, getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarValorFreteNotasFiscais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarValorDoFreteDasNotasFiscais, getType: typesKnockout.bool, val: ko.observable(false) });
    this.InserirDadosContabeisXCampoXTextCTe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InserirDadosContabeisEmXCampoXTextoCTe, getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoOperacaoExibeValorUnitarioDoProduto = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EsseTipoDeOperacaoExibeValorUnitarioDosProdutos, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UtilizarRecebedorPedidoParaSVM = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarRecebedorDoPedidoParaCTeDeSVM, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.UtilizarMoedaEstrangeira = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarMoedaEstrangeira, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.Moeda = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.Moeda, options: EnumMoedaCotacaoBancoCentral.obterOpcoesMoedasEstrangeiras(), val: ko.observable(EnumMoedaCotacaoBancoCentral.DolarVenda), def: EnumMoedaCotacaoBancoCentral.DolarVenda, issue: 0, visible: ko.observable(true) });

    this.PermiteInformarIscaNaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarISCANaCarga, val: ko.observable(false), def: false });
    this.ExigeInformarIscaNaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarISCANaCarga, val: ko.observable(false), def: false, visible: this.PermiteInformarIscaNaCarga.val });

    this.ExpressaoRegularNumeroBookingObservacaoCTe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe, val: ko.observable(false), def: false });
    this.ExpressaoRegularNumeroContainerObservacaoCTe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe, val: ko.observable(false), def: false });

    this.ExpressaoBooking = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExpressaoBooking, maxlength: 2000, visible: this.ExpressaoRegularNumeroBookingObservacaoCTe.val });
    this.ExpressaoContainer = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExpressaoContainer, maxlength: 2000, visible: this.ExpressaoRegularNumeroContainerObservacaoCTe.val });


    this.PermiteReordenarEntregasCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteReordenarEntregasDaCarga, val: ko.observable(false), def: false });
    this.GerarCargaFinalizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.GerarCargaFinalizada, val: ko.observable(false), def: false, visible: true });
    this.NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirEmitirCargaComMesmoNumeroDoOutroDocumento, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirDataInicioViagemAnteriorDataCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirDataDeInicioDeViagemAnteriorDataDeCarregamento, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoAguardarImportacaoDoCTeParaAvancar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoNecessarioQueTransportadorEnvieOsCTesDaCargaParaAvancarSomenteQuandoTransportadorEmiteForaDoEmbarcador, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.TipoImpressaoDiarioBordo = PropertyEntity({ val: ko.observable(EnumTipoImpressaoDiarioBordo.Nenhum), options: EnumTipoImpressaoDiarioBordo.obterOpcoes(), def: EnumTipoImpressaoDiarioBordo.Nenhum, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeImpressaoDiarioBordo, required: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarRelatorioDiarioBordo) });

    this.DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AposEmissaoDoDocumentosDisponibilizarPedidosParaSeparacaoWMS, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AposEmissaoDoDocumentosDisponibilizarPedidosComRecebedorParaSeparacaoWMS, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TransbordoRodoviario = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacaoDeTransbordoRodoviario, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.LogisticaReversa = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.OperacaoDeLogisticaReversa, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.GrupoTipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.GrupoDoTipoDeOperacao, enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.NaoProcessarTrocaAlvoViaMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoProcessarTrocaDeAlvoViaMonitoramento, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirCargaSemAverbacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirCargaSemAverbacao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ExigeChaveVendaAntesConfirmarNotas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigeChaveDeVendaAntesDeConfirmarNotas, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.GerarNovoNumeroCargaNoRedespacho = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarNovoNumeroDeCargaDeRedespacho, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.UtilizarDirecionamentoCustoExtra = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarDirecionamentoCustoExtra, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP || _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.DirecionamentoCustoExtra = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DirecionamentoCustoExtra, val: ko.observable(EnumTipoDirecionamentoCustoExtra.Faturar), options: _CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP ? EnumTipoDirecionamentoCustoExtra.obterOpcoesNftp() : EnumTipoDirecionamentoCustoExtra.obterOpcoes(), def: EnumTipoDirecionamentoCustoExtra.Faturar, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP || _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.ObrigatorioJustificarCustoExtra = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioJustificarCustoExtra, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.UtilizaIntegracaoOKColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizaIntegracaoOKColeta, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.BuscarDocumentosEAverbacaoPelaOSMae = PropertyEntity({ text: "Buscar Documentos e Averbacao Pela OS Mae / OS?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.AcordoFaturamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AcordoFaturamento.getFieldDescription(), val: ko.observable(EnumTipoAcordoFaturamento.NaoInformado), options: _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum ? EnumTipoAcordoFaturamento.obterOpcoes() : EnumTipoAcordoFaturamento.obterOpcoesProjetoNFTP(), def: EnumTipoAcordoFaturamento.NaoInformado, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP || _CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.TipoDocumentoProvedor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InformarTipoDocumentoProvedor.getFieldDescription(), val: ko.observable(EnumTipoDocumentoProvedor.Nenhum), options: EnumTipoDocumentoProvedor.obterOpcoesPesquisa(), def: EnumTipoAcordoFaturamento.Nenhum, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.ImportarRedespachoIntermediario = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirImportacaoDeCTesDeRedespachoIntermediarioNaCarga, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EmitenteImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.FixarEmitenteDoRedespachoIntermediario, idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.ExpedidorImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.FixarExpedidorDoRedespachoIntermediario, idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.RecebedorImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.FixarRecebedorDoRedespachoIntermediario, idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.BloquearDiferencaValorFreteEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.ExigirAutorizacaoParaCargasComValorDoEmbarcadorDiferenteDaTabelaDeFrete, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PercentualBloquearDiferencaValorFreteEmbarcador = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: Localization.Resources.Pedidos.TipoOperacao.PorcentagemDeDiferencaMinima, getType: typesKnockout.decimal, maxlength: 5, visible: ko.observable(true) });
    this.EmitirComplementoDiferencaFreteEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.GerarOcorrenciaAutomaticamenteParaCargasComValorDoEmbarcadorDiferenteDaTabelaDeFrete, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarOcorrenciaSemTabelaFrete = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.GerarOcorrenciaSemTabelaFrete, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoOcorrenciaSemTabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrencia, required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.LiberarDocumentosEmitidosQuandoEntregaForConfirmada = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.LiberarDocumentosEmitidosQuandoEntregaForConfirmada, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DisponibilizarComposicaoRateioCarga = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.DisponibilizarComposicaoRateioCarga, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoOcorrenciaComplementoDiferencaFreteEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrencia, required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.NaoPermitirVincularCTeComplementarEmCarga = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirVincularCTeComplementarEmCargas, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.TipoOcorrenciaCTeEmitidoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrenciaAutomatizacaoSeOcorrencias, required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.UtilizarOutroModeloDocumentoEmissaoMunicipal = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal });
    this.ModeloDocumentoFiscalEmissaoMunicipal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.GerarOutroDocumentoMunicipal, required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.LayoutEmailTipoPropostaTipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.LayoutEmailTipoPropostaTipoOperacao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });
    this.ConsiderarTomadorPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsiderarTomadorPedido, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });

    // Tab Janela Carregamento
    this.UtilizarCorJanelaCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: false });
    this.CorJanelaCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CorParaExibicaoNaJanela, visible: ko.observable(true), enable: ko.observable(false) });
    this.NaoUtilizaJanelaCarregamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoUtilizarJanelaCarregamento, def: false, visible: ko.observable(true) });
    this.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeDataDeRetiradaCTRNNaJanelaDoTransportador, val: ko.observable(false), def: false });
    this.ExigirMaxGrossJanelaCarregamentoTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeMaxGrossNaJanelaDoTransportador, val: ko.observable(false), def: false });
    this.ExigirNumeroContainerJanelaCarregamentoTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeNumeroDoContainerNaJanelaDoTransportador, val: ko.observable(false), def: false });
    this.ExigirTaraContainerJanelaCarregamentoTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoExigeTaraDoContainerNaJanelaDoTransportador, val: ko.observable(false), def: false });
    this.PermitirAdicionarNaJanelaDescarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoPermiteAdicionarNaJanelaDeDescarregamento, val: ko.observable(false), def: false });
    this.BloquearAdicaoNaJanelaDescarregamentoAutomaticamente = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EssaOperacaoBloqueiaAdicaoAutomaticaNaJanelaDeDescarregamento, val: ko.observable(false), def: false });
    this.UsaJanelaCarregamentoPorEscala = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.JanelaDeCarregamentoDeveSerVinculadaUmaEscalaConfigurada, issue: 619, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermiteTransportadorAvancarEtapaEmissao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteTransportadorAvancarEtapaDeEmissaoDaCarga, getType: typesKnockout.bool, val: ko.observable(true) });
    this.PermiteAlterarDataInicioCarregamentoNoControleEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteAlterarDataInicioCarregamentoNoControleDeEntrega, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteAlterarHorarioCarregamentoCargasFaturadas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteAlterarHorarioCarregamentoEmCargasJaFaturadas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AlterarDataInicioCarregamentoAoAtualizarDataDoAgendamentoDeEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAgendarDescargaAposDataEntregaSugerida = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAgendarDescargaAposDataEntregaSugerida, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoExigirQueEntregasSejamAgendadasComCliente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoExigirQueEntregasSejamAgendadasComCliente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAlterarVolumesNaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAlterarQuantidadeDeVolumesNaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossibilitarInicioViagemViaGuarita = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossibilitarInicioDeViagemViaGuarita, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloquearAvancoCargaVolumesZerados = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearAvancoDaCargaComVolumesZerados, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoUtilizarRecebedorDaNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoAlterarRecebedorNaImportacaoDasNotasFiscaisConsiderarSomenteRecebedorInformadoNoPedido, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearLiberacaoCargaComPedidosQuePossuamDatasDeAgendamentoDivergentes, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExigirTermoAceite = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirTermoDeAceite, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteAdicionarAnexosGuarita = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteAdicionarAnexosGuarita, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirVisualizarOrdenarAsZonasDeTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirVisualizarOrdenarZonasDeTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAdicionarObservacaoNaEtapaUmDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteAdicionarObservacaoNaEtapaUmDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirNumeroPedidoNosDetalhesDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirNumeroDoPedidoNosDetalhesDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirAlterarDataChegadaVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAlterarDataDaChegadaDoVeiculo, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarLicencaMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarLicencaDoMotorista, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRejeitarCargaJanelaCarregamentoTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirRejeitarCargaJanelaCarregamentoTransportador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermiteImprimirOrdemColetaNaGuarita = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteImprimirOrdemColetaNaGuarita, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirLiberarCargaComTipoFreteFOBParaTransportadores, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga) });
    this.LayoutImpressaoOrdemColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.LayoutImpressaoOrdemColeta.getFieldDescription(), options: EnumLayoutImpressaoOrdemColeta.obterOpcoes(), val: ko.observable(EnumLayoutImpressaoOrdemColeta.LayoutPadrao), def: EnumLayoutImpressaoOrdemColeta.LayoutPadrao, visible: ko.observable(true) });

    this.TermoAceite = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.OperacaoInsumos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OperacaoParaInsumos, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoGerarMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AtualizarRotaRealizadaDoMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtualizarRotaRealizadaDoMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuandoProcessarMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.QuandoAtualizarRotaRealizadaDoMonitoramento.getFieldDescription(), val: ko.observable(EnumQuandoProcessarMonitoramento.AoIniciarMonitoramento), options: EnumQuandoProcessarMonitoramento.obterOpcoes() });

    this.HabilitarTipoPagamentoValePedagio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.TipoPagamentoValePedagio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoPagamentoValePedagio.getFieldDescription(), val: ko.observable(EnumTipoPagamentoValePedagio.Cartao), options: EnumTipoPagamentoValePedagio.obterOpcoes(), enable: ko.observable(true) });

    this.ManterOrdemAoRoteirizarAgendaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ManterOrdemDaAgendaDeEntregaAoRoteirizar, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ReceberCTesAverbacaoPorWebService = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ReceberaCTesEAverbacaoPorWebService, val: ko.observable(false), def: false });
    this.OperacaoDestinadaCTeComplementar = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Pedidos.TipoOperacao.OperacaoDestinadaCTeComplementar), val: ko.observable(false), def: false });
    this.PermitirInformarRecebedorMontagemCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarRecebedorMontagemCarga, val: ko.observable(false), def: false });
    this.OperacaoTransferenciaContainer = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.OperacaoTransferenciaContainer, val: ko.observable(false), def: false });
    this.NaoPermitirFinalizarEntregaRejeitada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirFinalizarEntregaRejeitada, val: ko.observable(false), def: false });
    this.ValidarLoteProdutoVersusLoteNotaFiscal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ValidarLoteProdutoLoteNotaFiscal, val: ko.observable(false), def: false });
    this.ExecutarCalculoRelevanciaDeCustoNFePorCFOP = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExecutarCalculoRelevanciaDeCustoNFePorCFOP, val: ko.observable(false), def: false });
    this.AguardarRecebimentoProdutoParaProvisionar = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AguardarRecebimentoProdutoParaProvisionar, val: ko.observable(false), def: false });
    this.AlertarAlteracoesPedidoNoFluxoPatio = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AlertarAlteracoesPedidoNoFluxoPatio, val: ko.observable(false), def: false });
    this.AtivoModuloNaoConformidades = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AtivoModuloNaoConformidades, val: ko.observable(false), def: false });
    this.HerdarDadosDeTransporteCargaPrimeiroTrecho = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.HerdarDadosDeTransporteCargaPrimeiroTrecho, val: ko.observable(false), def: false });
    this.ConsiderarKMRecibidoDoEmbarcador = PropertyEntity({ visible: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.KMRecebidoDoEmbarcador, val: ko.observable(false), def: false });
    this.NaoPermitirLiberarSemValePedagio = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.LiberarSemValePedagio, val: ko.observable(false), def: false });
    this.BloquearInclusaoArquivosXMLDeNFeCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.BloquearInclusaoArquivosXMLDeNFeCarga, val: ko.observable(false), def: false });
    this.InformarDadosNotaCte = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.InformarDadosNotaCte, val: ko.observable(false), def: false });
    this.GerarRetornoAutomaticoMomento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarRetornoAutomaticoMomento, val: ko.observable(EnumGerarRetornoAutomaticoMomento.Nenhum), options: EnumGerarRetornoAutomaticoMomento.obterOpcoes(), def: EnumGerarRetornoAutomaticoMomento.Nenhum, visible: ko.observable(false) });
    this.TipoRetornoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoRetornoCarga.getRequiredFieldDescription(), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false) });
    this.DeixarPedidosDisponiveisParaMontegemCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.DeixarPedidosDisponiveisParaMontegemCarga, val: ko.observable(false), def: false });
    this.PrecisaEsperarNotasFilhaParaGerarPagamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PrecisaEsperarNotasFilhaParaGerarPagamento, val: ko.observable(false), def: false });
    this.PrecisaEsperarNotaTransferenciaParaGeraPagamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PrecisaEsperarNotaTransferenciaParaGeraPagamento, val: ko.observable(false), def: false });
    this.NaoPermitirUsoNotasQueEstaoEmOutraCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirUsoNotasQueEstaoEmOutraCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarPesoDasNotasRelevantes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarPesoDasNotasRelevantes, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ExigirTermoAceite.val.subscribe(function (valor) {
        if (valor)
            $("#liTabTermoAceite").show();
        else
            $("#liTabTermoAceite").hide();
    });

    this.OperacaoExigeInformarCargaRetorno.val.subscribe(function (novoValor) {
        if (novoValor)
            _tipoOperacao.GerarRetornoAutomaticoMomento.visible(novoValor);
        else
            _tipoOperacao.GerarRetornoAutomaticoMomento.visible(novoValor);
    });

    this.GerarRetornoAutomaticoMomento.val.subscribe(function (novoValor) {
        let tornarVisivel = novoValor != EnumGerarRetornoAutomaticoMomento.Nenhum;

        _tipoOperacao.TipoRetornoCarga.visible(tornarVisivel);
        _tipoOperacao.TipoRetornoCarga.required(tornarVisivel);
    });

    this.TipoUltimoPontoRoteirizacao.val.subscribe(() => TipoUltimoPontoRoteirizacaoChange());

    // Tab Mobile
    this.MobilePermiteEventos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarABAEventos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermiteChat = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarABAChat, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermiteSAC = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarABAHistoricoAtendimentos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileObrigarAssinaturaProdutor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarAssinaturaProdutor, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirAvaliacaoNaAssintura = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirAvaliacaoNaAssinatura, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteBaixarOsDocumentosDeTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteBaixarOsDocumentosDeTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NecessarioConfirmacaoMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NecessarioConfirmacaoMotorista, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TempoLimiteConfirmacaoMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoLimiteConfirmacaoMotorista, getType: typesKnockout.timeSec, type: types.timeSec, required: ko.observable(false), visible: ko.observable(false) });

    this.MobileForcarPreenchimentoSequencial = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ForcarPreenchimentoSequencialDasAbas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileBloquearRastreamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearRastreamento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermitirVisualizarProgramacaoAntesViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.VisualizarProgramacaoAntesDeIniciarViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileSolicitarJustificativaRegistroForaRaio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarJustificativaQuandoRegistroForFeitoForaDoRaioDoDestino, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileExibirEntregaAntesEtapaTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirControleDeEntregaAntesDaEtapaDeTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirEntregaEtapaEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirEntregaEtapaEmissaoDocumentos, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoListarProdutosColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoListarProdutosColetaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoApresentarDataInicioViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoApresentarDataInicioViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IniciarViagemNoControleDePatioAoIniciarViagemNoApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IniciarViagemNoControleDePatioAoIniciarViagemNoApp, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.MobileExibirEntregaAntesEtapaTransporte.val.subscribe(function (novoValor) {
        _tipoOperacao.ExibirEntregaEtapaEmissaoDocumentos.visible(novoValor);
    });

    // Tab de Entrega
    this.MobilePermiteConfirmarEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteConfirmarEntrega, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.MobilePermiteCanhotoModoManual = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarModoManualDeRecebimentoDeCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermiteFotosEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirRegistroDeFotos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileQuantidadeMinimasFotosEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.QuantidadeMinimasDeFotos.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MobilePermiteConfirmarChegadaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirConfirmarChegada, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermiteEntregaParcial = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarEscolhaDeProdutosRejeicaoEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileControlarTempoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ControlarTempoDeEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileObrigarFotoCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarFotoDoCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileObrigarAssinatura = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarAssinatura, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileObrigarDadosRecebedor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarInformacaoDosDadosDoRecebedorDaEntregaNomeCPF, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SolicitarReconhecimentoFacialDoRecebedor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarReconhecimentoFacialDoRecebedor, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ChecklistDeEntrega.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.IniciarMonitoramentoAutomaticamenteDataCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IniciarMonitoramentoAutomaticamenteDataCarregamento, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    // Tab de Coleta
    this.MobileExibirRelatorio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirRelatorioUltimasCargas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileNaoRetornarColetas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoRetornarAsColetasParaApp, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: this.GerarControleColeta.val });
    this.MobilePermiteConfirmarChegadaColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirConfirmarChegada, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileControlarTempoColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ControlarTempoDaColeta, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileNaoUtilizarProdutosNaColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoUtilizarProdutosNaColeta, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobilePermitirEscanearChavesNfe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirEscanearChavesNFe, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ChecklistDeColeta.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.MobilePermiteFotosColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirRegistroDeFotos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MobileQuantidadeMinimasFotosColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.QuantidadeMinimaDeFotos.getFieldDescription(), val: ko.observable(0), visible: this.MobilePermiteFotosColeta.val, def: 0, getType: typesKnockout.int });
    this.MobileObrigarEscanearChavesNfe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarEscanearChavesNFe, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false), def: false });
    this.MobilePermitirEscanearChavesNfe.val.subscribe(function (novoValor) { _tipoOperacao.MobileObrigarEscanearChavesNfe.visible(novoValor); });
    this.CheckListColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ChecklistDeColeta.getFieldDescription(), idBtnSearch: guid(), required: false });

    //Tab Licença
    this.ValidarLicencaVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarLicencaVeiculo, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarLicencaVeiculoPorCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarLicencaVeiculoPorCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirAvancarEtapaComLicencaInvalida = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAvancarEtapaComLicencaInvalida, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    //Tab Agendamento Coleta
    this.EmailAgendamentoColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ModeloDeEmailParaSerEnviado.getFieldDescription(), getType: typesKnockout.text, val: ko.observable(""), maxlength: 1500 });
    this.TagCNPJFornecedor = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#CNPJFornecedor") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.CNPJFornecedor, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagFornecedor = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#Fornecedor") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Fornecedor, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagTransportador = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#Transportador") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Transportador, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCNPJTransportador = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#CNPJTransportador") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.CNPJTransportador, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#NumeroCarga") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDataColeta = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#DataColeta") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.DataColeta, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroPedido = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#NumeroPedido") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroPedido, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagEmailSolicitante = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailAgendamentoColeta.id, "#EmailSolicitante") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.EmailSolicitante, enable: ko.observable(true), visible: ko.observable(true) });
    this.BloquearMontagemCargaSemNotaFiscal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.BloquearMontagemDeCargaSemNFe, val: ko.observable(false), def: false });
    this.PermiteInformarRecebedorAgendamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteInformarRecebedorNoAgendamento, val: ko.observable(false), def: false });
    this.UtilizarDataSaidaGuaritaComoTerminoCarregamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarDataSaidaGuaritaComoDataDeTerminoDeCarregamento, val: ko.observable(false), def: false });
    this.ObrigarInformarCTePortalFornecedor = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioInformarCTeNoPortalDoFornecedor, val: ko.observable(false), def: false, visible: this.FretePorContadoCliente.val });
    this.PedidoColetaEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PedidoDeColetaEntrega, val: ko.observable(false), def: false });
    this.ExigirNumeroIsisReturnParaAgendarEntrega = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirNumeroIsisReturnParaAgendarEntregas, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.RemoverEtapaAgendamentoDoAgendamentoColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.RemoverEtapaAgendamentoDoAgendamentoColeta, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoObrigarInformarModeloVeicularAgendamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoObrigarInformarModeloVeicularAgendamento, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEmailAoClienteComLinkDeAgendamentoQuandoGerarACarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EnviarEmailAoClienteComLinkDeAgendamentoQuandoGerarACarga, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoObrigarInformarTransportadorAgendamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoObrigarInformarTransportadorAgendamento, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira, val: ko.observable(false), def: false });
    this.ExigirQueCDDestinoSejaInformadoAgendamento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirQueCDDestinoSejaInformadoAgendamento, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ConsiderarDataEntregaComoInicioDoFluxoPatio = PropertyEntity({ getType: typesKnockout.bool, text: "Considerar a Data Entrega como inicio do Fluxo de Pátio", val: ko.observable(false), def: false });

    //Tab Carga
    this.EmailDisponibilidadeCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ModeloDeEmailParaSerEnviado.getFieldDescription(), getType: typesKnockout.text, val: ko.observable(""), maxlength: 1500 });
    this.TagDisponibilidadeCargaCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#CNPJCliente") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.CNPJCliente, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaCliente = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#Cliente") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Cliente, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaCNPJTransportador = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#CNPJTransportador") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.CNPJTransportador, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#NumeroCarga") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroCarga, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaNumeroPedido = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#NumeroPedido") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroPedido, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaDataHoraColeta = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#DataHoraColeta") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.DataHoraColeta, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaDataHoraPrevisaoEntrega = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#DataHoraPrevisaoEntrega") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.DataHoraPrevisaoDeEntrega, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaEmitente = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#Emitente") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.Emitente, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDisponibilidadeCargaCNPJEmitente = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#CNPJEmitente") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.CNPJEmitente, enable: ko.observable(true), visible: ko.observable(true) });
    this.LinkAgendamentoRetira = PropertyEntity({ eventClick: function (e) { InserirTag(e.EmailDisponibilidadeCarga.id, "#LinkAgendamentoRetira") }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.LinkAgendamentoRetira, enable: ko.observable(true), visible: ko.observable(true) });
    this.ExigirVeiculoComRastreador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EsseTipoDeOperacaoExigeVeiculosComRastreador, val: ko.observable(false), def: false });
    this.ObrigatorioVincularContainerCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioVincularUmContainerCargaParaSeguirComFinalizacaoDaCarga, val: ko.observable(false), def: false });
    this.ObrigatorioRealizarConferenciaContainerCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioRealizarConferenciaContainerCarga, val: ko.observable(false), def: false });
    this.ValidarSeCargaPossuiVinculoComPreCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ValidarSeCargaPossuiVinculoComPreCarga, val: ko.observable(false), def: false });
    this.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirInformarAjudantesNaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarAjudantesNaCarga, val: ko.observable(false), def: false });
    this.CargaTipoConsolidacao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.CargaDoTipoConsolidacao, val: ko.observable(false), def: false });
    this.TipoOperacaoMercosul = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacaoMercosul, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.TipoOperacaoInternacional = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeOperacaoInternacional, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CalcularPautaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CalcularPautaICMS, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirAvancarCargaSemRegraICMS = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirAvancarCargaSemRegraDeICMS, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirInformarMotoristaComCNHVencida = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirInformarMotoristaComCNHVencida, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeDeCaixas, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: this.AtualizarProdutosPorXmlNotaFiscal.val });
    this.ExigeInformarIscaNaCargaComValorMaiorQue = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigeInformarIscaNaCargaComValorMaiorQue.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ValorLimiteNaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorLimiteNaCarga.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
    this.ExibirOperadorInsercaoCargaNoPortalTransportador = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExibirOperadorInsercaoCargaNoPortalTransportador, val: ko.observable(false), def: false });
    this.TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoEmHorasParaAlertarPorEmailResponsavelDaFilialQueCargaAindaNaoTeveOsDadosDeTransporteInformados.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(0), def: 0 });
    this.TempoParaRecebimentoDosPacotes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoParaRecebimentoDosPacotes.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(0), def: 0 });
    this.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(0), def: 0 });
    this.QuantidadeDiasValidacaoNFeDataCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.QuantidadeDiasValidacaoNFeDataCarregamento.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(0), def: 0 });
    this.ExibirFiltroDePedidosEtapaNotaFiscal = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExibirFiltroDePedidosEtapaNotaFiscal, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAlterarDataRetornoCDCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAlterarDataRetornoCDCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IgnorarRateioConfiguradoPorto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IgnorarRateioConfiguradoPorto, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ObrigarInformarRICnaColetaDeConteiner = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ObrigarInformarRICnaColetaDeConteiner, val: ko.observable(false), visible: ko.observable(true), def: false });
    this.ExigirCargaRoteirizada = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirCargaRoteirizada, val: ko.observable(false), visible: ko.observable(true), def: false });
    this.CargaBloqueadaParaEdicaoIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.CargaBloqueadaParaEdicaoIntegracao, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirAdicionarNovosPedidosPorNotasAvulsas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAdicionarNovosPedidosPorNotasAvulsas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoConsolidacao = PropertyEntity({ val: ko.observable(EnumTipoConsolidacao.NaoConsolida), options: EnumTipoConsolidacao.obterOpcoes(), def: EnumTipoConsolidacao.NaoConsolida, text: Localization.Resources.Pedidos.TipoOperacao.TipoConsolidacao });
    this.ModalCarga = PropertyEntity({ val: ko.observable(EnumTipoModal.Rodoviario), options: EnumTipoModal.obterOpcoes(), def: EnumTipoModal.Rodoviario, text: Localization.Resources.Pedidos.TipoOperacao.ModalCarga });
    this.ObrigarInformarValePedagio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigarInformarValePedagio, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirRelacionarOutrasCargas = PropertyEntity({ text: "Permitir relacionar com outras cargas", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigeNotaFiscalTenhaTagRetirada = PropertyEntity({ text: "Exige que nota fiscal tenha tag retirada", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VincularPedidoDeAcordoComNumeroOrdem = PropertyEntity({ text: "Vincular pedido de acordo com o número da ordem", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirIntegrarPacotes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirIntegrarPacotesViaMetodoEnviarPacote, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarKMDaRotaFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsiderarKMDaRotaFrete, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoOperacaoCargaEspelho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de operação:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(true) });
    this.GerarCargaEspelhoAoConfirmarEntrega = PropertyEntity({ text: "Gerar carga espelho ao confirmar a entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga = PropertyEntity({ text: 'Gerar carga espelho automaticamente após finalizar a carga', getType: typesKnockout.bool, val: ko.observable(false) });
    this.NecessitaInformarPlacaCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NecessitaInformarPlacaCarregamento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(_CONFIGURACAO_TMS.ExibirListagemNotasFiscais) });
    this.PesoConsideradoNaCarga = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.UtilizarPesoLiquidoNFeParaCTeMDFe ? EnumPesoConsideradoCarga.PesoLiquido : EnumPesoConsideradoCarga.PesoBruto), options: EnumPesoConsideradoCarga.obterOpcoes(), def: (_CONFIGURACAO_TMS.UtilizarPesoLiquidoNFeParaCTeMDFe ? EnumPesoConsideradoCarga.PesoLiquido : EnumPesoConsideradoCarga.PesoBruto), text: "Selecionar qual peso deve ser considerado na carga" });
    this.NaoCriarAprovacaoCargaConfirmarDocumento = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoCriarAprovacaoCargaConfirmarDocumento, val: ko.observable(false), def: false });

    this.TipoRotaCarga = PropertyEntity({ val: ko.observable(EnumTipoRotaCarga.Nenhuma), options: EnumTipoRotaCarga.obterOpcoes(), def: EnumTipoRotaCarga.Nenhuma, text: Localization.Resources.Pedidos.TipoOperacao.TipoRotaCarga });
    this.GerarCargaEspelhoAoConfirmarEntrega.val.subscribe(function (novoValor) {
        const exibir = (novoValor || _tipoOperacao.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga.val());

        _tipoOperacao.TipoOperacaoCargaEspelho.visible(exibir);
        _tipoOperacao.TipoOperacaoCargaEspelho.required(exibir);
    });

    this.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga.val.subscribe(function (novoValor) {
        const exibir = (novoValor || _tipoOperacao.GerarCargaEspelhoAoConfirmarEntrega.val());

        _tipoOperacao.TipoOperacaoCargaEspelho.visible(exibir);
        _tipoOperacao.TipoOperacaoCargaEspelho.required(exibir);
    });

    this.ListaConfiguracaoCargaEstado = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });
    this.AdicionarConfiguracaoCargaEstado = PropertyEntity({ eventClick: adicionarConfiguracaoCargaEstadoModalClick, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarConfiguracaoCargaEstado });
    this.TipoDeCancelamentoDaCarga = PropertyEntity({ val: ko.observable(EnumTipoCancelamentoCargaDocumento.Carga), options: EnumTipoCancelamentoCargaDocumento.obterOpcoes(), def: EnumTipoCancelamentoCargaDocumento.Carga, text: Localization.Resources.Pedidos.TipoOperacao.TipoDeCancelamentoDaCarga.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.PermitirCancelarDocumentosCargaPeloCancelamentoCarga) });
    this.PermiteEmitirCargaDiferentesOrigensParcialmente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteEmitirCargaDiferentesOrigensParcialmente, getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarLacreNosDadosTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InformarLacreNosDadosTransporte, getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoDeEnvioPorSMSDeDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeEnvioPorSMSDeDocumentos, options: EnumTipoDeEnvioPorSMSDeDocumentos.obterOpcoes(), val: ko.observable(EnumTipoDeEnvioPorSMSDeDocumentos.Nenhum), def: EnumTipoDeEnvioPorSMSDeDocumentos.Nenhum, visible: ko.observable(false) });

    this.TipoOperacaoPrecheckin = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoOperacaoPreCheking.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOperacaoPrecheckinTransferencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoOperacaoPreChekingTransferencia.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.TipoConsolidacao.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoConsolidacao.PreCheckIn) {
            _tipoOperacao.TipoOperacaoPrecheckin.visible(true);
            _tipoOperacao.TipoOperacaoPrecheckinTransferencia.visible(true);
        }
        else {
            _tipoOperacao.TipoOperacaoPrecheckin.visible(false);
            _tipoOperacao.TipoOperacaoPrecheckinTransferencia.visible(false);
        }
    });

    this.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica, getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarDistribuidorPorRegiaoNaRegiaoDestino = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarDistribuidorPorRegiaoNaRegiaoDestino, getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarMDFeParaRecebedorDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarMDFeParaRecebedorDaCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarRedespachoAutomaticamenteAposEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarRedespachoAutomaticamenteAposEmissaoDocumentos, getType: typesKnockout.bool, val: ko.observable(false) });
    this.InformarTransportadorSubcontratadoEtapaUm = PropertyEntity({ text: 'Informar transportador subcontratado na etapa 1', getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizarRotaFreteInformadoPedido = PropertyEntity({ text: "Utilizar a rota informada no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaRetornoRejeitarTodasColetas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarCargaRetornoRejeitarTodasColetas, getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoOperacaoCargaRetornoColetasRejeitadas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de operação:", idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(true) });
    this.GerarCargaRetornoRejeitarTodasColetas.val.subscribe(function (novoValor) {
        _tipoOperacao.TipoOperacaoCargaRetornoColetasRejeitadas.visible(novoValor);
        _tipoOperacao.TipoOperacaoCargaRetornoColetasRejeitadas.required(novoValor);
    });

    this.ValidaValorPreCalculoValorFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidaValorPreCalculoValorFrete, getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarConsultaContainerEMP = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarConsultaContainerEMP, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.AtivarIntegracaoContainerEMP) });
    this.ValidarValorMinimoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarValorMinimoCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValorMinimoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorMinimoCarga.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(false), required: ko.observable(false) });
    this.AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento, getType: typesKnockout.bool, val: ko.observable(false) });
    this.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento, getType: typesKnockout.int, val: ko.observable("") });
    this.NaoAvancarEtapaSePlacaEstiverEmMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoAvancarEtapaSePlacaEstiverEmMonitoramento, getType: typesKnockout.bool, val: ko.observable(false) });
    this.RetornarSituacaoAoRemoverPedidos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RetornarSituacaoAoRemoverPedidos, getType: typesKnockout.bool, val: ko.observable(false) });
    this.SituacaoAposRemocaoPedidos = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Nova), options: EnumSituacoesCarga.obterOpcoesEmbarcador(), def: EnumSituacoesCarga.Nova, text: Localization.Resources.Pedidos.TipoOperacao.SituacaoAposRemocaoPedidos, enable: ko.observable(false) });
    this.TipoOperacaoPreCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoOperacaoPreCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermitirSelecionarPreCargaNaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirSelecionarPreCargaNaCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.obrigatorioInformarAliquotaImpostoSuspensoeValor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.obrigatorioInformarAliquotaImpostoSuspensoeValor, getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarCargaSemPlanejamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.LiberarCargaSemPlanejamento, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.IncrementaCodigoPorTipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IncrementaCodigoPorTipoOperacao.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true), val: ko.observable(0), def: 0 });
    this.AdicionaPrefixoCodigoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AdicionaPrefixoCodigoCarga, getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.AvancarCargaQuandoPedidoZeroPacotes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AvancarCargaQuandoPedidoZeroPacotes, getType: typesKnockout.bool, val: ko.observable(false) });
    this.LiberarPedidoComRecebedorParaMontagemCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.LiberarPedidoComRecebedorParaMontagemCarga, getType: typesKnockout.bool, val: ko.observable(false) });
    this.BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta, getType: typesKnockout.bool, val: ko.observable(false) });
    this.HabilitarVinculoMotoristaGenericoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarVinculoMotoristaGenericoNaCarga, getType: typesKnockout.bool, visible: ko.observable(true), val: ko.observable(false) });
    this.AdicionarFilialMotristaGenerico = PropertyEntity({ eventClick: adicionarMotoristaGenericoFilial, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: true });
    this.MotoristaGenerico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Gerais.Geral.Motorista.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.FilialMotoristaGenerico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false) });
    this.FiliaisMotoristasGenericos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.NaoPermitirAvancarCargaComTracaoSemReboque = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirAvancarCargaComTracaoSemReboque, getType: typesKnockout.bool, val: ko.observable(false) });
    this.RoteirizarCargaEtapaNotaFiscal = PropertyEntity({ text: "Roteirizar Carga na Etapa de Nota Fiscal (Usar emissão por pedido agrupado)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega = PropertyEntity({ text: "Gerar Carga de Redespacho vinculando apenas uma Nota Fiscal por Entrega", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.ValidarValorMinimoCarga.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.ValorMinimoCarga.visible(true);
            _tipoOperacao.ValorMinimoCarga.required(true);
        }
        else {
            _tipoOperacao.ValorMinimoCarga.visible(false);
            _tipoOperacao.ValorMinimoCarga.required(false);
        }
    });

    //Tab Transportador
    this.PermitirTransportadorEnviarNotasFiscais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirTransportadorEnviarNotasFiscais, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: this.PermiteImportarDocumentosManualmente.val });
    this.OcultarCargasComEsseTipoOperacaoNoPortalTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OcultarCargasComEsseTipoDeOperacaoNoPortalDoTransportador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirTransportadorSolicitarNotasFiscais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirTransportadorSolicitarNotasFiscais, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermitirEnvioImagemMultiplosCanhotos = PropertyEntity({ text: "Permitir o Envio de Imagens para Múltiplos Canhotos", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirTransportadorAjusteCargaSegundoTrecho = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirQueTransportadorAjusteCargaSegundoTrecho, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermitirRetornarEtapa = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirRetornarEtapa, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.BloquearTransportadorNaoIMOAptoCargasPerigosas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearTransportadorNaoIMOAptoCargasPerigosas, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.AlertarTransportadorNaoIMOCargasPerigosas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AlertarTransportadorNaoIMOCargasPerigosas, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.BloquearVeiculoSemEspelhamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearVeiculoSemEspelhamento, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    this.BloquearVeiculoSemEspelhamentoJanela = PropertyEntity({ text: "Bloquear veículo sem espelhamento - Janela de Carregamento do Transportador", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    // Tab Montagem Carga
    this.PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirQueTransportadorSelecioneEsseTipoDeOperacaoNaMontagemDeCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteInformarPesoCubadoNaMontagemDaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.PermiteInformarPesoCubadoNaMontagemDeCarga, val: ko.observable(false), def: false });
    this.NaoExigeRoteirizacaoMontagemCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoExigirRoteirizacaoParaMontagemDeCarga, val: ko.observable(false), def: false });
    this.NaoDisponibilizarCargaParaIntegracaoERP = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.NaoDisponibilizarCargasDestaOperacaoParaIntegracaoComERP, val: ko.observable(false), def: false });
    this.DisponibilizarPedidosMontagemAoFinalizarTransporte = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.DisponibilizarPedidosMontagemAoFinalizarTransporte, val: ko.observable(false), def: false });
    this.ExibirPedidosMontagemIntegracao = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExibirPedidosMontagemIntegracao, val: ko.observable(false), def: false });
    this.DisponibilizarPedidosMontagemDeterminadosTransportadores = PropertyEntity({ getType: typesKnockout.bool, text: "Disponibilizar pedidos para montagem de determinados Transportadores", val: ko.observable(false), def: false });
    this.OcultarTipoDeOperacaoNaMontagemDaCarga = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.OcultarEsseTipoDeOperacaoNaMontagemDaCarga, val: ko.observable(false), def: false });
    this.ControlarCapacidadePorUnidade = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ControlarCapacidadePorUnidadeNaMontagemDaCarga, val: ko.observable(false), def: false });
    this.ExigirInformarDataPrevisaoInicioViagem = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarDataPrevisaoInicioViagem, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RoteirizarNovamenteAoConfirmarDocumentos = PropertyEntity({ getType: typesKnockout.bool, text: "Roteirizar Carga Novamente Ao confirmar Documentos", val: ko.observable(false), def: false });
    this.AdicionarTransportadoresMontagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Transportador", idBtnSearch: guid(), visible: ko.observable(true), idGrid: guid() });
    this.MontagemComRecebedorNaoGerarCargaComoColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.MontagemComRecebedorNaoGerarCargaComoColeta, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ValidarLicencaVeiculo.val.subscribe(function (novoValor) {
        _tipoOperacao.ValidarLicencaVeiculoPorCarga.visible(novoValor);
        _tipoOperacao.PermitirAvancarEtapaComLicencaInvalida.visible(_tipoOperacao.ValidarLicencaVeiculoPorCarga.val() && _tipoOperacao.ValidarLicencaVeiculo.val());
    });

    // Tab Web Service
    this.PermiteAdicionarPedidoCargaFechada = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAdicionarPedidoCargaFechada, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarCTesPorWebService = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EnviarCTesPorWebService, val: ko.observable(false), def: false });
    this.EnviarSeguroAverbacaoPorWebService = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EnviarSeguroEAverbacaoPorWebService, val: ko.observable(false), def: false });

    //Tab Impressão
    this.NaoNecessarioConfirmarImpressaoDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoNecessarioConfirmarImpressaoDosDocumentos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteRealizarImpressaoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirRealizarImpressaoDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ImprimirCRT = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImprimirCRT, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirTransportadorInformarObservacaoImpressaoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirQueTransportadorInformeObservacaoSerImpressaNaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteBaixarComprovanteColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteBaixarComprovanteColeta, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ImprimirMinuta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImprimirMinuta, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlterarLayoutDaFaturaIncluirTipoServico = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AlterarLayoutDaFaturaIncluirTipoServico, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP) });

    this.UtilizarPlanoViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarPlanoDeViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailPlanoViagemSolicitarNotasCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEmailComPlanoDeViagemAoSolicitarAsNotasDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailPlanoViagemFinalizarCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEmailComPlanoDeViagemAoFinalizarCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEmailPlanoViagemTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarPlanoDeViagemAoTransportador, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarPlanoViagem.val.subscribe(function (novoValor) {
        if (!novoValor) {
            _tipoOperacao.EnviarEmailPlanoViagemSolicitarNotasCarga.val(false);
            _tipoOperacao.EnviarEmailPlanoViagemFinalizarCarga.val(false);
            _tipoOperacao.EnviarEmailPlanoViagemTransportador.val(false);
        }
    });
    this.OcultarQuantidadeValoresOrdemColeta = PropertyEntity({ text: "Ocultar quantidades e valores da ordem de coleta", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Tab Canhoto
    this.PermitirEnviarImagemParaMultiplosCanhotos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirEnvioDeImagemParaMultiplosCanhotos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirInformarDataEntregaParaMultiplosCanhotos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarDataDeEntregaParaMultiplosCanhotosSeparadoDaImagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarCanhotosPendentes = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.NotificarCanhotosPendentes.getFieldDescription() });
    this.NotificarCanhotosRejeitados = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.NotificarCanhotosRejeitado.getFieldDescription() });
    this.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Não permite Upload de Canhotos com CT-e Não Autorizado" });
    this.NotificarCanhotosPendentesDiariamente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.NotificarCanhotosPendentesDiariamente.getFieldDescription() });
    this.NotificarCanhotosRejeitadosDiariamente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pedidos.TipoOperacao.NotificarCanhotosRejeitadosTodosOsDias.getFieldDescription() });
    this.PrazoAposDataEmissaoCanhoto = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, text: Localization.Resources.Pedidos.TipoOperacao.PrazoAposDataEmissaoCanhoto.getFieldDescription() });
    this.DiaSemanaNotificarCanhotosPendentes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DiaSemanaNotificarCanhotosPendentes.getFieldDescription(), val: ko.observable(EnumDiaSemana.Segunda), options: EnumDiaSemana.obterOpcoes() });
    this.NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: "Não gerar canhoto avulso em cargas com ao menos um recebedor" });

    // Tab Controle Entrega
    this.DevolucaoProdutosPorPeso = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DevolucaoDeProdutosSeraPorPeso, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarLoteEntregaAutomaticamente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarLoteDeEntregaAutomaticamenteParaEntregasFinais, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.PermitirTransportadorConfirmarRejeitarEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirQueTransportadorConfirmeOuRejeiteEntregasNoPortal, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NotificarTransportadorAoAgendarEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NotificarTransportadorAoAgendarEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAgendarEntregaSomenteAposInicioViagemCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAgendarEntregaSomenteAposInicioDaViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarApenasDiasUteisNaPrevisaoDeEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsiderarApenasDiasUteisNaPrevisaoDaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CheckListDesembarque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ChecklistDeDesembarque.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.EnviarLinkAcompanhamentoParaClienteEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarLinkDeAcompanhamentoParaClienteDaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoAtualizarDataReprogramadaAposEntradaRaio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoAtualizarDataDeEntregaRecalculadaAposChegadaNoDestino, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RealizarBaixaEntradaNoRaio = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RealizarBaixaAutomaticaPartirDaEntradaNoRaio, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAtualizarEntregasCargasFinalizadas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirAtualizarEntregasCargasFinalizadas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteEnviarNotasComplementaresAposEmissaoDosDocumentosDeTransprote, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirAdicionarPedidoReentregaAposInicioViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteAdicionarPedidoDeReentregaAposInicioDaViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarEntregaPorNotaFiscalCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarUmaEntregaParaCadaNotaFiscalDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigeSenhaConfirmacaoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirQueRecebedorInformeSenhaParaConfirmacaoDaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NumeroTentativaSenhaConfirmacaoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NumeroDeTentativasPermitidas.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false), maxlength: 4 });
    this.EnviarComprovanteEntregaAposFinalizacaoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarComprovanteDeEntregaAoClienteAposFinalizacaoDaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.IniciarViagemPeloStatusViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IniciarViagemPorTransitoRemeteAoQuandoIniciarViagemNaConfiguracaoDoMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.InicioViagemPorCargaGerada = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InicioViagemPorCargaGerada, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LocalDeParqueamentoCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.LocalDeParqueamento, idBtnSearch: guid() });
    this.ExigirConferenciaProdutosAoConfirmarEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirConferenciaProdutosAoConfirmarEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirJustificativaParaEncerramentoManualViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirJustificativaParaEncerramentoManualViagem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarEventoColetaEntregaUnicoParaTodosTrechos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarEventoColetaEntregaUnicoParaTodosTrechos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirInformarNotasFiscaisNoControleEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirInformarNotasFiscaisNoControleEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirInformarNumeroPacotesNaColetaTrizy = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirInformarNumeroPacotesNaColetaTrizy, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.FinalizarControleEntregaAoFinalizarMonitoramentoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FinalizarControleEntregaAoFinalizarMonitoramentoCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RecriarControleDeEntregasAoConfirmarEnvioDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RecriarControleDeEntregasAoConfirmarEnvioDocumentos, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal, val: ko.observable(false), def: false });
    this.NaoFinalizarEntregasPorTrackingMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoFinalizarEntregasPeloTrackingDoMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SobrescreverDataEntradaSaidaAlvo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SobrescreverDataEntradaSaidaAlvo, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoFinalizarColetasPorTrackingMonitoramento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoFinalizarColetasPeloTrackingDoMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarControleEntregaSemRota = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarControleEntregaSemRota, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VincularApenasUmaNotaPorEntrega = PropertyEntity({ getType: typesKnockout.bool, text: "Vincular Apenas uma Nota fiscal por entrega", val: ko.observable(false), visible: ko.observable(true), def: false });

    this.OrdenarColetasPorDataCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OrdenarColetasPorDataCarregamento, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DataRealizacaoDoEvento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataRealizacaoDoEvento, val: ko.observable(EnumRetornoCargaTipo.DataConfirmacao), options: EnumTipoDataCalculoParadaNoPrazo.obterOpcoesDataRealizada(), def: EnumTipoDataCalculoParadaNoPrazo.DataConfirmacao });
    this.DataPrevistaDoEvento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataPrevistaDoEvento, val: ko.observable(EnumRetornoCargaTipo.DataPrevista), options: EnumTipoDataCalculoParadaNoPrazo.obterOpcoesDataPrevista(), def: EnumTipoDataCalculoParadaNoPrazo.DataPrevista });

    this.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao = PropertyEntity({ text: "Considerar Configurações de Previsao da entrega por tipo Operação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DesconsiderarSabadosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Sabados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesconsiderarDomingosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Domingos", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DesconsiderarFeriadosCalculoPrevisao = PropertyEntity({ text: "Desconsiderar Feriados", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ConsiderarJornadaMotorista = PropertyEntity({ text: "Considerar intervalo do Motorista", getType: typesKnockout.bool, val: ko.observable(false) });
    this.HorarioInicialAlmoco = PropertyEntity({ text: "Horário inicial período Almoço:", val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.MinutosIntervalo = PropertyEntity({ text: "Tempo Intervalo Almoço ", val: ko.observable(0), getType: typesKnockout.int });
    this.DesconsiderarHorariosParaPrazoEntrega = PropertyEntity({ text: "Desconsiderar Horários para prazo de entrega na data Prevista", val: ko.observable(false), getType: typesKnockout.bool });

    this.EnviarBoletimViagemAoFinalizarViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarBoletimViagemAoResponderPesquisaDesembarque, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarBoletimViagemAoFinalizarViagemParaRemetente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarBoletimViagemAoFinalizarViagemParaRemetente, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarBoletimViagemAoFinalizarViagemParaTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarBoletimViagemAoFinalizarViagemParaTransportador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.AdicionarSetor, idBtnSearch: guid(), visible: ko.observable(true), idGrid: guid() });

    this.GerarOcorrenciaPedidoEntregueForaPrazo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarUmaOcorrenciaParaCadaPedidoEntregueForaDoPrazo, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TipoOcorrenciaPedidoEntregueForaPrazo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDaOcorrencia.getRequiredFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });

    this.HabilitarCobrancaEstadiaAutomaticaPeloTracking = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarCobrancaDeEstadiaAutomaticaPeloTracking, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsiderarDatasDePrevisaoDoPedidoParaEstadia = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsiderarDatasDePrevisaoInformadasNoPedido, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), required: false });
    this.ModeloCobrancaEstadiaTracking = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ModeloDeCobracaDaEstadia.getFieldDescription(), options: EnumModeloCobrancaEstadiaTracking.ObterOpcoes(), val: ko.observable(EnumModeloCobrancaEstadiaTracking.PorEtapa), def: EnumModeloCobrancaEstadiaTracking.PorEtapa, visible: ko.observable(false) });
    this.TempoMinimoCobrancaEstadia = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoMinimo.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false), maxlength: 4 });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ComponenteDeFreteParaEstadia.getRequiredFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoDaOcorrenciaParaEstadia, enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });

    this.HabilitarCobrancaEstadiaAutomaticaPeloTracking.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.ComponenteFrete.required = true;
            _tipoOperacao.TipoOcorrencia.required = true;
        }
        else {
            _tipoOperacao.ComponenteFrete.required = false;
            _tipoOperacao.TipoOcorrencia.required = false;
        }
    });

    this.ValidarLicencaVeiculoPorCarga.val.subscribe(function (novoValor) {
        _tipoOperacao.PermitirAvancarEtapaComLicencaInvalida.visible(_tipoOperacao.ValidarLicencaVeiculoPorCarga.val() && _tipoOperacao.ValidarLicencaVeiculo.val());
    });

    //Tab Cálculo Frete
    this.CalculoFreteMesclarValorEmbarcadorComTabelaFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CombinarValorDoFreteDoEmbarcadorComValorCalculadoPelaTabelaDeFrete, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CalculoFreteBloquearAjusteConfiguracoesFreteCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearAjusteManualNasConfiguracoesDaEtapaDeFreteDaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CalculoFreteTipoCotacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDeCotacaoDaMoeda, options: EnumTipoCotacaoFreteInternacional.ObterOpcoes(), val: ko.observable(EnumTipoCotacaoFreteInternacional.CotacaoCorrente), def: EnumTipoCotacaoFreteInternacional.CotacaoCorrente, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true) });
    this.CalculoFreteValorMoedaCotacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorDaCotacaodaMoeda.getFieldDescription(), getType: typesKnockout.decimal, val: ko.observable(""), def: "", configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 20, visible: ko.observable(false) });
    this.CalculoFretePermiteInformarQuantidadePaletes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteInformarQuantidadeDePaletesNaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExecutarPreCalculoFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExecutarPreCalculoFrete, AtivoPreCalculo: ko.observable(false), getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.RatearValorFreteEntrePedidosAposReceberDocumentos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RatearValorFreteEntrePedidosAposReceberDocumentos, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.CalcularFretePeloBIDPedidoOrigem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CalcularFretePeloBIDPedidoOrigem, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.InformarValorFreteTerceiroManualmente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InformarValorFreteTerceiroManualmente, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.PermiteEscolherDestinacaoDoComplementoDeFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteEscolherDestinacaoDoComplementoDeFrete, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.CalculoFreteMesclarComponentesManuaisPedidoComTabelaFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CombinarOsComponentesInformadosManualmenteNoPedidoComOsComponentesCalculadosPelaTabelaDeFreteApenasParaCalculoPorPedido, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarContratoFreteCliente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarContratoFreteCliente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RatearValorFreteInformadoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RatearValorFreteInformadoEmbarcador, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizarCoberturaDeCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarCoberturaDeCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValorMaximoCalculoFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorMaximoCalculoFrete.getFieldDescription(), getType: typesKnockout.decimal, val: ko.observable("") });

    //Tab Emissão Documento
    this.EmissaoDocumentoFinalizarCargaAutomaticamente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FinalizarCargaAutomaticamenteQuandoNaoExistirMDFe, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.EmissaoDocumentoUtilizarExpedidorRecebedorPedidoSubcontratacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarExpedidorRecebedorDaCargaNoCTeDeSubcontratacaoRedespacho, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EmissaoDocumentoNaoPermitirAcessarDocumentosAntesCargaEmTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirAcessarOsDocumentosAntesDaCargaEstarEmTransporte, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EmissaoDocumentoRatearPesoModeloVeicularEntreCTes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RatearPesoDoModeloVeicularDaCargaEntreOsDocumentosEmitidosSeraExibidoNasUnidadesDeMedidaDoDocumento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EmissaoDocumentoObrigatorioAprovarCtesImportados = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioAprovarCtesImportados, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SempreEmitirSubcontratacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SempreEmitirSubcontratacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoPermitirEmissaoComMesmaOrigemEDestino = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirEmissaoComMesmaOrigemEDestino, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarRelevanciaNotasPrechekin = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarRelevanciaNotasPrechekin, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EmitirDocumentoSempreOrigemDestinoPedido = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EmitirDocumentoSemprePelaOrigemDestinoPedido, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.GerarCTeSimplificadoQuandoCompativel = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarCTeSimplificadoQuandoCompativel, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: true });
    this.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.EmissaoDocumentoDescricaoUnidadeMedidaPesoModeloVeicularRateado = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DescricaoDaUnidadeDeMedidaDoPesoRateado.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 20 });

    this.PermiteEmitirCargaSemAverbacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermiteEmitirCargaSemAverbacao, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), checked: ko.observable(false) });

    this.PermiteEmitirCargaSemAverbacao.val.subscribe(function (novoValor) {
        if (novoValor)
            _CONFIGURACAO_TMS.PermiteEmitirCargaSemAverbacao = false;
    });

    this.UtilizarXCampoSomenteNoRedespacho = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarXCampoXTextoSomenteNoRedespacho, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DocumentoXCampo = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.XCampo.getFieldDescription(), maxlength: 20 });
    this.DocumentoXTexto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.XTexto.getFieldDescription(), maxlength: 160 });

    this.ClassificacaoNFeRemessaVenda = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ClassificacaoNFeRemessaVenda, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EmitirCTENotaRemessa = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EmitirCteComNotaRemessa, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: true });
    this.EnviarParaObservacaoCTeNFeRemessa = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarParaObservacaoCTeNFeRemessa, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(false) });
    this.EnviarParaObservacaoCTeNFeVenda = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarParaObservacaoCTeNFeVenda, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(false) });

    this.ClassificacaoNFeRemessaVenda.val.subscribe(function (novoValor) {
        _tipoOperacao.EnviarParaObservacaoCTeNFeRemessa.visible(novoValor);
        _tipoOperacao.EnviarParaObservacaoCTeNFeRemessa.enable(novoValor);
        _tipoOperacao.EnviarParaObservacaoCTeNFeVenda.visible(novoValor);
        _tipoOperacao.EnviarParaObservacaoCTeNFeVenda.enable(novoValor);
    });
    this.EnviarParaObservacaoCTeNFeRemessa.val.subscribe(function (novoValor) {
        _tipoOperacao.EnviarParaObservacaoCTeNFeVenda.enable(!novoValor);
    });
    this.EnviarParaObservacaoCTeNFeVenda.val.subscribe(function (novoValor) {
        _tipoOperacao.EnviarParaObservacaoCTeNFeRemessa.enable(!novoValor);
    });
    this.AverbarContainerComAverbacaoCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AverbarContainerComAverbacaoCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ValorContainerAverbacao = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.ValorContainerAverbacao.getFieldDescription(), required: false, maxlength: 12, cssClass: ko.observable("col col-3"), visible: ko.observable(false) });

    this.AverbarContainerComAverbacaoCarga.val.subscribe(function (novoValor) {
        _tipoOperacao.ValorContainerAverbacao.visible(novoValor);
    });

    //Tab Integração
    this.PossuiIntegracaoTrafegus = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiIntegracaoComTrafegusGSat, getType: typesKnockout.bool, val: ko.observable(false) });
    this.PermiteConsultarPorPacotesLoggi = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermiteConsultarPorPacotesLoggi, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ValidarSomenteVeiculoMotoristaOpentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValidarSomenteVeiculoMotoristaOpentech, getType: typesKnockout.bool, val: ko.observable(false) });
    this.DefinirParaNaoMonitorarRetornoIntegracaoBounny = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DefinirParaNaoMonitorarRetornoIntegracaoBounny, getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoIntegrarOpentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarIntegraçaoOpentech, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.IntegrarPedidosNaIntegracaoOpentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarOpentechUsandoPedidos, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.EnviarApenasPrimeiroPedidoNaOpentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarApenasPrimeiroPedidoNaOpentech, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.EnviarInformacoesTotaisDaCargaNaOpentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarInformacoesTotaisDaCargaNaOpentech, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValorMinimoMercadoriaOpenTech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ValorMinimoDaMercadoriaParaIntegrarNaOpentech.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(false) });
    this.RealizarIntegracaoComMicDta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RealizarIntegraçaoComMICDTA, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.IntegrarMICDTAComSiscomex = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IntegrarMICDTAComSiscomex, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false), enable: ko.observable(false) });
    this.PermitirConsultaDeValoresPedagioSemParar = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirConsultaDeValoresParaValePedagio, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ConsultaDeValoresPedagioAdicionarComponenteFrete = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsultaDeValoresPedagioAdicionarComponenteFrete, val: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? true : false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.IntegracaoUtilizarTipoIntegracaoGrupoPessoas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizarTambemTipoDeIntegracaoDoGrupoDePessoasParaDefinirAsIntegracoesDaCargas, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: true });
    this.AtivarRegraCancelamentoDosPedidosMichelin = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarRegraCancelamentoDosPedidosMichelin, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.HorasParaCalculoCancelamento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HorasParaCalculoCancelamento, val: ko.observable(0), getType: typesKnockout.int, visible: ko.observable(false) });
    this.ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.GerarIntegracaoKlios = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.GerarIntegracaoKlios, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.EnviarTagsIntegracaoMarfrigComTomadorServico = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EnviarTagsIntegracaoMarfrigComTomadorServico, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.ConsultarTaxasKMM = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConsultaTaxasKMM, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TiposTerceiros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.Grid = PropertyEntity({ type: types.local });
    this.Terceiro = PropertyEntity({ type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarTipoTerceiro, idBtnSearch: guid() });
    this.NaoGerarIntegracaoRetornoConfirmacaoColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarIntegracaoRetornoConfirmacaoColeta, getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoIntegrarEtapa1Opentech = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoIntegrarEtapa1Opentech, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    this.PermitirConsultaDeValoresPedagioSemParar.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.ConsultaDeValoresPedagioAdicionarComponenteFrete.visible(true);
        }
        else {
            _tipoOperacao.ConsultaDeValoresPedagioAdicionarComponenteFrete.visible(false);
        }
    });

    this.ConsultarTaxasKMM.val.subscribe(function (novoValor) {
        var element = document.getElementById('knockoutTiposTerceiro');
        if (novoValor) {
            element.style.display = 'block';
        } else {
            element.style.display = 'none';
        }
    });

    this.RealizarIntegracaoComMicDta.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.IntegrarMICDTAComSiscomex.enable(true);
        } else {
            _tipoOperacao.IntegrarMICDTAComSiscomex.enable(false);
            _tipoOperacao.IntegrarMICDTAComSiscomex.val(false);
        }
    });

    this.AtivarRegraCancelamentoDosPedidosMichelin.val.subscribe((ativo) => {
        _tipoOperacao.HorasParaCalculoCancelamento.visible(ativo);
    });

    this.OperacaoDeRedespacho.val.subscribe((val) => {
        _tipoOperacao.ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho.visible(val);
    });

    //Tab Free Time
    this.TipoFreeTime = PropertyEntity({ val: ko.observable(1), options: EnumTipoFreeTime.obterOpcoes(), def: 1, text: Localization.Resources.Pedidos.TipoOperacao.TipoDoFreeTime, required: true, visible: ko.observable(true) });
    this.TempoColetas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoTotalNasColetasMinutos, val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TempoFronteiras = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoTotalNasFronteirasMinutos, val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TempoEntregas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoTotalNasEntregasMinutos, val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TempoTotalViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TempoTotalNaViagemMinutos, val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.UtilizarFatorCubagem.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.FatorCubagem.visible(true);
            _tipoOperacao.TipoUsoFatorCubagem.visible(true);
        } else {
            _tipoOperacao.FatorCubagem.visible(false);
            _tipoOperacao.TipoUsoFatorCubagem.visible(false);
        }
    });

    this.UtilizarPaletizacao.val.subscribe(function (novoValor) {
        if (novoValor)
            _tipoOperacao.PesoPorPallet.visible(true);
        else
            _tipoOperacao.PesoPorPallet.visible(false);
    });

    this.EmiteCTeFilialEmissora.val.subscribe(function (novoValor) {
        if (novoValor && !_CONFIGURACAO_TMS.CalcularFreteFilialEmissoraPorTabelaDeFrete)
            _tipoOperacao.CalculaFretePorTabelaFreteFilialEmissora.visible(true);
        else
            _tipoOperacao.CalculaFretePorTabelaFreteFilialEmissora.visible(false);
    });

    this.PermiteImportarDocumentosManualmente.val.subscribe(function (novoValor) {
        if (novoValor && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            _tipoOperacao.PermitirSelecionarNotasCompativeis.visible(true);
        else
            _tipoOperacao.PermitirSelecionarNotasCompativeis.visible(false);
    });

    //Tab Produtos Padrões
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.AdicionarProdutos, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Produtos = PropertyEntity({ type: types.local });

    //Tab Ocorrência
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.AdicionarOcorrencia, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ocorrencias = PropertyEntity({ type: types.local });
    this.PrazoSolicitacaoOcorrencia = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pedidos.TipoOperacao.PrazoSolicitacaoOcorrencia.getFieldDescription(), val: ko.observable(""), def: "", configInt: { precision: 0, allowZero: false, allowNegative: false } });

    //Tab Grupo de Tomadores Bloqueados
    this.GrupoPessoasTomadoresBloqueados = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.AdicionarGrupoDePessoas, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoTomadoresBloqueados = PropertyEntity({ type: types.local });

    //Tab Paletes
    this.NaoGerarControlePaletes = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoGerarControleDePaletes, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Tab Documentos para Emissão
    this.NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ImportarCTeSempreComoSubcontratacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ImportarCTeSempreComoSubcontratacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiNotaOrdemVenda = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PossuiNotaOrdemVenda, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UtilizaNotaVendaObjetoCTE = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UtilizaNotaVendaObjetoCTE, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoUtilizaNotaVendaObjetoCTE = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoUtilizaNotaVendaObjetoCTE, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MinutosAvancarParaEmissaoseInformadosDadosTransporte = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.MinutosAvancarParaEmissaoseInformadosDadosTransporte, getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: true });
    this.DesconsiderarNotaPalletEmissaoCTE = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DesconsiderarNotaPalletEmissaoCTE, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.AtualizarProdutosPorXmlNotaFiscal.val.subscribe(function (novoValor) {
        _tipoOperacao.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal.visible(novoValor);
        if (!novoValor)
            _tipoOperacao.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal.val(false);
    });

    this.CalculoFreteTipoCotacao.val.subscribe(function (novoValor) {
        _tipoOperacao.CalculoFreteValorMoedaCotacao.visible(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true && (novoValor == null || novoValor != EnumTipoCotacaoFreteInternacional.CotacaoCorrente));
    });

    //Tab Atendimento
    this.NaoPermitirGerarAtendimento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirGerarAtendimentoParaEsteTipoDeOperacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirSelecionarApenasAlgunsMotivosAtendimento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.PermitirSelecionarApenasAlgunsMotivosParaLancamentoDeAtendimento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CancelarAutomaticamenteAtendimentoNfeEntregue = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CancelarAutomaticamenteAtendimentoNfeEntregue, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FinalizarAutomaticamenteAtendimentoNfeEntregue = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FinalizarAutomaticamenteAtendimentoNfeEntregue, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoValidarRetornoGeradoParaFinalizacaoAtendimento = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoValidarRetornoGeradoParaFinalizacaoAtendimento, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AdicionarMotivosAtendimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.AdicionarMotivoAtendimento, idBtnSearch: guid(), visible: ko.observable(true) });
    this.AdicionarTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivosAtendimento = PropertyEntity({ type: types.local });
    this.TransportadoresMotivoAtendimento = PropertyEntity({ type: types.local });

    //Tab Comprovantes
    this.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ExigirComprovantesLiberacaoPagamentoContratoFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.Comprovantes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Mapeamento tabTiposCargas
    this.TiposCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Tab TipoCargaEmissao
    this.TiposCargasEmissao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ExigeNotaFiscalParaCalcularFrete.val.subscribe(function (valor) {
        let visible = _tipoOperacao.ExecutarPreCalculoFrete.AtivoPreCalculo() && valor;
        _tipoOperacao.ExecutarPreCalculoFrete.visible(visible);
    });

    //Tab Pagamentos
    this.TipoLiberacaoPagamento = PropertyEntity({ val: ko.observable(EnumTipoLiberacaoPagamento.Nenhum), options: EnumTipoLiberacaoPagamento.obterOpcoes(), def: EnumTipoLiberacaoPagamento.Nenhum, text: Localization.Resources.Pedidos.TipoOperacao.LiberarDocumentosPagamentoQuando.getFieldDescription() });

    //Tab Intercab
    this.Tomador = PropertyEntity({ val: ko.observable(EnumTipoTomaodorCabotagem.Todos), options: EnumTipoTomaodorCabotagem.obterOpcoes(), def: EnumTipoTomaodorCabotagem.Todos, text: Localization.Resources.Gerais.Geral.Tomador, required: false, visible: ko.observable(true) });
    this.ModalProposta = PropertyEntity({ val: ko.observable(EnumModalPropostaCabotagem.Todos), options: EnumModalPropostaCabotagem.obterOpcoes(), def: EnumModalPropostaCabotagem.Todos, text: Localization.Resources.Gerais.Geral.ModalProposta, required: false, visible: ko.observable(true) });
    this.TipoProposta = PropertyEntity({ val: ko.observable(EnumTipoPropostaCabotagem.Todos), options: EnumTipoPropostaCabotagem.obterOpcoes(), def: EnumTipoPropostaCabotagem.Todos, text: Localization.Resources.Gerais.Geral.TipoProposta, required: false, visible: ko.observable(true) });

    //Tab EMP
    this.AtivarIntegracaoComSIL = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.AtivarIntegracaoComSIL, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    //Tab App Trizy (Integrações Carga)
    this.EnviarEstouIndoColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEstouIndoColeta, getType: typesKnockout.bool, val: ko.observable(true), def: true, visible: ko.observable(true) });
    this.EnviarEstouIndoEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEstouIndoEntrega, getType: typesKnockout.bool, val: ko.observable(true), def: false, visible: ko.observable(true) });
    this.SolicitarComprovanteColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarComprovanteColetaEntrega, getType: typesKnockout.bool, val: ko.observable(true), def: true, visible: ko.observable(true) });
    this.EnviarInicioViagemColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarIniciarViagemColetaPreTrip, getType: typesKnockout.bool, val: ko.observable(true), def: false, visible: ko.observable(true) });
    this.EnviarInicioViagemEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarIniciarViagemEntregaPreTrip, getType: typesKnockout.bool, val: ko.observable(true), def: false, visible: ko.observable(true) });
    this.EnviarEventoIniciarViagemComoOpcional = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEventoIniciarViagemComoOpcional, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarEventosChegadaEConfirmacaoEntregaOpcionais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEventosChegadaEConfirmacaoEntregaOpcionais, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarQuantidadeDeFardos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarQuantidadeDeFardosTagInformacoesAdicionais, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarChat = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarChat, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.NaoEnviarDocumentosFiscais = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEnviarDocumentosFiscais, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEnviarTagValidacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEnviarTagValidacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarObservacoesDoClienteComoInformacoesAdicionaisNasColetasEEntregas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarObservacoesDoClienteComoInformacoesAdicionaisNasColetasEEntregas, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SolicitarFotoComoEvidenciaObrigatoria = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarFotoComoEvidenciaObrigatoria, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.SolicitarFotoComoEvidenciaOpcional = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarFotoComoEvidenciaOpcional, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DataEsperadaParaColetas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataEsperadaParaColetas, val: ko.observable(""), def: ko.observable(""), options: EnumDataEsperadaColetaEntregaTrizy.obterOpcoes(), visible: ko.observable(true) });
    this.DataEsperadaParaEntregas = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DataEsperadaParaEntregas, val: ko.observable(""), def: ko.observable(""), options: EnumDataEsperadaColetaEntregaTrizy.obterOpcoes(), visible: ko.observable(true) });
    this.EnviarInformacoesAdicionaisEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarInformacoesAdicionaisEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.TituloInformacaoAdicional = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TituloInformacaoAdicional, getType: typesKnockout.bool, val: ko.observable(Localization.Resources.Pedidos.TipoOperacao.InformacoesAdicionaisEntrega), def: ko.observable(Localization.Resources.Pedidos.TipoOperacao.InformacoesAdicionaisEntrega) });
    this.InformacoesAdicionaisEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.InformacoesAdicionaisEntrega, getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumInformacoesAdicionaisEntregaTrizy.obterOpcoesSemTelefone(), def: [] });
    this.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.SolicitarDocumentoNaConfirmacaoDeColetaEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarDocumentoNaConfirmacaoDeColetaEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.IdentificarNotaDeMercadoriaENotaDePallet = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.IdentificarNotaDeMercadoriaENotaDePallet, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarContatoInformacoesEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarContatoInformacoesEntrega, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.ContatosInformacoesEntrega = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ContatosInformacoesEntrega, getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumContatosInformacoesEntregaTrizy.obterOpcoes(), def: [] });
    this.ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarMascaraFixaParaoCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarMascaraFixaParaoCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarMascaraDinamicaParaoCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarMascaraDinamicaParaoCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoPermitirVincularFotosDaGaleriaParaCanhotos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirVincularFotosDaGaleriaParaCanhotos, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NecessarioFinalizarOrigem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NecessarioFinalizarOrigem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitarDevolucao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarDevolucao, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.HabilitarDevolucaoParcial = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitarDevolucaoParcial, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.NaoEnviarPolilinha = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEnviarPolilinha, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.SolicitarFotoComoEvidenciaObrigatoria.val.subscribe(function (value) {
        if (value)
            _tipoOperacao.SolicitarFotoComoEvidenciaOpcional.val(!value);
    })
    this.SolicitarFotoComoEvidenciaOpcional.val.subscribe(function (value) {
        if (value)
            _tipoOperacao.SolicitarFotoComoEvidenciaObrigatoria.val(!value);
    })
    this.EnviarMascaraFixaParaoCanhoto.val.subscribe(function (value) {
        if (value)
            _tipoOperacao.EnviarMascaraDinamicaParaoCanhoto.val(!value);
    })
    this.EnviarMascaraDinamicaParaoCanhoto.val.subscribe(function (value) {
        if (value)
            _tipoOperacao.EnviarMascaraFixaParaoCanhoto.val(!value);
    })

    //Tab App Trizy (Integrações Pré-trip)
    this.EnviarIniciarViagemPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarIniciarViagemPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEstouIndoColetaPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEstouIndoColetaPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEstouIndoEntregaPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEstouIndoEntregaPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarIniciarViagemColetaPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarIniciarViagemColetaPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarIniciarViagemEntregaPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarIniciarViagemEntregaPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarEventoIniciarViagemComoOpcionalPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarEventoIniciarViagemComoOpcional, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarPreTripJuntoAoNumeroCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarPreTripJuntoAoNumeroCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarMensagemAlertaPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarMensagemAlertaPreTrip, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarChegueiParaCarregar = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarChegueiParaCarregar, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarChegueiParaDescarregar = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarChegueiParaDescarregar, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarChegueiParaCarregarPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarChegueiParaCarregar, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.EnviarChegueiParaDescarregarPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarChegueiParaDescarregar, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoFinalizarPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoTrocarPreTripViagemNormal, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigirEnvioFotosDasNotasNaOrigemPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigirEnvioFotosDasNotasNaOrigem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SolicitarComprovanteEntregaSemOCRPreTrip = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarComprovateEntregaSemOCR, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SolicitarDataeHoraDoCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.SolicitarDataeHoraDoCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEnviarEventosNaOrigem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.NaoEnviarEventosNaOrigem, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VincularDataEHoraSolicitadaNoCanhoto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.VincularDataEHoraSolicitadaNoCanhoto, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.EnviarDadosEmpresaGR = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarDadosEmpresaGR, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CNPJEmpresaGR = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.CNPJEmpresaGR, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.DescricaoEmpresaGR = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DescricaoEmpresaGR, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });

    //Tab App Trizy (Eventos)
    this.GridEventosSuperApp = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), list: new Array() });
    this.CodigoEventoSuperApp = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.TipoEventoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoEventoSuperApp, val: ko.observable(""), def: ko.observable(""), options: EnumTipoEventoSuperApp.obterOpcoesCadastroEventos(), visible: ko.observable(true) });
    this.TituloEventoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TituloEventoSuperApp, val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.ObrigatorioEventoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ObrigatorioEventoSuperApp, getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.OrdemEventoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.OrdemEventoSuperApp, getType: typesKnockout.int, val: ko.observable(), def: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.TipoEventoCustomizadoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoEventoCustomizadoSuperApp, val: ko.observable(""), def: ko.observable(""), options: EnumTipoCustomEventAppTrizy.opcoesCadastroEventos(), visible: ko.observable(true) });
    this.TipoParadaEventoSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoParadaEventoSuperApp, val: ko.observable(""), def: ko.observable(""), options: EnumTipoParadaEventoSuperApp.opcoesCadastroEventos(), visible: ko.observable(true) });
    this.ChecklistEventoSuperApp = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: ko.observable(""), text: Localization.Resources.Pedidos.TipoOperacao.ChecklistEventoSuperApp, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.AdicionarCadastroEventoSuperApp = PropertyEntity({ eventClick: adicionarListaCadastroEventoSuperAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarCadastroEventoSuperApp = PropertyEntity({ eventClick: atualizarListaCadastroEventoSuperAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.CancelarCadastroEventoSuperApp = PropertyEntity({ eventClick: limparCamposCadastroEventoSuperAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.VerCadEvento = PropertyEntity({ val: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarEventoModal = PropertyEntity({ eventClick: adicionarEventoModalClick, type: types.event, text: "Adicionar Evento" });
    this.AvisoChecklistDesabilitado = PropertyEntity({ text: ko.observable(`O cadastro de evidências está liberado apenas para evento do tipo ${EnumTipoEventoSuperApp.obterDescricao(EnumTipoEventoSuperApp.EvidenciasDaEntrega)}`), visible: ko.observable(false) });
    this.TipoEventoSuperApp.val.subscribe(function (val) {
        let selecionouEvidencia = val == EnumTipoEventoSuperApp.EvidenciasDaEntrega;
        LimparCampo(_tipoOperacao.ChecklistEventoSuperApp);
        _tipoOperacao.ChecklistEventoSuperApp.enable(selecionouEvidencia);
        _tipoOperacao.AvisoChecklistDesabilitado.visible(!selecionouEvidencia);
    });

    //Tab App Trizy (Notificações)
    this.GridNotificacoesAppTrizy = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), list: new Array() });
    this.CodigoNotificacaoAppTrizy = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.TipoNotificacaoAppTrizy = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TipoDaNotificacao, val: ko.observable(EnumTipoNotificacaoApp.Todas), def: ko.observable(""), options: EnumTipoNotificacaoApp.opcoesCadastroNotificacoes(), visible: ko.observable(true) });
    this.TituloNotificacaoAppTrizy = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TituloDaNotificacao, val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.MensagemNotificacaoAppTrizy = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.MensagemDaNotificacao, enable: ko.observable(true), maxlength: 1000, val: ko.observable(""), def: ko.observable("") });
    this.AdicionarCadastroNotificacaoApp = PropertyEntity({ eventClick: adicionarListaCadastroNotificacaoAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarCadastroNotificacaoApp = PropertyEntity({ eventClick: atualizarListaCadastroNotificacaoAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.CancelarCadastroNotificacaoApp = PropertyEntity({ eventClick: limparCamposCadastroNotificacaoAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.TagNumeroRecibo = PropertyEntity({ eventClick: function (e) { InserirTag(this.MensagemNotificacaoAppTrizy.id, "#NumeroRecibo"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroDoRecibo, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroNFe = PropertyEntity({ eventClick: function (e) { InserirTag(this.MensagemNotificacaoAppTrizy.id, "#NumeroNFe"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.NumeroDaNFe, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDescricaoMotivo = PropertyEntity({ eventClick: function (e) { InserirTag(this.MensagemNotificacaoAppTrizy.id, "#DescricaoMotivo"); }, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.DescricaoDoMotivo, enable: ko.observable(true), visible: ko.observable(true) });

    // Tab App Trizy (Relatório Viagem)
    this.GridInformacoesAdicionaisRelatorioViagemSuperApp = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), list: new Array() });
    this.HabilitarEnvioRelatorio = PropertyEntity({ text: 'Habilitar Envio Relatório de Viagem no SuperApp', getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TituloRelatorioViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TituloRelatorioViagemSuperApp, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.TituloReciboViagem = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.TituloReciboViagemSuperApp, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.CodigoInformacaoAdicionalReciboViagemSuperApp = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.RotuloInformacaoAdicionalReciboViagemSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.RotuloInformacaoAdicionalReciboViagemSuperApp, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.DescricaoInformacaoAdicionalReciboViagemSuperApp = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.DescricaoInformacaoAdicionalReciboViagemSuperApp, getType: typesKnockout.string, val: ko.observable(""), def: ko.observable(""), enable: ko.observable(true) });
    this.AdicionarCadastroInformacaoAdicionalRelatorioViagemSuperApp = PropertyEntity({ eventClick: adicionarListaCadastroInformacaoAdicionalRelatorioViagemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarCadastroInformacaoAdicionalRelatorioViagemSuperApp = PropertyEntity({ eventClick: atualizarListaCadastroInformacaoAdicionalRelatorioViagemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.CancelarCadastroInformacaoAdicionalRelatorioViagemSuperApp = PropertyEntity({ eventClick: limparCamposCadastroInformacaoAdicionalRelatorioViagemClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.VerCadRelatorioViagem = PropertyEntity({ val: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarInformacaoAdicionalRelatorioViagemModal = PropertyEntity({ eventClick: adicionarInformacaoAdicionalModalClick, type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarInformacaoAdicionalRelatorioViagemModal });

    //Tab Pedido
    this.BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FiltrarPedidosPorRemetenteRetiradaProduto = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.FiltrarPedidosPorRemetenteRetiradaProduto, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.EnviarPedidoReentregaAutomaticamenteRoteirizar = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarPedidoReentregaAutomaticamenteRoteirizar, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });

    this.SelecionarRetiradaProduto.val.subscribe(function (novoValor) {
        _tipoOperacao.FiltrarPedidosPorRemetenteRetiradaProduto.visible(novoValor);

        if (!novoValor)
            _tipoOperacao.FiltrarPedidosPorRemetenteRetiradaProduto.val(novoValor);
    });


    this.Reentrega.val.subscribe(function (novoValor) {
        _tipoOperacao.EnviarPedidoReentregaAutomaticamenteRoteirizar.visible(novoValor);

        if (!novoValor)
            _tipoOperacao.EnviarPedidoReentregaAutomaticamenteRoteirizar.val(novoValor);
    });

    this.TipoNotificacaoAppTrizy.val.subscribe(function (val) {
        if (val === 7) {
            _tipoOperacao.TituloNotificacaoAppTrizy.enable(false);
            _tipoOperacao.TituloNotificacaoAppTrizy.val("Atendimento #NumeroAtendimento - #ChamadoCliente");
            _tipoOperacao.MensagemNotificacaoAppTrizy.enable(false);
            _tipoOperacao.MensagemNotificacaoAppTrizy.val("#TratativaChamado - #RetornoMotoristaChamado");
        }
        else {
            _tipoOperacao.MensagemNotificacaoAppTrizy.enable(true);
            _tipoOperacao.TituloNotificacaoAppTrizy.enable(true);
            LimparCampo(_tipoOperacao.MensagemNotificacaoAppTrizy);
            LimparCampo(_tipoOperacao.TituloNotificacaoAppTrizy);
        }
    });

    //Tab Vendedores

    this.Vendedores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListVendedores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    // this.GridGrupoPessoasVendedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });

    // Tab Gestão Devolução
    this.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    // Tab Tipo Propriedade Veiculo
    this.TipoPropriedadeVeiculo = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeVeiculoTipoOperacao.Ambos), options: EnumTipoPropriedadeVeiculoTipoOperacao.obterOpcoes(), def: EnumTipoPropriedadeVeiculoTipoOperacao.Ambos, text: Localization.Resources.Pedidos.TipoOperacao.VeiculosQuePodemTransportar });
    this.TipoProprietarioVeiculo = PropertyEntity({ val: ko.observable(EnumTipoProprietarioVeiculo.Todos), options: EnumTipoProprietarioVeiculo.obterOpcoesMotivoChamado(), def: EnumTipoProprietarioVeiculo.Todos, text: Localization.Resources.Pedidos.TipoOperacao.TipoTransportador, visible: ko.observable(true) });
    this.TiposTerceirosPropriedadeVeiculo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridTipoTerceiroPropriedadeVeiculo = PropertyEntity({ type: types.local });
    this.TerceiroPropriedadeVeiculo = PropertyEntity({ type: types.event, text: Localization.Resources.Pedidos.TipoOperacao.AdicionarTipoTerceiro, idBtnSearch: guid() });

    this.TipoPropriedadeVeiculo.val.subscribe(function (val) {
        var element = document.getElementById('knockoutTiposTerceiroPropriedadeVeiculo');
        if (val == EnumTipoPropriedadeVeiculoTipoOperacao.Ambos || val == EnumTipoPropriedadeVeiculoTipoOperacao.SomenteTerceiros) {
            _tipoOperacao.TipoProprietarioVeiculo.visible(true);
            element.style.display = 'block';
        }
        else {
            RecarregarGridTipoTerceiroPropriedadeVeiculo();
            _tipoOperacao.TipoProprietarioVeiculo.val(EnumTipoProprietarioVeiculo.Todos);
            _tipoOperacao.TipoProprietarioVeiculo.visible(false);
            element.style.display = 'none';
        }
    });

    // Tab Cotação de pedido
    this.HabilitaInformarDadosDosPedidosNaCotacao = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.HabilitaInformarDadosDosPedidosNaCotacao, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    // Tab Container

    this.GestaoViagemContainerFluxoUnico = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.GestaoViagemContainerFluxoUnico, required: false, maxlength: 12, visible: ko.observable(true) });
    this.NaoPermitirAlterarMotoristaAposAverbacaoContainer = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.NaoPermitirAlterarMotoristaAposAverbacaoContainer, required: false, maxlength: 12, visible: ko.observable(true) });
    this.ExigirComprovanteColetaContainerParaSeguir = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.ExigirComprovanteColetaContainerParaSeguir, required: false, maxlength: 12, visible: ko.observable(true) });
    this.TipoPagamentoAdiantamentoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoPagamentoAdiantamentoContainer.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.ModeloDocumentoDocumentoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.ModeloDocumentoContainer.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.TipoComprovanteColetaContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.TipoOperacao.TipoComprovanteColetaContainer.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });

    this.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer, required: false, maxlength: 12, visible: ko.observable(true) });
    this.ComprarValePedagioEtapaContainer = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.TipoOperacao.ComprarValePedagioEtapaContainer, required: false, maxlength: 12, visible: ko.observable(true) });
    this.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer.val.subscribe(function (novoValor) {
        _tipoOperacao.TipoPagamentoAdiantamentoContainer.visible(novoValor);
        _tipoOperacao.TipoPagamentoAdiantamentoContainer.required(novoValor);

        if (!novoValor && !_tipoOperacao.ComprarValePedagioEtapaContainer.val() && !_tipoOperacao.AverbarContainerComAverbacaoCarga.val()) {
            _tipoOperacao.GestaoViagemContainerFluxoUnico.val(false);
        }
    });

    this.ComprarValePedagioEtapaContainer.val.subscribe(function (novoValor) {
        if (!novoValor && !_tipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer.val() && !_tipoOperacao.AverbarContainerComAverbacaoCarga.val()) {
            _tipoOperacao.GestaoViagemContainerFluxoUnico.val(false);
        }
    });
    this.ExigirComprovanteColetaContainerParaSeguir.val.subscribe(function (novoValor) {
        _tipoOperacao.TipoComprovanteColetaContainer.visible(novoValor);
        _tipoOperacao.TipoComprovanteColetaContainer.required(novoValor);

    });

    this.GestaoViagemContainerFluxoUnico.val.subscribe(function (novoValor) {
        if (novoValor) {
            _tipoOperacao.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer.val(novoValor);
            _tipoOperacao.ComprarValePedagioEtapaContainer.val(novoValor);
            _tipoOperacao.AverbarContainerComAverbacaoCarga.val(novoValor);

        }
        _tipoOperacao.ModeloDocumentoDocumentoContainer.required(novoValor);

    });

};

var TipoOperacaoEmissao = function () {
    this.UsarConfiguracaoEmissao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.EspecificarOutraConfiguracaoDeEmissaoParaEsseTipoDeOperacao, issue: 624, def: false, visible: ko.observable(true) });
};

var TipoOperacaoFatura = function () {
    this.UsarConfiguracaoFaturaPorTipoOperacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pedidos.TipoOperacao.UtilizarConfiguracaoFaturaPorTipoOperacao, issue: 624, def: false, visible: ko.observable(true) });
};

var CRUDTipoOperacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

function exibirModalEventoCadastro() {
    Global.abrirModal('divModalCadastroEvento');
    $("#divModalCadastroEvento").one('hidden.bs.modal', function () {
        limparCamposCadastroEventoSuperApp();
    });
}

function exibirModalInformacaoAdicionalRelatorioViagem() {
    Global.abrirModal('divModalCadastroInformacaoAdicionalRelatorioViagem');
    $("#divModalCadastroInformacaoAdicionalRelatorioViagem").one('hidden.bs.modal', function () {
        limparCamposCadastroInformacaoAdicionalRelatorioViagem();
    });
}

// #endregion Classes

// #region Funções de Inicialização

async function loadTipoOperacao() {
    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutCadastroTipoOperacao");

    _pesquisaTipoOperacao = new PesquisaTipoOperacao();
    KoBindings(_pesquisaTipoOperacao, "knockoutPesquisaTipoOperacao", false, _pesquisaTipoOperacao.Pesquisar.id);

    _CRUDTipoOperacao = new CRUDTipoOperacao();
    KoBindings(_CRUDTipoOperacao, "knoutCRUDTipoOperacao");

    _tipoOperacaoEmissao = new TipoOperacaoEmissao();
    KoBindings(_tipoOperacaoEmissao, "knockoutEmissao");

    _tipoOperacaoFatura = new TipoOperacaoFatura();
    KoBindings(_tipoOperacaoFatura, "knockoutConfiguracaoFatura");

    _configuracaoLayoutEDI = new ConfiguracaoLayoutEDI("divConfiguracaoLayoutEDI", _tipoOperacao.ConfiguracaoLayoutEDI, function () {
        _configuracaoLayoutEDI.Configuracao.UtilizarLeituraArquivos.visible(false);
        _configuracaoLayoutEDI.Configuracao.UtilizarLeituraArquivos.defVisible = false;
    });

    HeaderAuditoria("TipoOperacao", _tipoOperacao);

    loadTiposTerceiros();
    loadTiposTerceirosPropriedadeVeiculo();
    loadSeguro();
    LoadRegraCancelamentoPedido();
    LoadConfiguracaoTerceiro();
    loadIntegracao();
    LoadConfiguracaoIntegracaoMultiEmbarcador();
    loadTipoOperacaoTransportador();
    loadTipoOperacaoControleColetaEntregaOcorrencia();
    buscarProdutosPadroes();
    buscarOcorrencias();
    loadGrupoTomadoresBloqueados();
    loadMotivoAtendimento();
    loadGridMotivoAtendimentoTransportador();
    loadConfiguracaoCargaEstado();
    LoadTiposCargas();
    LoadTipoCargaEmissao();
    loadTipoOperacaoControleEntregaSetor();
    loadGridTransportadorMontagem();
    loadAnexos();
    loadComprovante();
    verificarSeExiste();
    loadGridNotificacaoApp([]);
    loadGridEventoSuperApp([]);
    loadGridInformacoesAdicionaisRelatorioViagemSuperApp([]);
    LoadTipoOperacaoVendedores();
    loadGridMotoristaGenericoFilial();

    carregarCodigosIntegracaoTipoOperacao();

    new BuscarClientes(_tipoOperacao.Expedidor);
    new BuscarClientes(_tipoOperacao.Recebedor);
    new BuscarCentroResultado(_tipoOperacao.CentroDeResultado)
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoPadraoFerroviario);
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoRedespacho);
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoCargaEspelho);
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoCargaRetornoColetasRejeitadas);
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoPrecheckin);
    new BuscarTiposOperacao(_tipoOperacao.TipoOperacaoPrecheckinTransferencia);
    new BuscarClientes(_tipoOperacao.Pessoa, null, true);
    new BuscarGruposPessoas(_tipoOperacao.GrupoPessoas, consultarGrupoPessoasRetorno, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_tipoOperacao.GrupoTomador, null, null, null, EnumTipoGrupoPessoas.Ambos);
    new BuscarComponentesDeFrete(_tipoOperacao.ComponenteFrete);
    new BuscarTipoOcorrencia(_tipoOperacao.TipoOcorrencia);
    new BuscarTipoOcorrencia(_tipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo, null, null, null, null, null, null, null, null, true);

    new BuscarClientes(_pesquisaTipoOperacao.Pessoa, null, true);
    new BuscarGruposPessoas(_pesquisaTipoOperacao.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTiposdeCarga(_tipoOperacao.TipoDeCargaPadraoOperacao);
    new BuscarGrupoTipoOperacao(_tipoOperacao.GrupoTipoOperacao);
    new BuscarProdutos(_tipoOperacao.Produto, null, null, null, null, null, null, _gridProdutos);
    new BuscarTipoOcorrencia(_tipoOperacao.Ocorrencia, null, null, null, null, null, _gridOcorrencias);

    new BuscarCheckListTipo(_tipoOperacao.CheckListDesembarque);
    new BuscarCheckListTipo(_tipoOperacao.CheckListEntrega);
    new BuscarCheckListTipo(_tipoOperacao.CheckListColeta);
    new BuscarClientes(_tipoOperacao.LocalDeParqueamentoCliente);
    new BuscarTipoRetornoCarga(_tipoOperacao.TipoRetornoCarga);
    new BuscarPagamentoMotoristaTipo(_tipoOperacao.TipoPagamentoAdiantamentoContainer);
    new BuscarModeloDocumentoFiscal(_tipoOperacao.ModeloDocumentoDocumentoContainer, null, null, true);
    new BuscarTipoComprovante(_tipoOperacao.TipoComprovanteColetaContainer);
    new BuscarChecklistsSuperApp(_tipoOperacao.ChecklistEventoSuperApp);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _tipoOperacao.UtilizarMoedaEstrangeira.visible(true);
        _tipoOperacao.Moeda.visible(true);
    }

    if (_CONFIGURACAO_TMS.ExigirRotaRoteirizadaNaCarga) {
        _tipoOperacao.NaoExigeRotaRoteirizada.visible(true);
        _tipoOperacao.RoteirizarPorLocalidade.visible(true);
    }
    obterConfiguracaoTipoOperacaoCamposObrigatorios();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $(".visible-multiembarcador").removeClass("d-none");
        $("#liCadastroLayoutEDI").show();
        $("#liTabOcorrencias").show();
        $("#liTiposCargas").removeClass("d-none");
        _tipoOperacao.EmitirNFeRemessa.visible(false);
        _tipoOperacao.AlterarRemetentePedidoConformeNotaFiscal.visible(false);
        _tipoOperacao.AvancarCargaAutomaticaAposMontagem.visible(false);
        _tipoOperacao.AvancarEtapaFreteAutomaticamente.visible(true);
        _tipoOperacao.ValidarTomadorDoPedidoDiferenteDaCarga.visible(false);
        _tipoOperacao.ImportarTerminalDestinoComoRecebedor.visible(false);
        _tipoOperacao.ImportarTerminalDestinoComoRecebedor.visible(false);
        _tipoOperacao.TipoImpressao.visible(false);
        _tipoOperacao.GeraCargaAutomaticamente.visible(_CONFIGURACAO_TMS.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido);
        _tipoOperacao.OperacaoDeImportacaoExportacao.visible(false);
        _tipoOperacao.GrupoPessoas.visible(false);
        _tipoOperacao.Pessoa.visible(false);
        _tipoOperacao.TipoPessoa.visible(false);
        _tipoOperacao.ExigeNotaFiscalParaCalcularFrete.visible(true);
        _tipoOperacao.TipoOperacaoUtilizaCentroDeCustoPEP.visible(true);
        _tipoOperacao.TipoOperacaoUtilizaContaRazao.visible(true);
        _tipoOperacao.UtilizarExpedidorComoTransportador.visible(true);
        _tipoOperacao.TipoDeCargaPadraoOperacao.visible(true);
        _tipoOperacao.GrupoTomador.visible(true);
        _tipoOperacao.EmissaoMDFeManual.visible(true);
        _tipoOperacao.PermitirTransbordarNotasDeOutrasCargas.visible(true);
        _tipoOperacao.EmiteCTeFilialEmissora.visible(true);
        _tipoOperacao.AverbarDocumentoDaSubcontratacao.visible(true);
        _tipoOperacao.HabilitarGestaoPatio.visible(true);
        _tipoOperacao.HabilitarGestaoPatioDestino.visible(_CONFIGURACAO_TMS.GerarFluxoPatioDestino);
        _tipoOperacao.OperacaoRecolhimentoTroca.visible(true);
        _tipoOperacao.UsaJanelaCarregamentoPorEscala.visible(true);
        _tipoOperacao.UtilizarTipoOperacaoPreCargaAoGerarCarga.visible(true);
        _tipoOperacao.NaoExigeConformacaoDasNotasEmissao.visible(true);
        _tipoOperacao.LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento.visible(true);
        _tipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento.visible(true);
        _tipoOperacao.UtilizarTipoCargaPadrao.visible(true);
        _tipoOperacao.FretePorContadoCliente.visible(true);
        _tipoOperacao.IndicadorGlobalizadoRemetente.visible(true);
        _tipoOperacao.ProdutoPredominanteOperacao.visible(true);
        _tipoOperacao.EmitirDocumentosRetroativamente.visible(true);
        _tipoOperacao.ExigeRecebedor.visible(true);
        _tipoOperacao.OperacaoDeRedespacho.visible(true);
        _tipoOperacao.NaoExigeVeiculoParaEmissao.visible(true);
        _tipoOperacao.ExigeQueVeiculoIgualModeloVeicularDaCarga.visible(true);
        _tipoOperacao.PermiteImportarDocumentosManualmente.visible(true);
        _tipoOperacao.ValidarNotaFiscalPeloDestinatario.visible(true);
        _tipoOperacao.PermitirQualquerModeloVeicular.visible(true);
        _tipoOperacao.NaoValidarTransportadorImportacaoDocumento.visible(true);
        _tipoOperacao.EmissaoDocumentosForaDoSistema.visible(true);
        _tipoOperacao.CompraValePedagioDocsEmitidosFora.visible(true);
        _tipoOperacao.ExibirNumeroPedidoNosDetalhesDaCarga.visible(true);

        _tipoOperacao.NaoPermiteAgruparCargas.visible(true);
        _tipoOperacao.ManterUnicaCargaNoAgrupamento.visible(true);
        _tipoOperacao.GerarCargaViaMontagemDoTipoPreCarga.visible(true);
        _tipoOperacao.UsarRecebedorComoPontoPartidaCarga.visible(true);

        _tipoOperacao.OperacaoExigeInformarCargaRetorno.visible(true);
        _tipoOperacao.GerarCTeComplementarNaCarga.visible(true);
        _tipoOperacao.ExclusivaDeSubcontratacaoOuRedespacho.visible(true);
        _tipoOperacao.UtilizarRecebedorApenasComoParticipante.visible(true);
        _tipoOperacao.IndicadorGlobalizadoDestinatario.visible(true);
        _tipoOperacao.SempreUsarIndicadorGlobalizadoDestinatario.visible(true);
        _tipoOperacao.NotificarCasoNumeroPedidoForExistente.visible(true);
        _tipoOperacao.IndicadorGlobalizadoDestinatarioNFSe.visible(true);
        _tipoOperacao.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior.visible(true);
        _tipoOperacao.PermitirAdicionarRemoverPedidosEtapa1.visible(true);
        _tipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento.visible(_CONFIGURACAO_TMS.UtilizaPgtoCanhoto);
        _tipoOperacao.RetornarCarregamentoPendenteAposEtapaCTE.visible(true);
        _tipoOperacao.DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos.visible(true);
        _tipoOperacao.DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos.visible(true);
        _tipoOperacao.LogisticaReversa.visible(true);
        _tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes.visible(true);
        _tipoOperacao.NaoGerarCIOT.visible(true);
        _tipoOperacao.TipoOperacaoMercosul.visible(true);
        _tipoOperacao.TipoOperacaoInternacional.visible(true);
        _tipoOperacao.ObrigatorioPassagemExpedicao.visible(true);
        _tipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo.visible(true);
        _tipoOperacao.PermiteBaixarComprovanteColeta.visible(true);
        _tipoOperacao.PermitirTransportadorSolicitarNotasFiscais.visible(true);
        _tipoOperacao.NaoEmitirCargaComValorZerado.visible(true);
        _tipoOperacao.GerarLoteEntregaAutomaticamente.visible(true);
        _tipoOperacao.PermiteEmitirCargaSemAverbacao.visible(false);
        _tipoOperacao.EmissaoDocumentoFinalizarCargaAutomaticamente.visible(true);

        if (_CONFIGURACAO_TMS.NaoPermiteEmitirCargaSemAverbacao)
            _tipoOperacao.PermitirCargaSemAverbacao.visible(true);

        if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador) {
            _tipoOperacao.Pessoa.visible(true);
            _tipoOperacao.TipoPessoa.visible(true);
        }

        CarregarCamposIntegracaoTrizy();
       
        _pesquisaTipoOperacao.BuscaAvancada.visible(false);

        if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador)
            $("#liIntegracaoMultiEmbarcador").removeClass("d-none");
        else
            $("#liIntegracaoMultiEmbarcador").addClass("d-none");

        if (_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum)
            _tipoOperacao.ConsiderarTomadorPedido.visible(true);

        obterConfiguracaoTMSGeral();
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liCadastroLayoutEDI").show();
        $("#liTabComprovante").show();
        $(".visible-multiTMS").removeClass("d-none");
        _tipoOperacao.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido.visible(true);
        _tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes.visible(true);
        _tipoOperacao.IndicadorGlobalizadoDestinatario.visible(true);
        _tipoOperacao.NotificarCasoNumeroPedidoForExistente.visible(true);
        _tipoOperacao.IndicadorGlobalizadoDestinatarioNFSe.visible(true);
        _tipoOperacao.IndicadorGlobalizadoRemetente.visible(true);
        _tipoOperacao.ProdutoPredominanteOperacao.visible(true);
        _tipoOperacao.TipoDeCargaPadraoOperacao.visible(true);
        _tipoOperacao.UtilizarTipoCargaPadrao.visible(true);
        _tipoOperacao.OperacaoDeImportacaoExportacao.visible(true);
        _tipoOperacao.GeraCargaAutomaticamente.visible(true);
        _tipoOperacao.TipoImpressao.visible(false);
        _tipoOperacao.NaoExigeVeiculoParaEmissao.visible(true);
        _tipoOperacao.UtilizarFatorCubagem.visible(true);
        _tipoOperacao.UtilizarPaletizacao.visible(true);
        _tipoOperacao.NaoPermitirAlterarValorFreteNaCarga.visible(true);
        _tipoOperacao.ObrigatorioPassagemExpedicao.visible(true);
        _tipoOperacao.ColetaEmProdutorRural.visible(true);
        _tipoOperacao.GerarComissaoParcialMotorista.visible(true);
        _tipoOperacao.CamposSecundariosObrigatoriosPedido.visible(true);
        _tipoOperacao.NaoEmitirCargaComValorZerado.visible(true);
        _tipoOperacao.DeslocamentoVazio.visible(true);
        _tipoOperacao.PermitirAdicionarRemoverPedidosEtapa1.visible(true);
        _tipoOperacao.AvancarEtapaFreteAutomaticamente.visible(true);
        _tipoOperacao.NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento.visible(true);
        _tipoOperacao.TipoOperacaoMercosul.visible(true);
        _tipoOperacao.OperacaoExigeInformarCargaRetorno.visible(true);
        _tipoOperacao.GerarCTeComplementarNaCarga.visible(true);
        _tipoOperacao.HabilitarGestaoPatio.visible(true);
        _tipoOperacao.GerarLoteEntregaAutomaticamente.visible(true);
        _tipoOperacao.EmissaoDocumentoFinalizarCargaAutomaticamente.visible(true);

        if (_CONFIGURACAO_TMS.NaoPermiteEmitirCargaSemAverbacao)
            _tipoOperacao.PermitirCargaSemAverbacao.visible(true);

        CarregarCamposIntegracaoTrizy();

        $("#liCancelamentoPedido").removeClass("d-none");
        $("#liTerceiro").removeClass("d-none");
        $("#liControleColetaEntrega").removeClass("d-none");
    }

    if (_CONFIGURACAO_TMS.Pais != 0) {
        _tipoOperacao.TipoOperacaoPadraoParaFretesDentroDoPais.visible(true);
        _tipoOperacao.TipoOperacaoPadraoParaFretesForaDoPais.visible(true);
    }

    $("#liEmissao").show();
    _configuracaoEmissaoCTe = new ConfiguracaoEmissaoCTe("divConfiguracaoEmissaoCTe", _tipoOperacao.ConfiguracaoEmissaoCTe, _tipoOperacao.Pessoa, _tipoOperacao.GrupoPessoas, null, function () {
        _configuracaoEmissaoCTe.Configuracao.TipoEnvioEmail.visible(false);
        _configuracaoEmissaoCTe.Configuracao.ValorLimiteFaturamento.visible(false);
        _configuracaoEmissaoCTe.Configuracao.AvancarCargasDocumentosVinculados.eventClick = AvancarCargasDocumentosVinculadosClick;

        _configuracaoEmissaoCTe.Configuracao.TipoRateioDocumentos.val.subscribe(function (novoValor) {
            _tipoOperacao.DesconsiderarNotaPalletEmissaoCTE.visible(novoValor == EnumTipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual);
        });
    });

    $("#liTabConfiguracaoFatura").show();
    _configuracaoFatura = new ConfiguracaoFatura("divConfiguracaoFatura", _tipoOperacao.ConfiguracaoFatura, function () {
        _configuracaoFatura.Configuracao.Banco.visible(false);
        _configuracaoFatura.Configuracao.Agencia.visible(false);
        _configuracaoFatura.Configuracao.Digito.visible(false);
        _configuracaoFatura.Configuracao.NumeroConta.visible(false);
        _configuracaoFatura.Configuracao.TipoConta.visible(false);
    });

    buscarTiposOperacao();

    $("#" + _tipoOperacao.InformarProdutoPredominanteOperacao.id).click(InformarProdutoPredominanteOperacaoClick);
    $("#" + _tipoOperacao.UtilizarGrupoTomador.id).click(controlarCampoGrupoTomadorHabilitado);
    $("#" + _tipoOperacao.UtilizarTipoCargaPadrao.id).click(controlarCampoTipoDeCargaPadraoOperacaoHabilitado);
    $("#" + _tipoOperacao.UtilizarCorJanelaCarregamento.id).click(controlarCampoCorJanelaCarregamento);

    ConfigurarIntegracoesDisponiveis();
    ObterConfiguracoes();

    if (_CONFIGURACAO_TMS.AjustarTipoOperacaoPeloPeso) {
        _tipoOperacao.PesoMinimo.visible(true);
        _tipoOperacao.PesoMaximo.visible(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _tipoOperacao.TipoObrigacaoUsoTerminal.visible(true);
        _tipoOperacao.AvancarCargaAutomaticaAposMontagem.visible(true);
        _tipoOperacao.ExigeProdutoEmbarcadorPedido.visible(true);
        _tipoOperacao.BloquearEmisssaoComMesmoLocalDeOrigemEDestino.visible(true);
        _tipoOperacao.IgnorarRateioConfiguradoPorto.visible(true);
    }

    if (!_CONFIGURACAO_TMS.PermitirRejeitarCargaJanelaCarregamentoTransportador) {
        _tipoOperacao.PermitirRejeitarCargaJanelaCarregamentoTransportador.visible(false);
    }

    _tipoOperacao.ExigirInformarNumeroPacotesNaColetaTrizy.visible(_CONFIGURACAO_TMS.UtilizaAppTrizy);

    _tipoOperacao.GrupoTipoOperacao.visible(_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento);
    _tipoOperacao.GrupoTipoOperacao.enable(_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento);

    if (_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) {
        _tipoOperacao.OperacaoDestinadaCTeComplementar.text(Localization.Resources.Pedidos.TipoOperacao.CopiarDadosPlanejamentoOSMae);
    }

    if (_CONFIGURACAO_TMS.PossuiIntegracaoIntercab)
        $("#liTabIntercab").show();

    CarregarCamposIntegracaoTrizy();
    
    if (_CONFIGURACAO_TMS.PossuiIntegracaoEMP || _tipoOperacao.AtivarIntegracaoComSIL.val())
        $("#liTabEMP").show();

    if (_CONFIGURACAO_TMS.ControleComissaoPorTipoOperacao) {
        $("#liTabVendedor").show();
    }


    // Diária automática
    let configuracaoDiariaAutomatica = await obterConfiguracaoPadraoDiariaAutomatica();
    if (configuracaoDiariaAutomatica.HabilitarDiariaAutomatica) {
        $("#liTabFreeTime").show();
    }

    $("#txtEditor").summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link']],
            ['view', ['fullscreen', 'codeview']],
        ]
    });

    $("#txtEditor").summernote('code', '');
    atualizarConfiguracoes();
}

function loadGridMotoristaGenericoFilial() {
    var excluir = { descricao: "Remover", id: guid(), metodo: excluirFilialMotoristaGenerico }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Filial, width: "85%" },
        { data: "MotoristaCodigo", visible: false },
        { data: "MotoristaDescricao", title: Localization.Resources.Gerais.Geral.Motorista, width: "85%" },
    ];

    _gridFilialMotoristaGenerico = new BasicDataTable("grid-filial-motorista-generico", header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarMotoristas(_tipoOperacao.MotoristaGenerico);
    new BuscarFilial(_tipoOperacao.FilialMotoristaGenerico);

    _gridFilialMotoristaGenerico.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function adicionarClick() {
    preencherListasSelecao();
    let tudoCerto = true;
    _tipoOperacao.ComponenteFrete.requiredClass("form-control");
    _tipoOperacao.TipoOcorrencia.requiredClass("form-control");
    if (_tipoOperacao.HabilitarCobrancaEstadiaAutomaticaPeloTracking.val() === true) {
        tudoCerto = !((_tipoOperacao.ComponenteFrete.codEntity() === 0) || (_tipoOperacao.TipoOcorrencia.codEntity() === 0));
        if (!tudoCerto) {
            _tipoOperacao.ComponenteFrete.requiredClass("form-control is-invalid");
            _tipoOperacao.TipoOcorrencia.requiredClass("form-control is-invalid");
        }
    }
    if (_tipoOperacao.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val() === true && _tipoOperacao.ExigeNotaFiscalParaCalcularFrete.val() === true) {
        tudoCerto = false;
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Você não pode habilitar simultaneamente a opção 'Permitir informar Transportador na Etapa 1 sem nota fiscal recebida antes do cálculo do Frete' e 'Esta operação requer que a nota fiscal seja recebida antes de calcular o Frete'.");
        return;
    }
    if (_tipoOperacao.GerarCTeSimplificadoQuandoCompativel.val() === true && (_tipoOperacaoEmissao.UsarConfiguracaoEmissao.val() === false || _configuracaoEmissaoCTe.Configuracao.TipoRateioDocumentos.val() != EnumTipoEmissaoCTeDocumentos.NaoInformado)) {
        tudoCerto = false;
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.TipoOperacao.QuandoHabilitadoGerarCTeSimplificadoQuandoCompativelENecessario);
        return;
    }
    if (versaoTrizy == 3) {
        const eventosSuperApp = obterListaEventoSuperApp();
        if (!eventosSuperApp.some(evento => [1, 2].includes(evento.TipoParada))) {
            tudoCerto = false;
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "É necessário cadastrar pelo menos um evento para Entrega no App Trizy.");
            return;
        }
    }
    if (tudoCerto && ValidarCamposObrigatorios(_tipoOperacao)) {
        executarReST("TipoOperacao/Adicionar", obterTipoOperacaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridTipoOperacao.CarregarGrid();
                    limparCamposTipoOperacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}
function atualizarConfiguracoes() {
    if (!_configuracoesTipoOperacaoValorPadrao || !_tipoOperacao) {
        return;
    }

    _configuracoesTipoOperacaoValorPadrao.forEach(config => {
        const key = config.Descricao;

        if (_tipoOperacao[key] && typeof _tipoOperacao[key].val === 'function') {
            _tipoOperacao[key].val(config.Habilitar);
        } else {
        }
    });
}


const interval = setInterval(() => {
    if (typeof _configuracoesTipoOperacaoValorPadrao !== 'undefined') {
        clearInterval(interval);
        atualizarConfiguracoes();
    }
}, 100);



function atualizarClick() {
    preencherListasSelecao();
    let tudoCerto = true;
    _tipoOperacao.ComponenteFrete.requiredClass("form-control");
    _tipoOperacao.TipoOcorrencia.requiredClass("form-control");
    if (_tipoOperacao.HabilitarCobrancaEstadiaAutomaticaPeloTracking.val() === true) {
        tudoCerto = !((_tipoOperacao.ComponenteFrete.codEntity() === 0) || (_tipoOperacao.TipoOcorrencia.codEntity() === 0));
        if (!tudoCerto) {
            _tipoOperacao.ComponenteFrete.requiredClass("form-control is-invalid");
            _tipoOperacao.TipoOcorrencia.requiredClass("form-control is-invalid");
        }
    }
    if (_tipoOperacao.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val() === true && _tipoOperacao.ExigeNotaFiscalParaCalcularFrete.val() === true) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Você não pode habilitar simultaneamente a opção 'Permitir informar Transportador na Etapa 1 sem nota fiscal recebida antes do cálculo do Frete' e 'Esta operação requer que a nota fiscal seja recebida antes de calcular o Frete'.");
        return;
    }
    if (_tipoOperacao.GerarCTeSimplificadoQuandoCompativel.val() === true && (_tipoOperacaoEmissao.UsarConfiguracaoEmissao.val() === false || _configuracaoEmissaoCTe.Configuracao.TipoRateioDocumentos.val() != EnumTipoEmissaoCTeDocumentos.NaoInformado)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.TipoOperacao.QuandoHabilitadoGerarCTeSimplificadoQuandoCompativelENecessario);
        return;
    }
    if (_tipoOperacao.HabilitarVinculoMotoristaGenericoCarga.val() && _gridFilialMotoristaGenerico.BuscarRegistros().slice().length === 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "Ao habilitar a opção de vínculo de motorista genérico, é necessário adicionar ao menos uma filial vinculada ao motorista.");
        return;
    }
    if (versaoTrizy == 3) {
        const eventosSuperApp = obterListaEventoSuperApp();
        if (!eventosSuperApp.some(evento => [1, 2].includes(evento.TipoParada))) {
            tudoCerto = false;
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, "É necessário cadastrar pelo menos um evento para Entrega no App Trizy.");
            return;
        }
    }
    if (tudoCerto && ValidarCamposObrigatorios(_tipoOperacao)) {
        executarReST("TipoOperacao/Atualizar", obterTipoOperacaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    _gridTipoOperacao.CarregarGrid();
                    limparCamposTipoOperacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function cancelarClick() {
    limparCamposTipoOperacao();
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.TipoOperacao.RealmenteDesejaExcluirTipoDeCarga + _tipoOperacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoOperacao, "TipoOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridTipoOperacao.CarregarGrid();
                    limparCamposTipoOperacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

        }, null);
    });
}

function InformarProdutoPredominanteOperacaoClick() {
    if (_tipoOperacao.InformarProdutoPredominanteOperacao.val()) {
        _tipoOperacao.ProdutoPredominanteOperacao.required = true;
        _tipoOperacao.ProdutoPredominanteOperacao.enable(true);
    } else {
        _tipoOperacao.ProdutoPredominanteOperacao.required = false;
        _tipoOperacao.ProdutoPredominanteOperacao.enable(false);
        _tipoOperacao.ProdutoPredominanteOperacao.val("");
    }
}

function TipoPessoaChange(e) {
    if (e.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        e.Pessoa.required = false;
        e.Pessoa.visible(true);
        e.GrupoPessoas.required = false;
        e.GrupoPessoas.visible(false);
        LimparCampoEntity(e.GrupoPessoas);
    }
    else if (e.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        e.Pessoa.required = false;
        e.Pessoa.visible(false);
        e.GrupoPessoas.required = false;
        e.GrupoPessoas.visible(true);
        LimparCampoEntity(e.Pessoa);
    }
}

function TipoUltimoPontoRoteirizacaoChange(e) {
    validaEixosSuspensosVisible();
}

function validaEixosSuspensosVisible() {
    var visible = false;
    if ((_tipoOperacao.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.AteOrigem || _tipoOperacao.TipoUltimoPontoRoteirizacao.val() == EnumTipoUltimoPontoRoteirizacao.Retornando)) {
        var integracoes = EnumTipoIntegracao.obterOpcoesTipoOperacao();
        for (var i = 0; i < integracoes.length; i++) {
            if (integracoes[i].value == EnumTipoIntegracao.SemParar)
                visible = true;
        }
    }
    _tipoOperacao.EixosSuspenso.visible(visible);
    if (visible == false)
        _tipoOperacao.EixosSuspenso.val(EnumEixosSuspenso.Nenhum);
}

function AvancarCargasDocumentosVinculadosClick() {
    if (!_tipoOperacao.Codigo.val() > 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.TipoOperacao.TipoOperacaoNaoCadastrado);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.TipoOperacao.DesejaRealmenteAvancarAsCargasNaEtapaDoisComEsseTipoDeOperacao, function () {
        executarReST("TipoOperacao/AvancarCargasComDocumentosVinculadosPorTipoOperacao", { Codigo: _tipoOperacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function validarFilialEMotoristaGenerico() {
    return _tipoOperacao.FilialMotoristaGenerico.val() && _tipoOperacao.MotoristaGenerico.val();
}

function adicionarMotoristaGenericoFilial() {
    if (!validarFilialEMotoristaGenerico()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios");
        return;
    }
    let motoristaGenericoFilial = _gridFilialMotoristaGenerico.BuscarRegistros();
    let novoRegistro = preencherMotoristaGenericoFilialTipoOperacao();

    let cadastroDuplicado = motoristaGenericoFilial.find((x) => x.Codigo === (_tipoOperacao.FilialMotoristaGenerico.codEntity()));

    if (cadastroDuplicado) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.TipoOperacao.JaExisteUmMotoristaParaAFilialCadastrado.format(_tipoOperacao.FilialMotoristaGenerico.val()));
        return;
    }

    motoristaGenericoFilial.push(novoRegistro);
    _gridFilialMotoristaGenerico.CarregarGrid(motoristaGenericoFilial);
}

function excluirFilialMotoristaGenerico(registroSelecionado) {
    var listaFiliaisMotoristasGenericos = _gridFilialMotoristaGenerico.BuscarRegistros().slice();

    for (var i = 0; i < listaFiliaisMotoristasGenericos.length; i++) {
        if (registroSelecionado.Codigo == listaFiliaisMotoristasGenericos[i].Codigo) {
            listaFiliaisMotoristasGenericos.splice(i, 1);
            break;
        }
    }

    _gridFilialMotoristaGenerico.CarregarGrid(listaFiliaisMotoristasGenericos);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function buscarTiposOperacao() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (tipoCargaGrid) { editarTipoOperacao(tipoCargaGrid, false); }, tamanho: "20", icone: "" };
    var duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), evento: "onclick", metodo: function (tipoCargaGrid) { editarTipoOperacao(tipoCargaGrid, true); }, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, duplicar);
    var configuracoesExportacao = { url: "TipoOperacao/ExportarPesquisa", titulo: Localization.Resources.Pedidos.TipoOperacao.DescricaoTipoOperacao };

    _gridTipoOperacao = new GridViewExportacao(_pesquisaTipoOperacao.Pesquisar.idGrid, "TipoOperacao/Pesquisa", _pesquisaTipoOperacao, menuOpcoes, configuracoesExportacao);
    _gridTipoOperacao.CarregarGrid();
}

function adicionarEventoModalClick() {
    exibirModalEventoCadastro();
}

function adicionarInformacaoAdicionalModalClick() {
    exibirModalInformacaoAdicionalRelatorioViagem();
}

function ObterConfiguracoes() {
    executarReST("TipoOperacao/ObterConfiguracao", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;
            if (data.TemIntegracaoIntercab) {
                _tipoOperacao.CargaBloqueadaParaEdicaoIntegracao.visible(true);
            }
        }
    });
}

function ConfigurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;
            if (data.TiposExistentes != null && data.TiposExistentes.length > 0) {

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.BrasilRisk; }) && data.PossuiIntegracaoBrasilRisk) {
                    LoadConfiguracaoBrasilRisk();
                    $("#liBrasilRisk").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.MundialRisk; }) && data.PossuiIntegracaoMundialRisk) {
                    LoadConfiguracaoMundialRisk();
                    $("#liMundialRisk").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.GoldenService; }) && data.PossuiIntegracaoGoldenService) {
                    LoadConfiguracaoGoldenService();
                    $("#liGoldenService").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.AngelLira; })) {
                    LoadConfiguracaoAngelLira();
                    $("#liAngelLira").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.OpenTech })) {
                    LoadConfiguracaoOpenTech();
                    $("#liOpenTech").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Logiun; }) && data.PossuiIntegracaoLogiun) {
                    LoadConfiguracaoLogiun();
                    $("#liLogiun").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) {
                    return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.QualP || o == EnumTipoIntegracao.Pamcard
                        || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.DBTrans || o == EnumTipoIntegracao.DigitalCom
                })) {
                    _tipoOperacao.NaoComprarValePedagio.visible(true);
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.QualP || o == EnumTipoIntegracao.DBTrans || o == EnumTipoIntegracao.Pamcard || o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.Repom || o == EnumTipoIntegracao.EFrete })) {
                    _tipoOperacao.PermitirConsultaDeValoresPedagioSemParar.visible(true);
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Raster; })) {
                    LoadConfiguracaoRaster();
                    $("#liRaster").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.NOX; }) && data.PossuiIntegracaoNOX) {
                    LoadConfiguracaoNOX();
                    $("#liNOX").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.A52; })) {
                    LoadConfiguracaoA52();
                    $("#liA52").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.Trizy; })) {
                    LoadConfiguracaoTrizy();
                    $("#liTrizy").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.AX; })) {
                    LoadConfiguracaoAX();
                    $("#liAX").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.Buonny; })) {
                    LoadConfiguracaoBuonny();
                    $("#liBuonny").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o === EnumTipoIntegracao.Diageo; })) {
                    LoadConfiguracaoDiageo();
                    $("#liDiageo").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.TransSat; }) && data.PossuiIntegracaoTransSat) {
                    LoadConfiguracaoTransSat();
                    $("#liTransSat").removeClass("d-none");
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.OpenTech; })) {
                    _tipoOperacao.NaoIntegrarOpentech.visible(true);
                    _tipoOperacao.NaoIntegrarEtapa1Opentech.visible(true);
                    _tipoOperacao.IntegrarPedidosNaIntegracaoOpentech.visible(true);
                    _tipoOperacao.EnviarApenasPrimeiroPedidoNaOpentech.visible(true);
                    _tipoOperacao.EnviarInformacoesTotaisDaCargaNaOpentech.visible(true);
                    _tipoOperacao.ValorMinimoMercadoriaOpenTech.visible(true);
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Telerisco; }))
                    _tipoOperacao.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.BrasilRiskGestao; }))
                    _tipoOperacao.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.BrasilRisk; }))
                    _tipoOperacao.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Adagio; }))
                    _tipoOperacao.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Buonny; }))
                    _tipoOperacao.ValidarMotoristaBuonnyAoConfirmarTransportador.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Minerva; }))
                    _tipoOperacao.NotificarRemetentePorEmailAoSolicitarNotas.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Infolog; }))
                    _tipoOperacao.TipoPlanoInfolog.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.MicDta; })) {
                    _tipoOperacao.RealizarIntegracaoComMicDta.visible(true);
                    _tipoOperacao.IntegrarMICDTAComSiscomex.visible(true);
                }

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Isis; }))
                    _tipoOperacao.ExigirNumeroIsisReturnParaAgendarEntrega.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Klios; }))
                    _tipoOperacao.GerarIntegracaoKlios.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Marfrig; }))
                    _tipoOperacao.EnviarTagsIntegracaoMarfrigComTomadorServico.visible(true);

                if (data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.UnileverLinkNotas; }))
                    _tipoOperacao.EnviarPesoLiquidoLinkNotas.visible(true);

            }

            if (data.OperadorasCIOTExistentes != null && data.OperadorasCIOTExistentes.length > 0) {
                $("#liCIOT").removeClass("d-none");
                LoadConfiguracaoCIOT();

                if (data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Repom || o == EnumOperadoraCIOT.RepomFrete; })) {
                    LoadConfiguracaoRepom();
                    $("#liRepom").removeClass("d-none");
                }
                if (data.OperadorasCIOTExistentes.some(function (o) { return o == EnumOperadoraCIOT.Pagbem; })) {
                    LoadConfiguracaoPagbem();
                    $("#liPagbem").removeClass("d-none");
                }
            }

            _tipoOperacao.AtivarRegraCancelamentoDosPedidosMichelin.visible(data.PossuiIntegracaoMichelin);

        }
    });
}

function consultarGrupoPessoasRetorno(data) {
    executarReST("GrupoPessoas/BuscarPorCodigo", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {

                _tipoOperacao.GrupoPessoas.val(r.Data.Descricao);
                _tipoOperacao.GrupoPessoas.codEntity(r.Data.Codigo);

                if (r.Data.UtilizaMultiEmbarcador)
                    $("#liIntegracaoMultiEmbarcador").removeClass("d-none");
                else
                    $("#liIntegracaoMultiEmbarcador").addClass("d-none");

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, r.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function controlarCampoGrupoTomadorHabilitado() {
    if (_tipoOperacao.UtilizarGrupoTomador.val()) {
        _tipoOperacao.GrupoTomador.required = true;
        _tipoOperacao.GrupoTomador.enable(true);
    }
    else {
        _tipoOperacao.GrupoTomador.required = false;
        _tipoOperacao.GrupoTomador.enable(false);
        _tipoOperacao.GrupoTomador.val("");
        _tipoOperacao.GrupoTomador.codEntity(0);
        _tipoOperacao.GrupoTomador.entityDescription("");
    }
}

function controlarCampoTipoDeCargaPadraoOperacaoHabilitado() {
    if (_tipoOperacao.UtilizarTipoCargaPadrao.val()) {
        _tipoOperacao.TipoDeCargaPadraoOperacao.required = true;
        _tipoOperacao.TipoDeCargaPadraoOperacao.enable(true);
    }
    else {
        _tipoOperacao.TipoDeCargaPadraoOperacao.required = false;
        _tipoOperacao.TipoDeCargaPadraoOperacao.enable(false);
        _tipoOperacao.TipoDeCargaPadraoOperacao.val("");
        _tipoOperacao.TipoDeCargaPadraoOperacao.codEntity(0);
    }
}

function controlarCampoCorJanelaCarregamento() {
    if (_tipoOperacao.UtilizarCorJanelaCarregamento.val())
        _tipoOperacao.CorJanelaCarregamento.enable(true);
    else {
        _tipoOperacao.CorJanelaCarregamento.enable(false);
        _tipoOperacao.CorJanelaCarregamento.val("");
    }
}

function editarTipoOperacao(tipoCargaGrid, duplicar) {
    limparCamposTipoOperacao();

    executarReST("TipoOperacao/BuscarPorCodigo", { Codigo: tipoCargaGrid.Codigo, Duplicar: duplicar }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_tipoOperacao, arg);
                let dados = arg.Data;
                let objetos = [
                    null,
                    'ControleEntrega',
                    'CalculoFrete',
                    'Mobile',
                    'ConfiguracaoCIOTPamcard',
                    'FreeTime',
                    'EmissaoDocumentos',
                    'ConfiguracaoFatura',
                    'Integracao',
                    'CIOT',
                    'MultiEmbarcador',
                    'ConfiguracaoCarga',
                    'Paletes',
                    'ConfiguracaoTerceiro',
                    'ConfiguracaoAgendamentoColetaEntrega',
                    'ConfiguracaoImpressao',
                    'ConfiguracaoTransportador',
                    'ConfiguracaoCanhoto',
                    'ConfiguracaoDocumentoEmissao',
                    'ConfiguracaoControleEntrega',
                    'ConfiguracaoAtendimento',
                    'ConfiguracaoLicenca',
                    'ConfiguracaoJanelaCarregamento',
                    'ConfiguracaoMontagem',
                    'Comprovantes',
                    'ConfiguracaoIntegracaoDiageo',
                    'ConfiguracaoPagamentos',
                    'ConfiguracaoIntercab',
                    'ConfiguracaoEMP',
                    'ConfiguracaoTrizy',
                    'ConfiguracaoPedido',
                    'ConfiguracaoGestaoDevolucao',
                    'ConfiguracaoTipoPropriedadeVeiculo',
                    'ConfiguracaoCotacaoPedido',
                    'Vendedores',
                    'ConfiguracaoTrizy',
                    'ConfiguracaoIntegracaoTransSat',
                    'ConfiguracaoPesoConsideradoCarga',
                    'ConfiguracaoContainer',
                    'CodigosIntegracao'
                ];
                for (let objeto of objetos) {
                    let dadosObjeto = objeto == null ? dados : dados[objeto];
                    PreencherObjetoKnout(_tipoOperacao, { Data: dadosObjeto });
                }

                _pesquisaTipoOperacao.ExibirFiltros.visibleFade(false);
                _CRUDTipoOperacao.Atualizar.visible(!duplicar);
                _CRUDTipoOperacao.Cancelar.visible(true);
                _CRUDTipoOperacao.Excluir.visible(!duplicar);
                _CRUDTipoOperacao.Adicionar.visible(duplicar);
                _tipoOperacaoEmissao.UsarConfiguracaoEmissao.val(_tipoOperacao.UsarConfiguracaoEmissao.val());
                _tipoOperacaoFatura.UsarConfiguracaoFaturaPorTipoOperacao.val(_tipoOperacao.UsarConfiguracaoFaturaPorTipoOperacao.val());
                InformarProdutoPredominanteOperacaoClick();
                controlarCampoGrupoTomadorHabilitado();
                controlarCampoTipoDeCargaPadraoOperacaoHabilitado();
                controlarCampoCorJanelaCarregamento();
                _configuracaoEmissaoCTe.SetarValores(arg.Data.ConfiguracaoEmissaoCTe);
                _configuracaoFatura.SetarValores(arg.Data.ConfiguracaoFatura);
                _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);
                RecarregarGridTipoOperacaoVendedores();
                preencherIntegracao(arg.Data.Integracoes);
                preencherTipoOperacaoTransportador(arg.Data.ListaTransportador);
                preencherTipoOperacaoControleColetaEntregaOcorrencia(arg.Data.ListaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega);
                preencherListaOcorrencias(arg.Data);
                preencherListaGrupoTomadoresBloqueados(arg.Data);
                preencherListaExcecoesAngelLira(arg.Data.ExcecoesAngelLira);
                preencherListaMotivosAtendimento(arg.Data);
                preencherListaMotivoAtendimentoTransportador(arg.Data);
                recarregarGridComprovante();
                RecarregarGridTiposCargas();
                RecarregarGridTipoCargaEmissao();
                RecarregarGridTipoTerceiro();
                RecarregarGridTipoTerceiroPropriedadeVeiculo();
                preencherConfiguracaoCargaEstado(arg.Data.ConfiguracaoCarga.ConfiguracoesCargaEstado);
                preencherTipoOperacaoControleEntregaSetor(arg.Data.ConfiguracaoControleEntrega.Setores);
                preencherTipoOperacaoTranportadorMontagem(arg.Data.ConfiguracaoMontagem.TransportadoresMontagem);
                preencherGridNotificacoesAppTrizy(arg.Data.NotificacoesAppTrizy, duplicar);
                preencherGridEventosSuperApp(arg.Data.EventosSuperApp, duplicar);
                preencherGridInformacaoAdicionalRelatorioViagem(arg.Data.ConfiguracaoTrizy?.InformacoesAdicionaisRelatorio ?? [], duplicar);
                preencherGridCodigosIntegracaoTipoOperacao(arg.Data.CodigosIntegracao);
                _tipoOperacao.NaoPermitirLiberarSemValePedagio.val(arg.Data.EmissaoDocumentos.NaoPermitirLiberarSemValePedagio);
                if (_configuracaoCIOT != null)
                    _configuracaoCIOT.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro.val(arg.Data.ConfiguracaoTerceiro.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro);

                EditarListarAnexos(arg);
                _comprovante.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val(arg.Data.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa);
                _comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val(arg.Data.CalculoFrete.ExigirComprovantesLiberacaoPagamentoContratoFrete);

                if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador)
                    TipoPessoaChange(_tipoOperacao);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                    _gridProdutos.CarregarGrid(arg.Data.Produtos);
                    TipoPessoaChange(_tipoOperacao);
                    $("#liSeguro").hide();
                } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    $("#liSeguro").show();

                    _seguro.TipoOperacao.val(_tipoOperacao.Codigo.val());
                    _gridSeguro.CarregarGrid();
                }
                _gridFilialMotoristaGenerico.CarregarGrid(arg.Data.FiliaisMotoristasGenericos);

                //if (arg.Data.MultiEmbarcador.UtilizaMultiEmbarcador)
                //    $("#liIntegracaoMultiEmbarcador").removeClass("d-none");
                //else
                //    $("#liIntegracaoMultiEmbarcador").addClass("d-none");

                if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador)
                    $("#liIntegracaoMultiEmbarcador").removeClass("d-none");
                else
                    $("#liIntegracaoMultiEmbarcador").addClass("d-none");

                if (_configuracaoCIOT != null)
                    _configuracaoCIOT.UtilizarConfiguracaoPersonalizadaParcelasPamcard.visible(arg.Data.TipoOperadoraCIOT === EnumOperadoraCIOT.Pamcard);

                validaEixosSuspensosVisible();

                $("#txtEditor").summernote('code', arg.Data.TermoAceite);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function limparCamposTipoOperacao() {
    if (_configuracaoCIOT != null && _configuracaoCIOT != undefined && _configuracaoCIOT.UtilizarConfiguracaoPersonalizadaParcelasPamcard != undefined) {
        _configuracaoCIOT.UtilizarConfiguracaoPersonalizadaParcelasPamcard.visible(false);
    }
    _CRUDTipoOperacao.Atualizar.visible(false);
    _CRUDTipoOperacao.Cancelar.visible(false);
    _CRUDTipoOperacao.Excluir.visible(false);
    _CRUDTipoOperacao.Adicionar.visible(true);
    _tipoOperacao.GrupoPessoas.visible(false);

    if (_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _tipoOperacao.GrupoPessoas.visible(false);
        _tipoOperacao.Pessoa.visible(true);
        _tipoOperacao.TipoPessoa.visible(true);
    }
    else {
        _tipoOperacao.GrupoPessoas.visible(false);
        _tipoOperacao.Pessoa.visible(false);
        _tipoOperacao.TipoPessoa.visible(false);
    }
    _tipoOperacaoEmissao.UsarConfiguracaoEmissao.val(_tipoOperacaoEmissao.UsarConfiguracaoEmissao.def);
    _tipoOperacaoFatura.UsarConfiguracaoFaturaPorTipoOperacao.val(_tipoOperacaoFatura.UsarConfiguracaoFaturaPorTipoOperacao.def);
    LimparCampos(_tipoOperacao);
    InformarProdutoPredominanteOperacaoClick();
    controlarCampoGrupoTomadorHabilitado();
    controlarCampoTipoDeCargaPadraoOperacaoHabilitado();
    controlarCampoCorJanelaCarregamento();
    limparCamposTipoOperacaoTransportador();
    limparCamposTipoOperacaoControleColetaEntregaOcorrencia();
    LimparSeguroTipoOperacao();
    limparCamposIntegracao();
    limparListaGrupoTomadoresBloqueados();
    limparCamposAngelLira();
    limparListaMotivosAtendimento();
    limparListaMotivosAtendimentoTransportador();
    LimparCamposTiposCargas();
    LimparCamposTipoCargaEmissao();
    limparCamposTipoOperacaoControleEntregaSetor();
    limparAnexosTela();
    limparCamposComprovante();
    limparCamposCadastroNotificacaoApp();
    limparCamposCadastroEventoSuperApp();
    limparCamposCadastroInformacaoAdicionalRelatorioViagem();
    limparTipoOperacaoVendedores();
    _gridProdutos.CarregarGrid([]);
    _gridNotificacaoApp.CarregarGrid([]);
    _gridEventoSuperApp.CarregarGrid([]);
    _gridInformacoesAdicionaisRelatorioViagemSuperApp.CarregarGrid([]);
    _configuracaoEmissaoCTe.Limpar();
    _configuracaoFatura.Limpar();
    _configuracaoLayoutEDI.Limpar();
    limparCamposTiposTerceiro();
    limparCamposTiposTerceiroPropriedadeVeiculo();
    RecarregarGridTipoTerceiro();
    RecarregarGridTipoTerceiroPropriedadeVeiculo();
    limparCamposCodigosIntegracaoTipoOperacao();

    $("#liIntegracaoMultiEmbarcador").addClass("d-none");

    Global.ResetarAbas();

    $("#txtEditor").summernote('code', '');
}

function obterTipoOperacaoSalvar() {
    _tipoOperacao.Vendedores.val(JSON.stringify(_tipoOperacao.ListVendedores.val()));
    _tipoOperacao.TermoAceite.val($('#txtEditor').summernote('code'));
    var tipoOperacao = RetornarObjetoPesquisa(_tipoOperacao);

    tipoOperacao["TiposTerceiros"] = JSON.stringify(_tipoOperacao.TiposTerceiros.val());
    tipoOperacao["TiposTerceirosPropriedadeVeiculo"] = JSON.stringify(_tipoOperacao.TiposTerceirosPropriedadeVeiculo.val());
    tipoOperacao["UsarConfiguracaoEmissao"] = _tipoOperacaoEmissao.UsarConfiguracaoEmissao.val();
    tipoOperacao["UsarConfiguracaoFaturaPorTipoOperacao"] = _tipoOperacaoFatura.UsarConfiguracaoFaturaPorTipoOperacao.val();
    tipoOperacao["Produtos"] = "[]";
    tipoOperacao["FiliaisMotoristasGenericos"] = obterMotoristaGenericoFilialTipoOperacaoSalvar();
    tipoOperacao["TiposOcorrencia"] = obterOcorrenciaSalvar();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        tipoOperacao["Produtos"] = obterProdutoSalvar();
    else
        tipoOperacao["Produtos"] = JSON.stringify([]);
    tipoOperacao["GrupoTomadoresBloqueados"] = obterGrupoTomadoresBloqueadosSalvar();
    tipoOperacao["ConfiguracaoMotivosChamados"] = obterMotivosAtendimento();
    tipoOperacao["ConfiguracaoChamadoTransportadores"] = obterMotivoAtendimentoTransportadores();

    preencherIntegracaoSalvar(tipoOperacao);
    preencherTipoOperacaoTransportadorSalvar(tipoOperacao);
    preencherTipoOperacaoControleColetaEntregaOcorrenciaSalvar(tipoOperacao);
    preencherConfiguracaoCargaEstadoSalvar(tipoOperacao);
    obterTipoOperacaoNotificacaoAppSalvar(tipoOperacao);
    obterTipoOperacaoEventoSuperAppSalvar(tipoOperacao);

    tipoOperacao["AngelLiraExcecoes"] = _configuracaoAngelLira != undefined ? JSON.stringify(_configuracaoAngelLira.Excecoes.val()) : JSON.stringify([]);
    tipoOperacao["Setores"] = obterTipoOperacaoControleEntregaSetorSalvar();
    tipoOperacao["Comprovantes"] = obterListaTipoComprovanteSalvar();
    tipoOperacao["NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa"] = _comprovante.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa.val()
    tipoOperacao["ExigirComprovantesLiberacaoPagamentoContratoFrete"] = _comprovante.ExigirComprovantesLiberacaoPagamentoContratoFrete.val()
    tipoOperacao["PesoConsideradoNaCarga"] = _tipoOperacao.PesoConsideradoNaCarga.val()
    tipoOperacao["CodigosIntegracao"] = JSON.stringify(_gridCodigosIntegracaoTipoOperacao.BuscarRegistros());

    tipoOperacao["TransportadoresMontagem"] = obterTipoOperacaoTransportadorMontagemSalvar();
    if (_configuracaoCIOT != null)
        tipoOperacao["NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro"] = _configuracaoCIOT.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro.val();
    else
        tipoOperacao["NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro"] = false;

    // tipoOperacao[""] = _tipoOperacao.HabilitarEnvioRelatorioViagemSuperApp.val();

    obterTipoOperacaoInformacaoAdicionalRelatorioViagemSuperAppSalvar(tipoOperacao);

    return tipoOperacao;
}

async function obterConfiguracaoPadraoDiariaAutomatica() {
    return new Promise((resolve) => {
        executarReST("ConfiguracaoDiariaAutomatica/ObterConfiguracao", {}, function (r) {
            if (r.Success && r.Data) {
                resolve(r.Data);
            } else {
                resolve(null)
            }
        })
    })
}

function preencherListasSelecao() {
    _tipoOperacao.TiposCargas.val(JSON.stringify(_gridTiposCargas.BuscarRegistros()));
    _tipoOperacao.TiposCargasEmissao.val(JSON.stringify(_gridTipoCargaEmissao.BuscarRegistros()));

    let listaTiposTerceiro = new Array();
    $.each(_tiposTerceiros.Terceiro.basicTable.BuscarRegistros(), function (i, tiposTerceiros) {
        listaTiposTerceiro.push({ Terceiro: tiposTerceiros });
    });
    _tipoOperacao.TiposTerceiros.val(JSON.stringify(listaTiposTerceiro));

    // Tipo propriedade veiculo
    let listaTiposPropriedadeVeiculoTiposTerceiros = new Array();
    $.each(_tiposTerceirosPropriedadeVeiculo.TerceiroPropriedadeVeiculo.basicTable.BuscarRegistros(), function (i, tiposTerceirosTiposPropriedadeVeiculo) {
        listaTiposPropriedadeVeiculoTiposTerceiros.push({ TerceiroPropriedadeVeiculo: tiposTerceirosTiposPropriedadeVeiculo });
    });
    _tipoOperacao.TiposTerceirosPropriedadeVeiculo.val(JSON.stringify(listaTiposPropriedadeVeiculoTiposTerceiros));
}

function verificarSeExiste() {
    executarReST("TipoOperacao/VerificarSeExiste", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _tipoOperacao.ExigirQueCDDestinoSejaInformadoAgendamento.visible(retorno.Data.CentroDistribuicao);
            _tipoOperacao.ExecutarPreCalculoFrete.AtivoPreCalculo(retorno.Data.ExibirPreCalculo);
            _tipoOperacao.TipoOperacaoPadraoFerroviario.visible(retorno.Data.ExibirTipoOperacaoModalFerroviario);
        }
    });
}

function obterConfiguracaoTMSGeral() {
    executarReST("TipoOperacao/ObterConfiguracaoTMSGeral", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _tipoOperacao.TipoDeEnvioPorSMSDeDocumentos.visible(retorno.Data.HabilitarEnvioPorSMSDeDocumentos);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}
function obterConfiguracaoTipoOperacaoCamposObrigatorios() {
    executarReST("TipoOperacao/ObterConfiguracaoTipoOperacao", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data && retorno.Data.length > 0) {
                _configuracoesTipoOperacaoValorPadrao = retorno.Data;
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.message || "Falha ao obter configurações.");
        }
    });
}

function preencherMotoristaGenericoFilialTipoOperacao() {
    return {
        Codigo: _tipoOperacao.FilialMotoristaGenerico.codEntity(),
        Descricao: _tipoOperacao.FilialMotoristaGenerico.val(),
        MotoristaCodigo: _tipoOperacao.MotoristaGenerico.codEntity(),
        MotoristaDescricao: _tipoOperacao.MotoristaGenerico.val(),
    };
}

function obterMotoristaGenericoFilialTipoOperacaoSalvar() {
    let listaFiliaisMotoristasGenericos = _gridFilialMotoristaGenerico.BuscarRegistros();
    let listaFiliaisMotoristasGenericosRetornar = new Array();

    for (var i = 0; i < listaFiliaisMotoristasGenericos.length; i++) {
        listaFiliaisMotoristasGenericosRetornar.push({
            Codigo: listaFiliaisMotoristasGenericos[i].Codigo,
            Descricao: listaFiliaisMotoristasGenericos[i].Descricao,
            MotoristaCodigo: listaFiliaisMotoristasGenericos[i].MotoristaCodigo,
            MotoristaDescricao: listaFiliaisMotoristasGenericos[i].MotoristaDescricao
        });
    }

    return JSON.stringify(listaFiliaisMotoristasGenericosRetornar);
}
function CarregarCamposIntegracaoTrizy() {
    if (_CONFIGURACAO_TMS.UtilizaAppTrizy) {
        $("#liTabAppTrizy").show();
        versaoTrizy = _CONFIGURACAO_TMS.ValidarIntegracaoPorOperacao ? (_tipoOperacao?.VersaoIntegracaoTrizy?.val() ?? 1) : _CONFIGURACAO_TMS.VersaoIntegracaoTrizy;
        if (versaoTrizy == 1) {
            // inativa v3
            _tipoOperacao.VerCadEvento.visible(false);
            _tipoOperacao.VerCadRelatorioViagem.visible(false);
            _tipoOperacao.NaoEnviarPolilinha.visible(false);
            _tipoOperacao.HabilitarDevolucao.visible(false);
            _tipoOperacao.HabilitarDevolucaoParcial.visible(false);
            // ativa v1
            _tipoOperacao.SolicitarDataeHoraDoCanhoto.visible(true);
            _tipoOperacao.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega.visible(true);
            _tipoOperacao.EnviarEstouIndoColeta.visible(true);
            _tipoOperacao.EnviarEstouIndoEntrega.visible(true);
            _tipoOperacao.EnviarInicioViagemColeta.visible(true);
            _tipoOperacao.EnviarInicioViagemEntrega.visible(true);
            _tipoOperacao.EnviarChegueiParaCarregar.visible(true);
            _tipoOperacao.SolicitarComprovanteColetaEntrega.visible(true);
            _tipoOperacao.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega.visible(true);
            _tipoOperacao.SolicitarDocumentoNaConfirmacaoDeColetaEntrega.visible(true);
            _tipoOperacao.ExigirInformarNumeroPacotesNaColetaTrizy.visible(true);
            _tipoOperacao.SolicitarFotoComoEvidenciaOpcional.visible(true);
            _tipoOperacao.SolicitarFotoComoEvidenciaObrigatoria.visible(true);
            _tipoOperacao.NaoEnviarTagValidacao.visible(true);
            _tipoOperacao.NaoPermitirVincularFotosDaGaleriaParaCanhotos.visible(true);
            _tipoOperacao.EnviarEstouIndoColetaPreTrip.visible(true);
            _tipoOperacao.EnviarEstouIndoEntregaPreTrip.visible(true);
            _tipoOperacao.EnviarIniciarViagemPreTrip.visible(true);
            _tipoOperacao.EnviarChegueiParaCarregarPreTrip.visible(true);
        } else {
            // inativa v1
            _tipoOperacao.SolicitarDataeHoraDoCanhoto.visible(false);
            _tipoOperacao.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega.visible(false);
            _tipoOperacao.EnviarEstouIndoColeta.visible(false);
            _tipoOperacao.EnviarEstouIndoEntrega.visible(false);
            _tipoOperacao.EnviarInicioViagemColeta.visible(false);
            _tipoOperacao.EnviarInicioViagemEntrega.visible(false);
            _tipoOperacao.EnviarChegueiParaCarregar.visible(false);
            _tipoOperacao.SolicitarComprovanteColetaEntrega.visible(false);
            _tipoOperacao.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega.visible(false);
            _tipoOperacao.SolicitarDocumentoNaConfirmacaoDeColetaEntrega.visible(false);
            _tipoOperacao.ExigirInformarNumeroPacotesNaColetaTrizy.visible(false);
            _tipoOperacao.SolicitarFotoComoEvidenciaOpcional.visible(false);
            _tipoOperacao.SolicitarFotoComoEvidenciaObrigatoria.visible(false);
            _tipoOperacao.NaoEnviarTagValidacao.visible(false);
            _tipoOperacao.NaoPermitirVincularFotosDaGaleriaParaCanhotos.visible(false);
            _tipoOperacao.EnviarEstouIndoColetaPreTrip.visible(false);
            _tipoOperacao.EnviarEstouIndoEntregaPreTrip.visible(false);
            _tipoOperacao.EnviarIniciarViagemPreTrip.visible(false);
            _tipoOperacao.EnviarChegueiParaCarregarPreTrip.visible(false);
            // ativa v3
            _tipoOperacao.VerCadEvento.visible(true);
            _tipoOperacao.VerCadRelatorioViagem.visible(true);
            _tipoOperacao.NaoEnviarPolilinha.visible(true);
            _tipoOperacao.HabilitarDevolucao.visible(true);
            _tipoOperacao.HabilitarDevolucaoParcial.visible(true);
        }
    }
}
// #endregion Funções Privadas