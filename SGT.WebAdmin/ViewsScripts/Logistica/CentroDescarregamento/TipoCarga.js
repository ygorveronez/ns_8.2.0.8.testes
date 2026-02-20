/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="TempoDescarregamento.js" />
/// <reference path="CentroDescarregamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoCarga;
var _tipoCarga;

var TipoCarga = function () {
    this.Adicionar = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarTipoCarga, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), visible: ko.observable(true) });
}


//*******EVENTOS*******

function LoadTipoCarga() {
    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, 'knockoutTipoCarga');

    loadGridTipoCarga();

    if (_configuracaoCentroDescarregamento.UtilizarCentroDescarregamentoPorTipoCarga)
        $('#liTabTipoCarga').show();
}

function loadGridTipoCarga() {
    var linhasPorPaginas = 10;
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirTipoCargaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%", className: "text-align-left" },
    ];

    _gridTipoCarga = new BasicDataTable(_tipoCarga.Adicionar.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    BuscarTiposdeCarga(_tipoCarga.Adicionar, null, null, _gridTipoCarga);

    _gridTipoCarga.CarregarGrid([]);
}

function excluirTipoCargaClick(registroSelecionado) {
    let tps = _gridTipoCarga.BuscarRegistros();

    for (var i = 0; i < tps.length; i++) {
        if (registroSelecionado.Codigo == tps[i].Codigo) {
            tps.splice(i, 1);
            break;
        }
    }

    _gridTipoCarga.CarregarGrid(tps);
}

function renderizarGridTipoCarga() {

    let tps = _gridTipoCarga.BuscarRegistros();

    _gridTipoCarga.CarregarGrid(tps);
}

function recarregarGridTipoCarga() {
    _gridTipoCarga.CarregarGrid(_centroDescarregamento.TiposCarga.val());
}
