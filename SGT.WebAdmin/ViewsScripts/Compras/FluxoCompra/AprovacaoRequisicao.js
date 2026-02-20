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
/// <reference path="FluxoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aprovacaoRequisicao;
var _CRUDAprovacaoRequisicao;
var _gridAprovacaoRequisicoesMercadoria;

var AprovacaoRequisicao = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ClicouNaPesquisa = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });

    //Filtros requisições
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisa = PropertyEntity({ eventClick: PesquisarAprovacaoRequisicoesMercadoriaClick, type: types.event, text: "Pesquisar", enable: ko.observable(true) });

    this.RequisicoesMercadoria = PropertyEntity({ idGrid: guid() });
};

var CRUDAprovacaoRequisicao = function () {
    this.GerarCotacao = PropertyEntity({ eventClick: GerarCotacaoClick, type: types.event, text: "Gerar Cotação das Requisições", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadAprovacaoRequisicao() {
    _aprovacaoRequisicao = new AprovacaoRequisicao();
    KoBindings(_aprovacaoRequisicao, "knockoutAprovacaoRequisicao");

    _CRUDAprovacaoRequisicao = new CRUDAprovacaoRequisicao();
    KoBindings(_CRUDAprovacaoRequisicao, "knockoutCRUDAprovacaoRequisicao");

    new BuscarMotivoCompra(_aprovacaoRequisicao.Motivo);
    new BuscarProdutoTMS(_aprovacaoRequisicao.Produto);

    LoadAprovacaoFluxoCompra();
    carregarModalDetalhesRequisicao("ModalDetalhesRequisicao");
}

function GerarCotacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja gerar a cotação?", function () {
        executarReST("FluxoCompraCotacao/GerarCotacao", { Codigo: _aprovacaoRequisicao.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    var data = r.Data;
                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.Cotacao);
                    setarCotacaoFluxoCompra(data);

                    controleCamposAprovacaoRequisicao();
                    controleCamposCotacaoFluxoCompra();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cotação gerada com sucesso!");

                    RecarregarGridPesquisa();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function PesquisarAprovacaoRequisicoesMercadoriaClick() {
    CarregarAprovacaoFluxoCompra();
}

////*******MÉTODOS*******

function CarregarAprovacaoFluxoCompra() {
    _gridAprovacaoRequisicoesMercadoria.CarregarGrid();
}

function LoadAprovacaoFluxoCompra() {
    var editar = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalharRequisicao, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAprovacaoRequisicoesMercadoria = new GridView(_aprovacaoRequisicao.RequisicoesMercadoria.idGrid, "FluxoCompra/PesquisaRequisicaoMercadoria", _aprovacaoRequisicao, menuOpcoes);
}

function controleCamposAprovacaoRequisicao() {
    _aprovacaoRequisicao.Codigo.val(_fluxoCompra.Codigo.val());

    _CRUDAprovacaoRequisicao.GerarCotacao.visible(false);
    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.AprovacaoRequisicao && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto)
        _CRUDAprovacaoRequisicao.GerarCotacao.visible(true);
    else
        SetarEnableCamposKnockout(_aprovacaoRequisicao, false);
}

function LimparCamposAprovacaoRequisicao() {
    _CRUDAprovacaoRequisicao.GerarCotacao.visible(false);

    SetarEnableCamposKnockout(_aprovacaoRequisicao, true);

    LimparCampos(_aprovacaoRequisicao);
}