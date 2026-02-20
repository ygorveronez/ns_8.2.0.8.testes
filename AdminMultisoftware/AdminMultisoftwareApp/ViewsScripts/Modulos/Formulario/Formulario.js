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

var _gridFormulario;
var _formulario;
var _pesquisaFormulario;

var PesquisaFormulario = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFormulario.CarregarGrid();
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
}

var Formulario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.codigoFormulario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.CaminhoPagina = PropertyEntity({ text: "*Caminho página: ", required: true, maxlength: 150 });
    this.Sequencia = PropertyEntity({ text: "*Sequência: ", required: true, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Módulo:", idBtnSearch: guid() });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo de Serviço:", options: ko.observable(EnumTipoServicoMultisoftware.obterOpcoes()), visible: ko.observable(true) });
    this.EmHomologacao = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Este formulário está em homologação?", visible: ko.observable(true) });
    this.TranslationResourcePath = PropertyEntity({ text: "Translation Resource Path: ", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadFormulario() {

    _formulario = new Formulario();
    KoBindings(_formulario, "knockoutCadastroFormulario");

    _pesquisaFormulario = new PesquisaFormulario();
    KoBindings(_pesquisaFormulario, "knockoutPesquisaFormulario", false, _pesquisaFormulario.Pesquisar.id);

    new BuscarModulo(_formulario.Modulo);
    new BuscarModulo(_pesquisaFormulario.Modulo);

    buscarFormularios();

    HeaderAuditoria("Formulario", _formulario);
}

function adicionarClick(e, sender) {
    if (_formulario.TipoServico.val().length > 1)
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Selecione apenas um Tipo de Serviço, ou então mantenha como Todos");
    else {
        Salvar(e, "Formulario/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                    _gridFormulario.CarregarGrid();
                    limparCamposFormulario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    if (_formulario.TipoServico.val().length > 1)
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Selecione apenas um Tipo de Serviço, ou então mantenha como Todos");
    else {
        Salvar(e, "Formulario/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridFormulario.CarregarGrid();
                    limparCamposFormulario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o formulário " + _formulario.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_formulario, "Formulario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridFormulario.CarregarGrid();
                limparCamposFormulario();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFormulario();
}

//*******MÉTODOS*******

function buscarFormularios() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFormulario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFormulario = new GridView(_pesquisaFormulario.Pesquisar.idGrid, "Formulario/Pesquisa", _pesquisaFormulario, menuOpcoes, null);
    _gridFormulario.CarregarGrid();
}

function editarFormulario(formularioGrid) {
    limparCamposFormulario();
    _formulario.Codigo.val(formularioGrid.Codigo);
    BuscarPorCodigo(_formulario, "Formulario/BuscarPorCodigo", function (arg) {
        _pesquisaFormulario.ExibirFiltros.visibleFade(false);
        _formulario.Atualizar.visible(true);
        _formulario.Cancelar.visible(true);
        _formulario.Excluir.visible(true);
        _formulario.Adicionar.visible(false);
    }, null);
}

function limparCamposFormulario() {
    _formulario.Atualizar.visible(false);
    _formulario.Cancelar.visible(false);
    _formulario.Excluir.visible(false);
    _formulario.Adicionar.visible(true);
    LimparCampos(_formulario);
}
