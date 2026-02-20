/*InicioViagem.js*/
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
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

var _inicioViagem;
var _mapaInicioViagem = null;
var _CodigoCargaComMonitoramentoAtivo;

var InicioViagem = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioDaViagem.getFieldDescription(), val: ko.observable(""), def: "" });
    this.InformarInicioViagem = PropertyEntity({ eventClick: informarInicioViagemClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.IniciarViagem, idGrid: guid(), visible: ko.observable(false) });
    this.DataInicioViagemInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataHoraDeInicioDaViagem.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.DataPrevisaoInicioViagem = PropertyEntity({ val: ko.observable(""), def: "" });
    this.LatitudeInicioViagem = PropertyEntity({ text: "LatitudeInicioViagem", val: ko.observable(""), def: "" });
    this.LongitudeInicioViagem = PropertyEntity({ text: "LatitudeInicioViagem", val: ko.observable(""), def: "" });
    this.DataPreViagemInicio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PreTripIniciadaEm.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataPreViagemFim = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PreTripFinalizadaEm.getFieldDescription(), val: ko.observable(""), def: "" });


    this.Auditar = PropertyEntity({ eventClick: auditarInicioViagemCargaClick, type: types.event, visible: _CONFIGURACAO_TMS.PermiteAuditar });
    this.InformarInicioViagemAlterar = PropertyEntity({ eventClick: informarInicioViagemAlterarClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AlterarDataInicio, idGrid: guid(), visible: ko.observable(false) });
}

function loadInicioViagem() {
    _inicioViagem = new InicioViagem();
    KoBindings(_inicioViagem, "knockoutInicioViagem");
}

function exibirDetalhesInicioViagemControleEntrega(fluxoEntrega, etapa) {
    var permiteAlterarDatasInicioFimViagem = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem, _PermissoesPersonalizadasControleEntrega);

    _etapaAtualFluxo = fluxoEntrega;
    
    LimparCampos(_inicioViagem);

    executarReST("ControleEntregaInicioViagem/BuscarPorCarga", { Carga: etapa.Carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _inicioViagem.InformarInicioViagem.visible(false);
                _inicioViagem.InformarInicioViagemAlterar.visible(false);
                
                PreencherObjetoKnout(_inicioViagem, arg);
                
                if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && arg.Data.PermiteTransportadorConfirmarRejeitarEntrega) || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente, _PermissoesPersonalizadasControleEntrega)) {

                    if (!arg.Data.ViagemAberta && ((!arg.Data.ViagemFinalizada && arg.Data.PermiteAlterarDataInicioCarregamento) || permiteAlterarDatasInicioFimViagem)) {
                        _inicioViagem.InformarInicioViagemAlterar.visible(true);
                        _inicioViagem.DataInicioViagemInformada.val(arg.Data.DataInicioViagem);

                    } else {
                        if (arg.Data.ViagemAberta)
                            _inicioViagem.InformarInicioViagem.visible(true);
                        if (arg.Data.DataInicioViagemSugerida)
                            _inicioViagem.DataInicioViagemInformada.val(arg.Data.DataInicioViagemSugerida);
                    }
                }

                exibeModalEtapa('#divModalInicioViagem');

                // Se já foi iniciada, mostrar mapa
                if (_inicioViagem.DataInicioViagem.val()) {
                    $('#mapaInicioViagem').show();
                    carregarMapaInicioViagem();
                } else {
                    $('#mapaInicioViagem').hide();
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//********MÉTODOS*******

function auditarInicioViagemCargaClick(e) {
    var data = { Codigo: e.Carga.val() };
    var closureAuditoria = OpcaoAuditoria("Carga", null, e);

    closureAuditoria(data);
}

function informarInicioViagemRest() {
    executarReST("ControleEntregaInicioViagem/InformarInicioViagem", {
        Carga: _inicioViagem.Carga.val(),
        DataInicioViagemInformada: _inicioViagem.DataInicioViagemInformada.val(),
        CodigoCargaComMonitoramentoAtivo: _CodigoCargaComMonitoramentoAtivo
    }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ViagemIniciadaComSucesso);

                Global.fecharModal("divModalInicioViagem");

                atualizarControleEntrega();

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function informarInicioViagemClick() {
    if (!ValidarCamposObrigatorios(_inicioViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.VoceTemCertezaQueDesejaIniciarViagem, function () {
        executarReST("ControleEntregaInicioViagem/ExisteMonitoramentoAtivoParaVeiculoPlaca", {
            Carga: _inicioViagem.Carga.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data && r.Msg && r.Msg !== '') {
                    _CodigoCargaComMonitoramentoAtivo = r.Data.CodigoCargaComMonitoramentoAtivo;
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, r.Msg, informarInicioViagemRest, null, "Confirmar", "Cancelar");
                } else {
                    informarInicioViagemRest();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function informarInicioViagemAlterarClick() {
    if (!ValidarCamposObrigatorios(_inicioViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.VoceTemCertezaQueDesejaAlterarDataDeInicioDeCarregamento, function () {
        executarReST("ControleEntregaInicioViagem/InformarInicioViagemAlterar", {
            Carga: _inicioViagem.Carga.val(),
            DataInicioViagemInformadaAlterar: _inicioViagem.DataInicioViagemInformada.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataDeInicioAlteradaComSucesso);

                    Global.fecharModal("divModalInicioViagem");

                    atualizarControleEntrega();

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function carregarMapaInicioViagem() {
    _mapaInicioViagem = null;
    if (_mapaInicioViagem == null) {
        opcoesMapa = new OpcoesMapa(false, false);
        _mapaInicioViagem = new MapaGoogle("mapaInicioViagem", false, opcoesMapa);
    }

    _mapaInicioViagem.clear();

    criarMarkerInicioViagem();

    _mapaInicioViagem.direction.setZoom(13);
    setTimeout(function () { _mapaInicioViagem.draw.centerShapes(); }, 500);
}

function criarMarkerInicioViagem() {
    let markerCliente = new ShapeMarker();
    markerCliente.setPosition(_inicioViagem.LatitudeInicioViagem.val(), _inicioViagem.LongitudeInicioViagem.val());
    _mapaInicioViagem.draw.addShape(markerCliente);
}
