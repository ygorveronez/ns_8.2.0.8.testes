/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TipoEmbalagem.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/ContratoTransporteFrete.js" />
/// <reference path="../../Enumeradores/EnumAplicacaoTabela.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoAlteracaoValorTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroBaseTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTiposFreeTime.js" />
/// <reference path="Ajudante.js" />
/// <reference path="ComponenteFrete.js" />
/// <reference path="Distancia.js" />
/// <reference path="Filiais.js" />
/// <reference path="ModeloReboque.js" />
/// <reference path="ModeloTracao.js" />
/// <reference path="NumeroEntrega.js" />
/// <reference path="Pacotes.js" />
/// <reference path="Pallet.js" />
/// <reference path="PesoTransportado.js" />
/// <reference path="RotaEmbarcadorTabelaFrete.js" />
/// <reference path="../TabelaFrete/Subcontratacao.js" />
/// <reference path="Tempo.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="TipoOcorrencia.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="TipoEmbalagem.js" />
/// <reference path="Transportadores.js" />
/// <reference path="TransportadorTerceiro.js" />
/// <reference path="TipoTerceiro.js" />
/// <reference path="Veiculo.js" />
/// <reference path="Vigencia.js" />
/// <reference path="Reboque.js" />

// #region Objetos Globais do Arquivo

var _crudTabelaFrete;
var _gridTabelaFrete;
var _pesquisaTabelaFrete;
var _tabelaFrete;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDTabelaFrete = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Cancelar, visible: ko.observable(false) });
};

