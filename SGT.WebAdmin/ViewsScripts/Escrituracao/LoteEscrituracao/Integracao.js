
var _integracaoGeral;
var _HTMLIntegracaoLoteEscrituracao;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.LoteEscrituracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}

function BuscarHTMLINtegracaoLoteEscrituracao() {
    $.get("Content/Static/Escrituracao/LoteEscrituracaoIntegracao.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoLoteEscrituracao = data;
    });
}

function BuscarDadosIntegracoesLoteEscrituracao(sender) {
    executarReST("LoteEscrituracaoIntegracao/ObterDadosIntegracoes", { LoteEscrituracao: _selecaoDocumentos.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.TiposIntegracoesLote.length > 0 || r.Data.TiposIntegracoesEDI.length > 0) {
                $("#DivIntegracaoLoteEscrituracao").html(_HTMLIntegracaoLoteEscrituracao.replace(/\b#divIntegracao\b/g, _etapaLoteEscrituracao.Etapa2.idGrid));

                _integracaoGeral = new IntegracaoGeral();
                _integracaoGeral.LoteEscrituracao.val(_selecaoDocumentos.Codigo.val());

                KoBindings(_integracaoGeral, "divIntegracao_" + _etapaLoteEscrituracao.Etapa2.idGrid);

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDI(_selecaoDocumentos, "divIntegracaoEDI_" + _etapaLoteEscrituracao.Etapa2.idGrid);
                } else {
                    $("#" + "divIntegracaoEDI_" + _etapaLoteEscrituracao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + _etapaLoteEscrituracao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoLote_" + _etapaLoteEscrituracao.Etapa2.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesLote.length > 0) {
                    LoadIntegracaoLote(_selecaoDocumentos, "divIntegracaoLote_" + _etapaLoteEscrituracao.Etapa2.idGrid);

                    if (r.Data.TiposIntegracoesLote.length > 1) {
                        $("#" + "divIntegracaoLote_" + _etapaLoteEscrituracao.Etapa2.idGrid + " .divBotoesIntegracaoLote").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        $("#" + _integracaoLote.Pesquisar.id).removeClass("input-margin-top-24-md");
                        $("#" + _integracaoLote.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        _integracaoLote.Tipo.visible(true);
                    }
                } else {
                    $("#" + "divIntegracaoLote_" + _etapaLoteEscrituracao.Etapa2.idGrid).hide();
                    $("#" + "liIntegracaoLote_" + _etapaLoteEscrituracao.Etapa2.idGrid).hide();

                    if (r.Data.TiposIntegracoesEDI.length <= 0)
                        $("#" + "liIntegracaoLoteEscrituracao_" + _etapaLoteEscrituracao.Etapa2.idGrid + " a").tab('show');
                }

                if (_loteEscrituracao.Situacao.val() == EnumSituacaoLoteEscrituracao.AgIntegracao)
                    _integracaoGeral.FinalizarEtapa.visible(true);

                //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.LoteEscrituracao_ConfirmarIntegracao, _PermissoesPersonalizadasLoteEscrituracao))
                //    _integracaoGeral.FinalizarEtapa.visible(false);

            } else {
                $("#DivIntegracaoLoteEscrituracao").html('<p class="alert alert-success">Não existem integrações disponíveis para esse Lote de Escrituração.</p>');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("LoteEscrituracaoIntegracao/Finalizar", { LoteEscrituracao: _selecaoDocumentos.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa finalizada com sucesso!");
                    _loteEscrituracao.Situacao.val(EnumSituacaoLoteEscrituracao.Finalizado);
                    SetarEtapasLoteEscrituracao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}
