/// <reference path="../../../ViewsScripts/Enumeradores/EnumAlertaMonitorStatus.js" />

// #region Objetos Globais do Arquivo
var _tratativaAlerta;
var _exibicaoMapa;
var _informacoesAlerta;
var _acoesTratativa;
var _mapaTratativaAlerta;
var _grupoAlertas;
var _listaAlertasPorTipo = [];
// #endregion Objetos Globais do Arquivo

//#region Classes

var TratativaAlerta = function () {
    this.Tratativa = PropertyEntity({ text: "Tratativa", val: ko.observable(true), options: ko.observable([]), def: "", visible: ko.observable(true), required: true });
    this.Causas = PropertyEntity({ text: "Causas", val: ko.observable(""), options: ko.observable([]), def: "", visible: ko.observable(true), required: true });
    this.Observacao = PropertyEntity({ getType: typesKnockout.text, text: "Observação", val: ko.observable(""), visible: ko.observable(true) });
    this.ReprogramarAlerta = PropertyEntity({ getType: typesKnockout.bool, text: "Reprogramar Alerta", val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.TratarTodosDoMesmoTipo = PropertyEntity({ getType: typesKnockout.bool, text: "Tratar todos alertas do mesmo tipo", val: ko.observable(false), def: false, visible: ko.observable(true), required: false });
    this.TempoReprogramacaoAlerta = PropertyEntity({ getType: typesKnockout.time, text: "Tempo", val: ko.observable("hh:mm"), visible: ko.observable(false) });

    this.ReprogramarAlerta.val.subscribe(function (checked) {
        _tratativaAlerta.TempoReprogramacaoAlerta.visible(checked);
        _tratativaAlerta.TempoReprogramacaoAlerta.required = checked;
    });

}

var ExibicaoMapa = function () {
    this.ExibirMapaAlerta = PropertyEntity({ visible: ko.observable(true) });
}

var InformacoesAlerta = function () {
    this.TipoAlerta = PropertyEntity({ val: ko.observable(""), text: "Tipo de alerta" });
    this.NumeroAlerta = PropertyEntity({ val: ko.observable(""), text: "Número do alerta" });
    this.DescricaoStatus = PropertyEntity({ val: ko.observable(""), text: "Status" });
    this.DataInicio = PropertyEntity({ val: ko.observable(""), text: "Data de início" });
    this.DataFim = PropertyEntity({ val: ko.observable(""), text: "Data fim" });
    this.Responsavel = PropertyEntity({ val: ko.observable(""), text: "Responsável" });
    this.Tratativa = PropertyEntity({ val: ko.observable(""), text: "Tratativa" });
    this.Causa = PropertyEntity({ val: ko.observable(""), text: "Causa" });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: "Observação" });
    this.ResolvidoPor = PropertyEntity({ val: ko.observable(""), text: "Resolvido Por" });
    this.Gatilho = PropertyEntity({ val: ko.observable(""), text: "Gatilho" });
    this.ExibirNaoLocalizado = PropertyEntity({ visible: ko.observable(false) });
    this.AssumirAlerta = PropertyEntity({ type: types.event, eventClick: assumirAlertaClick, text: "Assumir Alerta", visible: ko.observable(true) });
    this.SalvarAlerta = PropertyEntity({ type: types.event, eventClick: salvarAlertaClick, text: "Salvar Alerta", visible: ko.observable(true) });
    this.DeixarAlerta = PropertyEntity({ type: types.event, eventClick: deixarAlertaClick, text: "Deixar Alerta", visible: ko.observable(true) });
}

var AcoesTratativaAlerta = function () {
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: cancelarTratativaClick, text: "Cancelar", visible: ko.observable(true) });
    this.Resolver = PropertyEntity({ type: types.event, eventClick: resolverTratativaClick, text: "Resolver", visible: ko.observable(true), enable: ko.observable(false) });
}

var GrupoAlertas = function () {
    this.Alertas = PropertyEntity({ val: ko.observable(new Array()) });
    this.QuantidadeAlertas = PropertyEntity({ val: ko.observable(""), text: "Quantidade de Alertas" });
}
// #endregion Classes

