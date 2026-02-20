/// <reference path="../../../../../ViewsScripts/Consultas/MarcaVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoServico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumPrioridadeOrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemServico, _pesquisaOrdemServico, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioOrdemServico;

var PesquisaOrdemServico = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), text: "Situação:", visible: ko.observable(true) });
    this.TipoOrdemServico = PropertyEntity({ val: ko.observable(""), options: EnumTipoOficina.ObterOpcoesPesquisa(), text: "Tipo Ordem Serviço:", visible: ko.observable(true) });
    this.TipoManutencao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, options: EnumTipoManutencaoOrdemServicoFrota.ObterOpcoes(), text: "Tipo de Manutenção:", visible: ko.observable(true) });
    this.NumeroInicial = PropertyEntity({ text: "Nº Inicial:", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Nº Final:", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Local de Manutenção:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Servico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Marca Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoServico = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadePessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cidade Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.UFPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OperadorLancamentoDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Lançamento Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OperadorFinalizaDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador Finalizou Documento:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataInicialInclusao = PropertyEntity({ text: "Data Ini. Inclusão:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalInclusao = PropertyEntity({ text: "Data Fin. Inclusão:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialInclusao.dateRangeLimit = this.DataFinalInclusao;
    this.DataFinalInclusao.dateRangeInit = this.DataInicialInclusao;
    this.Mecanicos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mecânico:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicialLimiteExecucao = PropertyEntity({ text: "Data Inicial Limite Execução:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalLimiteExecucao = PropertyEntity({ text: "Data Final Limite Execução:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialLimiteExecucao.dataRangeLimit = this.DataFinalLimiteExecucao;
    this.DataFinalLimiteExecucao.dataRangeInit = this.DataInicialLimiteExecucao;
    this.Prioridade = PropertyEntity({ text: "Prioridade: ", val: ko.observable(""), options: EnumPrioridadeOrdemServico.obterOpcoesPesquisa() });

    this.DataLiberacaoInicio = PropertyEntity({ text: "Data Inicial Liberação:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataLiberacaoFim = PropertyEntity({ text: "Data Final Liberação:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.DataFechamentoInicio = PropertyEntity({ text: "Data Inicial Fechamento:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFechamentoFim = PropertyEntity({ text: "Data Final Fechamento:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.DataReaberturaInicio = PropertyEntity({ text: "Data Inicial Reabertura:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataReaberturaFim = PropertyEntity({ text: "Data Final Reabertura:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.GrupoProdutoTMS = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ProdutoTMS = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOrdemServico.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOrdemServico.Visible.visibleFade() == true) {
                _pesquisaOrdemServico.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOrdemServico.Visible.visibleFade(true);
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

function LoadOrdemServico() {
    _pesquisaOrdemServico = new PesquisaOrdemServico();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridOrdemServico = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/OrdemServico/Pesquisa", _pesquisaOrdemServico);

    _gridOrdemServico.SetPermitirEdicaoColunas(true);
    _gridOrdemServico.SetQuantidadeLinhasPorPagina(10);

    _relatorioOrdemServico = new RelatorioGlobal("Relatorios/OrdemServico/BuscarDadosRelatorio", _gridOrdemServico, function () {
        _relatorioOrdemServico.loadRelatorio(function () {
            KoBindings(_pesquisaOrdemServico, "knockoutPesquisaOrdemServico", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOrdemServico", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOrdemServico", false);

            new BuscarClientes(_pesquisaOrdemServico.LocalManutencao);
            new BuscarVeiculos(_pesquisaOrdemServico.Veiculo);
            new BuscarMotoristas(_pesquisaOrdemServico.Motorista);
            new BuscarTipoOrdemServico(_pesquisaOrdemServico.Tipo);
            new BuscarServicoVeiculo(_pesquisaOrdemServico.Servico);
            new BuscarEquipamentos(_pesquisaOrdemServico.Equipamento);
            new BuscarMarcasVeiculo(_pesquisaOrdemServico.MarcaVeiculo);
            new BuscarModelosVeiculo(_pesquisaOrdemServico.ModeloVeiculo);
            new BuscarGrupoServico(_pesquisaOrdemServico.GrupoServico);
            new BuscarCentroResultado(_pesquisaOrdemServico.CentroResultado);
            new BuscarSegmentoVeiculo(_pesquisaOrdemServico.Segmento);
            new BuscarLocalidades(_pesquisaOrdemServico.CidadePessoa);
            new BuscarEstados(_pesquisaOrdemServico.UFPessoa);
            new BuscarFuncionario(_pesquisaOrdemServico.OperadorLancamentoDocumento);
            new BuscarFuncionario(_pesquisaOrdemServico.OperadorFinalizaDocumento);
            new BuscarFuncionario(_pesquisaOrdemServico.Mecanicos);
            new BuscarGruposProdutosTMS(_pesquisaOrdemServico.GrupoProdutoTMS);
            new BuscarProdutoTMS(_pesquisaOrdemServico.ProdutoTMS);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOrdemServico);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioOrdemServico.gerarRelatorio("Relatorios/OrdemServico/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioOrdemServico.gerarRelatorio("Relatorios/OrdemServico/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}