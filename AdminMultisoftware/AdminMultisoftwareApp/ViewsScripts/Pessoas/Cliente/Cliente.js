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
/// <reference path="ClienteConfiguracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCliente;
var _cliente;
var configuracaoCliente;
var _pesquisaCliente;

var PesquisaCliente = function () {
    this.RazaoSocial = PropertyEntity({ text: "Razão Social: " });
    this.CNPJ = PropertyEntity({ text: "CNPJ: ", getType: typesKnockout.cnpj });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCliente.CarregarGrid();
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

var Cliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RazaoSocial = PropertyEntity({ text: "*Razão Social: ", required: true });
    this.CNPJ = PropertyEntity({ text: "*CNPJ: ", required: true, getType: typesKnockout.cnpj });
    this.IE = PropertyEntity({ text: "*IE: ", required: true });

    this.NomeFantasia = PropertyEntity({ text: "*Nome Fantasia: ", required: true });
    this.Telefone = PropertyEntity({ text: "*Telefone: ", required: true, getType: typesKnockout.phone });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoOperadora = PropertyEntity({ val: ko.observable(1), def: 1, text: "*Tipo Operadora:", options: ko.observable(EnumTipoOperadora.obterOpcoes()), visible: ko.observable(true) });

    this.Email = PropertyEntity({ text: "*Email: ", required: true, getType: typesKnockout.email });

    this.Logradouro = PropertyEntity({ text: "*Logradouro: ", required: true });
    this.Bairro = PropertyEntity({ text: "*Bairro: ", required: true });
    this.CEP = PropertyEntity({ text: "CEP: ", getType: typesKnockout.cep });
    this.Complemento = PropertyEntity({ text: "*Complemento: ", required: true });
    this.Numero = PropertyEntity({ text: "*Numero: ", getType: typesKnockout.int, required: true });

    this.MobileURL = PropertyEntity({ text: "Mobile URL: " });
    this.MobileURLHomologacao = PropertyEntity({ text: "Mobile URL Homologação: " });

    this.BloquearLoginVersaoAntiga = PropertyEntity({ type: types.bool, text: "Bloquear Login na Versão Antiga ", val: ko.observable(false) });
    //this.CodigoReport = PropertyEntity({ text: "Código Report: " });
    //this.CodigoHomologacaoReport = PropertyEntity({ text: "Código Report Homologação: " });

    this.Site = PropertyEntity({ text: "Site: " });
    this.Logo = PropertyEntity({ text: "Logo: " });
    this.LogoLight = PropertyEntity({ text: "Logo Light: " });
    this.HeightLogo = PropertyEntity({ text: "Height Logo: ", getType: typesKnockout.int });

    this.ClienteConfiguracao = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.URLAutenticadaViaCodigoDeIntegracaoDoUsuarioParaPortalMultiClifor = PropertyEntity({ val: ko.observable(false), def: false, text: "URL Autenticada via Código de Integração do Usuário para o Portal MultiClifor." });
}

var CRUDCliente = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCliente() {

    _cliente = new Cliente();
    KoBindings(_cliente, "knockoutCadastroCliente");

    _crudCliente = new CRUDCliente();
    KoBindings(_crudCliente, "knockoutCRUDCliente");

    _pesquisaCliente = new PesquisaCliente();
    KoBindings(_pesquisaCliente, "knockoutPesquisaCliente", false, _pesquisaCliente.Pesquisar.id);

    loadClienteConfiguracao();
    buscarCliente();

    HeaderAuditoria("cliente", _cliente);
}

function adicionarClick(e, sender) {
    if (!validarCamposObrigatoriosConfiguracaoCliente()) {
        return false;
    }
    _cliente.ClienteConfiguracao.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoCliente)));
    Salvar(_cliente, "cliente/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");
                _gridCliente.CarregarGrid();
                limparCliente();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!validarCamposObrigatoriosConfiguracaoCliente()) {
        return false;
    }
    _cliente.ClienteConfiguracao.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoCliente)));
    Salvar(_cliente, "cliente/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCliente.CarregarGrid();
                limparCliente();
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
        ExcluirPorCodigo(_cliente, "cliente/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridCliente.CarregarGrid();
                limparCliente();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCliente();
}

//*******MÉTODOS*******


function buscarCliente() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCliente, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCliente = new GridView(_pesquisaCliente.Pesquisar.idGrid, "Cliente/Pesquisa", _pesquisaCliente, menuOpcoes, null);
    _gridCliente.CarregarGrid();
}

function editarCliente(clienteGrid) {
    limparCliente();
    _cliente.Codigo.val(clienteGrid.Codigo);
    BuscarPorCodigo(_cliente, "cliente/BuscarPorCodigo", function (arg) {
        _pesquisaCliente.ExibirFiltros.visibleFade(false);
        _crudCliente.Atualizar.visible(true);
        _crudCliente.Cancelar.visible(true);
        _crudCliente.Excluir.visible(true);
        _crudCliente.Adicionar.visible(false);

        if (arg.Data.ConfiguracaoCliente != null)
            PreencherObjetoKnout(_configuracaoCliente, { Data: arg.Data.ConfiguracaoCliente });
    }, null);
}

function limparCliente() {
    _crudCliente.Atualizar.visible(false);
    _crudCliente.Cancelar.visible(false);
    _crudCliente.Excluir.visible(false);
    _crudCliente.Adicionar.visible(true);
    LimparCampos(_cliente);
    limparCamposClienteConfiguracao();
}