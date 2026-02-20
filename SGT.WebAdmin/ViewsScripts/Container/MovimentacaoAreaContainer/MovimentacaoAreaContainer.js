/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoCargaEntrega.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="MovimentacaoAreaContainerConfirmacao.js" />

var _painelContainer,
    _pesquisaMovimentacaoAreaContainer;

var _registroSelecionado;

var PesquisaMovimentacaoAreaContainer = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Número Carga:" });
    this.Motorista = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Tipo Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Origem = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Origem:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Destino = PropertyEntity({ codEntity: ko.observable(0), required: false, type: types.entity, text: "Destino:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.SituacaoEntrega = PropertyEntity({ val: ko.observable(EnumSituacaoEntrega.NaoEntregue), def: EnumSituacaoEntrega.NaoEntregue, options: EnumSituacaoEntrega.obterOpcoesPesquisaContainer(), text: "Situação:" });
    this.TipoCargaEntrega = PropertyEntity({ val: ko.observable(EnumTipoCargaEntrega.Todos), def: EnumTipoCargaEntrega.Todos, options: EnumTipoCargaEntrega.obterOpcoesPesquisaAreaContainer(), text: "Tipo de Movimentação:" });
    this.NumeroEXP = PropertyEntity({text: "Número EXP: "});
    this.NumeroContainer = PropertyEntity({text: "Número Container: "});
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarMovimentacaoAreaContainer(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            }
            else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PainelContainer = function () {
    this.Entregas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });
}

var CardContainer = function (data) {
    this.Codigo = PropertyEntity({ val: data.Codigo });
    this.CodigoCarga = PropertyEntity({ val: data.CodigoCarga });
    this.Situacao = PropertyEntity({ val: data.Situacao });
    this.Carga = PropertyEntity({ val: data.Carga });
    this.TipoOperacao = PropertyEntity({ val: data.TipoOperacao });
    this.Veiculo = PropertyEntity({ val: data.Veiculo });
    this.ColetaDeContainer = PropertyEntity({ val: data.ColetaDeContainer });
    this.IsColeta = PropertyEntity({ val: data.IsColeta });
    this.TipoDescricao = PropertyEntity({ val: data.TipoDescricao });
    this.Armador = PropertyEntity({ val: data.Armador });
    this.LocalRetiradaContainer = PropertyEntity({ val: data.LocalRetiradaContainer });
    this.DescricaoTipoContainerCarga = PropertyEntity({ val: data.DescricaoTipoContainerCarga });
    this.CodigoContainerRetirar = PropertyEntity({ val: data.CodigoContainerRetirar });
    this.DescricaoContainerRetirar = PropertyEntity({ val: data.DescricaoContainerRetirar });
    this.TipoContainerCarga = PropertyEntity({ val: data.TipoContainerCarga });
    this.NumeroExp = PropertyEntity({ val: data.NumeroExp });
    this.AreasRedex = PropertyEntity({ val: data.AreasRedex });
    this.ClientePossuiAreaRedex = PropertyEntity({ val: data.ClientePossuiAreaRedex });

    this.Classe = PropertyEntity({ val: "margin-bottom-10 panel panel-" + (data.Situacao == EnumSituacaoEntrega.Entregue ? "success" : "primary") });

    this.DadosCarga = PropertyEntity({ type: types.event, eventClick: dadosCargaClick, text: "Dados da Carga" });
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: abrirModalMovimentacaoAreaContainerConfirmacao, text: "Confirmar", visible: ko.observable(data.Situacao != EnumSituacaoEntrega.Entregue) });
}

function loadMovimentacaoAreaContainer() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLComponenteControleEntrega(function () {
                _painelContainer = new PainelContainer();
                KoBindings(_painelContainer, "knockoutMovimentacaoAreaContainer");

                HeaderAuditoria("CargaEntrega", _painelContainer);

                _pesquisaMovimentacaoAreaContainer = new PesquisaMovimentacaoAreaContainer();
                KoBindings(_pesquisaMovimentacaoAreaContainer, "knockoutPesquisaMovimentacaoAreaContainer");

                new BuscarMotoristas(_pesquisaMovimentacaoAreaContainer.Motorista);
                new BuscarTiposOperacao(_pesquisaMovimentacaoAreaContainer.TipoOperacao);
                new BuscarVeiculos(_pesquisaMovimentacaoAreaContainer.Veiculo);
                new BuscarLocalidades(_pesquisaMovimentacaoAreaContainer.Origem);
                new BuscarLocalidades(_pesquisaMovimentacaoAreaContainer.Destino);

                loadMovimentacaoAreaContainerConfirmacao();
                loadCargaVeiculoContainerAnexo();
                registrarComponente();
                //buscarMovimentacaoAreaContainer(1, false);
            });
        });
    });
}

