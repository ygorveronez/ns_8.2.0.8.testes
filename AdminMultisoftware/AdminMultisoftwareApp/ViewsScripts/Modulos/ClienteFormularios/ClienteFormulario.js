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
/// <reference path="../../Enumeradores/EnumFormularioExclusivoBloqueado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridClienteFormulario;
var clienteFormulario;
var _pesquisaClienteFormulario;

var PesquisaClienteFormulario = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Formulario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Formulário:", idBtnSearch: guid() });
    this.FormularioExclusivo = PropertyEntity({ val: ko.observable(0), options: EnumFormularioExclusivoBloqueado.obterOpcoes(), def: 0, text: "Formulário Exclusivo" });
    this.FormularioBloqueado = PropertyEntity({ val: ko.observable(0), options: EnumFormularioExclusivoBloqueado.obterOpcoes(), def: 0, text: "Formulário Bloqueado" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridClienteFormulario.CarregarGrid();
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

var ClienteFormulario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 150, required: true });
    this.Formulario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Formulário:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cliente:", idBtnSearch: guid() });
    this.FormularioExclusivo = PropertyEntity({ type: types.bool, text: "Formulário Exclusivo", val: ko.observable(false) });
    this.FormularioBloqueado = PropertyEntity({ type: types.bool, text: "Formulário Bloqueado", val: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadClienteFormulario() {

    clienteFormulario = new ClienteFormulario();
    KoBindings(clienteFormulario, "knockoutCadastroClienteFormulario");

    _pesquisaClienteFormulario = new PesquisaClienteFormulario();
    KoBindings(_pesquisaClienteFormulario, "knockoutPesquisaClienteFormulario", false, _pesquisaClienteFormulario.Pesquisar.id);

    new BuscarFormulario(clienteFormulario.Formulario);
    new BuscarFormulario(_pesquisaClienteFormulario.Formulario);
    new BuscarCliente(clienteFormulario.Cliente);
    new BuscarCliente(_pesquisaClienteFormulario.Cliente);

    buscarClienteFormulario();

    HeaderAuditoria("ClienteFormulario", clienteFormulario);
}

function adicionarClick(e, sender) {
    Salvar(e, "ClienteFormulario/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridClienteFormulario.CarregarGrid();
                limparClienteFormulario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ClienteFormulario/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridClienteFormulario.CarregarGrid();
                limparClienteFormulario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o formulario de cliente?", function () {
        ExcluirPorCodigo(clienteFormulario, "ClienteFormulario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridClienteFormulario.CarregarGrid();
                limparClienteFormulario();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparClienteFormulario();
}

//*******MÉTODOS*******


function buscarClienteFormulario() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClienteFormulario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridClienteFormulario = new GridView(_pesquisaClienteFormulario.Pesquisar.idGrid, "ClienteFormulario/Pesquisa", _pesquisaClienteFormulario, menuOpcoes, null);
    _gridClienteFormulario.CarregarGrid();
}

function editarClienteFormulario(clienteFormularioGrid) {
    limparClienteFormulario();
    clienteFormulario.Codigo.val(clienteFormularioGrid.Codigo);
    BuscarPorCodigo(clienteFormulario, "ClienteFormulario/BuscarPorCodigo", function (arg) {
        _pesquisaClienteFormulario.ExibirFiltros.visibleFade(false);
        clienteFormulario.Atualizar.visible(true);
        clienteFormulario.Cancelar.visible(true);
        clienteFormulario.Excluir.visible(true);
        clienteFormulario.Adicionar.visible(false);
    }, null);
}

function limparClienteFormulario() {
    clienteFormulario.Atualizar.visible(false);
    clienteFormulario.Cancelar.visible(false);
    clienteFormulario.Excluir.visible(false);
    clienteFormulario.Adicionar.visible(true);
    LimparCampos(clienteFormulario);
}
