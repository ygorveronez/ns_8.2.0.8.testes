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
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Servico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Titulo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusVendaDireta.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusPedidoVendaDireta.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCobrancaVendaDireta.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumProdutoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioVendaDireta, _gridVendaDireta, _pesquisaVendaDireta, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaVendaDireta = function () {
    this.DataVendaInicial = PropertyEntity({ text: "Data Venda Inicial:", getType: typesKnockout.date });
    this.DataVendaFinal = PropertyEntity({ text: "Data Venda Final:", getType: typesKnockout.date });
    this.DataFinalizacaoInicial = PropertyEntity({ text: "Data Finalização Inicial:", getType: typesKnockout.date });
    this.DataFinalizacaoFinal = PropertyEntity({ text: "Data Finalização Final:", getType: typesKnockout.date });
    this.DataAgendamentoInicial = PropertyEntity({ text: "Data Agendamento Inicial:", getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ text: "Data Agendamento Final:", getType: typesKnockout.date });

    this.DataVencimentoCertificadoInicial = PropertyEntity({ text: "Vnct. Cert. Inicial:", getType: typesKnockout.date });
    this.DataVencimentoCertificadoFinal = PropertyEntity({ text: "Vnct. Cert. Final:", getType: typesKnockout.date });
    this.DataVencimentoCobrancaInicial = PropertyEntity({ text: "Vnct. Cobrança Inicial:", getType: typesKnockout.date });
    this.DataVencimentoCobrancaFinal = PropertyEntity({ text: "Vnct. Cobrança Final:", getType: typesKnockout.date });
    this.TipoCobrancaVendaDireta = PropertyEntity({ text: "Tipo de Cobrança da Venda:", val: ko.observable(0), options: EnumTipoCobrancaVendaDireta.obterOpcoesPesquisa(), def: EnumTipoCobrancaVendaDireta.Todos, enable: ko.observable(true) });

    this.NumeroPedido = PropertyEntity({ text: "Número Pedido:" });
    this.NumeroBoleto = PropertyEntity({ text: "Número Boleto:" });
    this.ProdutoServico = PropertyEntity({ text: "Tipo:", val: ko.observable(EnumProdutoServico.Todos), options: EnumProdutoServico.obterOpcoesPesquisa(), def: EnumProdutoServico.Todos, visible: ko.observable(true) });

    this.StatusVenda = PropertyEntity({ text: "Status Venda:", val: ko.observable(EnumStatusVendaDireta.Todos), options: EnumStatusVendaDireta.obterOpcoesPesquisa(), def: EnumStatusVendaDireta.Todos });
    this.StatusPedido = PropertyEntity({ text: "Status Pedido:", val: ko.observable(EnumStatusPedidoVendaDireta.Todos), options: EnumStatusPedidoVendaDireta.obterOpcoesPesquisa(), def: EnumStatusPedidoVendaDireta.Todos });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid() });
    this.Agendador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Agendador:", idBtnSearch: guid() });
    this.Validador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Validador:", idBtnSearch: guid() });    
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid() });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVendaDireta.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioVendaDireta() {

    _pesquisaVendaDireta = new PesquisaVendaDireta();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVendaDireta = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/VendaDireta/Pesquisa", _pesquisaVendaDireta, null, null, 10);
    _gridVendaDireta.SetPermitirEdicaoColunas(true);

    _relatorioVendaDireta = new RelatorioGlobal("Relatorios/VendaDireta/BuscarDadosRelatorio", _gridVendaDireta, function () {
        _relatorioVendaDireta.loadRelatorio(function () {
            KoBindings(_pesquisaVendaDireta, "knockoutPesquisaVendaDireta");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVendaDireta");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVendaDireta");

            new BuscarFuncionario(_pesquisaVendaDireta.Funcionario);
            new BuscarFuncionario(_pesquisaVendaDireta.Agendador);    
            new BuscarFuncionario(_pesquisaVendaDireta.Validador);                
            new BuscarClientes(_pesquisaVendaDireta.Pessoa);
            new BuscarGruposPessoas(_pesquisaVendaDireta.GrupoPessoas);
            new BuscarProdutoTMS(_pesquisaVendaDireta.Produto);
            new BuscarServicoTMS(_pesquisaVendaDireta.Servico);
            new BuscarTitulo(_pesquisaVendaDireta.Titulo, null, null, RetornoTitulo);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVendaDireta);
}

function RetornoTitulo(data) {
    _pesquisaVendaDireta.Titulo.val(data.Codigo);
    _pesquisaVendaDireta.Titulo.codEntity(data.Codigo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioVendaDireta.gerarRelatorio("Relatorios/VendaDireta/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioVendaDireta.gerarRelatorio("Relatorios/VendaDireta/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}