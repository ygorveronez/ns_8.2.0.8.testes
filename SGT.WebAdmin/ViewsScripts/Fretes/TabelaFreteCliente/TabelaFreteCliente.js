/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TabelaFreteVigencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ContratoTransporteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoCarga.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoEmissao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoTabelaFreteCliente.js" />
/// <reference path="../../Enumeradores/EnumEstruturaTabela.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroBaseTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoLanceBidding.js" />
/// <reference path="../../Fretes/TabelaValores/Valores.js" />
/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="../TabelaFreteCliente/Subcontratacao.js" />
/// <reference path="PracaPedagio.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaFreteCliente;
var _tabelaFreteCliente;
var _pesquisaTabelaFreteCliente;
var _tabelaFrete;
var _CRUDTabelaFreteCliente;
var _tipoIntegracao = [];

//var _tipoPagamento = [
//    { text: "Pago", value: EnumTipoPagamentoEmissao.Pago },
//    { text: "A Pagar", value: EnumTipoPagamentoEmissao.A_Pagar },
//    { text: "Outros", value: EnumTipoPagamentoEmissao.Outros },
//    { text: "Usar da Nota Fiscal", value: EnumTipoPagamentoEmissao.UsarDaNotaFiscal }
//];


var PesquisaTabelaFreteCliente = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFrete.getFieldDescription(), issue: 78, idBtnSearch: guid() });
    this.Vigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Vigencia.getFieldDescription(), idBtnSearch: guid(), issue: 82 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Codigo.getFieldDescription(), maxlength: 50 });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.RegiaoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Região de Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.RegiaoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.RegiaoDeDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: Localization.Resources.Fretes.TabelaFreteCliente.EstadoDeOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: Localization.Resources.Fretes.TabelaFreteCliente.EstadoDeDestino.getFieldDescription(), idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Transportador.getFieldDescription(), issue: 69, idBtnSearch: guid(), visible: ko.observable(false) });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Rota.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Tomador.getFieldDescription(), issue: 972, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.TipoDeOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid() });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(""), options: EnumTipoPagamentoEmissao.obterOpcoesPesquisa(), def: "", text: Localization.Resources.Fretes.TabelaFreteCliente.TipoDePagamento.getFieldDescription(), issue: 120 });
    this.PossuiRota = PropertyEntity({ val: ko.observable(""), options: EnumOpcaoRota.obterOpcoesPesquisa(), def: "", text: Localization.Resources.Fretes.TabelaFreteCliente.PossuiRota.getFieldDescription(), issue: 120, });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Situacao.getFieldDescription() });
    this.SituacaoTabelaFrete = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Fretes.TabelaFreteCliente.SituacaoTabelaFrete.getFieldDescription() });
    this.CEPOrigem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepDeOrigem.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestino = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepDeDestino.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.SomenteEmVigencia = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.SomenteTabelasVigentes, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.SituacaoAlteracao = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoTabelaFrete.Todas), options: EnumSituacaoAlteracaoTabelaFrete.obterOpcoesPesquisaTabelaFreteCliente(), def: EnumSituacaoAlteracaoTabelaFrete.Todas, text: Localization.Resources.Fretes.TabelaFreteCliente.SituacaoAprovacao.getFieldDescription(), visible: _CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoTabelaFrete });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Terceiro.getFieldDescription(), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.CanalEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.ContratoTransporteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoTabelaFreteCliente.Todas), options: EnumSituacaoIntegracaoTabelaFreteCliente.obterOpcoesPesquisa(), def: EnumSituacaoIntegracaoTabelaFreteCliente.Todas, text: Localization.Resources.Fretes.TabelaFreteCliente.SituacaoIntegracao.getFieldDescription(), visible: ko.observable(true) });

    this.ReenviarTabelas = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.ReenviarTodasComFalhaIntegracao, visible: ko.observable(true), eventClick: reenviarTodasProblemaIntegracaoClick });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteCliente.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.VincularRotasSemParar = PropertyEntity({
        eventClick: VincularRotasSemPararClick, type: types.event, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.VincularRotasSemParar), enable: ko.observable(true), idGrid: guid(), visible: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
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
        }, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TabelaFreteCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Fretes.TabelaFreteCliente.TabelaDeFrete.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 78 });
    this.Vigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Vigencia.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 82, val: ko.observable(""), def: "" });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Codigo.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 50, issue: 15 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Fretes.TabelaFreteCliente.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.Quilometragem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.Quilometragem.getFieldDescription(), val: ko.observable(0), getType: typesKnockout.int, def: 0, maxlength: 11, visible: ko.observable(_CONFIGURACAO_TMS.PermitirInformarQuilometragemTabelaFreteCliente) });
    this.ContratoTransporteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true) });

    this.FiltrarPorTransportadorContrato = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.EstadoDestino = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: Localization.Resources.Fretes.TabelaFreteCliente.EstadoDeDestino.getFieldDescription(), idBtnSearch: guid(), issue: 12 });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.Tomador.getFieldDescription()), idBtnSearch: guid(), issue: 972 });
    this.FormulaRateio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.FormulaDeRateio), issue: 257, visible: ko.observable(true), idBtnSearch: guid() });
    this.TipoRateioDocumentos = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoesTipoRateioDocumentos(), text: Localization.Resources.Fretes.TabelaFreteCliente.RateioDosDocumentos.getFieldDescription(), issue: 400, def: EnumTipoEmissaoCTeDocumentos.NaoInformado, required: false, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.IncluirICMSValorFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.IncluirIcmsNoValorDoFrete, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.HerdarInclusaoICMSTabelaFrete = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.HerdarAInclusaoDeIcmsNoFreteDaTabelaDeFrete, issue: 729, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.ExibirCamposIntegracaoOTM = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.ExibirCamposIntegracaoOTM, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoGrupoCarga = PropertyEntity({ val: ko.observable(EnumTipoGrupoCarga.Nenhum), options: ko.observable(EnumTipoGrupoCarga.obterOpcoes()), def: ko.observable(EnumTipoGrupoCarga.Nenhum), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.TipoGrupoDeCarga), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.GerenciarCapacidade = PropertyEntity({ val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: ko.observable(EnumSimNao.Nao), text: Localization.Resources.Fretes.TabelaFreteCliente.GerenciarCapacidade });
    this.EstruturaTabela = PropertyEntity({ text: ko.observable("Estrutura de Tabela:"), val: ko.observable(EnumEstruturaTabela.CustoFixo), options: ko.observable(EnumEstruturaTabela.obterOpcoes()), def: EnumEstruturaTabela.CustoFixo, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });

    this.PercentualRota = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.PercentualRota.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 6, visible: ko.observable(false), enable: ko.observable(true) });
    this.QuantidadeEntregas = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.QuantidadeEntregas.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(false) });
    this.CapacidadeOTM = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CapacidadeOTM.getFieldDescription(), val: ko.observable(""), options: Global.ObterOpcoesNaoSelecionadoBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: "", visible: ko.observable(false), enable: ko.observable(true) });
    this.DominioOTM = PropertyEntity({ text: ko.observable("Domínio OTM"), val: ko.observable(EnumDominioOTM.SAO), options: EnumDominioOTM.obterOpcoes(), def: EnumDominioOTM.SAO, visible: ko.observable(false), required: ko.observable(false) });
    this.PontoPlanejamentoTransporte = PropertyEntity({ text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.PontoPlanejamentoTransporte.getFieldDescription()), val: ko.observable(EnumPontoPlanejamentoTransporte.BR01), options: ko.observable(EnumPontoPlanejamentoTransporte.obterOpcoes()), def: EnumPontoPlanejamentoTransporte.BR01, visible: ko.observable(false), required: ko.observable(false) });
    this.TipoIntegracao = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.TipoIntegracao.getRequiredFieldDescription(), val: ko.observable(null), options: EnumTipoIntegracaoUnilever.obterOpcoes(), def: null, visible: ko.observable(false), required: ko.observable(false) });
    this.IDExterno = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.IDExterno.getFieldDescription(), val: ko.observable(""), def: "", enable: false, visible: ko.observable(false) });
    this.StatusAceiteTabela = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.StatusAceiteTabela.getFieldDescription(), enable: false, visible: ko.observable(false) });

    this.ObrigatorioInformarValePedagioCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.ObrigatorioInformarValePedagioCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.ContratoTransporteFrete.val.subscribe(function (novoValor) {
        if (_tipoIntegracao.filter(function (tipoIntegracao) { return tipoIntegracao.value == EnumTipoIntegracao.Unilever; }).length > 0) {
            _tabelaFreteCliente.DominioOTM.visible(novoValor);
            if (_CONFIGURACAO_TMS.PossuiIntegracaoLBC) {
                _tabelaFreteCliente.DominioOTM.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
                _tabelaFreteCliente.DominioOTM.text("*Domínio OTM");
            } else {
                _tabelaFreteCliente.DominioOTM.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
                _tabelaFreteCliente.DominioOTM.text("Domínio OTM");
            }
            _tabelaFreteCliente.PontoPlanejamentoTransporte.visible(novoValor);
            if (_CONFIGURACAO_TMS.PossuiIntegracaoLBC) {
                _tabelaFreteCliente.PontoPlanejamentoTransporte.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
                _tabelaFreteCliente.PontoPlanejamentoTransporte.text(Localization.Resources.Fretes.TabelaFreteCliente.PontoPlanejamentoTransporte.getRequiredFieldDescription());
                _tabelaFreteCliente.PontoPlanejamentoTransporte.options(EnumPontoPlanejamentoTransporte.obterOpcoesPesquisaIntegracaoLBC());
                _tabelaFreteCliente.PontoPlanejamentoTransporte.val(EnumPontoPlanejamentoTransporte.Selecione);
            } else {
                _tabelaFreteCliente.PontoPlanejamentoTransporte.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
                _tabelaFreteCliente.PontoPlanejamentoTransporte.text(Localization.Resources.Fretes.TabelaFreteCliente.PontoPlanejamentoTransporte.getFieldDescription());
            }
            _tabelaFreteCliente.TipoIntegracao.visible(novoValor);
            _tabelaFreteCliente.TipoIntegracao.val(EnumTipoIntegracaoUnilever.OTM);
            _tabelaFreteCliente.IDExterno.visible(novoValor);
            _tabelaFreteCliente.StatusAceiteTabela.visible(novoValor);
        }
    });

    this.PercentualCobrancaPadrao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, maxlength: 5, required: false });
    this.PercentualCobrancaVeiculoFrota = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, maxlength: 5, required: false });
    this.PercentualICMSIncluir = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.PercentualDeInclusao.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 6, val: ko.observable("100,00"), def: "100,00" });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamentoEmissao.UsarDaNotaFiscal), options: EnumTipoPagamentoEmissao.obterOpcoes(), def: EnumTipoPagamentoEmissao.UsarDaNotaFiscal, text: Localization.Resources.Fretes.TabelaFreteCliente.TipoDePagamento.getRequiredFieldDescription(), issue: 120, eventChange: tipoPagamentoChange });
    this.Moeda = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: Localization.Resources.Fretes.TabelaFreteCliente.Moeda.getRequiredFieldDescription(), issue: 0, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });
    this.CalcularFatorPesoPelaKM = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.MultiplicarOValorDoPesoPorUnidadePelaDistancia, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.ObservacaoInternaNaoSeraImpressaNoCtE.getFieldDescription(), maxlength: 500, visible: ko.observable(true) });
    this.PrioridadeUso = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.PrioridadeDeUso.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, maxlength: 2 });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.Transportador.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });

    this.TipoOperacao = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarTipoDeOperacao, idBtnSearch: guid(), issue: 121, enable: ko.observable(true) });
    this.GridTipoOperacao = PropertyEntity({ type: types.local });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Fronteira = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarFronteira, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.GridFronteira = PropertyEntity({ type: types.local });
    this.Fronteiras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TipoCarga = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarTipoDeCarga, idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridTipoCarga = PropertyEntity({ type: types.local });
    this.TiposCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TransportadorTerceiro = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarTransportadorTerceiro, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridTransportadorTerceiro = PropertyEntity({ type: types.local });
    this.TransportadoresTerceiros = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarModeloVeicularCarga = PropertyEntity({ type: types.event, idGrid: guid() });

    this.CodigoSemParar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Fretes.TabelaFreteCliente.CodigoSemParar.getFieldDescription(), visible: ko.observable(false) });
    this.LeadTime = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.LeadTime.getFieldDescription(), visible: ko.observable(false) });
    this.TipoLeadTime = PropertyEntity({ val: ko.observable(EnumPadraoTempoDiasMinutos.Minutos), options: ko.observable(EnumPadraoTempoDiasMinutos.obterOpcoes()), def: ko.observable(EnumPadraoTempoDiasMinutos.Minutos), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.TipoLeadTime), required: ko.observable(false) });
    this.LeadTimeMinutos = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.LeadTimeMinutos.getFieldDescription(), visible: ko.observable(true) });
    this.LeadTimeTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.LeadTimeTransportador.getFieldDescription(), visible: ko.observable(true) });
    this.RestricaoEntrega = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Fretes.TabelaFreteCliente.RestricaoDeEntrega.getFieldDescription(), val: ko.observable(new Array()), def: new Array(), options: EnumDiaSemana.obterOpcoes(), visible: ko.observable(false) });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.CanalEntrega.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Fretes.TabelaFreteCliente.CanalVenda.getFieldDescription()), visible: ko.observable(true), idBtnSearch: guid() });

    this.TipoIntegracao.val.subscribe(function (novoValor) {
        if (_tipoIntegracao.filter(function (tipoIntegracao) { return tipoIntegracao.value == EnumTipoIntegracao.Unilever; }).length > 0) {
            if (novoValor == EnumTipoIntegracaoUnilever.OTM) {
                _tabelaFreteCliente.Empresa.required(true);
                _tabelaFreteCliente.Empresa.text(Localization.Resources.Fretes.TabelaFreteCliente.Transportador.getRequiredFieldDescription());
                _tabelaFreteCliente.CanalVenda.required(true);
                _tabelaFreteCliente.CanalVenda.text(Localization.Resources.Fretes.TabelaFreteCliente.CanalVenda.getRequiredFieldDescription());
                _tabelaFreteCliente.CanalEntrega.required(false);
                _tabelaFreteCliente.CanalEntrega.text(Localization.Resources.Fretes.TabelaFreteCliente.CanalEntrega.getFieldDescription());
            } else {
                _tabelaFreteCliente.Empresa.required(false);
                _tabelaFreteCliente.Empresa.text(Localization.Resources.Fretes.TabelaFreteCliente.Transportador.getFieldDescription());
                _tabelaFreteCliente.CanalVenda.required(false);
                _tabelaFreteCliente.CanalVenda.text(Localization.Resources.Fretes.TabelaFreteCliente.CanalVenda.getFieldDescription());
                _tabelaFreteCliente.CanalEntrega.required(false);
                _tabelaFreteCliente.CanalEntrega.text(Localization.Resources.Fretes.TabelaFreteCliente.CanalEntrega.getFieldDescription());
            }
        }
    });

    //#region Origens

    this.FreteValidoParaQualquerOrigem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.OValorDestaTabelaEValidoParaQualquerUmaDasOrigensInformadas, issue: 727, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Origem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarOrigem, idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });
    this.GridOrigem = PropertyEntity({ type: types.local });
    this.Origens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ClienteOrigem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarCliente, idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.GridClienteOrigem = PropertyEntity({ type: types.local });
    this.ClientesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.EstadoOrigem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarEstado, idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.GridEstadoOrigem = PropertyEntity({ type: types.local });
    this.EstadosOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RegiaoOrigem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarRegiao, idBtnSearch: guid(), issue: 110, enable: ko.observable(true) });
    this.GridRegiaoOrigem = PropertyEntity({ type: types.local });
    this.RegioesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RotaOrigem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarRota, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridRotaOrigem = PropertyEntity({ type: types.local });
    this.RotasOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PaisOrigem = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarPais, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridPaisOrigem = PropertyEntity({ type: types.local });
    this.PaisesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ListaCEPsOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.CEPsOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridCEPOrigem = PropertyEntity({ type: types.local });
    this.CEPOrigemInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPOrigemFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.AdicionarCEPOrigem = PropertyEntity({ eventClick: AdicionarCEPOrigemClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });

    //#endregion Origens

    //#region Destinos

    this.FreteValidoParaQualquerDestino = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.OValorDestaTabelaEValidoParaQualquerUmDosDestinosInformados, issue: 727, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Destino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarDestino, idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });
    this.GridDestino = PropertyEntity({ type: types.local });
    this.Destinos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ClienteDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarCliente, idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.GridClienteDestino = PropertyEntity({ type: types.local });
    this.ClientesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.EstadoDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarEstado, idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.GridEstadoDestino = PropertyEntity({ type: types.local });
    this.EstadosDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RegiaoDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarRegiao, idBtnSearch: guid(), issue: 110, enable: ko.observable(true) });
    this.GridRegiaoDestino = PropertyEntity({ type: types.local });
    this.RegioesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RotaDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarRota, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridRotaDestino = PropertyEntity({ type: types.local });
    this.RotasDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PaisDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.AdicionarPais, idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridPaisDestino = PropertyEntity({ type: types.local });
    this.PaisesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ListaCEPsDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.CEPsDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridCEPDestino = PropertyEntity({ type: types.local });
    this.CEPDestinoInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestinoFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.CepFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestinoDiasUteis = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFreteCliente.PrazoDiasUteis.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 11, visible: ko.observable(false) });
    this.AdicionarCEPDestino = PropertyEntity({ eventClick: AdicionarCEPDestinoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });

    //#endregion Destinos

    //#region Subcontratacoes

    this.Subcontratacoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.SubcontratacoesGeral = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Valores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.Observacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresMinimosGarantidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresMaximos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresBases = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.ValoresExcedentes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });
    this.PercentuaisPagamentoAgregados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), idGrid: guid(), visible: false });

    this.PercentualICMSIncluir.val.subscribe(function (novoValor) {
        validarPercentualBlur(novoValor);
    });

    this.Empresa.codEntity.subscribe(function (novoValor) {
        //buscarVigenciaAtual(_tabelaFreteCliente);
    });

    this.TabelaFrete.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0) {
            _tabelaFreteCliente.LeadTime.visible(false);
            _tabelaFreteCliente.RestricaoEntrega.visible(false);
        }
        _tabelaFreteCliente.Vigencia.codEntity(0);
        _tabelaFreteCliente.Vigencia.val("");
    });

    //#endregion Subcontratacoes

    //#region Praça de pedágio

    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFreteCliente.Rota.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    //#endregion Praça de pedágio

    this.DataInicialContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinalContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.DataInicialVigencia = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: false });
    this.DataFinalVigencia = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: false });
};

