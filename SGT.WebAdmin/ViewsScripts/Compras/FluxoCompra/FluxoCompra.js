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
/// <reference path="../../Consultas/MotivoCompra.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFluxoCompra.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoCompra.js" />
/// <reference path="../../Enumeradores/EnumTratativaFluxoCompra.js" />
/// <reference path="Pesquisa.js" />
/// <reference path="Etapa.js" />
/// <reference path="AprovacaoRequisicao.js" />
/// <reference path="Cotacao.js" />
/// <reference path="CotacaoRetorno.js" />
/// <reference path="FluxoCompraOrdemCompra.js" />
/// <reference path="AprovacaoOrdemCompra.js" />
/// <reference path="RecebimentoProduto.js" />
/// <reference path="FluxoCompraTratativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fluxoCompra;
var _CRUDFluxoCompra;
var _gridSelecaoRequisicoesMercadoria;

var FluxoCompra = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SituacaoTratativa = PropertyEntity({ val: ko.observable(EnumTratativaFluxoCompra.Todos), def: EnumTratativaFluxoCompra.Todos });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFluxoCompra.Todos), def: EnumSituacaoFluxoCompra.Todos });
    this.EtapaAtual = PropertyEntity({ val: ko.observable(EnumEtapaFluxoCompra.Todos), def: EnumEtapaFluxoCompra.Todos });
    this.ClicouNaPesquisa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.VoltouParaEtapaAtual = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    //Filtros requisições
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisarRequisicoesMercadoriaClick, type: types.event, text: "Pesquisar", enable: ko.observable(true) });

    this.ListaRequisicoesMercadoria = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.RequisicoesMercadoria = PropertyEntity({ idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
};

var CRUDFluxoCompra = function () {
    this.Iniciar = PropertyEntity({ eventClick: IniciarClick, type: types.event, text: "Iniciar Fluxo de Compra", icon: "fal fa-chevron-right", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", icon: "fal fa-undo", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarFluxoCompraClick, type: types.event, text: "Cancelar", icon: "fal fa-ban", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadFluxoCompra() {
    _fluxoCompra = new FluxoCompra();
    KoBindings(_fluxoCompra, "knockoutCadastroFluxoCompra");

    HeaderAuditoria("FluxoCompra", _fluxoCompra);

    _CRUDFluxoCompra = new CRUDFluxoCompra();
    KoBindings(_CRUDFluxoCompra, "knockoutCRUDFluxoCompra");

    BuscarMotivoCompra(_fluxoCompra.Motivo);
    BuscarProdutoTMS(_fluxoCompra.Produto);

    LoadEtapaFluxoCompra();
    LoadPesquisaFluxoCompra();
    LoadAprovacaoRequisicao();
    LoadCotacao();
    LoadCotacaoRetorno();
    LoadFluxoCompraOrdemCompra();
    LoadAprovacaoOrdemCompra();
    LoadRecebimentoProduto();
    LoadFluxoCompraTratativa();
    BuscarRequisicoesMercadoria();
}

function IniciarClick() {
    preencheRequisicoesMercadoria();

    Salvar(_fluxoCompra, "FluxoCompra/Iniciar", function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_fluxoCompra, r);
                _CRUDFluxoCompra.Iniciar.visible(false);
                _CRUDFluxoCompra.Cancelar.visible(true);

                SetarEnableCamposKnockout(_fluxoCompra, false);

                controleCamposAprovacaoRequisicao();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo de compra iniciado com sucesso!");

                PesquisarRequisicoesMercadoriaClick();

                RecarregarGridPesquisa();
                SetarEtapaFluxoCompra();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarFluxoCompraClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar o fluxo de compra?", function () {
        executarReST("FluxoCompra/Cancelar", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo de compra cancelado com sucesso!");
                    LimparCamposFluxoCompra();
                    RecarregarGridPesquisa();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function PesquisarRequisicoesMercadoriaClick() {
    _fluxoCompra.ClicouNaPesquisa.val(true);
    _fluxoCompra.SelecionarTodos.visible(true);
    _fluxoCompra.SelecionarTodos.val(false);

    _gridSelecaoRequisicoesMercadoria.CarregarGrid();
}

function LimparCamposClick() {
    LimparCamposFluxoCompra();
}

////*******MÉTODOS*******

function preencheRequisicoesMercadoria() {
    var requisicoesSelecionadas;

    if (_fluxoCompra.SelecionarTodos.val())
        requisicoesSelecionadas = _gridSelecaoRequisicoesMercadoria.ObterMultiplosNaoSelecionados();
    else
        requisicoesSelecionadas = _gridSelecaoRequisicoesMercadoria.ObterMultiplosSelecionados();

    var codigosRequisicoes = new Array();

    for (var i = 0; i < requisicoesSelecionadas.length; i++)
        codigosRequisicoes.push(requisicoesSelecionadas[i].DT_RowId);

    if (codigosRequisicoes.length > 0 || _fluxoCompra.SelecionarTodos.val())
        _fluxoCompra.ListaRequisicoesMercadoria.val(JSON.stringify(codigosRequisicoes));
    else
        _fluxoCompra.ListaRequisicoesMercadoria.val("");
}

function BuscarRequisicoesMercadoria() {
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _fluxoCompra.SelecionarTodos,
        somenteLeitura: false
    };

    _gridSelecaoRequisicoesMercadoria = new GridView(_fluxoCompra.RequisicoesMercadoria.idGrid, "FluxoCompra/PesquisaRequisicaoMercadoria", _fluxoCompra, null, null, null, null, null, null, multiplaescolha);
    _gridSelecaoRequisicoesMercadoria.CarregarGrid();
}

function EditarFluxoCompra(fluxoCompraGrid) {
    LimparCamposFluxoCompra();

    _fluxoCompra.Codigo.val(fluxoCompraGrid.Codigo);

    BuscarPorCodigo(_fluxoCompra, "FluxoCompra/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                setarCotacaoFluxoCompra(data.CodigoCotacao);

                _pesquisaFluxoCompra.ExibirFiltros.visibleFade(false);
                _CRUDFluxoCompra.Iniciar.visible(false);
                if (_fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto)
                    _CRUDFluxoCompra.Cancelar.visible(true);

                SetarEnableCamposKnockout(_fluxoCompra, false);

                PesquisarRequisicoesMercadoriaClick();

                controleCamposAprovacaoRequisicao();
                controleCamposCotacaoFluxoCompra();
                controleCamposCotacaoRetornoFluxoCompra();
                controleCamposFluxoCompraOrdemCompra();
                controleCamposAprovacaoOrdemCompra();
                controleCamposRecebimentoProduto();

                SetarEtapaFluxoCompra();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LimparCamposFluxoCompra() {
    _CRUDFluxoCompra.Iniciar.visible(true);
    _CRUDFluxoCompra.Cancelar.visible(false);

    SetarEnableCamposKnockout(_fluxoCompra, true);

    LimparCampos(_fluxoCompra);
    LimparCamposAprovacaoRequisicao();
    LimparCamposCotacaoFluxoCompra();
    LimparCamposCotacaoRetornoFluxoCompra();
    LimparCamposFluxoCompraOrdemCompra();
    LimparCamposAprovacaoOrdemCompra();
    LimparCamposRecebimentoProduto();

    SetarEtapaInicioFluxoCompra();

    _gridSelecaoRequisicoesMercadoria.CarregarGrid();
}