/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/TipoRetornoCarga.js" />
/// <reference path="AreaVeiculo.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/

var _areaVeiculoTipoRetornoCarga;
var _gridAreaVeiculoTipoRetornoCarga;

/*
* Declaração das Classes
*/

var AreaVeiculoTipoRetornoCarga = function () {
    this.ListaTipoRetornoCarga = PropertyEntity({ type: types.map, required: false, text: "Adicionar Tipo", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAreaVeiculoTipoRetornoCarga() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTipoRetornoCarga, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridAreaVeiculoTipoRetornoCarga = new BasicDataTable(_areaVeiculoTipoRetornoCarga.ListaTipoRetornoCarga.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarTipoRetornoCarga(_areaVeiculoTipoRetornoCarga.ListaTipoRetornoCarga, null, _gridAreaVeiculoTipoRetornoCarga);
    _areaVeiculoTipoRetornoCarga.ListaTipoRetornoCarga.basicTable = _gridAreaVeiculoTipoRetornoCarga;

    _gridAreaVeiculoTipoRetornoCarga.CarregarGrid([]);
}

function loadAreaVeiculoTipoRetornoCarga() {
    _areaVeiculoTipoRetornoCarga = new AreaVeiculoTipoRetornoCarga();
    KoBindings(_areaVeiculoTipoRetornoCarga, "knockoutAreaVeiculoTipoRetornoCarga");

    loadGridAreaVeiculoTipoRetornoCarga();
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposAreaVeiculoTipoRetornoCarga() {
    _gridAreaVeiculoTipoRetornoCarga.CarregarGrid([]);
}

function obterAreaVeiculoTipoRetornoCargaSalvar() {
    var listaTipoRetornoCarga = obterListaTipoRetornoCarga();
    var listaTipoRetornoCargaRetornar = new Array();

    listaTipoRetornoCarga.forEach(function (tipoRetornoCarga) {
        listaTipoRetornoCargaRetornar.push({
            Codigo: tipoRetornoCarga.Codigo
        });
    });

    return JSON.stringify(listaTipoRetornoCargaRetornar);
}

function preencherAreaVeiculoTipoRetornoCarga(dadosTipoRetornoCarga) {
    _gridAreaVeiculoTipoRetornoCarga.CarregarGrid(dadosTipoRetornoCarga);
}

/*
 * Declaração das Funções
 */

function obterListaTipoRetornoCarga() {
    return _gridAreaVeiculoTipoRetornoCarga.BuscarRegistros();
}

function removerTipoRetornoCarga(registroSelecionado) {
    var listaTipoRetornoCarga = obterListaTipoRetornoCarga();

    for (var i = 0; i < listaTipoRetornoCarga.length; i++) {
        if (registroSelecionado.Codigo == listaTipoRetornoCarga[i].Codigo) {
            listaTipoRetornoCarga.splice(i, 1);
            break;
        }
    }

    _gridAreaVeiculoTipoRetornoCarga.CarregarGrid(listaTipoRetornoCarga);
}