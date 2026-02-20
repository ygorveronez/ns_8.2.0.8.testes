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
/// <reference path="../../Consultas/MarcaEPI.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoEPI;
var _tipoEPI;
var _pesquisaTipoEPI;

var PesquisaTipoEPI = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoEPI.CarregarGrid();
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

var TipoEPI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.MarcaEPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca do EPI:", idBtnSearch: guid() });

    this.Tamanho = PropertyEntity({ getType: typesKnockout.bool, text: "Tamanho", val: ko.observable(false), def: false });
    this.DiasRevisao = PropertyEntity({ getType: typesKnockout.bool, text: "Dias para revisão", val: ko.observable(false), def: false });
    this.DiasValidade = PropertyEntity({ getType: typesKnockout.bool, text: "Dias para validade", val: ko.observable(false), def: false });
    this.Descartavel = PropertyEntity({ getType: typesKnockout.bool, text: "É descartável?", val: ko.observable(false), def: false });
    this.InstrucaoUso = PropertyEntity({ getType: typesKnockout.bool, text: "Instruções de uso", val: ko.observable(false), def: false });
    this.Valor = PropertyEntity({ getType: typesKnockout.bool, text: "Valor", val: ko.observable(false), def: false });
    this.NumeroCertificado = PropertyEntity({ getType: typesKnockout.bool, text: "Número do Certificado", val: ko.observable(false), def: false });
    this.Caracteristica = PropertyEntity({ getType: typesKnockout.bool, text: "Características", val: ko.observable(false), def: false });
};

var CRUDTipoEPI = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTipoEPI() {
    _tipoEPI = new TipoEPI();
    KoBindings(_tipoEPI, "knockoutCadastroTipoEPI");

    HeaderAuditoria("TipoEPI", _tipoEPI);

    _crudTipoEPI = new CRUDTipoEPI();
    KoBindings(_crudTipoEPI, "knockoutCRUDTipoEPI");

    _pesquisaTipoEPI = new PesquisaTipoEPI();
    KoBindings(_pesquisaTipoEPI, "knockoutPesquisaTipoEPI", false, _pesquisaTipoEPI.Pesquisar.id);

    new BuscarMarcaEPI(_tipoEPI.MarcaEPI);

    buscarTipoEPI();
}

function adicionarClick(e, sender) {
    Salvar(_tipoEPI, "TipoEPI/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoEPI.CarregarGrid();
                limparCamposTipoEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoEPI, "TipoEPI/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoEPI.CarregarGrid();
                limparCamposTipoEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de EPI " + _tipoEPI.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoEPI, "TipoEPI/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoEPI.CarregarGrid();
                    limparCamposTipoEPI();
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
    limparCamposTipoEPI();
}

//*******MÉTODOS*******

function buscarTipoEPI() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoEPI, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoEPI = new GridView(_pesquisaTipoEPI.Pesquisar.idGrid, "TipoEPI/Pesquisa", _pesquisaTipoEPI, menuOpcoes);
    _gridTipoEPI.CarregarGrid();
}

function editarTipoEPI(tipoEPIGrid) {
    limparCamposTipoEPI();
    _tipoEPI.Codigo.val(tipoEPIGrid.Codigo);
    BuscarPorCodigo(_tipoEPI, "TipoEPI/BuscarPorCodigo", function (arg) {
        _pesquisaTipoEPI.ExibirFiltros.visibleFade(false);
        _crudTipoEPI.Atualizar.visible(true);
        _crudTipoEPI.Cancelar.visible(true);
        _crudTipoEPI.Excluir.visible(true);
        _crudTipoEPI.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoEPI() {
    _crudTipoEPI.Atualizar.visible(false);
    _crudTipoEPI.Cancelar.visible(false);
    _crudTipoEPI.Excluir.visible(false);
    _crudTipoEPI.Adicionar.visible(true);
    LimparCampos(_tipoEPI);
}