/// <reference path="../../../wwwroot/js/Global/Auditoria.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/ModeloFiltroPesquisa/ConfiguracaoModeloFiltroPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoPalletCliente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedidoGestaoPedido.js" />
/// <reference path="../../Enumeradores/EnumSituacaoRoteirizadorIntegracao.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="./GestaoPedidoDetalhesPedidoProdutos.js" />

// #region Objetos Globais do Arquivo

var _pesquisaGestaoPedido;
var _gridPedido;
var _detalhePedido;
var _botoes;
var _opcoes;
var _totalizadores;
var _legendaGestaoPedido;
var _buscaFilialParaTroca;
var _tiposPaletesClienteDetalhes = [];
var _gridIntegracaoGestaoPedido;
var _gridHistoricoIntegracaoGestaoPedido;
var _pesquisaHistoricoIntegracaoGestaoPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGestaoPedido = function () {
    var self = this;
    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().add(6, 'days').format("DD/MM/YYYY");

    this.DataInicio = PropertyEntity({ text: "Dt. Prev. entrega de", getType: typesKnockout.date, val: ko.observable(dataInicial), required: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: "Dt. Prev entrega até", dateRangeInit: this.DataInicio, getType: typesKnockout.date, val: ko.observable(dataFinal), required: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial", issue: 70, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaNumeroPedido = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "N° Pedido Embarcador", val: ko.observable(""), idBtnSearch: guid(), def: "", visible: ko.observable(true) });
    this.NumeroSessao = PropertyEntity({ text: "N° Sessão", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCarregamento = PropertyEntity({ text: "N° Carregamento", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação", issue: 121, visible: ko.observable(true), idBtnSearch: guid(), cssClass: "col col-sm-3 col-md-3 col-lg-3" });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Região", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa Destinatário", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Vendedor", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Gerente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Gerente", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Supervisor", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Canal de Entrega", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(EnumSituacaoPedido.Aberto), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.Aberto });
    this.SituacaoComercialPedido = PropertyEntity({ text: "Situação Comercial", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoPedido = PropertyEntity({ text: "Situação Pedido", val: ko.observable(EnumSituacaoPedidoGestaoPedido.Todos), options: EnumSituacaoPedidoGestaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedidoGestaoPedido.Todos });
    this.CodigoAgrupamentoCarregamento = PropertyEntity({ text: "Código Agrupador", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Origem:", idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "UF Destino:", idBtnSearch: guid() });
    this.SituacaoEstoqueProdutoArmazem = PropertyEntity({ text: "Situação Estoque", val: ko.observable(EnumSituacaoEstoqueProdutoArmazem.Todos), options: EnumSituacaoEstoqueProdutoArmazem.obterOpcoesPesquisa(), def: EnumSituacaoEstoqueProdutoArmazem.Todos });
    this.SituacaoRoteirizadorIntegracao = PropertyEntity({ text: "Situação Roteirizador Integração", val: ko.observable(EnumSituacaoRoteirizadorIntegracao.Todos), options: EnumSituacaoRoteirizadorIntegracao.obterOpcoesPesquisa(), def: EnumSituacaoRoteirizadorIntegracao.Todos });
    this.Reentrega = PropertyEntity({ text: "Reentrega", val: ko.observable("Todos"), options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), def: "Todos", });


    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.GestaoPedido,
        callbackRetornoPesquisa: function () {
            $("#" + _pesquisaGestaoPedido.Filial.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.TipoOperacao.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.RegiaoDestino.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.GrupoPessoaDestinatario.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.Vendedor.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.Gerente.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.Supervisor.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.Situacao.id).trigger("change");
            $("#" + _pesquisaGestaoPedido.SituacaoComercialPedido.id).trigger("change");
        }
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.GestaoPedido, _pesquisaGestaoPedido) }, type: types.event, text: Localization.Resources.Gerais.Geral.ConfiguracaoDeFiltros, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            //self.ExibirFiltros.visibleFade(false);
            carregarGridPedidos();
            limparGridListaPedidosSelecionados();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaGestaoPedido.ExibirFiltrosAvancados.visibleFade()) {
                _pesquisaGestaoPedido.ExibirFiltrosAvancados.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaGestaoPedido.ExibirFiltrosAvancados.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ExibirResumoPedidosSelecionados = PropertyEntity({ type: types.bool, val: ko.observable(true), def: true, text: "Exibir Resumo dos Pedidos Selecionados" });
    this.ExibirFiltrosAvancados = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.ListaPedidosSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.bool, getType: typesKnockout.bool });
};

var Opcoes = function () {
    this.Visivel = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.OpcaoRoteirizacao = PropertyEntity({ type: types.event, eventClick: roteirizarPedidosClick, text: "Roteirizar", visible: true });
    this.OpcaoCancelarRoteirizacao = PropertyEntity({ type: types.event, eventClick: cancelarRoteirizacaoPedidosClick, text: "Cancelar Roteirização", visible: _CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy });
    this.OpcaoTrocaFilial = PropertyEntity({ type: types.event, eventClick: trocarFilialPedidosClick, text: "Trocar Filial", visible: ko.observable(true) });
}

var Botoes = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarPedido, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true) });
}

var Totalizadores = function () {
    this.ExibirTotais = PropertyEntity({ type: types.bool, val: _pesquisaGestaoPedido.ExibirResumoPedidosSelecionados.val, def: true });
    this.TotalPedidosSelecionados = PropertyEntity({ type: types.int, text: "Pedidos", val: ko.observable(0) });
    this.PesoTotalSelecionados = PropertyEntity({ type: types.decimal, text: "Peso", val: ko.observable(0.00) });
    this.ValorTotalSelecionados = PropertyEntity({ type: types.decimal, text: "Valor", val: ko.observable(0.00) });
    this.QuantidadePalletsSelecionados = PropertyEntity({ type: types.int, text: "Pallets", val: ko.observable(0) });
    this.QuantidadeEntregasSelecionados = PropertyEntity({ type: types.int, text: "Entregas", val: ko.observable(0) });
}

var DetalhePedido = function (pedido) {

    this.Codigo = PropertyEntity({ visible: ko.observable(false) });
    this.SituacaoComercialPedido = PropertyEntity({ text: "Status comercial:", visible: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Número do pedido embarcador:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SessaoRoteirizador = PropertyEntity({ text: "Número Sessão:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription(), val: ko.observable("") });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataPrevistaDeEntregaBarraRetorno.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCarga.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription(), val: ko.observable("") });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription(), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Remetente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.PesoSaldoRestante = PropertyEntity({ text: "Saldo:", val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.NumeroPaletesFracionado = PropertyEntity({ text: "N° Pallets:", val: ko.observable(""), def: 0, getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.TipoPaleteCliente = PropertyEntity({ text: "Tipo Pallet:", val: ko.observable(EnumTipoPalletCliente.NaoDefinido), options: EnumTipoPalletCliente.obterOpcoes(), def: EnumTipoPalletCliente.NaoDefinido, visible: ko.observable(true) });
    this.TipoPaleteClienteDescricao = PropertyEntity({ text: "Tipo Pallet:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PesoTotalPaletes = PropertyEntity({ text: "Peso Palete:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.CodigoPedidoCliente = PropertyEntity({ text: "N° Pedido Cliente:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: "Data Agendamento:", val: ko.observable(""), visible: ko.observable(true) });
    this.SenhaAgendamento = PropertyEntity({ text: "Senha Agendamento:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ text: "Canal Entrega:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ text: "Vendedor:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Gerente = PropertyEntity({ text: "Gerente:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ text: "Supervisor:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.ValorTotalPedido = PropertyEntity({ text: "Valor Total Mercadorias:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.ObservacaoPedido = PropertyEntity({ text: "Observações Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true), enable: ko.observable(false) });
    this.ObservacaoInterna = PropertyEntity({ text: "Observações Interna:", val: ko.observable(""), def: "", visible: ko.observable(true), enable: ko.observable(false) });

    this.Saldo = PropertyEntity({ text: "Saldo", val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: "Cubagem (M³)", val: ko.observable("") });
    this.ObservacaoCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ObservacaoDoCliente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DescricaoTipoCondicaoPagamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDePagamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DescricaoTipoTomador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoTomador.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CanalVenda.getFieldDescription(), val: ko.observable("") });
    this.GrupoPessoa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoPessoa.getFieldDescription(), val: ko.observable("") });
    this.CategoriaCliente = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CategoriaCliente.getFieldDescription(), val: ko.observable("") });

    PreencherObjetoKnout(this, { Data: pedido });
};

var PesquisaHistoricoIntegracaoGestaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFiltroPesquisa() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.GestaoPedido };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisaGestaoPedido, res.Data.Dados);
            _pesquisaGestaoPedido.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisaGestaoPedido.ModeloFiltrosPesquisa.val(res.Data.Descricao);

            if (_pesquisaGestaoPedido.ModeloFiltrosPesquisa.callbackRetornoPesquisa instanceof Function)
                _pesquisaGestaoPedido.ModeloFiltrosPesquisa.callbackRetornoPesquisa();
        }

        carregarGridPedidos();
    });
}

function loadGestaoPedido() {
    _pesquisaGestaoPedido = new PesquisaGestaoPedido();
    KoBindings(_pesquisaGestaoPedido, "knockoutPesquisaGestaoPedido", false, _pesquisaGestaoPedido.Pesquisar.id);

    _detalhePedido = new DetalhePedido({});
    KoBindings(_detalhePedido, "knoutDetalhePedido");

    _botoes = new Botoes();
    KoBindings(_botoes, "knoutBotoes");

    _opcoes = new Opcoes();
    KoBindings(_opcoes, "knoutOpcoes");

    _totalizadores = new Totalizadores();
    KoBindings(_totalizadores, "knoutTotalizadores");

    _legendaGestaoPedido = new LegendaGestaoPedido();
    KoBindings(_legendaGestaoPedido, "knockoutLegendaGestaoPedido");

    BuscarFilial(_pesquisaGestaoPedido.Filial);
    BuscarTiposOperacao(_pesquisaGestaoPedido.TipoOperacao);
    BuscarRegioes(_pesquisaGestaoPedido.RegiaoDestino);
    BuscarGruposPessoas(_pesquisaGestaoPedido.GrupoPessoaDestinatario);
    BuscarFuncionario(_pesquisaGestaoPedido.Vendedor);
    BuscarFuncionario(_pesquisaGestaoPedido.Supervisor);
    BuscarFuncionario(_pesquisaGestaoPedido.Gerente);
    BuscarSituacaoComercialPedido(_pesquisaGestaoPedido.SituacaoComercialPedido);
    BuscarPedidos(_pesquisaGestaoPedido.ListaNumeroPedido, null, null, true);
    BuscarClientes(_pesquisaGestaoPedido.Remetente);
    BuscarClientes(_pesquisaGestaoPedido.Destinatario);
    BuscarLocalidades(_pesquisaGestaoPedido.Destino);
    BuscarTransportadores(_pesquisaGestaoPedido.Transportador, null, null, true);
    BuscarTiposdeCarga(_pesquisaGestaoPedido.TipoCarga);
    BuscarCanaisEntrega(_pesquisaGestaoPedido.CanalEntrega);
    BuscarEstados(_pesquisaGestaoPedido.EstadoOrigem);
    BuscarEstados(_pesquisaGestaoPedido.EstadoDestino);
    _buscaFilialParaTroca = new BuscarFilial(_opcoes.OpcaoTrocaFilial, retornoTrocaFilial);

    loadGestaoPedidosDetalhesPedidoProdutos();
    loadTiposPaletesDetalhes();
    loadGridPedidos();
    loadFiltroPesquisa();
}

function loadGridPedidos() {
    const opcaoDetalhes = { descricao: "Detalhes", id: guid(), metodo: detalhesPedidoTabelaPedidosClick, icone: "" };
    const opcaoEditarPedido = { descricao: "Editar Pedido", id: guid(), metodo: editarPedidoTabelaPedidosClick, icone: "", visibilidade: visibilidadeEditarPedido };
    const opcaoRoteirizacao = { descricao: "Roteirizar", id: guid(), metodo: roteirizarPedidoClick, icone: "", visibilidade: visibilidadeRoteirizarPedido };
    const opcaoAtualizacaoRoteirizacao = { descricao: "Atualizar Roteirização", id: guid(), metodo: atualizarRoteirizacaoPedidoClick, icone: "", visibilidade: visibilidadeAtualizarRoteirizacaoPedido };
    const opcaoCancelarRoteirizacao = { descricao: "Cancelar Roteirização", id: guid(), metodo: cancelarRoteirizacaoPedidoClick, icone: "", visibilidade: visibilidadeCancelarRoteirizacaoPedido };
    const opcaoHistoricoIntegracao = { descricao: "Histórico Integração", id: guid(), metodo: buscarIntegracoesClick, icone: "", visibilidade: visibilidadeHistoricoRoteirizacaoPedido };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoDetalhes, opcaoEditarPedido, opcaoRoteirizacao, opcaoAtualizacaoRoteirizacao, opcaoCancelarRoteirizacao, opcaoHistoricoIntegracao], tamanho: 10 };

    const multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        callbackNaoSelecionado: naoSelecionadoCallback,
        callbackSelecionado: selecionadoCallback,
        somenteLeitura: false,
        SelecionarTodosKnout: _opcoes.SelecionarTodos,
    }

    var configExportacao = {
        url: "GestaoPedido/ExportarPesquisa",
        titulo: "Gestão de Pedidos"
    };

    _gridPedido = new GridViewExportacao(
        "grid-gestao-pedidos",
        "GestaoPedido/Pesquisa",
        _pesquisaGestaoPedido,
        menuOpcoes,
        configExportacao,
        null,
        null,
        multiplaEscolha
    );

    _gridPedido.SetQuantidadeLinhasPorPagina(12);
    _gridPedido.SetPermitirEdicaoColunas(true);
    _gridPedido.SetSalvarPreferenciasGrid(true);
}

