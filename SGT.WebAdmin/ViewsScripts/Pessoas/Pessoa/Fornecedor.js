/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoOficina.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="Pessoa.js" />

var _fornecedor;
var _gridAnexoFornecedor;
var _gridTabelaValores, _gridTabelaMultiplosVencimentos;
var _gridFornecedorDestinatarios, _gridFornecedorTipoOperacoes, _gridFornecedorTransportadores, _gridFornecedorTipoCargas, _gridFornecedorModelosVeicular, _gridFornecedorRestricaoModelosVeicular;

var TabelaValorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Produto = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.CodigoProduto = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.UnidadeDeMedida = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoUnidadeDeMedida = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorFixo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorAte = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ type: types.map, val: ko.observable("") });    
    this.PercentualDesconto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataInicial = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorMoedaCotacao = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var TabelaMultiplosVencimentosMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Vencimento = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DataEmissao = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DiaEmissaoInicial = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DiaEmissaoFinal = PropertyEntity({ type: types.entity, val: ko.observable("") });
};

var _parcelas = [
    { value: 1, text: "1X" },
    { value: 2, text: "2X" },
    { value: 3, text: "3X" },
    { value: 4, text: "4X" },
    { value: 5, text: "5X" },
    { value: 6, text: "6X" },
    { value: 7, text: "7X" },
    { value: 8, text: "8X" },
    { value: 9, text: "9X" },
    { value: 10, text: "10X" },
    { value: 11, text: "11X" },
    { value: 12, text: "12X" },
    { value: 13, text: "13X" },
    { value: 14, text: "14X" },
    { value: 15, text: "15X" },
    { value: 16, text: "16X" },
    { value: 17, text: "17X" },
    { value: 18, text: "18X" },
    { value: 19, text: "19X" },
    { value: 20, text: "20X" },
    { value: 21, text: "21X" },
    { value: 22, text: "22X" },
    { value: 23, text: "23X" },
    { value: 24, text: "24X" },
    { value: 25, text: "25X" },
    { value: 26, text: "26X" },
    { value: 27, text: "27X" },
    { value: 28, text: "28X" },
    { value: 29, text: "29X" },
    { value: 30, text: "30X" },
    { value: 31, text: "31X" },
    { value: 32, text: "32X" },
    { value: 33, text: "33X" },
    { value: 34, text: "34X" },
    { value: 35, text: "35X" },
    { value: 36, text: "36X" },
    { value: 37, text: "37X" },
    { value: 38, text: "38X" },
    { value: 39, text: "39X" },
    { value: 40, text: "40X" },
    { value: 41, text: "41X" },
    { value: 42, text: "42X" },
    { value: 43, text: "43X" },
    { value: 44, text: "44X" },
    { value: 45, text: "45X" },
    { value: 46, text: "46X" },
    { value: 47, text: "47X" },
    { value: 48, text: "48X" },
    { value: 49, text: "49X" },
    { value: 50, text: "50X" }
];

//*******MAPEAMENTO KNOUCKOUT*******

var Fornecedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoFornecedor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PagoPorFatura = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.FornecedorPagoPorFatura, def: false, visible: ko.observable(true) });
    this.CodigoDocumentoFornecedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoDocumentacaoDoFornecedor.getFieldDescription(), required: ko.observable(false), maxlength: 50, visible: ko.observable(false), enable: ko.observable(true) });
    this.FormaTituloFornecedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FormaDoTitulo.getFieldDescription(), val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), def: EnumFormaTitulo.Outros, visible: ko.observable(true) });

    this.GerarDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.GerarDuplicataAutomaticamenteNaNotaDeEntrada, def: false, visible: ko.observable(true) });
    this.ParcelasDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: Localization.Resources.Pessoas.Pessoa.QuantidadeParcelas.getRequiredFieldDescription(), required: false, visible: ko.observable(true) });
    this.IntervaloDiasDuplicataNotaEntrada = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IntervaloDeDiasExParaIntervalosDiferentes, required: false, maxlength: 500, getType: typesKnockout.string, visible: ko.observable(true) });
    this.DiaPadraoDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Pessoas.Pessoa.DiaVencimento.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 2 });
    this.IgnorarDuplicataRecebidaXMLNotaEntrada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.IgnorarDuplicatasRecebidasDoXML, def: false, visible: ko.observable(true) });
    this.CodigoIntegracaoDuplicataNotaEntrada = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoIntegracao, required: false, maxlength: 100, getType: typesKnockout.string, visible: ko.observable(true) });

    this.SempreConsiderarValorOrcadoFechamentoOrdemServico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.SempreConsiderarCalorOrcadoNoFechamentoDaOrdemDeServico, def: false, visible: ko.observable(true) });
    this.Oficina = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.UmaOficina, def: false, visible: ko.observable(true) });
    this.TipoOficina = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDeOficina.getRequiredFieldDescription(), options: EnumTipoOficina.ObterOpcoes(), val: ko.observable(EnumTipoOficina.Interna), def: EnumTipoOficina.Interna, issue: 0, visible: ko.observable(true) });
    this.EmpresaOficina = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.EmpresaOficina.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PostoConveniado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.PostoConveniado, def: false, issue: 171, visible: ko.observable(true) });
    this.MultiplosVencimentos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.MultiplosVencimentosEmDataFixa, def: false, visible: ko.observable(true) });
    this.TipoMovimentoTituloPagar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.TipoMovimentoParaTitulosPagar.getFieldDescription(), visible: ko.observable(false), idBtnSearch: guid() });

    this.TabelaValores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid(), issue: 172, visible: ko.observable(true) });
    this.TabelaMultiplosVencimentos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid(), visible: ko.observable(true) });

    this.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento, def: false, visible: ko.observable(true) });
    this.GerarAgendamentoSomentePedidosExistentes = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.GerarAgendamentoSomentePedidosExistentes, def: false, visible: ko.observable(true) });

    //Posto Conveniado
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: "", text: Localization.Resources.Pessoas.Pessoa.Produto.getRequiredFieldDescription(), idBtnSearch: guid(), required: false, issue: 140, visible: ko.observable(true) });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(5), options: EnumUnidadeMedidaFornecedor.obterOpcoes(), text: Localization.Resources.Pessoas.Pessoa.UnidadeDeMedida.getRequiredFieldDescription(), def: 5, required: false, codEntity: ko.observable(0), issue: 88, visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoIntegracao.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 50, val: ko.observable(""), issue: 15, visible: ko.observable(true) });    

    this.ValorFixo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorFixo.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 9, configDecimal: { precision: 4, allowZero: false }, visible: ko.observable(true) });
    this.ValorAte = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorAte.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 9, configDecimal: { precision: 4, allowZero: false }, visible: ko.observable(true) });

    this.PercentualDesconto = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PorcentagemDesconto, required: false, getType: typesKnockout.decimal, maxlength: 6, visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataInicial.getFieldDescription(), required: false, getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataFinal.getFieldDescription(), required: false, getType: typesKnockout.dateTime, visible: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: Localization.Resources.Pessoas.Pessoa.Moeda.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ValorMoedaCotacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorMoeda.getFieldDescription(), required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorOriginalMoeda.getFieldDescription(), required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    //Múltiplos vencimentos em data fixa?
    this.DiaEmissaoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiaInicial.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false), maxlength: 2 });
    this.DiaEmissaoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DiaFinal.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: ko.observable(false), maxlength: 2 });
    this.Vencimento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Pessoas.Pessoa.Vencimento.getRequiredFieldDescription(), required: ko.observable(false), visible: ko.observable(true), maxlength: 2 });

    this.TipoOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarTipoOperacaoClick, val: ko.observable(new Array()), idGrid: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarTransportadorClick, val: ko.observable(new Array()), idGrid: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarTipoCargaClick, val: ko.observable(new Array()), idGrid: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.TipoDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.ModelosVeicular = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarModeloVeicularClick, val: ko.observable(new Array()), idGrid: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.ModeloVeicularDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.PermiteDownloadDocumentos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.PermiteDownloadDeDocumentos, def: false });
    this.Destinatarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarDestinatarioClick, val: ko.observable(new Array()), idGrid: guid() });

    // Restrição modelos veiculares
    this.RestricaoModelosVeicular = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), eventClick: adicionarRestricaoModeloVeicularClick, val: ko.observable(new Array()), idGrid: guid() });
    this.RestricaoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.ModeloVeicularDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.RestricaoTipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid() });

    this.TipoOperacoes.val.subscribe(recarregarGridTipoOperacao);
    this.Transportadores.val.subscribe(recarregarGridTransportador);
    this.TipoCargas.val.subscribe(recarregarGridTipoCarga);
    this.ModelosVeicular.val.subscribe(recarregarGridModeloVeicular);
    this.RestricaoModelosVeicular.val.subscribe(recarregarGridRestricaoModeloVeicular);
    this.Destinatarios.val.subscribe(recarregarGridDestinatario);

    this.AvisoFornecedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TextoDoAviso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 1000 });
    this.DescricaoArquivo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos.getFieldDescription(), type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Arquivo.val.subscribe(function (novoValor) { _fornecedor.NomeArquivo.val(novoValor.replace('C:\\fakepath\\', '')); });
    this.Anexos.val.subscribe(recarregarGridAnexoFornecedor);

    this.AdicionarArquivo = PropertyEntity({ eventClick: adicionarAnexoFornecedorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.EnviarEmailFornecedorDadosTransporte = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.NotificarFornecedorQuandoOsDadosDeTransporteDaCargaForemInformados, val: ko.observable(false) });
    this.NaoEObrigatorioInformarNfeNaColeta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.NaoEObrigatorioInformarNfeNaColeta, val: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTabelaValorClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.AdicionarSalvarValores, visible: ko.observable(true) });
    this.AdicionarVencimento = PropertyEntity({ eventClick: adicionarTabelaMultiplosVencimentosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTabelaMultiplosVencimentosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.MultiploVencimentoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Pessoas.Pessoa.Destinatario.getFieldDescription(), idBtnSearch: guid() });
};

//*******EVENTOS*******

