/*VeiculoMonitoramento.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />


var _gridVeiculoMonitoramento;
var _pesquisaVeiculoMonitoramento;
var _cabecalhoVeiculoMonitoramento;

/*
 * Declaração das Classes
 */
var PesquisaVeiculoMonitoramento = function () {

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaVeiculoMonitoramento)) {

                loadGridVeiculoMonitoramento();

                Global.fecharModal('divModalFiltrosPesquisa');
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: Localization.Resources.Veiculos.VeiculoMonitoramento.Filtrar, idGrid: guid(), visible: ko.observable(true)
    });

    this.PlacaVeiculo = PropertyEntity({ text: Localization.Resources.Veiculos.VeiculoMonitoramento.Placa.getFieldDescription(), col: 12 });
    this.TipoVeiculo = PropertyEntity({ visible: ko.observable(true), al: ko.observable("-1"), options: EnumTipoVeiculo.obterOpcoesPesquisa(), def: "-1", text: Localization.Resources.Veiculos.VeiculoMonitoramento.TipoVeiculo.getFieldDescription() });
    this.Transportador = PropertyEntity({ visible: ko.observable(true), type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Veiculos.VeiculoMonitoramento.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid() });
    this.TecnologiaRastreador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Veiculos.VeiculoMonitoramento.Tecnologia.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SomenteVeiculosAtivos = PropertyEntity({ visible: ko.observable(true), val: ko.observable(true), text: Localization.Resources.Veiculos.VeiculoMonitoramento.SomenteVeiculosAtivos, getType: typesKnockout.bool });
    this.TipoPropriedade = PropertyEntity({ visible: ko.observable(false), val: ko.observable(EnumTipoPropriedadeVeiculo.Todos), text: Localization.Resources.Veiculos.VeiculoMonitoramento.PropriedadeVeiculo.getFieldDescription(), getType: typesKnockout.bool, def: EnumTipoPropriedadeVeiculo.Todos, options: EnumTipoPropriedadeVeiculo.obterOpcoesPesquisa() });
    this.Terminal = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Veiculos.VeiculoMonitoramento.Terminal.getFieldDescription() });
    this.DataPosicao = PropertyEntity({ visible: ko.observable(true), text: Localization.Resources.Veiculos.VeiculoMonitoramento.DataPosicaoApartirDe.getFieldDescription(), getType: typesKnockout.date });
    this.RastreadorPosicionado = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), def: false, text: Localization.Resources.Veiculos.VeiculoMonitoramento.RastreadorPosicionado, getType: typesKnockout.bool });
    this.VeiculoOnlineOffline = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });
    this.VeiculoOffline = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });
    this.VeiculoTotal = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });

}

var CabecalhoVeiculoMonitoramento = function () {
    this.Grid = PropertyEntity({ type: types.event, idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            Global.abrirModal("divModalFiltrosPesquisa");
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Total = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), eventClick: ClickTotal });
    this.Online = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), eventClick: ClickOnline });
    this.Offline = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), eventClick: ClickOffline });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadVeiculoMonitoramento() {
    loadPesquisaVeiculoMonitoramento()
    loadCabecalhoVeiculoMonitoramento();

    new BuscarTransportadores(_pesquisaVeiculoMonitoramento.Transportador, null, null, true);
    new BuscarTecnologiaRastreador(_pesquisaVeiculoMonitoramento.TecnologiaRastreador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaVeiculoMonitoramento.Transportador.visible(false);
        _pesquisaVeiculoMonitoramento.TipoPropriedade.visible(true);
    }
    $("#veiculo-monitoramento-container").hide();
}

function loadGridVeiculoMonitoramento() {
    var limiteRegistros = 20;
    var totalRegistrosPorPagina = 20;

    var configuracoesExportacao = { url: "VeiculoMonitoramento/ExportarPesquisa", titulo: "VeiculoMonitoramento" };
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    var ordenacaInicial = { column: 0, dir: orderDir.asc };
    _gridVeiculoMonitoramento = new GridView("grid-veiculo-monitoramento", "VeiculoMonitoramento/Pesquisa", _pesquisaVeiculoMonitoramento, undefined, ordenacaInicial,
        totalRegistrosPorPagina, null, true, false, multiplaescolha,
        limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, callbackRowVeiculoMonitoramento);

    _gridVeiculoMonitoramento.SetPermitirEdicaoColunas(true);
    _gridVeiculoMonitoramento.SetSalvarPreferenciasGrid(true);
    _gridVeiculoMonitoramento.CarregarGrid();

    ObterInformacoesVeiculos();

    $('#carousel-filters').show();
    $("#veiculo-monitoramento-container").show();
}

function getSelectedVehicle() {
    var veiculoSelecionado = _gridVeiculoMonitoramento.ObterMultiplosSelecionados();
    return veiculoSelecionado.length > 0 ? veiculoSelecionado[0] : undefined;
}
function loadPesquisaVeiculoMonitoramento() {
    _pesquisaVeiculoMonitoramento = new PesquisaVeiculoMonitoramento();
    KoBindings(_pesquisaVeiculoMonitoramento, "knockoutPesquisaVeiculoMonitoramento", false, _pesquisaVeiculoMonitoramento.Pesquisar.id);
}

