/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="GerarEscala.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVeiculo;
var _veiculoEscalaVeiculo;

/*
 * Declaração das Classes
 */

var VeiculoEscalaVeiculo = function () {
    this.EmEdicao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });

    this.ListaVeiculo = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridVeiculo() {
    var linhasPorPaginas = 12;
    var ordenacao = { column: 7, dir: orderDir.asc };
    var ordenacaoFixa = {
        "pre": [ 1, orderDir.desc ]
    };

    var header = [
        { data: "DT_RowId", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "CodigoMotorista", visible: false },
        { data: "CodigoVeiculo", visible: false },
        { data: "Motorista", visible: false },
        { data: "Veiculo", title: "Veículo", width: "22%", className: "text-align-center" },
        { data: "ModeloVeicularCarga", title: "Modelo", width: "28%" },
        { data: "Quantidade", title: "Capacidade", width: "22%", className: "text-align-right" },
        { data: "Empresa", title: "Empresa", width: "28%" }
    ];

    _gridVeiculo = new BasicDataTable(_veiculoEscalaVeiculo.ListaVeiculo.idGrid, header, null, ordenacao, null, linhasPorPaginas, undefined, undefined, undefined, undefined, true, ordenacaoFixa);
    _gridVeiculo.CarregarGrid([]);
}

function loadVeiculoEscalaVeiculo() {
    _veiculoEscalaVeiculo = new VeiculoEscalaVeiculo();
    KoBindings(_veiculoEscalaVeiculo, "knockoutVeiculo");

    loadGridVeiculo();
}

/*
 * Declaração das Funções Públicas
 */

function adicionarEscalaVeiculoEscalado(veiculoEscalado) {
    var veiculo = obterVeiculoPorCodigo(veiculoEscalado.CodigoVeiculo);

    if (!veiculo)
        return;

    veiculo.CodigosEscalaVeiculoEscalado.push(veiculoEscalado.Codigo);
    veiculo.DT_RowColor = "#ffcc99";

    recarregarGridVeiculo();
}

function excluirEscalaVeiculoEscalado(veiculoEscalado) {
    var veiculo = obterVeiculoPorCodigo(veiculoEscalado.CodigoVeiculo);

    if (!veiculo)
        return;

    for (var i = 0; i < veiculo.CodigosEscalaVeiculoEscalado.length; i++)
        veiculo.CodigosEscalaVeiculoEscalado.splice(i, 1)

    veiculo.DT_RowColor = (veiculo.CodigosEscalaVeiculoEscalado.length > 0) ? "#ffcc99" : "#ffffff";

    recarregarGridVeiculo();
}

function limparCamposVeiculoEscalaVeiculo() {
    _veiculoEscalaVeiculo.EmEdicao.val(false);

    preencherListaVeiculo([]);
}

function obterVeiculoPorCodigo(codigo) {
    var listaVeiculo = obterListaVeiculo();

    for (var i = 0; i < listaVeiculo.length; i++) {
        var veiculo = listaVeiculo[i];

        if (veiculo.CodigoVeiculo == codigo)
            return veiculo;
    }

    return undefined;
}

function preencherVeiculoEscalaVeiculo(listaVeiculo) {
    _veiculoEscalaVeiculo.EmEdicao.val(isPermitirEditarVeiculoEscala());

    if (!_veiculoEscalaVeiculo.EmEdicao.val())
        return;

    if (listaVeiculo) {
        preencherListaVeiculo(listaVeiculo);
        return;
    }

    executarReST("GerarEscala/BuscarVeiculosEmEscala", { Codigo: _gerarEscala.Codigo.val() }, function (retorno) {
        if (retorno.Success)
            preencherListaVeiculo(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções Privadas
 */

function preencherListaVeiculo(listaVeiculo) {
    _veiculoEscalaVeiculo.ListaVeiculo.val(listaVeiculo);

    recarregarGridVeiculo();
}

function obterListaVeiculo() {
    return _veiculoEscalaVeiculo.ListaVeiculo.val().slice();
}

function recarregarGridVeiculo() {
    var listaVeiculo = obterListaVeiculo();

    _gridVeiculo.CarregarGrid(listaVeiculo);
}