function loadFornecedor() {
    _fornecedor = new Fornecedor();
    KoBindings(_fornecedor, "knockoutCadastroFornecedor");

    new BuscarProdutoTMS(_fornecedor.Produto, retornoSelecaoProduto);
    new BuscarTipoMovimento(_fornecedor.TipoMovimentoTituloPagar);
    new BuscarTiposOperacao(_fornecedor.TipoOperacao);
    new BuscarTransportadores(_fornecedor.Transportador);
    new BuscarTiposdeCarga(_fornecedor.TipoCarga);
    new BuscarModelosVeicularesCarga(_fornecedor.ModeloVeicular);
    new BuscarModelosVeicularesCarga(_fornecedor.RestricaoModeloVeicular);
    new BuscarTiposOperacao(_fornecedor.RestricaoTipoOperacao);
    new BuscarTransportadores(_fornecedor.EmpresaOficina);
    new BuscarClientes(_fornecedor.Destinatario, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirTabelaValorClick, tamanho: "15", icone: "" };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarTabelaValorClick, tamanho: "15", icone: "" };
    var auditoriaPreco = { descricao: Localization.Resources.Pessoas.Pessoa.Auditar, id: guid(), metodo: OpcaoAuditoria("PostoCombustivelTabelaValores"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [editar, excluir, auditoriaPreco] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoUnidadeDeMedida", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "Produto", title: Localization.Resources.Pessoas.Pessoa.Produto, width: "20%", className: "text-align-left" },
        { data: "UnidadeDeMedida", title: Localization.Resources.Pessoas.Pessoa.Unidade, width: "10%", className: "text-align-left" },
        { data: "CodigoIntegracao", title: Localization.Resources.Pessoas.Pessoa.CodigoIntegracao, width: "10%", className: "text-align-left" },
        { data: "ValorFixo", title: Localization.Resources.Pessoas.Pessoa.Valor, width: "10%", className: "text-align-rigth" },
        { data: "ValorAte", title: Localization.Resources.Pessoas.Pessoa.ValorAte, width: "10%", className: "text-align-rigth" },
        { data: "PercentualDesconto", title: Localization.Resources.Pessoas.Pessoa.PorcentagemDesconto, width: "10%", className: "text-align-rigth" },
        { data: "DataInicial", title: Localization.Resources.Gerais.Geral.Inicio, width: "15%", className: "text-align-center" },
        { data: "DataFinal", title: Localization.Resources.Gerais.Geral.Fim, width: "15%", className: "text-align-center" }
    ];
    _gridTabelaValores = new BasicDataTable(_fornecedor.TabelaValores.idGrid, header, menuOpcoes);
    recarregarGridTabelaValores();

    loadTabelaMultiplosVencimentos();
    loadTabelaAnexoFornecedor();
    loadFornecedorTipoOperacoes();
    loadFornecedorTransportadores();
    loadFornecedorTipoCargas();
    loadFornecedorModelosVeicular();
    loadFornecedorRestricaoModelosVeicular();
    loadFornecedorDestinatario();

    configurarLayoutFornecedorPorTipoSistema();
}

function configurarLayoutFornecedorPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        $(".liFornecedorDados").show().find('a').click();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _fornecedor.TipoMovimentoTituloPagar.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _fornecedor.CodigoDocumentoFornecedor.visible(true);
        _fornecedor.EmpresaOficina.visible(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _fornecedor.MoedaCotacaoBancoCentral.visible(true);
        _fornecedor.ValorMoedaCotacao.visible(true);
        _fornecedor.ValorOriginalMoedaEstrangeira.visible(true);
    }
}

function loadFornecedorTipoOperacoes() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirTipoOperacaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%", className: "text-align-left" },
    ];
    _gridFornecedorTipoOperacoes = new BasicDataTable(_fornecedor.TipoOperacoes.idGrid, header, menuOpcoes);
    _gridFornecedorTipoOperacoes.CarregarGrid([]);
}

function loadFornecedorTransportadores() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirTransportadorClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricoo, width: "80%", className: "text-align-left" },
    ];
    _gridFornecedorTransportadores = new BasicDataTable(_fornecedor.Transportadores.idGrid, header, menuOpcoes);
    _gridFornecedorTransportadores.CarregarGrid([]);
}

function loadFornecedorTipoCargas() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirTipoCargaClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%", className: "text-align-left" },
    ];
    _gridFornecedorTipoCargas = new BasicDataTable(_fornecedor.TipoCargas.idGrid, header, menuOpcoes);
    _gridFornecedorTipoCargas.CarregarGrid([]);
}

function loadFornecedorModelosVeicular() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirModeloVeicularClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%", className: "text-align-left" },
    ];
    _gridFornecedorModelosVeicular = new BasicDataTable(_fornecedor.ModelosVeicular.idGrid, header, menuOpcoes);
    _gridFornecedorModelosVeicular.CarregarGrid([]);
}

