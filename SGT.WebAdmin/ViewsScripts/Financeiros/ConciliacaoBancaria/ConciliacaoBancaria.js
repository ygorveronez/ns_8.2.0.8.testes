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
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoConciliacaoBancaria.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoMovimento.js" />
/// <reference path="../../Enumeradores/EnumDebitoCredito.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumDebitoCredito.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConciliacaoBancaria;
var _gridMovimentos;
var _gridExtratos;
var _conciliacaoBancaria;
var _GRUDConciliacaoBancaria;
var _pesquisaConciliacaoBancaria;
var _adicionarExtrato;
var _adicionarMovimentoFinanceiro;

var PesquisaConciliacaoBancaria = function () {
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Conta:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.SituacaoConciliacaoBancaria = PropertyEntity({ val: ko.observable(EnumSituacaoConciliacaoBancaria.Aberto), options: EnumSituacaoConciliacaoBancaria.obterOpcoesPesquisa(), def: EnumSituacaoConciliacaoBancaria.Aberto, text: "Situação: " });
    this.ValorExtratoInicial = PropertyEntity({ text: "Valor Extrato Inicial: ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: true } });
    this.ValorExtratoFinal = PropertyEntity({ text: "Valor Extrato Final: ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: true } });
    this.ValorMovimentoInicial = PropertyEntity({ text: "Valor Movimento Inicial: ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: true } });
    this.ValorMovimentoFinal = PropertyEntity({ text: "Valor Movimento Final: ", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: true } });

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.DataGeracaoMovimentoFinanceiro = PropertyEntity({ text: "Data Geração Movimento Financeiro: ", getType: typesKnockout.date });
    this.NumeroDocumentoMovimentoFinanceiro = PropertyEntity({ text: "N° Documento Movimento Financeiro: " });
    this.CodigoTitulo = PropertyEntity({ text: "Cód. Título: ", getType: typesKnockout.int });
    this.ValorMovimentoFinanceiroInicial = PropertyEntity({ text: "Valor Movimento Financeiro Inicial: ", getType: typesKnockout.decimal });
    this.ValorMovimentoFinanceiroFinal = PropertyEntity({ text: "Valor Movimento Financeiro Final: ", getType: typesKnockout.decimal });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConciliacaoBancaria.CarregarGrid();
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
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDConciliacaoBancaria = function () {

    this.FecharConciliacaoEContinuar = PropertyEntity({ eventClick: FecharConciliacaoEContinuarClick, type: types.event, text: "Fechar Conciliação e Continuar", visible: ko.observable(true) });
    this.FecharConciliacao = PropertyEntity({ eventClick: FecharConciliacaoClick, type: types.event, text: "Fechar Conciliação", visible: ko.observable(true) });
    this.EstornarConcilicao = PropertyEntity({ eventClick: EstornarConcilicaoClick, type: types.event, text: "Cancelar Conciliação", visible: ko.observable(false) });
    this.ReAbrirConciliacao = PropertyEntity({ eventClick: ReAbrirConciliacaoClick, type: types.event, text: "Reabrir Conciliação", visible: ko.observable(false) });
    this.ImprimirDetalhado = PropertyEntity({ eventClick: ImprimirDetalhadoClick, type: types.event, text: ko.observable("Detalhado"), visible: ko.observable(false), icon: "fal fa-file-pdf" });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar / Atualizar", visible: ko.observable(true) });
};

var ConciliacaoBancaria = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoConciliacaoBancaria = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AnaliticoSintetico = PropertyEntity({ val: ko.observable(EnumAnaliticoSintetico.Analitico), options: EnumAnaliticoSintetico.obterOpcoes(), text: "*Tipo Conta: ", def: EnumAnaliticoSintetico.Analitico, visible: ko.observable(true), enable: ko.observable(true) });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Plano de Conta:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PlanoContaSintetico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Plano de Conta:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", required: false, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", required: false, getType: typesKnockout.date, enable: ko.observable(true) });
    this.RealizarConciliacaoAutomatica = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Conciliação Automática?", def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Processar = PropertyEntity({ eventClick: ProcessarClick, type: types.event, text: "Consultar Conciliação", visible: ko.observable(true), enable: ko.observable(true) });

    this.DataPesquisaExtrato = PropertyEntity({ text: "Data De: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataAtePesquisaExtrato = PropertyEntity({ text: "Data até: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.ValorPesquisaExtrato = PropertyEntity({ text: "Valor De: ", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorAtePesquisaExtrato = PropertyEntity({ text: "Valor Até: ", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.NumeroDocumentoPesquisaExtrato = PropertyEntity({ text: "Nº Documento: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.ObservacaoExtrato = PropertyEntity({ text: "Obs: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.CodigoLancamentoPesquisaExtrato = PropertyEntity({ text: "Cód. Lançamento: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.DebitoCreditoExtrato = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Todos), options: EnumDebitoCredito.ObterOpcoesPesquisa(), text: "Débito/Crédito: ", def: EnumDebitoCredito.Todos, enable: ko.observable(true) });
    this.PesquisarExtrato = PropertyEntity({ eventClick: PesquisarExtratoClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });

    this.SelecionarTodosExtratos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.Extratos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: "grid-conciliacao-extrato", idTab: guid(), enable: ko.observable(false) });
    this.ValorTotalDebitoExtrato = PropertyEntity({ text: "Total Débito Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalCreditoExtrato = PropertyEntity({ text: "Total Crédito Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalExtrato = PropertyEntity({ text: "Total Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });

    this.ValorTotalGeralDebitoExtrato = PropertyEntity({ text: "Total Geral Débito: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalGeralCreditoExtrato = PropertyEntity({ text: "Total Geral Crédito: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalGeralExtrato = PropertyEntity({ text: "Total Geral: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });

    this.ImportarExtrato = PropertyEntity({ eventClick: ImportarExtratoClick, type: types.event, text: "Importar Extrato", visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarLancamento = PropertyEntity({ eventClick: AdicionarLancamentoClick, type: types.event, text: "Adicionar Extrato", visible: ko.observable(true), enable: ko.observable(true) });
    this.Extrato = PropertyEntity({ type: types.file, eventChange: EnviarExtrato, codEntity: ko.observable(0), text: "Extrato:", val: ko.observable(""), visible: ko.observable(false) });
    this.ListaExtratos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaExtratosNaoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.SelecionarTodosMovimentos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.Movimentos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: "grid-conciliacao-movimento", idTab: guid(), enable: ko.observable(false) });
    this.ValorTotalDebitoMovimento = PropertyEntity({ text: "Total Débito Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalCreditoMovimento = PropertyEntity({ text: "Total Crédito Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalMovimento = PropertyEntity({ text: "Total Selecionado: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });

    this.ValorTotalGeralDebitoMovimento = PropertyEntity({ text: "Total Geral Débito: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalGeralCreditoMovimento = PropertyEntity({ text: "Total Geral Crédito: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.ValorTotalGeralMovimento = PropertyEntity({ text: "Total Geral: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });

    this.AdicionarMovimento = PropertyEntity({ eventClick: AdicionarMovimentoClick, type: types.event, text: "Adicionar Movimento", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaMovimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaMovimentosNaoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CodigoTituloMovimento = PropertyEntity({ text: "Cód. Título: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.DataPesquisaMovimento = PropertyEntity({ text: "Data De: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataAtePesquisaMovimento = PropertyEntity({ text: "Data Até: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.ValorPesquisaMovimento = PropertyEntity({ text: "Valor De: ", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorAtePesquisaMovimento = PropertyEntity({ text: "Valor Até: ", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ObservacaoMovimento = PropertyEntity({ text: "Obs: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.NumeroDocumentoPesquisaMovimento = PropertyEntity({ text: "Nº Documento: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.NumeroChequePesquisaMovimento = PropertyEntity({ text: "Nº Cheque: ", getType: typesKnockout.string, enable: ko.observable(true) });
    this.DebitoCreditoMovimento = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Todos), options: EnumDebitoCredito.ObterOpcoesPesquisa(), text: "Débito/Crédito: ", def: EnumDebitoCredito.Todos, enable: ko.observable(true) });
    this.TipoPessoaPesquisaMovimento = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", eventChange: TipoPessoaPesquisaMovimentoChange, enable: ko.observable(true) });
    this.PessoaPesquisaMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoPessoaPesquisaMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.PesquisarMovimento = PropertyEntity({ eventClick: PesquisarMovimentoClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });

    this.AnaliticoSintetico.val.subscribe(function (novoValor) {
        if (novoValor === EnumAnaliticoSintetico.Analitico) {
            LimparCampoEntity(_conciliacaoBancaria.PlanoContaSintetico);
            _conciliacaoBancaria.PlanoConta.required(true);
            _conciliacaoBancaria.PlanoContaSintetico.required(false);
            _conciliacaoBancaria.PlanoConta.visible(true);
            _conciliacaoBancaria.PlanoContaSintetico.visible(false);
        } else {
            LimparCampoEntity(_conciliacaoBancaria.PlanoConta);
            _conciliacaoBancaria.PlanoConta.required(false);
            _conciliacaoBancaria.PlanoContaSintetico.required(true);
            _conciliacaoBancaria.PlanoConta.visible(false);
            _conciliacaoBancaria.PlanoContaSintetico.visible(true);
        }
    });
};

var AdicionarExtratoBancario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.DebitoCredito = PropertyEntity({ val: ko.observable(EnumDebitoCredito.Credito), options: EnumDebitoCredito.ObterOpcoes(), text: "*Débito/Crédito: ", def: EnumDebitoCredito.Credito, required: true });
    this.TipoDocumentoMovimento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: EnumTipoDocumentoMovimento.obterOpcoes(), text: "*Tipo Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.Documento = PropertyEntity({ text: "*Nº Documento: ", required: true });

    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Conta:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.ExtratoBancarioTipoLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Lançamento:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.CodigoLancamento = PropertyEntity({ text: "Cód. Tipo Lanç.: ", required: false, val: ko.observable("") });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarExtratoBancarioClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
};

var AdicionarMovimentoFinanceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.DataBase = PropertyEntity({ text: "*Data Base: ", required: true, getType: typesKnockout.date });
    this.ValorMovimento = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: EnumTipoDocumentoMovimento.obterOpcoes(), text: "*Tipo Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº Documento: ", required: true });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Movimento:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Saída:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarMovimentoFinanceiroClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadConciliacaoBancaria() {

    _pesquisaConciliacaoBancaria = new PesquisaConciliacaoBancaria();
    KoBindings(_pesquisaConciliacaoBancaria, "knockoutPesquisaConciliacaoBancaria", false, _pesquisaConciliacaoBancaria.Pesquisar.id);

    _conciliacaoBancaria = new ConciliacaoBancaria();
    KoBindings(_conciliacaoBancaria, "knockoutConciliacaoBancaria");

    HeaderAuditoria("ConciliacaoBancaria", _conciliacaoBancaria);

    _GRUDConciliacaoBancaria = new CRUDConciliacaoBancaria();
    KoBindings(_GRUDConciliacaoBancaria, "knockoutCRUDConciliacaoBancaria");

    _adicionarExtrato = new AdicionarExtratoBancario();
    KoBindings(_adicionarExtrato, "knoutAdicionarExtratoBancario");

    _adicionarMovimentoFinanceiro = new AdicionarMovimentoFinanceiro();
    KoBindings(_adicionarMovimentoFinanceiro, "knoutAdicionarMovimentoFinanceiro");

    new BuscarFuncionario(_pesquisaConciliacaoBancaria.Operador);
    new BuscarPlanoConta(_pesquisaConciliacaoBancaria.PlanoConta, "Selecione a Conta", "Plano de Conta");
    new BuscarPlanoConta(_conciliacaoBancaria.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_conciliacaoBancaria.PlanoContaSintetico, "Selecione a Conta Sintético", "Contas Sintéticas", null, EnumAnaliticoSintetico.Sintetico);

    new BuscarCentroResultado(_adicionarMovimentoFinanceiro.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", null, EnumAnaliticoSintetico.Analitico, _adicionarMovimentoFinanceiro.TipoMovimento);
    new BuscarPlanoConta(_adicionarMovimentoFinanceiro.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_adicionarMovimentoFinanceiro.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTipoMovimento(_adicionarMovimentoFinanceiro.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

    new BuscarTransportadores(_adicionarExtrato.Empresa);
    new BuscarClientes(_conciliacaoBancaria.PessoaPesquisaMovimento);
    new BuscarGruposPessoas(_conciliacaoBancaria.GrupoPessoaPesquisaMovimento);
    new BuscarExtratoBancarioTipoLancamento(_adicionarExtrato.ExtratoBancarioTipoLancamento, RetornoExtratoBancarioTipoLancamento);
    new BuscarPlanoConta(_adicionarExtrato.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _adicionarExtrato.Empresa.visible(false);
        _adicionarExtrato.Empresa.required(false);
    }

    buscarConciliacaoBancaria();
    CarregarGridExtratoMovimento();
}

function RetornoExtratoBancarioTipoLancamento(data) {
    _adicionarExtrato.ExtratoBancarioTipoLancamento.codEntity(data.Codigo);
    _adicionarExtrato.ExtratoBancarioTipoLancamento.val(data.Descricao);
    _adicionarExtrato.CodigoLancamento.val(data.CodigoIntegracao);
}

function TipoPessoaPesquisaMovimentoChange() {
    if (_conciliacaoBancaria.TipoPessoaPesquisaMovimento.val() === EnumTipoPessoaGrupo.Pessoa) {
        _conciliacaoBancaria.PessoaPesquisaMovimento.visible(true);
        _conciliacaoBancaria.GrupoPessoaPesquisaMovimento.visible(false);
        LimparCampoEntity(_conciliacaoBancaria.GrupoPessoaPesquisaMovimento);
    } else if (_conciliacaoBancaria.TipoPessoaPesquisaMovimento.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _conciliacaoBancaria.PessoaPesquisaMovimento.visible(false);
        _conciliacaoBancaria.GrupoPessoaPesquisaMovimento.visible(true);
        LimparCampoEntity(_conciliacaoBancaria.PessoaPesquisaMovimento);
    }
}

function CarregarGridExtratoMovimento() {
    _conciliacaoBancaria.SelecionarTodosMovimentos.visible(true);
    _conciliacaoBancaria.SelecionarTodosMovimentos.val(false);

    var multiplaescolhaMovimentos = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorMovimentos();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorMovimentos();
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _conciliacaoBancaria.SelecionarTodosMovimentos,
        somenteLeitura: false
    };
    _gridMovimentos = new GridView(_conciliacaoBancaria.Movimentos.idGrid, "ConciliacaoBancaria/PesquisaMovimentos", _conciliacaoBancaria, null, { column: 3, dir: orderDir.asc }, 20, null, null, null, multiplaescolhaMovimentos);
    _gridMovimentos.SetPermitirEdicaoColunas(true);
    _gridMovimentos.SetPermitirReordenarColunas(false);
    _gridMovimentos.SetSalvarPreferenciasGrid(true);
    _gridMovimentos.CarregarGrid();

    _conciliacaoBancaria.SelecionarTodosExtratos.visible(true);
    _conciliacaoBancaria.SelecionarTodosExtratos.val(false);

    var multiplaescolhaExtratos = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorExtratos();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorExtratos();
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _conciliacaoBancaria.SelecionarTodosExtratos,
        somenteLeitura: false
    };
    _gridExtratos = new GridView(_conciliacaoBancaria.Extratos.idGrid, "ConciliacaoBancaria/PesquisaExtratoBancario", _conciliacaoBancaria, null, null, 20, null, null, null, multiplaescolhaExtratos);
    _gridExtratos.SetPermitirEdicaoColunas(true);
    _gridExtratos.SetPermitirReordenarColunas(false);
    _gridExtratos.SetSalvarPreferenciasGrid(true);
    _gridExtratos.CarregarGrid();
}

function AtualizarValorMovimentos() {

    _conciliacaoBancaria.ValorTotalCreditoMovimento.val("0,00");
    _conciliacaoBancaria.ValorTotalDebitoMovimento.val("0,00");
    _conciliacaoBancaria.ValorTotalMovimento.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralDebitoMovimento.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralCreditoMovimento.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralMovimento.val("0,00");

    var movimentosSelecionados = null;
    var movimentosNaoSelecionados = null;

    //if (_conciliacaoBancaria.SelecionarTodosMovimentos.val()) {
    //    movimentosSelecionados = _gridMovimentos.ObterMultiplosNaoSelecionados();
    //} else {
    movimentosSelecionados = _gridMovimentos.ObterMultiplosSelecionados();
    movimentosNaoSelecionados = _gridMovimentos.ObterMultiplosNaoSelecionados();
    //}

    var codigosMovimentos = new Array();
    var codigosMovimentosNaoSelecionados = new Array();

    for (var i = 0; i < movimentosSelecionados.length; i++)
        codigosMovimentos.push(movimentosSelecionados[i].DT_RowId);

    for (var i = 0; i < movimentosNaoSelecionados.length; i++)
        codigosMovimentosNaoSelecionados.push(movimentosNaoSelecionados[i].DT_RowId);

    //if (codigosMovimentos && (codigosMovimentos.length > 0 || _conciliacaoBancaria.SelecionarTodosMovimentos.val())) {
    _conciliacaoBancaria.ListaMovimentos.val(JSON.stringify(codigosMovimentos));
    _conciliacaoBancaria.ListaMovimentosNaoSelecionados.val(JSON.stringify(codigosMovimentosNaoSelecionados));

    executarReST("ConciliacaoBancaria/AtualizarMovimentosSelecionados", RetornarObjetoPesquisa(_conciliacaoBancaria), function (r) {
        if (r.Success) {
            if (r.Data) {
                CarregarConciliacaoBancaria(r, false);
                //_conciliacaoBancaria.SelecionarTodosMovimentos.val(false);                
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
    //}
}

function AtualizarValorExtratos() {

    _conciliacaoBancaria.ValorTotalCreditoExtrato.val("0,00");
    _conciliacaoBancaria.ValorTotalDebitoExtrato.val("0,00");
    _conciliacaoBancaria.ValorTotalExtrato.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralDebitoExtrato.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralCreditoExtrato.val("0,00");
    _conciliacaoBancaria.ValorTotalGeralExtrato.val("0,00");

    var extratosSelecionados = null;
    var extratosNaoSelecionados = null;

    //if (_conciliacaoBancaria.SelecionarTodosExtratos.val()) {
    //    extratosSelecionados = _gridExtratos.ObterMultiplosNaoSelecionados();
    //} else {
    extratosSelecionados = _gridExtratos.ObterMultiplosSelecionados();
    extratosNaoSelecionados = _gridExtratos.ObterMultiplosNaoSelecionados();
    //}

    var codigosExtratos = new Array();
    var codigosExtratosNaoSelecionados = new Array();

    for (var i = 0; i < extratosSelecionados.length; i++)
        codigosExtratos.push(extratosSelecionados[i].DT_RowId);

    for (var i = 0; i < extratosNaoSelecionados.length; i++)
        codigosExtratosNaoSelecionados.push(extratosNaoSelecionados[i].DT_RowId);

    //if (codigosExtratos && (codigosExtratos.length > 0 || _conciliacaoBancaria.SelecionarTodosExtratos.val())) {
    _conciliacaoBancaria.ListaExtratos.val(JSON.stringify(codigosExtratos));
    _conciliacaoBancaria.ListaExtratosNaoSelecionados.val(JSON.stringify(codigosExtratosNaoSelecionados));

    executarReST("ConciliacaoBancaria/AtualizarExtratosSelecionados", RetornarObjetoPesquisa(_conciliacaoBancaria), function (r) {
        if (r.Success) {
            if (r.Data) {
                CarregarConciliacaoBancaria(r, false);
                //_conciliacaoBancaria.SelecionarTodosExtratos.val(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
    //}
}

function RetornoTipoMovimento(data) {
    if (data !== null) {
        _adicionarMovimentoFinanceiro.TipoMovimento.codEntity(data.Codigo);
        _adicionarMovimentoFinanceiro.TipoMovimento.val(data.Descricao);

        if (data.CodigoDebito > 0) {
            _adicionarMovimentoFinanceiro.PlanoDebito.codEntity(data.CodigoDebito);
            _adicionarMovimentoFinanceiro.PlanoDebito.val(data.PlanoDebito);
        }
        if (data.CodigoCredito > 0) {
            _adicionarMovimentoFinanceiro.PlanoCredito.codEntity(data.CodigoCredito);
            _adicionarMovimentoFinanceiro.PlanoCredito.val(data.PlanoCredito);
        }
        if (data.CodigoResultado > 0) {
            _adicionarMovimentoFinanceiro.CentroResultado.codEntity(data.CodigoResultado);
            _adicionarMovimentoFinanceiro.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_adicionarMovimentoFinanceiro.CentroResultado);
        }
    }
}

function AdicionarExtratoBancarioClick(e, sender) {
    Salvar(_adicionarExtrato, "ConciliacaoBancaria/AdicionarExtratoBancario", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Valor do extrato bancário lançado com sucesso.");

                CarregarConciliacaoBancaria(arg, true);

                Global.fecharModal('divAdicionarExtratoBancario');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function AdicionarMovimentoFinanceiroClick(e, sender) {
    Salvar(_adicionarMovimentoFinanceiro, "ConciliacaoBancaria/AdicionarMovimentoFinanceiro", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Movimento financeiro lançado com sucesso.");

                CarregarConciliacaoBancaria(arg, true);

                Global.fecharModal('divAdicionarMovimentoFinanceiro');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function FecharConciliacaoEContinuarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Fechar a Conciliação selecionada?", function () {
        Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/FecharConciliacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação fechada com sucesso.");

                    //editarConciliacaoBancaria(arg.Data);
                    limparCamposConciliacaoBancaria();

                    _gridExtratos.AtualizarRegistrosNaoSelecionados([]);
                    _gridExtratos.AtualizarRegistrosSelecionados([]);
                    _gridExtratos.CarregarGrid();

                    _gridMovimentos.AtualizarRegistrosSelecionados([]);
                    _gridMovimentos.AtualizarRegistrosNaoSelecionados([]);
                    _gridMovimentos.CarregarGrid();

                    _conciliacaoBancaria.AnaliticoSintetico.val(arg.Data.AnaliticoSintetico);
                    _conciliacaoBancaria.PlanoConta.val(arg.Data.PlanoConta);
                    _conciliacaoBancaria.PlanoConta.codEntity(arg.Data.CodigoPlanoConta);
                    _conciliacaoBancaria.PlanoContaSintetico.val(arg.Data.PlanoContaSintetico);
                    _conciliacaoBancaria.PlanoContaSintetico.codEntity(arg.Data.CodigoPlanoContaSintetico);
                    _conciliacaoBancaria.DataInicial.val(arg.Data.DataInicial);
                    _conciliacaoBancaria.DataFinal.val(arg.Data.DataFinal);

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function FecharConciliacaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Fechar a Conciliação selecionada?", function () {
        Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/FecharConciliacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação fechada com sucesso.");

                    editarConciliacaoBancaria(arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function ReAbrirConciliacaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Reabrir a Conciliação selecionada?", function () {
        Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/ReAbrirConciliacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação aberta com sucesso.");

                    limparCamposConciliacaoBancaria();

                    _conciliacaoBancaria.Codigo.val(arg.Data.Codigo);
                    BuscarPorCodigo(_conciliacaoBancaria, "ConciliacaoBancaria/BuscarPorCodigo", function (arg) {

                        _pesquisaConciliacaoBancaria.ExibirFiltros.visibleFade(false);
                        CarregarConciliacaoBancaria(arg, true);

                        _GRUDConciliacaoBancaria.FecharConciliacaoEContinuar.visible(true);
                        _GRUDConciliacaoBancaria.FecharConciliacao.visible(true);
                        _GRUDConciliacaoBancaria.EstornarConcilicao.visible(true);
                        _GRUDConciliacaoBancaria.ReAbrirConciliacao.visible(false);
                        _GRUDConciliacaoBancaria.ImprimirDetalhado.visible(false);

                    }, null);

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function EstornarConcilicaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja Cancelar a Conciliação selecionada?", function () {
        Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/EstornarConciliacao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação cancelada com sucesso.");

                    limparCamposConciliacaoBancaria();

                    _gridExtratos.CarregarGrid();
                    _gridMovimentos.CarregarGrid();
                    _gridExtratos.AtualizarRegistrosSelecionados([]);
                    _gridMovimentos.AtualizarRegistrosSelecionados([]);

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function PesquisarMovimentoClick(e, sender) {
    _gridMovimentos.CarregarGrid(BuscarMovimentosSelecionados);
}

function PesquisarExtratoClick(e, sender) {
    _gridExtratos.CarregarGrid(BuscarExtratosSelecionados);
}

function ProcessarClick(e, sender) {
    Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/ProcessarConciliacaoBancaria", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação processada com sucesso.");

                CarregarConciliacaoBancaria(arg, true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function ImportarExtratoClick(e, sender) {
    if (_conciliacaoBancaria.Codigo.val() === 0 || _conciliacaoBancaria.Codigo.val() === "" || _conciliacaoBancaria.Codigo.val() === null || _conciliacaoBancaria.Codigo.val() === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor inicie a conciliação bancária antes de importar um novo extrato bancário.");
    } else {
        $("#" + _conciliacaoBancaria.Extrato.id).trigger("click");
    }
}

function EnviarExtrato(e, sender) {
    if (_conciliacaoBancaria.Extrato.val() != "") {
        var file = document.getElementById(_conciliacaoBancaria.Extrato.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        var data = { Codigo: _conciliacaoBancaria.Codigo.val(), PlanoConta: _conciliacaoBancaria.PlanoConta.codEntity(), PlanoContaSintetico: _conciliacaoBancaria.PlanoContaSintetico.codEntity() };
        enviarArquivo("ConciliacaoBancaria/ImportarExtrato", data, formData, function (arg) {

            var fileControl = $("#" + _conciliacaoBancaria.Extrato.id);
            fileControl.replaceWith(fileControl = fileControl.clone(true));

            if (arg.Success) {
                if (arg.Data) {
                    CarregarConciliacaoBancaria(arg, true);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function AdicionarLancamentoClick(e, sender) {
    if (_conciliacaoBancaria.Codigo.val() === 0 || _conciliacaoBancaria.Codigo.val() === "" || _conciliacaoBancaria.Codigo.val() === null || _conciliacaoBancaria.Codigo.val() === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor inicie a conciliação bancária antes de realizar o lançamento.");
    } else {
        LimparCampos(_adicionarExtrato);
        _adicionarExtrato.Codigo.val(_conciliacaoBancaria.Codigo.val());
        Global.abrirModal("divAdicionarExtratoBancario");
    }
}

function AdicionarMovimentoClick(e, sender) {
    if (_conciliacaoBancaria.Codigo.val() === 0 || _conciliacaoBancaria.Codigo.val() === "" || _conciliacaoBancaria.Codigo.val() === null || _conciliacaoBancaria.Codigo.val() === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor inicie a conciliação bancária antes de realizar o lançamento.");
    } else {
        LimparCampos(_adicionarMovimentoFinanceiro);
        _adicionarMovimentoFinanceiro.Codigo.val(_conciliacaoBancaria.Codigo.val());
        Global.abrirModal("divAdicionarMovimentoFinanceiro");                
    }
}

function ImprimirDetalhadoClick(e, sender) {
    var data = { Codigo: _conciliacaoBancaria.Codigo.val(), Detalhado: true };
    executarDownload("ConciliacaoBancaria/BaixarRelatorio", data);
}

function LimparClick(e, sender) {
    limparCamposConciliacaoBancaria();

    _gridExtratos.CarregarGrid();
    _gridMovimentos.CarregarGrid();
    _gridExtratos.AtualizarRegistrosSelecionados([]);
    _gridMovimentos.AtualizarRegistrosSelecionados([]);
}

//*******MÉTODOS*******

function editarConciliacaoBancaria(conciliacaoBancariaGrid) {

    limparCamposConciliacaoBancaria();

    _conciliacaoBancaria.Codigo.val(conciliacaoBancariaGrid.Codigo);
    BuscarPorCodigo(_conciliacaoBancaria, "ConciliacaoBancaria/BuscarPorCodigo", function (arg) {

        _pesquisaConciliacaoBancaria.ExibirFiltros.visibleFade(false);
        CarregarConciliacaoBancaria(arg, true);
    }, null);
}

function BuscarMovimentosSelecionados() {
    Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/BuscarMovimentosSelecionados", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _conciliacaoBancaria.SelecionarTodosMovimentos.val(false);
                _gridMovimentos.AtualizarRegistrosSelecionados(new Array());
                _gridMovimentos.AtualizarRegistrosNaoSelecionados(new Array());
                _gridMovimentos.AtualizarRegistrosSelecionados(arg.Data.MovimentosConcolidados);
                _gridMovimentos.DrawTable(false);

                _conciliacaoBancaria.ValorTotalDebitoMovimento.val(arg.Data.ValorTotalDebitoMovimento);
                _conciliacaoBancaria.ValorTotalCreditoMovimento.val(arg.Data.ValorTotalCreditoMovimento);
                _conciliacaoBancaria.ValorTotalMovimento.val(arg.Data.ValorTotalMovimento);
                _conciliacaoBancaria.ValorTotalGeralDebitoMovimento.val(arg.Data.ValorTotalGeralDebitoMovimento);
                _conciliacaoBancaria.ValorTotalGeralCreditoMovimento.val(arg.Data.ValorTotalGeralCreditoMovimento);
                _conciliacaoBancaria.ValorTotalGeralMovimento.val(arg.Data.ValorTotalGeralMovimento);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, exibirCamposObrigatorio);
}

function BuscarExtratosSelecionados() {
    Salvar(_conciliacaoBancaria, "ConciliacaoBancaria/BuscarExtratosSelecionados", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _conciliacaoBancaria.SelecionarTodosExtratos.val(false);
                _gridExtratos.AtualizarRegistrosSelecionados(new Array());
                _gridExtratos.AtualizarRegistrosNaoSelecionados(new Array());
                _gridExtratos.AtualizarRegistrosSelecionados(arg.Data.ExtratosConcolidados);
                _gridExtratos.DrawTable(false);

                _conciliacaoBancaria.ValorTotalDebitoExtrato.val(arg.Data.ValorTotalDebitoExtrato);
                _conciliacaoBancaria.ValorTotalCreditoExtrato.val(arg.Data.ValorTotalCreditoExtrato);
                _conciliacaoBancaria.ValorTotalExtrato.val(arg.Data.ValorTotalExtrato);

                _conciliacaoBancaria.ValorTotalGeralDebitoExtrato.val(arg.Data.ValorTotalGeralDebitoExtrato);
                _conciliacaoBancaria.ValorTotalGeralCreditoExtrato.val(arg.Data.ValorTotalGeralCreditoExtrato);
                _conciliacaoBancaria.ValorTotalGeralExtrato.val(arg.Data.ValorTotalGeralExtrato);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, exibirCamposObrigatorio);
}

function CarregarConciliacaoBancaria(arg, editando) {
    PreencherObjetoKnout(_conciliacaoBancaria, arg);

    if (_conciliacaoBancaria.SituacaoConciliacaoBancaria.val() === EnumSituacaoConciliacaoBancaria.Aberto) {
        _GRUDConciliacaoBancaria.FecharConciliacaoEContinuar.visible(true);
        _GRUDConciliacaoBancaria.FecharConciliacao.visible(true);
        _GRUDConciliacaoBancaria.EstornarConcilicao.visible(true);
        _GRUDConciliacaoBancaria.ReAbrirConciliacao.visible(false);
        _GRUDConciliacaoBancaria.ImprimirDetalhado.visible(true);
        _gridExtratos.SetarRegistrosSomenteLeitura(false);
        _gridMovimentos.SetarRegistrosSomenteLeitura(false);
        SetarEnableCamposKnockout(_conciliacaoBancaria, true);
    } else if (_conciliacaoBancaria.SituacaoConciliacaoBancaria.val() === EnumSituacaoConciliacaoBancaria.Finalizado) {
        _GRUDConciliacaoBancaria.FecharConciliacaoEContinuar.visible(false);
        _GRUDConciliacaoBancaria.FecharConciliacao.visible(false);
        _GRUDConciliacaoBancaria.EstornarConcilicao.visible(true);
        _GRUDConciliacaoBancaria.ReAbrirConciliacao.visible(true);
        _GRUDConciliacaoBancaria.ImprimirDetalhado.visible(true);
        _gridExtratos.SetarRegistrosSomenteLeitura(true);
        _gridMovimentos.SetarRegistrosSomenteLeitura(true);
        SetarEnableCamposKnockout(_conciliacaoBancaria, false);
    }
    else {
        _GRUDConciliacaoBancaria.FecharConciliacaoEContinuar.visible(false);
        _GRUDConciliacaoBancaria.FecharConciliacao.visible(false);
        _GRUDConciliacaoBancaria.EstornarConcilicao.visible(false);
        _GRUDConciliacaoBancaria.ReAbrirConciliacao.visible(false);
        _GRUDConciliacaoBancaria.ImprimirDetalhado.visible(false);
        SetarEnableCamposKnockout(_conciliacaoBancaria, false);
        _gridExtratos.SetarRegistrosSomenteLeitura(true);
        _gridMovimentos.SetarRegistrosSomenteLeitura(true);
    }

    //_gridExtratos.CarregarGrid();
    if (editando) {
        _gridExtratos.AtualizarRegistrosNaoSelecionados([]);
        _gridExtratos.AtualizarRegistrosSelecionados(arg.Data.ExtratosConcolidados);
        _gridExtratos.CarregarGrid();

        _gridMovimentos.AtualizarRegistrosSelecionados(arg.Data.MovimentosConcolidados);
        _gridMovimentos.AtualizarRegistrosNaoSelecionados([]);
        _gridMovimentos.CarregarGrid();
    }

    _conciliacaoBancaria.PlanoConta.enable(false);
    _conciliacaoBancaria.PlanoContaSintetico.enable(false);
    _conciliacaoBancaria.AnaliticoSintetico.enable(false);
}

function atualizarMovimentosConcolidados(arg) {
    _gridMovimentos.AtualizarRegistrosSelecionados(arg.Data.MovimentosConcolidados);
    _gridMovimentos.DrawTable();
}

function atualizarExtratosConcolidados(arg) {
    _gridExtratos.AtualizarRegistrosSelecionados(arg.Data.ExtratosConcolidados);
    _gridExtratos.DrawTable();
}

function buscarConciliacaoBancaria() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConciliacaoBancaria, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConciliacaoBancaria = new GridView(_pesquisaConciliacaoBancaria.Pesquisar.idGrid, "ConciliacaoBancaria/Pesquisa", _pesquisaConciliacaoBancaria, menuOpcoes, null);
    _gridConciliacaoBancaria.CarregarGrid();
}

function limparCamposConciliacaoBancaria() {
    LimparCampos(_conciliacaoBancaria);

    _GRUDConciliacaoBancaria.Limpar.visible(true);
    _GRUDConciliacaoBancaria.FecharConciliacaoEContinuar.visible(true);
    _GRUDConciliacaoBancaria.FecharConciliacao.visible(true);
    _GRUDConciliacaoBancaria.EstornarConcilicao.visible(false);
    _GRUDConciliacaoBancaria.ReAbrirConciliacao.visible(false);
    _GRUDConciliacaoBancaria.ImprimirDetalhado.visible(false);

    _gridExtratos.SetarRegistrosSomenteLeitura(false);
    _gridMovimentos.SetarRegistrosSomenteLeitura(false);
    SetarEnableCamposKnockout(_conciliacaoBancaria, true);
    TipoPessoaPesquisaMovimentoChange();

    _gridConciliacaoBancaria.CarregarGrid();
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}
