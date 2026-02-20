/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="Pedidos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _clientes;
var _gridClientes;

var Clientes = function () {

    this.Clientes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.Clientes.val.subscribe(function () {
        RenderizarGridCliente();
    });

    this.ImportarClientes = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        cssClass: "btn-default",
        UrlImportacao: "Pedido/ImportarClientes",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacaoClientes",
        FecharModalSeSucesso: true,
        CallbackImportacao: function (arg) {
            if (arg.Data.Retorno != null)
                AdicionarClientesImportados(arg.Data.Retorno);
        }
    });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), idBtnSearch: guid() });
    this.Exportar = PropertyEntity({ type: types.event, eventClick: ExportarClientesClick, text: Localization.Resources.Gerais.Geral.Exportar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadClientes() {
    _clientes = new Clientes();
    KoBindings(_clientes, "knockoutClientes");

    LoadGridClientes();

    if (_CONFIGURACAO_TMS.ExibirAssociacaoClientesNoPedido)
        $("#tabClientes").show();

    // Buscas
    new BuscarClientes(_clientes.Adicionar, AdicionarClienteAGrid, null, null, null, _gridClientes);
}

function AdicionarClienteAGrid(data) {
    // Pega registros
    var dataGrid = _clientes.Clientes.val();

    data.forEach(function (cli) {
        // Objeto cliente
        var cliente = {
            Codigo: cli.Codigo,
            CPF_CNPJ: cli.CPF_CNPJ,
            Nome: cli.Descricao.replace(" (" + cli.CPF_CNPJ + ")", ""),
            Localidade: cli.Localidade
        };


        // Adiciona a lista e atualiza a grid
        dataGrid.push(cliente);
    });

    _clientes.Clientes.val(dataGrid);
}

function RemoverClienteClick(data) {
    var dataGrid = _gridClientes.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _clientes.Clientes.val(dataGrid);
}

function ExportarClientesClick() {
    var cnpjsClientes = _clientes.Clientes.val().map(function (cli) {
        return cli.Codigo;
    });

    executarDownload("Pedido/ExportarPesquisaCliente", { Clientes: JSON.stringify(cnpjsClientes), Grid: "{}" });
}

//*******MÉTODOS*******

function EditarClientes(data) {
    _clientes.Clientes.val(data.ListaClientes);
    RenderizarGridCliente();
}

function LimparCamposClientes() {
    LimparCampos(_clientes);
    _clientes.Clientes.val([]);
    RenderizarGridCliente();
}

function AdicionarClientesImportados(clientesImportados) {
    var clientesGrid = _gridClientes.BuscarRegistros();
    var clientesFiltrado = clientesImportados.filter(function (cli) {
        if (!clientesGrid.some(function (cliCad) { return cliCad.Codigo == cli.Codigo }))
            return cli;
    });
    var clientes = clientesGrid.concat(clientesFiltrado).slice();

    _gridClientes.CarregarGrid(clientes);
    _clientes.Clientes.val(clientes);
}

function LoadGridClientes() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: Localization.Resources.Gerais.Geral.Excluir,
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {

                    RemoverClienteClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF_CNPJ", title: Localization.Resources.Pedidos.Pedido.Cnpj, width: "30%", className: "text-align-center" },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "40%", className: "text-align-left" },
        { data: "Localidade", title: Localization.Resources.Gerais.Geral.Cidade, width: "20%", className: "text-align-left" },
    ];

    // Grid
    _gridClientes = new BasicDataTable(_clientes.Clientes.idGrid, header, menuOpcoes, null, null, 10);
    _gridClientes.CarregarGrid([]);
}

function RenderizarGridCliente() {
    var clientes = _clientes.Clientes.val();

    if (clientes)
        _gridClientes.CarregarGrid(clientes);
}


