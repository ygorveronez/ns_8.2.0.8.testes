/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../TravamentoChave/TravamentoChave.js" />
/// <reference path="FluxoPatio.js" />

// #region Funções de Inicialização

function OnLoadTravamentoChave() {
    //if (PermiteAuditar()) {
    //    _travamentoChave.Auditar.visible(true);
    //}
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function ExibirDetalhesTravaChaveFluxoPatio(knoutFluxo, opt) {
    exibirDetalhesTravaChaveFluxoPatioCallback(knoutFluxo, EnumEtapaFluxoGestaoPatio.TravamentoChave, function () {
        if (_travamentoChave.Travado.val()) {
            _travamentoChave.Atualizar.visible(false);
            _travamentoChave.PaletesPBR.enable(false);
            _travamentoChave.PaletesChep.enable(false);
        }

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_TravarChave, _PermissoesPersonalizadasFluxoPatio))
            _travamentoChave.Atualizar.visible(false);
    });
}

function ExibirDetalhesLiberaChaveFluxoPatio(knoutFluxo, opt) {
    exibirDetalhesTravaChaveFluxoPatioCallback(knoutFluxo, EnumEtapaFluxoGestaoPatio.LiberacaoChave, function () {
        if (!_travamentoChave.Travado.val()) {
            _travamentoChave.Atualizar.visible(false);
            _travamentoChave.PaletesPBR.enable(false);
            _travamentoChave.PaletesChep.enable(false);
        }

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_LiberarChave, _PermissoesPersonalizadasFluxoPatio))
            _travamentoChave.Atualizar.visible(false);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function AtualizarTravamentoChaveGestaoPatio() {
    atualizarFluxoPatio();
    Global.fecharModal('divModalDetalhesTravamentoChave');
}

function exibirDetalhesTravaChaveFluxoPatioCallback(knoutFluxo, etapa, callback) {
    _travamentoChave.Atualizar.visible(true);
    _travamentoChave.PaletesPBR.enable(true);
    _travamentoChave.PaletesChep.enable(true);
    _callbackTravamentoChaveAtualizado = AtualizarTravamentoChaveGestaoPatio;
    _fluxoAtual = knoutFluxo;

    executarReST("TravamentoChave/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val(), Etapa: etapa }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _travamentoChave.PrevisaoTravamento.visible(false);
                _travamentoChave.DataTravamento.visible(false);
                _travamentoChave.VoltarEtapa.visible(false);
                _travamentoChave.Etapa.val(etapa);

                limparCamposTravamentoChave();
                PreencherObjetoKnout(_travamentoChave, arg);
                preecherRetornoTravaChave(arg);
                callback();

                _travamentoChave.PrevisaoTravamento.val(arg.Data.DataLiberacaoChavePrevista);

                if (arg.Data.DataLiberacaoChave != "") {
                    _travamentoChave.DataTravamento.val(arg.Data.DataLiberacaoChave);
                    _travamentoChave.DataTravamento.visible(true);
                }

                if (arg.Data.PermitirEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    if (!primeiraEtapa) {
                        if ((etapa == EnumEtapaFluxoGestaoPatio.LiberacaoChave) && _configuracaoGestaoPatio.LiberaChavePermiteVoltar)
                            _travamentoChave.VoltarEtapa.visible(true);
                        else if ((etapa == EnumEtapaFluxoGestaoPatio.TravamentoChave) && _configuracaoGestaoPatio.TravaChavePermiteVoltar)
                            _travamentoChave.VoltarEtapa.visible(true);
                    }
                }

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_travamentoChave);

                Global.abrirModal('divModalDetalhesTravamentoChave');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

// #endregion Funções Privadas
