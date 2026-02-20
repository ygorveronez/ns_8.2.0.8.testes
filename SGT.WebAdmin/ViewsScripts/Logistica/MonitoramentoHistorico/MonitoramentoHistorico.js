/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../Tracking/Tracking.lib.js" />

// #region Objetos Globais do Arquivo

var _graficoTemperatura;
var _graficoVelocidade;
var _gridMapaHistoricosParada;
var _gridMapaHistoricosPosicao;
var _historicoMonitoramento;
var _mapaHistoricos;
var _mapaPosicao;
var _pesquisaHistoricosMapa;
var _pesquisaHistoricosPosicao;
var _pesquisaHistoricosTemperatura;
var _pesquisaHistoricosVelocidade;

// #endregion Objetos Globais do Arquivo

// #region Classes

var HistoricoMonitoramento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var PesquisaHistoricos = function (pesquisaFunction) {
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicial, getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFinal, getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.PosicaoTemperaturaValida = PropertyEntity({ text: Localization.Resources.Cargas.MonitoramentoHistorico.ApenasPosicoesTemperaturaEnviada, val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });

    this.PosicaoTecnologia = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MonitoramentoHistorico.ApenasPosicaoTecnologia), val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });
    this.PosicaoMobile = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MonitoramentoHistorico.ApenasPosicaoMobile), val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisaFunction();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadHistoricoMonitoramento() {
    loadHtmlHistoricoMonitoramento(function () {
        _historicoMonitoramento = new HistoricoMonitoramento();

        _pesquisaHistoricosMapa = new PesquisaHistoricos(carregarDadosHistoricoMonitoramentoMapa);
        KoBindings(_pesquisaHistoricosMapa, "knockoutPesquisaHistoricosMapa", false, _pesquisaHistoricosMapa.Pesquisar.id);

        _pesquisaHistoricosPosicao = new PesquisaHistoricos(carregarDadosHistoricoMonitoramentoPosicao);
        KoBindings(_pesquisaHistoricosPosicao, "knockoutPesquisaHistoricosPosicao", false, _pesquisaHistoricosPosicao.Pesquisar.id);

        _pesquisaHistoricosTemperatura = new PesquisaHistoricos(carregarDadosHistoricoMonitoramentoTemperatura);
        KoBindings(_pesquisaHistoricosTemperatura, "knockoutPesquisaHistoricosTemperatura", false, _pesquisaHistoricosTemperatura.Pesquisar.id);

        _pesquisaHistoricosVelocidade = new PesquisaHistoricos(carregarDadosHistoricoMonitoramentoVelocidade);
        KoBindings(_pesquisaHistoricosVelocidade, "knockoutPesquisaHistoricosVelocidade", false, _pesquisaHistoricosVelocidade.Pesquisar.id);

        _mapaHistoricos = new MapaGoogle("mapHistoricos", false, new OpcoesMapa(false, false));


    });
}

function loadHtmlHistoricoMonitoramento(callback) {
    $.get("Content/Static/Logistica/MonitoramentoHistorico.html?dyn=" + guid(), function (data) {
        var $containerModalMonitoramentoHistorico = $("#containerModalMonitoramentoHistorico");

        if ($containerModalMonitoramentoHistorico.length == 0) {
            $("#js-page-content").append("<div id='containerModalMonitoramentoHistorico'></div>");
            $containerModalMonitoramentoHistorico = $("#containerModalMonitoramentoHistorico");
        }

        $containerModalMonitoramentoHistorico.html(data);
        callback();
    });
}

// #endregion Funções de Inicialização

// #region Métodos Públicos

function exibirHistoricoMonitoramentoPorCarga(codigoCarga) {
    _historicoMonitoramento.Carga.val(codigoCarga);

    carregarDadosHistoricoMonitoramento();
}

function exibirHistoricoMonitoramentoPorCodigo(codigoMonitoramento) {
    _historicoMonitoramento.Codigo.val(codigoMonitoramento);

    carregarDadosHistoricoMonitoramento();
}

// #endregion Métodos Públicos

// #region Funções Privadas

