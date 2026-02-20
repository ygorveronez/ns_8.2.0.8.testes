/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../..//wwwroot/js/Global/Grid.js" />
/// <reference path="../../..//wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../..//wwwroot/js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoNotasGestaoDevolucao.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoNotasFiscais.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoGestaoDevolucao.js" />
/// <reference path="../../../wwwroot/js/Global/Auditoria.js" />

//#region Objetos Globais do Arquivo
const MODO_NOTAS_FISCAIS = 1;
const MODO_DEVOLUCOES = 2;

var _pesquisaGestaoDevolucao;
var _gridGestaoDevolucaoNFPallet;
var _gridGestaoDevolucaoDevolucoes;
var _atendimentosGestaoDevolucao;
var _gridAtendimentosGestaoDevolucao;
var _gestaoDevolucaoPallets;
var _detalhesNota;
var _gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador;
var _solicitacaoCancelamento;
var _detalhesCancelamento;
var _GestaoDevolucaoAprovarCancelamento;
var _GestaoDevolucaoReprovarCancelamento;
// #endregion Objetos Globais do Arquivo

//#region Classes
var PesquisaGestaoDevolucao = function () {
    this.ModoDeUso = PropertyEntity({ val: ko.observable(MODO_NOTAS_FISCAIS) });
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Carga = PropertyEntity({ val: ko.observable(""), text: "Carga de Origem", visible: ko.observable(true) });
    this.CargaDevolucao = PropertyEntity({ val: ko.observable(""), text: "Carga de Devolução", visible: ko.observable(true) });
    this.NFOrigem = PropertyEntity({ val: ko.observable(""), text: "NF de Origem", visible: ko.observable(true) });
    this.NFDevolucao = PropertyEntity({ val: ko.observable(""), text: "NF de Devolução", visible: ko.observable(true) });
    this.DataEmissaoNFInicial = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Emissão da NF Inicial", visible: ko.observable(true) });
    this.DataEmissaoNFFinal = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "Emissão da NF Final", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DevolucaoGerada = PropertyEntity({ text: "Devolução Gerada?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
    this.TipoNotasGestaoDevolucao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Tipo de Nota Fiscal", options: EnumTipoNotasGestaoDevolucao.obterOpcoes(), visible: ko.computed(() => this.ModoDeUso.val() === MODO_DEVOLUCOES) });
    this.TipoNotasFiscais = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Tipo de Nota Fiscal", options: EnumTipoNotasFiscais.obterOpcoes(), visible: ko.computed(() => this.ModoDeUso.val() === MODO_NOTAS_FISCAIS) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Situação Devolução", options: EnumSituacaoGestaoDevolucao.obterOpcoes(), visible: ko.observable(true) });
    this.EtapaAtual = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Etapa Atual", options: EnumEtapaGestaoDevolucao.obterOpcoes(), visible: ko.observable(true) });
    this.TipoFluxoGestaoDevolucao = PropertyEntity({ text: "Fluxo de Devolução", val: ko.observable(EnumTipoFluxoGestaoDevolucao.Todos), options: EnumTipoFluxoGestaoDevolucao.obterOpcoesPesquisa(), def: EnumTipoFluxoGestaoDevolucao.Todos });
    this.EscritorioVendas = PropertyEntity({ val: ko.observable(""), text: "Escritório de Vendas", visible: ko.observable(true) });
    this.EquipeVendas = PropertyEntity({ val: ko.observable(""), text: "Equipe de Vendas", visible: ko.observable(true) });
    this.TipoGestaoDevolucao = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Tipo de Devolução", options: EnumTipoGestaoDevolucao.obterOpcoes(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), text: "Cliente", issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.GestaoDevolucao,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.GestaoDevolucao, _pesquisaGestaoDevolucao) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            loadGridGestaoDevolucaoDevolucoes();
            controlarVisibilidadeGrids();
            loadGridGestaoDevolucaoPallets();
            loadInformacoesDevolucao();
            $('.aviso-filtragem-dados').hide();
            Global.fecharModal('divModalFiltroPesquisaGestaoDevolucao');
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var GestaoDevolucaoSolicitacaoCancelamento = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.ObservacaoCancelamento = PropertyEntity({ text: "Adicionar Observação", val: ko.observable(""), getType: typesKnockout.string });
}

