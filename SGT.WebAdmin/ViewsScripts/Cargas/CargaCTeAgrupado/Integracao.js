

var _integracaoGeral;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.CTeAgrupado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}
var _HTMLIntegracoesCTeAgrupado;
function LoadIntegracoes() {
    $.get("Content/Static/CTeAgrupado/CTeAgrupadoIntegracoes.html?dyn=" + guid(), function (data) {
        _HTMLIntegracoesCTeAgrupado = data;

    });
}

function BuscarDadosIntegracoesCTeAgrupado(e, sender) {
    executarReST("CargaCTeAgrupadoIntegracao/ObterDadosIntegracoes", { CargaCTeAgrupado: _cargaCTeAgrupado.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesCTe.length > 0) {
                $("#DivIntegracaoCTeAgrupado").html(_HTMLIntegracoesCTeAgrupado);
                _integracaoGeral = new IntegracaoGeral();
                KoBindings(_integracaoGeral, "divIntegracao");
                _integracaoGeral.CTeAgrupado.val(_cargaCTeAgrupado.Codigo.val());

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoCTeAgrupado(_cargaCTeAgrupado, "divIntegracaoCTeAgrupado");

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#divIntegracaoCTe .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        $("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoCTe.Tipo.visible(true);
                    }
                } else {
                    $("#divIntegracaoCTeAgrupado").hide();

                    $("#liIntegracaoCTeAgrupado a").tab('show');
                }

                

                if (_cargaCTeAgrupado.Situacao.val() == EnumSituacaoCargaCTeAgrupado.FalhaIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);


            } else {
                $("#DivIntegracaoCTeAgrupado").html('<p class="alert alert-success">Não existem integrações para esse CT-e Agrupado</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Finalizar sem concluir integrações", function () {
        executarReST("CargaCTeAgrupadoIntegracao/Finalizar", { CargaCTeAgrupado: _cargaCTeAgrupado.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SolicitacaoRealizadaComSucesso);
                    BuscarCargaCTeAgrupadoPorCodigo(_cargaCTeAgrupado.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}