function loadFornecedorRestricaoModelosVeicular() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirRestricaoModeloVeicularClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Pessoas.Pessoa.ModeloVeicularDeCarga, width: "40%", className: "text-align-left" },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "DescricaoTipoOperacao", title: Localization.Resources.Pessoas.Pessoa.TipoDeOperacao, width: "40%", className: "text-align-left" },

    ];
    _gridFornecedorRestricaoModelosVeicular = new BasicDataTable(_fornecedor.RestricaoModelosVeicular.idGrid, header, menuOpcoes);
    _gridFornecedorRestricaoModelosVeicular.CarregarGrid([]);
}

function loadFornecedorDestinatario() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirDestinatarioClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [excluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%", className: "text-align-left" },
    ];
    _gridFornecedorDestinatarios = new BasicDataTable(_fornecedor.Destinatarios.idGrid, header, menuOpcoes);
    _gridFornecedorDestinatarios.CarregarGrid([]);
}

function recarregarGridTipoOperacao() {
    var dados = _fornecedor.TipoOperacoes.val().slice();
    _gridFornecedorTipoOperacoes.CarregarGrid(dados);
}

function recarregarGridTransportador() {
    var dados = _fornecedor.Transportadores.val().slice();
    _gridFornecedorTransportadores.CarregarGrid(dados);
}

function recarregarGridTipoCarga() {
    var dados = _fornecedor.TipoCargas.val().slice();
    _gridFornecedorTipoCargas.CarregarGrid(dados);
}

function recarregarGridModeloVeicular() {
    var dados = _fornecedor.ModelosVeicular.val().slice();
    _gridFornecedorModelosVeicular.CarregarGrid(dados);
}

function recarregarGridRestricaoModeloVeicular() {
    var dados = _fornecedor.RestricaoModelosVeicular.val().slice();
    _gridFornecedorRestricaoModelosVeicular.CarregarGrid(dados);
}

function recarregarGridDestinatario() {
    var dados = _fornecedor.Destinatarios.val().slice();
    _gridFornecedorDestinatarios.CarregarGrid(dados);
}

function adicionarTipoOperacaoClick() {
    var codigo = _fornecedor.TipoOperacao.codEntity();
    if (codigo <= 0)
        return;

    var dados = _fornecedor.TipoOperacoes.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == codigo)
            return;
    }

    dados.push({
        Codigo: codigo,
        Descricao: _fornecedor.TipoOperacao.val()
    });

    _fornecedor.TipoOperacao.codEntity(_fornecedor.TipoOperacao.defCodEntity);
    _fornecedor.TipoOperacao.val(_fornecedor.TipoOperacao.def);

    _fornecedor.TipoOperacoes.val(dados);
}

function adicionarTransportadorClick() {
    var codigo = _fornecedor.Transportador.codEntity();
    if (codigo <= 0)
        return;

    var dados = _fornecedor.Transportadores.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == codigo)
            return;
    }

    dados.push({
        Codigo: codigo,
        Descricao: _fornecedor.Transportador.val()
    });

    _fornecedor.Transportador.codEntity(_fornecedor.Transportador.defCodEntity);
    _fornecedor.Transportador.val(_fornecedor.Transportador.def);

    _fornecedor.Transportadores.val(dados);
}

function adicionarTipoCargaClick() {
    var codigo = _fornecedor.TipoCarga.codEntity();
    if (codigo <= 0)
        return;

    var dados = _fornecedor.TipoCargas.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == codigo)
            return;
    }

    dados.push({
        Codigo: codigo,
        Descricao: _fornecedor.TipoCarga.val()
    });

    _fornecedor.TipoCarga.codEntity(_fornecedor.TipoCarga.defCodEntity);
    _fornecedor.TipoCarga.val(_fornecedor.TipoCarga.def);

    _fornecedor.TipoCargas.val(dados);
}

function adicionarModeloVeicularClick() {
    var codigo = _fornecedor.ModeloVeicular.codEntity();
    if (codigo <= 0)
        return;

    var dados = _fornecedor.ModelosVeicular.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == codigo)
            return;
    }

    dados.push({
        Codigo: codigo,
        Descricao: _fornecedor.ModeloVeicular.val()
    });

    _fornecedor.ModeloVeicular.codEntity(_fornecedor.ModeloVeicular.defCodEntity);
    _fornecedor.ModeloVeicular.val(_fornecedor.ModeloVeicular.def);

    _fornecedor.ModelosVeicular.val(dados);
}

function adicionarRestricaoModeloVeicularClick() {

    var codigoModeloVeicular = _fornecedor.RestricaoModeloVeicular.codEntity();
    if (codigoModeloVeicular <= 0)
        return;

    var codigoTipoOperacao = _fornecedor.RestricaoTipoOperacao.codEntity();

    var dados = _fornecedor.RestricaoModelosVeicular.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].CodigoModeloVeicular == codigoModeloVeicular && dados[i].CodigoTipoOperacao == codigoTipoOperacao)
            return;
    }

    dados.push({
        Codigo: 0,
        CodigoModeloVeicular: codigoModeloVeicular,
        DescricaoModeloVeicular: _fornecedor.RestricaoModeloVeicular.val(),
        CodigoTipoOperacao: codigoTipoOperacao,
        DescricaoTipoOperacao: _fornecedor.RestricaoTipoOperacao.val()
    });

    _fornecedor.RestricaoModeloVeicular.codEntity(_fornecedor.RestricaoModeloVeicular.defCodEntity);
    _fornecedor.RestricaoModeloVeicular.val(_fornecedor.RestricaoModeloVeicular.def);

    _fornecedor.RestricaoTipoOperacao.codEntity(_fornecedor.RestricaoTipoOperacao.defCodEntity);
    _fornecedor.RestricaoTipoOperacao.val(_fornecedor.RestricaoTipoOperacao.def);

    _fornecedor.RestricaoModelosVeicular.val(dados);
}

