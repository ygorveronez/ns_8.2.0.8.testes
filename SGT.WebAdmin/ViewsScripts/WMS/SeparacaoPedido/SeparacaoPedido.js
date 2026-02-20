/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSeparacaoPedido.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="SeparacaoPedidoEtapa.js" />
/// <reference path="Pedido.js" />
/// <reference path="Integracoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaSeparacaoPedido;
var _separacaoPedido;
var _crudSeparacaoPedido;
var _gridFuncionarios;
var _pesquisaSelecoes;
var _gridSelecoes;

var PesquisaSelecoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable("") });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSeparacaoPedido.Todos), options: EnumSituacaoSeparacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoSeparacaoPedido.Todos, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecoes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltrosPesquisa.visibleFade()) {
                e.ExibirFiltrosPesquisa.visibleFade(false);
            } else {
                e.ExibirFiltrosPesquisa.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PesquisaSeparacaoPedido = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pedido:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.LocalExpedicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Expedição:", idBtnSearch: guid(), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.NumeroNotaFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Nº Notas Fiscais", idBtnSearch: guid(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.ExibirSomentePedidosEmAberto = PropertyEntity({ text: "Exibir somente pedidos em aberto", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomentePedidosDeReentrega = PropertyEntity({ text: "Exibir somente pedidos de reentrega", getType: typesKnockout.bool, val: ko.observable(false) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarPedidos();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
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

var SeparacaoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSeparacaoPedido.Aberto), def: EnumSituacaoSeparacaoPedido.Aberto, getType: typesKnockout.int });

    this.Pedidos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });
};

var CRUDSeparacaoPedido = function () {
    this.Cancelar = PropertyEntity({ eventClick: CancelarSeparacaoClick, type: types.event, text: "Cancelar", visible: ko.observable(false), enable: ko.observable(true) });
    //this.EnviarSeparacao = PropertyEntity({ eventClick: EnviarSeparacaoClick, type: types.event, text: "Enviar para a Integração", visible: ko.observable(false), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar/Nova Separação", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadSeparacaoPedido() {
    _separacaoPedido = new SeparacaoPedido();
    KoBindings(_separacaoPedido, "knoutSeparacaoPedido");

    HeaderAuditoria("SeparacaoPedido", _separacaoPedido);

    _crudSeparacaoPedido = new CRUDSeparacaoPedido();
    KoBindings(_crudSeparacaoPedido, "knoutCRUDSeparacaoPedido");

    _pesquisaSeparacaoPedido = new PesquisaSeparacaoPedido();
    KoBindings(_pesquisaSeparacaoPedido, "knoutPesquisaSeparacaoPedido");

    _pesquisaSelecoes = new PesquisaSelecoes();
    KoBindings(_pesquisaSelecoes, "knockoutPesquisaSelecoes");

    new BuscarPedidos(_pesquisaSelecoes.Pedido);

    new BuscarPedidos(_pesquisaSeparacaoPedido.Pedido);
    new BuscarClientes(_pesquisaSeparacaoPedido.Remetente);
    new BuscarClientes(_pesquisaSeparacaoPedido.Destinatario);
    new BuscarClientes(_pesquisaSeparacaoPedido.LocalExpedicao);
    new BuscarFilial(_pesquisaSeparacaoPedido.Filial);
    new BuscarLocalidades(_pesquisaSeparacaoPedido.Origem);
    new BuscarLocalidades(_pesquisaSeparacaoPedido.Destino);
    new BuscarXMLNotaFiscal(_pesquisaSeparacaoPedido.NumeroNotaFiscal);

    loadDetalhesPedido(buscarPedidosMontagem);
    loadSeparacaoPedidoIntegracoes();
    LoadEtapasSeparacaoPedido();
    LoadSeparacaoInformacao();

    buscarSelecoes();
}