var DetalhesCancelamentoViewModel = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable() });
    this.NFOrigem = PropertyEntity({ val: ko.observable() });
    this.NFDevolucao = PropertyEntity({ val: ko.observable() });
    this.CargaDevolucao = PropertyEntity({ val: ko.observable() });
    this.CargaOrigem = PropertyEntity({ val: ko.observable() });
    this.Origem = PropertyEntity({ val: ko.observable() });
    this.Filial = PropertyEntity({ val: ko.observable() });
    this.Transportador = PropertyEntity({ val: ko.observable() });
    this.Tomadores = PropertyEntity({ val: ko.observable() });
    this.EtapaAtual = PropertyEntity({ val: ko.observable() });
    this.Aprovado = PropertyEntity({ val: ko.observable() });
    this.Laudo = PropertyEntity({ val: ko.observable() });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable() });
    this.PendenciaFinanceira = PropertyEntity({ val: ko.observable() });
    this.Atendimentos = PropertyEntity({ val: ko.observable() });
    this.ObservacaoCancelamento = PropertyEntity({ val: ko.observable() });
    this.ConfirmacaoCancelamento = PropertyEntity({ text: "Aprovar Cancelamento", eventClick: abrirModalAprovacaoCancelamento, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
    this.ReprovacaoCancelamento = PropertyEntity({ text: "Reprovar Cancelamento", eventClick: abrirModalReprovacaoCancelamento, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
};

var InformacoesDevolucao = function () {
    this.CodigoDevolucao = PropertyEntity({ val: ko.observable(0) });
    this.NumeroNotaFiscal = PropertyEntity({ val: ko.observable(0) });
    this.DataEmissao = PropertyEntity({ val: ko.observable("") });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable("") });
};

var GestaoDevolucaoAprovarCancelamento = function () {
    this.CodigoDevolucao = PropertyEntity({ val: ko.observable(0) });
    this.ConfirmarAprovacaoCancelamento = PropertyEntity({ text: "Sim", eventClick: AprovarCancelamento, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
};

var GestaoDevolucaoReprovarCancelamento = function () {
    this.CodigoDevolucao = PropertyEntity({ val: ko.observable(0) });
    this.ConfirmarReprovacaoCancelamento = PropertyEntity({ text: "Sim", eventClick: ReprovarCancelamento, type: types.event, visible: ko.observable(true), enable: ko.observable(true) });
};

var AtendimentosGestaoDevolucao = function () {
    this.AtendimentosGestaoDevolucao = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
}

var GestaoDevolucaoPallets = function () {
    this.GerarDevolucaoPallet = PropertyEntity({ text: "Gerar Devolução", eventClick: gerarDevolucaoPalletClick, type: types.event, visible: ko.observable(false), enable: ko.observable(true) });
}

var DetalhesNota = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), text: "", visible: ko.observable(true) });
    this.Chave = PropertyEntity({ val: ko.observable(""), text: "CHAVE", visible: ko.observable(true) });
    this.Serie = PropertyEntity({ val: ko.observable(""), text: "SÉRIE", visible: ko.observable(true) });
    this.StringDataEmissao = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), text: "DATA DE EMISSÃO NF", visible: ko.observable(true) });
    this.DescricaoTipoNF = PropertyEntity({ val: ko.observable(""), text: "TIPO NF", visible: ko.observable(true) });
    this.NumeroNFDevolucao = PropertyEntity({ val: ko.observable(0), text: "NÚMERO DEVOLUÇÃO", visible: ko.observable(true) });
    this.DescricaoDevolucaoGerada = PropertyEntity({ text: "GEROU DEVOLUÇÃO?", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });
    this.DescricaoResponsavelPallet = PropertyEntity({ text: "PENDÊNCIA", val: ko.observable(""), visible: ko.observable(true) });
    this.PrazoGeracaoDevolucao = PropertyEntity({ text: "PRAZO GERACAO DEVOLUCAO", val: ko.observable(""), visible: ko.observable(true) });
    this.NumerosAtendimentos = PropertyEntity({ text: "NÚMERO ATENDIMENTOS", val: ko.observable("") });
    this.MotivoDevolucao = PropertyEntity({ text: "MOTIVO DEVOLUÇÕES", val: ko.observable("") });
    this.DevolucaoTotal = PropertyEntity({ text: "TIPO DA DEVOLUÇÃO", val: ko.observable("") });
    this.NomeCNPJFilial = PropertyEntity({ val: ko.observable(0), text: "FILIAL", visible: ko.observable(true) });
    this.NomeCNPJTransportador = PropertyEntity({ val: ko.observable(0), text: "TRANSPORTADOR", visible: ko.observable(true) });
    this.NomeCNPJCliente = PropertyEntity({ val: ko.observable(""), text: "DESTINATÁRIO", visible: ko.observable(true) });
};

var GetaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador = function (quantidadeNotas) {
    this.NotasSelecionadas = PropertyEntity({ val: ko.observable("") });
    this.QuantidadeNotasSelecionadas = PropertyEntity({ text: "Notas Selecionadas", val: ko.observable(0), enable: ko.observable(false) });
    this.TipoFluxoDeDevolucao = PropertyEntity({ text: "Fluxo de Devolução", val: ko.observable(EnumTipoFluxoGestaoDevolucao.Normal), options: ko.observable(EnumTipoFluxoGestaoDevolucao.obterOpcoes()), def: EnumTipoFluxoGestaoDevolucao.Normal, enable: ko.observable(true) });

    this.GerarDevolucaoFluxoEmbarcador = PropertyEntity({ text: "Gerar", eventClick: gerarDevolucaoFluxoEmbarcador, type: types.event, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", eventClick: fecharModalGeracaoDevolucaoFluxoEmbarcador, type: types.event, visible: ko.observable(false), enable: ko.observable(true) });
};
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucao() {

    _pesquisaGestaoDevolucao = new PesquisaGestaoDevolucao();
    KoBindings(_pesquisaGestaoDevolucao, "knockoutPesquisaGestaoDevolucao", false, _pesquisaGestaoDevolucao.Pesquisar.id);

    _gestaoDevolucaoPallets = new GestaoDevolucaoPallets();
    KoBindings(_gestaoDevolucaoPallets, "grid-devolucoes-pallet");

    _detalhesNota = new DetalhesNota();
    KoBindings(_detalhesNota, "knoutDetalhesNota");

    $('.aviso-filtragem-dados').show();

    loadEtapasGestaoDevolucao();
    loadChamado();

    BuscarFilial(_pesquisaGestaoDevolucao.Filial);
    BuscarTransportadores(_pesquisaGestaoDevolucao.Transportador);

    BuscarClientes(_pesquisaGestaoDevolucao.Cliente);

    loadAtendimentosGestaoDevolucao();
    loadSolicitacaoCancelamentoGestaoDevolucao();
    loadConexaoSignalRGestaoDevolucao();
}

function loadGridGestaoDevolucaoDevolucoes() {
    let escolherTipoDevolucao = {
        descricao: "Definir Tipo de Devolução",
        id: guid(),
        evento: "onclick",
        metodo: definirTipoDevolucao,
        tamanho: "10",
        icone: "",
        visibilidade: visibilidadeDefinirTipoDevolucao
    };

    let auditoria = {
        descricao: "Auditoria",
        id: guid(),
        evento: "onclick",
        metodo: OpcaoAuditoria("GestaoDevolucao"),
        tamanho: "10",
        icone: "",
    };

    let atendimentos = {
        descricao: "Atendimentos",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalAtendimentosGestaoDevolucao,
        tamanho: "10",
        icone: "",
        visibilidade: visibilidadeAtendimentos
    };

    let solicitacaoCancelamento = {
        descricao: "Solicitar Cancelamento",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalSolicitacaoCancelamentoGestaoDevolucao,
        tamanho: "10",
        icone: "",
        visibilidade: visibilidadeSolicitacaoCancelamento
    };

    let detalhesCancelamento = {
        descricao: "Detalhes Cancelamento",
        id: guid(),
        evento: "onclick",
        metodo: exibirModalDetalhesCancelamento,
        tamanho: "10",
        icone: "",
        visibilidade: visibilidadeDetalhesCancelamento
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [escolherTipoDevolucao, atendimentos, solicitacaoCancelamento, detalhesCancelamento]
    };

    if (VisibilidadeOpcaoAuditoria())
        menuOpcoes.opcoes.push(auditoria);

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: callbackNaoSelecionadoDevolucao,
        callbackSelecionado: callbackSelecionadoDevolucao,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: true
    };

    var configuracaoExportacao = {
        url: "GestaoDevolucao/ExportarPesquisaDevolucoes",
        titulo: "Devoluções"
    };

    HeaderAuditoria("GestaoDevolucao", _pesquisaGestaoDevolucao);

    _gridGestaoDevolucaoDevolucoes = new GridView("grid-gestao-devolucao-devolucoes", "GestaoDevolucao/PesquisaDevolucoes", _pesquisaGestaoDevolucao, menuOpcoes, null, 10, null, null, null, multiplaEscolha, null, null, configuracaoExportacao, null, null, null, callbackColumnGestaoDevolucaoDevolucoes);
    _gridGestaoDevolucaoDevolucoes.SetPermitirEdicaoColunas(true);
    _gridGestaoDevolucaoDevolucoes.SetSalvarPreferenciasGrid(true);
    _gridGestaoDevolucaoDevolucoes.CarregarGrid();
}

function loadGridGestaoDevolucaoPallets() {
    let detalhesNota = {
        descricao: "Detalhes da Nota",
        id: guid(),
        evento: "onclick",
        metodo: detalhesNotaClick,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: callbackNaoSelecionadoDevolucaoPallet,
        callbackSelecionado: callbackSelecionadoDevolucaoPallet,
        callbackSelecionarTodos: null,
        somenteLeitura: false,
        permitirSelecionarSomenteUmRegistro: false
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhesNota] };

    var configuracaoExportacao = {
        url: "GestaoDevolucao/ExportarGridPesquisaNotaFiscal",
        titulo: "Notas fiscais de devolução"
    };

    _gridGestaoDevolucaoNFPallet = new GridView("grid-gestao-devolucao-pallet", "GestaoDevolucao/PesquisaNotasFiscaisPallet", _pesquisaGestaoDevolucao, menuOpcoes, null, 15, null, null, null, multiplaEscolha, null, null, configuracaoExportacao, null, null, null, callbackColumnGestaoDevolucaoPallet);
    _gridGestaoDevolucaoNFPallet.SetPermitirEdicaoColunas(true);
    _gridGestaoDevolucaoNFPallet.SetSalvarPreferenciasGrid(true);
    _gridGestaoDevolucaoNFPallet.CarregarGrid();
}

