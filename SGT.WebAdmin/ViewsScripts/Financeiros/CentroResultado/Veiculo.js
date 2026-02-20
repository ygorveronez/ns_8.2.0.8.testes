//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculo, _veiculo;

var Veiculo = function () {
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento do Veículo:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: "Adicionar Veículo", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.SegmentoVeiculo.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            _veiculo.SegmentoVeiculo.codEntity(0);
    });

    this.SegmentoVeiculo.codEntity.subscribe(function (novoValor) {
        if (novoValor > 0)
            _veiculo.Veiculo.visible(false);
        else
            _veiculo.Veiculo.visible(true);
    });
};

//*******EVENTOS*******

function LoadVeiculo() {

    _veiculo = new Veiculo();
    KoBindings(_veiculo, "knockoutVeiculos");

    new BuscarSegmentoVeiculo(_veiculo.SegmentoVeiculo);

    _centroResultado.SegmentoVeiculo = _veiculo.SegmentoVeiculo;

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

    new BuscarVeiculos(_veiculo.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculo, null, function () { ControlarVisibilidadeVeiculo(null); });

    _veiculo.Veiculo.basicTable = _gridVeiculo;

    RecarregarGridVeiculo();
}

function RecarregarGridVeiculo() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_centroResultado.Veiculos.val())) {

        $.each(_centroResultado.Veiculos.val(), function (i, veiculo) {
            var veiculoGrid = new Object();

            veiculoGrid.Codigo = veiculo.Codigo;
            veiculoGrid.Placa = veiculo.Placa;
            veiculoGrid.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
            veiculoGrid.NumeroFrota = veiculo.NumeroFrota;

            data.push(veiculoGrid);
        })
    }
        _gridVeiculo.CarregarGrid(data);

    ControlarVisibilidadeVeiculo(data);
}


function ExcluirVeiculoClick(knoutVeiculo, data) {
    var veiculosGrid = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculosGrid.length; i++) {
        if (data.Codigo == veiculosGrid[i].Codigo) {
            veiculosGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculosGrid);

    ControlarVisibilidadeVeiculo(veiculosGrid);
}

function LimparCamposVeiculo() {
    LimparCampos(_veiculo);
}

function ControlarVisibilidadeVeiculo(veiculosGrid) {

    if (veiculosGrid == null)
        veiculosGrid = _veiculo.Veiculo.basicTable.BuscarRegistros();

    if (veiculosGrid.length > 0)
        _veiculo.SegmentoVeiculo.visible(false);
    else
        _veiculo.SegmentoVeiculo.visible(true);

    if (_veiculo.SegmentoVeiculo.codEntity() > 0)
        _veiculo.Veiculo.visible(false);
    else
        _veiculo.Veiculo.visible(true);
}