function SalvarClick() {
    var data = { Selecao: JSON.stringify(RetornarObjetoPesquisa(_separacaoPedido)) };

    executarReST("SeparacaoPedido/CriarSeparacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                limparDadosSelecao();
                buscarSelecoes();
                _separacaoPedido.Codigo.val(arg.Data.Codigo);
                var data = { Codigo: _separacaoPedido.Codigo.val() };
                retornoSelecao(data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function LimparClick() {
    limparDadosSelecao();
}



function CancelarSeparacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a separação selecionada?", function () {
        var data = { Codigo: _separacaoPedido.Codigo.val() };

        executarReST("SeparacaoPedido/CancelarSeparacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    limparDadosSelecao();
                    buscarSelecoes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function retornoSelecao(selecao) {
    executarReST("SeparacaoPedido/BuscarPorCodigo", selecao, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                preencherSelecao(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function preencherSelecao(separacaoPedido) {
    var dataSeparacaoPedido = { Data: separacaoPedido.SeparacaoPedido };
    PreencherObjetoKnout(_separacaoPedido, dataSeparacaoPedido);
    preencherInformacoesSeparacao(separacaoPedido.SeparacaoPedido);

    SetarEtapasSeparacaoPedido();

    _pesquisaSeparacaoPedido.Codigo.val(_separacaoPedido.Codigo.val());
    _separacaoInformacoes.NotasIntegradas.val(separacaoPedido.SeparacaoPedido.NotasIntegradas);
    _separacaoInformacoes.SelecionarNotas.val(separacaoPedido.SeparacaoPedido.SelecionarNotasParaIntegracao);

    buscarPedidosMontagem();

    desmarcarKnoutsPedido();
    for (var i = 0; i < separacaoPedido.SeparacaoPedido.Pedidos.length; i++) {
        var pedido = separacaoPedido.SeparacaoPedido.Pedidos[i];
        var index = obterIndiceKnoutPedido(pedido);
        if (index >= 0) {
            _knoutsPedidos[index].InfoPedido.cssClass("well well-pedido-selecionada no-padding padding-5");
        }
    }
    VerificarCompatibilidasKnoutsPedido();

    ValidarBotoes();
    recarregarSeparacaoPedidoIntegracoes();
}

function pesquisarPedidos() {

    limparDadosSelecao();
    buscarPedidosMontagem();
}

function buscarSelecoes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSelecao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSelecoes = new GridView(_pesquisaSelecoes.Pesquisar.idGrid, "SeparacaoPedido/Pesquisa", _pesquisaSelecoes, menuOpcoes, null);
    _gridSelecoes.CarregarGrid();
}

function editarSelecao(data) {
    limparDadosSelecao();
    _separacaoPedido.Codigo.val(data.Codigo);
    data = { Codigo: _separacaoPedido.Codigo.val() };
    _pesquisaSelecoes.ExibirFiltrosPesquisa.visibleFade(false);
    retornoSelecao(data);
}

function limparDadosSelecao() {
    $("#" + _AreaPedido.Pedido.id).html("");
    _AreaPedido.Inicio.val(0);
    _knoutsPedidos = new Array();
    var possui = false;
    if (_separacaoPedido.Codigo.val() > 0 || _separacaoPedido.Pedidos.val().length == 0) 
        possui = true;
    
    _pesquisaSeparacaoPedido.Codigo.val(0);
    LimparCampos(_separacaoPedido);

    if (possui) {
        _separacaoPedido.Pedidos.val(new Array());
    }

    _crudSeparacaoPedido.Cancelar.visible(false);
    _separacaoInformacoes.EnviarSeparacao.visible(false);
    _crudSeparacaoPedido.Salvar.visible(true);
    _crudSeparacaoPedido.Limpar.visible(true);

    SetarEtapaInicioSeparacaoPedido();
    recarregarSeparacaoPedidoIntegracoes();
}

function ValidarBotoes() {
    _separacaoInformacoes.EnviarSeparacao.visible(false);
    _crudSeparacaoPedido.Cancelar.visible(false);
    _crudSeparacaoPedido.Salvar.visible(true);
    _crudSeparacaoPedido.Limpar.visible(true);

    if (_separacaoPedido.Situacao.val() === EnumSituacaoSeparacaoPedido.Aberto) {
        if (_separacaoPedido.Codigo.val() > 0)
            _separacaoInformacoes.EnviarSeparacao.visible(true);
        else
            _separacaoInformacoes.EnviarSeparacao.visible(false);
        _crudSeparacaoPedido.Salvar.visible(true);
        _crudSeparacaoPedido.Cancelar.visible(true);
    } else {
        _separacaoInformacoes.EnviarSeparacao.visible(false);
        _crudSeparacaoPedido.Salvar.visible(false);
    }
}