var PesquisaTabelaFrete = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Descricao.getFieldDescription() });

    var visivelTipoTabelaFrete = (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS);

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Transportador.getFieldDescription()), visible: ko.observable(visivelTipoTabelaFrete), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Filial.getFieldDescription()), visible: ko.observable(visivelTipoTabelaFrete), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.TipoDeCarga.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.TipoDeOcorrencia.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Fretes.TabelaFrete.Veiculo.getFieldDescription(), visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.TipoDeOperacao.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoEmbalagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.TipoDeEmbalagem.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoTabelaFrete = PropertyEntity({ val: ko.observable(0), visible: visivelTipoTabelaFrete, options: EnumTipoTabelaFrete.obterOpcoesTipoTabelaFretePesquisa(), text: Localization.Resources.Fretes.TabelaFrete.TipoDeTabelaDeFrete.getFieldDescription(), def: 0 });
    this.AplicacaoTabela = PropertyEntity({ val: ko.observable(EnumAplicacaoTabela.Todas), visible: ko.observable(true), options: EnumAplicacaoTabela.obterOpcoesPesquisa(), text: Localization.Resources.Fretes.TabelaFrete.AplicacaoDaTabela.getFieldDescription(), def: EnumAplicacaoTabela.Todas });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Fretes.TabelaFrete.Situacao.getFieldDescription() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Codigo.getFieldDescription(), issue: 15 });
    this.SituacaoAlteracao = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoTabelaFrete.Todas), options: EnumSituacaoAlteracaoTabelaFrete.obterOpcoesPesquisaTabelaFrete(), def: EnumSituacaoAlteracaoTabelaFrete.Todas, text: Localization.Resources.Fretes.TabelaFrete.SituacaoAprovacao.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFrete.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
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
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TabelaFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Descricao.getRequiredFieldDescription(), issue: 586, required: true, CssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-6"), val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Codigo.getFieldDescription(), required: false, maxlength: 50, visible: ko.observable(false), issue: 15 });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Observacao.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(false), issue: 76 });
    this.ObservacaoTerceiro = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ObservacaoParaTerceiro.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(false), issue: 76 });

    this.AplicacaoTabela = PropertyEntity({ val: ko.observable(EnumAplicacaoTabela.Carga), visible: ko.observable(false), options: EnumAplicacaoTabela.obterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.AplicacaoDaTabela.getFieldDescription(), def: EnumAplicacaoTabela.Carga });
    this.AplicacaoTabela.val.subscribe(aplicacaoTabelaSubscribe);
    var visivelTipoTabelaFrete = true;
    var tipoTabelaPadrao = EnumTipoTabelaFrete.tabelaRota;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        visivelTipoTabelaFrete = false;
        tipoTabelaPadrao = EnumTipoTabelaFrete.tabelaCliente;
    }

    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.TipoDeOperacao.getFieldDescription()), issue: 121, visible: ko.observable(false), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Filial.getFieldDescription()), visible: ko.observable(false), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.Transportador.getFieldDescription()), visible: ko.observable(false), idBtnSearch: guid() });
    this.TipoTabelaFrete = PropertyEntity({ val: ko.observable(tipoTabelaPadrao), options: EnumTipoTabelaFrete.obterOpcoesTipoTabelaFrete(), visible: visivelTipoTabelaFrete, text: Localization.Resources.Fretes.TabelaFrete.TipoDeTabelaDeFrete.getRequiredFieldDescription(), issue: 690, def: tipoTabelaPadrao });
    this.TipoCalculo = PropertyEntity({ val: ko.observable(EnumTipoCalculoTabelaFrete.PorCarga), options: EnumTipoCalculoTabelaFrete.obterOpcoes(), visible: ko.observable(false), text: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculo.getRequiredFieldDescription(), issue: 1448, def: EnumTipoCalculoTabelaFrete.PorCarga });
    this.ParametroBase = PropertyEntity({ val: ko.observable(EnumTipoParametroBaseTabelaFrete.Nenhum), options: EnumTipoParametroBaseTabelaFrete.obterOpcoesComNenhum(), def: EnumTipoParametroBaseTabelaFrete.Nenhum, text: Localization.Resources.Fretes.TabelaFrete.ParametroBase.getFieldDescription(), visible: ko.observable(false), issue: 126 });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFrete.GrupoDePessoas.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false), issue: 58 });
    this.TipoFreteTabelaFrete = PropertyEntity({ val: ko.observable(EnumTipoFreteTabelaFrete.NaoInformado), options: EnumTipoFreteTabelaFrete.obterOpcoes(), def: EnumTipoFreteTabelaFrete.NaoInformado, text: Localization.Resources.Fretes.TabelaFrete.TipoDeFrete.getFieldDescription(), visible: ko.observable(false), issue: 126 });
    this.LocalFreeTime = PropertyEntity({ val: ko.observable(EnumTiposFreeTime.Nenhum), options: EnumTiposFreeTime.obterOpcoes(), def: EnumTiposFreeTime.Nenhum, text: Localization.Resources.Fretes.TabelaFrete.TipoFreeTime.getFieldDescription(), visible: ko.observable(false) });

    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Fretes.TabelaFrete.ContratoDeFrete.getFieldDescription(), issue: 1450, idBtnSearch: guid(), visible: ko.observable(false) });
    this.ContratoFreteTransportador.codEntity.subscribe(ContratoFreteTransportadorAlterado);
    this.ContratoFreteTransportadorPossuiFranquia = PropertyEntity({ val: ko.observable(false), def: false });
    this.NaoPermitirLancarValorPorTipoDeCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidarPorDataCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirVigenciasSobrepostas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.UsarComoDataBaseVigenciaDataAtual = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.ContratoFreteCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Fretes.TabelaFrete.ContratoFreteCliente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Fretes.TabelaFrete.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.ContratoFreteTransportadorPossuiFranquia.val.subscribe(PossuiFranquiaAlterado);
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Fretes.TabelaFrete.Situacao.getRequiredFieldDescription(), issue: 557, CssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-3") });
    this.PermiteAlterarValor = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermiteAlterarOValor, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false), issue: 75 });
    this.TipoAlteracaoValor = PropertyEntity({ val: ko.observable(0), options: EnumTipoAlteracaoValorTabelaFrete.obterOpcoes(), text: Localization.Resources.Fretes.TabelaFrete.PermiteAlterar.getRequiredFieldDescription(), visible: ko.observable(false) });
    this.ImprimirObservacaoCTe = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ImprimirObservacaoCTe, issue: 695, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PossuiMinimoGarantido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ATabelaPodeTerValorMinimoGarantido, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PossuiValorMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ATabelaPodeConterUmValorMaximo, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PossuiValorBase = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ATabelaPodeConterUmValorBase, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizarDiferencaDoValorBaseApenasFretePagos = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarDiferencaDoMaiorValorBaseEntrePedidosApenasParaFretesPagos, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ValorMinimoDiferencaFreteNegativo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorMinimoSeDiferencaDoMaiorEntrePedidosForNegativo.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 2, val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), visible: ko.observable(false) });

    this.TabelaFreteMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TabelaDeFreteMinima, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TabelaFreteMinima.val.subscribe(alterouTabelaFreteMinima);
    this.EmitirOcorrenciaAutomatica = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EmitirOcorrenciaAutomaticamente, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EmitirOcorrenciaAutomatica.val.subscribe(alterouEmitirOcorrenciaAutomatica);
    this.TipoOcorrenciaTabelaMinima = PropertyEntity({ visible: ko.observable(false), type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: ko.observable(false), text: Localization.Resources.Fretes.TabelaFrete.TipoDeOcorrencia.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 410 });
    this.UtilizaTabelaFreteMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermiteUtilizarTabelaDeFreteMinima, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizaModeloVeicularVeiculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarModeloVeicularParaCalculo, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.MultiplicarValorDaFaixa = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MultiplicarValorDaFaixaPelaQuantidade, issue: 1451, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EmissaoAutomaticaCTe = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EmissaoAutomaticaDosCTes, issue: 696, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.IncluirICMSValorFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.IncluirIcmsNoValorDoFrete, issue: 698, val: ko.observable(true), def: true, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizarValorMinimoParaRateio = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarValorMinimoParaRateio, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.CalcularFreteDestinoPrioritario = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CalcularFretePorDestinoPrioritario, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ValorParametroBaseObrigatorioParaCalculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorDoParametroBaseObrigatorioParaCalculoDoFrete, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PercentualICMSIncluir = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PercentualDeInclusao.getRequiredFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 6, val: ko.observable(Globalize.format(100, "n2")), def: Globalize.format(100, "n2"), visible: ko.observable(false) });
    this.ValorMinimoParaRateio = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorMinimoParaRateio.getRequiredFieldDescription(), getType: typesKnockout.decimal, required: function () { return this.UtilizarValorMinimoParaRateio.visible() }.bind(this), maxlength: 6, val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2") });
    this.Padrao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TabelaDeFretePadrao, issue: 720, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PagamentoTerceiro = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EstaTabelaUtilizadaParaPagementoDeTerceiros, issue: 1532, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ObrigatorioInformarTerceiro = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ObrigatorioInformarTerceiroNaTabela, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizarParticipantePedidoParaCalculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarRemetenteDoPedidoParaCalculoFrete, issue: 2019, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarModeloVeicularDaCargaParaCalculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarModeloVeicularDaCargaParaCalculo, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoConsiderarExpedidorERecebedor = PropertyEntity({ text: "Não considerar expedidor e recebedor para cálculo de frete", val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador = PropertyEntity({ text: "Utilizar valor da tabela mesmo informando um valor de frete operador", val: ko.observable(true), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.CalcularQuantidadeEntregaPorParticipantesPedido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarParticipantesPedidoCalculoQuantidadeEntregas, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TabelaCalculoCliente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TabelaParaCalculoFreteCliente, issue: 720, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.CalcularFreteCliente) });
    this.PermiteInformarDiasUteisPorFaixaCEP = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PermiteInformarDiasUteisDeEntregaPorFaixaDeCep, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizarTipoOperacaoPedido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarTipoDeOperacaoDosPedidosParaCalculo, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NaoCalculaSemFreteValor = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NaoCalculaSemFreteValor, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.IncluirICMSValorFreteNaCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.IncluirICMSValorFreteNaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoDestacarResultadoConsultaPedagioComoComponente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NaoDestacarResultadoConsultaPedagioComoComponente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.PermiteAlterarValorFretePedidoPosCalculoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.permiteAlterarValorFretePedidoPosCalculoFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NaoUsarCanalEntregaComoFiltroParaCotacao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });



    this.DiasVencimentoAdiantamentoContratoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DiasVencimentoAdiantamento.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(false), issue: 0 });
    this.DiasVencimentoSaldoContratoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DiasVencimentoSaldo.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(false), issue: 0 });
    this.ReterImpostosContratoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ReterImpostosContratoFrete, issue: 0, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TextoAdicionalContratoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TextoAdicionalParaContratoFrete.getFieldDescription(), required: false, maxlength: 100000, visible: ko.observable(false), issue: 0 });
    this.ObservacaoContratoFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ObservacaoParaContratoDeFrete.getFieldDescription(), required: false, maxlength: 400, visible: ko.observable(false), issue: 0 });
    this.TagContratoFreteValorTotal = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#ValorTotal"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.ValorTotal });
    this.TagContratoFretePercentualAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#PercentualAdiantamento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemAdiantamento });
    this.TagContratoFreteValorAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#ValorAdiantamento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.ValorAdiantamento });
    this.TagContratoFretePercentualAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#PercentualAbastecimento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemAbastecimento });
    this.TagContratoFreteValorAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#ValorAbastecimento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.ValorAbastecimento });
    this.TagContratoFreteSaldoReceber = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#SaldoReceber"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.SaldoAReceber });
    this.TagContratoFreteVencimentoAdiantamento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#VencimentoAdiantamento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.VencimentoAdiantamento });
    this.TagContratoFreteVencimentoSaldo = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#VencimentoSaldo"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.VencimentoSaldo });
    this.TagContratoFreteOperadoraValePedagio = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#OperadoraValePedagio"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.OperadoraValePedagio });
    this.TagContratoFreteCartaoAbastecimento = PropertyEntity({ eventClick: function (e) { InserirTag(_tabelaFrete.ObservacaoContratoFrete.id, "#CartaoAbastecimento"); }, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.CartaoAbastecimento });

    this.TiposDeOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoEmbalagens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ModelosTracao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ModelosReboque = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ComponentesFrete = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.NumeroEntregas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Pacotes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Pallets = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Tempos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Ajudantes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Horas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.PesosTransportados = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Distancias = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.RotasFreteEmbarcador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Subcontratacoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.TransportadoresTerceiros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposTerceiros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Fronteiras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Contratos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Reboques = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Tracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Veiculo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PercentualCobrancaPadrao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, maxlength: ko.observable(6), required: false, requiredClass: ko.observable("") });
    this.PercentualCobrancaVeiculoFrota = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, maxlength: ko.observable(6), required: false, requiredClass: ko.observable("") });

    this.ComponenteFreteQuilometragemExcedente = PropertyEntity({});
    this.UtilizarComponenteFreteQuilometragemExcedente = PropertyEntity({});

    this.UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga = PropertyEntity({});

    this.ComponenteFreteQuilometragem = PropertyEntity({});
    this.UtilizarComponenteFreteQuilometragem = PropertyEntity({});

    this.ComponenteFretePeso = PropertyEntity({});
    this.UtilizarComponenteFretePeso = PropertyEntity({});

    this.ComponenteFreteAjudante = PropertyEntity({});
    this.UtilizarComponenteFreteAjudante = PropertyEntity({});

    this.ComponenteFreteHora = PropertyEntity({});
    this.UtilizarComponenteFreteHora = PropertyEntity({});

    this.ComponenteFreteTempo = PropertyEntity({});
    this.UtilizarComponenteFreteTempo = PropertyEntity({});

    this.ComponenteFretePallet = PropertyEntity({});
    this.UtilizarComponenteFretePallet = PropertyEntity({});

    this.ComponenteFreteNumeroEntregas = PropertyEntity({});
    this.UtilizarComponenteFreteNumeroEntregas = PropertyEntity({});

    this.ComponenteFretePacotes = PropertyEntity({});
    this.UtilizarComponenteFretePacotes = PropertyEntity({});

    this.CalcularFatorPesoPelaKM = PropertyEntity({});
    this.MultiplicarValorPorPallet = PropertyEntity({});

    this.ComponenteFreteDestacar = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DestacarComponente, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.DestacarComponenteFrete = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(!!this.ComponenteFreteDestacar) });
    this.NaoSomarValorTotalAReceber = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorDesteComponenteNaoDeveSerSomadoAoValorAReceber, getType: typesKnockout.bool, val: ko.observable(false), def: false, issue: 80001 });
    this.NaoSomarValorTotalPrestacao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorDesteComponenteNaoDeveSerSomadoAoValorTotalDaPrestacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, issue: 80002 });
    this.DescontarComponenteFreteLiquido = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorDesteComponenteDeveSerDescontadoDoValorLiquidoDoFrete, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DescontarDoValorAReceberOICMSDoComponente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DescontarDoValorAReceberOICMSDoComponente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DescontarDoValorAReceberValorComponente = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.DescontarDoValorAReceberOValorDoComponente, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS, getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.NaoDestacarResultadoConsultaPedagioComoComponente.val.subscribe(novoValor => {
        if (novoValor) {
            _tabelaFrete.DestacarComponenteFrete.val(false);
            _tabelaFrete.DestacarComponenteFrete.enable(false);

            return;
        }

        _tabelaFrete.DestacarComponenteFrete.enable(true);
    });

    this.TipoTabelaFrete.val.subscribe(function (novoValor) {
        alterarTipoTabelaFrete(novoValor);
    });

    this.TipoCalculo.val.subscribe(function (novoValor) {
        ValidarTipoCalculoChange(novoValor);
    });

    //this.ParametroBase.val.subscribe(function (novoValor) {
    //    if (_pesoTransportado.Tipo.val() == EnumTipoPesoTransportado.PorFaixaPesoTransportado)
    //        _pesoTransportado.ModeloVeicularCarga.visible(novoValor == EnumTipoParametroBaseTabelaFrete.Peso || novoValor == EnumTipoParametroBaseTabelaFrete.Nenhum);
    //});

    this.UtilizarDiferencaDoValorBaseApenasFretePagos.val.subscribe(function (novoValor) {
        _pesoTransportado.ParaCalcularValorBase.visible(visibilidadeParaValorFreteBase());
        _tabelaFrete.PesosTransportados.list = new Array();
        CarregarGridDadosTransporte();
    });

    this.PagamentoTerceiro.val.subscribe(function (novoValor) {

        _tabelaFrete.ObservacaoContratoFrete.visible(novoValor);
        _tabelaFrete.TextoAdicionalContratoFrete.visible(novoValor);
        _tabelaFrete.ReterImpostosContratoFrete.visible(novoValor);
        _tabelaFrete.DiasVencimentoAdiantamentoContratoFrete.visible(novoValor);
        _tabelaFrete.DiasVencimentoSaldoContratoFrete.visible(novoValor);
        _tabelaFrete.ObrigatorioInformarTerceiro.visible(novoValor);
        _componenteFrete.Justificativa.visible(novoValor);

        if (novoValor) {
            _tabelaFrete.Observacao.visible(false);
            _tabelaFrete.ObservacaoTerceiro.visible(false);
            $("#liTabTiposTerceiros").show();
        } else {
            _tabelaFrete.Observacao.visible(true);
            _tabelaFrete.ObservacaoTerceiro.visible(true);
            _tabelaFrete.ObrigatorioInformarTerceiro.val(false);
            $("#liTabTiposTerceiros").hide();
        }

        $("#tabDadosGeraisObservacoes a:visible:eq(0)").tab("show");
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadTabelaFrete() {
    _tabelaFrete = new TabelaFrete();

    var configuracaoObservacaoCTe = new ConfiguracaoObservacaoCTe();

    var parametrosConfiguracaoObservacaoCTe = {
        Knouckout: _tabelaFrete,
        KnoutObservacao: _tabelaFrete.Observacao,
        KnoutObservacaoTerceiro: _tabelaFrete.ObservacaoTerceiro,
        IdContainerObservacao: "divContainerObservacao",
        IdContainerObservacaoTerceiro: "divContainerObservacaoTerceiro"
    };

    configuracaoObservacaoCTe.Load(parametrosConfiguracaoObservacaoCTe).then(function () {

        KoBindings(_tabelaFrete, "knockoutDetalhes");

        _crudTabelaFrete = new CRUDTabelaFrete();
        KoBindings(_crudTabelaFrete, "knockoutCRUDTabelaFrete");

        _pesquisaTabelaFrete = new PesquisaTabelaFrete();
        KoBindings(_pesquisaTabelaFrete, "knockoutPesquisaTabelaFrete", false, _pesquisaTabelaFrete.Pesquisar.id);

        HeaderAuditoria("TabelaFrete", _tabelaFrete);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            alterarTipoTabelaFrete(EnumTipoTabelaFrete.tabelaCliente);

            _tabelaFrete.PagamentoTerceiro.visible(true);

            _tabelaFrete.CalcularQuantidadeEntregaPorParticipantesPedido.visible(true);
            _tabelaFrete.TipoFreteTabelaFrete.visible(false);

        }
        else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _tabelaFrete.Descricao.CssClass("col col-xs-12 col-sm-12 col-md-10 col-lg-4");
            _tabelaFrete.Ativo.CssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
            _tabelaFrete.GrupoPessoas.text("Grupo de Pessoas:");
            _tabelaFrete.GrupoPessoas.required = false;
            _tabelaFrete.Padrao.visible(true);

            $("#liTabRegrasPedidoComponenteCalculado").hide();
        }

        if (!_CONFIGURACAO_TMS.ExisteIntegracaoLoggi)
            $("#liTabPacotes").hide();

        $("#" + _tabelaFrete.IncluirICMSValorFrete.id).click(function () {
            _tabelaFrete.PercentualICMSIncluir.required = _tabelaFrete.IncluirICMSValorFrete.val();
        });

        BuscarGruposPessoas(_tabelaFrete.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        BuscarGruposPessoas(_pesquisaTabelaFrete.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        BuscarTiposdeCarga(_pesquisaTabelaFrete.TipoCarga);
        BuscarTipoOcorrencia(_pesquisaTabelaFrete.TipoOcorrencia);
        BuscarVeiculos(_pesquisaTabelaFrete.Veiculo);
        BuscarTransportadores(_pesquisaTabelaFrete.Empresa);
        BuscarTipoEmbalagem(_pesquisaTabelaFrete.TipoEmbalagem);
        BuscarFilial(_pesquisaTabelaFrete.Filial);
        BuscarTiposOperacao(_tabelaFrete.TipoOperacao, null, _tabelaFrete.GrupoPessoas);
        BuscarTiposOperacao(_pesquisaTabelaFrete.TipoOperacao, null, _pesquisaTabelaFrete.GrupoPessoas);
        BuscarContratoFreteTransportador(_tabelaFrete.ContratoFreteTransportador, RetornoContratoFreteTransportador);
        BuscarContratoFreteCliente(_tabelaFrete.ContratoFreteCliente)
        BuscarTipoOcorrencia(_tabelaFrete.TipoOcorrenciaTabelaMinima, null, null, ["", EnumFinalidadeTipoOcorrencia.Valor]);
        BuscarCanaisEntrega(_tabelaFrete.CanalEntrega);
        BuscarComponentesDeFrete(_tabelaFrete.ComponenteFreteDestacar);

        LimparComponentePorFlag(_tabelaFrete.ComponenteFreteDestacar, _tabelaFrete.DestacarComponenteFrete);

        buscarTabelaFretes();

        loadTransportador();
        loadTipoEmbalagem();
        loadFilial();
        loadTipoOperacao();
        loadTipoOcorrencia();
        loadTipoCarga();
        _tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga = _tipoCargaGeral.NaoPermitirLancarValorPorTipoDeCarga;
        loadModeloTracao();
        loadModeloReboque();
        loadVigencia();
        loadComponenteFrete();
        loadNumeroEntrega();
        loadPallet();
        loadPesoTransportado();
        loadDistancia();
        loadSubcontratacao();
        loadRotaEmbarcadorTabelaFrete();
        LoadTempo();
        LoadAjudante();
        LoadHora();
        LoadTransportadoresTerceiros();
        LoadTiposTerceiros();
        loadAnexo();
        LoadFronteira();
        loadTabelaFreteTracao();
        loadTabelaFreteReboque();
        LoadContrato();
        loadPacotes();
        loadTabelaFreteIntegracoes();
        limparCamposTabelaFrete();

        verificarSeExisteRegraCotacao();

        preencherTabelaFreteAutomaticamente();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatoriosTabelaFrete())
        return;

    var tabelaFrete = obterTabelaFreteSalvar();

    console.log(tabelaFrete);

    executarReST("TabelaFrete/Adicionar", tabelaFrete, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.CadastradoComSucesso);
                enviarArquivosAnexados(retorno.Data.Codigo);
                _gridTabelaFrete.CarregarGrid();
                limparCamposTabelaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
    });
}

function aplicacaoTabelaSubscribe(val) {
    if (_tabelaFrete.TipoTabelaFrete.val() === EnumTipoTabelaFrete.tabelaCliente) {
        if (val !== EnumAplicacaoTabela.Carga) {
            $("#liTabTipoOcorrencia").show();
            $("#liTabHora").show();
        }
        else {
            $("#liTabHora").hide();
            $("#liTabTipoOcorrencia").hide();
        }

    }
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatoriosTabelaFrete())
        return;

    var tabelaFrete = obterTabelaFreteSalvar();

    executarReST("TabelaFrete/Atualizar", tabelaFrete, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.AtualizadoComSucesso);
                _gridTabelaFrete.CarregarGrid();
                limparCamposTabelaFrete();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
    });
}