function loadInformacoesDevolucao() {
    _informacoesDevolucao = new InformacoesDevolucao();
}

function loadAtendimentosGestaoDevolucao() {
    _atendimentosGestaoDevolucao = new AtendimentosGestaoDevolucao();
    KoBindings(_atendimentosGestaoDevolucao, "knockoutAtendimentosGestaoDevolucao");
}

function loadSolicitacaoCancelamentoGestaoDevolucao() {
    _solicitacaoCancelamento = new GestaoDevolucaoSolicitacaoCancelamento();
    KoBindings(_solicitacaoCancelamento, "knockoutSolicitacaoCancelamento");
}

function loadDetalhesCancelamentoGestaoDevolucao() {
    _detalhesCancelamento = new DetalhesCancelamentoViewModel();
    KoBindings(_detalhesCancelamento, "knockoutDetalhesCancelamento");
}

function loadGestaoDevolucaoAprovarCancelamento() {
    _GestaoDevolucaoAprovarCancelamento = new GestaoDevolucaoAprovarCancelamento();
    KoBindings(_GestaoDevolucaoAprovarCancelamento, "knockoutAprovacaoCancelamento");
}
function loadGestaoDevolucaoReprovarCancelamento() {
    _GestaoDevolucaoReprovarCancelamento = new GestaoDevolucaoReprovarCancelamento();
    KoBindings(_GestaoDevolucaoReprovarCancelamento, "knockoutReprovacaoCancelamento");
}

function exibirModalAtendimentosGestaoDevolucao(registroSelecionado) {
    executarReST("GestaoDevolucao/BuscarAtendimentos", { CodigoGestaoDevolucao: registroSelecionado.Codigo }, function (arg) {
        if (arg.Success) {
            carregarGridAtendimentosGestaoDevolucao(arg.Data);
            Global.abrirModal("divModalVisualizarAtendimentosGestaoDevolucao");
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resouces.Gerais.Geral.Falha, 'Falha ao consultar atendimentos da devolução.');
    });
};

function salvarSolicitacaoCancelamento() {
    const codigo = _solicitacaoCancelamento.CodigoGestaoDevolucao.val();
    const observacao = _solicitacaoCancelamento.ObservacaoCancelamento.val();

    if (!observacao) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A observação é obrigatória!");
        return;
    }

    executarReST("GestaoDevolucao/SolicitarCancelamento", { CodigoGestaoDevolucao: codigo, ObservacaoCancelamento: observacao }, function (response) {
        if (response.Success) {
            if (response.Data) {
                exibirMensagem(tipoMensagem.ok, "Solicitação efetuada com sucesso!", response.Msg);

                Global.fecharModal("divModalSolicitacaoCancelamento");
                _gridGestaoDevolucaoDevolucoes.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, response.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Msg);
    }
    );
}

function exibirModalSolicitacaoCancelamentoGestaoDevolucao(registroSelecionado) {

    _solicitacaoCancelamento.CodigoGestaoDevolucao.val(registroSelecionado.Codigo);
    _solicitacaoCancelamento.ObservacaoCancelamento.val("");
    Global.abrirModal("divModalSolicitacaoCancelamento");
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {

        if (retorno.Codigo !== 0) {
            e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
            e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

            PreencherJsonFiltroPesquisa(_pesquisaGestaoDevolucao, retorno.Dados);
        }
    });

    buscaFiltros.AbrirBusca();
}