function adicionarDestinatarioClick() {
    var codigo = _fornecedor.Destinatario.codEntity();
    if (codigo <= 0)
        return;

    var dados = _fornecedor.Destinatarios.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == codigo)
            return;
    }

    dados.push({
        Codigo: codigo,
        Descricao: _fornecedor.Destinatario.val()
    });

    _fornecedor.Destinatario.codEntity(_fornecedor.Destinatario.defCodEntity);
    _fornecedor.Destinatario.val(_fornecedor.Destinatario.def);

    _fornecedor.Destinatarios.val(dados);
}

function excluirTipoOperacaoClick(dado) {
    var dados = _fornecedor.TipoOperacoes.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.TipoOperacoes.val(dados);
}

function excluirTransportadorClick(dado) {
    var dados = _fornecedor.Transportadores.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.Transportadores.val(dados);
}

function excluirTipoCargaClick(dado) {
    var dados = _fornecedor.TipoCargas.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.TipoCargas.val(dados);
}

function excluirModeloVeicularClick(dado) {
    var dados = _fornecedor.ModelosVeicular.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.ModelosVeicular.val(dados);
}

function excluirRestricaoModeloVeicularClick(dado) {
    var dados = _fornecedor.RestricaoModelosVeicular.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.RestricaoModelosVeicular.val(dados);
}

function excluirDestinatarioClick(dado) {
    var dados = _fornecedor.Destinatarios.val().slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Codigo == dado.Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    _fornecedor.Destinatarios.val(dados);
}

function PreencherDadosFornecedor(dados) {
    PreencherObjetoKnout(_fornecedor, { Data: dados });
}

function LimparCamposFornecedor() {
    LimparCampos(_fornecedor);
    _fornecedor.TipoOperacoes.val([]);
    _fornecedor.Transportadores.val([]);
    $("a[href='#tabFornecedorAgendamentoTransportador']").click();
}

function adicionarAnexoFornecedorClick() {
    var arquivo = document.getElementById(_fornecedor.Arquivo.id);

    if (arquivo.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Anexos, Localization.Resources.Pessoas.Pessoa.NenhumArquivoSelecionado);

    var anexo = {
        Codigo: guid(),
        Descricao: _fornecedor.DescricaoArquivo.val(),
        NomeArquivo: _fornecedor.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    enviarAnexosFornecedor(_fornecedor.CodigoFornecedor.val(), [anexo]);

    _fornecedor.DescricaoArquivo.val(_fornecedor.DescricaoArquivo.def);
    _fornecedor.NomeArquivo.val(_fornecedor.NomeArquivo.def);

    arquivo.value = null;
}

function retornoSelecaoProduto(data) {
    _fornecedor.UnidadeMedida.val(data.CodigoUnidadeMedida);
    _fornecedor.Produto.val(data.Descricao);
    _fornecedor.Produto.codEntity(data.Codigo);
}

function enviarArquivoAvisoClick() {
    if (_fornecedor.CodigoFornecedor.val() <= 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.ClienteNaoCadastrado, Localization.Resources.Pessoas.Pessoa.AntesDeEnviarUmAnexoDeAvisoParaFornecedorCadastreCliente);
        return;
    }

    _fornecedor.AdicionarArquivo.visible(false);
    _fornecedor.DownloadArquivo.visible(true);
}

