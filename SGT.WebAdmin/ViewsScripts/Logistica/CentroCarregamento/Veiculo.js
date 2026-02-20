/// <reference path="CentroCarregamento.js" />
/// <reference path="..\..\Consultas\Veiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVeiculo;
var _veiculo;
var _veiculos = new Array();

/*
 * Declaração das Classes
 */

var Veiculo = function () {
    this.LimiteCargasPorVeiculoPorDia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.LimiteCargasPorVeiculoPorDia.getFieldDescription() });
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculo = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarVeiculos, idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridVeiculo() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) { excluirVeiculoClick(_veiculo.Veiculo, data) } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [ opcaoExcluir ] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: Localization.Resources.Logistica.CentroCarregamento.Placa, width: "80%" }
    ];

    _gridVeiculo = new BasicDataTable(_veiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function LoadVeiculo() {
    _veiculo = new Veiculo();
    KoBindings(_veiculo, "knockoutVeiculo");

    loadGridVeiculo();

    new BuscarVeiculos(_veiculo.Veiculo, function (retorno) {
        if (retorno != null) {
            for (var i = 0; i < retorno.length; i++)
                _veiculos.push({ Codigo: retorno[i].Codigo, Placa: retorno[i].Placa });

            _gridVeiculo.CarregarGrid(_veiculos);
        }
    }, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculo);

    _veiculo.Veiculo.basicTable = _gridVeiculo;

    recarregarGridVeiculo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirVeiculoClick(knoutVeiculo, data) {
    var veiculos = knoutVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < veiculos.length; i++) {
        if (data.Codigo == veiculos[i].Codigo) {
            veiculos.splice(i, 1);
            break;
        }
    }

    knoutVeiculo.basicTable.CarregarGrid(veiculos);
}

/*
 * Declaração das Funções
 */

function LimparCamposVeiculo() {
    LimparCampos(_veiculo);
    _gridVeiculo.CarregarGrid(new Array());
    _veiculos = new Array();
}

function preencherCentroCarregamentoVeiculo(dadosVeiculo) {
    PreencherObjetoKnout(_veiculo, { Data: dadosVeiculo });
}

function preencherCentroCarregamentoVeiculoSalvar(centroCarregamento) {
    centroCarregamento["LimiteCargasPorVeiculoPorDia"] = _veiculo.LimiteCargasPorVeiculoPorDia.val();
}

function obterVeiculos() {
    return JSON.stringify(_veiculo.Veiculo.basicTable.BuscarRegistros());
}

function limparCamposCentroCarregamentoVeiculo() {
    LimparCampo(_veiculo.LimiteCargasPorVeiculoPorDia);
}

function recarregarGridVeiculo() {
    var veiculos = _centroCarregamento.Veiculos.val();

    _gridVeiculo.CarregarGrid(veiculos);
    _veiculos = new Array();

    for (var i = 0; i < veiculos.length; i++) {
        _veiculos.push({ Codigo: veiculos[i].Codigo, Placa: veiculos[i].Placa });
    }
}