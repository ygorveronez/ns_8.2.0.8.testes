/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NaturezaOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Servico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoMovimento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoDocumentoEntrada.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusFinanceiro.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNotasDetalhadas, _gridNotasDetalhadas, _pesquisaNotasDetalhadas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusFinanceiroPesquisa = [
    { text: "Todos", value: EnumStatusFinanceiro.Todos },
    { text: "Em Aberto", value: EnumStatusFinanceiro.Aberto },
    { text: "Quitado", value: EnumStatusFinanceiro.Quitado },
    { text: "Renegociado", value: EnumStatusFinanceiro.Renegociado }
];

var PesquisaNotasDetalhadas = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Emissão Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Emissão Final: ", getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.Chave = PropertyEntity({ text: "Chave: ", getType: typesKnockout.string, maxlength: 44 });
    this.DataEntradaInicial = PropertyEntity({ text: "Data Entrada Inicial: ", getType: typesKnockout.date });
    this.DataEntradaFinal = PropertyEntity({ text: "Data Entrada Final: ", getType: typesKnockout.date });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Data Vencimento Final: ", getType: typesKnockout.date });

    this.StatusNotaEntrada = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoEntrada.Todos), options: EnumSituacaoDocumentoEntrada.obterOpcoesPesquisa(), def: EnumSituacaoDocumentoEntrada.Todos, text: "Status: ", visible: ko.observable(true) });
    this.SituacaoFinanceiraNotaEntrada = PropertyEntity({ val: ko.observable(0), options: _statusFinanceiroPesquisa, def: 0, text: "Situação Financeira: ", visible: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "Tip. Movimento: " });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.NaturezaOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Natureza da Operação: ", idBtnSearch: guid(), visible: true });
    this.EstadoEmitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF Emitente: ", idBtnSearch: guid(), visible: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: true });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço: ", idBtnSearch: guid(), visible: true });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo: ", idBtnSearch: guid(), visible: false });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0 });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento Veículo: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoProduto = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoMovimentoItem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tip. Movimento Doc. Entrada:", idBtnSearch: guid(), issue: 0 });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa: ", idBtnSearch: guid() });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataFinalizacaoInicial = PropertyEntity({ text: "Data Finalização Inicial: ", getType: typesKnockout.date });
    this.DataFinalizacaoFinal = PropertyEntity({ text: "Data Finalização Final: ", getType: typesKnockout.date });
    this.DataFinalizacaoInicial.dataRangeLimit = this.DataFinalizacaoFinal;
    this.DataFinalizacaoFinal.dataRangeInit = this.DataFinalizacaoInicial;

    this.OperadorLancamentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador de Lançamento: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OperadorFinalizaEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador de Finalização: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.NotasComDiferencaDeValorTabelaFornecedor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Notas Com Diferença De Valor Na Tabela do Fornecedor", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNotasDetalhadas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNotasDetalhadas.Visible.visibleFade()) {
                _pesquisaNotasDetalhadas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNotasDetalhadas.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function loadRelatorioNotasDetalhadas() {

    _pesquisaNotasDetalhadas = new PesquisaNotasDetalhadas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNotasDetalhadas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NotasDetalhadas/Pesquisa", _pesquisaNotasDetalhadas, null, null, 10);
    _gridNotasDetalhadas.SetPermitirEdicaoColunas(true);

    _relatorioNotasDetalhadas = new RelatorioGlobal("Relatorios/NotasDetalhadas/BuscarDadosRelatorio", _gridNotasDetalhadas, function () {
        _relatorioNotasDetalhadas.loadRelatorio(function () {
            KoBindings(_pesquisaNotasDetalhadas, "knockoutPesquisaNotasDetalhadas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNotasDetalhadas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNotasDetalhadas");

            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaNotasDetalhadas.NaturezaOperacao);
            new BuscarClientes(_pesquisaNotasDetalhadas.Pessoa);
            new BuscarProdutoTMS(_pesquisaNotasDetalhadas.Produto);
            new BuscarServicoTMS(_pesquisaNotasDetalhadas.Servico);
            new BuscarModeloDocumentoFiscal(_pesquisaNotasDetalhadas.Modelo);
            new BuscarTipoMovimento(_pesquisaNotasDetalhadas.TipoMovimentoItem);
            new BuscarEmpresa(_pesquisaNotasDetalhadas.Empresa);
            new BuscarEstados(_pesquisaNotasDetalhadas.EstadoEmitente);
            new BuscarVeiculos(_pesquisaNotasDetalhadas.Veiculo);
            new BuscarGruposProdutosTMS(_pesquisaNotasDetalhadas.GrupoProduto, null);
            new BuscarSegmentoVeiculo(_pesquisaNotasDetalhadas.Segmento);
            new BuscarModeloDocumentoFiscal(_pesquisaNotasDetalhadas.ModeloDocumentoFiscal);
            new BuscarGruposPessoas(_pesquisaNotasDetalhadas.GrupoPessoa);
            new BuscarEquipamentos(_pesquisaNotasDetalhadas.Equipamento);
            new BuscarOperador(_pesquisaNotasDetalhadas.OperadorFinalizaEntrada);
            new BuscarOperador(_pesquisaNotasDetalhadas.OperadorLancamentoEntrada);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaNotasDetalhadas.Empresa.visible(false);
                _pesquisaNotasDetalhadas.StatusNotaEntrada.visible(false);
                _pesquisaNotasDetalhadas.SituacaoFinanceiraNotaEntrada.visible(false);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNotasDetalhadas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotasDetalhadas.gerarRelatorio("Relatorios/NotasDetalhadas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotasDetalhadas.gerarRelatorio("Relatorios/NotasDetalhadas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
