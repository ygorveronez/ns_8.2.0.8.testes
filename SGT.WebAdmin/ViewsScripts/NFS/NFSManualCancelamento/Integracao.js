
var _integracaoGeral;
var _HTMLIntegracaoNFSManualCancelamento;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.NFSManualCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
};

function BuscarHTMLIntegracaoNFSManualCancelamento() {
    var p = new promise.Promise();

    if (!string.IsNullOrWhiteSpace(_HTMLIntegracaoNFSManualCancelamento))
        p.done();
    else {
        $.get("Content/Static/NFSManualCancelamento/NFSManualCancelamentoIntegracao.html?dyn=" + guid(), function (data) {
            _HTMLIntegracaoNFSManualCancelamento = data;
            p.done();
        });
    }

    return p;
}

function BuscarDadosIntegracoesNFSManualCancelamento() {
    BuscarHTMLIntegracaoNFSManualCancelamento().then(function () {
        executarReST("NFSManualCancelamentoIntegracao/ObterDadosIntegracoes", { NFSManualCancelamento: _cancelamento.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0) {
                    $("#divIntegracaoNFSManualCancelamento").html(_HTMLIntegracaoNFSManualCancelamento.replace(/\b#divIntegracao\b/g, _etapaCancelamento.Etapa3.idGrid));

                    _integracaoGeral = new IntegracaoGeral();
                    _integracaoGeral.NFSManualCancelamento.val(_cancelamento.Codigo.val());

                    KoBindings(_integracaoGeral, "divIntegracao_" + _etapaCancelamento.Etapa3.idGrid);

                    if (r.Data.TiposIntegracoesEDI.length > 0) {
                        LoadIntegracaoEDI(_cancelamento, "divIntegracaoEDI_" + _etapaCancelamento.Etapa3.idGrid);
                    } else {
                        $("#" + "divIntegracaoEDI_" + _etapaCancelamento.Etapa3.idGrid).hide();
                        $("#" + "liIntegracaoEDI_" + _etapaCancelamento.Etapa3.idGrid).hide();
                        $("#" + "liIntegracaoCTe_" + _etapaCancelamento.Etapa3.idGrid + " a").tab('show');
                    }

                    if (r.Data.TiposIntegracoesCTe.length > 0) {
                        LoadIntegracaoCTe(_cancelamento, "divIntegracaoCTe_" + _etapaCancelamento.Etapa3.idGrid);

                        if (r.Data.TiposIntegracoesCTe.length > 1) {
                            $("#" + "divIntegracaoCTe_" + _etapaCancelamento.Etapa3.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                            $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                            $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                            _integracaoCTe.Tipo.visible(true);
                        }
                    } else {
                        $("#" + "divIntegracaoCTe_" + _etapaCancelamento.Etapa3.idGrid).hide();
                        $("#" + "liIntegracaoCTe_" + _etapaCancelamento.Etapa3.idGrid).hide();

                        if (r.Data.TiposIntegracoesEDI.length <= 0)
                            $("#" + "liIntegracaoLancamentoNFSManual_" + _etapaCancelamento.Etapa3.idGrid + " a").tab('show');
                    }

                    if (_cancelamento.Situacao.val() == EnumSituacaoNFSManualCancelamento.AgIntegracao)
                        _integracaoGeral.FinalizarEtapa.visible(true);

                    //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.LancamentoNFSManual_ConfirmarIntegracao, _PermissoesPersonalizadasLancamentoNFSManual))
                    //    _integracaoGeral.FinalizarEtapa.visible(false);

                } else {
                    $("#divIntegracaoNFSManualCancelamento").html('<p class="alert alert-success">Não existem integrações disponíveis para este cancelamento de NFS Manual.</p>');
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("NFSManualCancelamentoIntegracao/Finalizar", { NFSManualCancelamento: _cancelamento.Codigo.val() }, function (r) {
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

//*******MÉTODOS*******