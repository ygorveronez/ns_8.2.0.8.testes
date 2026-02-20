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
/// <reference path="GrupoPessoas.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />



var _gridClientes;
var _cliente;
var _gridPessoas;
var _clienteGrupo;
//*******MAPEAMENTO KNOUCKOUT*******

var ClienteGrupo = function () {
    this.Clientes = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarPessoasForaRaizCNPJ, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.ClientesComRaiz = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarPessoasRaizCNPJ, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ConsultarClientes = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarPessoasViaRaizCNPJ, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: this.Clientes.idGrid, visible: false, enable: ko.observable(true) });
}

var ClienteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.CNPJ = PropertyEntity({ type: types.map, val: "" });
    this.Localidade = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******

function loadCliente() {

    _clienteGrupo = new ClienteGrupo();
    KoBindings(_clienteGrupo, "knockoutPessoasGrupo");

    var editar = {
        descricao: Localization.Resources.Pessoas.GrupoPessoas.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            excluirClienteClick(_clienteGrupo.Clientes, data)
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [{ data: "Codigo", visible: false },
    { data: "CPF_CNPJ", title: "CNPJ / CPF", width: "10%", className: "text-align-left" },
    { data: "Nome", title: Localization.Resources.Pessoas.GrupoPessoas.RazaoSocial, width: "40%", className: "text-align-left" },
    { data: "Localidade", title: Localization.Resources.Pessoas.GrupoPessoas.Localidade, width: "25%", className: "text-align-left" }
    ];
    _gridClientes = new BasicDataTable(_clienteGrupo.Clientes.idGrid, header, menuOpcoes);
    _clienteGrupo.Clientes.basicTable = _gridClientes;
    _clienteGrupo.ClientesComRaiz.basicTable = _gridClientes;
    _clienteGrupo.ConsultarClientes.basicTable = _gridClientes;

    new BuscarClientes(_clienteGrupo.Clientes, null, null, null, null, _gridClientes);
    new BuscarClientes(_clienteGrupo.ClientesComRaiz, null, null, null, null, _gridClientes, _grupoPessoas.RaizCNPJ, _grupoPessoas.Codigo);
    new BuscarClientes(_clienteGrupo.ConsultarClientes, null, null, null, null, _gridClientes);

    recarregarGridClientes();
}

function excluirClienteClick(knoutClientes, data) {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Confirmacao, Localization.Resources.Pessoas.GrupoPessoas.RealmenteDesejaExcluirPessoaX.format(data.Nome), function () {
        var clienteGrid = knoutClientes.basicTable.BuscarRegistros();

        for (var i = 0; i < clienteGrid.length; i++) {
            if (data.Codigo == clienteGrid[i].Codigo) {
                clienteGrid.splice(i, 1);
                break;
            }
        }
        knoutClientes.basicTable.CarregarGrid(clienteGrid);
    });

}

//*******MÉTODOS*******

function recarregarGridClientes() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_grupoPessoas.Clientes.val())) {
        $.each(_grupoPessoas.Clientes.val(), function (i, cliente) {
            var obj = new Object();
            obj.Codigo = cliente.Codigo;
            obj.CPF_CNPJ = cliente.CPF_CNPJ;
            obj.Nome = cliente.Nome;
            obj.Localidade = cliente.Localidade;
            data.push(obj);
        });      
    }
    _gridClientes.CarregarGrid(data);
}


function limparCamposClientes() {
    LimparCampos(_clienteGrupo);
}