var CRUDTabelaFreteCliente = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.Cancelar, visible: ko.observable(true) });
    this.LimparCamposParcial = PropertyEntity({ eventClick: limparCamposTabelaFreteClienteParcial, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.CancelarEContinuarDigitando, visible: ko.observable(true) });

    this.ReprocessarTabelasAntigas = PropertyEntity({ eventClick: ReprocessarTabelasAntigasClick, type: types.event, text: Localization.Resources.Fretes.TabelaFreteCliente.ReprocessarTabelasAntigas, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTabelaFreteCliente() {
    _tabelaFreteCliente = new TabelaFreteCliente();
    KoBindings(_tabelaFreteCliente, "knockoutCadastroTabelaFreteCliente");

    _CRUDTabelaFreteCliente = new CRUDTabelaFreteCliente();
    KoBindings(_CRUDTabelaFreteCliente, "knockoutCRUDCadastroTabelaFreteCliente");

    _pesquisaTabelaFreteCliente = new PesquisaTabelaFreteCliente();
    KoBindings(_pesquisaTabelaFreteCliente, "knockoutPesquisaTabelaFreteCliente", false);

    HeaderAuditoria("TabelaFreteCliente", _tabelaFreteCliente);

    ObterTiposIntegracao().then(function () {
        if (_tipoIntegracao.filter(function (tipoIntegracao) { return tipoIntegracao.value == EnumTipoIntegracao.SemParar; }).length > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            $("#liTabPracaPedagio").show();
            _tabelaFreteCliente.CodigoSemParar.visible(true);
            _pesquisaTabelaFreteCliente.VincularRotasSemParar.visible(true);
            ValidarVinculoRotasSemParar();
        }

        if (_tipoIntegracao.filter(function (tipoIntegracao) { return tipoIntegracao.value == EnumTipoIntegracao.Unilever; }).length > 0) {
            _tabelaFreteCliente.TipoIntegracao.val(EnumTipoIntegracaoUnilever.OTM);
            _tabelaFreteCliente.TipoIntegracao.def = EnumTipoIntegracaoUnilever.OTM;
            _tabelaFreteCliente.TipoIntegracao.required(true);
        }
    });

    BuscarTabelasDeFrete(_tabelaFreteCliente.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaCliente);
    BuscarClientes(_tabelaFreteCliente.Tomador);
    BuscarVigenciasTabelaFrete(_tabelaFreteCliente.Vigencia, _tabelaFreteCliente.TabelaFrete, function (data) { retornoBuscaVigencias(_tabelaFreteCliente, data); }, true, _tabelaFreteCliente.Empresa, _tabelaFreteCliente.DataInicialContrato, _tabelaFreteCliente.DataFinalContrato, _tabelaFreteCliente.DataInicialVigencia, _tabelaFreteCliente.DataFinalVigencia, _tabelaFreteCliente.ContratoTransporteFrete);
    BuscarRateioFormulas(_tabelaFreteCliente.FormulaRateio);
    BuscarTransportadores(_tabelaFreteCliente.Empresa);
    //new BuscarFronteiras(_tabelaFreteCliente.Fronteira);
    //new BuscarClientes(_tabelaFreteCliente.Fronteira, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    BuscarCanaisEntrega(_tabelaFreteCliente.CanalEntrega);
    BuscarCanaisVenda(_tabelaFreteCliente.CanalVenda);
    BuscarContratosTransporteFrete(_tabelaFreteCliente.ContratoTransporteFrete, retornoBuscarContratos, null, _tabelaFreteCliente.FiltrarPorTransportadorContrato, _tabelaFreteCliente.TabelaFrete);

    BuscarTabelasDeFrete(_pesquisaTabelaFreteCliente.TabelaFrete, retornoTabelaPesquisa, EnumTipoTabelaFrete.tabelaCliente);
    BuscarVigenciasTabelaFrete(_pesquisaTabelaFreteCliente.Vigencia, _pesquisaTabelaFreteCliente.TabelaFrete, function (data) { retornoBuscaVigencias(_pesquisaTabelaFreteCliente, data); });
    BuscarClientes(_pesquisaTabelaFreteCliente.Remetente);
    BuscarClientes(_pesquisaTabelaFreteCliente.Destinatario);
    BuscarClientes(_pesquisaTabelaFreteCliente.Tomador);
    BuscarLocalidades(_pesquisaTabelaFreteCliente.LocalidadeOrigem);
    BuscarLocalidades(_pesquisaTabelaFreteCliente.LocalidadeDestino);
    BuscarRegioes(_pesquisaTabelaFreteCliente.RegiaoDestino);
    BuscarRegioes(_pesquisaTabelaFreteCliente.RegiaoOrigem);
    BuscarTiposOperacao(_pesquisaTabelaFreteCliente.TipoOperacao);
    BuscarTransportadores(_pesquisaTabelaFreteCliente.Empresa);
    BuscarRotasFrete(_pesquisaTabelaFreteCliente.RotaFrete);
    BuscarClientes(_pesquisaTabelaFreteCliente.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    BuscarCanaisEntrega(_pesquisaTabelaFreteCliente.CanalEntrega);
    BuscarContratosTransporteFrete(_pesquisaTabelaFreteCliente.ContratoTransporteFrete);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaTabelaFreteCliente.Empresa.visible(true);
        _tabelaFreteCliente.Empresa.visible(true);
        _pesquisaTabelaFreteCliente.RotaFrete.visible(true);
    }

    buscarTabelaFreteClientes();
    buscarTabelaFretePadrao(retornoTabelaFrete);

    verificarIntegracaoLBC(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);

    LoadOrigens();
    LoadDestinos();
    loadValores();
    loadSubcontratacao();
    LoadTiposOperacao();
    LoadTransportadoresTerceiros();
    loadTabelaFreteClienteAutorizacao();
    loadPracaPedagio();
    LoadFronteira();
    LoadTipoCarga();
    loadModeloVeicularCarga();
    loadTabelaFreteClienteIntegracoes();
    ObterIntegracoesHabilitadas();

    preencherTabelaFreteClienteAutomaticamente();
}

function tipoPagamentoChange(e) {
    if (_tabelaFreteCliente.TipoPagamento.val() == EnumTipoPagamentoEmissao.Outros) {
        _tabelaFreteCliente.Tomador.text(Localization.Resources.Fretes.TabelaFreteCliente.Tomador.getRequiredFieldDescription());
        _tabelaFreteCliente.Tomador.required = true;
    } else {
        _tabelaFreteCliente.Tomador.text(Localization.Resources.Fretes.TabelaFreteCliente.Tomador.getFieldDescription());
        _tabelaFreteCliente.Tomador.required = false;
    }
}

function buscarTabelaFretePadrao(callback) {
    var data = { TipoTabelaFrete: EnumTipoTabelaFrete.tabelaCliente };
    executarReST("TabelaFrete/BuscarTabelasPorTipo", data, function (arg) {
        if (arg.Success)
            if (arg.Data.length == 1)
                callback(arg.Data[0]);
    });
}

function retornoTabelaFrete(e) {
    //limparCamposTabelaFreteCliente();

    _tabelaFreteCliente.TabelaFrete.codEntity(e.Codigo);
    _tabelaFreteCliente.TabelaFrete.val(e.Descricao);

    Global.setarFocoProximoCampo(_tabelaFreteCliente.Vigencia.id);

    _tabelaFreteCliente.LeadTime.visible(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente && (e.TipoCalculo === EnumTipoCalculoTabelaFrete.PorPedido || e.TipoCalculo === EnumTipoCalculoTabelaFrete.PorPedidosAgrupados));
    _tabelaFreteCliente.RestricaoEntrega.visible(_CONFIGURACAO_TMS.PermitirInformarLeadTimeTabelaFreteCliente && (e.TipoCalculo === EnumTipoCalculoTabelaFrete.PorPedido || e.TipoCalculo === EnumTipoCalculoTabelaFrete.PorPedidosAgrupados));

    buscarDadosTabelaFrete();
    if (!_CONFIGURACAO_TMS.NaoBuscarAutomaticamenteVigenciaTabelaFrete)
        buscarVigenciaAtual(_tabelaFreteCliente);
}

function retornoTabelaPesquisa(e) {
    _pesquisaTabelaFreteCliente.TabelaFrete.codEntity(e.Codigo);
    _pesquisaTabelaFreteCliente.TabelaFrete.val(e.Descricao);

    if (!_CONFIGURACAO_TMS.NaoBuscarAutomaticamenteVigenciaTabelaFrete)
        buscarVigenciaAtual(_pesquisaTabelaFreteCliente);
}

function buscarDadosTabelaFrete(duplicar) {
    executarReST("TabelaFrete/BuscarPorCodigo", { Codigo: _tabelaFreteCliente.TabelaFrete.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _tabelaFrete = arg.Data;
                _tabelaFreteCliente.GrupoPessoas.codEntity(_tabelaFrete.GrupoPessoas.Codigo);
                _tabelaFreteCliente.GrupoPessoas.val(_tabelaFrete.GrupoPessoas.Descricao);
                _tabelaFreteCliente.CalcularFatorPesoPelaKM.val(_tabelaFrete.CalcularFatorPesoPelaKM);

                if (_tabelaFreteCliente.Codigo.val() <= 0) {
                    _tabelaFreteCliente.IncluirICMSValorFrete.val(_tabelaFrete.IncluirICMSValorFrete);
                    _tabelaFreteCliente.PercentualICMSIncluir.val(_tabelaFrete.PercentualICMSIncluir);
                }

                if (arg.Data.Contratos.length > 0) {
                    _tabelaFreteCliente.ContratoTransporteFrete.text(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador.getRequiredFieldDescription());
                    _tabelaFreteCliente.ContratoTransporteFrete.required(true);
                    _tabelaFreteCliente.FiltrarPorTransportadorContrato.val(true);
                } else {
                    _tabelaFreteCliente.ContratoTransporteFrete.text(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador.getFieldDescription());
                    _tabelaFreteCliente.ContratoTransporteFrete.required(false);
                    _tabelaFreteCliente.FiltrarPorTransportadorContrato.val(false);
                }

                if (arg.Data.DadosVigencia.Vigencias.length > 0) {
                    _tabelaFreteCliente.DataInicialVigencia.val(arg.Data.DadosVigencia.Vigencias[0].DataInicial);
                    _tabelaFreteCliente.DataFinalVigencia.val(arg.Data.DadosVigencia.VigenciasFinal[0].DataFinal);
                }

                _tabelaFreteCliente.CEPDestinoDiasUteis.visible(_tabelaFrete.PermiteInformarDiasUteisPorFaixaCEP);
                LoadFaixaCEPDestino();

                if (!duplicar)
                    CarregarModelosVeicularesGrid(arg.Data.ModelosReboque);

                montarTabelaValores();
                if (_tabelaFrete.PagamentoTerceiro)
                    $("#liTabTransportadorTerceiro").removeClass("d-none");
                else {
                    $("#liTabTransportadorTerceiro").addClass("d-none");
                    _tabelaFreteCliente.TransportadorTerceiro.basicTable.CarregarGrid([]);
                }
            }
        }
    });
}

