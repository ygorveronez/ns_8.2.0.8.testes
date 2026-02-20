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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridJustificativaDeIndisponibilidade;
var _justificativaDeIndisponibilidade;
var _pesquisaDeIndisponibilidade;

var PesquisaDeIndisponibilidade = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativaDeIndisponibilidade.CarregarGrid();
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

var JustificativaDeIndisponibilidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDJustificativaDeIndisponibilidade = function () {
    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadJustificativaDeIndisponibilidade() {
    _justificativaDeIndisponibilidade = new JustificativaDeIndisponibilidade();
    KoBindings(_justificativaDeIndisponibilidade, "knockoutCadastroJustificativaDeIndisponibilidade");

    HeaderAuditoria("JustificativaDeIndisponibilidade", _justificativaDeIndisponibilidade);

    _crudJustificativaDeIndisponibilidade = new CRUDJustificativaDeIndisponibilidade();
    KoBindings(_crudJustificativaDeIndisponibilidade, "knockoutCRUDJustificativaDeIndisponibilidade");

    _pesquisaDeIndisponibilidade = new PesquisaDeIndisponibilidade();
    KoBindings(_pesquisaDeIndisponibilidade, "JustificativaDeIndisponibilidade", false, _pesquisaDeIndisponibilidade.Pesquisar.id);

    buscarJustificativaDeIndisponibilidade();
}

function salvarClick(e, sender) {
    var inserindo = (_justificativaDeIndisponibilidade.Codigo.val() == 0);
    
    Salvar(_justificativaDeIndisponibilidade, "JustificativaIndisponibilidadeFrota/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (inserindo)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                else
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                _gridJustificativaDeIndisponibilidade.CarregarGrid();
                limparCamposJustificativaDeIndisponibilidade();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir essa Justificativa? " + _justificativaDeIndisponibilidade.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_justificativaDeIndisponibilidade, "JustificativaIndisponibilidadeFrota/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridJustificativaDeIndisponibilidade.CarregarGrid();
                    limparCamposJustificativaDeIndisponibilidade();
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
    limparCamposJustificativaDeIndisponibilidade();
}

//*******MÉTODOS*******

function buscarJustificativaDeIndisponibilidade() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarJustificativaDeIndisponibilidade, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridJustificativaDeIndisponibilidade = new GridView(_pesquisaDeIndisponibilidade.Pesquisar.idGrid, "JustificativaIndisponibilidadeFrota/Pesquisa", _pesquisaDeIndisponibilidade, menuOpcoes);
    _gridJustificativaDeIndisponibilidade.CarregarGrid();
}

function editarJustificativaDeIndisponibilidade(justificativaDeIndisponibilidadeGrid) {
    limparCamposJustificativaDeIndisponibilidade();
    _justificativaDeIndisponibilidade.Codigo.val(justificativaDeIndisponibilidadeGrid.Codigo);
    BuscarPorCodigo(_justificativaDeIndisponibilidade, "JustificativaIndisponibilidadeFrota/BuscarPorCodigo", function (arg) {
        _pesquisaDeIndisponibilidade.ExibirFiltros.visibleFade(false);
        _crudJustificativaDeIndisponibilidade.Salvar.visible(true);
        _crudJustificativaDeIndisponibilidade.Cancelar.visible(true);
        _crudJustificativaDeIndisponibilidade.Excluir.visible(true);
    }, null);
}

function limparCamposJustificativaDeIndisponibilidade() {
    _crudJustificativaDeIndisponibilidade.Cancelar.visible(false);
    _crudJustificativaDeIndisponibilidade.Excluir.visible(false);
    _crudJustificativaDeIndisponibilidade.Salvar.visible(true);
    LimparCampos(_justificativaDeIndisponibilidade);
}