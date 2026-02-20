/// <reference path="../../Enumeradores/EnumSituacaoValePedagio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoInfracao.js" />
/// <reference path="Extratos.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridExtratoValePedagio;
var _pesquisaExtratoValePedagio;


var PesquisaExtratoValePedagio = function () {
    this.ExibirFiltros = PropertyEntity({
        text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true),
        eventClick: function (e, sender) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }
    });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Código da carga:" });
    this.DataCargaInicial = PropertyEntity({ text: "Data carga inicial: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataCargaFinal = PropertyEntity({ text: "Data carga final: ", getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable("") });
    this.DataCargaInicial.dateRangeLimit = this.DataCargaFinal;
    this.DataCargaFinal.dateRangeInit = this.DataCargaInicial;
    this.NumeroValePedagio = PropertyEntity({ text: "Número VP: ", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.SituacaoExtrato = PropertyEntity({ text: "Situação do extrato:", options: EnumSituacaoExtrato.obterOpcoesPesquisa(), val: ko.observable(EnumSituacaoExtrato.Todas), def: EnumSituacaoExtrato.Todas });

    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarExtratoValePedagio });
};

//*******EVENTOS*******

function loadExtratoValePedagio() {
    _pesquisaExtratoValePedagio = new PesquisaExtratoValePedagio();
    KoBindings(_pesquisaExtratoValePedagio, "knockoutPesquisaExtratoValePedagio", false, _pesquisaExtratoValePedagio.Pesquisar.id);

    loadGridExtratoValePedagio();
    loadExtratos();
}

function loadGridExtratoValePedagio() {
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalheExtratoValePedagioClick };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalhes], tamanho: 5 };
    var configuracoesExportacao = { url: "ExtratoValePedagio/ExportarPesquisa", titulo: "Extrato Vale Pedágio" };

    _gridExtratoValePedagio = new GridViewExportacao(_pesquisaExtratoValePedagio.Pesquisar.idGrid, "ExtratoValePedagio/Pesquisa", _pesquisaExtratoValePedagio, menuOpcoes, configuracoesExportacao, null, 5);
    _gridExtratoValePedagio.CarregarGrid();
}

function pesquisarExtratoValePedagio(e, sender) {
    _gridExtratoValePedagio.CarregarGrid();
}

function detalheExtratoValePedagioClick(data) {
    _pesquisaExtratos.CodigoExtrato.val(data.Codigo);
    consultarExtratosValePedagio();
}