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

var _gridClienteURLAcesso;
var clienteURLAcesso;
var _pesquisaClienteURLAcesso;

var PesquisaClienteURLAcesso = function () {
    this.URLAcesso = PropertyEntity({ text: "URL de Acesso: " });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.TipoServico = PropertyEntity({ val: ko.observable(""), def: "", text: "Tipo de Serviço:", options: ko.observable(EnumTipoServicoMultisoftware.obterOpcoesPesquisa()), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "*Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridClienteURLAcesso.CarregarGrid();
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

var ClienteURLAcesso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoServico = PropertyEntity({ val: ko.observable(1), def: 1, text: "Tipo de Serviço:", options: ko.observable(EnumTipoServicoMultisoftware.obterOpcoes()), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: true });

    this.URLAcesso = PropertyEntity({ text: "URL de Acesso: ", maxlength: 150 });
    this.URLSistemaEmissaoCte = PropertyEntity({ text: "URL Sistema de Emissão CT-e: ", maxlength: 150 });
    this.URLWebServiceConsultaCte = PropertyEntity({ text: "URL Web Service Consulta CT-e: ", maxlength: 150 });
    this.URLWebServiceOracle = PropertyEntity({ text: "URL Web Service Oracle: ", maxlength: 150 });

    this.Layout = PropertyEntity({ text: "Layout: " });
    this.Logo = PropertyEntity({ text: "Logo: " });
    this.CorFuncionario = PropertyEntity({ text: "Cor Funcionário: " });
    this.Icone = PropertyEntity({ text: "Ícone: " });

    this.LayoutLogin = PropertyEntity({ text: "Layout Login: " });
    this.LogoLogin = PropertyEntity({ text: "Logo Login: " });
    this.CorFuncionarioLogin = PropertyEntity({ text: "Cor Funcionário Login: " });
    this.IconeLogin = PropertyEntity({ text: "Ícone Login: " });

    this.PossuiFila = PropertyEntity({ text: "Possui Fila" });
    this.URLHomologacao = PropertyEntity({ text: "URL de Homologação" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadClienteURLAcesso() {

    clienteURLAcesso = new ClienteURLAcesso();
    KoBindings(clienteURLAcesso, "knockoutCadastroClienteURLAcesso");

    _pesquisaClienteURLAcesso = new PesquisaClienteURLAcesso();
    KoBindings(_pesquisaClienteURLAcesso, "knockoutPesquisaClienteURLAcesso", false, _pesquisaClienteURLAcesso.Pesquisar.id);

    new BuscarCliente(clienteURLAcesso.Cliente);
    new BuscarCliente(_pesquisaClienteURLAcesso.Cliente);

    buscarClienteURLAcesso();

    HeaderAuditoria("ClienteURLAcesso", clienteURLAcesso);
}

function adicionarClick(e, sender) {
    Salvar(e, "ClienteURLAcesso/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _gridClienteURLAcesso.CarregarGrid();
                limparClienteURLAcesso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ClienteURLAcesso/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridClienteURLAcesso.CarregarGrid();
                limparClienteURLAcesso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir?", function () {
        ExcluirPorCodigo(clienteURLAcesso, "ClienteURLAcesso/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridClienteURLAcesso.CarregarGrid();
                limparClienteURLAcesso();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparClienteURLAcesso();
}

//*******MÉTODOS*******


function buscarClienteURLAcesso() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClienteURLAcesso, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridClienteURLAcesso = new GridView(_pesquisaClienteURLAcesso.Pesquisar.idGrid, "ClienteURLAcesso/Pesquisa", _pesquisaClienteURLAcesso, menuOpcoes, null);
    _gridClienteURLAcesso.CarregarGrid();
}

function editarClienteURLAcesso(clienteURLAcessoGrid) {
    limparClienteURLAcesso();
    clienteURLAcesso.Codigo.val(clienteURLAcessoGrid.Codigo);
    BuscarPorCodigo(clienteURLAcesso, "ClienteURLAcesso/BuscarPorCodigo", function (arg) {
        _pesquisaClienteURLAcesso.ExibirFiltros.visibleFade(false);
        clienteURLAcesso.Atualizar.visible(true);
        clienteURLAcesso.Cancelar.visible(true);
        clienteURLAcesso.Excluir.visible(true);
        clienteURLAcesso.Adicionar.visible(false);
    }, null);
}

function limparClienteURLAcesso() {
    clienteURLAcesso.Atualizar.visible(false);
    clienteURLAcesso.Cancelar.visible(false);
    clienteURLAcesso.Excluir.visible(false);
    clienteURLAcesso.Adicionar.visible(true);
    LimparCampos(clienteURLAcesso);
}
