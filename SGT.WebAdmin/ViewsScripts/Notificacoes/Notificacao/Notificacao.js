/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNotificacao.js" />
/// <reference path="../../Enumeradores/EnumTipoNotificacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridNotificacao;
var _notificacao;
var _pesquisaNotificacao;

var _situacao = [
    { text: "Todos", value: EnumSituacaoNotificacao.Todas },
    { text: "Não lidos", value: EnumSituacaoNotificacao.Nova },
    { text: "Lidos", value: EnumSituacaoNotificacao.Lida }
];


var PesquisaNotificacao = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoNotificacao.Todas), options: _situacao, def: EnumSituacaoNotificacao.Todas, getType: typesKnockout.int, text: "Situação: " });
    this.DataInicio = PropertyEntity({ text: "Data inicio: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotificacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}
//*******EVENTOS*******


function loadNotificacao() {

    _pesquisaNotificacao = new PesquisaNotificacao();
    KoBindings(_pesquisaNotificacao, "knockoutPesquisaNotificacao", false, _pesquisaNotificacao.Pesquisar.id);

    buscarNotificacaos();

}

//*******MÉTODOS*******


function buscarNotificacaos() {
    var editar = { descricao: "Ir para notificacao", id: "clasEditar", evento: "onclick", metodo: detalheNotificacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNotificacao = new GridView(_pesquisaNotificacao.Pesquisar.idGrid, "Notificacao/Pesquisa", _pesquisaNotificacao, menuOpcoes, null);
    _gridNotificacao.CarregarGrid();
}

function detalheNotificacao(e) {
    if (e.SituacaoNotificacao == EnumSituacaoNotificacao.Nova) {
        _RequisicaoIniciada = true;
        var data = { Codigo: e.Codigo };
        executarReST("Notificacao/MarcarNotificacaoComoLida", data, function (arg) {
            _RequisicaoIniciada = false;
            direcionar(e);
        });
    } else {
            direcionar(e);
    }


}

function direcionar(e) {
    if (e.TipoNotificacao == EnumTipoNotificacao.relatorio) {
        var data = { Codigo: e.CodigoObjetoNotificacao };
        executarDownload("Relatorios/Relatorio/DownloadRelatorio", data);
    }
    else if (e.TipoNotificacao == EnumTipoNotificacao.arquivo) {
        var data = { Codigo: e.CodigoObjetoNotificacao };
        executarDownload("ControleGeracaoArquivo/DownloadArquivo", data);
    }
    else if (e.CodigoObjetoNotificacao > 0 && e.URLPagina != "" && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        _notificacaoGlobal.CodigoObjeto.val(e.CodigoObjetoNotificacao);

        if (location.hash == "#" + e.URLPagina)
            checkURL();
        else
            location.href = "#" + e.URLPagina;
    }
}