function registrarComponente() {
    if (ko.components.isRegistered('carga-card-container'))
        return;

    ko.components.register('carga-card-container', {
        template: {
            element: "carga-card-container"
        }
    });
}

//#region Eventos

function buscarMovimentacaoAreaContainer(page, paginou) {
    var itensPorPagina = 10;
    var data = RetornarObjetoPesquisa(_pesquisaMovimentacaoAreaContainer);

    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    executarReST("MovimentacaoAreaContainer/BuscarMovimentacaoAreaContainer", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _painelContainer.Entregas.val.removeAll();
                _pesquisaMovimentacaoAreaContainer.ExibirFiltros.visibleFade(false);

                for (var i = 0; i < retorno.Data.CargasEntrega.length; i++) {
                    var data = retorno.Data.CargasEntrega[i];
                    var cardContainer = new CardContainer(data);
                    _painelContainer.Entregas.val.push(cardContainer);
                }

                configurarPaginacao(page, paginou, retorno, itensPorPagina);
                window.scrollTo(0, 0);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirMenuClick(conteudo, e) {
    _registroSelecionado = conteudo;

    var $menu = $("#ulMenuControleContainerOperador");
    var $menuConfirmarEntrega = $menu.find('#liMenuConfirmarEntrega');

    $menuConfirmarEntrega.removeClass('hidden');

    if (_registroSelecionado.Situacao != EnumSituacaoEntrega.NaoEntregue)
        $menuConfirmarEntrega.addClass('hidden');

    var $elemento = $(e.target.parentElement)[0];
    var offsetX = $("aside").width();
    var offsetY = $("header#header").height();

    $menu.css({
        display: 'block',
        top: $elemento.getBoundingClientRect().top + window.scrollY - offsetY + 10,
        left: $elemento.getBoundingClientRect().left + window.scrollX - offsetX + 15
    });

    var mouseLeaveHandle = function () {
        var $this = $(this);

        var timeOut = setTimeout(function () {
            $this.hide();

            $menu.off("mouseleave", mouseLeaveHandle);
            _registroSelecionado = null;
        }, 400);

        $menu.one("mouseenter", function () {
            clearTimeout(timeOut);
        });
    };

    $menu.on("mouseleave", mouseLeaveHandle);
}

function dadosCargaClick(e) {
    var data = { Carga: e.CodigoCarga.val };

    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                _cargaAtual = knoutCarga;
                Global.abrirModal('divModalDetalhesCargaFDS');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function confirmarEntregaColetaControleContainerOperadorClick() {

}

//#endregion

//#region Métodos Privados

function configurarPaginacao(page, paginou, retorno, itensPorPagina) {
    var clicouNoPaginar = false;

    if (!paginou) {
        if (retorno.QuantidadeRegistros > 0) {
            $("#divPaginationMovimentacaoAreaContainer").html('<ul style="float:right" id="paginacaoMovimentacaoAreaContainer" class="pagination"></ul>');
            var paginas = Math.ceil((retorno.QuantidadeRegistros / itensPorPagina));
            $('#paginacaoMovimentacaoAreaContainer').twbsPagination({
                first: 'Primeiro',
                prev: 'Anterior',
                next: 'Próximo',
                last: 'Último',
                totalPages: paginas,
                visiblePages: 5,
                onPageClick: function (event, page) {
                    if (clicouNoPaginar)
                        buscarMovimentacaoAreaContainer(page, true);
                    clicouNoPaginar = true;
                }
            });
        }
        else
            $("#divPaginationMovimentacaoAreaContainer").html('<span>Nenhum Registro Encontrado</span>');
    }
}

//#endregion