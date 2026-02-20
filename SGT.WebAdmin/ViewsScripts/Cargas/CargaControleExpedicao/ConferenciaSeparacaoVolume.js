/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _conferenciaSeparacaoVolume;
var _gridConferenciaSeparacaoVolumeVolume;

/*
 * Declaração das Classes
 */

var ConferenciaSeparacaoVolume = function () {
    this.Expedicao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoBarras = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.CargaControleExpedicao.CodigoDeBarras, required: true, eventClick: ChangeCodigoBarrasVolume });

    this.Volumes = PropertyEntity({ idGrid: guid() });
    this.Mensagem = PropertyEntity({ type: types.local, visible: ko.observable(false), cssClass: ko.observable(""), eventClick: function () { _conferenciaSeparacaoVolume.Mensagem.visible(false) } });

    this.Finalizar = PropertyEntity({ eventClick: finalizarVolumeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Finalizar, visible: ko.observable(true) });
    this.AutorizarVolumesFaltantes = PropertyEntity({ eventClick: AutorizarVolumesFaltantesClick, type: types.event, text: Localization.Resources.Cargas.CargaControleExpedicao.AutorizarVolumesFaltantes, visible: ko.observable(false) });
    this.VolumesFaltantes = PropertyEntity({ eventClick: VolumesFaltantesClick, type: types.event, text: Localization.Resources.Cargas.CargaControleExpedicao.VolumesFaltantes, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadConferenciaSeparacaoVolume() {
    _conferenciaSeparacaoVolume = new ConferenciaSeparacaoVolume();
    KoBindings(_conferenciaSeparacaoVolume, "knockoutConferenciaSeparacaoVolume");

    loadGridVolumesSeparacao();

    _conferenciaSeparacaoVolume.CodigoBarras.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key === ENTER_KEY)
                ChangeCodigoBarrasVolume();
        });

    $modalConferenciaSeparacaoVolume.on('hidden.bs.modal', limparCamposConferenciaSeparacaoVolume);
}

function loadGridVolumesSeparacao() {
    _gridConferenciaSeparacaoVolumeVolume = new GridView(_conferenciaSeparacaoVolume.Volumes.idGrid, "ConferenciaSeparacaoVolume/Pesquisa", _conferenciaSeparacaoVolume);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function EditarConferenciaSeparacaoVolumeClick(itemGrid) {
    // Limpa os campos
    limparCamposConferenciaSeparacaoVolume();

    // Seta o codigo do objeto
    _conferenciaSeparacaoVolume.Expedicao.val(itemGrid.Codigo);
    _conferenciaSeparacaoVolume.Carga.val(itemGrid.CodigoCarga);

    // Busca informacoes para edicao
    _gridConferenciaSeparacaoVolumeVolume.CarregarGrid(function () {
        if (itemGrid.AptoConferir) {
            _conferenciaSeparacaoVolume.Finalizar.visible(true);
        } else {
            _conferenciaSeparacaoVolume.Finalizar.visible(false);
        }

        Global.abrirModal('divModalConferenciaSeparacaoVolume')
        //$modalConferenciaSeparacaoVolume.modal('show');
        _conferenciaSeparacaoVolume.CodigoBarras.val("");
        _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();
    });
}

function ChangeCodigoBarrasVolume() {
    setTimeout(function () {
        if (_conferenciaSeparacaoVolume.CodigoBarras.val() !== "") {
            executarReST("ConferenciaSeparacaoVolume/Adicionar", { Carga: _conferenciaSeparacaoVolume.Carga.val(), CodigoBarras: _conferenciaSeparacaoVolume.CodigoBarras.val().trim(), Expedicao: _conferenciaSeparacaoVolume.Expedicao.val() }, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        var data = arg.Data;

                        if (!data.CodigoBarrasValido) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.CargaControleExpedicao.CodigoBarraInformadoConstaVolumesExpedicao);

                            _conferenciaSeparacaoVolume.CodigoBarras.val("");
                            _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();

                            return;
                        }

                        if (data.JaConferido) {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.CargaControleExpedicao.CodigoBarraConferido);
                            _conferenciaSeparacaoVolume.CodigoBarras.val("");
                            _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();

                            return;
                        }

                        _gridConferenciaSeparacaoVolumeVolume.CarregarGrid();
                        _conferenciaSeparacaoVolume.CodigoBarras.val("");
                        _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CargaControleExpedicao.VolumeConferido);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                        _conferenciaSeparacaoVolume.CodigoBarras.val("");
                        _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    _conferenciaSeparacaoVolume.CodigoBarras.val("");
                    _conferenciaSeparacaoVolume.CodigoBarras.get$().focus();
                }
            });
        }
    }, 100);
}

function finalizarVolumeClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.CargaControleExpedicao.FinalizarSeparacao, Localization.Resources.Cargas.CargaControleExpedicao.VoceCertezaDesejaFinalizarSeparacao, function () {
        executarReST("ConferenciaSeparacaoVolume/Finalizar", { Expedicao: _conferenciaSeparacaoVolume.Expedicao.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalConferenciaSeparacaoVolume')
                    //$modalConferenciaSeparacaoVolume.modal('hide');
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
                    _gridConferenciaSeparacaoVolumeVolume.CarregarGrid();
                    _gridCargaControleExpedicao.CarregarGrid();
                    limparCamposConferenciaSeparacaoVolume();
                } else {
                    if (arg.Msg.indexOf("faltantes") >= 0 || arg.Msg.indexOf(Localization.Resources.Cargas.CargaControleExpedicao.ExistemVolumesNaoConferidos) >= 0)
                        _conferenciaSeparacaoVolume.AutorizarVolumesFaltantes.visible(true);
                    _conferenciaSeparacaoVolume.Mensagem.cssClass("warning");
                    _conferenciaSeparacaoVolume.Mensagem.val(arg.Msg);
                    _conferenciaSeparacaoVolume.Mensagem.visible(true);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function AutorizarVolumesFaltantesClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Autorizacao, Localization.Resources.Cargas.CargaControleExpedicao.VoceCertezaDesejaAutorizarConferenciaVolumesFaltantes, function () {
        executarReST("ConferenciaSeparacaoVolume/AutorizarVolumesFaltantes", { Expedicao: _conferenciaSeparacaoVolume.Expedicao.val() }, function (arg) {
            if (arg.Success) {
                _conferenciaSeparacaoVolume.AutorizarVolumesFaltantes.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        }, sender);
    });
}

function VolumesFaltantesClick(e, sender) {
    var data = { RecebimentoVolume: false, CodigoExpedicao: _conferenciaSeparacaoVolume.Expedicao.val() };
    executarReST("RecebimentoMercadoria/VolumesFaltantes", data, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CargaControleExpedicao.AguardeGeracaArquivoimpresaoVolumesFaltantes);
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

/*
 * Declaração das Funções
 */

function limparCamposConferenciaSeparacaoVolume() {
    var tmpCodigoExpedicao = _conferenciaSeparacaoVolume.Expedicao.val();
    var tmpCodigoCarga = _conferenciaSeparacaoVolume.Carga.val();

    LimparCampos(_conferenciaSeparacaoVolume);
    _conferenciaSeparacaoVolume.AutorizarVolumesFaltantes.visible(false);
    _conferenciaSeparacaoVolume.Mensagem.visible(false);
    _conferenciaSeparacaoVolume.Expedicao.val(tmpCodigoExpedicao);
    _conferenciaSeparacaoVolume.Carga.val(tmpCodigoCarga);
}