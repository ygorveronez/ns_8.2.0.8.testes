/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="./DetalhesDocumento.js" />
/// <reference path="GestaoDocumento.js" />

// #region Objetos Globais do Arquivo

var _detalheGestaoDocumento;
var _detalheCTe;
var _detalhePreCTe;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalheGestaoDocumento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.MotivoInconsistenciaGestaoDocumento = PropertyEntity({ val: ko.observable(EnumMotivoInconsistenciaGestaoDocumento.SemCarga), options: EnumMotivoInconsistenciaGestaoDocumento.obterOpcoes(), def: EnumMotivoInconsistenciaGestaoDocumento.SemCarga });
    this.SituacaoGestaoDocumento = PropertyEntity({ val: ko.observable(EnumSituacaoGestaoDocumento.Inconsistente), options: EnumSituacaoGestaoDocumento.obterOpcoes(), def: EnumSituacaoGestaoDocumento.Inconsistente });
    this.ObservacaoAprovacao = PropertyEntity({ text: "Motivo Aprovação: ", maxlength: 500, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    
    this.EmTratativa = PropertyEntity({ eventClick: emTratativaClick, type: types.event, text: "Em Tratativa", idGrid: guid(), visible: ko.observable(true) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarClick, type: types.event, text: "Aprovar", idGrid: guid(), visible: ko.observable(true) });
    this.Reprovar = PropertyEntity({ eventClick: reprovarClick, type: types.event, text: "Reprovar", idGrid: guid(), visible: ko.observable(true) });
    this.DesfazerAprovacao = PropertyEntity({ eventClick: desfazerAprovacaoClick, type: types.event, text: "Defazer Aprovação", idGrid: guid(), visible: ko.observable(false) });
    this.DesfazerRejeicao = PropertyEntity({ eventClick: desfazerRejeicaoClick, type: types.event, text: "Defazer Rejeição", idGrid: guid(), visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalheGestaoDocumento() {
    _detalheGestaoDocumento = new DetalheGestaoDocumento();
    KoBindings(_detalheGestaoDocumento, "knockoutDetalheGestaoDocumento");

    _detalheCTe = new DetalheDocumento();
    KoBindings(_detalheCTe, "knoutDetalheCTe");

    _detalhePreCTe = new DetalheDocumento();
    KoBindings(_detalhePreCTe, "knoutDetalhePreCTe");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja aprovar esse CT-e?", function () {
        executarReST("GestaoDocumento/AprovarInconsistencia", RetornarObjetoPesquisa(_detalheGestaoDocumento), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento aprovado com sucesso");
                    atualizarGridGestaoDocumentos();
                    fecharModalDetalheGestaoDocumento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function emTratativaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja colocar o CT-e em tratativa?", function () {
        executarReST("GestaoDocumento/EmTratativa", { Codigo: _detalheGestaoDocumento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e colocado em tratativa");
                    atualizarGridGestaoDocumentos();
                    fecharModalDetalheGestaoDocumento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function desfazerAprovacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja desfazer a aprovação deste CT-e?", function () {
        executarReST("GestaoDocumento/DesfazerAprovacao", { Codigo: _detalheGestaoDocumento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aprovação do documento revertida com sucesso");
                    atualizarGridGestaoDocumentos();
                    fecharModalDetalheGestaoDocumento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function desfazerRejeicaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja desfazer a rejeição deste CT-e?", function () {
        executarReST("GestaoDocumento/DesfazerRejeicao", { Codigo: _detalheGestaoDocumento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Rejeição do documento revertida com sucesso");
                    atualizarGridGestaoDocumentos();
                    fecharModalDetalheGestaoDocumento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function reprovarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja reprovar esse CT-e?", function () {
        executarReST("GestaoDocumento/ReprovarInconsistencia", RetornarObjetoPesquisa(_detalheGestaoDocumento), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento rejeitado com sucesso");
                    atualizarGridGestaoDocumentos();
                    fecharModalDetalheGestaoDocumento();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalheGestaoDocumento(registroSelecionado) {
    LimparCampos(_detalheGestaoDocumento);
    LimparCampos(_detalheCTe);
    LimparCampos(_detalhePreCTe);

    executarReST("GestaoDocumento/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalheGestaoDocumento, { Data: retorno.Data.Detalhes });
                PreencherObjetoKnout(_detalheCTe, { Data: retorno.Data.CTe });

                if (retorno.Data.PreCTe != null) {
                    PreencherObjetoKnout(_detalhePreCTe, { Data: retorno.Data.PreCTe });

                    _detalhePreCTe.DownloadXMLPreCTe.visible(true);

                    verificarDivergenciasEntreDocumentoEsperadoERecebido(_detalhePreCTe, _detalheCTe);
                }
                else {
                    _detalhePreCTe.DownloadXMLPreCTe.visible(false);

                    definirDocumentoRecebidoComoDivergente(_detalhePreCTe, _detalheCTe);
                }

                controlarComponentesDetalheGestaoDocumento();

                Global.abrirModal('divModalDetalheGestaoDocumento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarComponentesDetalheGestaoDocumento() {
    var habilitarObservacaoAprovacao = false;
    var exibirBotaoAprovar = false;
    var exibirBotaoDesfazerAprovacao = false;
    var exibirBotaoDesfazerRejeicao = false;
    var exibirBotaoReprovar = false;
    var exibirBotaoEmTratativa = false;
    var exibirObservacaoAprovacao = false;


    if (_detalheGestaoDocumento.SituacaoGestaoDocumento.val() == EnumSituacaoGestaoDocumento.Inconsistente || _detalheGestaoDocumento.SituacaoGestaoDocumento.val() == EnumSituacaoGestaoDocumento.EmTratativa) {
        habilitarObservacaoAprovacao = true;
        exibirBotaoAprovar = true;
        exibirBotaoReprovar = true;
        exibirBotaoEmTratativa = _detalheGestaoDocumento.SituacaoGestaoDocumento.val() == EnumSituacaoGestaoDocumento.Inconsistente && _CONFIGURACAO_TMS.PermitirDeixarDocumentoEmTratativa;
        exibirObservacaoAprovacao = true;
    }
    else {
        _detalheGestaoDocumento.DesfazerAprovacao.visible(false);

        if (_detalheGestaoDocumento.SituacaoGestaoDocumento.val() == EnumSituacaoGestaoDocumento.Rejeitado)
            exibirBotaoDesfazerRejeicao = true;
        else
            exibirObservacaoAprovacao = true;

        if (EnumSituacaoGestaoDocumento.isAprovado(_detalheGestaoDocumento.SituacaoGestaoDocumento.val()))
            exibirBotaoDesfazerAprovacao = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao, _PermissoesPersonalizadas);
    }

    if (_detalheGestaoDocumento.MotivoInconsistenciaGestaoDocumento.val() == EnumMotivoInconsistenciaGestaoDocumento.SemCarga) {
        exibirBotaoAprovar = false;
        exibirObservacaoAprovacao = false;
    }

    if (!permitirEditarInformacoesGestaoDocumentos()) {
        habilitarObservacaoAprovacao = false;
        exibirBotaoAprovar = false;
        exibirBotaoDesfazerAprovacao = false;
        exibirBotaoDesfazerRejeicao = false;
        exibirBotaoReprovar = false;
    }

    _detalheGestaoDocumento.Aprovar.visible(exibirBotaoAprovar);
    _detalheGestaoDocumento.DesfazerAprovacao.visible(exibirBotaoDesfazerAprovacao);
    _detalheGestaoDocumento.DesfazerRejeicao.visible(exibirBotaoDesfazerRejeicao);
    _detalheGestaoDocumento.ObservacaoAprovacao.visible(exibirObservacaoAprovacao);
    _detalheGestaoDocumento.ObservacaoAprovacao.enable(habilitarObservacaoAprovacao);
    _detalheGestaoDocumento.Reprovar.visible(exibirBotaoReprovar);
    _detalheGestaoDocumento.EmTratativa.visible(exibirBotaoEmTratativa);
}

function fecharModalDetalheGestaoDocumento() {
    Global.fecharModal('divModalDetalheGestaoDocumento');
}

// #endregion Funções Privadas