function cancelarClick(e) {
    limparCamposTabelaFrete();
}

function DuplicarTabelaFreteClick(tabelaFreteGrid) {
    editarTabelaFrete(tabelaFreteGrid, true);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Fretes.TabelaFrete.Confirmacao, Localization.Resources.Fretes.TabelaFrete.RealmenteDesejaExcluirTabelaFreteX.format(_tabelaFrete.Descricao.val()), function () {
        ExcluirPorCodigo(_tabelaFrete, "TabelaFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Fretes.TabelaFrete.ExcluidoComSucesso);
                    _gridTabelaFrete.CarregarGrid();
                    limparCamposTabelaFrete();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, arg.Msg);
            }
        }, null);
    });
}

function integrarAlteracaoTabelaFreteClick(registroSelecionado) {
    executarReST("TabelaFreteIntegracao/AdicionarIntegracoes", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Fretes.TabelaFrete.Sucesso, Localization.Resources.Gerais.Geral.IntegradoComSucesso);
                _gridTabelaFrete.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Fretes.TabelaFrete.Falha, retorno.Msg);
    });
}

function ValidarTipoCalculoChange() {
    var tipoCalculo = _tabelaFrete.TipoCalculo.val();
    if (tipoCalculo === EnumTipoCalculoTabelaFrete.PorPedido) {
        _componenteFrete.ValorUnicoParaCarga.visible(true);
        _tabelaFrete.UtilizarTipoOperacaoPedido.visible(true);
    } else {
        _componenteFrete.ValorInformadoNaTabela.enable(true);
        _componenteFrete.ValorUnicoParaCarga.visible(false);
        _tabelaFrete.UtilizarTipoOperacaoPedido.visible(false);
    }

    if (tipoCalculo === EnumTipoCalculoTabelaFrete.PorCarga) {
        _numeroEntregaDadosGerais.CalcularQuantidadeEntregaPorNumeroDePedidos.visible(true);
    } else {
        _numeroEntregaDadosGerais.CalcularQuantidadeEntregaPorNumeroDePedidos.val(false);
        _numeroEntregaDadosGerais.CalcularQuantidadeEntregaPorNumeroDePedidos.visible(false);
    }

    if (tipoCalculo == EnumTipoCalculoTabelaFrete.PorPedidosAgrupados) {
        _tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado.visible(true);
    } else {
        _tabelaFrete.AgrupaPorRecebedorAoCalcularPorPedidoAgrupado.visible(true);
    }

    ControleExibicaoUtilizarValorMinimoParaRateio();
    ControlePermiteAlterarValorFretePedidoPosCalculoFrete();
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function alterarTipoTabelaFrete(tipo) {
    if (tipo == EnumTipoTabelaFrete.tabelaCliente) {
        _tabelaFrete.Ativo.CssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
        _tabelaFrete.CodigoIntegracao.visible(true);
        _tabelaFrete.Observacao.visible(true);
        _tabelaFrete.ObservacaoTerceiro.visible(true);
        _tabelaFrete.GrupoPessoas.visible(true);
        _tabelaFrete.ContratoFreteTransportador.visible(true);
        _tabelaFrete.ContratoFreteCliente.visible(true);
        _tabelaFrete.PermiteAlterarValor.visible(true);
        _tabelaFrete.TipoAlteracaoValor.visible(true);
        _tabelaFrete.ImprimirObservacaoCTe.visible(true);
        _tabelaFrete.PossuiValorMaximo.visible(true);
        _tabelaFrete.PossuiValorBase.visible(true);
        _tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos.visible(true);
        _tabelaFrete.CalcularFreteDestinoPrioritario.visible(true);
        _tabelaFrete.ValorParametroBaseObrigatorioParaCalculo.visible(true);
        _tabelaFrete.PossuiMinimoGarantido.visible(true);
        _tabelaFrete.EmissaoAutomaticaCTe.visible(true);
        _tabelaFrete.IncluirICMSValorFrete.visible(true);
        _tabelaFrete.NaoCalculaSemFreteValor.visible(true);
        _tabelaFrete.PercentualICMSIncluir.visible(true);
        _tabelaFrete.ParametroBase.visible(true);
        _tabelaFrete.TipoCalculo.visible(true);
        _tabelaFrete.AplicacaoTabela.visible(true);
        _tabelaFrete.MultiplicarValorDaFaixa.visible(true);
        _tabelaFrete.TabelaFreteMinima.visible(true);
        _tabelaFrete.UtilizaTabelaFreteMinima.visible(true);
        _tabelaFrete.UtilizaTabelaFreteMinima.visible(true);
        _tabelaFrete.UtilizaModeloVeicularVeiculo.visible(true);
        _tabelaFrete.TipoFreteTabelaFrete.visible(true);
        _tabelaFrete.CanalEntrega.visible(true);
        _tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP.visible(true);
        _tabelaFrete.LocalFreeTime.visible(true);
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.visible(false);
        _tabelaFrete.DestacarComponenteFrete.visible(true);
        _tabelaFrete.ComponenteFreteDestacar.visible(true);

        $("#liTabTipoCarga").show();
        $("#liTabModeloTracao").show();
        $("#liTabModeloReboque").show();
        $("#liTabComponente").show();
        $("#liTabNumeroEntrega").show();
        $("#liTabPeso").show();
        $("#liTabDistancia").show();
        $("#liTabSubcontratacao").show();
        $("#liTabRotas").show();
        $("#liTabPallet").show();
        $("#liTabTipoOperacao").show();
        $("#liTabTipoEmbalagem").show();

        if (_tabelaFrete.AplicacaoTabela.val() !== EnumAplicacaoTabela.Carga) {
            $("#liTabHora").show();
            $("#liTabTipoOcorrencia").show();
        }


        $("#liTabTempo").show();
        $("#liTabAjudante").show();
        $("#liTabTransportadoresTerceiros").show();

        if (_tabelaFrete.ContratoFreteTransportador.codEntity() <= 0)
            $("#liTabVigencia").show();

    }
    else if (tipo == EnumTipoTabelaFrete.tabelaComissaoProduto) {
        _tabelaFrete.Ativo.CssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
        _tabelaFrete.CodigoIntegracao.visible(false);
        _tabelaFrete.Observacao.visible(false);
        _tabelaFrete.ObservacaoTerceiro.visible(false);
        _tabelaFrete.GrupoPessoas.visible(false);
        _tabelaFrete.ContratoFreteTransportador.visible(false);
        _tabelaFrete.ContratoFreteCliente.visible(false);
        _tabelaFrete.PermiteAlterarValor.visible(false);
        _tabelaFrete.TipoAlteracaoValor.visible(false);
        _tabelaFrete.ImprimirObservacaoCTe.visible(false);
        _tabelaFrete.PossuiValorMaximo.visible(false);
        _tabelaFrete.PossuiValorBase.visible(false);
        _tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos.visible(false);
        _tabelaFrete.AplicacaoTabela.visible(false);
        _tabelaFrete.CalcularFreteDestinoPrioritario.visible(false);
        _tabelaFrete.ValorParametroBaseObrigatorioParaCalculo.visible(false);
        _tabelaFrete.PossuiMinimoGarantido.visible(false);
        _tabelaFrete.EmissaoAutomaticaCTe.visible(false);
        _tabelaFrete.IncluirICMSValorFrete.visible(false);
        _tabelaFrete.NaoCalculaSemFreteValor.visible(false);
        _tabelaFrete.PercentualICMSIncluir.visible(false);
        _tabelaFrete.ParametroBase.visible(false);
        _tabelaFrete.GrupoPessoas.required = false;
        _tabelaFrete.TipoCalculo.visible(false);
        _tabelaFrete.MultiplicarValorDaFaixa.visible(false);
        _tabelaFrete.TabelaFreteMinima.visible(false);
        _tabelaFrete.UtilizaTabelaFreteMinima.visible(false);
        _tabelaFrete.UtilizaModeloVeicularVeiculo.visible(false);
        _tabelaFrete.TipoFreteTabelaFrete.visible(false);
        _tabelaFrete.CanalEntrega.visible(false);
        _tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP.visible(false);
        _tabelaFrete.LocalFreeTime.visible(false);
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.visible(false);
        _tabelaFrete.DestacarComponenteFrete.visible(false);
        _tabelaFrete.ComponenteFreteDestacar.visible(false);

        $("#liTabTipoEmbalagem").hide();
        $("#liTabTipoCarga").hide();
        $("#liTabModeloTracao").hide();
        $("#liTabModeloReboque").hide();
        $("#liTabVigencia").hide();
        $("#liTabComponente").hide();
        $("#liTabNumeroEntrega").hide();
        $("#liTabPeso").hide();
        $("#liTabDistancia").hide();
        $("#liTabSubcontratacao").hide();
        $("#liTabPallet").hide();
        $("#liTabRotas").hide();
        $("#liTabTipoOperacao").show();
        $("#liTabTipoOcorrencia").hide();
        $("#liTabTempo").hide();
        $("#liTabAjudante").hide();
        $("#liTabHora").hide();
    }
    else {
        _tabelaFrete.Ativo.CssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-2");
        _tabelaFrete.CodigoIntegracao.visible(false);
        _tabelaFrete.Observacao.visible(false);
        _tabelaFrete.ObservacaoTerceiro.visible(false);
        _tabelaFrete.GrupoPessoas.visible(false);
        _tabelaFrete.ContratoFreteTransportador.visible(false);
        _tabelaFrete.ContratoFreteCliente.visible(false);
        _tabelaFrete.PermiteAlterarValor.visible(false);
        _tabelaFrete.TipoAlteracaoValor.visible(false);
        _tabelaFrete.ImprimirObservacaoCTe.visible(false);
        _tabelaFrete.PossuiValorMaximo.visible(false);
        _tabelaFrete.PossuiValorBase.visible(false);
        _tabelaFrete.UtilizarDiferencaDoValorBaseApenasFretePagos.visible(false);
        _tabelaFrete.CalcularFreteDestinoPrioritario.visible(false);
        _tabelaFrete.ValorParametroBaseObrigatorioParaCalculo.visible(false);
        _tabelaFrete.PossuiMinimoGarantido.visible(false);
        _tabelaFrete.EmissaoAutomaticaCTe.visible(false);
        _tabelaFrete.IncluirICMSValorFrete.visible(false);
        _tabelaFrete.NaoCalculaSemFreteValor.visible(false);
        _tabelaFrete.PercentualICMSIncluir.visible(false);
        _tabelaFrete.ParametroBase.visible(false);
        _tabelaFrete.TipoCalculo.visible(false);
        _tabelaFrete.MultiplicarValorDaFaixa.visible(false);
        _tabelaFrete.TipoFreteTabelaFrete.visible(false);
        _tabelaFrete.CanalEntrega.visible(false);
        _tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP.visible(false);

        _tabelaFrete.TabelaFreteMinima.visible(false);
        _tabelaFrete.UtilizaTabelaFreteMinima.visible(false);
        _tabelaFrete.UtilizaModeloVeicularVeiculo.visible(false);
        _tabelaFrete.LocalFreeTime.visible(false);
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.visible(false);
        _tabelaFrete.DestacarComponenteFrete.visible(false);
        _tabelaFrete.ComponenteFreteDestacar.visible(false);

        $("#liTabTipoEmbalagem").hide();
        $("#liTabTipoCarga").hide();
        $("#liTabModeloTracao").hide();
        $("#liTabModeloReboque").hide();
        $("#liTabVigencia").hide();
        $("#liTabComponente").hide();
        $("#liTabNumeroEntrega").hide();
        $("#liTabPeso").hide();
        $("#liTabDistancia").hide();
        $("#liTabSubcontratacao").hide();
        $("#liTabRotas").hide();
        $("#liTabPallet").hide();
        //$("#liTabTipoOperacao").hide();
        $("#liTabTempo").hide();
        $("#liTabTipoOcorrencia").hide();
        $("#liTabAjudante").hide();
        $("#liTabHora").hide();
    }

    ControleExibicaoUtilizarValorMinimoParaRateio();
    ControlePermiteAlterarValorFretePedidoPosCalculoFrete();
}

function alterouEmitirOcorrenciaAutomatica() {
    var habilitar = _tabelaFrete.EmitirOcorrenciaAutomatica.val();

    _tabelaFrete.TipoOcorrenciaTabelaMinima.visible(habilitar);
    _tabelaFrete.TipoOcorrenciaTabelaMinima.required(habilitar);
}

function alterouTabelaFreteMinima() {

    var habilitar = _tabelaFrete.TabelaFreteMinima.val();

    _tabelaFrete.EmitirOcorrenciaAutomatica.visible(habilitar);

    if (!habilitar)
        _tabelaFrete.EmitirOcorrenciaAutomatica.val(false);
}

function ControleExibicaoUtilizarValorMinimoParaRateio() {
    var tipoCalculo = _tabelaFrete.TipoCalculo.val();
    var isTabelaClienteCliente = _tabelaFrete.TipoTabelaFrete.val() == EnumTipoTabelaFrete.tabelaCliente;

    if (isTabelaClienteCliente && (tipoCalculo === EnumTipoCalculoTabelaFrete.PorCarga || tipoCalculo === EnumTipoCalculoTabelaFrete.PorMaiorValorPedido || tipoCalculo === EnumTipoCalculoTabelaFrete.PorMaiorValorPedidoAgrupados)) {
        _tabelaFrete.UtilizarValorMinimoParaRateio.visible(true);
    } else {
        _tabelaFrete.UtilizarValorMinimoParaRateio.val(false);
        _tabelaFrete.UtilizarValorMinimoParaRateio.visible(false);
    }
}


function ControlePermiteAlterarValorFretePedidoPosCalculoFrete() {
    const tipoCalculo = _tabelaFrete.TipoCalculo.val();
    if ((tipoCalculo === EnumTipoCalculoTabelaFrete.PorPedido || tipoCalculo === EnumTipoCalculoTabelaFrete.PorPedidosAgrupados)) {
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.visible(true);
    } else {
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.val(false);
        _tabelaFrete.PermiteAlterarValorFretePedidoPosCalculoFrete.visible(false);
    }
}


function buscarTabelaFretes() {
    var editar = { descricao: Localization.Resources.Fretes.TabelaFrete.Editar, id: guid(), evento: "onclick", metodo: editarTabelaFrete, tamanho: "15", icone: "" };
    var duplicar = { descricao: Localization.Resources.Fretes.TabelaFrete.Duplicar, id: guid(), evento: "onclick", metodo: DuplicarTabelaFreteClick, tamanho: "15", icone: "" };
    var integrarAlteracao = { descricao: Localization.Resources.Gerais.Geral.Integrar, id: guid(), evento: "onclick", metodo: integrarAlteracaoTabelaFreteClick, tamanho: "15", icone: "", visibilidade: permitirIntegrarAlteracaoTabelaFrete };
    var menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(duplicar);
    menuOpcoes.opcoes.push(integrarAlteracao);

    var configuracoesExportacao = { url: "TabelaFrete/ExportarPesquisa", titulo: "Tabela-de-Frete" };

    _gridTabelaFrete = new GridViewExportacao(_pesquisaTabelaFrete.Pesquisar.idGrid, "TabelaFrete/Pesquisa", _pesquisaTabelaFrete, menuOpcoes, configuracoesExportacao);
    _gridTabelaFrete.CarregarGrid();
}

function ContratoFreteTransportadorAlterado(cod) {
    if (cod == 0 || cod == null) {
        _tabelaFrete.ContratoFreteTransportadorPossuiFranquia.val(false);
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liTabFiliais").show();
            $("#liTabTransportadores").show();
        }

        if (_tabelaFrete.TipoTabelaFrete.val() == EnumTipoTabelaFrete.tabelaCliente)
            $("#liTabVigencia").show();
    }
    else {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liTabTransportadores").hide();
            limparCamposTransportador();

            $("#liTabFiliais").hide();
            limparCamposFilial();
        }

        $("#liTabVigencia").hide();
    }
}

