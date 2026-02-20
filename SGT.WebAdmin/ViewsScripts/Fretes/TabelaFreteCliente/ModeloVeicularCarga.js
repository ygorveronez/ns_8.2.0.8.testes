/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />

// #region Objetos Globais do Arquivo

var _gridModeloVeicularCarga;

// #endregion Objetos Globais do Arquivo


// #region Funções de Inicialização

function loadGridModeloVeicularCarga() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: null,
    };
    var _editavelCell = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal()
    }
    var _editavelCellString = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.string,
        numberMask: ConfigDecimal({ allowZero: true, precision: 2 })
    }

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "CapacidadeOTM", visible: false },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Fretes.TabelaFreteCliente.ModeloVeicularDeCarga, width: "30%" },
        { data: "PercentualRota", title: Localization.Resources.Fretes.TabelaFreteCliente.PercentualRota, width: "15%", className: "text-align-center", editableCell: _editavelCellString },
        { data: "QuantidadeEntregas", title: Localization.Resources.Fretes.TabelaFreteCliente.QuantidadeEntregas, width: "15%", className: "text-align-center", editableCell: _editavelCellString },
        { data: "DescricaoCapacidadeOTM", title: Localization.Resources.Fretes.TabelaFreteCliente.CapacidadeOTM, width: "15%", className: "text-align-center", editableCell: _editavelCellString }
    ];

    _gridModeloVeicularCarga = new BasicDataTable(_tabelaFreteCliente.AdicionarModeloVeicularCarga.idGrid, header, null, ordenacao, null, linhasPorPaginas, null, null, editarColuna);
    _gridModeloVeicularCarga.CarregarGrid([]);
}

function loadModeloVeicularCarga() {

    loadGridModeloVeicularCarga();
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function limparCamposModeloVeicularCarga() {
    preencherModeloVeicularCarga([]);
}

function preencherModeloVeicularCarga(dadosModeloVeicularCarga) {
    _gridModeloVeicularCarga.CarregarGrid(dadosModeloVeicularCarga);
}

function preencherModeloVeicularCargaSalvar(tabelaFreteCliente) {
    tabelaFreteCliente["ModelosVeicularesCarga"] = obterListaModeloVeicularCargaSalvar();
}

// #endregion Funções Públicas

// #region Funções Privadas

function obterCadastroModeloVeicularCargaSalvar() {
    return {
        Codigo: _cadastroModeloVeicularCarga.Codigo.val(),
        CodigoModeloVeicularCarga: _cadastroModeloVeicularCarga.ModeloVeicularCarga.codEntity(),
        CapacidadeOTM: _cadastroModeloVeicularCarga.CapacidadeOTM.val(),
        ModeloVeicularCarga: _cadastroModeloVeicularCarga.ModeloVeicularCarga.val(),
        PercentualRota: _cadastroModeloVeicularCarga.PercentualRota.val(),
        QuantidadeEntregas: _cadastroModeloVeicularCarga.QuantidadeEntregas.val(),
        DescricaoCapacidadeOTM: EnumSimNao.obterDescricao(_cadastroModeloVeicularCarga.CapacidadeOTM.val())
    };
}

function obterListaModeloVeicularCarga() {
    return _gridModeloVeicularCarga.BuscarRegistros().slice();
}

function obterListaModeloVeicularCargaSalvar() {
    var listaModeloVeicularCarga = obterListaModeloVeicularCarga();
    let listaRetornar = [];

    for (var i = 0; i < listaModeloVeicularCarga.length; i++) {
        let modelo = listaModeloVeicularCarga[i];

        if ((!modelo.DescricaoCapacidadeOTM || (modelo.DescricaoCapacidadeOTM.toUpperCase() != "SIM" && modelo.DescricaoCapacidadeOTM.toUpperCase() != "NAO")) && !modelo.PercentualRota && Number(modelo.QuantidadeEntregas) == 0)
            continue;

        modelo.CapacidadeOTM = modelo.DescricaoCapacidadeOTM.toUpperCase() == "SIM" ? 1 : 0;

        listaRetornar.push(modelo);
    }
    return JSON.stringify(listaRetornar);
}


function CarregarModelosVeicularesGrid(modelosVeiculares) {
    if (_tabelaFreteCliente.Codigo.val() != 0 || modelosVeiculares.length == 0)
        return;

    let listaModelos = new Array();

    modelosVeiculares.forEach(modeloVeicular => {
        let novoModelo = new Object();

        novoModelo.Codigo = 0;
        novoModelo.CodigoModeloVeicularCarga = modeloVeicular.Modelo.Codigo;
        novoModelo.CapacidadeOTM = "";
        novoModelo.ModeloVeicularCarga = modeloVeicular.Modelo.Descricao;
        novoModelo.PercentualRota = "";
        novoModelo.QuantidadeEntregas = ""
        novoModelo.DescricaoCapacidadeOTM = "";
        novoModelo.DT_Enable = true;

        listaModelos.push(novoModelo);
    });

    _gridModeloVeicularCarga.CarregarGrid(listaModelos);
}

// #endregion Funções Privadas
