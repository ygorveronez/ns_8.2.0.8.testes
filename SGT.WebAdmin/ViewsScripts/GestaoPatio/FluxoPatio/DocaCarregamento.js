/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../DocaCarregamento/DocaCarregamento.js" />
/// <reference path="FluxoPatio.js" />

// #region Funções de Inicialização

function OnLoadDocaCarregamento() {
    if (_configuracaoGestaoPatio.DocaDetalhada) {
        _docaCarregamento.DocaDetalhada.val(true);
        $("#divModalDetalhesDocaCarregamento").removeClass("modal-wide");
    }

    _docaCarregamento.NumeroDocaCarga.visible(true);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function ExibirDetalhesDocaCarregamentoFluxoPatio(knoutFluxo, opt) {
    _callbackDocaCarregamentoAtualizado = AtualizarDocaCarregamentoGestaoPatio;
    _callbackDocaCarregamentoVoltouEtapa = AtualizarDocaCarregamentoGestaoPatio;
    _callbackDocaCarregamentoRejeitarEtapa = AtualizarDocaCarregamentoGestaoPatio;
    _callbackDocaCarregamentoReabrirFluxo = AtualizarDocaCarregamentoGestaoPatio;
    _fluxoAtual = knoutFluxo;

    executarReST("DocaCarregamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                _docaCarregamento.PrevisaoCarregamento.visible(false);
                _docaCarregamento.VoltarEtapa.visible(false);
                _docaCarregamento.Atualizar.visible(false);
                _docaCarregamento.RejeitarEtapa.visible(false);
                _docaCarregamento.ReabrirFluxo.visible(false);
                _docaCarregamento.IntegrarDoca.visible(retorno.Data.ExibirBotaoIntegrarDoca && knoutFluxo.PossuiCarga.val());
                _docaCarregamento.ImprimirComprovanteCargaInformada.visible(isPermitirImprimirComprovanteCargaInformada() && knoutFluxo.PossuiCarga.val());

                limparCamposDocaCarregamento();
                PreencherObjetoKnout(_docaCarregamento, retorno);

                var possuiCarga = _docaCarregamento.Carga.val() != "";
                var primeiraEtapa = knoutFluxo.EtapaAtual.val() == 0;
                var fluxoRejeitado = (knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado || knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Cancelado);
                var fluxoAberto = (knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);

                _docaCarregamento.NumeroDoca.visible(possuiCarga && !_configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento);
                _docaCarregamento.NumeroDoca.required = _docaCarregamento.NumeroDoca.visible();
                _docaCarregamento.LocalCarregamento.visible(possuiCarga && _configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento);
                _docaCarregamento.LocalCarregamento.required = _docaCarregamento.LocalCarregamento.visible();
                _docaCarregamento.PossuiLaudo.visible(retorno.Data.PermiteInformarDadosLaudo);
                _docaCarregamento.EtapaAntecipada.val(!opt.etapaLiberada);

                if (retorno.Data.PodeConfirmarDoca)
                    _docaCarregamento.Atualizar.visible(true);

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDoca, _PermissoesPersonalizadasFluxoPatio))
                    _docaCarregamento.Atualizar.visible(false);

                if (_configuracaoGestaoPatio.InformarDocaCarregamentoPermiteVoltar) {
                    if (!primeiraEtapa && retorno.Data.PodeVoltarEtapa)
                        _docaCarregamento.VoltarEtapa.visible(true);

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.InformarDoca) {
                        _docaCarregamento.RejeitarEtapa.visible(fluxoAberto && _configuracaoGestaoPatio.PermitirRejeicaoFluxo);
                        _docaCarregamento.ReabrirFluxo.visible(fluxoRejeitado && knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado);
                    }
                }

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_docaCarregamento);

                Global.abrirModal('divModalDetalhesDocaCarregamento');                
                $(window).one('keyup', function (e) {
                    if (e.keyCode == 27)
                        Global.fecharModal('divModalDetalhesDocaCarregamento');
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function AtualizarDocaCarregamentoGestaoPatio() {
    atualizarFluxoPatio();
    Global.fecharModal('divModalDetalhesDocaCarregamento');
}

// #endregion Funções Privadas