function editarTabelaFrete(tabelaFreteGrid, duplicar) {
    limparCamposTabelaFrete();
    _tabelaFrete.Codigo.val(tabelaFreteGrid.Codigo);

    var url = (duplicar === true) ? "TabelaFrete/BuscarParaDuplicar" : "TabelaFrete/BuscarPorCodigo";

    BuscarPorCodigo(_tabelaFrete, url, function (retorno) {
        _pesquisaTabelaFrete.ExibirFiltros.visibleFade(false);

        if (duplicar !== true) {
            _crudTabelaFrete.Atualizar.visible(true);
            _crudTabelaFrete.Cancelar.visible(true);
            _crudTabelaFrete.Excluir.visible(true);
            _crudTabelaFrete.Adicionar.visible(false);
        }

        resetarTabs();

        preencherVigencia(retorno.Data.DadosVigencia);
        preencherTabelaFreteReboque(retorno.Data.Reboques);
        preencherTabelaFreteTracao(retorno.Data.Tracoes);

        RecarregarGridTransportadorTerceiro();
        RecarregarGridTipoTerceiro();
        recarregarGridTipoOperacao();
        recarregarGridTipoOcorrencia();
        recarregarGridTransportador();
        recarregarGridTipoEmbalagem();
        recarregarGridFilial();
        recarregarGridTipoCarga();
        recarregarGridModeloTracao();
        recarregarGridModeloReboque();
        recarregarGridComponenteFrete();
        recarregarGridNumeroEntrega();
        recarregarGridPesoTransportado();
        recarregarGridDistancia();
        recarregarGridSubcontratacao();
        recarregarGridPallet();
        recarregarGridRotaEmbarcadorTabelaFrete();
        RecarregarGridTempo();
        RecarregarGridAjudante();
        RecarregarGridHora();
        RecarregarGridFronteira();
        RecarregarGridContrato();
        recarregarTabelaFreteIntegracoes();
        RecarregarGridReboque();
        RecarregarGridTracao();
        recarregarGridPacote();

        PossuiFranquiaAlterado(retorno.Data.ContratoFreteTransportadorPossuiFranquia);

        if (duplicar !== true)
            _anexo.Anexos.val(retorno.Data.Anexos);

    }, null);
}