function adicionarTabelaValorClick() {
    _fornecedor.Produto.requiredClass("form-control");
    _fornecedor.ValorFixo.requiredClass("form-control");
    _fornecedor.ValorAte.requiredClass("form-control");
    _fornecedor.PercentualDesconto.requiredClass("form-control");

    var tudoCerto = true;
    tudoCerto = (tudoCerto && _fornecedor.Produto.codEntity() > 0);
    if (tudoCerto) {
        var existe = false;

        if (!existe) {

            if (_fornecedor.Codigo.val() > 0 || _fornecedor.Codigo.val() != "") {
                var listaAtualizada = new Array();
                $.each(_fornecedor.TabelaValores.list, function (i, Tabela) {
                    if (Tabela.Codigo.val != _fornecedor.Codigo.val()) {
                        listaAtualizada.push(Tabela);
                    }
                });
                _fornecedor.TabelaValores.list = listaAtualizada;
            }
            _fornecedor.Codigo.val(0);
            var obj = new TabelaValorMap();
            obj.Codigo.val = guid();
            obj.Produto.val = _fornecedor.Produto.val();
            obj.CodigoProduto.val = _fornecedor.Produto.codEntity();
            //obj.UnidadeDeMedida.val = _UnidadeMedida[_fornecedor.UnidadeMedida.val() - 1].text;
            obj.UnidadeDeMedida.val = EnumUnidadeMedidaFornecedor.obterDescricao(_fornecedor.UnidadeMedida.val());
            obj.CodigoUnidadeDeMedida.val = _fornecedor.UnidadeMedida.val();
            obj.DataInicial.val = _fornecedor.DataInicial.val();
            obj.DataFinal.val = _fornecedor.DataFinal.val();
            if (_fornecedor.ValorFixo.val() == "") {
                _fornecedor.ValorFixo.val(0);
                obj.ValorFixo.val = mvalor(_fornecedor.ValorFixo.val().toFixed(2).toString());
            } else
                obj.ValorFixo.val = _fornecedor.ValorFixo.val();
            if (_fornecedor.ValorAte.val() == "") {
                _fornecedor.ValorAte.val(0);
                obj.ValorAte.val = mvalor(_fornecedor.ValorAte.val().toFixed(2).toString());
            } else
                obj.ValorAte.val = _fornecedor.ValorAte.val();

            obj.CodigoIntegracao.val = _fornecedor.CodigoIntegracao.val();

            if (_fornecedor.PercentualDesconto.val() == "") {
                _fornecedor.PercentualDesconto.val(0);
                obj.PercentualDesconto.val = mvalor(_fornecedor.PercentualDesconto.val().toFixed(2).toString());
            } else
                obj.PercentualDesconto.val = _fornecedor.PercentualDesconto.val();

            obj.MoedaCotacaoBancoCentral.val = _fornecedor.MoedaCotacaoBancoCentral.val();
            if (_fornecedor.ValorMoedaCotacao.val() == "") {
                _fornecedor.ValorMoedaCotacao.val(0);
                obj.ValorMoedaCotacao.val = mvalor(_fornecedor.ValorMoedaCotacao.val().toFixed(2).toString());
            } else
                obj.ValorMoedaCotacao.val = _fornecedor.ValorMoedaCotacao.val();
            if (_fornecedor.ValorOriginalMoedaEstrangeira.val() == "") {
                _fornecedor.ValorOriginalMoedaEstrangeira.val(0);
                obj.ValorOriginalMoedaEstrangeira.val = mvalor(_fornecedor.ValorOriginalMoedaEstrangeira.val().toFixed(2).toString());
            } else
                obj.ValorOriginalMoedaEstrangeira.val = _fornecedor.ValorOriginalMoedaEstrangeira.val();

            _fornecedor.TabelaValores.list.push(obj);

            $("#" + _fornecedor.Produto.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Pessoas.Pessoa.ProdutoJaInformado, Localization.Resources.Pessoas.Pessoa.ProdutoJaFoiInformadoParaEstaTabelaDeValores.format(_fornecedor.Produto.val()));
        }
        LimparCamposTabelaValor();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.InformeCamposObrigatoriosParaLancamentoDaTabelaDeValores);
        if (_fornecedor.Produto.codEntity() > 0)
            _fornecedor.Produto.requiredClass("form-control");
        else
            _fornecedor.Produto.requiredClass("form-control is-invalid");
    }
}

function editarTabelaValorClick(data) {
    _fornecedor.Codigo.val(data.Codigo);
    _fornecedor.ValorFixo.val(data.ValorFixo);
    _fornecedor.ValorAte.val(data.ValorAte);
    _fornecedor.CodigoIntegracao.val(data.CodigoIntegracao);
    _fornecedor.PercentualDesconto.val(data.PercentualDesconto);
    _fornecedor.DataInicial.val(data.DataInicial);
    _fornecedor.DataFinal.val(data.DataFinal);
    _fornecedor.UnidadeMedida.val(data.CodigoUnidadeDeMedida);
    _fornecedor.Produto.codEntity(data.CodigoProduto);
    _fornecedor.Produto.val(data.Produto);
    _fornecedor.MoedaCotacaoBancoCentral.val(data.MoedaCotacaoBancoCentral);
    _fornecedor.ValorMoedaCotacao.val(data.ValorMoedaCotacao);
    _fornecedor.ValorOriginalMoedaEstrangeira.val(data.ValorOriginalMoedaEstrangeira);
}

function excluirTabelaValorClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Pessoa.RealmenteDesejaExcluirTabelaDeValor + data.Produto + "?", function () {
        var listaAtualizada = new Array();
        //$.each(_fornecedor.TabelaValores.list, function (i, Tabela) {
        //    if (Tabela.CodigoProduto.val != data.CodigoProduto) {
        //        listaAtualizada.push(Tabela);
        //    }
        //});
        $.each(_fornecedor.TabelaValores.list, function (i, Tabela) {
            if (Tabela.Codigo.val != data.Codigo) {
                listaAtualizada.push(Tabela);
            }
        });
        _fornecedor.TabelaValores.list = listaAtualizada;
        recarregarGridTabelaValores();
    });
}

function requiredCamposMultiplosVencimentos(required) {
    _fornecedor.DiaEmissaoInicial.required(required);
    _fornecedor.DiaEmissaoFinal.required(required);
    _fornecedor.Vencimento.required(required);
}