function loadTiposPaletesDetalhes() {
    executarReST("GestaoPedido/BuscarTiposPaletesDetalhes", null, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _tiposPaletesClienteDetalhes = retorno.Data;
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

var LegendaGestaoPedido = function () {
    this.IntegradoComSucesso = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IntegradoComSucesso, visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy), totalItens: ko.observable("") });
    this.IntegracaoCancelada = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IntegracaoCancelada, visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy), totalItens: ko.observable("") });
    this.SemEstoque = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.SemEstoque, visible: ko.observable(true), totalItens: ko.observable("") });
};

function cancelarRoteirizacaoPedidoClick(registroSelecionado) {
    var codigosPedidos = [registroSelecionado.Codigo];

    cancelarRoteirizacaoPedidos(codigosPedidos);
}

function cancelarRoteirizacaoPedidosClick() {
    var codigosPedidos = _pesquisaGestaoPedido.ListaPedidosSelecionados.val();

    if (_gridPedido.ObterMultiplosSelecionados().some(o => o.SituacaoPedido == EnumSituacaoPedido.Cancelado)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Não é possivel cancelar a roteirização de pedidos cancelados");
        return;
    }

    cancelarRoteirizacaoPedidos(codigosPedidos);
}

function detalhesPedidoTabelaPedidosClick(pedidoSelecionado) {
    ativarDesativarEdicaoPedido(false);
    ObterDetalhesPedido(pedidoSelecionado.Codigo, true);
}

