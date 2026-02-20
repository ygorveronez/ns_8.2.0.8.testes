/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscalInutilizar;
var _notaFiscalInutilizar;
var _pesquisaNotaFiscalInutilizar;

var PesquisaNotaFiscalInutilizar = function () {
    this.Justificativa = PropertyEntity({ text: "Justificativa: " });
    this.NumeroInicial = PropertyEntity({ text: "Numero Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Numero Final: ", getType: typesKnockout.int });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotaFiscalInutilizar.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var NotaFiscalInutilizar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", required: true, maxlength: 255 });
    this.Modelo = PropertyEntity({ val: ko.observable("55"), def: "55", text: "*Modelo: ", required: true, maxlength: 2 });

    this.NumeroInicial = PropertyEntity({ text: "*Número Inicial: ", required: true, getType: typesKnockout.int, maxlength: 10 });
    this.NumeroFinal = PropertyEntity({ text: "*Número Final: ", required: true, getType: typesKnockout.int, maxlength: 10 });

    this.EmpresaSerie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Série:", idBtnSearch: guid(), required: false, visible: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: false, visible: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadNotaFiscalInutilizar() {

    _pesquisaNotaFiscalInutilizar = new PesquisaNotaFiscalInutilizar();
    KoBindings(_pesquisaNotaFiscalInutilizar, "knockoutPesquisaNotaFiscalInutilizar", false, _pesquisaNotaFiscalInutilizar.Pesquisar.id);

    _notaFiscalInutilizar = new NotaFiscalInutilizar();
    KoBindings(_notaFiscalInutilizar, "knockoutCadastroNotaFiscalInutilizar");

    HeaderAuditoria("NotaFiscalInutilizar", _notaFiscalInutilizar);

    buscarNotaFiscalInutilizars();
}

function adicionarClick(e, sender) {
    Salvar(e, "NotaFiscalInutilizar/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridNotaFiscalInutilizar.CarregarGrid();
                limparCamposNotaFiscalInutilizar();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "NotaFiscalInutilizar/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridNotaFiscalInutilizar.CarregarGrid();
                limparCamposNotaFiscalInutilizar();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposNotaFiscalInutilizar();
}

//*******MÉTODOS*******


function buscarNotaFiscalInutilizars() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNotaFiscalInutilizar, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNotaFiscalInutilizar = new GridView(_pesquisaNotaFiscalInutilizar.Pesquisar.idGrid, "NotaFiscalInutilizar/Pesquisa", _pesquisaNotaFiscalInutilizar, menuOpcoes, null);
    _gridNotaFiscalInutilizar.CarregarGrid();
}

function editarNotaFiscalInutilizar(notaFiscalInutilizarGrid) {
    limparCamposNotaFiscalInutilizar();
    _notaFiscalInutilizar.Codigo.val(notaFiscalInutilizarGrid.Codigo);
    BuscarPorCodigo(_notaFiscalInutilizar, "NotaFiscalInutilizar/BuscarPorCodigo", function (arg) {
        _pesquisaNotaFiscalInutilizar.ExibirFiltros.visibleFade(false);
        _notaFiscalInutilizar.Atualizar.visible(true);
        _notaFiscalInutilizar.Cancelar.visible(true);
        _notaFiscalInutilizar.Adicionar.visible(false);
    }, null);
}

function limparCamposNotaFiscalInutilizar() {
    _notaFiscalInutilizar.Atualizar.visible(false);
    _notaFiscalInutilizar.Cancelar.visible(false);
    _notaFiscalInutilizar.Adicionar.visible(true);
    LimparCampos(_notaFiscalInutilizar);
}
