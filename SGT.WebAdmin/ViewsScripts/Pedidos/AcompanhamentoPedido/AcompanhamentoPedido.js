/*AcompanhamentoPedido.js*/
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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Pedidos.js" />
/// <reference path="DadosPedido.js" />
/// <reference path="DadosEntrega.js" />
/// <reference path="DadosColeta.js" />
/// <reference path="DadosTransporte.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _pesquisaAcompanhamentoPedido;
var _acompanhamentoPedido;
var _itensPorPaginaAcompanhamentoPedido = 10;
var _listaKnockout = [];

var PesquisaAcompanhamentoPedido = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) || (!_fornecedorTMS)) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor) || (!_fornecedorTMS)) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NaoMostrarCargasDeslocamentoVazio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NaoMostrarCargasDeslocamentoVazio, type: types.bool, visible: ko.observable(false), val: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial.getFieldDescription(), getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal.getFieldDescription(), getType: typesKnockout.date });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoCargaEmbarcador, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Pedido.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.NotaFiscal.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Placa.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SituacaoAcompanhamentoPedido = PropertyEntity({ val: ko.observable(EnumSituacaoAcompanhamentoPedido.Todos), options: EnumSituacaoAcompanhamentoPedido.obterOpcoesPesquisa(), def: EnumSituacaoAcompanhamentoPedido.Todas, text: Localization.Resources.Cargas.ControleEntrega.SituacaoAcompanhamentoPedido, visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CentroResultado.getFieldDescription(), idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            ObterAcompanhamentoPedidos(1, false);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var AcompanhamentoPedido = function () {
    this.Pedidos = PropertyEntity({ val: ko.observable([]) });
}

//*******MÉTODOS*******

function carregarHTMLComponenteAcompanhamentoPedido(callback) {
    $.get('Content/Static/Pedido/AcompanhamentoPedido/ComponenteAcompanhamentoPedido.html?dyn=' + guid(), function (html) {
        $('#ComponenteAcompanhamentoPedidoContent').html(html);
        callback();
    });
}

//function RegistraComponenteAcompanhamentoPedido() {
//    if (ko.components.isRegistered('acompanhamento-pedido'))
//        return;

//    ko.components.register('acompanhamento-pedido', {
//        viewModel: EtapaAcompanhamentoPedido,
//        template: {
//            element: 'acompanhamento-pedido-templete'
//        }
//    });
//}

function loadAcompanhamentoPedido() {
    carregarHTMLComponenteAcompanhamentoPedido(function () {

        RegistraComponenteAcompanhamentoPedido();

        loadEtapaAcompanhamentoPedido();

        _pesquisaAcompanhamentoPedido = new PesquisaAcompanhamentoPedido();
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
            _pesquisaAcompanhamentoPedido.Remetente.visible(_CONFIGURACAO_TMS.CompartilharAcessoEntreGrupoPessoas);
            _pesquisaAcompanhamentoPedido.Destinatario.visible(_CONFIGURACAO_TMS.CompartilharAcessoEntreGrupoPessoas);
            _pesquisaAcompanhamentoPedido.Destinatario.type = types.entity;
        }
        KoBindings(_pesquisaAcompanhamentoPedido, "knoutPesquisaAcompanhamentoPedido");

        _acompanhamentoPedido = new AcompanhamentoPedido();
        KoBindings(_acompanhamentoPedido, "knoutAcompanhamentoPedido");

        new BuscarFilial(_pesquisaAcompanhamentoPedido.Filial);
        BuscarClientesFactory(_pesquisaAcompanhamentoPedido.Remetente, null, { filtrarGrupoFornecedor: true });
        BuscarClientesFactory(_pesquisaAcompanhamentoPedido.Destinatario, null, { filtrarGrupoFornecedor: true });
        new BuscarTransportadores(_pesquisaAcompanhamentoPedido.Transportador);
        new BuscarCentroResultado(_pesquisaAcompanhamentoPedido.CentroResultado);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            _pesquisaAcompanhamentoPedido.Filial.visible(false);

        ObterConfiguracaoPadrao();
    });
}

function ComponentePaginacao(totalRegistros) {
    if (totalRegistros > 0) {
        var $ul = $('<ul class="pagination"></ul>');
        var paginas = Math.ceil(totalRegistros / _itensPorPaginaAcompanhamentoPedido);

        $("#paginacao-acompanhamento-pedido").empty().append($ul);

        _executarPesquisa = false;

        $ul.twbsPagination({
            first: Localization.Resources.Gerais.Geral.Primeiro,
            prev: Localization.Resources.Gerais.Geral.Anterior,
            next: Localization.Resources.Gerais.Geral.Proximo,
            last: Localization.Resources.Gerais.Geral.Ultimo,
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: null,
            onPageClick: function (event, page) {
                if (_executarPesquisa)
                    ObterAcompanhamentoPedidos(page, true);
            }
        });

        _executarPesquisa = true;

    } else {
        $("#paginacao-acompanhamento-pedido").html(`<h3>${Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado}</h3>`);
    }
}

function ObterAcompanhamentoPedidos(page, eventoPorPaginacao) {
    var data = RetornarObjetoPesquisa(_pesquisaAcompanhamentoPedido);

    /*    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS && data.Filial <= 0) && (_pesquisaAcompanhamentoPedido.NumeroPedidoEmbarcador.val() == ""))
            return exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatório informar a filial");*/

    data.inicio = _itensPorPaginaAcompanhamentoPedido * (page - 1);
    data.limite = _itensPorPaginaAcompanhamentoPedido;

    executarReST("AcompanhamentoPedido/ObterAcompanhamentoPedido", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _mapDadosTransporte = null;
                _acompanhamentoPedido.Pedidos.val(arg.Data);

                _listaKnockout = [];

                if (!eventoPorPaginacao) {
                    ComponentePaginacao(arg.QuantidadeRegistros);
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function findListaKnockout(valor) {
    for (var i = 0; i < _listaKnockout.length; i++) {
        if (_listaKnockout[i] == valor)
            return true;
    }
    return false;
}

function ObterConfiguracaoPadrao() {
    executarReST("AcompanhamentoPedido/ObterConfiguracaoGeral", {}, function (r) {
        if (r.Success && r.Data) {
            _pesquisaAcompanhamentoPedido.NaoMostrarCargasDeslocamentoVazio.visible(r.Data.MostrarNoAcompanhamentoDePedidosDeslocamentoVazio);
        }
    });
}