function loadCabecalhoVeiculoMonitoramento() {
    _cabecalhoVeiculoMonitoramento = new CabecalhoVeiculoMonitoramento();
    KoBindings(_cabecalhoVeiculoMonitoramento, "knockoutCabecalhoVeiculoMonitoramento");
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function consultarVeiculoPorPlaca() {
    var veiculo = getSelectedVehicle();
    if (veiculo != undefined) {
        sessionStorage.setItem('placaVeiculo', veiculo.Placa);
        window.open("#Veiculos/Veiculo", '_blank');
    } else {
         exibirMensagem("atencao", "Seleção de veículo necessária", "Por favor selecione algum veículo");
    }
}


/*
 * Declaração das Funções
 */
function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
}

function gerarColunaRastreador(nRow, aData) {
    var indice = _gridVeiculoMonitoramento.GetColumnIndex('Rastreador');
    if (indice == undefined) return;
    var colunaRastreador = $(nRow).find('td').eq(indice);
    if (colunaRastreador) {
        color = "#e74c3c";

        if (aData.Rastreador == true)
            var color = "#33cc33";

        var icone =
            ' <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="20" height="20" viewBox="0 0 172 172" ' +
            ' style=" fill:#000000;"><g fill="none" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt" stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0" font-family="none" font-weight="none" font-size="none" text-anchor="none" style="mix-blend-mode: normal"><path d="M0,172v-172h172v172z" fill="none"></path>' +
            '<g fill="' + color + '"><path d="M86,14.33333c-39.5815,0 -71.66667,32.08517 -71.66667,71.66667c0,39.5815 32.08517,71.66667 71.66667,71.66667c39.5815,0 71.66667,-32.08517 71.66667,-71.66667c0,-39.5815 -32.08517,-71.66667 -71.66667,-71.66667zM78.83333,28.66667h14.33333v57.33333h-14.33333zM86,143.33333c-31.61217,0 -57.33333,-25.72117 -57.33333,-57.33333c0,-24.00833 14.84933,-44.58383 35.83333,-53.11217v15.9315c-12.82833,7.44617 -21.5,21.3065 -21.5,37.18067c0,23.7145 19.2855,43 43,43c23.7145,0 43,-19.2855 43,-43c0,-15.87417 -8.67167,-29.7345 -21.5,-37.18067v-15.9315c20.984,8.52833 35.83333,29.10383 35.83333,53.11217c0,31.61217 -25.72117,57.33333 -57.33333,57.33333z"></path></g></g></svg >';

        var html = '<div>' + icone + '</div>';
        $(colunaRastreador).addClass('rastreador');
        $(colunaRastreador).html(html);
    }
}

function callbackRowVeiculoMonitoramento(nRow, aData) {
    gerarColunaRastreador(nRow, aData);
}


function ObterInformacoesVeiculos() {
    var data = RetornarObjetoPesquisa(_pesquisaVeiculoMonitoramento);
    executarReST("VeiculoMonitoramento/ObterEstatisticasVeiculos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data.VeiculosTotal != undefined) {
                    _cabecalhoVeiculoMonitoramento.Total.val(arg.Data.VeiculosTotal);
                    _cabecalhoVeiculoMonitoramento.Online.val(arg.Data.VeiculosOnline);
                    _cabecalhoVeiculoMonitoramento.Offline.val(arg.Data.VeiculosOffline);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, false);
}

this.ClickOnline = PropertyEntity({
    eventClick: function (e) {
        if (e === undefined) return;
        _pesquisaVeiculoMonitoramento.VeiculoOnlineOffline.val(1);

        loadGridVeiculoMonitoramento()
        // Atualiza visualmente os botões
        $("#view-offline").removeClass("active");
        $("#view-total").removeClass("active");
        $("#view-online").addClass("active");

        desativeBotoes();
    }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
});

this.ClickOffline = PropertyEntity({
    eventClick: function (e) {
        if (e === undefined) return;
        _pesquisaVeiculoMonitoramento.VeiculoOnlineOffline.val(0);

        loadGridVeiculoMonitoramento()
        // Atualiza visualmente os botões
        $("#view-online").removeClass("active");
        $("#view-total").removeClass("active");
        $("#view-offline").addClass("active");

        desativeBotoes();
    }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
});

this.ClickTotal = PropertyEntity({
    eventClick: function (e) {
        if (e === undefined) return;
        _pesquisaVeiculoMonitoramento.VeiculoOnlineOffline.val(2);

        loadGridVeiculoMonitoramento()
        // Atualiza visualmente os botões
        // Atualiza visualmente os botões
        $("#view-online").removeClass("active");
        $("#view-total").removeClass("active");
        $("#view-offline").addClass("active");

        desativeBotoes();
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
});


function desativeBotoes() {
    $("#view-total").removeClass("active");
    $("#view-online").removeClass("active");
    $("#view-offline").removeClass("active");
}