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
/// <reference path="Etapa.js" />
/// <reference path="Integracoes.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/SituacaoComercialPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLoteLiberacaoComercialPedido;
var _gridLoteLiberacaoComercialPedidoBloqueado;
var _pesquisaLoteLiberacaoComercialPedido;
var _pesquisaCadastroLoteLiberacaoComercialPedido;
var _cadastroGridLoteLiberacaoComercialPedido;
var _CRUDLoteLiberacaoComercialPedido;

var PesquisaLoteLiberacaoComercialPedido = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoLoteLiberacaoComercialPedido.Todos), options: EnumSituacaoLoteLiberacaoComercialPedido.obterOpcoesPesquisa(), def: EnumSituacaoLoteLiberacaoComercialPedido.Todos });
    this.Pedido = PropertyEntity({ text: "Pedido:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteLiberacaoComercialPedido.CarregarGrid();
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
};

var PesquisaCadastroLoteLiberacaoComercialPedido = function () {

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoLoteLiberacaoComercialPedido.EmIntegracao), options: EnumSituacaoLoteLiberacaoComercialPedido.obterOpcoesPesquisa(), def: EnumSituacaoLoteLiberacaoComercialPedido.EmIntegracao });

    let dataAtual = moment().format("DD/MM/YYYY");

    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataInicialPedido = PropertyEntity({ text: "Data pedido de: ", getType: typesKnockout.date, val: ko.observable(dataAtual), enable: ko.observable(true) });
    this.DataFinalPedido = PropertyEntity({ text: "Data pedido até: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicialPedido.dateRangeLimit = this.DataFinalPedido;
    this.DataFinalPedido.dateRangeInit = this.DataInicialPedido;
    this.Pedido = PropertyEntity({ text: "Pedido:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Destinatario = PropertyEntity({ text: "Destinatário:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.SituacaoComercialPedido = PropertyEntity({ text: "Situação Comercial do Pedido:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Vendedor = PropertyEntity({ text: "Vendedor:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Gerente = PropertyEntity({ text: "Gerente:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Supervisor = PropertyEntity({ text: "Supervisor:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ text: "Canal de Entrega:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });

    this.GrupoPessoas = PropertyEntity({ text: "Grupo de Pessoas:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Categoria = PropertyEntity({ text: "Categoria:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Regiao = PropertyEntity({ text: "Região:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaPedidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLoteLiberacaoComercialPedidoBloqueado.CarregarGrid();
            _pesquisaCadastroLoteLiberacaoComercialPedido.ExibirFiltros.visibleFade(false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true),
        enable: ko.observable(true)
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

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CadastroGridLoteLiberacaoComercialPedido = function () {

    this.Grid = PropertyEntity({ idGrid: guid() });
};


var GridLoteLiberacaoComercialPedido = function () {

    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLoteLiberacaoComercialPedido, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLoteLiberacaoComercialPedido = new GridView(_pesquisaLoteLiberacaoComercialPedido.Pesquisar.idGrid, "LoteLiberacaoComercialPedido/Pesquisa", _pesquisaLoteLiberacaoComercialPedido, menuOpcoes);
    _gridLoteLiberacaoComercialPedido.CarregarGrid();
};

var GridLoteLiberacaoComercialPedidoBloqueado = function () {

    if (_gridLoteLiberacaoComercialPedidoBloqueado != null)
        _gridLoteLiberacaoComercialPedidoBloqueado.Destroy();

    _pesquisaCadastroLoteLiberacaoComercialPedido.SelecionarTodos.visible(true);
    _pesquisaCadastroLoteLiberacaoComercialPedido.SelecionarTodos.val(false);

    let multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCadastroLoteLiberacaoComercialPedido.SelecionarTodos,
        somenteLeitura: false
    };

    _gridLoteLiberacaoComercialPedidoBloqueado = new GridView(_pesquisaCadastroLoteLiberacaoComercialPedido.Pesquisar.idGrid, "LoteLiberacaoComercialPedido/PesquisaPedidos", _pesquisaCadastroLoteLiberacaoComercialPedido, null, null, null, null, null, null, multiplaescolha);
    _gridLoteLiberacaoComercialPedidoBloqueado.CarregarGrid();

};

var CRUDLoteLiberacaoComercialPedido = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposClick, type: types.event, text: "Limpar Campos / Novo", visible: ko.observable(true) });
    this.LiberarcomIntegracaoFalha = PropertyEntity({ eventClick: function (e) { LiberarcomIntegracaoFalhaClick(); }, type: types.event, text: "Liberar com Integração Falha", idGrid: guid(), visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadLoteLiberacaoComercialPedido() {

    _pesquisaLoteLiberacaoComercialPedido = new PesquisaLoteLiberacaoComercialPedido();
    KoBindings(_pesquisaLoteLiberacaoComercialPedido, "knockoutPesquisaLoteLiberacaoComercialPedido", false, _pesquisaLoteLiberacaoComercialPedido.Pesquisar.id);

    _pesquisaCadastroLoteLiberacaoComercialPedido = new PesquisaCadastroLoteLiberacaoComercialPedido();
    KoBindings(_pesquisaCadastroLoteLiberacaoComercialPedido, "knockoutPesquisaCadastroLoteLiberacaoComercialPedido", false, _pesquisaCadastroLoteLiberacaoComercialPedido.Pesquisar.id);

    HeaderAuditoria("LoteLiberacaoComercialPedido", _pesquisaCadastroLoteLiberacaoComercialPedido);

    _CRUDLoteLiberacaoComercialPedido = new CRUDLoteLiberacaoComercialPedido();
    KoBindings(_CRUDLoteLiberacaoComercialPedido, "knockoutCRUDLoteLiberacaoComercialPedido");

    BuscarFilial(_pesquisaCadastroLoteLiberacaoComercialPedido.Filial);
    BuscarPedidos(_pesquisaCadastroLoteLiberacaoComercialPedido.Pedido);
    BuscarPedidos(_pesquisaLoteLiberacaoComercialPedido.Pedido);
    BuscarClientes(_pesquisaCadastroLoteLiberacaoComercialPedido.Destinatario);
    BuscarFuncionario(_pesquisaCadastroLoteLiberacaoComercialPedido.Gerente);
    BuscarFuncionario(_pesquisaCadastroLoteLiberacaoComercialPedido.Vendedor);
    BuscarFuncionario(_pesquisaCadastroLoteLiberacaoComercialPedido.Supervisor);
    BuscarCanaisEntrega(_pesquisaCadastroLoteLiberacaoComercialPedido.CanalEntrega);
    BuscarGruposPessoas(_pesquisaCadastroLoteLiberacaoComercialPedido.GrupoPessoas);
    BuscarCategoriaPessoa(_pesquisaCadastroLoteLiberacaoComercialPedido.Categoria);
    BuscarRegioes(_pesquisaCadastroLoteLiberacaoComercialPedido.Regiao);
    BuscarSituacaoComercialPedido(_pesquisaCadastroLoteLiberacaoComercialPedido.SituacaoComercialPedido);

    LoadEtapaLoteLiberacaoComercialPedido();
    LoadLoteLiberacaoComercialPedidoIntegracao();

    GridLoteLiberacaoComercialPedido();
    GridLoteLiberacaoComercialPedidoBloqueado();
}

function AdicionarClick() {

    ObterPedidos();

    Salvar(_pesquisaCadastroLoteLiberacaoComercialPedido, "LoteLiberacaoComercialPedido/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                LimparCamposPesquisa();
                _gridLoteLiberacaoComercialPedido.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function LimparCamposClick() {
    LimparCamposPesquisa();
}

function EtapaLoteLiberacaoComercialPedidoClick() {
    recarregarLoteLiberacaoComercialPedidoIntegracoes();
}

//*******MÉTODOS*******

function ObterPedidos() {

    let pedidos;

    if (_pesquisaCadastroLoteLiberacaoComercialPedido.SelecionarTodos.val())
        pedidos = _gridLoteLiberacaoComercialPedidoBloqueado.ObterMultiplosNaoSelecionados();
    else
        pedidos = _gridLoteLiberacaoComercialPedidoBloqueado.ObterMultiplosSelecionados();

    let codigos = pedidos.map(function (pedido) {
        return pedido.DT_RowId;
    });

    if (codigos && (codigos.length > 0 || _pesquisaCadastroLoteLiberacaoComercialPedido.SelecionarTodos.val()))
        _pesquisaCadastroLoteLiberacaoComercialPedido.ListaPedidos.val(JSON.stringify(codigos));
    else
        _pesquisaCadastroLoteLiberacaoComercialPedido.ListaPedidos.val("");
}

function editarLoteLiberacaoComercialPedido(loteLiberacaoComercialPedido) {
    _pesquisaCadastroLoteLiberacaoComercialPedido.Codigo.val(loteLiberacaoComercialPedido.Codigo);
    _pesquisaCadastroLoteLiberacaoComercialPedido.Situacao.val(EnumSituacaoLoteLiberacaoComercialPedido.obterValorEnum(loteLiberacaoComercialPedido.Situacao));
    _CRUDLoteLiberacaoComercialPedido.Adicionar.visible(false);
    _pesquisaLoteLiberacaoComercialPedido.ExibirFiltros.visibleFade(false);
    _pesquisaCadastroLoteLiberacaoComercialPedido.ExibirFiltros.visibleFade(true);
    SetarEnableCamposKnockout(_pesquisaCadastroLoteLiberacaoComercialPedido, false);
    LimpaSelecionados();

    if (_pesquisaCadastroLoteLiberacaoComercialPedido.Situacao.val() === EnumSituacaoLoteLiberacaoComercialPedido.Finalizado)
        _CRUDLoteLiberacaoComercialPedido.LiberarcomIntegracaoFalha.visible(false);
    else
        _CRUDLoteLiberacaoComercialPedido.LiberarcomIntegracaoFalha.visible(true);

    _gridLoteLiberacaoComercialPedidoBloqueado.CarregarGrid(SetarEtapaLoteLiberacaoComercialPedido);
}

function LimparCamposPesquisa() {
    _CRUDLoteLiberacaoComercialPedido.Adicionar.visible(true);
    _CRUDLoteLiberacaoComercialPedido.LiberarcomIntegracaoFalha.visible(false);
    SetarEtapaInicioLoteLiberacaoComercialPedido();
    SetarEnableCamposKnockout(_pesquisaCadastroLoteLiberacaoComercialPedido, true);
    LimparCampos(_pesquisaCadastroLoteLiberacaoComercialPedido);
    LimpaSelecionados();
    _gridLoteLiberacaoComercialPedidoBloqueado.CarregarGrid();
}

function LimpaSelecionados() {
    _gridLoteLiberacaoComercialPedidoBloqueado.AtualizarRegistrosNaoSelecionados(new Array());
    _gridLoteLiberacaoComercialPedidoBloqueado.AtualizarRegistrosSelecionados(new Array());
}