function adicionarTabelaMultiplosVencimentosClick() {
    requiredCamposMultiplosVencimentos(true);
    if (!ValidarCampoObrigatorioMap(_fornecedor.DiaEmissaoInicial) || !ValidarCampoObrigatorioMap(_fornecedor.DiaEmissaoFinal) || !ValidarCampoObrigatorioMap(_fornecedor.Vencimento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        requiredCamposMultiplosVencimentos(false);
        return;
    }

    var obj = new TabelaMultiplosVencimentosMap();

    obj.Codigo.val = guid();
    obj.Vencimento.val = _fornecedor.Vencimento.val();
    obj.DiaEmissaoInicial.val = _fornecedor.DiaEmissaoInicial.val();
    obj.DiaEmissaoFinal.val = _fornecedor.DiaEmissaoFinal.val();
    obj.DataEmissao.val = obterDataFormatada(_fornecedor.DiaEmissaoInicial, _fornecedor.DiaEmissaoFinal);

    _fornecedor.TabelaMultiplosVencimentos.list.push(obj);
    LimparCamposTabelaVencimentos();
}

function editarTabelaMultiplosVencimentosClick(data) {
    _fornecedor.AdicionarVencimento.visible(false);
    _fornecedor.Atualizar.visible(true);
    _fornecedor.DiaEmissaoInicial.val(data.DiaEmissaoInicial);
    _fornecedor.MultiploVencimentoCodigo.val(data.Codigo);
    _fornecedor.DiaEmissaoFinal.val(data.DiaEmissaoFinal);
    _fornecedor.Vencimento.val(data.Vencimento);
}

function excluirTabelaMultiplosVencimentosClick(data) {
    var novaLista = new Array();

    $.each(_fornecedor.TabelaMultiplosVencimentos.list, function (i, Tabela) {
        if (Tabela.Codigo.val != data.Codigo) {
            novaLista.push(Tabela);
        }
    });

    _fornecedor.TabelaMultiplosVencimentos.list = novaLista;
    recarregarGridTabelaMultiplosVencimentos();
}

function atualizarTabelaMultiplosVencimentosClick() {
    requiredCamposMultiplosVencimentos(true);
    if (!ValidarCampoObrigatorioMap(_fornecedor.DiaEmissaoInicial) || !ValidarCampoObrigatorioMap(_fornecedor.DiaEmissaoFinal) || !ValidarCampoObrigatorioMap(_fornecedor.Vencimento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        requiredCamposMultiplosVencimentos(false);
        return;
    }

    for (let i = 0; i < _fornecedor.TabelaMultiplosVencimentos.list.length; i++) {
        if (_fornecedor.TabelaMultiplosVencimentos.list[i].Codigo.val == _fornecedor.MultiploVencimentoCodigo.val()) {
            _fornecedor.TabelaMultiplosVencimentos.list[i].Vencimento.val = _fornecedor.Vencimento.val();
            _fornecedor.TabelaMultiplosVencimentos.list[i].DiaEmissaoInicial.val = _fornecedor.DiaEmissaoInicial.val();
            _fornecedor.TabelaMultiplosVencimentos.list[i].DiaEmissaoFinal.val = _fornecedor.DiaEmissaoFinal.val();
            _fornecedor.TabelaMultiplosVencimentos.list[i].DataEmissao.val = obterDataFormatada(_fornecedor.DiaEmissaoInicial, _fornecedor.DiaEmissaoFinal);
        }
    }
    LimparCamposTabelaVencimentos();
    _fornecedor.AdicionarVencimento.visible(true);
    _fornecedor.Atualizar.visible(false);
    _fornecedor.MultiploVencimentoCodigo.val(0);
}

//*******MÉTODOS*******

function obterFormDataAnexoFornecedor(anexos) {
    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function enviarAnexosFornecedor(codigo, anexos) {
    var formData = obterFormDataAnexoFornecedor(anexos);

    if (formData) {
        enviarArquivo("PessoaFornecedor/AnexarArquivos?callback=?", { Codigo: codigo }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _fornecedor.Anexos.val(retorno.Data.Anexos);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function downloadAnexoFornecedorClick(registroSelecionado) {
    executarDownload("PessoaFornecedor/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function removerAnexoFornecedorClick(registroSelecionado) {
    executarReST("PessoaFornecedor/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.AnexoExcluidoComSucesso);
                removerAnexoFornecedorDaGrid(registroSelecionado.Codigo);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function obterDataFormatada(datainicial, datafinal) {
    return datainicial.val() + Localization.Resources.Pessoas.Pessoa.Ate + datafinal.val();
}

function recarregarGridTabelaValores() {
    var data = new Array();
    $.each(_fornecedor.TabelaValores.list, function (i, tabela) {

        var obj = new Object();
        obj.Codigo = tabela.Codigo.val;
        obj.Produto = tabela.Produto.val;
        obj.CodigoProduto = tabela.CodigoProduto.val;
        obj.UnidadeDeMedida = tabela.UnidadeDeMedida.val;
        obj.CodigoUnidadeDeMedida = tabela.CodigoUnidadeDeMedida.val;
        obj.DataInicial = tabela.DataInicial.val;
        obj.DataFinal = tabela.DataFinal.val;
        obj.ValorFixo = tabela.ValorFixo.val;
        obj.ValorAte = tabela.ValorAte.val;
        obj.CodigoIntegracao = tabela.CodigoIntegracao.val;        
        obj.PercentualDesconto = tabela.PercentualDesconto.val;
        obj.MoedaCotacaoBancoCentral = tabela.MoedaCotacaoBancoCentral.val;
        obj.ValorMoedaCotacao = tabela.ValorMoedaCotacao.val;
        obj.ValorOriginalMoedaEstrangeira = tabela.ValorOriginalMoedaEstrangeira.val;

        data.push(obj);
    });
    _gridTabelaValores.CarregarGrid(data);
}

function recarregarGridTabelaMultiplosVencimentos() {
    var dataArray = new Array();

    $.each(_fornecedor.TabelaMultiplosVencimentos.list, function (i, tabela) {

        var obj = new Object();
        obj.Codigo = tabela.Codigo.val;
        obj.DataEmissao = tabela.DataEmissao.val;
        obj.DiaEmissaoInicial = tabela.DiaEmissaoInicial.val;
        obj.DiaEmissaoFinal = tabela.DiaEmissaoFinal.val;
        obj.Vencimento = tabela.Vencimento.val;

        dataArray.push(obj);
    });

    _gridTabelaMultiplosVencimentos.CarregarGrid(dataArray);
}

function loadTabelaAnexoFornecedor() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadAnexoFornecedorClick, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoFornecedorClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "25%", className: "text-align-left" }
    ];

    _gridAnexoFornecedor = new BasicDataTable(_fornecedor.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoFornecedor.CarregarGrid([]);
}

function loadTabelaMultiplosVencimentos() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: excluirTabelaMultiplosVencimentosClick, tamanho: "15", icone: "" };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarTabelaMultiplosVencimentosClick, tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [editar, excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DiaEmissaoFinal", visible: false },
        { data: "DiaEmissaoInicial", visible: false },
        { data: "DataEmissao", title: Localization.Resources.Pessoas.Pessoa.DataEmissao, width: "80%", className: "text-align-left" },
        { data: "Vencimento", title: Localization.Resources.Pessoas.Pessoa.Vencimento, width: "20%", className: "text-align-center" }
    ];
    _gridTabelaMultiplosVencimentos = new BasicDataTable(_fornecedor.TabelaMultiplosVencimentos.idGrid, header, menuOpcoes);
    recarregarGridTabelaMultiplosVencimentos();
}

function validarCamposPessoaFornecedor() {
    var tudoCerto = _pessoa.TipoFornecedor.val() ? ValidarCamposObrigatorios(_fornecedor) : true;
    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        $("#liFornecedor a").tab("show");
    }

    return tudoCerto;
}

function LimparCamposTabelaValor() {
    recarregarGridTabelaValores();
    LimparCampoEntity(_fornecedor.Produto);
    _fornecedor.Codigo.val(0);
    _fornecedor.ValorFixo.val("");
    _fornecedor.ValorAte.val("");
    _fornecedor.CodigoIntegracao.val("");
    _fornecedor.PercentualDesconto.val("");
    _fornecedor.DataInicial.val("");
    _fornecedor.DataFinal.val("");
    _fornecedor.UnidadeMedida.val(5);
    LimparCampo(_fornecedor.MoedaCotacaoBancoCentral);
    _fornecedor.ValorMoedaCotacao.val("");
    _fornecedor.ValorOriginalMoedaEstrangeira.val("");

    _fornecedor.Produto.requiredClass("form-control");
    _fornecedor.ValorFixo.requiredClass("form-control");
    _fornecedor.PercentualDesconto.requiredClass("form-control");
    _fornecedor.DataInicial.requiredClass("form-control");
    _fornecedor.DataFinal.requiredClass("form-control");
}

function LimparCamposTabelaVencimentos() {
    recarregarGridTabelaMultiplosVencimentos();
    _fornecedor.Vencimento.val(0);
    _fornecedor.DiaEmissaoFinal.val("");
    _fornecedor.DiaEmissaoInicial.val("");
    requiredCamposMultiplosVencimentos(false);
}

function mvalor(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function EditarListaAnexosFornecedor(anexos) {
    $("#liFornecedorAvisos").show();
    _fornecedor.Anexos.val(anexos);
}

function removerAnexoFornecedorDaGrid(codigo) {
    var anexos = obterAnexosFornecedor();

    for (var i = 0; i < anexos.length; i++) {
        if (anexos[i].Codigo == codigo) {
            anexos.splice(i, 1);
            break;
        }
    }

    _fornecedor.Anexos.val(anexos);
}

function obterAnexosFornecedor() {
    return _fornecedor.Anexos.val().slice();
}

function recarregarGridAnexoFornecedor() {
    var anexos = obterAnexosFornecedor();

    _gridAnexoFornecedor.CarregarGrid(anexos);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}
