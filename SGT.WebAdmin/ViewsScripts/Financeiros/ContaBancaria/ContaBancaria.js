
//*******MAPEAMENTO KNOUCKOUT*******

var _gridContaBancaria;
var _contaBancaria;
var _pesquisaContaBancaria;


var PesquisaContaBancaria = function () {
    this.ClientePortadorConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador Da Conta", idBtnSearch: guid(), required: false });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco", issue: 49, idBtnSearch: guid(), required: false });
    this.Agencia = PropertyEntity({ text: ko.observable("Agência"), required: false, visible: ko.observable(true), maxlength: 10 });
    this.NumeroConta = PropertyEntity({ text: ko.observable("Número da Conta"), required: false, visible: ko.observable(true), maxlength: 12 });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Todos), options: EnumTipoConta.obterOpcoesPesquisa(), def: EnumTipoConta.Todos, text: "Tipo de conta", required: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContaBancaria.CarregarGrid();
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

var ContaBancaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ClientePortadorConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Portador Da Conta", idBtnSearch: guid(), required: true });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Banco", issue: 49, idBtnSearch: guid(), required: true });
    this.Agencia = PropertyEntity({ text: ko.observable("*Agência"), required: true, visible: ko.observable(true), maxlength: 10 });
    this.Digito = PropertyEntity({ text: ko.observable("*Dígito"), required: true, visible: ko.observable(true), maxlength: 1 });
    this.NumeroConta = PropertyEntity({ text: ko.observable("*Número da Conta"), required: true, visible: ko.observable(true), maxlength: 12 });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: "Tipo de conta", required: false });
    this.TipoChavePix = PropertyEntity({ val: ko.observable(""), options: EnumTipoChavePix.obterOpcoes(), def: "", text: "Tipo Chave Pix", required: false });
    this.ChavePix = PropertyEntity({ text: ko.observable("Chave Pix"), required: false, visible: ko.observable(true), maxlength: 36 });
    this.CodigoIntegracaoDadosBancarios = PropertyEntity({ text: ko.observable("Código Integração"), required: false, visible: ko.observable(true), maxlength: 200 });


    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });

}

//*******EVENTOS*******

function loadContaBancaria() {
    _pesquisaContaBancaria = new PesquisaContaBancaria();
    KoBindings(_pesquisaContaBancaria, "knockoutPesquisaContaBancaria", false, _pesquisaContaBancaria.Pesquisar.id);

    _contaBancaria = new ContaBancaria();
    KoBindings(_contaBancaria, "knockoutCadastroContaBancaria");

    HeaderAuditoria("Conta Bancária", _contaBancaria);

    buscarContaBancaria();

    new BuscarBanco(_pesquisaContaBancaria.Banco);
    new BuscarClientes(_pesquisaContaBancaria.ClientePortadorConta);

    new BuscarBanco(_contaBancaria.Banco);
    new BuscarClientes(_contaBancaria.ClientePortadorConta, retornoClientePortadorConta);
}
function retornoClientePortadorConta(dado) {
    _contaBancaria.ClientePortadorConta.codEntity(dado.Codigo);
    _contaBancaria.ClientePortadorConta.val(dado.Nome + " - " + dado.CPF_CNPJ);
}
function atualizarClick(e, sender) {
    Salvar(e, "ContaBancaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContaBancaria.CarregarGrid();
                limparCamposContaBancaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}
function adicionarClick(e, sender) {
    Salvar(e, "ContaBancaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _contaBancaria.Codigo.val(arg.Data);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContaBancaria.CarregarGrid();
                limparCamposContaBancaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}
function cancelarClick(e) {
    limparCamposContaBancaria();
}
function excluirClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a conta bancária " + _contaBancaria.NumeroConta.val() + "?", function () {
        ExcluirPorCodigo(_contaBancaria, "ContaBancaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridContaBancaria.CarregarGrid();
                    limparCamposContaBancaria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}
//*******MÉTODOS*******


function buscarContaBancaria() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContaBancaria, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContaBancaria = new GridView(_pesquisaContaBancaria.Pesquisar.idGrid, "ContaBancaria/Pesquisa", _pesquisaContaBancaria, menuOpcoes, null);
    _gridContaBancaria.CarregarGrid();
}

function editarContaBancaria(ContaBancariaGrid) {
    limparCamposContaBancaria();
    _contaBancaria.Codigo.val(ContaBancariaGrid.Codigo);
    BuscarPorCodigo(_contaBancaria, "ContaBancaria/BuscarPorCodigo", function (arg) {
        _pesquisaContaBancaria.ExibirFiltros.visibleFade(false);
        _contaBancaria.Atualizar.visible(true);
        _contaBancaria.Cancelar.visible(true);
        _contaBancaria.Adicionar.visible(false);
        _contaBancaria.Excluir.visible(true);


    }, null);
}

function limparCamposContaBancaria() {
    _contaBancaria.Atualizar.visible(false);
    _contaBancaria.Cancelar.visible(true);
    _contaBancaria.Adicionar.visible(true);
    _contaBancaria.Excluir.visible(false);

    LimparCampos(_contaBancaria);
}