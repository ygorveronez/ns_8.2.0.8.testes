
var _integracaoGeral;
var _HTMLIntegracaoLancamentoNFSManual;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.LancamentoNFSManual = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}

function BuscarHTMLINtegracaoNFSManual() {
    $.get("Content/Static/NFS/NFSManualIntegracao.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoLancamentoNFSManual = data;
    });
}

function BuscarDadosIntegracoesLancamentoNFSManual(sender) {
    executarReST("NFSManualIntegracao/ObterDadosIntegracoes", { LancamentoNFSManual: _dadosEmissao.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0) {
                $("#DivIntegracaoNFSManual").html(_HTMLIntegracaoLancamentoNFSManual.replace(/\b#divIntegracao\b/g, _etapaNFS.Etapa4.idGrid));

                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.LancamentoNFSManual.val(_dadosEmissao.Codigo.val());

                KoBindings(_integracaoGeral, "divIntegracao_" + _etapaNFS.Etapa4.idGrid);

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDI(_dadosEmissao, "divIntegracaoEDI_" + _etapaNFS.Etapa4.idGrid);
                } else {
                    $("#" + "divIntegracaoEDI_" + _etapaNFS.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + _etapaNFS.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaNFS.Etapa4.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoCTe(_dadosEmissao, "divIntegracaoCTe_" + _etapaNFS.Etapa4.idGrid);

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#" + "divIntegracaoCTe_" + _etapaNFS.Etapa4.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoCTe.Tipo.visible(true);
                    }
                } else {
                    $("#" + "divIntegracaoCTe_" + _etapaNFS.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaNFS.Etapa4.idGrid).hide();

                    if (r.Data.TiposIntegracoesEDI.length <= 0)
                        $("#" + "liIntegracaoLancamentoNFSManual_" + _etapaNFS.Etapa4.idGrid + " a").tab('show');
                }

                if (_nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.AgIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);

                //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.LancamentoNFSManual_ConfirmarIntegracao, _PermissoesPersonalizadasLancamentoNFSManual))
                //    _integracaoGeral.FinalizarEtapa.visible(false);

            } else {
                $("#DivIntegracaoNFSManual").html('<p class="alert alert-success">Não existem integrações disponíveis para este lançamento de NFS Manual.</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("NFSManualIntegracao/Finalizar", { LancamentoNFSManual: _dadosEmissao.Codigo.val() }, function (r) {
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
