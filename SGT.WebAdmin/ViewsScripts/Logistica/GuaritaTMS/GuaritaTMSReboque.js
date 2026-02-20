/// <reference path="GuaritaTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _guaritaTMSReboque, _gridGuaritaTMSReboque;

var GuaritaTMSReboque = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Reboque = PropertyEntity({ type: types.event, text: "Adicionar Reboque(s)", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadGuaritaTMSReboque() {
    _guaritaTMSReboque = new GuaritaTMSReboque();
    KoBindings(_guaritaTMSReboque, "knockoutGuaritaTMSReboque");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirReboqueClick(_guaritaTMSReboque.Reboque, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "20%" },
        { data: "ModeloVeicularCarga", title: "Modelo de Carga", width: "30%" },
        { data: "TipoVeiculo", title: "Tipo de Veículo", width: "20%" },
        { data: "DescricaoTipo", title: "Proprietário", width: "20%" }
    ];
    _gridGuaritaTMSReboque = new BasicDataTable(_guaritaTMSReboque.Grid.id, header, menuOpcoes);

    new BuscarVeiculos(_guaritaTMSReboque.Reboque, function (r) {
        if (r != null) {
            var reboques = _gridGuaritaTMSReboque.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                reboques.push({
                    Codigo: r[i].Codigo,
                    Placa: r[i].Placa,
                    ModeloVeicularCarga: r[i].ModeloVeicularCarga,
                    TipoVeiculo: r[i].TipoVeiculo,
                    DescricaoTipo: r[i].DescricaoTipo,
                });

            _gridGuaritaTMSReboque.CarregarGrid(reboques);
        }
    }, null, null, null, null, null, null, null, null, null, null, null, _gridGuaritaTMSReboque, null, null, null, null, null, "1");

    _guaritaTMSReboque.Reboque.basicTable = _gridGuaritaTMSReboque;

    RecarregarGridGuaritaTMSReboque();
}

//*******MÉTODOS*******


function RecarregarGridGuaritaTMSReboque() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_guaritaTMS.Reboques.val())) {
        $.each(_guaritaTMS.Reboques.val(), function (i, veiculo) {
            var veiculoGrid = new Object();

            veiculoGrid.Codigo = veiculo.VEICULO.Codigo;
            veiculoGrid.Placa = veiculo.VEICULO.Placa;
            veiculoGrid.ModeloVeicularCarga = veiculo.VEICULO.ModeloVeicularCarga;
            veiculoGrid.TipoVeiculo = veiculo.VEICULO.TipoVeiculo;
            veiculoGrid.DescricaoTipo = veiculo.VEICULO.DescricaoTipo;

            data.push(veiculoGrid);
        });
    }

    _gridGuaritaTMSReboque.CarregarGrid(data);
}

function ExcluirReboqueClick(knoutVeiculo, data) {
    var veiculoGrid = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculoGrid.length; i++) {
        if (data.Codigo == veiculoGrid[i].Codigo) {
            veiculoGrid.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculoGrid);
}

function preencherListasSelecaoGuaritaTMSReboque() {
    var veiculos = new Array();

    $.each(_guaritaTMSReboque.Reboque.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculos.push({ VEICULO: veiculo });
    });

    _guaritaTMS.Reboques.val(JSON.stringify(veiculos));
}

function limparCamposGuaritaTMSReboque() {
    LimparCampos(_guaritaTMSReboque);
    RecarregarGridGuaritaTMSReboque();
    SetarEnableCamposKnockout(_guaritaTMSReboque, true);
    _gridGuaritaTMSReboque.HabilitarOpcoes();
}