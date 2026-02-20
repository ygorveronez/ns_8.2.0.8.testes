/// <reference path="../../enumeradores/enumsituacaofechamentocarga.js" />
/*CargaFechamento.js*/
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumEtapaCarga.js" />

/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />

var _gridCargasFechamento;
var _pesquisaCargasFechamento;

/*
 * Declaração das Classes
 */

var PesquisaCargasFechamento = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaCargasFechamento)) {
                _pesquisaCargasFechamento.ExibirFiltros.visibleFade(false);
                _gridCargasFechamento.CarregarGrid();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFechamentoCarga.Todas), options: EnumSituacaoFechamentoCarga.ObterOpcoesPesquisa(), def: EnumSituacaoFechamentoCarga.Todas, text: "Situação:" });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: "Pedido Embarcador:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};


function loadPesquisaCargasPendentes() {
    _pesquisaCargasFechamento = new PesquisaCargasFechamento();
    KoBindings(_pesquisaCargasFechamento, "knockoutPesquisaCargas", false, _pesquisaCargasFechamento.Pesquisar.id);
}


function loadGridCargas() {
    var draggableRows = false;
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 50;

    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: visualizarDetalhesCargaClick, tamanho: "7", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    _gridCargasFechamento = new GridView("grid-cargas_fechamento", "CargaFechamento/Pesquisa", _pesquisaCargasFechamento, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, null, limiteRegistros, undefined, null);
    _gridCargasFechamento.CarregarGrid();
}

function loadCargaFechamento() {
    loadPesquisaCargasPendentes();
    loadGridCargas();

    loadDetalhesCarga();
}

function loadDetalhesCarga() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
        });
    });
}

function visualizarDetalhesCargaClick(filaSelecionada) {
    atualizaTituloModalCarga(filaSelecionada);
    ObterDetalhesCargaFluxo(filaSelecionada.CodigoCarga);
}


function atualizaTituloModalCarga(carga) {
    $(".title-carga-codigo-embarcador").html(carga.CargaEmbarcador);
    $(".title-carga-placa").html(carga.Tracao);
}

function ObterDetalhesCargaFluxo(carga) {
    if (carga == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não existe carga para este veículo.");
        return;
    }

    var data = { Carga: carga };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
               
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

