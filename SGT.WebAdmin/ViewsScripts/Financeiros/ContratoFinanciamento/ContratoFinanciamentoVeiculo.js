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
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="ContratoFinanciamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoVeiculo, _gridVeiculosContratoFinanciamento;

var ContratoFinanciamentoVeiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículo(s)", idBtnSearch: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoVeiculo() {
    _contratoFinanciamentoVeiculo = new ContratoFinanciamentoVeiculo();
    KoBindings(_contratoFinanciamentoVeiculo, "knockoutVeiculoContratoFinanciamento");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirVeiculoClick(_contratoFinanciamentoVeiculo.Veiculo, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "20%" },
        { data: "ModeloVeicularCarga", title: "Modelo de Carga", width: "30%" },
        { data: "TipoVeiculo", title: "Tipo de Veículo", width: "20%" },
        { data: "DescricaoTipo", title: "Proprietário", width: "20%" }
    ];
    _gridVeiculosContratoFinanciamento = new BasicDataTable(_contratoFinanciamentoVeiculo.Grid.id, header, menuOpcoes);

    new BuscarVeiculos(_contratoFinanciamentoVeiculo.Veiculo, function (r) {
        if (r != null) {
            var bens = _gridVeiculosContratoFinanciamento.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                bens.push({
                    Codigo: r[i].Codigo,
                    Placa: r[i].Placa,
                    ModeloVeicularCarga: r[i].ModeloVeicularCarga,
                    TipoVeiculo: r[i].TipoVeiculo,
                    DescricaoTipo: r[i].DescricaoTipo,
                });

            _gridVeiculosContratoFinanciamento.CarregarGrid(bens);
        }
    }, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculosContratoFinanciamento);

    _contratoFinanciamentoVeiculo.Veiculo.basicTable = _gridVeiculosContratoFinanciamento;

    RecarregarGridContratoFinanciamentoVeiculo();
}

//*******MÉTODOS*******

function RecarregarGridContratoFinanciamentoVeiculo() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_contratoFinanciamento.Veiculos.val())) {

        $.each(_contratoFinanciamento.Veiculos.val(), function (i, veiculo) {
            var veiculoGrid = new Object();

            veiculoGrid.Codigo = veiculo.VEICULO.Codigo;
            veiculoGrid.Placa = veiculo.VEICULO.Placa;
            veiculoGrid.ModeloVeicularCarga = veiculo.VEICULO.ModeloVeicularCarga;
            veiculoGrid.TipoVeiculo = veiculo.VEICULO.TipoVeiculo;
            veiculoGrid.DescricaoTipo = veiculo.VEICULO.DescricaoTipo;

            data.push(veiculoGrid);
        });
    }

    _gridVeiculosContratoFinanciamento.CarregarGrid(data);
}

function ExcluirVeiculoClick(knoutVeiculo, data) {
    var veiculoGrid = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculoGrid.length; i++) {
        if (data.Codigo == veiculoGrid[i].Codigo) {
            veiculoGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculoGrid);
}

function preencherListasSelecaoContratoFinanciamentoVeiculo() {
    var veiculos = new Array();

    $.each(_contratoFinanciamentoVeiculo.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculos.push({ VEICULO: veiculo });
    });

    _contratoFinanciamento.Veiculos.val(JSON.stringify(veiculos));
}

function limparCamposContratoFinanciamentoVeiculo() {
    LimparCampos(_contratoFinanciamentoVeiculo);
    RecarregarGridContratoFinanciamentoVeiculo();
}

function obterSomenteVeiculosGrid() {
    var veiculos = new Array();

    $.each(_contratoFinanciamentoVeiculo.Veiculo.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculos.push({ VEICULO: veiculo });
    });

    return JSON.stringify(veiculos);
}