function limparCamposTabelaFrete() {
    _crudTabelaFrete.Atualizar.visible(false);
    _crudTabelaFrete.Cancelar.visible(false);
    _crudTabelaFrete.Excluir.visible(false);
    _crudTabelaFrete.Adicionar.visible(true);

    _pesoTransportado.UnidadeMedida.defCodEntity = 0;
    _pesoTransportado.UnidadeMedida.def = "";

    LimparCampos(_tabelaFrete);

    LimparCamposTransportadorTerceiro();
    LimparCamposTipoTerceiro();
    limparCamposTransportador();
    limparCamposTipoEmbalagem();
    limparCamposFilial();
    limparCamposTipoOperacao();
    limparCamposTipoOcorrencia();
    limparCamposTipoCarga();
    limparCamposModeloTracao();
    limparCamposModeloReboque();
    limparCamposVigencia();
    limparCamposComponenteFrete();
    limparCamposNumeroEntrega();
    limparCamposPesoTransportado();
    limparCamposDistancia();
    limparCamposSubcontratacao();
    limparCamposPallet();
    limparCamposRotaEmbarcadorTabelaFrete();
    LimparCamposTempo();
    LimparCamposAjudante();
    LimparCamposHora();
    limparCamposAnexo();
    LimparCamposFronteira();
    LimparCamposContrato();
    limparCamposTabelaFreteReboque();
    limparCamposTabelaFreteTracao();
    limparCamposPacotes();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabFiliais").show();
        $("#liTabTransportadores").show();
    }
    else {
        $("#liTabFiliais").hide();
        $("#liTabTransportadores").hide();
    }

    resetarTabs();

    RecarregarGridTransportadorTerceiro();
    RecarregarGridTipoTerceiro();
    recarregarGridTransportador();
    recarregarGridTipoEmbalagem();
    recarregarGridFilial();
    recarregarGridTipoOperacao();
    recarregarGridTipoOcorrencia();
    recarregarGridTipoCarga();
    recarregarGridModeloTracao();
    recarregarGridModeloReboque();
    recarregarGridPallet();
    recarregarGridComponenteFrete();
    recarregarGridNumeroEntrega();
    recarregarGridPesoTransportado();
    recarregarGridDistancia();
    recarregarGridSubcontratacao();
    recarregarGridRotaEmbarcadorTabelaFrete();
    RecarregarGridTempo();
    RecarregarGridAjudante();
    RecarregarGridHora();
    RecarregarGridFronteira();
    RecarregarGridContrato();
    recarregarTabelaFreteIntegracoes();
    RecarregarGridReboque();
    RecarregarGridTracao();
    recarregarGridPacote();
}

