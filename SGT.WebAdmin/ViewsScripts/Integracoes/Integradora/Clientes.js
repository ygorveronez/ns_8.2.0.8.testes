//*******MAPEAMENTO KNOUCKOUT*******

var _gridClienteIntegradora;
var _clienteIntegradora;

var ClienteIntegradora = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Cliente = PropertyEntity({ type: types.event, text: Localization.Resources.Integracoes.Integradora.AdicionarCliente, idBtnSearch: guid(), issue: 0 });
};


//*******EVENTOS*******

function LoadClienteIntegradora() {
    _clienteIntegradora = new ClienteIntegradora();
    KoBindings(_clienteIntegradora, "knockoutClientes");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirClienteIntegradoraClick(_clienteIntegradora.Cliente, data);  
            }
        }]
    };

    var header = [
        { data: "CPF_CNPJ", visible: false },
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Endereco", visible: false },
        { data: "Numero", visible: false },
        { data: "CEP", visible: false },
        { data: "CodigoIBGE", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "50%" },
        { data: "Localidade", title: Localization.Resources.Gerais.Geral.Localidade, width: "30%" }
    ];

    _gridClienteIntegradora = new BasicDataTable(_clienteIntegradora.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_clienteIntegradora.Cliente, null, null, null, null, _gridClienteIntegradora);

    _clienteIntegradora.Cliente.basicTable = _gridClienteIntegradora;

    RecarregarGridClienteIntegradora();
}

function RecarregarGridClienteIntegradora() {
    _gridClienteIntegradora.CarregarGrid(_integradora.Clientes.val());
}

function ExcluirClienteIntegradoraClick(knoutCliente, data) {

    var clientesGrid = knoutCliente.basicTable.BuscarRegistros();

    for (var i = 0; i < clientesGrid.length; i++) {
        if (data.Codigo == clientesGrid[i].Codigo) {
            clientesGrid.splice(i, 1);
            break;
        }
    }

    knoutCliente.basicTable.CarregarGrid(clientesGrid);
}

function LimparCamposClienteIntegradora() {
    LimparCampos(_clienteIntegradora);
    _gridClienteIntegradora.CarregarGrid(new Array());
}