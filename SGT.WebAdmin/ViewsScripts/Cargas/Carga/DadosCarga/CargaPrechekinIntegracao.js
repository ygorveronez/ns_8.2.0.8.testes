/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js"/>

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridIntegracoesPreChekin;
var _cargaPreChekinIntegracao;
var _pesquisaCargaPreChekinIntegracoes;
var _pesquisaHistoricoIntegracaoPreChekin;
var _gridHistoricoIntegracaoPreChekin;
var _codigoDadosEmisao;
/*
 * Declaração das Classes
 */
function PesquisaHistoricoIntegracaoPreChekin() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function PesquisaCargaPreChekinIntegracoes() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaIntegracaoOutros.Todos), options: EnumSituacaoCargaIntegracaoOutros.ObterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoCargaIntegracaoOutros.Todos });

    this.ReenviarTodos = PropertyEntity({
        eventClick: () => { }, type: types.event, text: "Liberar Sem Integração", idGrid: guid(), visible: ko.observable(false), visible: ko.observable(false)
    });
    this.Pesquisar = PropertyEntity({ eventClick: carregarIntegracoesCargaPreChekin, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};


function CargaPreChekinIntegracao() {
    this.TotalAguardandoIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoIntegracao.getFieldDescription() });
    this.TotalAguardandoRetorno = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.AguardandoRetorno.getFieldDescription() });
    this.TotalGeral = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.TotalGeral.getFieldDescription() });
    this.TotalIntegrado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.Integrados.getFieldDescription() });
    this.TotalProblemaIntegracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Gerais.Geral.ProblemasIntegracao.getFieldDescription() });

    this.ObterTotais = PropertyEntity({ eventClick: carregarTotaisIntegracaoPreChekin, type: types.event, text: Localization.Resources.Gerais.Geral.ObterTotais, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCargaPreChekinIntegracoes() {
    var linhasPorPaginas = 5;
    var historico = { descricao: Localization.Resources.Gerais.Geral.HistoricoIntegracao, id: guid(), metodo: exibirHistoricoIntegracoesPrechekinClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [historico] };

    _gridIntegracoesPreChekin = new GridView(_pesquisaCargaPreChekinIntegracoes.Pesquisar.idGrid, "CargaPreChekinIntegracao/PesquisaCargaPreChekinIntegracoes", _pesquisaCargaPreChekinIntegracoes, menuOpcoes, null, linhasPorPaginas);
    _gridIntegracoesPreChekin.CarregarGrid();
    carregarTotaisIntegracaoPreChekin();
}

function loadDadosIntegracaoCargaPreChekinIntegracao(carga, etapaTransportador) {
    $.get("Content/Static/Integracao/Integracao.html?dyn=" + guid(), function (html) {
        if (etapaTransportador)
            _codigoDadosEmisao = carga.EtapaDadosTransportador.idGrid;
        else
            _codigoDadosEmisao = carga.EtapaInicioTMS.idGrid;

        $("#tabIntegracaoPreChekin_" + _codigoDadosEmisao).removeClass("active");

        VerificarExisteDivIntegracaoPreChekin(_codigoDadosEmisao, html)

        LocalizeCurrentPage();

        let codigoCarga = carga.Codigo.val();

        _cargaPreChekinIntegracao = new CargaPreChekinIntegracao();
        KoBindings(_cargaPreChekinIntegracao, "knockoutDadosIntegracao_" + _codigoDadosEmisao);

        _pesquisaCargaPreChekinIntegracoes = new PesquisaCargaPreChekinIntegracoes();
        KoBindings(_pesquisaCargaPreChekinIntegracoes, "knockoutPesquisaIntegracao_" + _codigoDadosEmisao, false, _pesquisaCargaPreChekinIntegracoes.Pesquisar.id);

        _pesquisaCargaPreChekinIntegracoes.Codigo.val(codigoCarga);

        loadGridCargaPreChekinIntegracoes();

    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirHistoricoIntegracoesPrechekinClick(integracao) {
    BuscarHistoricoIntegracaoCargaPreChekin(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCargaPreChekin");
}


/*
 * Declaração das Funções Públicas
 */

function recarregarCargaFreteIntegracoes() {
    if (_pesquisaCargaPreChekinIntegracoes == null)
        return;

    _pesquisaCargaPreChekinIntegracoes.Codigo.val(_carga.Codigo.val());
    carregarIntegracoesCargaPreChekin();

}

/*
 * Declaração das Funções
 */

function DownloadArquivosHistoricoIntegracaoCargaPreChekinClick(integraco) {
    executarDownload("CargaPreChekinIntegracao/DownloadArquivosHistoricoIntegracao", { Codigo: integraco.Codigo });
}

function BuscarHistoricoIntegracaoCargaPreChekin(integracao) {
    _pesquisaHistoricoIntegracaoPreChekin = new PesquisaHistoricoIntegracaoPreChekin();
    _pesquisaHistoricoIntegracaoPreChekin.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCargaPreChekinClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoPreChekin = new GridView("tblHistoricoIntegracaoCargaPreChekin", "CargaPreChekinIntegracao/ConsultarHistoricoIntegracaoCargaPreChekin", _pesquisaHistoricoIntegracaoPreChekin, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoPreChekin.CarregarGrid();
}

function carregarIntegracoesCargaPreChekin() {
    _gridIntegracoesPreChekin.CarregarGrid();
    carregarTotaisIntegracaoPreChekin();
}

function carregarTotaisIntegracaoPreChekin() {
    executarReST("CargaPreChekinIntegracao/ObterTotaisIntegracoesCargaPreChekin", { Codigo: _pesquisaCargaPreChekinIntegracoes.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            _cargaPreChekinIntegracao.TotalGeral.val(retorno.Data.TotalGeral);
            _cargaPreChekinIntegracao.TotalAguardandoIntegracao.val(retorno.Data.TotalAguardandoIntegracao);
            _cargaPreChekinIntegracao.TotalAguardandoRetorno.val(retorno.Data.TotalAguardandoRetorno);
            _cargaPreChekinIntegracao.TotalProblemaIntegracao.val(retorno.Data.TotalProblemaIntegracao);
            _cargaPreChekinIntegracao.TotalIntegrado.val(retorno.Data.TotalIntegrado);
            VisualizacaoEtapa();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function VerificarExisteDivIntegracaoPreChekin(id, html) {
    html = html.replace("knockoutPesquisaIntegracao", "knockoutPesquisaIntegracao_" + id);
    html = html.replace("knockoutDadosIntegracao", "knockoutDadosIntegracao_" + id);
    $("#tabIntegracaoPreChekin_" + id).html(html);
}