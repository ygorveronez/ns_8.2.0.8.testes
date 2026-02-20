/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTipoCarga;
var _tipoCarga;

/*
 * Declaração das Classes
 */

var TipoCarga = function () {
    this.TipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid(), idGrid: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadTipoCarga() {
    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, "knockoutTipoCarga");

    loadGridTipoCarga();
}

function loadGridTipoCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTipoCargaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTipoCarga = new BasicDataTable(_tipoCarga.TipoCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_tipoCarga.TipoCarga, null, null, _gridTipoCarga);

    _tipoCarga.TipoCarga.basicTable = _gridTipoCarga;
    _tipoCarga.TipoCarga.basicTable.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirTipoCargaClick(registroSelecionado) {
    var listaTipoCarga = obterListaTipoCarga();

    for (var i = 0; i < listaTipoCarga.length; i++) {
        if (registroSelecionado.Codigo == listaTipoCarga[i].Codigo) {
            listaTipoCarga.splice(i, 1);
            break;
        }
    }

    _gridTipoCarga.CarregarGrid(listaTipoCarga);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposTipoCarga() {
    LimparCampos(_tipoCarga);
    _gridTipoCarga.CarregarGrid([]);
}

function preencherTipoCarga(tiposCarga) {
    _gridTipoCarga.CarregarGrid(tiposCarga);
}

function preencherTipoCargaSalvar(configuracaoRotaFrete) {
    configuracaoRotaFrete["TiposCarga"] = obterListaTipoCargaSalvar();
}

/*
 * Declaração das Funções Privadas
 */

function obterListaTipoCarga() {
    return _tipoCarga.TipoCarga.basicTable.BuscarRegistros();
}

function obterListaTipoCargaSalvar() {
    var listaTipoCarga = obterListaTipoCarga();

    return JSON.stringify(listaTipoCarga);
}