function LimparComponentePorFlag(koEntity, koBool) {
    koBool.val.subscribe(function (val) {
        if (!val)
            LimparCampoEntity(koEntity);
    });
}

function obterTabelaFreteSalvar() {
    preencherListasSelecao();
    _tabelaFrete.Observacao.val($("#" + _tabelaFrete.Observacao.id).val());

    var tabelaFrete = RetornarObjetoPesquisa(_tabelaFrete);

    preencherVigenciaSalvar(tabelaFrete);
    Veiculos(tabelaFrete);
    preencherCodigosRotasBiddingSalvar(tabelaFrete);

    return tabelaFrete;
}


function Veiculos(tabelaFrete) {
    var reboque = preencherTabelaFreteReboqueSalvar();
    var tracao = preencherTabelaFreteTracaoSalvar();

    var dadosPesquisa = [...reboque, ...tracao];

    tabelaFrete["Veiculos"] = JSON.stringify(dadosPesquisa);
}

function preencherListasSelecao() {
    let modelosReboque = new Array();
    let modelosTracao = new Array();
    let tiposCargas = new Array();
    let tiposOperacoes = new Array();
    let tiposOcorrencia = new Array();
    let transportadores = new Array();
    let tipoEmbalagens = new Array();
    let filiais = new Array();
    let transportadoresTerceiros = new Array();
    let tiposTerceiros = new Array();
    let fronteiras = new Array();
    let contratos = new Array();
    let tracoes = new Array();
    let reboques = new Array();

    $.each(_modeloReboque.Modelo.basicTable.BuscarRegistros(), function (i, modeloReboque) {
        modelosReboque.push({ Modelo: modeloReboque });
    });

    $.each(_modeloTracao.Modelo.basicTable.BuscarRegistros(), function (i, modeloTracao) {
        modelosTracao.push({ Modelo: modeloTracao });
    });

    $.each(_tipoCarga.Tipo.basicTable.BuscarRegistros(), function (i, tipoCarga) {
        tiposCargas.push({ Tipo: tipoCarga });
    });

    $.each(_tipoOperacao.Tipo.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacoes.push({ Tipo: tipoOperacao });
    });

    $.each(_tipoOcorrencia.Tipo.basicTable.BuscarRegistros(), function (i, tipoOcorrencia) {
        tiposOcorrencia.push({ Tipo: tipoOcorrencia });
    });

    $.each(_transportador.Transportador.basicTable.BuscarRegistros(), function (i, transportador) {
        transportadores.push({ Transportador: transportador });
    });

    $.each(_filial.Filial.basicTable.BuscarRegistros(), function (i, filial) {
        filiais.push({ Filial: filial });
    });

    $.each(_transportadorTerceiro.TransportadorTerceiro.basicTable.BuscarRegistros(), function (i, transportadorTerceiro) {
        transportadoresTerceiros.push(transportadorTerceiro.Codigo);
    });

    $.each(_tipoTerceiro.TipoTerceiro.basicTable.BuscarRegistros(), function (i, tipoTerceiro) {
        tiposTerceiros.push(tipoTerceiro.Codigo);
    });

    $.each(_tipoEmbalagem.TipoEmbalagem.basicTable.BuscarRegistros(), function (i, tipoEmbalagem) {
        tipoEmbalagens.push({ TipoEmbalagem: tipoEmbalagem });
    });

    $.each(_fronteira.Fronteira.basicTable.BuscarRegistros(), function (i, fronteira) {
        fronteiras.push(fronteira.Codigo);
    });
    $.each(_contrato.Contrato.basicTable.BuscarRegistros(), function (i, contrato) {
        contratos.push(contrato.Codigo);
    });


    $.each(_contrato.Contrato.basicTable.BuscarRegistros(), function (i, contrato) {
        contratos.push(contrato.Codigo);
    });

    $.each(_tabelaFreteReboque.Reboque.basicTable.BuscarRegistros(), function (i, reboque) {
        reboques.push(reboque.Codigo);
    });

    $.each(_tabelaFreteTracao.Tracao.basicTable.BuscarRegistros(), function (i, tracao) {
        tracoes.push(tracao.Codigo);
    });

    _tabelaFrete.TransportadoresTerceiros.val(JSON.stringify(transportadoresTerceiros));
    _tabelaFrete.TiposTerceiros.val(JSON.stringify(tiposTerceiros));
    _tabelaFrete.ModelosReboque.val(JSON.stringify(modelosReboque));
    _tabelaFrete.ModelosTracao.val(JSON.stringify(modelosTracao));
    _tabelaFrete.TiposCargas.val(JSON.stringify(tiposCargas));
    _tabelaFrete.TiposOperacoes.val(JSON.stringify(tiposOperacoes));
    _tabelaFrete.TiposDeOcorrencia.val(JSON.stringify(tiposOcorrencia));
    _tabelaFrete.Transportadores.val(JSON.stringify(transportadores));
    _tabelaFrete.TipoEmbalagens.val(JSON.stringify(tipoEmbalagens));
    _tabelaFrete.Filiais.val(JSON.stringify(filiais));
    _tabelaFrete.PercentualCobrancaPadrao.val(_subcontratacao.PercentualCobrancaPadrao.val());
    _tabelaFrete.Fronteiras.val(JSON.stringify(fronteiras));
    _tabelaFrete.Contratos.val(JSON.stringify(contratos));
    _tabelaFrete.Reboques.val(JSON.stringify(reboques));
    _tabelaFrete.Tracoes.val(JSON.stringify(tracoes));
    _tabelaFrete.PercentualCobrancaVeiculoFrota.val(_subcontratacao.PercentualCobrancaVeiculoFrota.val());
}

