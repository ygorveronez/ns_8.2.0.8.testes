/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
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
/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoMovimento.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoMovimento.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoMovimento.js" />
/// <reference path="../../Enumeradores/TipoConsolidacaoMovimentoFinanceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMovimentoFinanceiro, _pesquisaMovimentoFinanceiro, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioMovimentoFinanceiro;

var _SituacaoMovimento = [
    { text: "Todos", value: TipoConsolidacaoMovimentoFinanceiro.Todos },
    { text: "Consolidado", value: TipoConsolidacaoMovimentoFinanceiro.Consolidado },
    { text: "Não consolidado", value: TipoConsolidacaoMovimentoFinanceiro.NaoConsolidado }
]


var PesquisaMovimentoFinanceiro = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });   

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid() });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Crédito:", idBtnSearch: guid() });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Débito:", idBtnSearch: guid() });
    this.ValorMovimento = PropertyEntity({ text: "Valor: ", getType: typesKnockout.decimal });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Todos), options: EnumTipoDocumentoMovimento.obterOpcoesPesquisa(), text: "Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Todos });
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, def: ko.observable("") });
    this.DataMovimentoInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataMovimentoFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataBaseFinanceiro = PropertyEntity({ text: "Data Base: ", getType: typesKnockout.date });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento: " });
    this.Observacao = PropertyEntity({ text: "Observação: " });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid() });
    this.SituacaoMovimento = PropertyEntity({ val: ko.observable(TipoConsolidacaoMovimentoFinanceiro.Todos), options: _SituacaoMovimento, text: "Situação do Movimento: ", def: TipoConsolidacaoMovimentoFinanceiro.Todos });

    this.DataMovimentoInicial.dateRangeLimit = this.DataMovimentoFinal;
    this.DataMovimentoFinal.dateRangeInit = this.DataMovimentoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentoFinanceiro.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentoFinanceiro.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function LoadMovimentoFinanceiro() {
    _pesquisaMovimentoFinanceiro = new PesquisaMovimentoFinanceiro();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMovimentoFinanceiro = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/MovimentoFinanceiro/Pesquisa", _pesquisaMovimentoFinanceiro);

    _gridMovimentoFinanceiro.SetPermitirEdicaoColunas(true);
    _gridMovimentoFinanceiro.SetQuantidadeLinhasPorPagina(10);



    _relatorioMovimentoFinanceiro = new RelatorioGlobal("Relatorios/MovimentoFinanceiro/BuscarDadosRelatorio", _gridMovimentoFinanceiro, function () {
        _relatorioMovimentoFinanceiro.loadRelatorio(function () {
            KoBindings(_pesquisaMovimentoFinanceiro, "knockoutPesquisaMovimentoFinanceiro", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMovimentoFinanceiro", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMovimentoFinanceiro", false);

            new BuscarTipoMovimento(_pesquisaMovimentoFinanceiro.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

            new BuscarCentroResultado(_pesquisaMovimentoFinanceiro.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", null, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaMovimentoFinanceiro.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
            new BuscarPlanoConta(_pesquisaMovimentoFinanceiro.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
            //new BuscarTipoMovimento(_pesquisaMovimentoFinanceiro.TipoMovimento, null, null, null, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

            new BuscarClientes(_pesquisaMovimentoFinanceiro.Pessoa);
            new BuscarGruposPessoas(_pesquisaMovimentoFinanceiro.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Ambos);


            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMovimentoFinanceiro);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMovimentoFinanceiro.gerarRelatorio("Relatorios/MovimentoFinanceiro/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMovimentoFinanceiro.gerarRelatorio("Relatorios/MovimentoFinanceiro/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function RetornoTipoMovimento(data) {
    if (data != null) {
        _pesquisaMovimentoFinanceiro.TipoMovimento.codEntity(data.Codigo);
        _pesquisaMovimentoFinanceiro.TipoMovimento.val(data.Descricao);

        if (data.CodigoDebito > 0) {
            _pesquisaMovimentoFinanceiro.PlanoDebito.codEntity(data.CodigoDebito);
            _pesquisaMovimentoFinanceiro.PlanoDebito.val(data.PlanoDebito);
        }
        if (data.CodigoCredito > 0) {
            _pesquisaMovimentoFinanceiro.PlanoCredito.codEntity(data.CodigoCredito);
            _pesquisaMovimentoFinanceiro.PlanoCredito.val(data.PlanoCredito);
        }
        if (data.CodigoResultado > 0) {
            _pesquisaMovimentoFinanceiro.CentroResultado.codEntity(data.CodigoResultado);
            _pesquisaMovimentoFinanceiro.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_pesquisaMovimentoFinanceiro.CentroResultado);
        }
    }
}

function RetornoBuscarClientes(data) {
    _pesquisaMovimentoFinanceiro.Pessoa.val(data.Nome);
    _pesquisaMovimentoFinanceiro.Pessoa.codEntity(data.Codigo);
    if (data.CodigoGrupo > 0) {
        LimparCampoEntity(_pesquisaMovimentoFinanceiro.GrupoPessoa);
        _pesquisaMovimentoFinanceiro.GrupoPessoa.val(data.DescricaoGrupo);
        _pesquisaMovimentoFinanceiro.GrupoPessoa.codEntity(data.CodigoGrupo);
    }
}