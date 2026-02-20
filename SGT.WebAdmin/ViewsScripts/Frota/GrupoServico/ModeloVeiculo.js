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
/// <reference path="../../Consultas/ModeloVeiculo.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloVeiculo;
var _modeloVeiculo;

var ModeloVeiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ModeloVeiculo = PropertyEntity({ type: types.event, text: "Adicionar Modelo de Veículo", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloVeiculo() {
    _modeloVeiculo = new ModeloVeiculo();
    KoBindings(_modeloVeiculo, "knockoutGrupoServicoModeloVeiculo");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirModeloVeiculoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridModeloVeiculo = new BasicDataTable(_modeloVeiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeiculo(_modeloVeiculo.ModeloVeiculo, null, null, null, null,_gridModeloVeiculo);
    _modeloVeiculo.ModeloVeiculo.basicTable = _gridModeloVeiculo;

    RecarregarGridModeloVeiculo();
}

function RecarregarGridModeloVeiculo() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_grupoServico.ModelosVeiculo.val())) {

        $.each(_grupoServico.ModelosVeiculo.val(), function (i, modeloVeiculo) {
            var modeloVeiculoGrid = new Object();

            modeloVeiculoGrid.Codigo = modeloVeiculo.Codigo;
            modeloVeiculoGrid.Descricao = modeloVeiculo.Descricao;

            data.push(modeloVeiculoGrid);
        });
    }

    _gridModeloVeiculo.CarregarGrid(data);
}

function ExcluirModeloVeiculoClick(data) {
    var modeloVeiculoGrid = _modeloVeiculo.ModeloVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < modeloVeiculoGrid.length; i++) {
        if (data.Codigo == modeloVeiculoGrid[i].Codigo) {
            modeloVeiculoGrid.splice(i, 1);
            break;
        }
    }

    _modeloVeiculo.ModeloVeiculo.basicTable.CarregarGrid(modeloVeiculoGrid);
}

function LimparCamposModeloVeiculo() {
    LimparCampos(_modeloVeiculo);
    _modeloVeiculo.ModeloVeiculo.basicTable.CarregarGrid(new Array());
}