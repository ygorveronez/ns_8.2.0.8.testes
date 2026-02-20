/// <reference path="GrupoBonificacao.js" />
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
    this.Grid = PropertyEntity({ type: types.local });
    this.Veiculos = PropertyEntity({ type: types.event, text: "Adicionar Veiculos", idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridVeiculo() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) { excluirVeiculoClick(_veiculo.Veiculos, data) } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: "Placa", width: "12%" },
        { data: "NumeroFrota", title: "Nº Frota", width: "12%" },
        { data: "Empresa", title: "Transportador", width: "66%" }
    ];

    _gridVeiculo = new BasicDataTable(_veiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}

function LoadVeiculo() {
    _veiculo = new Veiculo();
    KoBindings(_veiculo, "knockoutVeiculos");

    loadGridVeiculo();

    new BuscarVeiculos(_veiculo.Veiculos, function (retorno) {
        if (retorno != null) {
            for (var i = 0; i < retorno.length; i++)
                _veiculos.push({ Codigo: retorno[i].Codigo, Placa: retorno[i].Placa, NumeroFrota: retorno[i].NumeroFrota, Empresa: retorno[i].Empresa });

            _gridVeiculo.CarregarGrid(_veiculos);
        }
    }, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculo);

    _veiculo.Veiculos.basicTable = _gridVeiculo;

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

function preencherGrupoBonificacaoVeiculo(dadosVeiculo) {
    PreencherObjetoKnout(_veiculo, { Data: dadosVeiculo });
}

function preencherCentroCarregamentoVeiculoSalvar(centroCarregamento) {
}

function obterVeiculos() {
    return JSON.stringify(_veiculo.Veiculos.basicTable.BuscarRegistros());
}

function limparCamposCentroCarregamentoVeiculo() {
}

function recarregarGridVeiculo() {
    var veiculos = _grupoBonificacao.Veiculos.val();

    _gridVeiculo.CarregarGrid(veiculos);
    _veiculos = new Array();

    for (var i = 0; i < veiculos.length; i++) {
        _veiculos.push({ Codigo: veiculos[i].Codigo, Placa: veiculos[i].Placa, NumeroFrota: veiculos[i].NumeroFrota, Empresa: veiculos[i].Empresa });
    }
}