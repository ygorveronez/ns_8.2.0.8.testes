/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoTitulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _documentosConciliacao, _gridDocumentoConciliacao, _pesquisaDocumentoConciliacao, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _StatusTituloReceber = [
    { text: "Todos", value: EnumSituacaoTitulo.Todos },
    { text: "Em aberto", value: EnumSituacaoTitulo.EmAberto },
    { text: "Não provisionado", value: EnumSituacaoTitulo.Atrazado },
    { text: "Quitado", value: EnumSituacaoTitulo.Quitado },
    { text: "Bloqueado", value: EnumSituacaoTitulo.Bloqueado }
];

var PesquisaDocumentosConciliacao = function () {
    this.NumeroCTe = PropertyEntity({ text: "Número CT-e:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial (Emissão CT-e):", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Emissão CT-e):", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.StatusTitulo = PropertyEntity({ val: ko.observable(EnumSituacaoTitulo.EmAberto), options: _StatusTituloReceber, def: EnumSituacaoTitulo.EmAberto, text: "Situação: " });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", issue: 69, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente", idBtnSearch: guid() });
    this.NumeroFatura = PropertyEntity({ text: "Nº. Fatura: ", getType: typesKnockout.int });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.PossuiIntegracaoMarfrig = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoConciliacao.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function BuscarConfiguracaoIntegracao() {
    var p = new promise.Promise();

    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success) {
            if (r.Data.PossuiIntegracaoMarfrig) {
                _pesquisaDocumentoConciliacao.PossuiIntegracaoMarfrig.val(true);
            }

            p.done();

        } else {
            p.done();
        }

    });

    return p;
}

function loadRelatorioCTeTituloReceber() {
    _pesquisaDocumentoConciliacao = new PesquisaDocumentosConciliacao();

    BuscarConfiguracaoIntegracao().then(function () {

        _CRUDRelatorio = new CRUDRelatorio();
        _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

        var menuOpcoes = null;
        console.log(_pesquisaDocumentoConciliacao.PossuiIntegracaoMarfrig.val());

        if (_pesquisaDocumentoConciliacao.PossuiIntegracaoMarfrig.val()) {
            menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
            menuOpcoes.opcoes.push({ descricao: "Consultar", id: guid(), metodo: ReenviarIntegracao, tamanho: "20", icone: "" });
            menuOpcoes.opcoes.push({ descricao: "Histórico de Integração", id: guid(), metodo: ExibirHistoricoIntegracao, tamanho: "20", icone: "" });
        }

        _gridDocumentoConciliacao = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CTeTituloReceber/Pesquisa", _pesquisaDocumentoConciliacao, menuOpcoes, null, 10);
        _gridDocumentoConciliacao.SetPermitirEdicaoColunas(true);

        _documentosConciliacao = new RelatorioGlobal("Relatorios/CTeTituloReceber/BuscarDadosRelatorio", _gridDocumentoConciliacao, function () {
            _documentosConciliacao.loadRelatorio(function () {
                KoBindings(_pesquisaDocumentoConciliacao, "knockoutPesquisaCTeTituloReceber");
                KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTeTituloReceber");
                KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTeTituloReceber");

                new BuscarTransportadores(_pesquisaDocumentoConciliacao.Empresa);
                new BuscarFilial(_pesquisaDocumentoConciliacao.Filial);
                new BuscarClientes(_pesquisaDocumentoConciliacao.Remetente);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    _pesquisaDocumentoConciliacao.Empresa.visible(true);
                    _pesquisaDocumentoConciliacao.Filial.visible(true);
                }

                $("#divConteudoRelatorio").show();
            })
        }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaDocumentoConciliacao);

    });

}

function GerarRelatorioPDFClick(e, sender) {
    _documentosConciliacao.gerarRelatorio("Relatorios/CTeTituloReceber/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _documentosConciliacao.gerarRelatorio("Relatorios/CTeTituloReceber/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ReenviarIntegracao(data) {
    executarReST("Relatorios/CTeTituloReceber/IntegrarTitulo", { Codigo: data.Codigo }, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Título Integrado com sucesso.");
            _gridDocumentoConciliacao.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ExibirHistoricoIntegracao(integracao) {
    BuscarHistoricoIntegracao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoMarfrig");
}

var _pesquisaHistoricoIntegracao;

var PesquisaHistoricoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

function BuscarHistoricoIntegracao(integracao) {
    _pesquisaHistoricoIntegracao = new PesquisaHistoricoIntegracao();
    _pesquisaHistoricoIntegracao.Codigo.val(integracao.Codigo);

    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracao = new GridView("tblHistoricoIntegracao", "Relatorios/CTeTituloReceber/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracao(historicoConsulta) {
    executarDownload("Relatorios/CTeTituloReceber/DownloadArquivosHistoricoIntegracao", { CodigoIntegracao: _pesquisaHistoricoIntegracao.Codigo.val(), Codigo: historicoConsulta.Codigo });
}