
var _integracaoGeral;
var _HTMLIntegracaoLoteEscrituracaoCancelamento;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.LoteEscrituracaoCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}

function BuscarHTMLIntegracaoLoteEscrituracaoCancelamento() {
    $.get("Content/Static/Escrituracao/LoteEscrituracaoCancelamentoIntegracao.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoLoteEscrituracaoCancelamento = data;
    });
}

function BuscarDadosIntegracoesLoteEscrituracaoCancelamento(sender) {
    executarReST("LoteEscrituracaoCancelamentoIntegracao/ObterDadosIntegracoes", { LoteEscrituracaoCancelamento: _selecaoDocumentos.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesEDI.length > 0) {
                $("#divIntegracaoLoteEscrituracaoCancelamento").html(_HTMLIntegracaoLoteEscrituracaoCancelamento.replace(/\b#divIntegracao\b/g, _etapaLoteEscrituracaoCancelamento.Etapa2.idGrid));

                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.LoteEscrituracaoCancelamento.val(_selecaoDocumentos.Codigo.val());

                KoBindings(_integracaoGeral, "divIntegracao_" + _etapaLoteEscrituracaoCancelamento.Etapa2.idGrid);

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDI(_selecaoDocumentos, "divIntegracaoEDI_" + _etapaLoteEscrituracaoCancelamento.Etapa2.idGrid);
                } else {
                    $("#" + "divIntegracaoEDI_" + _etapaLoteEscrituracaoCancelamento.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + _etapaLoteEscrituracaoCancelamento.Etapa2.idGrid).hide();
                }

                if (_loteEscrituracaoCancelamento.Situacao.val() == EnumSituacaoLoteEscrituracaoCancelamento.AgIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);
            } else {
                $("#divIntegracaoLoteEscrituracaoCancelamento").html('<p class="alert alert-success">Não existem integrações disponíveis para esse lote de escrituração.</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("LoteEscrituracaoCancelamentoIntegracao/Finalizar", { LoteEscrituracaoCancelamento: _dadosEmissao.Codigo.val() }, function (r) {
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
