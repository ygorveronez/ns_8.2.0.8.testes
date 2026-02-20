/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="TabelaFrete.js" />

// #region Objetos Globais do Arquivo

var _gridTabelaFreteTracao;
var _tabelaFreteTracao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var TabelaFreteTracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ val: ko.observable(0), required: false, getType: typesKnockout.string });
    this.Tracao = PropertyEntity({ type: types.event, text: "Adicionar Tração", idBtnSearch: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridTabelaFreteTracao() {
    var opcaoExcluir = { descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirTabelaFreteTracaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoExcluir] };

    _gridTabelaFreteTracao = CriarGridVeiculos("tracao", menuOpcoes, "TabelaFrete/ExportarVeiculos");

    new BuscarVeiculos(_tabelaFreteTracao.Tracao, retornoConsultaTracaoTabelaFrete, null, null, null, null, null, null, null, null, null, "0", null, _gridTabelaFreteTracao, null, null, null, null, null, null, null, null, null, obterGridTransportadores());
    _tabelaFreteTracao.Tracao.basicTable = _gridTabelaFreteTracao;

    _gridTabelaFreteTracao.CarregarGrid([]);
}

function loadTabelaFreteTracao() {
    _tabelaFreteTracao = new TabelaFreteTracao();
    KoBindings(_tabelaFreteTracao, "knockoutTabelaFreteTracao");

    loadGridTabelaFreteTracao();
    RecarregarGridTracao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirTabelaFreteTracaoClick(registroSelecionado) {
    var tracoes = _gridTabelaFreteTracao.BuscarRegistros().slice();

    for (var i = 0; i < tracoes.length; i++) {
        if (registroSelecionado.Codigo == tracoes[i].Codigo) {
            tracoes.splice(i, 1);
            break;
        }
    }

    _gridTabelaFreteTracao.CarregarGrid(tracoes);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposTabelaFreteTracao() {
    _gridTabelaFreteTracao.CarregarGrid([]);
}

function preencherTabelaFreteTracao(tracoes) {
    _gridTabelaFreteTracao.CarregarGrid(tracoes);
}

function preencherTabelaFreteTracaoSalvar() {
    var tracoes = _gridTabelaFreteTracao.BuscarRegistros().slice();
    var veiculosSalvar = [];

    for (var i = 0; i < tracoes.length; i++) {
        var tracao = tracoes[i];

        veiculosSalvar.push({
            Codigo: tracao.Codigo,
            CodigoVeiculo: tracao.CodigoVeiculo
        });
    }

    return veiculosSalvar;
}

function retornoConsultaTracaoTabelaFrete(registrosSeleconados) {
    var tracoes = _gridTabelaFreteTracao.BuscarRegistros().slice();

    for (var i = 0; i < registrosSeleconados.length; i++) {
        var tracaoSelecionado = registrosSeleconados[i];

        tracoes.push({
            Codigo: guid(),
            CodigoVeiculo: tracaoSelecionado.Codigo,
            Placa: tracaoSelecionado.Placa,
            ModeloVeicularCarga: tracaoSelecionado.ModeloVeicularCarga,
            CapacidadeKG: tracaoSelecionado.CapacidadeKG,
            CapacidadeM3: tracaoSelecionado.CapacidadeM3,
            Empresa: tracaoSelecionado.Empresa
        });
    }

    _gridTabelaFreteTracao.CarregarGrid(tracoes);
}

function RecarregarGridTracao() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Tracoes.val())) {

        $.each(_tabelaFrete.Tracoes.val(), function (i, tracao) {
            let tracaoGrid = new Object();

            tracaoGrid.Codigo = tracao.Codigo;
            tracaoGrid.CodigoVeiculo = tracao.CodigoVeiculo;
            tracaoGrid.Placa = tracao.Placa;
            tracaoGrid.ModeloVeicularCarga = tracao.ModeloVeicularCarga;
            tracaoGrid.CapacidadeKG = tracao.CapacidadeKG;
            tracaoGrid.CapacidadeM3 = tracao.CapacidadeM3;
            tracaoGrid.Empresa = tracao.Empresa;

            data.push(tracaoGrid);
        });
    }

    _gridTabelaFreteTracao.CarregarGrid(data);
}

// #endregion Funções Públicas