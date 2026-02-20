/// <reference path="../../Enumeradores/EnumSituacaoCargaMDFeManual.js" />


var _integracaoGeral;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.MDFeManual = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}
var _HTMLIntegracoesMDFeManual;
function LoadIntegracoesMDFeManual() {
    $.get("Content/Static/Carga/CargaMDFeManualIntegracoes.html?dyn=" + guid(), function (data) {
        _HTMLIntegracoesMDFeManual = data;

    });
}

function BuscarDadosIntegracoesMDFeManual(e, sender) {
    executarReST("CargaMDFeManualIntegracao/ObterDadosIntegracoes", { CargaMDFeManual: _cargaMDFeManual.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesCTe.length > 0) {
                $("#DivIntegracaoMDFeManual").html(_HTMLIntegracoesMDFeManual);
                _integracaoGeral = new IntegracaoGeral();
                KoBindings(_integracaoGeral, "divIntegracao");
                _integracaoGeral.MDFeManual.val(_cargaMDFeManual.Codigo.val());

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoMDFeManual(_cargaMDFeManual, "divIntegracaoMDFeManual");

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#divIntegracaoCTe .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoCTe.Tipo.visible(true);
                    }
                } else {
                    $("#divIntegracaoMDFeManual").hide();

                    $("#liIntegracaoMDFeManual a").tab('show');
                }

                if (_cargaMDFeManual.Situacao.val() == EnumSituacaoMDFeManual.FalhaIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);


            } else {
                $("#DivIntegracaoMDFeManual").html('<p class="alert alert-success">Não existem integrações para esse MDF-e Manual</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Finalizar sem concluir integrações", function () {
        executarReST("CargaMDFeManualIntegracao/Finalizar", { CargaMDFeManual: _cargaMDFeManual.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SolicitacaoRealizadaComSucesso);
                    BuscarCargaMDFeManualPorCodigo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}