function buscarVigenciaAtual(ko) {
    executarReST("TabelaFrete/BuscarVigenciaAtual", { TabelaFrete: ko.TabelaFrete.codEntity(), Empresa: ko.Empresa.codEntity() }, function (arg) {
        if (arg.Success && arg.Data != null)
            retornoBuscaVigencias(ko, arg.Data);
    });
}

function retornoBuscaVigencias(ko, data) {
    if (data.Codigo > 0) {
        ko.Vigencia.codEntity(data.Codigo);
        ko.Vigencia.val("De " + data.DataInicial + (data.DataFinal != "" ? " até " + data.DataFinal : ""));
        ko.Vigencia.entityDescription(ko.Vigencia.val());
    }   
}

function PreencherListasDeSelecao() {
    _tabelaFreteCliente.Origens.val(JSON.stringify(_tabelaFreteCliente.Origem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.ClientesOrigem.val(JSON.stringify(_tabelaFreteCliente.ClienteOrigem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.EstadosOrigem.val(JSON.stringify(_tabelaFreteCliente.EstadoOrigem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.RegioesOrigem.val(JSON.stringify(_tabelaFreteCliente.RegiaoOrigem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.RotasOrigem.val(JSON.stringify(_tabelaFreteCliente.RotaOrigem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.PaisesOrigem.val(JSON.stringify(_tabelaFreteCliente.PaisOrigem.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.ListaCEPsOrigem.val(JSON.stringify(_tabelaFreteCliente.CEPsOrigem.val()));

    _tabelaFreteCliente.Destinos.val(JSON.stringify(_tabelaFreteCliente.Destino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.ClientesDestino.val(JSON.stringify(_tabelaFreteCliente.ClienteDestino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.EstadosDestino.val(JSON.stringify(_tabelaFreteCliente.EstadoDestino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.RegioesDestino.val(JSON.stringify(_tabelaFreteCliente.RegiaoDestino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.RotasDestino.val(JSON.stringify(_tabelaFreteCliente.RotaDestino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.PaisesDestino.val(JSON.stringify(_tabelaFreteCliente.PaisDestino.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.ListaCEPsDestino.val(JSON.stringify(_tabelaFreteCliente.CEPsDestino.val()));

    _tabelaFreteCliente.PercentualCobrancaPadrao.val(_subcontratacaoGeral.PercentualCobrancaPadrao.val());
    _tabelaFreteCliente.PercentualCobrancaVeiculoFrota.val(_subcontratacaoGeral.PercentualCobrancaVeiculoFrota.val());
    _tabelaFreteCliente.TiposOperacao.val(JSON.stringify(_tabelaFreteCliente.TipoOperacao.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.TransportadoresTerceiros.val(JSON.stringify(_tabelaFreteCliente.TransportadorTerceiro.basicTable.BuscarRegistros()));
    _tabelaFreteCliente.TiposCarga.val(JSON.stringify(_tabelaFreteCliente.TipoCarga.basicTable.BuscarRegistros()));

    let fronteiras = new Array();
    $.each(_tabelaFreteCliente.Fronteira.basicTable.BuscarRegistros(), function (i, fronteira) {
        fronteiras.push(fronteira.Codigo);
    });
    _tabelaFreteCliente.Fronteiras.val(JSON.stringify(fronteiras));

}

function adicionarClick() {
    var tabelaFreteCliente = obterTabelaFreteClienteSalvar();

    if (!tabelaFreteCliente)
        return;

    executarReST("TabelaFreteCliente/Adicionar", tabelaFreteCliente, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.CadastradoComSucesso);
                _gridTabelaFreteCliente.CarregarGrid();
                limparCamposTabelaFreteClienteParcial();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function atualizarClick() {
    var tabelaFreteCliente = obterTabelaFreteClienteSalvar();

    if (!tabelaFreteCliente)
        return;

    executarReST("TabelaFreteCliente/Atualizar", tabelaFreteCliente, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.AtualizadoComSucesso);
                _gridTabelaFreteCliente.CarregarGrid();
                limparCamposTabelaFreteCliente();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Fretes.TabelaFreteCliente.Confirmacao, Localization.Resources.Fretes.TabelaFreteCliente.RealmenteDesejaExcluirEsseFrete, function () {
        ExcluirPorCodigo(_tabelaFreteCliente, "TabelaFreteCliente/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.ExcluidoComSucesso);
                    _gridTabelaFreteCliente.CarregarGrid();
                    limparCamposTabelaFreteCliente();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteClienteSugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function ReprocessarTabelasAntigasClick() {
    exibirConfirmacao(Localization.Resources.Fretes.TabelaFreteCliente.Confirmacao, Localization.Resources.Fretes.TabelaFreteCliente.DesejaRealmenteReprocessarAsTabelasDeFreteAntigas, function () {
        executarReST("TabelaFreteCliente/ReprocessarTabelasAntigas", {}, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Fretes.TabelaFreteCliente.ReprocessadoComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFreteCliente.Sugestao, r.Msg, 9999999);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function validarPercentualBlur(novoValor) {
    var valor = Globalize.parseFloat(novoValor);
    if (isNaN(valor) || valor == 0 || valor > 100) {
        _tabelaFreteCliente.PercentualICMSIncluir.val(Globalize.format(100, "n2"));
    }
}

function cancelarClick(e) {
    limparCamposTabelaFreteCliente();
}

function DuplicarTabelaFreteClienteClick(tabelaFreteClienteGrid, duplicarComoRetorno) {
    editarTabelaFreteCliente(tabelaFreteClienteGrid, true, duplicarComoRetorno);
}

function buscarTabelaFreteClientes() {
    var editar = { descricao: Localization.Resources.Fretes.TabelaFreteCliente.Editar, id: guid(), evento: "onclick", metodo: editarTabelaFreteCliente, tamanho: "5", icone: "" };
    var duplicar = { descricao: Localization.Resources.Fretes.TabelaFreteCliente.Duplicar, id: guid(), evento: "onclick", metodo: DuplicarTabelaFreteClienteClick, tamanho: "5", icone: "" };
    var duplicarComoRetorno = {
        descricao: Localization.Resources.Fretes.TabelaFreteCliente.DuplicarComoRetorno, id: guid(), evento: "onclick", metodo: function (registroSelecionado) {
            DuplicarTabelaFreteClienteClick(registroSelecionado, true);
        }, tamanho: "5", icone: ""
    };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = [editar, duplicar, duplicarComoRetorno];

    _gridTabelaFreteCliente = new GridView(_pesquisaTabelaFreteCliente.Pesquisar.idGrid, "TabelaFreteCliente/Pesquisa", _pesquisaTabelaFreteCliente, menuOpcoes, null, null);
    _gridTabelaFreteCliente.CarregarGrid();
}

function editarTabelaFreteCliente(tabelaFreteClienteGrid, duplicar, duplicarComoRetorno) {
    limparCamposTabelaFreteCliente();

    var url = (duplicar === true) ? "TabelaFreteCliente/BuscarParaDuplicar" : "TabelaFreteCliente/BuscarPorCodigo";

    var dados = {
        Codigo: tabelaFreteClienteGrid.Codigo,
        DuplicarComoRetorno: duplicarComoRetorno === true
    };

    executarReST(url, dados, function (e) {
        if (e.Success) {
            PreencherObjetoKnout(_tabelaFreteCliente, e);

            _pesquisaTabelaFreteCliente.ExibirFiltros.visibleFade(false);
            _tabelaFreteCliente.LeadTime.visible(e.Data.PermiteLeadTime);
            _tabelaFreteCliente.RestricaoEntrega.visible(e.Data.PermiteLeadTime);

            if (duplicar !== true) {
                _CRUDTabelaFreteCliente.Atualizar.visible(true);
                _CRUDTabelaFreteCliente.Cancelar.visible(true);
                _CRUDTabelaFreteCliente.Excluir.visible(true);
                _CRUDTabelaFreteCliente.Adicionar.visible(false);
            }

            _tabelaFreteCliente.TipoOperacao.basicTable.CarregarGrid(_tabelaFreteCliente.TiposOperacao.val());
            _tabelaFreteCliente.Fronteira.basicTable.CarregarGrid(_tabelaFreteCliente.Fronteiras.val());
            _tabelaFreteCliente.TransportadorTerceiro.basicTable.CarregarGrid(_tabelaFreteCliente.TransportadoresTerceiros.val());
            _tabelaFreteCliente.TipoCarga.basicTable.CarregarGrid(_tabelaFreteCliente.TiposCarga.val());

            _tabelaFreteCliente.Origem.basicTable.CarregarGrid(_tabelaFreteCliente.Origens.val());
            _tabelaFreteCliente.ClienteOrigem.basicTable.CarregarGrid(_tabelaFreteCliente.ClientesOrigem.val());
            _tabelaFreteCliente.EstadoOrigem.basicTable.CarregarGrid(_tabelaFreteCliente.EstadosOrigem.val());
            _tabelaFreteCliente.RegiaoOrigem.basicTable.CarregarGrid(_tabelaFreteCliente.RegioesOrigem.val());
            _tabelaFreteCliente.RotaOrigem.basicTable.CarregarGrid(_tabelaFreteCliente.RotasOrigem.val());
            _tabelaFreteCliente.PaisOrigem.basicTable.CarregarGrid(_tabelaFreteCliente.PaisesOrigem.val());
            RecarregarGridCEPOrigem();
            ValidarOrigensDisponiveis();

            _tabelaFreteCliente.Destino.basicTable.CarregarGrid(_tabelaFreteCliente.Destinos.val());
            _tabelaFreteCliente.ClienteDestino.basicTable.CarregarGrid(_tabelaFreteCliente.ClientesDestino.val());
            _tabelaFreteCliente.EstadoDestino.basicTable.CarregarGrid(_tabelaFreteCliente.EstadosDestino.val());
            _tabelaFreteCliente.RegiaoDestino.basicTable.CarregarGrid(_tabelaFreteCliente.RegioesDestino.val());
            _tabelaFreteCliente.RotaDestino.basicTable.CarregarGrid(_tabelaFreteCliente.RotasDestino.val());
            _tabelaFreteCliente.PaisDestino.basicTable.CarregarGrid(_tabelaFreteCliente.PaisesDestino.val());

            RecarregarGridCEPDestino();
            ValidarDestinosDisponiveis();

            tipoPagamentoChange(_tabelaFreteCliente);
            buscarDadosTabelaFrete(duplicar);
            recarregarGridSubcontratacao();
            recarregarGridSubcontratacaoValorAdicional();
            recarregarGridSubcontratacaoValorAdicionalGeral();
            recarregarGridPracaPedagioTarifaRota();
            recarregarTabelaFreteClienteIntegracoes();
            preencherModeloVeicularCarga(e.Data.ModelosVeicularesCarga);

            if (duplicar !== true)
                preencherTabelaFreteClienteAutorizacao(_tabelaFreteCliente.Codigo.val());

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    });
}

function limparCamposTabelaFreteCliente() {

    _CRUDTabelaFreteCliente.Atualizar.visible(false);
    _CRUDTabelaFreteCliente.Cancelar.visible(true);
    _CRUDTabelaFreteCliente.Excluir.visible(false);
    _CRUDTabelaFreteCliente.Adicionar.visible(true);

    _tabelaFreteCliente.TipoOperacao.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.Fronteira.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.TipoCarga.basicTable.CarregarGrid(new Array());

    _tabelaFreteCliente.Origem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.ClienteOrigem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.EstadoOrigem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.RegiaoOrigem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.RotaOrigem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.PaisOrigem.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.CEPsOrigem.val([]);

    _tabelaFreteCliente.Destino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.ClienteDestino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.EstadoDestino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.RegiaoDestino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.RotaDestino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.PaisDestino.basicTable.CarregarGrid(new Array());
    _tabelaFreteCliente.CEPsDestino.val([]);

    _gridTipoOperacao.CarregarGrid([]);
    _gridTipoCarga.CarregarGrid([]);
    _tabelaFreteCliente.TransportadorTerceiro.basicTable.CarregarGrid([]);

    LimparCampos(_tabelaFreteCliente);

    RecarregarGridCEPDestino();
    RecarregarGridCEPOrigem();

    _tabelaFrete = null;

    _tabelaFreteCliente.Subcontratacoes.list = new Array();
    _tabelaFreteCliente.SubcontratacoesGeral.list = new Array();
    recarregarGridSubcontratacao();
    limparCamposSubcontratacao();
    limparTabelaFreteClienteAutorizacao();
    limparCamposPracaPedagio();
    limparCamposModeloVeicularCarga();

    montarTabelaValores();

    ValidarOrigensDisponiveis();
    ValidarDestinosDisponiveis();

    $('.nav-tabs').each(function () {
        $(this).find('li:eq(0) a').tab('show');
    });

    $("#liTabTransportadorTerceiro").addClass("d-none");

    recarregarTabelaFreteClienteIntegracoes();
}

function limparCamposTabelaFreteClienteParcial() {

    _CRUDTabelaFreteCliente.Atualizar.visible(false);
    _CRUDTabelaFreteCliente.Cancelar.visible(true);
    _CRUDTabelaFreteCliente.Excluir.visible(false);
    _CRUDTabelaFreteCliente.Adicionar.visible(true);

    _tabelaFreteCliente.Destino.basicTable.CarregarGrid([]);
    _tabelaFreteCliente.Destinos.val([]);

    _tabelaFreteCliente.EstadosDestino.val([]);
    _tabelaFreteCliente.EstadoDestino.basicTable.CarregarGrid([]);

    _tabelaFreteCliente.RegioesDestino.val([]);
    _tabelaFreteCliente.RegiaoDestino.basicTable.CarregarGrid([]);

    _tabelaFreteCliente.RotasDestino.val([]);
    _tabelaFreteCliente.RotaDestino.basicTable.CarregarGrid(new Array());

    _tabelaFreteCliente.PaisesDestino.val([]);
    _tabelaFreteCliente.PaisDestino.basicTable.CarregarGrid(new Array());

    _tabelaFreteCliente.ClientesDestino.val([]);
    _tabelaFreteCliente.ClienteDestino.basicTable.CarregarGrid([]);

    _tabelaFreteCliente.CEPsDestino.val([]);
    RecarregarGridCEPDestino();

    _tabelaFreteCliente.Valores.val(new Array());
    _tabelaFreteCliente.Observacoes.val(new Array());
    _tabelaFreteCliente.ValoresMinimosGarantidos.val(new Array());
    _tabelaFreteCliente.ValoresBases.val(new Array());
    _tabelaFreteCliente.Subcontratacoes.list = new Array();
    _tabelaFreteCliente.SubcontratacoesGeral.list = new Array();
    _tabelaFreteCliente.Codigo.val(0);

    limparCamposTabelaValores();
    limparCamposSubcontratacao();
    limparTabelaFreteClienteAutorizacao();

    recarregarGridSubcontratacao();

    ValidarDestinosDisponiveis();
}

function ValidarTabelaFrete() {
    var valido = true;
    var mensagem = "";
    var regiao = "região";
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        regiao = "rota";

    if (_tabelaFreteCliente.Destino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.ClienteDestino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.RegiaoDestino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.RotaDestino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.PaisDestino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.EstadoDestino.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.CEPsDestino.val().length <= 0) {
        valido = false;
        mensagem += Localization.Resources.Fretes.TabelaFreteCliente.SelecioneAoMenosUmDestinoCepClienteCidadeEstadoPais.format(regiao);
    }

    if (_tabelaFreteCliente.Origem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.ClienteOrigem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.RegiaoOrigem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.RotaOrigem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.EstadoOrigem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.PaisOrigem.basicTable.BuscarRegistros().length <= 0 &&
        _tabelaFreteCliente.CEPsOrigem.val().length <= 0) {
        valido = false;
        mensagem += Localization.Resources.Fretes.TabelaFreteCliente.SelecioneAoMenosUmaOrigemCepClienteCidadeEstadoPais.format(regiao)
    }

    if (!valido)
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atenção, mensagem);

    return valido;
}

function obterTabelaFreteClienteSalvar() {
    if (!ValidarTabelaFrete())
        return null;

    if (!obterValoresTabelaValores(_CONFIGURACAO_TMS.PossuiIntegracaoLBC)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, "Preencha a Tabela de Valores do Frete com pelo menos um valor");
        return null;
    }
    PreencherListasDeSelecao();

    if (!ValidarCamposObrigatorios(_tabelaFreteCliente)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return null;
    }

    var percentualCobrancaPadrao = Globalize.parseFloat(_subcontratacaoGeral.PercentualCobrancaPadrao.val());
    var percentualCobrancaVeiculoFrota = Globalize.parseFloat(_subcontratacaoGeral.PercentualCobrancaVeiculoFrota.val());

    if (percentualCobrancaPadrao > 100) {
        exibirMensagem(tipoMensagem.atencao, "Percentual", "O percentual 'Cobrança Padrão' não pode ser maior que 100%");
        _subcontratacaoGeral.PercentualCobrancaPadrao.requiredClass("form-control is-invalid");
        return false;
    } else {
        _subcontratacaoGeral.PercentualCobrancaPadrao.requiredClass("form-control");
    }

    if (percentualCobrancaVeiculoFrota > 100) {
        exibirMensagem(tipoMensagem.atencao, "Percentual", "O percentual 'Cobrança Veículo Frota' não pode ser maior que 100%");
        _subcontratacaoGeral.PercentualCobrancaVeiculoFrota.requiredClass("form-control is-invalid");
        return false;
    } else {
        _subcontratacaoGeral.PercentualCobrancaVeiculoFrota.requiredClass("form-control");
    }

    var tabelaFreteCliente = RetornarObjetoPesquisa(_tabelaFreteCliente);

    preencherModeloVeicularCargaSalvar(tabelaFreteCliente);

    return tabelaFreteCliente;
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {}, function (r) {
        if (r.Success) {
            _tipoIntegracao = [{ value: "", text: Localization.Resources.Gerais.Geral.Todos }];

            for (var i = 0; i < r.Data.length; i++)
                _tipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}

function VincularRotasSemPararClick() {
    executarReST("TabelaFreteCliente/VincularRotasSemParar", {}, function (r) {
        if (r.Success) {
            _pesquisaTabelaFreteCliente.VincularRotasSemParar.visible(false);
            _pesquisaTabelaFreteCliente.VincularRotasSemParar.enable(false);
            _pesquisaTabelaFreteCliente.VincularRotasSemParar.text(Localization.Resources.Fretes.TabelaFreteCliente.VinculandoRotas);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ValidarVinculoRotasSemParar() {
    executarReST("TabelaFreteCliente/VerificarVinculoDeRotas", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _pesquisaTabelaFreteCliente.VincularRotasSemParar.visible(r.Data.Vinculadas);
                _pesquisaTabelaFreteCliente.VincularRotasSemParar.enable(r.Data.Vinculadas);

                if (!r.Data.Vinculadas)
                    _pesquisaTabelaFreteCliente.VincularRotasSemParar.text(Localization.Resources.Fretes.TabelaFreteCliente.VinculandoRotas);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ObterIntegracoesHabilitadas() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.OpenTech; })) {
                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe)
                    controlarExibicaoAbaIntegracoes(true);
            }
        }
    });
}

function retornoBuscarContratos(e) {
    _tabelaFreteCliente.ContratoTransporteFrete.val(e.Descricao);
    _tabelaFreteCliente.ContratoTransporteFrete.codEntity(e.Codigo);
    _tabelaFreteCliente.ContratoTransporteFrete.enable(true);
    _tabelaFreteCliente.Empresa.val(e.Transportador);
    _tabelaFreteCliente.Empresa.codEntity(e.CodigoTransportador);

    buscarVigenciaAtualContratoTransportador(_tabelaFreteCliente);
}

function buscarVigenciaAtualContratoTransportador(ko) {
    executarReST("TabelaFrete/BuscarVigenciaAtualContratoTransportador", { ContratoTransporteFrete: ko.ContratoTransporteFrete.codEntity() }, function (arg) {
        if (arg.Success && arg.Data != null)
            retornoBuscarVigenciaContratoTransportador(ko, arg.Data);
    });
}

function retornoBuscarVigenciaContratoTransportador(ko, data) {
    ko.DataInicialContrato.val(data.DataInicial);
    ko.DataFinalContrato.val(data.DataFinal);
}

function reenviarTodasProblemaIntegracaoClick() {
    executarReST("TabelaFreteClienteIntegracao/ReenviarTodasProblemaIntegracao", {}, (arg) => {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ReenvioSolicitadoComSucesso);
            _gridTabelaFreteCliente.CarregarGrid();
        }
        else
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    })
}

function preencherTabelaFreteClienteAutomaticamente() {
    const tabelaFrete = sessionStorage.getItem('tabelaFrete');
    const origem = sessionStorage.getItem('origem');
    const destino = sessionStorage.getItem('destino');
    const clienteOrigens = sessionStorage.getItem('clienteOrigens');
    const clienteDestinos = sessionStorage.getItem('clienteDestinos');
    const estadoOrigens = sessionStorage.getItem('estadoOrigens');
    const estadoDestinos = sessionStorage.getItem('estadoDestinos');
    const regiaoOrigens = sessionStorage.getItem('regiaoOrigens');
    const regiaoDestinos = sessionStorage.getItem('regiaoDestinos');
    const rotasOrigens = sessionStorage.getItem('rotasOrigens');
    const rotasDestinos = sessionStorage.getItem('rotasDestinos');
    const cepOrigens = sessionStorage.getItem('cepOrigens');
    const cepDestinos = sessionStorage.getItem('cepDestinos');
    const paisOrigens = sessionStorage.getItem('paisOrigens');
    const paisDestinos = sessionStorage.getItem('paisDestinos');

    if (tabelaFrete)
        obterDadosTabelaFrete(tabelaFrete);

    if (origem)
        obterDadosOrigem(origem);

    if (destino)
        obterDadosDestino(destino);

    if (clienteOrigens)
        obterDadosClienteOrigem(clienteOrigens);

    if (clienteDestinos)
        obterDadosClienteDestino(clienteDestinos);

    if (estadoOrigens)
        obterDadosEstadoOrigem(estadoOrigens);

    if (estadoDestinos)
        obterDadosEstadoDestino(estadoDestinos);

    if (regiaoOrigens)
        obterDadosRegiaoOrigem(regiaoOrigens);

    if (regiaoDestinos)
        obterDadosRegiaoDestino(regiaoDestinos);

    if (rotasOrigens)
        obterDadosRotasOrigem(rotasOrigens);

    if (rotasDestinos)
        obterDadosRotasDestino(rotasDestinos);

    if (cepOrigens)
        obterDadosCepOrigem(cepOrigens);

    if (cepDestinos)
        obterDadosCepDestino(cepDestinos);

    if (paisOrigens)
        obterDadosPaisOrigem(paisOrigens);

    if (paisDestinos)
        obterDadosPaisDestino(paisDestinos);

    sessionStorage.removeItem('tabelaFrete');
    sessionStorage.removeItem('origem');
    sessionStorage.removeItem('destino');
    sessionStorage.removeItem('clienteOrigens');
    sessionStorage.removeItem('clienteDestinos');
    sessionStorage.removeItem('estadoOrigens');
    sessionStorage.removeItem('estadoDestinos');
    sessionStorage.removeItem('regiaoOrigens');
    sessionStorage.removeItem('regiaoDestinos');
    sessionStorage.removeItem('rotasOrigens');
    sessionStorage.removeItem('rotasDestinos');
    sessionStorage.removeItem('cepOrigens');
    sessionStorage.removeItem('cepDestinos');
    sessionStorage.removeItem('paisOrigens');
    sessionStorage.removeItem('paisDestinos');
}

function obterDadosTabelaFrete(dados) {
    var dadoFormatado = dados.split("-");
    _tabelaFreteCliente.TabelaFrete.codEntity(dadoFormatado[0]);
    _tabelaFreteCliente.TabelaFrete.val(dadoFormatado[1]);
    buscarDadosTabelaFrete();
}

function obterDadosOrigem(dados) {
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

    _gridOrigem.CarregarGrid(data);
}

function obterDadosDestino(dados) {
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

    _gridDestino.CarregarGrid(data);
}

function obterDadosClienteOrigem(dados) {
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

    _gridClienteOrigem.CarregarGrid(data);
}

function obterDadosClienteDestino(dados) {
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

    _gridClienteDestino.CarregarGrid(data);
}

function obterDadosEstadoOrigem(dados) {
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

    _gridEstadoOrigem.CarregarGrid(data);
}

function obterDadosEstadoDestino(dados) {
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

    _gridEstadoDestino.CarregarGrid(data);
}

function obterDadosRegiaoOrigem(dados) {
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

    _gridRegiaoOrigem.CarregarGrid(data);
}

function obterDadosRegiaoDestino(dados) {
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

    _gridRegiaoDestino.CarregarGrid(data);
}

function obterDadosRotasOrigem(dados) {
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

    _gridRotaOrigem.CarregarGrid(data);
}

function obterDadosRotasDestino(dados) {
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

    _gridRotaDestino.CarregarGrid(data);
}

function obterDadosCepOrigem(dados) {
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

    _gridCEPOrigem.CarregarGrid(data);
}

function obterDadosCepDestino(dados) {
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

    _gridCEPDestino.CarregarGrid(data);
}

function obterDadosPaisOrigem(dados) {
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

    _gridPaisOrigem.CarregarGrid(data);
}

function obterDadosPaisDestino(dados) {
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

    _gridPaisDestino.CarregarGrid(data);
}

function verificarIntegracaoLBC(integracaoLBC) {
    if (integracaoLBC) {
        _tabelaFreteCliente.TipoGrupoCarga.options(EnumTipoGrupoCarga.obterOpcoesPesquisaIntegracaoLBC());
        _tabelaFreteCliente.TipoGrupoCarga.val(null);
        _tabelaFreteCliente.TipoGrupoCarga.text(Localization.Resources.Fretes.TabelaFreteCliente.TipoGrupoDeCarga.getRequiredFieldDescription().replace(":", ""));
        _tabelaFreteCliente.EstruturaTabela.options(EnumEstruturaTabela.obterOpcoesIntegracaoLBC());
        _tabelaFreteCliente.EstruturaTabela.val(EnumEstruturaTabela.Selecione);
        _tabelaFreteCliente.EstruturaTabela.text("*Estrutura de Tabela");
    }
}