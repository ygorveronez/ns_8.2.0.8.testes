/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridModeloVeicularCarga;
var _modeloVeicularCarga;

/*
 * Declaração das Classes
 */

var ModeloVeicularCarga = function () {
    this.ModeloVeicularCarga = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid(), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadModeloVeicularCarga() {
    _modeloVeicularCarga = new ModeloVeicularCarga();
    KoBindings(_modeloVeicularCarga, "knockoutModeloVeicularCarga");

    loadGridModeloVeicularCarga();
}

function loadGridModeloVeicularCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirModeloVeicularCargaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridModeloVeicularCarga = new BasicDataTable(_modeloVeicularCarga.ModeloVeicularCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_modeloVeicularCarga.ModeloVeicularCarga, null, null, null, null, null,_gridModeloVeicularCarga);

    _modeloVeicularCarga.ModeloVeicularCarga.basicTable = _gridModeloVeicularCarga;
    _modeloVeicularCarga.ModeloVeicularCarga.basicTable.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirModeloVeicularCargaClick(registroSelecionado) {
    var listaModeloVeicularCarga = obterListaModeloVeicularCarga();

    for (var i = 0; i < listaModeloVeicularCarga.length; i++) {
        if (registroSelecionado.Codigo == listaModeloVeicularCarga[i].Codigo) {
            listaModeloVeicularCarga.splice(i, 1);
            break;
        }
    }

    _gridModeloVeicularCarga.CarregarGrid(listaModeloVeicularCarga);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposModeloVeicularCarga() {
    LimparCampos(_modeloVeicularCarga);
    _gridModeloVeicularCarga.CarregarGrid([]);
}

function preencherModeloVeicularCarga(modelosVeicularesCarga) {
    _gridModeloVeicularCarga.CarregarGrid(modelosVeicularesCarga);
}

function preencherModeloVeicularCargaSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["ModelosVeicularesCarga"] = obterListaModeloVeicularCargaSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function obterListaModeloVeicularCarga() {
    return _modeloVeicularCarga.ModeloVeicularCarga.basicTable.BuscarRegistros();
}

function obterListaModeloVeicularCargaSalvar() {
    var listaModeloVeicularCarga = obterListaModeloVeicularCarga();

    return JSON.stringify(listaModeloVeicularCarga);
}