// #region Funções de Inicialização
function loadTratativaAlerta(alerta, alertas) {
    $.get("Content/TorreControle/TratativaAlerta/ModaisTratativaAlerta.html?dyn=" + guid(), function (htmlModaisTratativaAlerta) {
        $("#ModaisTratativaAlerta").html(htmlModaisTratativaAlerta);

        _tratativaAlerta = new TratativaAlerta();

        KoBindings(_tratativaAlerta, "knockoutTratativaAlerta");

        _exibicaoMapa = new ExibicaoMapa();
        KoBindings(_exibicaoMapa, "knockoutExibicaoMapa");

        _informacoesAlerta = new InformacoesAlerta();
        KoBindings(_informacoesAlerta, "knockoutInformacoesAlerta");

        _acoesTratativa = new AcoesTratativaAlerta();
        KoBindings(_acoesTratativa, "knockoutAcoesTratativaAlerta");

        _grupoAlertas = new GrupoAlertas();
        KoBindings(_grupoAlertas, "knockoutGrupoAlertas");

        _listaAlertasPorTipo = alertas;

        buscarInformacoesAlerta(alerta.CodigoAlerta);
    });
}

function carregarMapaTratativaAlerta() {

    opcoesMapa = new OpcoesMapa(false, false);

    _mapaTratativaAlerta = new MapaGoogle("mapaAlerta", false, opcoesMapa);


    _mapaTratativaAlerta.clear();
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function assumirAlertaClick(alerta) {
    AssumirAlerta(alerta.NumeroAlerta.val(), atualizarAlertaTratado);
}

function cancelarTratativaClick() {
    LimparCampos(_tratativaAlerta);
    LimparCampos(_informacoesAlerta);
    _listaAlertasPorTipo = [];
    Global.fecharModal("divModalInformacoesAlerta");
}

function resolverTratativaClick() {
    ResolverAlerta(_informacoesAlerta.NumeroAlerta.val(), atualizarAlertaTratado);
}

function deixarAlertaClick(alerta) {
    DeixarAlerta(alerta.NumeroAlerta.val(), atualizarAlertaTratado);
}

function salvarAlertaClick(alerta) {
    SalvarAlerta(alerta.NumeroAlerta.val());
}

// #endregion Funções Associadas a Eventos


// #region Funções Privadas
function buscarInformacoesAlerta(codigoAlerta) {
    if (_tratativaAlerta)
        LimparCampos(_tratativaAlerta);
    if (_exibicaoMapa)
        LimparCampos(_exibicaoMapa);
    if (_informacoesAlerta)
        LimparCampos(_informacoesAlerta);

    let buscarTodosDaCarga = !(_listaAlertasPorTipo && _listaAlertasPorTipo.length > 0);
    executarReST("AlertaMonitoramento/BuscarPorCodigo", { Codigo: codigoAlerta, BuscarTodosDaCarga: buscarTodosDaCarga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _exibicaoMapa.ExibirMapaAlerta.visible(true);
                _informacoesAlerta.ExibirNaoLocalizado.visible(false);
                PreencherObjetoKnout(_informacoesAlerta, { Data: arg.Data });
                carregarMapaTratativaAlerta();
                if (arg.Data.Latitude == null || arg.Data.Latitude == 0 || arg.Data.Longitude == null || arg.Data.Longitude == 0) {
                    _exibicaoMapa.ExibirMapaAlerta.visible(false);
                    _informacoesAlerta.ExibirNaoLocalizado.visible(true);
                } else {
                    criarMakerPosicaoAlerta({ lat: arg.Data.Latitude, lng: arg.Data.Longitude });
                }

                _tratativaAlerta.Causas.options(arg.Data.Causas);
                _tratativaAlerta.Tratativa.options(arg.Data.Tratativas);
                if (buscarTodosDaCarga)
                    _listaAlertasPorTipo = arg.Data.ListaAlertasCarga;

                if (arg.Data.Status == EnumAlertaMonitorStatus.EmTratativa)
                    preencherInformacoesTratativa(arg.Data);

                criarhtmlAlertasParaTratativa(codigoAlerta, arg.Data.Tipo, _listaAlertasPorTipo);
                controlarVisibilidadeTratativaAlerta(arg.Data);
                mudarImagemStatusAlerta(arg.Data.Codigo, arg.Data.ImagemStatus);
                Global.abrirModal("divModalInformacoesAlerta");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function criarMakerPosicaoAlerta(coordenada) {

    if ((coordenada.lat == "") && (coordenada.lng == ""))
        return;

    if ((typeof coordenada.lat) == "string")
        coordenada.lat = Globalize.parseFloat(coordenada.lat);

    if ((typeof coordenada.lng) == "string")
        coordenada.lng = Globalize.parseFloat(coordenada.lng);

    var marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.icon = 'Content/TorreControle/Icones/alertas/pin-mapa-alerta.svg';
    _mapaTratativaAlerta.draw.addShape(marker);
    _mapaTratativaAlerta.direction.centralizar(coordenada.lat, coordenada.lng);
}

function obterTratativaAlerta(salvarTratativa) {
    var tratativaAlerta = {
        UtilizaTratativa: true,
        CodigoAlerta: _informacoesAlerta.NumeroAlerta.val(),
        Observacao: _tratativaAlerta.Observacao.val(),
        Tratativa: _tratativaAlerta.Tratativa.val(),
        Causa: _tratativaAlerta.Causas.val(),
        ReprogramarAlerta: _tratativaAlerta.ReprogramarAlerta.val(),
        TratarTodosDoMesmoTipo: _tratativaAlerta.TratarTodosDoMesmoTipo.val(),
        TempoReprogramacaoAlerta: _tratativaAlerta.TempoReprogramacaoAlerta.val(),
        SalvarTratativa: salvarTratativa
    }

    return tratativaAlerta;
}

function controlarVisibilidadeTratativaAlerta(data) {
    $('.informacoes-tratativa-alerta').hide();
    if (data.Status == EnumAlertaMonitorStatus.Finalizado) {
        $('#knockoutTratativaAlerta').hide();
        $('.actions-menu-tratativa').hide();
        $('.informacoes-tratativa-alerta').show();
        _acoesTratativa.Resolver.visible(false);
        _acoesTratativa.Cancelar.visible(false);
        _informacoesAlerta.AssumirAlerta.visible(false);
    }
    else if (data.UsuarioResponsavelLogado && data.Status == EnumAlertaMonitorStatus.EmTratativa) {
        $('#knockoutTratativaAlerta').show();
        $('.actions-menu-tratativa').show();
        _informacoesAlerta.AssumirAlerta.visible(false);
        _acoesTratativa.Resolver.visible(true);
        _acoesTratativa.Cancelar.visible(true);
    }
    else if (!data.PossuiResponsavel && data.Status != EnumAlertaMonitorStatus.Finalizado) {
        $('#knockoutTratativaAlerta').hide();
        $('.actions-menu-tratativa').hide();
        $('.informacoes-tratativa-alerta').hide();
        _informacoesAlerta.AssumirAlerta.visible(true);
        _acoesTratativa.Resolver.visible(false);
        _acoesTratativa.Cancelar.visible(false);
    }
    else if (!data.UsuarioResponsavelLogado && data.Status == EnumAlertaMonitorStatus.EmTratativa) {
        $('#knockoutTratativaAlerta').hide();
        $('.actions-menu-tratativa').hide();
        _informacoesAlerta.AssumirAlerta.visible(false);
        _acoesTratativa.Cancelar.visible(true);
        _acoesTratativa.Resolver.visible(false);
    }

    $("#tratarOutrosAlertas").hide();
    let alertasEmAberto = _listaAlertasPorTipo && _listaAlertasPorTipo.length > 1 ? _listaAlertasPorTipo.reduce(function (n, a) { return n + (a.Status != 1); }, 0) : [];
    if (alertasEmAberto > 1)
        $("#tratarOutrosAlertas").show();
        
}

function controlarVisibilidadeModalAoAssumirAlerta(alerta) {
    _informacoesAlerta.AssumirAlerta.visible(false);
    _acoesTratativa.Cancelar.visible(true);
    _acoesTratativa.Resolver.visible(true);
    $('.actions-menu-tratativa').show();
    $("#knockoutTratativaAlerta").show();
    mudarImagemStatusAlerta(_informacoesAlerta.NumeroAlerta.val(), alerta.ImagemStatus);
}

function controlarVisibilidadeModalAoDeixarAlerta(alerta) {
    _informacoesAlerta.AssumirAlerta.visible(true);
    _acoesTratativa.Cancelar.visible(false);
    _acoesTratativa.Resolver.visible(false);
    $('.actions-menu-tratativa').hide();
    $("#knockoutTratativaAlerta").hide();
    mudarImagemStatusAlerta(_informacoesAlerta.NumeroAlerta.val(), alerta.ImagemStatus);
}

function controlarVisibilidadeModalAoResolverAlerta(alerta) {
    $('#knockoutTratativaAlerta').hide();
    $('.actions-menu-tratativa').hide();
    $('.informacoes-tratativa-alerta').show();
    _acoesTratativa.Resolver.visible(false);
    _acoesTratativa.Cancelar.visible(false);
    _informacoesAlerta.AssumirAlerta.visible(false);
    mudarImagemStatusAlerta(_informacoesAlerta.NumeroAlerta.val(), alerta.ImagemStatus);
}

function mudarImagemStatusAlerta(codigoAlerta, imagemStatusAlerta) {
    let elemento = $('#imgAlertaCarousel_' + codigoAlerta);
    if (elemento && elemento[0])
        elemento[0].src = imagemStatusAlerta;
}

function preencherInformacoesTratativa(data) {
    if (data.CodigoCausa > 0)
        _tratativaAlerta.Causas.val(data.CodigoCausa);

    if (data.CodigoTratativa)
        _tratativaAlerta.Tratativa.val(data.CodigoTratativa);

    if (data.Observacao)
        _tratativaAlerta.Observacao.val(data.Observacao);

    _tratativaAlerta.ReprogramarAlerta.val(data.ReprogramarAlerta);

    if (data.TempoReprogramacaoAlerta)
        _tratativaAlerta.TempoReprogramacaoAlerta.val(data.TempoReprogramacaoAlerta);
}

function criarhtmlAlertasParaTratativa(alerta, tipoAlerta, alertas) {
    let listaAlertasAgrupados = [];
    let listaAlertasPorTipo = [];

    listaAlertasAgrupados = Object.groupBy(alertas, (x) => x.TipoAlerta);

    listaAlertasPorTipo = listaAlertasAgrupados[tipoAlerta];

    if (listaAlertasPorTipo == undefined || listaAlertasPorTipo.length <= 1) {
        $('#knockoutGrupoAlertas').hide();
        return;
    }

    _grupoAlertas.Alertas.val(listaAlertasPorTipo);
    _grupoAlertas.QuantidadeAlertas.val(listaAlertasPorTipo.length);

    let html = '';
    let carouselItem = '';
    let itemsPerCarouselItem = 10;
    let $alertas = $('#carouselGrupoAlertas');

    // Salva o índice da página ativa.
    let activeIndex = $alertas.find('.carousel-item.active').index();
    activeIndex = activeIndex < 0 ? 0 : activeIndex;

    $alertas.html("");

    for (let i = 0; i < listaAlertasPorTipo.length; i++) {
        if (i % itemsPerCarouselItem === 0) {
            if (i !== 0) {
                // Cria nova página do carousel.
                html += '<div class="carousel-item">' + carouselItem + '</div>';
                carouselItem = '';
            }
        }

        carouselItem += '<div class="alerta ' + ((alerta == listaAlertasPorTipo[i].CodigoAlerta) ? "selected" : "not-selected") + '" id="' + listaAlertasPorTipo[i].CodigoAlerta + '">' +
            '   <div class="d-flex justify-content-center">' +
            '       <img id="imgAlertaCarousel_' + listaAlertasPorTipo[i].CodigoAlerta + '" style="width: 14px; height: 14px;margin-bottom:2px;" src="' + listaAlertasPorTipo[i].ImagemStatus + '" />' +
            '   </div>' +
            '   <div class="d-flex justify-content-center">' +
            '       <a data-bind="attr:{title: ' + listaAlertasPorTipo.length + '}" data-toggle="tab" style="cursor: pointer;">' +
            '           <img style="cursor: pointer;" height="34" width="34" src="' + listaAlertasPorTipo[i].Imagem + '" onclick="buscarInformacoesAlerta(' + listaAlertasPorTipo[i].CodigoAlerta + ')" />' +
            '       </a>' +
            '   </div>' +
            '</div>';

        if (i === listaAlertasPorTipo.length - 1) {
            html += '<div class="carousel-item">' + carouselItem + '</div>';
        }
    }

    $alertas.html(html);
    $('#knockoutGrupoAlertas').show();

    // Define a página ativa.
    $alertas.find('.carousel-item').eq(activeIndex).addClass('active');
}

function AssumirAlerta(codigoAlerta = 0, callback) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja assumir o Alerta #" + codigoAlerta + "? Ele será atualizado para sua responsabilidade.", function () {
        executarReST("AlertaMonitoramento/AssumirResponsabilidadeAlerta", { Codigo: codigoAlerta }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "Você agora é responsável pelo alerta #" + codigoAlerta + "");

                    if (_informacoesAlerta) {
                        preencherInformacoesAlerta(arg.Data);
                        controlarVisibilidadeModalAoAssumirAlerta(arg.Data);
                    }

                    if (callback)
                        callback(arg.Data);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ResolverAlerta(codigoAlerta = 0, callback) {
    if (!ValidarCamposObrigatorios(_tratativaAlerta)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja <b>resolver</b> o Alerta #" + codigoAlerta + "? Ele será atualizado com as informações inseridas.", function () {
        tratativaAlerta = obterTratativaAlerta(false);

        executarReST("AlertaTratativa/Adicionar", tratativaAlerta, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, "Alerta resolvido com sucesso!");

                    if (_informacoesAlerta) {
                        preencherInformacoesAlerta(arg.Data);
                        controlarVisibilidadeModalAoResolverAlerta(arg.Data);
                    }

                    if (_tratativaAlerta.TratarTodosDoMesmoTipo.val()) {
                        _listaAlertasPorTipo = [];
                        buscarInformacoesAlerta(codigoAlerta);
                    }
                    
                    if (callback)
                        callback(arg.Data);

                    if (_listaAlertasPorTipo && _listaAlertasPorTipo.length <= 1)
                        Global.fecharModal("divModalInformacoesAlerta");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function DeixarAlerta(codigoAlerta = 0, callback) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja <b>deixar</b> o Alerta #" + codigoAlerta + "? Ele não ficará mais vinculado ao seu usuário.", function () {
        executarReST("AlertaMonitoramento/DeixarAlerta", { Codigo: codigoAlerta }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "O alerta está disponível para um novo responsável #" + codigoAlerta + "");

                    if (_informacoesAlerta) {
                        preencherInformacoesAlerta(arg.Data);
                        controlarVisibilidadeModalAoDeixarAlerta(arg.Data);
                    }

                    if (callback)
                        callback(arg.Data);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function SalvarAlerta(codigoAlerta = 0, callback) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja <b>salvar</b> o Alerta #" + codigoAlerta + "? As informações preenchidas ficarão armazenadas.", function () {
        var tratativaAlerta = obterTratativaAlerta(true);

        executarReST("AlertaTratativa/Adicionar", tratativaAlerta, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "Tratativa de Alerta salva com sucesso.");

                    if (callback)
                        callback();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function atualizarAlertaTratado(alertaTratado) {
    if (alertaTratado) {
        //Atualiza o status do item selecionado no carousel.
        if (alertaTratado.Status) {
            let alertaEtapa = $('#alertaEtapa_' + alertaTratado.CodigoAlerta);
            if (alertaEtapa && alertaEtapa[0])
                alertaEtapa[0].src = alertaTratado.ImagemStatus;
        }

        //Atualiza informações do Alerta na lista de alertas do carousel.
        if (_listaAlertasPorTipo && _listaAlertasPorTipo.length > 0) {
            let indexAlerta = _listaAlertasPorTipo.findIndex(x => x.CodigoAlerta == alertaTratado.CodigoAlerta);
            if (indexAlerta > -1) {
                _listaAlertasPorTipo[indexAlerta].ImagemStatus = alertaTratado.ImagemStatus;
                _listaAlertasPorTipo[indexAlerta].Responsavel = alertaTratado.Responsavel;
                _listaAlertasPorTipo[indexAlerta].Status = alertaTratado.EnumStatus;
            }
        }
    }
}

function preencherInformacoesAlerta(alerta) {
    if (alerta.Responsavel)
        _informacoesAlerta.Responsavel.val(alerta.Responsavel);
    if (alerta.DescricaoStatus)
        _informacoesAlerta.DescricaoStatus.val(alerta.DescricaoStatus);
    if (alerta.Tratativa)
        _informacoesAlerta.Tratativa.val(alerta.Tratativa);
    if (alerta.Causa)
        _informacoesAlerta.Causa.val(alerta.Causa);
    if (alerta.Observacao)
        _informacoesAlerta.Observacao.val(alerta.Observacao);
    if (alerta.ResolvidoPor)
        _informacoesAlerta.ResolvidoPor.val(alerta.ResolvidoPor);
    if (alerta.DataFim)
        _informacoesAlerta.DataFim.val(alerta.DataFim);
}
// #endregion Funções Privadas