function preencherCodigosRotasBiddingSalvar(tabelaFrete) {
    const rotas = sessionStorage.getItem('rotas');
    if (!rotas) return;
    var dadosRecebidos = rotas.split("-");
    const codigosRotas = new Array();

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;
        codigosRotas.push(dadosRecebidos[i]);
    }

    tabelaFrete["RotasBidding"] = JSON.stringify(codigosRotas);
    sessionStorage.removeItem('rotas');
}

function resetarTabs() {
    $("#tabTabelaFrete a:first").tab("show");
}

function RetornoContratoFreteTransportador(data) {
    _tabelaFrete.ContratoFreteTransportador.val(data.Descricao);
    _tabelaFrete.ContratoFreteTransportador.codEntity(data.Codigo);

    _tabelaFrete.ContratoFreteTransportadorPossuiFranquia.val(data.PossuiFranquia);
}

function ValidarCamposObrigatoriosTabelaFrete() {
    if (!ValidarCamposObrigatorios(_tabelaFrete)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
        resetarTabs();
        return false;
    }

    if (!isExisteVigenciaCadastrada() && (_tabelaFrete.ContratoFreteTransportador.codEntity() == 0)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.VigenciaObrigatorio, Localization.Resources.Fretes.TabelaFrete.ObrigatorioAdicionarVigenciaParaTabelaFrete);
        $('.nav-tabs a[href="#knockoutVigencia"]').tab('show');
        return false;
    }

    if (_tabelaFrete.DestacarComponenteFrete.val() && _tabelaFrete.ComponenteFreteDestacar.codEntity() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.SelecioneOComponenteDeFreteParaDestacar);
        resetarTabs();
        return false;
    }
    var percentualCobrancaPadrao = Globalize.parseFloat(_subcontratacao.PercentualCobrancaPadrao.val());
    var percentualCobrancaVeiculoFrota = Globalize.parseFloat(_subcontratacao.PercentualCobrancaVeiculoFrota.val());

    if (percentualCobrancaPadrao > 100) {
        exibirMensagem(tipoMensagem.atencao, "Percentual", "O percentual 'Cobrança Padrão' não pode ser maior que 100%");
        _subcontratacao.PercentualCobrancaPadrao.requiredClass("form-control is-invalid");
        return false;
    } else {
        _subcontratacao.PercentualCobrancaPadrao.requiredClass("form-control");
    }

    if (percentualCobrancaVeiculoFrota > 100) {
        exibirMensagem(tipoMensagem.atencao, "Percentual", "O percentual 'Cobrança Veículo Frota' não pode ser maior que 100%");
        _subcontratacao.PercentualCobrancaVeiculoFrota.requiredClass("form-control is-invalid");
        return false;
    } else {
        _subcontratacao.PercentualCobrancaVeiculoFrota.requiredClass("form-control");
    }

    return true;
}

function permitirIntegrarAlteracaoTabelaFrete(registroSelecionado) {
    return registroSelecionado.PermitirIntegrarAlteracao;
}

function verificarSeExisteRegraCotacao() {
    executarReST("TabelaFrete/VerificarSeExisteRegraCotacao", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.RegraCotacao) {
                _tabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao.visible(true);
            } else {
                _tabelaFrete.NaoUsarCanalEntregaComoFiltroParaCotacao.visible(false);
            }
        }
    });
}

function preencherTabelaFreteAutomaticamente() {
    const transportador = sessionStorage.getItem('transportador');
    const tipoBidding = sessionStorage.getItem('tipoBidding');
    const descricao = sessionStorage.getItem('descricao');
    const inicioVigencia = sessionStorage.getItem('inicioVigencia');
    const fimVigencia = sessionStorage.getItem('fimVigencia');
    const formato = parseInt(sessionStorage.getItem('formato'));
    const modeloVeicularTracao = sessionStorage.getItem('modeloVeicularTracao');
    const modeloVeicularReboque = sessionStorage.getItem('modeloVeicularReboque');
    const tipoCarga = sessionStorage.getItem('tipoCarga');
    const filiaisParticipantes = sessionStorage.getItem('filiaisParticipantes');
    const faixaEntrega = sessionStorage.getItem('faixaEntrega');
    const faixaPeso = sessionStorage.getItem('faixaPeso');
    const faixaAjudante = sessionStorage.getItem('faixaAjudante');

    var numeroRegistrosTracao = 0;
    var numeroRegistrosReboque = 0;
    _tabelaFrete.Descricao.val(descricao);

    if (transportador)
        obterDadosTransportador(transportador);

    if (formato)
        obterDadosTipoLanceBidding(formato);

    if (inicioVigencia && transportador)
        obterDadosVigencia(inicioVigencia, fimVigencia, transportador);

    if (modeloVeicularTracao) {
        var dadosModeloVeicularTracao = modeloVeicularTracao.split("_");
        obterDadosModeloVeicularTracao(dadosModeloVeicularTracao);
        numeroRegistrosTracao = dadosModeloVeicularTracao.length;
    }

    if (modeloVeicularReboque) {
        var dadosModeloVeicularReboque = modeloVeicularReboque.split("_");
        obterDadosModeloVeicularReboque(dadosModeloVeicularReboque);
        numeroRegistrosReboque = dadosModeloVeicularReboque.length;
    }

    if (tipoCarga)
        obterDadosTipoCarga(tipoCarga);

    if (filiaisParticipantes)
        obterDadosFiliaisParticipantes(filiaisParticipantes);

    if (faixaEntrega) {
        if (formato != EnumTipoLanceBidding.LancePorViagemEntregaAjudante)
            obterDadosFaixaEntrega(faixaEntrega);
    }

    if (faixaPeso)
        obterDadosFaixaPeso(faixaPeso);

    if (faixaAjudante)
        obterDadosFaixaAjudante(faixaAjudante);

    if (transportador || inicioVigencia || modeloVeicularTracao || modeloVeicularReboque || tipoCarga)
        _tabelaFrete.TipoTabelaFrete.val(EnumTipoTabelaFrete.tabelaCliente);

    if (numeroRegistrosTracao > numeroRegistrosReboque)
        _tabelaFrete.ParametroBase.val(EnumTipoParametroBaseTabelaFrete.ModeloTracao);
    else
        _tabelaFrete.ParametroBase.val(EnumTipoParametroBaseTabelaFrete.ModeloReboque);

    sessionStorage.removeItem('transportador');
    sessionStorage.removeItem('descricao');
    sessionStorage.removeItem('tipoBidding');
    sessionStorage.removeItem('inicioVigencia');
    sessionStorage.removeItem('fimVigencia');
    sessionStorage.removeItem('formato');
    sessionStorage.removeItem('modeloVeicularTracao');
    sessionStorage.removeItem('modeloVeicularReboque');
    sessionStorage.removeItem('tipoCarga');
    sessionStorage.removeItem('faixaEntrega');
    sessionStorage.removeItem('faixaPeso');
    sessionStorage.removeItem('faixaAjudante');
}

