/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="TabelaFrete.js" />

// #region Objetos Globais do Arquivo

var _gridTabelaFreteReboque;
var _tabelaFreteReboque;

// #endregion Objetos Globais do Arquivo

// #region Classes

var TabelaFreteReboque = function () {
    this.Reboque = PropertyEntity({ type: types.event, text: "Adicionar Reboque", idBtnSearch: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridTabelaFreteReboque() {
    var opcaoExcluir = { descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirTabelaFreteReboqueClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoExcluir] };

    _gridTabelaFreteReboque = CriarGridVeiculos("reboque", menuOpcoes, "TabelaFrete/ExportarVeiculos");

    new BuscarVeiculos(_tabelaFreteReboque.Reboque, retornoConsultaReboqueTabelaFrete, null, null, null, null, null, null, null, null, null, "1", null, _gridTabelaFreteReboque, null, null, null, null, null, null, null, null, null, obterGridTransportadores());
    _tabelaFreteReboque.Reboque.basicTable = _gridFronteira;

    _gridTabelaFreteReboque.CarregarGrid([]);
}

function loadTabelaFreteReboque() {
    _tabelaFreteReboque = new TabelaFreteReboque();
    KoBindings(_tabelaFreteReboque, "knockoutTabelaFreteReboque");

    loadGridTabelaFreteReboque();
}

function CriarGridVeiculos(tipoVeiculo, menuOpcoes, url) {
    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Placa", title: Localization.Resources.Fretes.TabelaFrete.Placa, width: "30%" },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Fretes.TabelaFrete.Modelo, width: "30%" },
        { data: "CapacidadeKG", title: Localization.Resources.Fretes.TabelaFrete.CapacidadeKG, width: "15%", className: "text-align-right" },
        { data: "CapacidadeM3", title: Localization.Resources.Fretes.TabelaFrete.CapacidadeM3, width: "15%", className: "text-align-right" },
        { data: "Empresa", title: "Transportador", width: "15%", className: "text-align-right" }
    ];

    var configExportacao = {
        url: url,
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return { Codigo: _tabelaFrete.Codigo.val(), Tipo: tipoVeiculo == "tracao" ? "0" : "1" };
        }
    }

    return new BasicDataTable(`grid-tabela-frete-${tipoVeiculo}`, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, null, null, null, null, configExportacao);
}

// #endregion Funções de Inicialização
// #region Funções Associadas a Eventos

function excluirTabelaFreteReboqueClick(registroSelecionado) {
    var reboques = _gridTabelaFreteReboque.BuscarRegistros().slice();

    for (var i = 0; i < reboques.length; i++) {
        if (registroSelecionado.Codigo == reboques[i].Codigo) {
            reboques.splice(i, 1);
            break;
        }
    }

    _gridTabelaFreteReboque.CarregarGrid(reboques);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposTabelaFreteReboque() {
    _gridTabelaFreteReboque.CarregarGrid([]);
}

function preencherTabelaFreteReboque(reboques) {
    _gridTabelaFreteReboque.CarregarGrid(reboques);
}


function preencherTabelaFreteReboqueSalvar() {
    var reboques = _gridTabelaFreteReboque.BuscarRegistros().slice();
    var veiculosSalvar = [];

    for (var i = 0; i < reboques.length; i++) {
        var reboque = reboques[i];

        veiculosSalvar.push({
            Codigo: reboque.Codigo,
            CodigoVeiculo: reboque.CodigoVeiculo
        });
    }

    return veiculosSalvar;
}

function retornoConsultaReboqueTabelaFrete(registrosSeleconados) {
    var reboques = _gridTabelaFreteReboque.BuscarRegistros().slice();

    for (var i = 0; i < registrosSeleconados.length; i++) {
        var reboqueSelecionado = registrosSeleconados[i];

        reboques.push({
            Codigo: guid(),
            CodigoVeiculo: reboqueSelecionado.Codigo,
            Placa: reboqueSelecionado.Placa,
            ModeloVeicularCarga: reboqueSelecionado.ModeloVeicularCarga,
            CapacidadeKG: reboqueSelecionado.CapacidadeKG,
            CapacidadeM3: reboqueSelecionado.CapacidadeM3,
            Empresa: reboqueSelecionado.Empresa
        });
    }

    _gridTabelaFreteReboque.CarregarGrid(reboques);
}

function RecarregarGridReboque() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Reboques.val())) {

        $.each(_tabelaFrete.Reboques.val(), function (i, tracao) {
            let tracaoGrid = new Object();

            tracaoGrid.Codigo = tracao.Codigo;
            tracaoGrid.Placa = tracao.Placa;
            tracaoGrid.CodigoVeiculo = tracao.CodigoVeiculo;
            tracaoGrid.ModeloVeicularCarga = tracao.ModeloVeicularCarga;
            tracaoGrid.CapacidadeKG = tracao.CapacidadeKG;
            tracaoGrid.CapacidadeM3 = tracao.CapacidadeM3;
            tracaoGrid.Empresa = tracao.Empresa;

            data.push(tracaoGrid);
        });
    }

    _gridTabelaFreteReboque.CarregarGrid(data);
}

// #endregion Funções Públicas