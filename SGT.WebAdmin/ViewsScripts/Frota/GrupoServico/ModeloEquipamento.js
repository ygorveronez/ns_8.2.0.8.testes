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
/// <reference path="../../Consultas/ModeloEquipamento.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloEquipamento;
var _modeloEquipamento;

var ModeloEquipamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.ModeloEquipamento = PropertyEntity({ type: types.event, text: "Adicionar Marca de Veículo", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadModeloEquipamento() {
    _modeloEquipamento = new ModeloEquipamento();
    KoBindings(_modeloEquipamento, "knockoutGrupoServicoModeloEquipamento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirModeloEquipamentoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridModeloEquipamento = new BasicDataTable(_modeloEquipamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModeloEquipamentos(_modeloEquipamento.ModeloEquipamento, null, null, null, _gridModeloEquipamento);
    _modeloEquipamento.ModeloEquipamento.basicTable = _gridModeloEquipamento;

    RecarregarGridModeloEquipamento();
}

function RecarregarGridModeloEquipamento() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_grupoServico.ModelosEquipamento.val())) {

        $.each(_grupoServico.ModelosEquipamento.val(), function (i, modeloEquipamento) {
            var modeloEquipamentoGrid = new Object();

            modeloEquipamentoGrid.Codigo = modeloEquipamento.Codigo;
            modeloEquipamentoGrid.Descricao = modeloEquipamento.Descricao;

            data.push(modeloEquipamentoGrid);
        });
    }

    _gridModeloEquipamento.CarregarGrid(data);
}

function ExcluirModeloEquipamentoClick(data) {
    var modeloEquipamentoGrid = _modeloEquipamento.ModeloEquipamento.basicTable.BuscarRegistros();

    for (var i = 0; i < modeloEquipamentoGrid.length; i++) {
        if (data.Codigo == modeloEquipamentoGrid[i].Codigo) {
            modeloEquipamentoGrid.splice(i, 1);
            break;
        }
    }

    _modeloEquipamento.ModeloEquipamento.basicTable.CarregarGrid(modeloEquipamentoGrid);
}

function LimparCamposModeloEquipamento() {
    LimparCampos(_modeloEquipamento);
    _modeloEquipamento.ModeloEquipamento.basicTable.CarregarGrid(new Array());
}