function obterDadosTransportador(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");
        itemGrid.Codigo = dadoFormatado[0];
        itemGrid.Descricao = dadoFormatado[1];

        data.push(itemGrid);
    }

    _gridTransportador.CarregarGrid(data);
}

function obterDadosTipoLanceBidding(dados) {
    var data = new Array();

    if (dados == EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        const faixas = 15;
        for (let i = 0; i < faixas; i++) {
            var itemGrid = new Object();
            var itemGrid2 = new Object();
            var valor = i + 1;
            var codigoItemGrid = guid();
            var codigoItemGrid2 = guid();

            itemGrid.Codigo = codigoItemGrid;
            itemGrid.Descricao = "De " + valor + " á " + valor + " entregas";
            itemGrid.DescricaoTipo = "Por faixa de entrega";
            itemGrid.NumeroInicialEntrega = valor;
            itemGrid.ComAjudante = true;
            itemGrid.ComAjudanteDescri = "Sim";
            itemGrid.Tipo = 1;

            _numeroEntrega.Codigo.val(codigoItemGrid);
            _numeroEntrega.Tipo.val(1);
            _numeroEntrega.NumeroInicialEntrega.val(valor);
            _numeroEntrega.NumeroFinalEntrega.val(valor);
            _numeroEntrega.ComAjudante.val(true);

            data.push(itemGrid);
            _tabelaFrete.NumeroEntregas.list.push(SalvarListEntity(_numeroEntrega));

            itemGrid2.Codigo = codigoItemGrid2;
            itemGrid2.Descricao = "De " + valor + " á " + valor + " entregas";
            itemGrid2.DescricaoTipo = "Por faixa de entrega";
            itemGrid2.NumeroInicialEntrega = valor;
            itemGrid2.ComAjudante = false;
            itemGrid2.ComAjudanteDescri = "Não";
            itemGrid2.Tipo = 1;

            _numeroEntrega.Codigo.val(codigoItemGrid2);
            _numeroEntrega.Tipo.val(1);
            _numeroEntrega.NumeroInicialEntrega.val(valor);
            _numeroEntrega.NumeroFinalEntrega.val(valor);
            _numeroEntrega.ComAjudante.val(false);

            data.push(itemGrid2);
            _tabelaFrete.NumeroEntregas.list.push(SalvarListEntity(_numeroEntrega));
        }
        _gridNumeroEntrega.CarregarGrid(data);
    } else {
        _tabelaFrete.PossuiValorBase.val(true);
    }
}

function obterDadosVigencia(dadosInicio, dadosFim, transportador) {
    var data = new Array();
    var itemGrid = new Object();
    var empresa = new Object();

    transportador = transportador.split("-");
    empresa.Codigo = transportador[0];
    empresa.Descricao = transportador[1];
    dadosInicio = dadosInicio.split(" ");
    dadosFim = dadosFim.split(" ");

    itemGrid.Codigo = guid();
    itemGrid.DataInicial = dadosInicio[0];
    itemGrid.DataFinal = dadosFim[0];
    itemGrid.Transportador = transportador[1];
    itemGrid.Empresa = empresa;
    itemGrid.Situacao = obterSituacaoVigencia(dadosInicio[0], dadosFim[0]);

    data.push(itemGrid);
    _gridVigencia.CarregarGrid(data);
    _vigencia.ListaVigencia.val().push(itemGrid);

    $("#liTabVigencia").show();
}

function obterDadosModeloVeicularTracao(dadosRecebidos) {
    var data = new Array();

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");
        itemGrid.Codigo = dadoFormatado[0];
        itemGrid.Descricao = dadoFormatado[1];

        data.push(itemGrid);
    }

    _gridModeloTracao.CarregarGrid(data);
    $("#liTabModeloTracao").show();
}

function obterDadosModeloVeicularReboque(dadosRecebidos) {
    var data = new Array();

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");
        itemGrid.Codigo = dadoFormatado[0];
        itemGrid.Descricao = dadoFormatado[1];

        data.push(itemGrid);
    }

    _gridModeloReboque.CarregarGrid(data);
    $("#liTabModeloReboque").show();
}

function obterDadosTipoCarga(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");
        itemGrid.Codigo = dadoFormatado[0];
        itemGrid.Descricao = dadoFormatado[1];

        data.push(itemGrid);
    }

    _gridTipoCarga.CarregarGrid(data);
    $("#liTabTipoCarga").show();
}

function obterDadosFiliaisParticipantes(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");
        itemGrid.Codigo = dadoFormatado[0];
        itemGrid.Descricao = dadoFormatado[1];

        data.push(itemGrid);
    }

    _gridFilial.CarregarGrid(data);
    $("#liTabFiliais").show();
}

function obterDadosFaixaEntrega(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");
    var codigo = guid();

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");

        if (dadoFormatado[1] > 0) {
            itemGrid.ComAjudante = true;
            itemGrid.ComAjudanteDescri = "Sim";
            _numeroEntrega.ComAjudante.val(true);
        } else {
            itemGrid.ComAjudante = false;
            itemGrid.ComAjudanteDescri = "Não";
            _numeroEntrega.ComAjudante.val(false);
        }

        itemGrid.Codigo = codigo;
        itemGrid.Descricao = "De " + dadoFormatado[0] + " á " + dadoFormatado[0] + " entregas";
        itemGrid.DescricaoTipo = "Por faixa de entrega";
        itemGrid.NumeroInicialEntrega = dadoFormatado[0];
        itemGrid.Tipo = 1;

        _numeroEntrega.Codigo.val(codigo);
        _numeroEntrega.Tipo.val(1);
        _numeroEntrega.NumeroInicialEntrega.val(dadoFormatado[0]);
        _numeroEntrega.NumeroFinalEntrega.val(dadoFormatado[0]);

        data.push(itemGrid);
        _tabelaFrete.NumeroEntregas.list.push(SalvarListEntity(_numeroEntrega));
    }

    _gridNumeroEntrega.CarregarGrid(data);
}

function obterDadosFaixaPeso(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");

        if (dadoFormatado[1] > 0) {
            itemGrid.ComAjudante = true;
            itemGrid.ComAjudanteDescri = "Sim";
        } else {
            itemGrid.ComAjudante = false;
            itemGrid.ComAjudanteDescri = "Não";
        }

        itemGrid.Codigo = guid();
        itemGrid.Descricao = "Até " + dadoFormatado[0];
        itemGrid.DescricaoTipo = "Por Faixa de Peso";
        itemGrid.CodigoComponenteFrete = 0;
        itemGrid.CalculoPeso = 1;
        itemGrid.Tipo = 1;
        itemGrid.UnidadeMedida = "Tonelada";
        itemGrid.DescricaoCalculoPeso = "Por Valor Fixo";
        itemGrid.DescricaoComponente = "";
        itemGrid.ModeloVeicularCarga = "";
        itemGrid.ParaCalcularValorBase = false;
        itemGrid.ParaCalcularValorBaseDescri = "Não";
        itemGrid.PesoInicial = "0,000";

        data.push(itemGrid);
    }

    _gridPesoTransportado.CarregarGrid(data);
}

function obterDadosFaixaAjudante(dados) {
    var data = new Array();
    var dadosRecebidos = dados.split("_");

    for (let i = 0; i < dadosRecebidos.length; i++) {
        if (dadosRecebidos[i] == "") continue;

        var itemGrid = new Object();
        var dadoFormatado = dadosRecebidos[i].split("-");

        itemGrid.Codigo = guid();
        itemGrid.Descricao = "De " + dadoFormatado[0] + " à " + dadoFormatado[0] + " ajudantes";
        itemGrid.DescricaoTipo = "Por faixa de ajudantes";

        data.push(itemGrid);
    }

    _gridAjudante.CarregarGrid(data);
}

// #endregion Funções Privadas
