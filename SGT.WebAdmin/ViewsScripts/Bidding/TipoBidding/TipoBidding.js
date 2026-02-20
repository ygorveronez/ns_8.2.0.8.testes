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
/// <reference path="../../Consultas/ComponenteFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoBidding;
var _tipoBidding;
var _pesquisaTipoBidding;

var PesquisaTipoBidding = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoBidding.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoBidding = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", maxlength: 50, issue: 15 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.ExibirRankOfertas = PropertyEntity({ text: "Exibir Grid de Rank ", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoIncluirImpostoValorTotalOferta = PropertyEntity({ text: "Não incluir imposto no valor total da oferta", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.NaoPossuiPedagioFluxoOferta = PropertyEntity({ text: "Não possui pedágio para o fluxo de oferta", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirOfertasComponentes = PropertyEntity({ text: "Permitir ofertas de componentes", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ComponentesFrete = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "Componentes de frete:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), required: false });

    this.PermitirOfertasComponentes.val.subscribe(function () {
        if (_tipoBidding.PermitirOfertasComponentes.val())
            _tipoBidding.ComponentesFrete.visible(true);
        else
            _tipoBidding.ComponentesFrete.visible(false);
    });
};

var CRUDTipoBidding = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoBidding() {
    _tipoBidding = new TipoBidding();
    KoBindings(_tipoBidding, "knockoutCadastroTipoBidding");

    HeaderAuditoria("TipoBidding", _tipoBidding);

    _crudTipoBidding = new CRUDTipoBidding();
    KoBindings(_crudTipoBidding, "knockoutCRUDTipoBidding");

    _pesquisaTipoBidding = new PesquisaTipoBidding();
    KoBindings(_pesquisaTipoBidding, "knockoutPesquisaTipoBidding", false, _pesquisaTipoBidding.Pesquisar.id);

    BuscarComponentesDeFrete(_tipoBidding.ComponentesFrete);

    buscarTipoBidding();
    loadAnexo();
}

function adicionarClick(e, sender) {
    Salvar(_tipoBidding, "TipoBidding/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");

                enviarArquivosAnexados(arg.Data.Codigo);
                _gridTipoBidding.CarregarGrid();
                limparCamposTipoBidding();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoBidding, "TipoBidding/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoBidding.CarregarGrid();
                limparCamposTipoBidding();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de Bidding " + _tipoBidding.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoBidding, "TipoBidding/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoBidding.CarregarGrid();
                    limparCamposTipoBidding();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoBidding();
}

//*******MÉTODOS*******

function buscarTipoBidding() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoBidding, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoBidding = new GridView(_pesquisaTipoBidding.Pesquisar.idGrid, "TipoBidding/Pesquisa", _pesquisaTipoBidding, menuOpcoes, null);
    _gridTipoBidding.CarregarGrid();
}

function editarTipoBidding(tipoBiddingGrid) {
    limparCamposTipoBidding();
    _tipoBidding.Codigo.val(tipoBiddingGrid.Codigo);
    BuscarPorCodigo(_tipoBidding, "TipoBidding/BuscarPorCodigo", function (arg) {
        _pesquisaTipoBidding.ExibirFiltros.visibleFade(false);
        _listaAnexo.Anexos.val(arg.Data.Anexos.slice());
        _crudTipoBidding.Atualizar.visible(true);
        _crudTipoBidding.Cancelar.visible(true);
        _crudTipoBidding.Excluir.visible(true);
        _crudTipoBidding.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoBidding() {
    _crudTipoBidding.Atualizar.visible(false);
    _crudTipoBidding.Cancelar.visible(false);
    _crudTipoBidding.Excluir.visible(false);
    _crudTipoBidding.Adicionar.visible(true);
    _listaAnexo.Anexos.val([]);
    LimparCampos(_tipoBidding);
}