function editarPedidoTabelaPedidosClick(pedidoSelecionado) {
    ativarDesativarEdicaoPedido(true);
    ObterDetalhesPedido(pedidoSelecionado.Codigo, false);
}

function roteirizarPedidoClick(registroSelecionado) {
    var codigosPedidos = [registroSelecionado.Codigo];

    roteirizarPedidos(codigosPedidos);
}

function roteirizarPedidosClick() {
    var codigosPedidos = _pesquisaGestaoPedido.ListaPedidosSelecionados.val();

    if (_gridPedido.ObterMultiplosSelecionados().some(o => o.SituacaoPedido == EnumSituacaoPedido.Cancelado)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Não é possivel roteirizar pedidos cancelados");
        return;
    }

    roteirizarPedidos(codigosPedidos);
}

function buscarIntegracoesClick(registroSelecionado) {

    buscarIntegracaoGestaoPedido(registroSelecionado);

    Global.abrirModal("divModalIntegracaoGestaoPedido");
}

function historicoIntegracaoClick(registroSelecionado) {

    buscarHistoricoIntegracaoGestaoPedido(registroSelecionado);

    Global.abrirModal("divModalHistoricoIntegracaoGestaoPedido");
}

function tipoPaleteClienteChange(tipoPaleteClienteSelecionado) {
    _detalhePedido.PesoTotalPaletes.val(Globalize.format(0, "n2"));
    var tipoPaleteClienteSelecionadoDesc = EnumTipoPalletCliente.obterDescricao(tipoPaleteClienteSelecionado);
    _detalhePedido.TipoPaleteClienteDescricao.val(tipoPaleteClienteSelecionadoDesc);

    if (tipoPaleteClienteSelecionado === EnumTipoPalletCliente.NaoDefinido || _detalhePedido.NumeroPaletesFracionado.val() <= 0) return;

    calcularPesoTotalPaletes();
}

