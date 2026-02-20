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
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/NaturezaOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Servico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Empresa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CategoriaPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Equipamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoDocumentoEntrada.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusFinanceiro.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNotas, _gridNotas, _pesquisaNotas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusFinanceiroPesquisa = [
    { text: "Todos", value: EnumStatusFinanceiro.Todos },
    { text: "Em Aberto", value: EnumStatusFinanceiro.Aberto },
    { text: "Quitado", value: EnumStatusFinanceiro.Quitado },
    { text: "Renegociado", value: EnumStatusFinanceiro.Renegociado }
];

var PesquisaNotas = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Serie = PropertyEntity({ text: "Série: ", getType: typesKnockout.int });
    this.Chave = PropertyEntity({ text: "Chave: ", getType: typesKnockout.string, maxlength: 44 });
    this.DataEntradaInicial = PropertyEntity({ text: "Entrada Inicial: ", getType: typesKnockout.date });
    this.DataEntradaFinal = PropertyEntity({ text: "Entrada Final: ", getType: typesKnockout.date });

    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid(), visible: true });
    this.NaturezaOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Natureza da Operação: ", idBtnSearch: guid(), visible: true });
    this.EstadoEmitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "UF Emitente: ", idBtnSearch: guid(), visible: true });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto: ", idBtnSearch: guid(), visible: false });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço: ", idBtnSearch: guid(), visible: false });
    this.Modelo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo: ", idBtnSearch: guid(), visible: false });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0 });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento Veículo: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.StatusNotaEntrada = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoEntrada.Todos), options: EnumSituacaoDocumentoEntrada.obterOpcoesPesquisa(), def: EnumSituacaoDocumentoEntrada.Todos, text: "Status: ", visible: ko.observable(true) });
    this.SituacaoFinanceiraNotaEntrada = PropertyEntity({ val: ko.observable(0), options: _statusFinanceiroPesquisa, def: 0, text: "Situação Financeira: ", visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "Tipo Movimento: " });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro Resultado:", idBtnSearch: guid(), issue: 0 });
    this.OperadorLancamentoDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Lançamento Documento:", idBtnSearch: guid(), issue: 0 });
    this.OperadorFinalizaDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Finalizou Documento:", idBtnSearch: guid(), issue: 0 });

    this.DataInicialFinalizacao = PropertyEntity({ getType: typesKnockout.date, text: "Data Inicial Finalização:"});
    this.DataFinalFinalizacao = PropertyEntity({ getType: typesKnockout.date, text: "Data Final Finalização:"});

    this.DocFinalizadoAutomaticamente = PropertyEntity({ text: "Nota Finalizada Automaticamente?", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });

    this.Categoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Categoria:", idBtnSearch: guid()});

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridNotas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNotas.Visible.visibleFade()) {
                _pesquisaNotas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNotas.Visible.visibleFade(true);
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

function loadRelatorioNotas() {

    _pesquisaNotas = new PesquisaNotas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNotas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Notas/Pesquisa", _pesquisaNotas, null, null, 10);
    _gridNotas.SetPermitirEdicaoColunas(true);

    _relatorioNotas = new RelatorioGlobal("Relatorios/Notas/BuscarDadosRelatorio", _gridNotas, function () {
        _relatorioNotas.loadRelatorio(function () {
            KoBindings(_pesquisaNotas, "knockoutPesquisaNotas");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNotas");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNotas");

            new BuscarNaturezasOperacoesNotaFiscal(_pesquisaNotas.NaturezaOperacao);
            new BuscarClientes(_pesquisaNotas.Pessoa);
            new BuscarProdutoTMS(_pesquisaNotas.Produto);
            new BuscarServicoTMS(_pesquisaNotas.Servico);
            new BuscarModeloDocumentoFiscal(_pesquisaNotas.Modelo);
            new BuscarEmpresa(_pesquisaNotas.Empresa);
            new BuscarEstados(_pesquisaNotas.EstadoEmitente);
            new BuscarVeiculos(_pesquisaNotas.Veiculo);
            new BuscarGruposProdutosTMS(_pesquisaNotas.GrupoProduto, null);
            new BuscarSegmentoVeiculo(_pesquisaNotas.Segmento);
            new BuscarModeloDocumentoFiscal(_pesquisaNotas.ModeloDocumentoFiscal);
            new BuscarEquipamentos(_pesquisaNotas.Equipamento);
            new BuscarCentroResultado(_pesquisaNotas.CentroResultado);
            new BuscarFuncionario(_pesquisaNotas.OperadorLancamentoDocumento);
            new BuscarFuncionario(_pesquisaNotas.OperadorFinalizaDocumento);
            new BuscarCategoriaPessoa(_pesquisaNotas.Categoria);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaNotas.Empresa.visible(false);
                _pesquisaNotas.StatusNotaEntrada.visible(false);
                _pesquisaNotas.SituacaoFinanceiraNotaEntrada.visible(false);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNotas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotas.gerarRelatorio("Relatorios/Notas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotas.gerarRelatorio("Relatorios/Notas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
