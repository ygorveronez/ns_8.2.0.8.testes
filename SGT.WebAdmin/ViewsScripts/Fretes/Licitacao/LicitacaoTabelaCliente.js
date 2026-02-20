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
/// <reference path="Licitacao.js" />
/// <reference path="../../Consultas/TabelaFreteCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLicitacaoTabelaFreteCliente;
var _licitacaoTabelaFreteCliente;

var LicitacaoTabelaFreteCliente = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TabelaFreteCliente = PropertyEntity({ type: types.event, text: "Adicionar Tabela Frete Cliente", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadLicitacaoTabelaFreteCliente() {
    _licitacaoTabelaFreteCliente = new LicitacaoTabelaFreteCliente();
    KoBindings(_licitacaoTabelaFreteCliente, "knockoutLicitacaoTabelaFreteCliente");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirLicitacaoTabelaFreteClienteClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Origem", title: "Origem", width: "40%" },
        { data: "Destino", title: "Destino", width: "40%" }
    ];

    _gridLicitacaoTabelaFreteCliente = new BasicDataTable(_licitacaoTabelaFreteCliente.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTabelasDeFreteCliente(_licitacaoTabelaFreteCliente.TabelaFreteCliente, null, _licitacao.TabelaFrete, _gridLicitacaoTabelaFreteCliente);
    _licitacaoTabelaFreteCliente.TabelaFreteCliente.basicTable = _gridLicitacaoTabelaFreteCliente;

    RecarregarGridLicitacaoTabelaFreteCliente();
}

function RecarregarGridLicitacaoTabelaFreteCliente() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_licitacao.TabelasFreteCliente.val())) {

        $.each(_licitacao.TabelasFreteCliente.val(), function (i, licitacaoTabelaFreteCliente) {
            var licitacaoTabelaFreteClienteGrid = new Object();

            licitacaoTabelaFreteClienteGrid.Codigo = licitacaoTabelaFreteCliente.Codigo;
            licitacaoTabelaFreteClienteGrid.Origem = licitacaoTabelaFreteCliente.Origem;
            licitacaoTabelaFreteClienteGrid.Destino = licitacaoTabelaFreteCliente.Destino;

            data.push(licitacaoTabelaFreteClienteGrid);
        });
    }

    _gridLicitacaoTabelaFreteCliente.CarregarGrid(data);
}

function ExcluirLicitacaoTabelaFreteClienteClick(data) {
    var licitacaoTabelaFreteClienteGrid = _licitacaoTabelaFreteCliente.TabelaFreteCliente.basicTable.BuscarRegistros();

    for (var i = 0; i < licitacaoTabelaFreteClienteGrid.length; i++) {
        if (data.Codigo == licitacaoTabelaFreteClienteGrid[i].Codigo) {
            licitacaoTabelaFreteClienteGrid.splice(i, 1);
            break;
        }
    }

    _licitacaoTabelaFreteCliente.TabelaFreteCliente.basicTable.CarregarGrid(licitacaoTabelaFreteClienteGrid);
}

function LimparCamposLicitacaoTabelaFreteCliente() {
    LimparCampos(_licitacaoTabelaFreteCliente);
    _licitacaoTabelaFreteCliente.TabelaFreteCliente.basicTable.CarregarGrid(new Array());
}