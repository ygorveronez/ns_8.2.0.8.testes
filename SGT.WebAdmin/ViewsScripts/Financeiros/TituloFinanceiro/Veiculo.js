/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="TituloFinanceiro.js" />
/// <reference path="CentroResultadoTipoDespesa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculo;
var _veiculo;

var Veiculo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadVeiculo() {

    _veiculo = new Veiculo();
    KoBindings(_veiculo, "tabVeiculos");

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

    new BuscarVeiculos(_veiculo.Veiculo, RetornoVeiculoTitulo, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculo);

    _veiculo.Veiculo.basicTable = _gridVeiculo;

    RecarregarGridVeiculo();
}

function RetornoVeiculoTitulo(data) {
    var dataGrid = _gridVeiculo.BuscarRegistros();

    for (var i = 0; i < data.length; i++) {
        var obj = new Object();
        var dados = data[i];

        obj.Codigo = dados.Codigo;
        obj.Placa = dados.Placa;
        obj.NumeroFrota = dados.NumeroFrota;
        obj.ModeloVeicularCarga = dados.ModeloVeicularCarga;

        if (_CONFIGURACAO_TMS.AtivarControleDespesas && i == 0) {
            _centroResultadoTipoDespesa.CentroResultado.codEntity(dados.CodigoCentroResultado);
            _centroResultadoTipoDespesa.CentroResultado.val(dados.CentroResultado);

            AplicarCentroResultadoTipoDespesaNaLista();
        }

        dataGrid.push(obj);
    }

    _gridVeiculo.CarregarGrid(dataGrid);
}

function RecarregarGridVeiculo() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tituloFinanceiro.Veiculos.val())) {
        $.each(_tituloFinanceiro.Veiculos.val(), function (i, veiculo) {
            var veiculoGrid = new Object();

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
    if (_tituloFinanceiro.StatusTitulo.val() != EnumSituacaoTitulo.EmAberto) {
        exibirMensagem(tipoMensagem.aviso, "Atenção!", "Não é possível remover o veículo na situação atual do título.");
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