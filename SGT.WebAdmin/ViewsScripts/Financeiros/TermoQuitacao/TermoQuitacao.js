/// <reference path="../../Enumeradores/EmumAprovacaoPendente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacaoFinanciero.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlcadaRegra.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="Anexos.js" />

//#region Variaveis Globais
var _termoQuitacao;
var _pesquitaTermoQuitacao;
var _gridTermoQuitacao;
var _gridFiliais;
var _crudTermoQuitacao;
var _dataInicialAnterior;
var _dataFinallAnterior;
//#endregion

//#region Constructores

var _statusProvisasaoPesquisa = {
    Todos: "",
    Sim: 1,
    Nao: 0
}

var _statusAprovacaoProvisao = {
    Todas: "",
    Pendente: 0,
    Aprovada: 1,
    Rejeitada: 9,
}

var _statusProvisasaoPesquisaOpcoes = [
    { text: "Sim", value: _statusProvisasaoPesquisa.Sim },
    { text: "Não", value: _statusProvisasaoPesquisa.Nao },
    { text: "Todos", value: _statusProvisasaoPesquisa.Todos },
]

var _stautsAprovacaoProvisaoPesquisaOpcoes = [
    { text: "Pendente", value: _statusAprovacaoProvisao.Pendente },
    { text: "Aprovada", value: _statusAprovacaoProvisao.Aprovada },
    { text: "Rejeitada", value: _statusAprovacaoProvisao.Rejeitada },
    { text: "Todos", value: _statusProvisasaoPesquisa.Todos },
]