function carregarDadosHistoricoMonitoramento() {
    executarReST("Monitoramento/ObterDetalhesMonitoramentoHistorico", { Monitoramento: _historicoMonitoramento.Codigo.val(), Carga: _historicoMonitoramento.Carga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_historicoMonitoramento, retorno);

                var dataAtual = Global.DataHoraAtual();
                var dataInicial;
                var dataFinal;

                if (retorno.Data.Status != EnumMonitoramentoStatus.Aguardando) {
                    dataInicial = retorno.Data.DataInicioMonitoramento;
                    dataFinal = (retorno.Data.Status == EnumMonitoramentoStatus.Iniciado) ? dataAtual : retorno.Data.DataFimMonitoramento;
                }
                else {
                    dataInicial = dataAtual;
                    dataFinal = dataAtual;
                }

                _pesquisaHistoricosMapa.DataInicial.val(dataInicial);
                _pesquisaHistoricosMapa.DataFinal.val(dataFinal);
                _pesquisaHistoricosPosicao.DataInicial.val(dataInicial);
                _pesquisaHistoricosPosicao.DataFinal.val(dataFinal);
                _pesquisaHistoricosTemperatura.DataInicial.val(dataInicial);
                _pesquisaHistoricosTemperatura.DataFinal.val(dataFinal);
                _pesquisaHistoricosVelocidade.DataInicial.val(dataInicial);
                _pesquisaHistoricosVelocidade.DataFinal.val(dataFinal);

                if (retorno.Data.Tecnologia != "") {
                    _pesquisaHistoricosPosicao.PosicaoTecnologia.text("Posições Tecnologia (" + retorno.Data.Tecnologia + ")");
                    _pesquisaHistoricosMapa.PosicaoTecnologia.text("Posições Tecnologia (" + retorno.Data.Tecnologia + ")");
                }

                $(".title-carga-codigo-embarcador").html(" " + retorno.Data.CargaEmbarcador);
                $(".title-carga-placa").html(retorno.Data.Tracao + " " + retorno.Data.Reboques);

                exibirModalHistoricoMonitoramento();
                carregarDadosHistoricoMonitoramentoMapa();
                carregarDadosHistoricoMonitoramentoPosicao();
                carregarDadosHistoricoMonitoramentoParada();
                carregarDadosHistoricoMonitoramentoTemperatura();
                carregarDadosHistoricoMonitoramentoVelocidade();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function carregarDadosHistoricoMonitoramentoMapa() {
    _mapaHistoricos.clear();
    executarReST("Monitoramento/ObterDadosMapaHistoricoPosicao", {
        Codigo: _historicoMonitoramento.Codigo.val(),
        DataInicial: _pesquisaHistoricosMapa.DataInicial.val(),
        DataFinal: _pesquisaHistoricosMapa.DataFinal.val(),
        PosicaoMobile: _pesquisaHistoricosMapa.PosicaoMobile.val(),
        PosicaoTecnologia: _pesquisaHistoricosMapa.PosicaoTecnologia.val()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                setTimeout(function () {
                    TrackingDesenharInformacoesMapa(_mapaHistoricos, arg.Data);
                    var ultimo = false;
                    for (var i = 0; i < arg.Data.Posicoes.length; i++) {
                        if (i == arg.Data.Posicoes.length - 1) {
                            ultimo = true;
                        }
                        criarMakerHistoricoMonitoramentoPosicao(arg.Data.Posicoes[i], ultimo);
                    }
                    _mapaHistoricos.draw.centerShapes();

                    TrackingCriarMarkerVeiculo(_mapaHistoricos, arg.Data.Veiculo, false, 0);

                    if (arg.Data.PosicaoInicioPreTrip.Latitude != 0 && arg.Data.PosicaoInicioPreTrip.Longitude != 0)
                        criarMakerPreTrip(arg.Data.PosicaoInicioPreTrip);

                }, 500);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function carregarDadosHistoricoMonitoramentoParada() {
    var configuracoesExportacao = { url: "Monitoramento/ExportarParadas?codigo=" + _historicoMonitoramento.Codigo.val(), titulo: "ParadasCarga" };
    _gridMapaHistoricosParada = new GridView("grid-historicos-parada", "Monitoramento/ObterParadas?codigo=" + _historicoMonitoramento.Codigo.val(), null, null, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridMapaHistoricosParada.CarregarGrid();
}

function carregarDadosHistoricoMonitoramentoPosicao() {
    var opcaoSelecionar = { descricao: "<i class=\"fas fa-map-marker-alt\" style=\"font-size:25px\"></i>", id: guid(), evento: "onclick", metodo: montarVisualizarMapaPosicao, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoSelecionar]
    };

    var configuracoesExportacao = { url: "Monitoramento/ExportarHistoricoPosicao?codigo=" + _historicoMonitoramento.Codigo.val(), titulo: "HistoricoPosicoesCarga" };
    _gridMapaHistoricosPosicao = new GridView("grid-historicos-posicao", "Monitoramento/ObterHistoricoPosicao?codigo=" + _historicoMonitoramento.Codigo.val(), _pesquisaHistoricosPosicao, menuOpcoes, null, 10, null, true, null, null, 10000, true, configuracoesExportacao, null, true, null, false);
    _gridMapaHistoricosPosicao.CarregarGrid();
}
function montarVisualizarMapaPosicao(info) {
    Global.abrirModal("divModalMapaPosicao");
    _mapaPosicao = new MapaGoogle("mapaPosicaoMonitoramentoHistorico", false, new OpcoesMapa(false, false));

    _mapaPosicao.direction.setZoom(17);
    _mapaPosicao.direction.centralizar(info.Latitude, info.Longitude);

    var marker = new ShapeMarker();
    marker.setPosition(info.Latitude, info.Longitude);
    marker.icon = _mapaPosicao.draw.icons.point();

    var shape = _mapaPosicao.draw.addShape(marker);


}

function carregarDadosHistoricoMonitoramentoTemperatura() {
    executarReST("Monitoramento/ObterHistoricoTemperatura", {
        Codigo: _historicoMonitoramento.Codigo.val(),
        DataInicial: _pesquisaHistoricosTemperatura.DataInicial.val(),
        DataFinal: _pesquisaHistoricosTemperatura.DataFinal.val(),
        PosicoesValidas: _pesquisaHistoricosTemperatura.PosicaoTemperaturaValida.val(),
    }, function (retorno) {
        if (retorno.Success) {
            if (_graficoTemperatura) _graficoTemperatura.destroy();
            var temperaturas = new Array();
            var datas = new Array();
            for (var i = 0; i < retorno.Data.Temperaturas.length; i++) {
                temperaturas.push(retorno.Data.Temperaturas[i].Temperatura);
                datas.push(retorno.Data.Temperaturas[i].Data);
            }
            var annotations = [];
            if (retorno.Data.FaixaInicial != null) {
                annotations.push({
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-1',
                    value: retorno.Data.FaixaInicial,
                    borderColor: 'blue',
                    borderWidth: 2,
                    borderDash: [2, 2],
                    borderDashOffset: 5,
                    label: {
                        enabled: true,
                        content: Localization.Resources.Cargas.MonitoramentoHistorico.FaixaInicial,
                        fontSize: 10,
                        fontStyle: "normal",
                        fontColor: "blue",
                        backgroundColor: '#FFF',
                        position: "left",
                        xPadding: 0,
                        yPadding: 0,
                        xAdjust: 5,
                        yAdjust: -7,
                    }
                });
            }
            if (retorno.Data.FaixaFinal != null) {
                annotations.push({
                    type: 'line',
                    mode: 'horizontal',
                    scaleID: 'y-axis-1',
                    value: retorno.Data.FaixaFinal,
                    borderColor: 'red',
                    borderWidth: 2,
                    borderDash: [2, 2],
                    borderDashOffset: 5,
                    label: {
                        enabled: true,
                        content: Localization.Resources.Cargas.MonitoramentoHistorico.FaixaFinal,
                        fontSize: 10,
                        fontStyle: "normal",
                        fontColor: "red",
                        backgroundColor: '#FFF',
                        position: "left",
                        xPadding: 0,
                        yPadding: 0,
                        xAdjust: 5,
                        yAdjust: 7,
                    }
                });
            }

            var lineTemperaturas = {
                labels: datas,
                datasets: [{
                    label: Localization.Resources.Cargas.MonitoramentoHistorico.Temperatura,
                    borderColor: "green",
                    backgroundColor: "green",
                    fill: false,
                    data: temperaturas,
                    yAxisID: 'y-axis-1',
                }]
            };

            var config1 = {
                data: lineTemperaturas,
                options: {
                    maintainAspectRatio: false,
                    hoverMode: 'index',
                    stacked: false,
                    title: {
                        display: false,
                    },
                    elements: {
                        line: {
                            tension: 0
                        }
                    },
                    legend: {
                        display: false,
                        position: 'bottom'
                    },
                    scales: {
                        xAxes: [{
                            display: true
                        }],
                        yAxes: [{
                            type: 'linear',
                            display: true,
                            position: 'left',
                            id: 'y-axis-1'
                        }],
                    },
                    annotation: {
                        annotations: annotations
                    }
                }
            };

            var ctx1 = document.getElementById('grafico-temperatura').getContext('2d');
            _graficoTemperatura = Chart.Line(ctx1, config1);

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function carregarDadosHistoricoMonitoramentoVelocidade() {
    executarReST("Monitoramento/ObterHistoricoVelocidade", {
        Codigo: _historicoMonitoramento.Codigo.val(),
        DataInicial: _pesquisaHistoricosVelocidade.DataInicial.val(),
        DataFinal: _pesquisaHistoricosVelocidade.DataFinal.val()
    }, function (retorno) {
        if (retorno.Success) {
            if (_graficoVelocidade) _graficoVelocidade.destroy();
            var velocidades = new Array();
            var datas = new Array();
            for (var i = 0; i < retorno.Data.length; i++) {
                velocidades.push(retorno.Data[i].Velocidade);
                datas.push(retorno.Data[i].Data);
            }
            var lineVelocidades = {
                labels: datas,
                datasets: [{
                    label: Localization.Resources.Cargas.MonitoramentoHistorico.Velocidade,
                    borderColor: "green",
                    backgroundColor: "green",
                    fill: false,
                    data: velocidades,
                    yAxisID: 'y-axis-1',
                }]
            };

            var config1 = {
                data: lineVelocidades,
                options: {
                    maintainAspectRatio: false,
                    hoverMode: 'index',
                    stacked: false,
                    title: {
                        display: false,
                    },
                    elements: {
                        line: {
                            tension: 0
                        }
                    },
                    legend: {
                        display: false,
                        position: 'bottom'
                    },
                    scales: {
                        xAxes: [{
                            display: true
                        }],
                        yAxes: [{
                            type: 'linear',
                            display: true,
                            position: 'left',
                            id: 'y-axis-1'
                        }],
                    }
                }
            };

            var ctx1 = document.getElementById('grafico-velocidade').getContext('2d');
            _graficoVelocidade = Chart.Line(ctx1, config1);

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function criarMakerHistoricoMonitoramentoPosicao(info, ultimo) {
    if ((typeof info.Latitude) == "string")
        info.Latitude = Globalize.parseFloat(info.Latitude);

    if ((typeof info.Longitude) == "string")
        info.Longitude = Globalize.parseFloat(info.Longitude);

    var icone = TrackingIconRastreador(info.online);

    var marker = new ShapeMarker();
    marker.setPosition(info.Latitude, info.Longitude);
    marker.icon = _mapaHistoricos.draw.icons.point();
    marker.title = info.Placa;
    marker.content =
        '<div style="width: 140px; float: left;"><strong>' + info.Placa + '</strong></div>' + '<div class="tracking-indicador" style="float: right; width: 50px; margin-top:5px" title="' + info.Data + '">' + icone + '</div>' +
        '<div>' + info.Data + '</div>' +
        '<div>' + info.Descricao + '<div>' +
        '<div style="font-style:italic">' + info.Latitude + ',' + info.Longitude + '</div>';

    var shape = _mapaHistoricos.draw.addShape(marker, false, "click");

    shape.addListener("click", function () {
        navigator.clipboard.writeText
            ('Placa: ' + info.Placa + ' Data: ' + info.Data + ' Latitude: ' + info.Latitude + ' Longitude: ' + info.Longitude);
    });

    if (ultimo)
        shape.AbrirInfo();

}

function exibirModalHistoricoMonitoramento() {
    $(".legenda-rotas-container").hide();

    if ($("#divModalHistoricos").is(":hidden")) {
        Global.abrirModal('divModalHistoricos');
        $("#divModalHistoricos").one('hidden.bs.modal', function () {
            LimparCampos(_historicoMonitoramento);
        });
    }
}

function criarMakerPreTrip(info) {
    var marker = new ShapeMarker();
    var icon = {
        url: "../../../../Content/TorreControle/Icones/alertas/pre-trip-iniciado.svg",
        scaledSize: new google.maps.Size(45, 45),
    };
    marker.icon = icon
    marker.setPosition(info.Latitude, info.Longitude);
    marker.content =
        '<div>Início da pré-trip</div>' +
        '<div style="font-style:italic">' + info.Latitude + ',' + info.Longitude + '</div>';


    _mapaHistoricos.draw.addShape(marker, false, "click");

    marker.addListener("click", () => {
        infowindow.open({
            anchor: marker,
            _mapaHistoricos,
        });
    });


}

// #endregion Funções Privadas
