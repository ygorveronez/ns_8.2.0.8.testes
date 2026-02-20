/*MonitoramentoTecnologia.js*/
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />

//var _gridVeiculoMonitoramento;
var _pesquisaMonitoramentoTecnologia;
var _tabelaRastreadores;
var _versaoMonitoramento;

/*
 * Declaração das Classes
 */
var PesquisaMonitoramentoTecnologia = function () {

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaMonitoramentoTecnologia)) {

                 loadTabelaRastreadores();
                _pesquisaMonitoramentoTecnologia.ExibirFiltros.visibleFade(false);

            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.TecnologiaRastreador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tecnologia:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SomenteRastreadoresAtivos = PropertyEntity({ visible: ko.observable(true), val: ko.observable(true), text: "Somente ativos ", getType: typesKnockout.bool });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


function loadMonitoramentoTecnologia() {
    _pesquisaMonitoramentoTecnologia = new PesquisaMonitoramentoTecnologia();
    //KoBindings(_pesquisaMonitoramentoTecnologia, "knockoutPesquisaMonitoramentoTecnologia", false, _pesquisaMonitoramentoTecnologia.Pesquisar.id);

    //new BuscarTecnologiaRastreador(_pesquisaMonitoramentoTecnologia.TecnologiaRastreador);

    _tabelaRastreadores = new TabelaRastreadores();
    KoBindings(_tabelaRastreadores, "knoutContainerRastreadores");

    _versaoMonitoramento = new VersaoMonitoramento();
    KoBindings(_versaoMonitoramento, "versao-monitoramento");


    loadTabelaRastreadores();
}

var TabelaRastreadores = function () {
    this.Rastreadores = ko.observableArray([]);
};

var Rastreador = function (rastreador, index) {
    this.CodigoPosicao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Index = PropertyEntity({ val: ko.observable(index), def: index, getType: typesKnockout.int });
    this.Tecnologia = PropertyEntity({ val: ko.observable("") });
    this.Logo = PropertyEntity({ val: ko.observable("") });
    this.DataProcessada = PropertyEntity({  val: ko.observable("") });
    this.DataRecebida = PropertyEntity({ val: ko.observable("") });
    this.Latitude = PropertyEntity({ val: ko.observable("") });
    this.Longitude = PropertyEntity({  val: ko.observable("") });
    this.Endereco = PropertyEntity({  val: ko.observable("") });
    this.Placa = PropertyEntity({ val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable("") });
    this.Rastreador = PropertyEntity({ val: ko.observable(false) });

    PreencherObjetoKnout(this, { Data: rastreador });
}

var VersaoMonitoramento = function () {
    this.Versao = PropertyEntity({ val: ko.observable("") });
};
function loadTabelaRastreadores() {
    executarReST("MonitoramentoTecnologia/BuscarRastreadores", RetornarObjetoPesquisa(_pesquisaMonitoramentoTecnologia), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                // PreencherObjetoKnout(_tabelaRastreadores, { Data: retorno.Data.Rastreadores });
                _tabelaRastreadores.Rastreadores.removeAll();
                _versaoMonitoramento.Versao.val(retorno.Data.Versao);

                for (var i = 0; i < retorno.Data.Rastreadores.length; i++) {
                    var rastreador = retorno.Data.Rastreadores[i];

                    var knoutRastreador = new Rastreador(rastreador, i);

                    _tabelaRastreadores.Rastreadores.push(knoutRastreador);
                }

                for (var i = 0; i < retorno.Data.Gerenciadores.length; i++) {
                    var gerenciador = retorno.Data.Gerenciadores[i];

                    var knoutGerenciador = new Rastreador(gerenciador, i);

                    _tabelaRastreadores.Rastreadores.push(knoutGerenciador);
                }

            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */
function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function gerarColunaRastreador(nRow, aData) {
    var indice = _gridVeiculoMonitoramento.GetColumnIndex('Rastreador');
    if (indice == undefined) return;
    var colunaRastreador = $(nRow).find('td').eq(indice);
    if (colunaRastreador) {
        color = "#e74c3c";
        if (aData.Rastreador)
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
