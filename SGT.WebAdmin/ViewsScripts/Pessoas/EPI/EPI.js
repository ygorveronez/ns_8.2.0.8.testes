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
/// <reference path="../../Consultas/TipoEPI.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEPI;
var _epi;
var _pesquisaEPI;

var PesquisaEPI = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEPI.CarregarGrid();
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

var EPI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoEPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de EPI:", idBtnSearch: guid() });

    this.Tamanho = PropertyEntity({ getType: typesKnockout.int, text: "Tamanho:", val: ko.observable(""), def: "", maxlength: 11, visible: ko.observable(false) });
    this.DiasRevisao = PropertyEntity({ getType: typesKnockout.int, text: "Dias para revisão:", val: ko.observable(""), def: "", maxlength: 11, visible: ko.observable(false) });
    this.DiasValidade = PropertyEntity({ getType: typesKnockout.int, text: "Dias para validade:", val: ko.observable(""), def: "", maxlength: 11, visible: ko.observable(false) });
    this.Descartavel = PropertyEntity({ getType: typesKnockout.bool, text: "É descartável?", val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.InstrucaoUso = PropertyEntity({ getType: typesKnockout.string, text: "Instruções de uso:", val: ko.observable(""), def: "", maxlength: 3000, visible: ko.observable(false) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.NumeroCertificado = PropertyEntity({ getType: typesKnockout.string, text: "Número do Certificado:", val: ko.observable(""), def: "", maxlength: 50, visible: ko.observable(false) });
    this.Caracteristica = PropertyEntity({ getType: typesKnockout.string, text: "Características:", val: ko.observable(""), def: "", maxlength: 3000, visible: ko.observable(false) });

    this.TipoEPI.codEntity.subscribe(function (valorNovo) {
        if (valorNovo == 0)
        {
            _epi.Tamanho.visible(false);
            _epi.DiasRevisao.visible(false);
            _epi.DiasValidade.visible(false);
            _epi.Descartavel.visible(false);
            _epi.InstrucaoUso.visible(false);
            _epi.Valor.visible(false);
            _epi.NumeroCertificado.visible(false);
            _epi.Caracteristica.visible(false);
        }
    });
};

var CRUDEPI = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadEPI() {
    _epi = new EPI();
    KoBindings(_epi, "knockoutCadastroEPI");

    HeaderAuditoria("EPI", _epi);

    _crudEPI = new CRUDEPI();
    KoBindings(_crudEPI, "knockoutCRUDEPI");

    _pesquisaEPI = new PesquisaEPI();
    KoBindings(_pesquisaEPI, "knockoutPesquisaEPI", false, _pesquisaEPI.Pesquisar.id);

    new BuscarTipoEPI(_epi.TipoEPI, alterarVisibilidadeCampos);

    buscarEPI();
}

function adicionarClick(e, sender) {
    Salvar(_epi, "EPI/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridEPI.CarregarGrid();
                limparCamposEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_epi, "EPI/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEPI.CarregarGrid();
                limparCamposEPI();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de EPI " + _epi.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_epi, "EPI/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridEPI.CarregarGrid();
                    limparCamposEPI();
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
    limparCamposEPI();
}

//*******MÉTODOS*******

function buscarEPI() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarEPI, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridEPI = new GridView(_pesquisaEPI.Pesquisar.idGrid, "EPI/Pesquisa", _pesquisaEPI, menuOpcoes);
    _gridEPI.CarregarGrid();
}

function editarEPI(EPIGrid) {
    limparCamposEPI();
    _epi.Codigo.val(EPIGrid.Codigo);
    BuscarPorCodigo(_epi, "EPI/BuscarPorCodigo", function (arg) {
        alterarVisibilidadeCampos(arg.Data.TipoEPI);
        _pesquisaEPI.ExibirFiltros.visibleFade(false);
        _crudEPI.Atualizar.visible(true);
        _crudEPI.Cancelar.visible(true);
        _crudEPI.Excluir.visible(true);
        _crudEPI.Adicionar.visible(false);
    }, null);
}

function limparCamposEPI() {
    _crudEPI.Atualizar.visible(false);
    _crudEPI.Cancelar.visible(false);
    _crudEPI.Excluir.visible(false);
    _crudEPI.Adicionar.visible(true);
    LimparCampos(_epi);
}

function alterarVisibilidadeCampos(data) {
    _epi.TipoEPI.codEntity(data.Codigo);
    _epi.TipoEPI.val(data.Descricao);
    _epi.Tamanho.visible(data.Tamanho);
    _epi.DiasRevisao.visible(data.DiasRevisao);
    _epi.DiasValidade.visible(data.DiasValidade);
    _epi.Descartavel.visible(data.Descartavel);
    _epi.InstrucaoUso.visible(data.InstrucaoUso);
    _epi.Valor.visible(data.Valor);
    _epi.NumeroCertificado.visible(data.NumeroCertificado);
    _epi.Caracteristica.visible(data.Caracteristica);
}