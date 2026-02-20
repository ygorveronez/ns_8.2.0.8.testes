/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContaTransportador;
var _contaTransportador;
var _pesquisaContaTransportador;

var PesquisaContaTransportador = function () {
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco: ", idBtnSearch: guid() });
    this.NumeroAgencia = PropertyEntity({ text: "Agência:", maxlength: 10 });
    this.NumeroConta = PropertyEntity({ text: "Número Conta:", maxlength: 10 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContaTransportador.CarregarGrid();
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

var ContaTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banco: ", issue: 49, idBtnSearch: guid(), required: true });

    this.NumeroAgencia = PropertyEntity({ text: "*Agência:", required: true, maxlength: 10 });
    this.DigitoAgencia = PropertyEntity({ text: "*Dígito: ", required: true, maxlength: 1 });
    this.NumeroConta = PropertyEntity({ text: "*Número Conta:", required: true, maxlength: 10 });

    this.TipoContaBanco = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: "*Tipo Conta: ", required: true });
};

var CRUDContaTransportador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContaTransportador() {
    _contaTransportador = new ContaTransportador();
    KoBindings(_contaTransportador, "knockoutCadastroContaTransportador");

    HeaderAuditoria("ContaTransportador", _contaTransportador);

    _crudContaTransportador = new CRUDContaTransportador();
    KoBindings(_crudContaTransportador, "knockoutCRUDContaTransportador");

    _pesquisaContaTransportador = new PesquisaContaTransportador();
    KoBindings(_pesquisaContaTransportador, "knockoutPesquisaContaTransportador", false, _pesquisaContaTransportador.Pesquisar.id);

    new BuscarBanco(_contaTransportador.Banco);
    new BuscarBanco(_pesquisaContaTransportador.Banco);

    buscarContaTransportador();
}

function adicionarClick(e, sender) {
    Salvar(_contaTransportador, "ContaTransportador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContaTransportador.CarregarGrid();
                limparCamposContaTransportador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_contaTransportador, "ContaTransportador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContaTransportador.CarregarGrid();
                limparCamposContaTransportador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Conta do Transportador?", function () {
        ExcluirPorCodigo(_contaTransportador, "ContaTransportador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridContaTransportador.CarregarGrid();
                limparCamposContaTransportador();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposContaTransportador();
}

//*******MÉTODOS*******


function buscarContaTransportador() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContaTransportador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContaTransportador = new GridView(_pesquisaContaTransportador.Pesquisar.idGrid, "ContaTransportador/Pesquisa", _pesquisaContaTransportador, menuOpcoes, null);
    _gridContaTransportador.CarregarGrid();
}

function editarContaTransportador(contaTransportadorGrid) {
    limparCamposContaTransportador();
    _contaTransportador.Codigo.val(contaTransportadorGrid.Codigo);
    BuscarPorCodigo(_contaTransportador, "ContaTransportador/BuscarPorCodigo", function (arg) {
        _pesquisaContaTransportador.ExibirFiltros.visibleFade(false);
        _crudContaTransportador.Atualizar.visible(true);
        _crudContaTransportador.Cancelar.visible(true);
        _crudContaTransportador.Excluir.visible(true);
        _crudContaTransportador.Adicionar.visible(false);
    }, null);
}

function limparCamposContaTransportador() {
    _crudContaTransportador.Atualizar.visible(false);
    _crudContaTransportador.Cancelar.visible(false);
    _crudContaTransportador.Excluir.visible(false);
    _crudContaTransportador.Adicionar.visible(true);
    LimparCampos(_contaTransportador);
}