function visualizarDevolucaoClick(registroSelecionado) {
    mostrarGridDevolucoes();
    $('#grid-gestao-devolucao-devolucoes' + ' #' + registroSelecionado.CodigoDevolucao).addClass('selected');
    $('#grid-gestao-devolucao-devolucoes' + ' #' + registroSelecionado.CodigoDevolucao).click();

};

function callbackSelecionadoDevolucao(argumentoNulo, registroSelecionado) {

    preencherInformacoesDevolucao(registroSelecionado);
    _pesquisaGestaoDevolucao.Codigo.val(registroSelecionado.Codigo);
    montarEtapasDevolucao(registroSelecionado);
}

function callbackNaoSelecionadoDevolucao() {
    _informacoesDevolucao.CodigoDevolucao.val(0);
    limparEtapasDevolucao();
}

function callbackSelecionadoDevolucaoPallet(argumentoNulo, registroSelecionado) {
    let listaSelecionados = _gridGestaoDevolucaoNFPallet.ObterMultiplosSelecionados();

    if (listaSelecionados.length > 0) {
        _gestaoDevolucaoPallets.GerarDevolucaoPallet.visible(true);
    }

    if (registroSelecionado.GerouDevolucao == true) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Devolução já gerada para essa nota.");
        document.getElementById(registroSelecionado.Codigo.toString()).click();
        return;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        let possuiTipoDiferente = listaSelecionados.some(item => item.TipoTomadorDescricao !== registroSelecionado.TipoTomadorDescricao);

        if (possuiTipoDiferente) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Todos os itens selecionados devem ter o mesmo Tipo de Tomador.");
            document.getElementById(registroSelecionado.Codigo.toString()).click();
            return;
        }
    }

    const quantidadeNFsPallet = listaSelecionados.filter(
        item => item.TipoNF === EnumTipoNotasGestaoDevolucao.Pallet
    ).length;


    if (quantidadeNFsPallet > 0 && quantidadeNFsPallet !== listaSelecionados.length) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", `Não é possível gerar uma devolução com uma ou mais NF-e do tipo: ${EnumTipoNotasGestaoDevolucao.obterDescricao(EnumTipoNotasGestaoDevolucao.Pallet)} em conjunto com outras de tipos diferentes.`);
        document.getElementById(registroSelecionado.Codigo.toString()).click();
    }
}

function callbackNaoSelecionadoDevolucaoPallet(argumentoNulo, registroNaoSelecionado) {
    if (_gridGestaoDevolucaoNFPallet.ObterMultiplosSelecionados().length == 0) {
        _gestaoDevolucaoPallets.GerarDevolucaoPallet.visible(false);
    }
}

function gerarDevolucaoPalletClick() {
    let notasSelecionadas = _gridGestaoDevolucaoNFPallet.ObterMultiplosSelecionados();
    let jsonNotasSelecionadas = JSON.stringify(notasSelecionadas);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador = new GetaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador();
        KoBindings(_gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador, "knockoutModalGeracaoDevolucaoFluxoEmbarcador");
        _gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador.NotasSelecionadas.val(jsonNotasSelecionadas);
        _gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador.QuantidadeNotasSelecionadas.val(notasSelecionadas.length + " nota(s) selecionada(s)");
        Global.abrirModal("divModalGeracaoDevolucaoFluxoEmbarcador");
    }
    else
        exibirConfirmacao("Confirmação", "Realmente deseja gerar a devolução das notas selecionadas?", function () { gerarDevolucao(jsonNotasSelecionadas) });
}

function gerarDevolucaoFluxoEmbarcador() {
    gerarDevolucao(_gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador.NotasSelecionadas.val(), _gestaoDevolucaoModalGeracaoDevolucaoFluxoEmbarcador.TipoFluxoDeDevolucao.val());
}
function fecharModalGeracaoDevolucaoFluxoEmbarcador() {
    Global.fecharModal("divModalGeracaoDevolucaoFluxoEmbarcador");
}

