/// <reference path="../../Enumeradores/EnumSituacaoPosicaoFrota.js" />
/// <reference path="../../Enumeradores/EnumTipoModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Transportadores/Transportador/Transportador.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaDraw.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _gridPosicaoFrota;
var _pesquisaPosicaoFrota;
var _novoMapa = null;
var _markers = null;

/*
 * Declaração das Classes
 */

var PesquisaPosicaoFrota = function () {
    this.Veiculo = PropertyEntity({ text: "Veículos:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoModeloVeicular = PropertyEntity({ text: "Tipo de Veículo do Modelo Veicular:", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoModeloVeicularCarga.obterOpcoesPesquisaSemModelo(), cssClass: ko.observable("") });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("Transportador:"), issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CategoriaPessoa = PropertyEntity({ text: "Categoria:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false) });
    this.DataInicio = PropertyEntity({ text: "Data Início: ", col: 4, getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Fim: ", col: 4, getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(EnumSituacaoPosicaoFrota.obterTodos()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoPosicaoFrota.obterOpcoes(), cssClass: ko.observable("") });
    this.GrupoStatusViagem = PropertyEntity({ text: "Status da Viagem: ", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.GrupoTipoOperacao = PropertyEntity({ text: "Tipo de operação: ", val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Carga: ", col: 12 });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.EmAlvo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Apenas veículos em alvo" });
    this.VeiculosComMonitoramento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Apenas veículos com monitoramento em andamento" });
    this.VeiculosComContratoDeFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Apenas veículos que possuem contrato de frete", visible: ko.observable(true) });
    this.ClientesComVeiculoEmAlvo = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Clientes com veículos em alvo", visible: ko.observable(true) });
    this.ClientesAlvosEstrategicos = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Clientes alvos estratégicos", visible: ko.observable(true) });

    this.SituacaoVeiculo = PropertyEntity({ text: "Situação Veículo:", val: ko.observable(EnumSituacaoVeiculo.obterTodos()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoVeiculo.obterOpcoes(), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.StatusViagemControleEntrega = PropertyEntity({ text: "Viagem: ", val: ko.observable(EnumStatusViagemControleEntrega.Todas), options: EnumStatusViagemControleEntrega.obterOpcoesPesquisa(), def: EnumStatusViagemControleEntrega.Todas });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPosicaoFrota.CarregarGrid(function (result) {
                $("#legenda-totais-container").hide();
                if (_novoMapa != null && _markers != null) {
                    _markers.clearLayers();
                }
                if (result.data.length > 0) {
                    _pesquisaPosicaoFrota.ExibirFiltros.visibleFade(false);
                    actionObterDadosMapa();
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ExibirMapa = PropertyEntity({
        eventClick: function (e) {
            e.ExibirMapa.visibleFade(true);
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ExibirResultado = PropertyEntity({
        eventClick: function (e) {
            e.ExibirResultado.visibleFade(!e.ExibirResultado.visibleFade());
        }, type: types.event, text: "Resultado", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

}


/*
 * Declaração das Funções de Inicialização
 */

function loadPosicaoFrota() {
    loadPesquisaPosicaoFrota();
    buscaGrupoStatusViagem(loadPosicaoFrotaGoGo);
}

function loadPosicaoFrotaGoGo() {
    new BuscarVeiculos(_pesquisaPosicaoFrota.Veiculo);
    new BuscarFilial(_pesquisaPosicaoFrota.Filial);
    new BuscarCategoriaPessoa(_pesquisaPosicaoFrota.CategoriaPessoa);
    new BuscarClientes(_pesquisaPosicaoFrota.Cliente);
    new BuscarTransportadores(_pesquisaPosicaoFrota.Transportador, null, null, true);
    new BuscarTiposOperacao(_pesquisaPosicaoFrota.TipoOperacao);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaPosicaoFrota.Filial.visible(false);
        _pesquisaPosicaoFrota.Transportador.visible(false);
        _pesquisaPosicaoFrota.SituacaoVeiculo.visible(true);
        _pesquisaPosicaoFrota.TipoOperacao.visible(true);
        _pesquisaPosicaoFrota.VeiculosComContratoDeFrete.visible(false);
    }

    var cssClass = "col col-sm-12 col-2";
    if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento) {
        _pesquisaPosicaoFrota.GrupoTipoOperacao.visible(true);
        buscaGrupoTipoOperacao();
        if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
            _pesquisaPosicaoFrota.GrupoStatusViagem.visible(false);
        } else {
            _pesquisaPosicaoFrota.GrupoStatusViagem.visible(true);
            cssClass = "col col-sm-12 col-1"
        }
    } else {
        _pesquisaPosicaoFrota.GrupoTipoOperacao.visible(false);
    }
    //_pesquisaPosicaoFrota.Situacao.cssClass(cssClass);

    var configExportacao = {
        url: "PosicaoFrota/Exportar", 
        titulo: "Logística - Posição da Frota"
    };
    _gridPosicaoFrota = new GridView("grid-posicao-frota", "PosicaoFrota/Pesquisa", _pesquisaPosicaoFrota, undefined, undefined, 10, undefined, true, false, undefined, undefined, undefined, configExportacao, undefined, undefined, callbackRowPosicaoFrota);

    // Inicialização do mapa
    loadMap();

    // Busca as legendas
    actionLegendas();

    $("#" + _pesquisaPosicaoFrota.EmAlvo.id).click(verificarPesquisaEmAlvo);

    // Executa a pesquisa com os filtros iniciais
    _pesquisaPosicaoFrota.Pesquisar.eventClick();

}

function verificarPesquisaEmAlvo(e, sender) {
    if (_pesquisaPosicaoFrota.EmAlvo.val() == true) {
        _pesquisaPosicaoFrota.Cliente.enable(true);
        _pesquisaPosicaoFrota.CategoriaPessoa.enable(true);
    } else {
        _pesquisaPosicaoFrota.Cliente.enable(false);
        _pesquisaPosicaoFrota.Cliente.val("");
        _pesquisaPosicaoFrota.CategoriaPessoa.enable(false);
        _pesquisaPosicaoFrota.CategoriaPessoa.val("");
    }
}

function callbackRowPosicaoFrota(nRow, aData) {
    var span = $(nRow).find('td').eq(4).find('span')[0];
    if (span) {
        moment.locale('pt-br');
        var permanencia = moment(aData.TempoDaUltimaPosicaoFormatada, "DD/MM/YYYY HH:mm:ss").fromNow();
        $(span).text(permanencia);
    }
}

/*
 * Declaração das Funções do Mapa
 */

function loadPesquisaPosicaoFrota() {
    _pesquisaPosicaoFrota = new PesquisaPosicaoFrota();
    KoBindings(_pesquisaPosicaoFrota, "knockoutPesquisaPosicaoFrota", _pesquisaPosicaoFrota.Pesquisar.id, _pesquisaPosicaoFrota.Pesquisar.id);
}

function loadMap() {
    if (!_novoMapa) {

        let layers = [
            L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
                attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
                maxZoom: 18,
                id: 'mapbox/satellite-v9',
                tileSize: 512,
                zoomOffset: -1,
                accessToken: 'pk.eyJ1IjoibXVsdGlzb2Z0d2FyZW1hcHMiLCJhIjoiY20ydXZkdTVxMDVseTJzcTFleHRvbDgxcSJ9.t9Ojs736OH3XHMJwbjeLMw'
            }),
            L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
                attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
                maxZoom: 18,
                id: 'mapbox/streets-v11',
                tileSize: 512,
                zoomOffset: -1,
                accessToken: 'pk.eyJ1IjoibXVsdGlzb2Z0d2FyZW1hcHMiLCJhIjoiY20ydXZkdTVxMDVseTJzcTFleHRvbDgxcSJ9.t9Ojs736OH3XHMJwbjeLMw'
            }),
        ];

        _novoMapa = L.map('map', { zoomControl: false, layers: [layers[1]] }).setView([-14.235004, -51.925280], 4);

        var modos = {
            "Satélite": layers[0],
            "Ruas": layers[1],
        };

        L.control.layers(modos).addTo(_novoMapa);

    }
}

function actionObterDadosMapa() {
    executarReST("PosicaoFrota/ObterDadosMapa", RetornarObjetoPesquisa(_pesquisaPosicaoFrota), function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _markers = L.markerClusterGroup();
                pinMarkersVeiculo(arg.Data.Veiculos);
                pinAreasAlvos(arg.Data.Alvos);
                $("#legenda-totais-container").show();
                $("#legenda-totais-container ul li").remove();
                for (var i = 0; i < arg.Data.Grupos.length; i++) {
                    InserirLegenda("#legenda-totais-container ul", arg.Data.Grupos[i].Descricao + ": " + arg.Data.Grupos[i].Total, arg.Data.Grupos[i].Cor);
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, true);
}

function pinMarkersVeiculo(data) {
    for (var i = 0; i < data.length; i++) {
        var v = data[i];
        var iconeVeiculoOptions = null;
        let mapDraw = new MapaDraw();

        if (v.PossuiParada) {
            if (v.TipoModeloVeicular == EnumTipoModeloVeicularCarga.Reboque) {
                iconeVeiculoOptions = {
                    iconUrl: mapDraw.icons.truckTrailerStop(40, 40, v.Cor)
                }
            } else {
                iconeVeiculoOptions = {
                    iconUrl: mapDraw.icons.truckStop(40, 40, v.Cor)
                }
            }
        } else {
            if (v.TipoModeloVeicular == EnumTipoModeloVeicularCarga.Reboque) {
                iconeVeiculoOptions = {
                    iconUrl: mapDraw.icons.truckTrailerSignal(28, 28, v.Cor, v.Rastreador)
                }
            } else {
                iconeVeiculoOptions = {
                    iconUrl: mapDraw.icons.truckSignal(28, 28, v.Cor, v.Rastreador)
                }
            }
        }

        let iconeVeiculo = L.Icon.extend({
            options: iconeVeiculoOptions
        });

        var conteudo = '';
        conteudo += `<!--${v.CodigoVeiculo}--><div id="InfoWindowVeiculo"><div class="placa">${v.PlacaVeiculo}</div>Carregando...</div>`;

        let marker = L.marker([v.Latitude, v.Longitude], { icon: new iconeVeiculo() });
        marker.bindPopup(conteudo, { offset: [12, 0] });
        marker.on('click', (e) => clickMarkerVeiculo(e.target.getPopup()))
        _markers.addLayer(marker);
    }
}

function clickMarkerVeiculo(popup) {
    // ... já foram carregados, apenas apresenta
    if (popup.getContent().substring(0, 4) != '<!--') {
        return;
    }

    var codigoVeiculo = popup.getContent().substring(4, popup.getContent().indexOf('-->'));
    executarReST("PosicaoFrota/ObterDadosVeiculo", { CodigoVeiculo: codigoVeiculo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var html = `<div id="InfoWindowVeiculo">`;
                html += `<div class="placa">${arg.Data.PlacaVeiculo}</div>`;
                if (arg.Data.Transportador != undefined && arg.Data.Transportador != '') html += `<div class="transportadora"><label>Transportadora</label><span>${arg.Data.Transportador}</span></div>`;
                if (arg.Data.Status != undefined && arg.Data.Status != '') html += `<div class="statu"><label>Status</label><span>${arg.Data.Status}</span></div>`;
                if (arg.Data.Carga != undefined && arg.Data.Carga != '') html += `<div class="plano"><label>Nº plano</label><span>${arg.Data.Carga}</span></div>`;
                if (arg.Data.DataDaPosicao != undefined && arg.Data.DataDaPosicao != '') html += `<div class="data-ultima-posicao"><label>Data última posição</label><span>${arg.Data.DataDaPosicao}</span></div>`;
                if (arg.Data.Descricao != undefined && arg.Data.Descricao != '') html += `<div class="desc-ultima-posicao"><label>Desc última posição</label><span>${arg.Data.Descricao}</span></div>`;
                if (arg.Data.Latitude != undefined && arg.Data.Latitude != undefined) html += `<div class="lat-long"><label>Latitude,Longitude</label><span>${arg.Data.Latitude}, ${arg.Data.Longitude}</span></div>`;
                if (arg.Data.Embarcador != undefined && arg.Data.Embarcador != '') html += `<div class="embarcador"><label>Embarcador</label><span>${arg.Data.Embarcador}</span></div>`;
                if (arg.Data.Destinos != undefined && arg.Data.Destinos.length > 0) {
                    for (var i = 0; i < arg.Data.Destinos.length; i++) {
                        html += `<div class="destinos"><label>Destino ${arg.Data.Destinos[i].Ordem}</label><span>${arg.Data.Destinos[i].Descricao}</span></div>`;
                    }
                }
                if (arg.Data.Paradas && arg.Data.Paradas.length > 0) {
                    html += '<br/><div><strong>Parada(s):</strong>';
                    for (var i = 0; i < arg.Data.Paradas.length; i++) {
                        html += `<br/>- ${arg.Data.Paradas[i].Tipo} (${arg.Data.Paradas[i].Data})`;
                    }
                    html += '</div>'
                }
                html += '</div>'
                popup.setContent(html);
                popup.update();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, false);
   
}

function pinAreasAlvos(data) {
    for (var i = 0; i < data.length; i++) {
        var alvo = data[i];
        if (!validLatLng(alvo.Latitude, alvo.Longitude)) continue;

        _markers.addLayer(L.marker([alvo.Latitude, alvo.Longitude]).bindTooltip(alvo.Descricao, { direction: 'top', offset: [-15, -15] }));
        
        if (alvo.TipoArea == 'Raio') {
            let cor = alvo.Cor != null && alvo.Cor != '' ? alvo.Cor : '#FF0000';

            L.circle([alvo.Latitude, alvo.Longitude], {
                color: cor,
                fillColor: cor,
                fillOpacity: 0.5,
                radius: alvo.Raio,
                stroke: false
            }).addTo(_novoMapa);
        } else {
            if (alvo.Area && alvo.Area.length > 0) {
                var path;
                try {
                    var path = JSON.parse(alvo.Area);
                } catch (e) {
                    console.log('Path inválida para o raio ' + alvo.Nome);
                    return;
                }
                for (var j = 0; j < path.length; j++) {
                    switch (path[j].type) {
                        case google.maps.drawing.OverlayType.CIRCLE:
                            let cor = alvo.Cor != null && alvo.Cor != '' ? alvo.Cor : '#FF0000';

                            L.circle(path[j].center, {
                                color: cor,
                                fillColor: cor,
                                fillOpacity: 0.7,
                                radius: path[j].radius,
                                stroke: false
                            }).addTo(_novoMapa);

                            break;

                        case google.maps.drawing.OverlayType.RECTANGLE:

                            let corRectangle = (alvo.Cor != null && alvo.Cor != '') ? alvo.Cor : path[j].fillColor;
                            L.rectangle(path[j].bounds, { color: corRectangle, stroke: false, fillOpacity: 0.7, }).addTo(map);
                            break;

                        case google.maps.drawing.OverlayType.POLYGON:
                            
                            let corPolygon = (alvo.Cor != null && alvo.Cor != '') ? alvo.Cor : path[j].fillColor;
                            L.polygon(
                                path[j].paths,
                                { color: corPolygon, stroke: false, fillOpacity: 0.7, }
                            ).addTo(_novoMapa);
                            break;
                    }
                }
            }
        }
    }

    _novoMapa.addLayer(_markers);
    _novoMapa.fitBounds(_markers.getBounds());
}

function validLatLng(lat, lng) {
    return lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180;
}

/*
 * Declaração das Funções Associadas a Eventos
 */
function ExibirLegenda() {
    Global.abrirModal('divModalLegenda');
}

/*
 * Demais funções
 */

function actionLegendas() {
    executarReST("PosicaoFrota/Legendas", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data.Grupos != undefined) {
                    for (var i = 0; i < arg.Data.Grupos.length; i++) {
                        InserirLegenda(".legenda-veiculos ul", arg.Data.Grupos[i].Descricao, arg.Data.Grupos[i].Cor);
                        if (arg.Data.Grupos[i].StatusViagem != undefined) {
                            for (var j = 0; j < arg.Data.Grupos[i].StatusViagem.length; j++) {
                                InserirLegenda(".legenda-veiculos ul", arg.Data.Grupos[i].StatusViagem[j].Descricao, arg.Data.Grupos[i].StatusViagem[j].Cor, null, 'sub');
                            }
                        }
                    }
                }
                if (arg.Data.Categorias != undefined) {
                    for (var i = 0; i < arg.Data.Categorias.length; i++) {
                        InserirLegenda(".legenda-categorias ul", arg.Data.Categorias[i].Descricao, arg.Data.Categorias[i].Cor, arg.Data.Categorias[i].Cor);
                    }
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, true);
}

function InserirLegenda(selector, text, backgroundColor, borderColor, className) {
    if (backgroundColor) {
        if (!borderColor) borderColor == 'none';
        if (!className) className = '';
        $(selector).append('<li class="' + className + '"><span style="background-color:' + backgroundColor + ';border-color:' + borderColor + 'px;"></span>' + text + '</li>');
    } else {
        $(selector).append('<li>' + text + '</li>');
    }
}

function buscaGrupoStatusViagem(callback) {
    if (!_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento || !_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
        executarReST("MonitoramentoGrupoStatusViagem/BuscarTodos", null, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var selected = [];
                    for (var i = 0; i < arg.Data.GrupoStatusViagem.length; i++) {
                        if (arg.Data.GrupoStatusViagem[i].selected == 'selected') {
                            selected.push(arg.Data.GrupoStatusViagem[i].value);
                        }
                    }
                    _pesquisaPosicaoFrota.GrupoStatusViagem.options(arg.Data.GrupoStatusViagem);
                    _pesquisaPosicaoFrota.GrupoStatusViagem.val(selected);

                    $("#" + _pesquisaPosicaoFrota.GrupoStatusViagem.id).selectpicker('refresh');
                    
                    callback();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        callback();
    }
}

function buscaGrupoTipoOperacao(callback) {
    executarReST("GrupoTipoOperacao/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisaPosicaoFrota.GrupoTipoOperacao.options(arg.Data.GrupoTipoOperacao);

                $("#" + _pesquisaPosicaoFrota.GrupoTipoOperacao.id).selectpicker('refresh');
                
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