function trocarFilialPedidosClick() {
    _buscaFilialParaTroca.abrirBusca();
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function ativarDesativarEdicaoPedido(visible) {
    _detalhePedido.NumeroPaletesFracionado.visible(visible);
    _detalhePedido.PesoTotalPaletes.visible(visible);
    _detalhePedido.TipoPaleteCliente.visible(visible);
    _botoes.Salvar.visible(visible);
}

function buscarPedidosSelecionados() {
    if (!string.IsNullOrWhiteSpace(_pesquisaGestaoPedido.ListaPedidosSelecionados.val())) {
        executarReST("GestaoPedido/BuscarPedidosSelecionados", RetornarObjetoPesquisa(_pesquisaGestaoPedido), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _pesquisaGestaoPedido.ListaPedidosSelecionados.val(retorno.Data.CodigosPedidos);

                    _totalizadores.TotalPedidosSelecionados.val(retorno.Data.TotalPedidos);
                    _totalizadores.PesoTotalSelecionados.val(retorno.Data.PesoTotalPedidos);
                    _totalizadores.ValorTotalSelecionados.val(retorno.Data.ValorTotalPedidos);
                    _totalizadores.QuantidadePalletsSelecionados.val(retorno.Data.QuantidadePalletsPedidos);
                    _totalizadores.QuantidadeEntregasSelecionados.val(retorno.Data.QuantidadeEntregasSelecionados);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    } else {
        _totalizadores.TotalPedidosSelecionados.val(0);
        _totalizadores.PesoTotalSelecionados.val(0.00);
        _totalizadores.ValorTotalSelecionados.val(0.00);
        _totalizadores.QuantidadePalletsSelecionados.val(0);
        _totalizadores.QuantidadeEntregasSelecionados.val(0);
    }
}

function buscarRowsSelecionadas() {
    var pedidosSelecionados = null;

    _pesquisaGestaoPedido.SelecionarTodos.val(_opcoes.SelecionarTodos.val());

    if (_opcoes.SelecionarTodos.val())
        pedidosSelecionados = _gridPedido.ObterMultiplosNaoSelecionados();
    else
        pedidosSelecionados = _gridPedido.ObterMultiplosSelecionados();

    var codigosPedidosSelecionados = new Array();

    for (var i = 0; i < pedidosSelecionados.length; i++)
        codigosPedidosSelecionados.push(pedidosSelecionados[i].DT_RowId);

    if (codigosPedidosSelecionados && (codigosPedidosSelecionados.length > 0 || _opcoes.SelecionarTodos.val()))
        _pesquisaGestaoPedido.ListaPedidosSelecionados.val(JSON.stringify(codigosPedidosSelecionados));
    else
        _pesquisaGestaoPedido.ListaPedidosSelecionados.val("");
}

function calcularPesoTotalPaletes() {
    var valor = _tiposPaletesClienteDetalhes.find(tipo => tipo.Codigo === _detalhePedido.TipoPaleteCliente.val()).Valor;

    if (valor == 0) return;

    var numeroPaletesFracionado = Globalize.parseFloat(_detalhePedido.NumeroPaletesFracionado.val());

    _detalhePedido.PesoTotalPaletes.val(Globalize.format(numeroPaletesFracionado * valor, "n2"));
}

function cancelarRoteirizacaoPedidos(codigosPedidos) {
    executarReST("GestaoPedido/IntegrarCancelamentoPedidosRoutEasy", { CodigosPedidos: JSON.stringify(codigosPedidos) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "Cancelamento dos Pedidos integrado com sucesso");
                carregarGridPedidos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarGridPedidos() {
    _gridPedido.CarregarGrid(function (grid) {
        var temDados = grid.data.length > 0;

        _opcoes.SelecionarTodos.visible(temDados);
        _opcoes.SelecionarTodos.val(false);

        _gridPedido.AtualizarRegistrosSelecionados([]);
        _gridPedido.AtualizarRegistrosNaoSelecionados([]);

        mostrarOpcoes();
    });
}

function mostrarOpcoes() {
    var pedidosSelecionados = _gridPedido.ObterMultiplosSelecionados();
    var pedidosComFilialDiferente = pedidosSelecionados?.some((value, _, array) => value.Filial !== array[0].Filial);

    _opcoes.Visivel.visible(Boolean(pedidosSelecionados.length))
    _opcoes.OpcaoTrocaFilial.visible(!pedidosComFilialDiferente);
}

function naoSelecionadoCallback() {
    buscarRowsSelecionadas();
    buscarPedidosSelecionados();
    mostrarOpcoes();
}

function roteirizarPedidos(codigosPedidos) {
    if (_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy)
        integrarPedidosSelecionadosRoutEasy(codigosPedidos, false);
    else
        validarPedidosSelecionados(codigosPedidos);
}

function atualizarRoteirizacaoPedidoClick(registroSelecionado) {
    var codigosPedidos = [registroSelecionado.Codigo];

    integrarPedidosSelecionadosRoutEasy(codigosPedidos, true);
}

function selecionadoCallback() {
    buscarRowsSelecionadas();
    buscarPedidosSelecionados();
    mostrarOpcoes();
}

function retornoTrocaFilial(filialSelecionada) {
    var filialPedidosSelecionado = _gridPedido.ObterMultiplosSelecionados()[0].Filial;

    exibirConfirmacao("Confirmação", `Deseja realmente alterar a filial (${filialPedidosSelecionado}) para a filial (${filialSelecionada.Descricao}) dos pedidos selecionados?`, function () {
        trocarFilial(filialSelecionada.Codigo);
    });
}

function trocarFilial(codigoFilialSelecionada) {
    var dados = { CodigosPedidos: JSON.stringify(_pesquisaGestaoPedido.ListaPedidosSelecionados.val()), CodigoFilial: codigoFilialSelecionada };

    executarReST("GestaoPedido/TrocarFilial", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Troca de filial realizada com sucesso!");

                carregarGridPedidos();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function calcularTotaisPedidosSelecionados() {
    var dados = { CodigosPedidos: JSON.stringify(_pesquisaGestaoPedido.ListaPedidosSelecionados.val()) };

    executarReST("GestaoPedido/CalcularTotaisPedidosSelecionados", dados, function (retorno) {
        if (retorno.Success) {
            console.log(retorno.Data);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function validarPedidosSelecionados(codigosPedidos) {
    executarReST("GestaoPedido/ValidarPedidosSelecionados", { CodigosPedidos: JSON.stringify(codigosPedidos) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.SessaoRoteirizador > 0)
                    validarQuantidadePedidosSelecionadosSessao(retorno.Data);
                else
                    navegarParaCarregamentoMapa(retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function validarQuantidadePedidosSelecionadosSessao(data, confirmou) {
    var dados = { CodigosPedidos: JSON.stringify(data.Pedidos), SessaoRoteirizador: data.SessaoRoteirizador }

    executarReST("GestaoPedido/ValidarQuantidadePedidosSessao", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                navegarParaCarregamentoMapa(data);
            } else {
                exibirConfirmacao("Confirmação", "A quantidade de pedidos da sessão é maior que os pedidos selecionados, realmente deseja continuar?", function () {
                    navegarParaCarregamentoMapa(data);
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function navegarParaCarregamentoMapa(data) {
    //exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Informações enviadas para a tela com sucesso.");
    //return;

    var origin = window.location.origin;
    var win = window.open(origin + "/#Cargas/MontagemCargaMapa", '_blank');

    if (win)
        win['DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS_WINDOW'] = {
            ...data,
            Pedidos: data.SessaoRoteirizador > 0 ? [] : data.Pedidos,
            CodigosAgrupadores: data.SessaoRoteirizador > 0 ? [] : data.CodigosAgrupadores,
            DataInicio: _pesquisaGestaoPedido.DataInicio.val(),
            DataFim: _pesquisaGestaoPedido.DataFim.val(),
        };
}

function integrarPedidosSelecionadosRoutEasy(codigosPedidos, atualizacaoPedido) {
    executarReST("GestaoPedido/IntegrarPedidosRoutEasy", { CodigosPedidos: JSON.stringify(codigosPedidos), AtualizacaoPedido: atualizacaoPedido }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "Pedidos integrados com sucesso");
                carregarGridPedidos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ObterDetalhesPedido(codigo, somenteVizualizacao) {
    executarReST("GestaoPedido/BuscarPorCodigoPedido", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhePedido, { Data: retorno.Data });

                Global.abrirModal('modalDetalhePedido');

                var subscribes = [];

                if (!somenteVizualizacao) {
                    subscribes.push(_detalhePedido.TipoPaleteCliente.val.subscribe(tipoPaleteClienteChange));
                    subscribes.push(_detalhePedido.NumeroPaletesFracionado.val.subscribe(tipoPaleteClienteChange));
                }

                $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                    if (subscribes.length > 0)
                        subscribes.forEach(sub => sub.dispose());

                    LimparCampos(_detalhePedido);
                    LimparCampos(_detalhesPedidoProdutos);
                    Global.ResetarAba("modalDetalhePedido");
                });

                carregarDetalhesPedidoProdutos(codigo, somenteVizualizacao);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarPedido(e, sender) {
    var produtos = _gridDetalhesPedidoProdutos.BuscarRegistros();

    var dados = {
        ProdutosPedido: JSON.stringify(produtos),
        CodigoPedido: _detalhePedido.Codigo.val(),
        PesoTotalPaletes: _detalhePedido.PesoTotalPaletes.val(),
        NumeroPaletesFracionado: _detalhePedido.NumeroPaletesFracionado.val(),
        TipoPaleteCliente: _detalhePedido.TipoPaleteCliente.val(),
    }

    executarReST("GestaoPedido/AtualizarPedido", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                this.fecharModalPedido(e, sender);

                carregarGridPedidos();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function fecharModalPedido(e, sender) {
    Global.fecharModal('modalDetalhePedido');
}

function visibilidadeEditarPedido(registroSelecionado) {
    return registroSelecionado.SessaoRoteirizador == "" && (registroSelecionado.SituacaoPedido != EnumSituacaoPedido.Cancelado);
}

function visibilidadeCancelarRoteirizacaoPedido(registroSelecionado) {
    return (_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy && (registroSelecionado.SituacaoRoteirizadorIntegracao == EnumSituacaoRoteirizadorIntegracao.Integrado) && (registroSelecionado.SituacaoPedido != EnumSituacaoPedido.Cancelado));
}

function visibilidadeRoteirizarPedido(registroSelecionado) {
    return (registroSelecionado.SituacaoPedido != EnumSituacaoPedido.Cancelado && (!registroSelecionado.ReentregaSolicitada || !_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy));
}

function visibilidadeAtualizarRoteirizacaoPedido(registroSelecionado) {
    return (_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy && (registroSelecionado.SituacaoRoteirizadorIntegracao == EnumSituacaoRoteirizadorIntegracao.Cancelado || registroSelecionado.SituacaoRoteirizadorIntegracao == EnumSituacaoRoteirizadorIntegracao.Integrado));
}

function visibilidadeHistoricoRoteirizacaoPedido(registroSelecionado) {
    return (_CONFIGURACAO_TMS.PossuiIntegracaoRoutEasy);
}

function limparGridListaPedidosSelecionados() {
    _pesquisaGestaoPedido.ListaPedidosSelecionados.val("");
    buscarPedidosSelecionados();
}

function buscarIntegracaoGestaoPedido(registroSelecionado) {
    _pesquisaHistoricoIntegracaoGestaoPedido = new PesquisaHistoricoIntegracaoGestaoPedido();
    _pesquisaHistoricoIntegracaoGestaoPedido.Codigo.val(registroSelecionado.Codigo);

    let historico = { descricao: "Arquivos Integração", id: guid(), evento: "onclick", metodo: historicoIntegracaoClick, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [historico]
    };

    _gridIntegracaoGestaoPedido = new GridView("tblIntegracaoGestaoPedido", "GestaoPedido/ConsultarIntegracoesPedido", _pesquisaHistoricoIntegracaoGestaoPedido, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridIntegracaoGestaoPedido.CarregarGrid();
}

function buscarHistoricoIntegracaoGestaoPedido(registroSelecionado) {
    _pesquisaHistoricoIntegracaoGestaoPedido = new PesquisaHistoricoIntegracaoGestaoPedido();
    _pesquisaHistoricoIntegracaoGestaoPedido.Codigo.val(registroSelecionado.Codigo);

    let download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoGestaoPedido, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoGestaoPedido = new GridView("tblHistoricoIntegracaoGestaoPedido", "GestaoPedido/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoGestaoPedido, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoGestaoPedido.CarregarGrid();
}

function downloadArquivosHistoricoIntegracaoGestaoPedido(data) {
    executarDownload("GestaoPedido/DownloadArquivosIntegracao", { Codigo: data.Codigo });
}

// #endregion Funções Privadas