function gerarDevolucao(notasSelecionadas, tipoFluxoDevolucao) {
    executarReST("GestaoDevolucao/GerarGestaoDevolucao", { NotasSelecionadas: notasSelecionadas, TipoFluxoDevolucao: tipoFluxoDevolucao }, function (r) {
        if (r.Success) {
            _gridGestaoDevolucaoNFPallet.CarregarGrid();
            _gridGestaoDevolucaoDevolucoes.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Msg);
            fecharModalGeracaoDevolucaoFluxoEmbarcador();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function exibirModalAtendimentosGestaoDevolucao(registroSelecionado) {
    executarReST("GestaoDevolucao/BuscarAtendimentos", { CodigoGestaoDevolucao: registroSelecionado.Codigo }, function (arg) {
        if (arg.Success) {
            carregarGridAtendimentosGestaoDevolucao(arg.Data);
            Global.abrirModal("divModalVisualizarAtendimentosGestaoDevolucao");
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resouces.Gerais.Geral.Falha, 'Falha ao consultar atendimentos da devolução.');
    });
};

function exibirModalDetalhesCancelamento(registroSelecionado) {

    executarReST("GestaoDevolucao/BuscarDadosAnaliseCancelamento", { CodigoGestaoDevolucao: registroSelecionado.Codigo }, function (response) {
        if (response.Success) {

            _detalhesCancelamento = new DetalhesCancelamentoViewModel();
            PreencherObjetoKnout(_detalhesCancelamento, response);

            _detalhesCancelamento.CodigoGestaoDevolucao.val(registroSelecionado.Codigo);

            KoBindings(_detalhesCancelamento, "knockoutDetalhesCancelamento");

            Global.abrirModal("divModalAnaliseCancelamento");


        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Message);
        }
    });
}

function AprovarCancelamento(gestaoDevolucaoAprovarCancelamento) {

    executarReST("GestaoDevolucao/AprovarCancelamento", { CodigoGestaoDevolucao: gestaoDevolucaoAprovarCancelamento.CodigoDevolucao.val() }, function (response) {
        if (response.Success) {

            exibirMensagem(tipoMensagem.Success, Localization.Resources.Gerais.Geral.Sucesso, "Cancelamento Devolução aprovado com sucesso!");
            Global.fecharModal("modalConfirmacaoCancelamento");
            Global.fecharModal("divModalAnaliseCancelamento");
            _gridGestaoDevolucaoDevolucoes.CarregarGrid();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Message);
        }
    });
}

function ReprovarCancelamento(gestaoDevolucaoReprovarCancelamento) {

    executarReST("GestaoDevolucao/ReprovarCancelamento", { CodigoGestaoDevolucao: gestaoDevolucaoReprovarCancelamento.CodigoDevolucao.val() }, function (response) {
        if (response.Success) {

            exibirMensagem(tipoMensagem.Success, Localization.Resources.Gerais.Geral.Sucesso, "Devolução reprovada com sucesso!");
            Global.fecharModal("modalReprovacaoCancelamento");
            Global.fecharModal("divModalAnaliseCancelamento");
            _gridGestaoDevolucaoDevolucoes.CarregarGrid();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, response.Message);
        }
    });
}


function abrirModalReprovacaoCancelamento(detalhesCancelamento) {
    loadGestaoDevolucaoReprovarCancelamento()

    _GestaoDevolucaoReprovarCancelamento.CodigoDevolucao.val(detalhesCancelamento.CodigoGestaoDevolucao.val());

    Global.abrirModal("modalReprovacaoCancelamento");
}

