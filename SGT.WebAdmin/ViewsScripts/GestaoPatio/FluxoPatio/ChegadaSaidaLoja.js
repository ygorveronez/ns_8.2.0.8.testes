/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _chegadaSaidaLoja;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ChegadaSaidaLoja = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Etapa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), type: types.local });

    this.CargaChegadaLoja = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Loja = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataChegada = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataChegada.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataSaida = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataSaida.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DevolucaoChegadaLoja = PropertyEntity({ val: ko.observable(0), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Nao, text: Localization.Resources.GestaoPatio.FluxoPatio.Devolucao.getFieldDescription(), eventChange: devolucaoChegadaLojaChange, enable: ko.observable(false) });
    this.NotaFiscalChegadaLoja = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NotaFiscal.getFieldDescription(), getType: typesKnockout.int, maxlength: 50, enable: ko.observable(false) });

    this.SalvarDadosEtapa = PropertyEntity({ eventClick: salvarDadosEtapaChegadaSaidaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.SalvarDados, idGrid: guid(), visible: ko.observable(false) });
    this.InformarChegadaLoja = PropertyEntity({ eventClick: informarChegadaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarChegada, idGrid: guid(), visible: ko.observable(false) });
    this.InformarSaidaLoja = PropertyEntity({ eventClick: informarSaidaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarSaida, idGrid: guid(), visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaChegadaSaidaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.VoltarEtapaChegadaLoja = PropertyEntity({ eventClick: voltarEtapaChegadaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, idGrid: guid(), visible: ko.observable(false) });
    this.VoltarEtapaSaidaLoja = PropertyEntity({ eventClick: voltarEtapaSaidaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, idGrid: guid(), visible: ko.observable(false) });
    this.RejeitarEtapaChegadaLoja = PropertyEntity({ eventClick: rejeitarEtapaChegadaLojaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, idGrid: guid(), visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadChegadaSaidaLojaLoja() {
    _chegadaSaidaLoja = new ChegadaSaidaLoja();
    KoBindings(_chegadaSaidaLoja, "knockoutChegadaSaidaLoja");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function devolucaoChegadaLojaChange(e) {
    _chegadaSaidaLoja.NotaFiscalChegadaLoja.enable(false);

    if (_chegadaSaidaLoja.DevolucaoChegadaLoja.val() == "0")
        _chegadaSaidaLoja.NotaFiscalChegadaLoja.val("");
    else if (_chegadaSaidaLoja.DevolucaoChegadaLoja.enable())
        _chegadaSaidaLoja.NotaFiscalChegadaLoja.enable(true);
}

function informarChegadaLojaClick(e) {
    Salvar(e, "ChegadaSaidaLoja/SalvarInformacaoEtapa", function (r) {
        if (r.Success) {
            if (r.Data) {
                executarReST("ChegadaSaidaLoja/InformarChegadaLoja", { Codigo: _chegadaSaidaLoja.Codigo.val() }, function (r) {
                    if (r.Success) {
                        if (r.Data !== false) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.ChegadaInformadaComSucesso);
                            atualizarFluxoPatio();
                            Global.fecharModal('divModalChegadaSaidaLoja');
                        } else {
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function informarSaidaLojaClick() {
    executarReST("ChegadaSaidaLoja/InformarSaidaLoja", { Codigo: _chegadaSaidaLoja.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SaidaInformadaComSucesso);
                atualizarFluxoPatio();
                Global.fecharModal('divModalChegadaSaidaLoja');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function observacoesEtapaChegadaSaidaLojaClick() {
    buscarObservacoesEtapa(_chegadaSaidaLoja.Etapa.val());
}

function rejeitarEtapaChegadaLojaClick() {

}

function salvarDadosEtapaChegadaSaidaLojaClick(e) {
    Salvar(e, "ChegadaSaidaLoja/SalvarInformacaoEtapa", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DadosSalvosComSucesso);
                Global.fecharModal('divModalChegadaSaidaLoja');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function voltarEtapaChegadaLojaClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRetornarEtapaAnterior, function () {
        executarReST("ChegadaSaidaLoja/VoltarEtapaChegadaLoja", { Codigo: _chegadaSaidaLoja.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornada);

                    atualizarFluxoPatio();
                    Global.fecharModal('divModalChegadaSaidaLoja');
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    });
}

function voltarEtapaSaidaLojaClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRetornarEtapaAnterior, function () {
        executarReST("ChegadaSaidaLoja/VoltarEtapaSaidaLoja", { Codigo: _chegadaSaidaLoja.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornada);

                    atualizarFluxoPatio();
                    Global.fecharModal('divModalChegadaSaidaLoja');
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function ExibirChegadaVeiculoLoja(knoutFluxo, opt) {
    SetaTituloChegadaSaida(opt.text);

    ExibirChegadaSaidaVeiculoLoja(knoutFluxo, EnumEtapaFluxoGestaoPatio.ChegadaLoja, function () {
        _chegadaSaidaLoja.InformarSaidaLoja.visible(false);
        _chegadaSaidaLoja.VoltarEtapaSaidaLoja.visible(false);
    });
}

function ExibirSaidaVeiculoLoja(knoutFluxo, opt) {
    SetaTituloChegadaSaida(opt.text);

    ExibirChegadaSaidaVeiculoLoja(knoutFluxo, EnumEtapaFluxoGestaoPatio.SaidaLoja, function () {
        _chegadaSaidaLoja.InformarChegadaLoja.visible(false);
        _chegadaSaidaLoja.VoltarEtapaChegadaLoja.visible(false);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function ExibirChegadaSaidaVeiculoLoja(knoutFluxo, etapa, callback) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_chegadaSaidaLoja);

    executarReST("ChegadaSaidaLoja/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val(), Etapa: etapa }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _chegadaSaidaLoja.VoltarEtapaChegadaLoja.visible(false);
                _chegadaSaidaLoja.VoltarEtapaSaidaLoja.visible(false);
                _chegadaSaidaLoja.InformarChegadaLoja.visible(false);
                _chegadaSaidaLoja.InformarSaidaLoja.visible(false);
                _chegadaSaidaLoja.DevolucaoChegadaLoja.enable(false);

                PreencherObjetoKnout(_chegadaSaidaLoja, arg);

                _chegadaSaidaLoja.Etapa.val(etapa);

                var permiteEditarEtapa = arg.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarChegadaLoja, _PermissoesPersonalizadasFluxoPatio);

                if (arg.Data.VeiculoAindaNaoChegou && permiteEditarEtapa) {
                    _chegadaSaidaLoja.InformarChegadaLoja.visible(true);
                    _chegadaSaidaLoja.DevolucaoChegadaLoja.enable(true);
                }

                if (arg.Data.VeiculoAindaNaoSaiu || arg.Data.PermitirEditarEtapa)
                    _chegadaSaidaLoja.InformarSaidaLoja.visible(true);

                if (arg.Data.PermitirEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _chegadaSaidaLoja.DevolucaoChegadaLoja.enable(true);

                    if (!primeiraEtapa) {
                        if ((etapa == EnumEtapaFluxoGestaoPatio.ChegadaLoja) && (_configuracaoGestaoPatio.ChegadaLojaPermiteVoltar && permiteEditarEtapa))
                            _chegadaSaidaLoja.VoltarEtapaChegadaLoja.visible(true);
                        else if ((etapa == EnumEtapaFluxoGestaoPatio.SaidaLoja) && _configuracaoGestaoPatio.SaidaLojaPermiteVoltar)
                            _chegadaSaidaLoja.VoltarEtapaSaidaLoja.visible(true);
                    }
                }

                callback();

                if (edicaoEtapaFluxoPatioBloqueada()) {
                    _chegadaSaidaLoja.DevolucaoChegadaLoja.enable(false);
                    ocultarBotoesEtapa(_chegadaSaidaLoja);
                }

                devolucaoChegadaLojaChange();
                                
                Global.abrirModal('divModalChegadaSaidaLoja');
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function SetaTituloChegadaSaida(titulo) {
    $("#chegada-saida-titulo").text(titulo);
}

// #endregion Funções Privadas
