/// <reference path="DocumentoEntrada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculo;
var _veiculo;

var Veiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), issue: 0, enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadVeiculo() {

    _veiculo = new Veiculo();
    KoBindings(_veiculo, "divVeiculos");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirVeiculoClick(_veiculo.Veiculo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "12%" },
        { data: "NumeroFrota", title: "Nº Frota", width: "12%" },
        { data: "ModeloVeicularCarga", title: "Modelo Veicular", width: "66%" }
    ];

    _gridVeiculo = new BasicDataTable(_veiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarVeiculos(_veiculo.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculo);

    _veiculo.Veiculo.basicTable = _gridVeiculo;

    RecarregarGridVeiculo();
}

function RecarregarGridVeiculo() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_documentoEntrada.Veiculos.val())) {
        $.each(_documentoEntrada.Veiculos.val(), function (i, veiculo) {
            let veiculoGrid = new Object();

            veiculoGrid.Codigo = veiculo.Codigo;
            veiculoGrid.Placa = veiculo.Placa;
            veiculoGrid.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
            veiculoGrid.NumeroFrota = veiculo.NumeroFrota;

            data.push(veiculoGrid);
        });
    }

    _gridVeiculo.CarregarGrid(data);
}

function ExcluirVeiculoClick(knoutVeiculo, data) {
    if (_documentoEntrada.Situacao.val() !== EnumSituacaoDocumentoEntrada.Aberto) {
        exibirMensagem(tipoMensagem.aviso, "Atenção!", "Não é possível remover o veículo na situação atual do documento de entrada.");
        return;
    }

    var veiculosGrid = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculosGrid.length; i++) {
        if (data.Codigo == veiculosGrid[i].Codigo) {
            veiculosGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculosGrid);
}

function LimparCamposVeiculo() {
    LimparCampos(_veiculo);
    _veiculo.Veiculo.enable(true);
}