function abrirModalAprovacaoCancelamento(detalhesCancelamento) {
    loadGestaoDevolucaoAprovarCancelamento()

    _GestaoDevolucaoAprovarCancelamento.CodigoDevolucao.val(detalhesCancelamento.CodigoGestaoDevolucao.val());

    Global.abrirModal("modalConfirmacaoCancelamento");
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function ExibirFiltros() {
    Global.abrirModal('divModalFiltroPesquisaGestaoDevolucao');
}

function mostrarGridDevolucoes() {
    $('#grid-devolucoes-pallet').removeClass('active');
    $('#grid-devolucoes').addClass('active');
    $('#texto-botao-visualizao-grids').text('Devoluções');
    if (_gridGestaoDevolucaoDevolucoes)
        controlarVisibilidadeGrids();

    _pesquisaGestaoDevolucao.ModoDeUso.val(MODO_DEVOLUCOES);
}

function mostrarGridNotasFiscaisPallet() {
    $('#grid-devolucoes').removeClass('active');
    $('#grid-devolucoes-pallet').addClass('active');
    $('#texto-botao-visualizao-grids').text('Notas Fiscais');
    if (_gridGestaoDevolucaoNFPallet)
        controlarVisibilidadeGrids();

    _pesquisaGestaoDevolucao.ModoDeUso.val(MODO_NOTAS_FISCAIS);
}
// #endregion Funções Públicas

// #region Funções Privadas
function definirTipoDevolucao(registro) {
    _informacoesDevolucao.CodigoDevolucao.val(registro.Codigo);

    loadDefinicaoTipoDevolucao();
    montarEtapasDevolucao(registro);
}

function callbackColumnGestaoDevolucaoPallet(cabecalho, valorColuna, row) {
    if (cabecalho.name == "GerouDevolucao")
        return obterHtmlColunaGerouDevolucaoPallet(valorColuna);
    if (cabecalho.name == "PrazoGeracaoDevolucao")
        return obterHtmlColunaPrazoGeracaoDevolucao(valorColuna, row);
}

function callbackColumnGestaoDevolucaoDevolucoes(cabecalho, valorColuna, row) {
    if (cabecalho.name == "Laudo")
        return obterHtmlColunaSimNao(valorColuna);

    if (cabecalho.name == "Aprovado")
        return obterHtmlColunaSimNao(valorColuna);

    if (cabecalho.name == "TipoDevolucaoDescricao")
        return obterHtmlColunaTipoDevolucao(valorColuna);

    if (cabecalho.name == "PrazoEscolhaTipoDevolucao")
        return obterHtmlColunaPrazoEscolhaTipoDevolucao(valorColuna, row);

    if (cabecalho.name == "ControleFinalizacaoDevolucaoDescricao")
        return obterHtmlColunaSimNao(valorColuna, row);
}

function obterHtmlColunaSimNao(valorColuna) {
    let cor = valorColuna == "Sim" ? '#1F7C4A' : '#FF4E4E';

    return '<span style="color:' + cor + '" class="text-uppercase">' + valorColuna + '</span>';
}

function obterHtmlColunaTipoDevolucao(valorColuna) {
    if (valorColuna == "Definir")
        return '<span style="color: #38AAE1" class="text-uppercase">' + valorColuna + '</span>';
}

function obterHtmlColunaGerouDevolucaoPallet(valorColuna) {
    let cor = valorColuna == true ? '#1F7C4A' : '#FF4E4E';
    let span = valorColuna == true ? 'SIM' : 'NÃO';

    return '<span style="color:' + cor + '">' + span + '</span>';
}

function obterHtmlColunaPrazoEscolhaTipoDevolucao(valorColuna, row) {
    let idRelogio = row.Codigo;
    setTimeout(() => {
        AtivarRelogioPrazoGestaoDevolucao(valorColuna, idRelogio, "#relogioPrazoEscolhaTipoDevolucao");
    }, 100);

    return '<span class="mt-4 col-6" id="relogioPrazoEscolhaTipoDevolucao' + idRelogio + '"></span>';
}

function obterHtmlColunaPrazoGeracaoDevolucao(valorColuna, row) {
    let idRelogio = row.Codigo;

    if (valorColuna != "") {
        setTimeout(() => {
            AtivarRelogioPrazoGestaoDevolucao(valorColuna, idRelogio, "#relogioPrazoGeracaoDevolucao");
        }, 100);
    } else return '<span class="mt-4 col-6"> - </span > ';

    return '<span class="mt-4 col-6" id="relogioPrazoGeracaoDevolucao' + idRelogio + '"></span>';
}

function visibilidadeDefinirTipoDevolucao(registro) {
    if (registro.TipoDevolucao == 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor)
        return true;

    return false;
}

function visibilidadeAtendimentos(registroSelecionado) {
    return registroSelecionado.Atendimentos != ''
}

function visibilidadeDetalhesCancelamento(registroSelecionado) {
    return registroSelecionado.SituacaoDevolucao === 1 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador
}

function visibilidadeSolicitacaoCancelamento(registroSelecionado) {
    return registroSelecionado.SituacaoDevolucao === 0;
}

function visibilidadeFluxoDevolucao(registroSelecionado) {
    if (registroSelecionado.EmDevolucao)
        return true;
    else
        return false;
}

function controlarVisibilidadeGrids() {
    if ($('#grid-notas-fiscais').hasClass('active')) {
        $('#grid-notas-fiscais').show();
        $('#grid-devolucoes').hide();
        $("#panel-etapas-devolucao").hide();
        $("#aviso-selecionar-registro").hide();
        $("#grid-devolucoes-pallet").hide();
    }
    if ($('#grid-devolucoes').hasClass('active')) {
        $('#grid-devolucoes').show();
        $('#grid-notas-fiscais').hide();
        $("#panel-etapas-devolucao").hide();
        $("#grid-devolucoes-pallet").hide();

        $('#aviso-selecionar-registro').show();
    }
    if ($('#grid-devolucoes-pallet').hasClass('active')) {
        $('#grid-devolucoes-pallet').show();
        $('#grid-devolucoes').hide();
        $('#grid-notas-fiscais').hide();
        $("#panel-etapas-devolucao").hide();

        $('#aviso-selecionar-registro').hide();
    }

    $('#container-principal').hide();
}

function preencherInformacoesDevolucao(registroSelecionado) {
    _informacoesDevolucao.CodigoDevolucao.val(registroSelecionado.Codigo);
    _informacoesDevolucao.NumeroNotaFiscal.val(registroSelecionado.NFDevolucao);
    _informacoesDevolucao.DataEmissao.val(registroSelecionado.DataEmissaoNFDevolucao);
    _informacoesDevolucao.TipoDevolucao.val(registroSelecionado.TipoDevolucaoDescricao);
}
function buscarInformacoesDevolucao(etapa) {
    return {
        CodigoGestaoDevolucao: _informacoesDevolucao.CodigoDevolucao.val(),
        NumeroNotaFiscal: _informacoesDevolucao.NumeroNotaFiscal.val(),
        DataEmissao: _informacoesDevolucao.DataEmissao.val(),
        TipoDevolucao: _informacoesDevolucao.TipoDevolucao.val(),
        EtapaGestaoDevolucao: etapa ? etapa.etapa : 0,
    };
}

function PermiteAuditar() {
    return _CONFIGURACAO_TMS.PermiteAuditar;
}

function VisibilidadeOpcaoAuditoria() {
    return PermiteAuditar();
}

function carregarGridAtendimentosGestaoDevolucao(dados) {
    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: verDetalhesAtendimentoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.VerDetalhes, tamanho: 20, opcoes: [opcaoDetalhes] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "NumeroNotaFiscal", title: "Nota", width: "10%", className: "text-align-left" },
        { data: "NumeroChamado", title: "Atendimento", width: "10%", className: "text-align-left" },
        { data: "DataCriacao", title: "Data de criação", width: "20%", className: "text-align-left" },
        { data: "MotivoChamadoDescricao", title: "Motivo", width: "50%", className: "text-align-left" },
        { data: "SituacaoDescricao", title: "Situação", width: "20%", className: "text-align-left" },
    ];

    _gridAtendimentosGestaoDevolucao = new BasicDataTable(_atendimentosGestaoDevolucao.AtendimentosGestaoDevolucao.idGrid, header, menuOpcoes, null);
    _gridAtendimentosGestaoDevolucao.CarregarGrid(dados);
}

