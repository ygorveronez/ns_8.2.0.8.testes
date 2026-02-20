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
/// <reference path="../../Consultas/MarcaVeiculo.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMarcaVeiculo;
var _marcaVeiculo;

var MarcaVeiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.MarcaVeiculo = PropertyEntity({ type: types.event, text: "Adicionar Marca de Veículo", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadMarcaVeiculo() {
    _marcaVeiculo = new MarcaVeiculo();
    KoBindings(_marcaVeiculo, "knockoutGrupoServicoMarcaVeiculo");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirMarcaVeiculoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridMarcaVeiculo = new BasicDataTable(_marcaVeiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarMarcasVeiculo(_marcaVeiculo.MarcaVeiculo, null, null, null, null, null, _gridMarcaVeiculo);
    _marcaVeiculo.MarcaVeiculo.basicTable = _gridMarcaVeiculo;

    RecarregarGridMarcaVeiculo();
}

function RecarregarGridMarcaVeiculo() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_grupoServico.MarcasVeiculo.val())) {

        $.each(_grupoServico.MarcasVeiculo.val(), function (i, marcaVeiculo) {
            var marcaVeiculoGrid = new Object();

            marcaVeiculoGrid.Codigo = marcaVeiculo.Codigo;
            marcaVeiculoGrid.Descricao = marcaVeiculo.Descricao;

            data.push(marcaVeiculoGrid);
        });
    }

    _gridMarcaVeiculo.CarregarGrid(data);
}

function ExcluirMarcaVeiculoClick(data) {
    var marcaVeiculoGrid = _marcaVeiculo.MarcaVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < marcaVeiculoGrid.length; i++) {
        if (data.Codigo == marcaVeiculoGrid[i].Codigo) {
            marcaVeiculoGrid.splice(i, 1);
            break;
        }
    }

    _marcaVeiculo.MarcaVeiculo.basicTable.CarregarGrid(marcaVeiculoGrid);
}

function LimparCamposMarcaVeiculo() {
    LimparCampos(_marcaVeiculo);
    _marcaVeiculo.MarcaVeiculo.basicTable.CarregarGrid(new Array());
}