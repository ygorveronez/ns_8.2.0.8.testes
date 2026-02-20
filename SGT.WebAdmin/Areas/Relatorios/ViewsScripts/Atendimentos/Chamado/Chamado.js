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
/// <reference path="../../../../../ViewsScripts/Consultas/Sistema.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Modulo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tela.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoAtendimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioChamados, _gridChamados, _pesquisaChamados, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _prioridadePesquisa = [
    { text: "Todos", value: EnumPrioridadeAtendimento.Todos },
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _statusChamadoPesquisa = [
    { text: "Todos", value: EnumStatusAtendimentoChamado.Todos },
    { text: "Aberto", value: EnumStatusAtendimentoChamado.Aberto },
    { text: "Cancelado", value: EnumStatusAtendimentoChamado.Cancelado },
    { text: "Finalizado", value: EnumStatusAtendimentoChamado.Finalizado }
];

var PesquisaChamados = function () {
    this.DataChamadoInicial = PropertyEntity({ text: "Data Chamado Inicial: ", getType: typesKnockout.date });
    this.DataChamadoFinal = PropertyEntity({ text: "Data Chamado Final: ", getType: typesKnockout.date });
    this.DataAtendimentoInicial = PropertyEntity({ text: "Data Atendimento Inicial: ", getType: typesKnockout.date });
    this.DataAtendimentoFinal = PropertyEntity({ text: "Data Atendimento Final: ", getType: typesKnockout.date });

    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAtendimentoChamado.Todos), options: _statusChamadoPesquisa, def: EnumStatusAtendimentoChamado.Todos, text: "Status: " });
    this.Prioridade = PropertyEntity({ val: ko.observable(EnumPrioridadeAtendimento.Todos), options: _prioridadePesquisa, def: EnumPrioridadeAtendimento.Todos, text: "Prioridade: " });
    this.Titulo = PropertyEntity({ text: "Título: ", maxlength: 100 });

    this.Tela = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tela: ", idBtnSearch: guid(), visible: true });
    this.Sistema = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Sistema: ", idBtnSearch: guid(), visible: true });
    this.Modulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Módulo: ", idBtnSearch: guid(), visible: true });
    this.Tipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Atendimento: ", idBtnSearch: guid(), visible: true });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Atendente: ", idBtnSearch: guid(), visible: true });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa: ", idBtnSearch: guid(), visible: true });
    this.EmpresaFilho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa Filho: ", idBtnSearch: guid(), visible: true });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Solicitante:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridChamados.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioChamados() {

    _pesquisaChamados = new PesquisaChamados();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridChamados = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Chamado/Pesquisa", _pesquisaChamados, null, null, 10);
    _gridChamados.SetPermitirEdicaoColunas(true);

    _relatorioChamados = new RelatorioGlobal("Relatorios/Chamado/BuscarDadosRelatorio", _gridChamados, function () {
        _relatorioChamados.loadRelatorio(function () {
            KoBindings(_pesquisaChamados, "knockoutPesquisaChamados");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaChamados");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaChamados");

            new BuscarSistemas(_pesquisaChamados.Sistema);
            new BuscarModulos(_pesquisaChamados.Modulo);
            new BuscarTelas(_pesquisaChamados.Tela);
            new BuscarFuncionario(_pesquisaChamados.Funcionario);
            new BuscarTipoAtendimento(_pesquisaChamados.Tipo);
            new BuscarFuncionario(_pesquisaChamados.Solicitante);

            new BuscarEmpresa(_pesquisaChamados.Empresa, function (data) {
                _pesquisaChamados.Empresa.codEntity(data.Codigo);
                _pesquisaChamados.Empresa.val(data.RazaoSocial);
            }, null);
            new BuscarEmpresa(_pesquisaChamados.EmpresaFilho, function (data) {
                _pesquisaChamados.EmpresaFilho.codEntity(data.Codigo);
                _pesquisaChamados.EmpresaFilho.val(data.RazaoSocial);
            }, null);
            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaChamados);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioChamados.gerarRelatorio("Relatorios/Chamado/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioChamados.gerarRelatorio("Relatorios/Chamado/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}