function verDetalhesAtendimentoClick(e) {
    buscarChamadoPorCodigo(e.Codigo, function () {
        let $modalChamado = $("#divModalAtendimentoGestaoDevolucao");

        Global.abrirModal("divModalAtendimentoGestaoDevolucao");

        $modalChamado.one('hidden.bs.modal', function () {
            $(window).one('keyup', function (e) {
                if (e.keyCode == 27)
                    Global.fecharModal("divModalAtendimentoGestaoDevolucao");
            });
        });

        $(window).unbind('keyup');
        $(window).one('keyup', function (e) {
            if (e.keyCode == 27)
                Global.fecharModal("divModalAtendimentoGestaoDevolucao");
        });
    });
}

function AtivarRelogioPrazoGestaoDevolucao(dataLimiteTratativa, idRelogio, idDivRelogio) {
    let relogio = $(idDivRelogio + idRelogio);
    relogio.empty();

    relogio.text("Tempo limite atingido");

    let dataMoment = moment(dataLimiteTratativa, "DD/MM/YYYY HH:mm:ss");

    relogio
        .countdown(dataMoment.format("YYYY/MM/DD HH:mm:ss"), { elapse: false, precision: 1000 })
        .on('update.countdown', function (event) {
            let totalHoras = String(event.offset.totalDays).padStart(2, '0');
            let totalMinutos = String(event.offset.minutes).padStart(2, '0');
            let totalSegundos = String(event.offset.seconds).padStart(2, '0');
            $(this).text(totalHoras + "d " + totalMinutos + "min " + totalSegundos + "seg");
        })
        .on('finish.countdown', function (event) {
            $(this).text("Tempo limite atingido");
        });
}

function detalhesNotaClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.CodigoNF }

    executarReST("GestaoDevolucao/BuscarDetalhes", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                console.log(dados);
                console.log(retorno);
                PreencherObjetoKnout(_detalhesNota, retorno);

                abrirDetalhesNota();

                $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                    LimparCampos(_detalhesNota);
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function abrirDetalhesNota() {
    Global.abrirModal('divModalDetalhesNota');
}

// #endregion Funções Privadas