/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />

var _pesquisaConsolidacaoSolicitaoGas;
var _consolidacaoSolicitacaoGas;
var _gridConsolidacaoSolicitacaoGas;
var _gridQuantidades;

var PesquisaConsolidacaoSolicitacaoGas = function () {
    this.BaseSatelite = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Base Satélite:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.BaseSupridora = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.multiplesEntities, text: "Base Supridora:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.Usuario = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Usuário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.DataSolicitacao = PropertyEntity({ val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, text: "*Data Solicitação:", visible: ko.observable(true), enable: ko.observable(true), required: true });

    this.DataCriacaoInicial = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, text: "Data Criação Inicial:", visible: ko.observable(true), enable: ko.observable(true) });
    this.DataCriacaoFinal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date, text: "Data Criação Final:", visible: ko.observable(true), enable: ko.observable(true) });

    this.DataCriacaoFinal.dateRangeInit = this.DataCriacaoInicial;
    this.DataCriacaoInicial.dateRangeLimit = this.DataCriacaoFinal;

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarConsolidacaoSolicitacaoGas, type: types.event, text: "Pesquisar", visible: ko.observable(true), idGrid: guid() });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

var ConsolidacaoSolicitacaoGas = function () {
    this.GridConsolidacoes = PropertyEntity({ visible: ko.observable(true), idGrid: guid() });
    this.GridQuantidadesDisponiveis = PropertyEntity({ visible: ko.observable(true), idGrid: guid() });
}

function loadConsolidacaoSolicitacaoGas() {
    _pesquisaConsolidacaoSolicitaoGas = new PesquisaConsolidacaoSolicitacaoGas();
    _consolidacaoSolicitacaoGas = new ConsolidacaoSolicitacaoGas();

    KoBindings(_pesquisaConsolidacaoSolicitaoGas, "knockoutPesquisaConsolidacaoSolicitacaoGas");
    KoBindings(_consolidacaoSolicitacaoGas, "knockoutConsolidacaoSolicitacaoGas");

    new BuscarClientes(_pesquisaConsolidacaoSolicitaoGas.BaseSatelite, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarClientes(_pesquisaConsolidacaoSolicitaoGas.BaseSupridora, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    new BuscarFuncionario(_pesquisaConsolidacaoSolicitaoGas.Usuario);

    loadConsolidarSolicitacaoGas();
    loadConsolidacoesGeradas();

    loadGridConsolidacaoSolicitacaoGas();
    loadGridQuantidades();
}

function loadGridConsolidacaoSolicitacaoGas() {
    var opcaoConsolidar = {
        descricao: "Consolidar",
        id: guid(),
        evento: "onclick",
        metodo: consolidarClick,
        tamanho: "5",
        icone: ""
    };

    var opcaoCarregarSolicitacao = {
        descricao: "Carregar",
        id: guid(),
        evento: "onclick",
        metodo: carregarSolicitacaoClick,
        tamanho: "5",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [opcaoConsolidar, opcaoCarregarSolicitacao]
    };

    _gridConsolidacaoSolicitacaoGas = new GridViewExportacao(_consolidacaoSolicitacaoGas.GridConsolidacoes.idGrid, "ConsolidacaoSolicitacaoGas/Pesquisar", _pesquisaConsolidacaoSolicitaoGas, menuOpcoes, null, { column: 5, dir: orderDir.asc }, 10);
    _gridConsolidacaoSolicitacaoGas.CarregarGrid();
}

function loadGridQuantidades() {
    _gridQuantidades = new GridViewExportacao(_consolidacaoSolicitacaoGas.GridQuantidadesDisponiveis.idGrid, "ConsolidacaoSolicitacaoGas/PesquisarQuantidades", _pesquisaConsolidacaoSolicitaoGas, null, null, null, 5);
    _gridQuantidades.CarregarGrid();
}


function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function carregarSolicitacaoClick(registroSelecionado) {
    carregarAbastecimentoGasClick(registroSelecionado, false, true);
}

function consolidarClick(registroSelecionado) {
    _consolidarSolicitacaoGas.Solicitacao.val(registroSelecionado.Codigo);
    _consolidarSolicitacaoGas.Produto.val(registroSelecionado.CodigoProduto);

    _consolidarSolicitacaoGas.QuantidadeDisponivel.val(obterQuantidadeDisponivel(registroSelecionado.BaseSupridoraCodigo, registroSelecionado.CodigoProduto) + " ton");
    _consolidarSolicitacaoGas.QuantidadeFaltante.val(registroSelecionado.QuantidadeFaltante + " ton");
    _consolidarSolicitacaoGas.FilialSolicitante.val(registroSelecionado.BaseSatelite);
    _consolidarSolicitacaoGas.ProdutoSolicitacao.val(registroSelecionado.Produto);

    buscarInformacoesConsolidacao();

    $("#divModalConsolidarSolicitacao").modal("show")
        .on('hidden.bs.modal', function () {
            limparCamposConsolidarSolicitacao();
            limparCamposConsolidacoesGeradas();
            Global.ResetarAba("divModalConsolidarSolicitacao");
        });
}

function pesquisarConsolidacaoSolicitacaoGas() {
    if (!ValidarCamposObrigatorios(_pesquisaConsolidacaoSolicitaoGas)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    _gridConsolidacaoSolicitacaoGas.CarregarGrid();
    _gridQuantidades.CarregarGrid();
}