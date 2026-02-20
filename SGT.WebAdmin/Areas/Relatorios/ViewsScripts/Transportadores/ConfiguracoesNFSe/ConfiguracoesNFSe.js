/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracoesNFSe, _pesquisaConfiguracoesNFSe, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioConfiguracoesNFSe;

var PesquisaConfiguracoesNFSe = function () {

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", issue: 69, idBtnSearch: guid(), required: true });
    this.LocalidadePrestacaoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade da Prestação do Serviço:", issue: 766, idBtnSearch: guid() });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço", issue: 768, idBtnSearch: guid() });
    this.ClienteTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente do Tomador", idBtnSearch: guid() });
    this.GrupoTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo do Tomador", idBtnSearch: guid() });
    this.LocalidadeTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade do Tomador", issue: 766, idBtnSearch: guid() });
    this.UFTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF Tomador", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatoriosNFSe())
                _gridConfiguracoesNFSe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaConfiguracoesNFSe.Visible.visibleFade() == true) {
                _pesquisaConfiguracoesNFSe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaConfiguracoesNFSe.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadConfiguracoesNFSe() {
    _pesquisaConfiguracoesNFSe = new PesquisaConfiguracoesNFSe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridConfiguracoesNFSe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ConfiguracoesNFSe/Pesquisa", _pesquisaConfiguracoesNFSe);

    _gridConfiguracoesNFSe.SetPermitirEdicaoColunas(true);
    _gridConfiguracoesNFSe.SetQuantidadeLinhasPorPagina(20);

    _relatorioConfiguracoesNFSe = new RelatorioGlobal("Relatorios/ConfiguracoesNFSe/BuscarDadosRelatorio", _gridConfiguracoesNFSe, function () {
        _relatorioConfiguracoesNFSe.loadRelatorio(function () {
            KoBindings(_pesquisaConfiguracoesNFSe, "knockoutPesquisaConfiguracoesNFSe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaConfiguracoesNFSe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaConfiguracoesNFSe", false);

            new BuscarEmpresa(_pesquisaConfiguracoesNFSe.Transportador);
            new BuscarLocalidades(_pesquisaConfiguracoesNFSe.LocalidadePrestacaoServico);
            new BuscarLocalidades(_pesquisaConfiguracoesNFSe.LocalidadeTomador);
            new BuscarServicoNFSe(_pesquisaConfiguracoesNFSe.Servico);
            new BuscarClientes(_pesquisaConfiguracoesNFSe.ClienteTomador);
            new BuscarGruposPessoas(_pesquisaConfiguracoesNFSe.GrupoTomador);
            new BuscarEstados(_pesquisaConfiguracoesNFSe.UFTomador);
            new BuscarTiposOperacao(_pesquisaConfiguracoesNFSe.TipoOperacao);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaConfiguracoesNFSe);
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatoriosNFSe())
        _relatorioConfiguracoesNFSe.gerarRelatorio("Relatorios/ConfiguracoesNFSe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatoriosNFSe())
        _relatorioConfiguracoesNFSe.gerarRelatorio("Relatorios/ConfiguracoesNFSe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatoriosNFSe() {
    if (!ValidarCamposObrigatorios(_pesquisaConfiguracoesNFSe)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os filtros obrigatórios");
        return false;
    }
    else
        return true;
}
