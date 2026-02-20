var _integracaoGeral;
var _HTMLIntegracaoLoteContabilizacao;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.LoteContabilizacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
};

function BuscarHTMLIntegracaoLoteContabilizacao() {
    $.get("Content/Static/Financeiro/LoteContabilizacaoIntegracao.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoLoteContabilizacao = data;
    });
}

function BuscarDadosIntegracoesLoteContabilizacao(sender) {
    executarReST("LoteContabilizacaoIntegracao/ObterDadosIntegracoes", { LoteContabilizacao: _selecaoMovimentos.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0) {
                $("#DivIntegracaoLoteContabilizacao").html(_HTMLIntegracaoLoteContabilizacao.replace(/\b#divIntegracao\b/g, _etapaLoteContabilizacao.Etapa2.idGrid));

                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.LoteContabilizacao.val(_selecaoMovimentos.Codigo.val());

                KoBindings(_integracaoGeral, "divIntegracao_" + _etapaLoteContabilizacao.Etapa2.idGrid);

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDI(_selecaoMovimentos, "divIntegracaoEDI_" + _etapaLoteContabilizacao.Etapa2.idGrid);
                } else {
                    $("#" + "divIntegracaoEDI_" + _etapaLoteContabilizacao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + _etapaLoteContabilizacao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaLoteContabilizacao.Etapa2.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoCTe(_selecaoMovimentos, "divIntegracaoCTe_" + _etapaLoteContabilizacao.Etapa2.idGrid);

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#" + "divIntegracaoCTe_" + _etapaLoteContabilizacao.Etapa2.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoCTe.Tipo.visible(true);
                    }
                } else {
                    $("#" + "divIntegracaoCTe_" + _etapaLoteContabilizacao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaLoteContabilizacao.Etapa2.idGrid).hide();

                    if (r.Data.TiposIntegracoesEDI.length <= 0)
                        $("#" + "liIntegracaoLoteContabilizacao_" + _etapaLoteContabilizacao.Etapa2.idGrid + " a").tab('show');
                }

                if (_loteContabilizacao.Situacao.val() == EnumSituacaoLoteContabilizacao.AgIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);

                //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.LoteEscrituracao_ConfirmarIntegracao, _PermissoesPersonalizadasLoteEscrituracao))
                //    _integracaoGeral.FinalizarEtapa.visible(false);

            } else {
                $("#DivIntegracaoLoteContabilizacao").html('<p class="alert alert-success">Não existem integrações disponíveis para esse Lote de Escrituração.</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("LoteContabilizacaoIntegracao/Finalizar", { LoteContabilizacao: _selecaoMovimentos.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa finalizada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}
