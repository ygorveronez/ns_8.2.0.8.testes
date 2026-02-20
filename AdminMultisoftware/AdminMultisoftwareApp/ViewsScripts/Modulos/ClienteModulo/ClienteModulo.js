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
/// <reference path="../../Enumeradores/EnumModuloExclusivoBloqueado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridClienteModulo;
var clienteModulo;
var _pesquisaClienteModulo;

var PesquisaClienteModulo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo:", idBtnSearch: guid() });
    this.ModuloExclusivo = PropertyEntity({ val: ko.observable(0), options: EnumModuloExclusivoBloqueado.obterOpcoes(), def: 0, text: "Módulo Exclusivo" });
    this.ModuloBloqueado = PropertyEntity({ val: ko.observable(0), options: EnumModuloExclusivoBloqueado.obterOpcoes(), def: 0, text: "Módulo Bloqueado" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridClienteModulo.CarregarGrid();
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

var ClienteModulo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 150, required: true });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Módulo:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Cliente:", idBtnSearch: guid() });
    this.ModuloExclusivo = PropertyEntity({ type: types.bool, text: "Módulo Exclusivo", val: ko.observable(false) });
    this.ModuloBloqueado = PropertyEntity({ type: types.bool, text: "Módulo Bloqueado", val: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadClienteModulo() {

    clienteModulo = new ClienteModulo();
    KoBindings(clienteModulo, "knockoutCadastroClienteModulo");

    _pesquisaClienteModulo = new PesquisaClienteModulo();
    KoBindings(_pesquisaClienteModulo, "knockoutPesquisaClienteModulo", false, _pesquisaClienteModulo.Pesquisar.id);

    new BuscarCliente(clienteModulo.Cliente);
    new BuscarCliente(_pesquisaClienteModulo.Cliente);
    new BuscarModulo(clienteModulo.Modulo);
    new BuscarModulo(_pesquisaClienteModulo.Modulo);

    buscarClienteModulo();

    HeaderAuditoria("ClienteModulo", clienteModulo);
}

function adicionarClick(e, sender) {
    Salvar(e, "ClienteModulo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridClienteModulo.CarregarGrid();
                limparClienteModulo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ClienteModulo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridClienteModulo.CarregarGrid();
                limparClienteModulo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Módulo do Cliente?", function () {
        ExcluirPorCodigo(clienteModulo, "ClienteModulo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridClienteModulo.CarregarGrid();
                limparClienteModulo();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparClienteModulo();
}

//*******MÉTODOS*******


function buscarClienteModulo() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClienteModulo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridClienteModulo = new GridView(_pesquisaClienteModulo.Pesquisar.idGrid, "ClienteModulo/Pesquisa", _pesquisaClienteModulo, menuOpcoes, null);
    _gridClienteModulo.CarregarGrid();
}

function editarClienteModulo(clienteModuloGrid) {
    limparClienteModulo();
    clienteModulo.Codigo.val(clienteModuloGrid.Codigo);
    BuscarPorCodigo(clienteModulo, "ClienteModulo/BuscarPorCodigo", function (arg) {
        _pesquisaClienteModulo.ExibirFiltros.visibleFade(false);
        clienteModulo.Atualizar.visible(true);
        clienteModulo.Cancelar.visible(true);
        clienteModulo.Excluir.visible(true);
        clienteModulo.Adicionar.visible(false);
    }, null);
}

function limparClienteModulo() {
    clienteModulo.Atualizar.visible(false);
    clienteModulo.Cancelar.visible(false);
    clienteModulo.Excluir.visible(false);
    clienteModulo.Adicionar.visible(true);
    LimparCampos(clienteModulo);
}