function PesquisaTermoQuitacaoFinanceiro() {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.NumeroDeTermo = PropertyEntity({ text: "Número do Termo: ", val: ko.observable(""), getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(true) });
    this.ProvisaoPendente = PropertyEntity({ text: "Provisão Pendente: ", val: ko.observable(_statusProvisasaoPesquisa.Todos), options: _statusProvisasaoPesquisaOpcoes, def: _statusProvisasaoPesquisa.Todos });
    this.SituacaoTermoQuitacao = PropertyEntity({ text: "Situação Termo Quitação: ", val: ko.observable(EnumSituacaoTermoQuitacaoFinanceiro.Todas), options: EnumSituacaoTermoQuitacaoFinanceiro.obterOpcoesPesquisa(), def: EnumSituacaoTermoQuitacaoFinanceiro.Todas });
    this.AprovacaoPendente = PropertyEntity({ text: "Aprovação Pendentes: ", val: ko.observable(_statusAprovacaoProvisao.Todas), options: _stautsAprovacaoProvisaoPesquisaOpcoes, def: _statusAprovacaoProvisao.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTermoQuitacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.CriarTermo = PropertyEntity({
        eventClick: criarTermoManual, type: types.event, text: "Criar Termo", idGrid: guid(), visible: ko.observable(true)
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
}

function TermoQuitacao() {
    this.NumeroTermo = PropertyEntity({ text: "Número Termo: ", val: ko.observable(0) })
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoTermoQuitacaoFinanceiro.AguardandoAprovacaoProvisao) })
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), enable: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", enable: ko.observable(false) });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Movimentacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PagamentosEDescontosViaCreditoEmConta = PropertyEntity({ text: "Pagamentos e descontos via crédito em conta: ", val: ko.observable("R$ 0,00") });
    this.PagamentosEDescontosViaConfiming = PropertyEntity({ text: "Pagamentos e descontos via confirming: ", val: ko.observable("R$ 0,00") });
    this.CreditoEmConta = PropertyEntity({ text: "Crédito em Conta: ", val: ko.observable("R$ 0,00") });
    this.TotalAdiantamento = PropertyEntity({ text: "Total Adiantamento: ", val: ko.observable("R$ 0,00") });
    this.NotasCompensadasAdiantamentos = PropertyEntity({ text: "Notas Compensadas Contra Adiantamentos: ", val: ko.observable("R$ 0,00") });
    this.SaldoAdiantamentoEmAberto = PropertyEntity({ text: "Saldo do adiantamento em aberto: ", val: ko.observable("R$ 0,00") });
    this.TotalGeralPagamento = PropertyEntity({ text: "Total Geral dos Pagamentos: ", val: ko.observable("R$ 0,00") });
    this.PossuiRegrasAprovacao = PropertyEntity({ val: ko.observable(false) });

    this.ExportarTermo = PropertyEntity({ text: 'Exportar Termo', eventClick: exportarTermoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.ExportarResumo = PropertyEntity({ text: 'Exportar Resumo', eventClick: exportarResumoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Avancar = PropertyEntity({ text: 'Gerar Termo', eventClick: criarTermoQuitacaoManual, type: types.event, idGrid: guid(), visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ eventClick: gerenciarAnexosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionaRemoverContaPagar = PropertyEntity({ eventClick: adicionaRemoverContaPagar, type: types.event, text: "Adicionar/Remover Conta a Pagar", visible: ko.observable(false), enable: ko.observable(true) });
}
function CRUDTermoQuitacao() {
    this.Cancelar = PropertyEntity({ text: 'Cancelar (Limpar Termo)', eventClick: cancelarTermoQuitacao, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Reprocessar = PropertyEntity({ text: 'Reprocessar', eventClick: reprocessarTermoQuitacao, type: types.event, idGrid: guid(), visible: ko.observable(false) });
}

//#endregion

//#region Funções de Carregamento
function loadTermoQuitacaoFinanceiro() {
    _pesquitaTermoQuitacao = new PesquisaTermoQuitacaoFinanceiro();
    KoBindings(_pesquitaTermoQuitacao, "knockoutPesquisaTermoQuitacaoFinanceiro");

    _termoQuitacao = new TermoQuitacao();
    KoBindings(_termoQuitacao, "TermoQuitacaoEtapa1");

    _crudTermoQuitacao = new CRUDTermoQuitacao();
    KoBindings(_crudTermoQuitacao, "knoutCRUDTermoQuitacao");

    new BuscarTransportadores(_pesquitaTermoQuitacao.Transportador, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    loadEtapasTermoQuitacaoFinanceiro();
    loadGridTermoQuitacaoFinanceiro();
    loadGridFiliais();
    loadAprovacaoProvisao();
    loadEtapaAprovacaoTransportador();
    loadCriacaoManualTermoQuitacao();
    loadAnexosTermoQuitacao();
    loadModalMovimentacoes();
}
//#endregion

//#region Funções Auxiliares


function loadGridTermoQuitacaoFinanceiro() {
    var editarRegistro = {
        descricao: "Editar",
        id: "clasEditar",
        evento: "onclick",
        metodo: editarRegistroClick,
        tamanho: "15",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editarRegistro]
    }

    var configExportacao = {
        url: "TermoQuitacaoFinanceiro/ExportarPesquisa",
        titulo: "Termo Quitação Financeiro"
    };

    _gridTermoQuitacao = new GridView(_pesquitaTermoQuitacao.Pesquisar.idGrid, "TermoQuitacaoFinanceiro/Pesquisa", _pesquitaTermoQuitacao, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridTermoQuitacao.CarregarGrid();
}

function exportarResumoClick() {
    let dados = new Object();
    $.each(_termoQuitacao, (i, prop) => {
        if (prop.type == types.entity)
            dados[i] = prop.codEntity();
        else
            dados[i] = prop.val();
    });

    executarDownload("TermoQuitacaoFinanceiro/ExportarResumo", dados);
}
function exportarTermoClick() {
    executarDownload("TermoQuitacaoFinanceiro/ExportarTermo", { Codigo: _termoQuitacao.Codigo.val() });
}
function criarTermoQuitacaoManual() {
    var dados = { Transportador: _termoQuitacao.Transportador.codEntity(), DataInicial: _termoQuitacao.DataInicial.val(), DataFinal: _termoQuitacao.DataFinal.val() };
    executarReST("TermoQuitacaoFinanceiro/GerarTermoManual", dados, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo de Quitação manualmente gerado");
        EnviarArquivosAnexadosTermoQuitacao(arg.Data.Codigo);
        _gridTermoQuitacao.CarregarGrid();
        cancelarTermoQuitacao();
        FadeTermoQuitacaoDetalhes();
    })
}

function loadGridFiliais() {
    _gridFiliais = CriGridBasicFiliais(_termoQuitacao.Filiais.idGrid);
    _gridFiliais.CarregarGrid([]);
}
function CriGridBasicFiliais(idGrid) {

    var header = [
        { data: "CodigoIntegracao", title: "Código Integração", width: "15%", className: "text-align-center" },
        { data: "CNPJ", title: "CNPJ", width: "15%", className: "text-align-center" },
        { data: "Cidade", title: "Cidade", width: "15%", className: "text-align-center" },
        { data: "UF", title: "UF", width: "15%", className: "text-align-center" },
    ];

    return new BasicDataTable(idGrid, header, null);
}


function recarregarGridFiliais() {
    let listaFiliais = _termoQuitacao.Filiais.val() || [];
    _gridFiliais.CarregarGrid(listaFiliais);
}

function criarTermoManual() {
    Global.abrirModal("divModalNovoTermo");
    cancelarTermoQuitacao();
    FadeTermoQuitacaoDetalhes();
}

function editarRegistroClick(e) {
    _termoQuitacao.Codigo.val(e.Codigo);
    BuscarPorCodigo(_termoQuitacao, "TermoQuitacaoFinanceiro/BuscarPorCodigo", (arg) => {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Message);

        _aprovacaoProvisao.Codigo.val(_termoQuitacao.Codigo.val());
        _aprovacaoTransportadorTermo.Codigo.val(_termoQuitacao.Codigo.val());

        recarregarGridFiliais();
        setarEtapasSetarEtapaTermoQuitacao();

        _gridAutorizacoesProvisao.CarregarGrid();

        detalharAutorizacaoProvisaoPendenteClick();
        loadAprovacoesTransportador();
        FadeTermoQuitacaoDetalhes(true);

        if (!_termoQuitacao.PossuiRegrasAprovacao.val())
            _crudTermoQuitacao.Reprocessar.visible(true);

        _anexosTermoQuitacao.Anexos.val(arg.Data.Anexos);
        _modalMovimentacoesContaPagar.Movimentacoes.val(_termoQuitacao.Movimentacoes.val());
        CarregarDadosGridMovimentacoes();
        ObterDetalhesGeraisAprovacao()
        _termoQuitacao.AdicionaRemoverContaPagar.visible(_termoQuitacao.Situacao.val() == EnumSituacaoTermoQuitacaoFinanceiro.RejeitadoTransportador);

    });
}
function reprocessarTermoQuitacao(e) {
    executarReST("TermoQuitacaoFinanceiro/ReprocessarRegras", { Codigo: _termoQuitacao.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        cancelarTermoQuitacao();
        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    })
}
function cancelarTermoQuitacao() {
    LimparTodosCamposTermoQuitacao();
    _gridFiliais.CarregarGrid([]);
    FadeTermoQuitacaoDetalhes();
}

function FadeTermoQuitacaoDetalhes(fade = false) {
    if (!fade) {
        $("#cardTermo").removeClass("cardTermoShow");
        $("#cardTermo").addClass("cardTermoHide");
        return;
    }
    $("#cardTermo").removeClass("cardTermoHide");
    $("#cardTermo").addClass("cardTermoShow");
}

function LimparTodosCamposTermoQuitacao() {
    LimparCampos(_termoQuitacao);
    LimparCampos(_aprovacaoProvisao);
    LimparCampos(_aprovacaoTransportadorTermo);
    limparTermoQuitacaoAnexos();
}

function adicionaRemoverContaPagar() {
    Global.abrirModal("divModalAdicionarMovimentacoes");
}
//#endregion
