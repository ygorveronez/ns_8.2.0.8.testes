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
/// <reference path="../../../ViewsScripts/Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCalculoISS;
var _calculoISS;
var _pesquisaCalculoISS;

var PesquisaCalculoISS = function () {
    this.CodigoServico = PropertyEntity({ text: "Código do Serviço: " });

    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCalculoISS.CarregarGrid();
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

var CalculoISS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoServico = PropertyEntity({ text: "*Código do Serviço: ", required: ko.observable(true), maxlength: 500 });
    this.Aliquota = PropertyEntity({ text: "*Alíquota: ", getType: typesKnockout.decimal, maxlength: 18, required: ko.observable(true) });
    this.PercentualRetencao = PropertyEntity({ text: "% Retenção: ", getType: typesKnockout.decimal, maxlength: 18 });

    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade:", idBtnSearch: guid(), required: ko.observable(true) });
};

var CRUDCalculoISS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCalculoISS() {
    _calculoISS = new CalculoISS();
    KoBindings(_calculoISS, "knockoutCadastroCalculoISS");

    HeaderAuditoria("CalculoISS", _calculoISS);

    _crudCalculoISS = new CRUDCalculoISS();
    KoBindings(_crudCalculoISS, "knockoutCRUDCalculoISS");

    _pesquisaCalculoISS = new PesquisaCalculoISS();
    KoBindings(_pesquisaCalculoISS, "knockoutPesquisaCalculoISS", false, _pesquisaCalculoISS.Pesquisar.id);

    new BuscarLocalidades(_pesquisaCalculoISS.Localidade);
    new BuscarLocalidades(_calculoISS.Localidade);

    buscarCalculoISS();
}

function adicionarClick(e, sender) {
    Salvar(_calculoISS, "CalculoISS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridCalculoISS.CarregarGrid();
                limparCamposCalculoISS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_calculoISS, "CalculoISS/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCalculoISS.CarregarGrid();
                limparCamposCalculoISS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o cálculo de ISS?", function () {
        ExcluirPorCodigo(_calculoISS, "CalculoISS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCalculoISS.CarregarGrid();
                    limparCamposCalculoISS();
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
    limparCamposCalculoISS();
}

//*******MÉTODOS*******

function buscarCalculoISS() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCalculoISS, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCalculoISS = new GridView(_pesquisaCalculoISS.Pesquisar.idGrid, "CalculoISS/Pesquisa", _pesquisaCalculoISS, menuOpcoes);
    _gridCalculoISS.CarregarGrid();
}

function editarCalculoISS(calculoISSGrid) {
    limparCamposCalculoISS();
    _calculoISS.Codigo.val(calculoISSGrid.Codigo);
    BuscarPorCodigo(_calculoISS, "CalculoISS/BuscarPorCodigo", function (arg) {
        _pesquisaCalculoISS.ExibirFiltros.visibleFade(false);
        _crudCalculoISS.Atualizar.visible(true);
        _crudCalculoISS.Cancelar.visible(true);
        _crudCalculoISS.Excluir.visible(true);
        _crudCalculoISS.Adicionar.visible(false);
    }, null);
}

function limparCamposCalculoISS() {
    _crudCalculoISS.Atualizar.visible(false);
    _crudCalculoISS.Cancelar.visible(false);
    _crudCalculoISS.Excluir.visible(false);
    _crudCalculoISS.Adicionar.visible(true);
    LimparCampos(_calculoISS);
}