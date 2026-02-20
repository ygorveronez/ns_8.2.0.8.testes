
var _integracaoGeralMiro;
var _HTMLIntegracaoLoteEscrituracaoMiro;

//*******EVENTOS*******

var IntegracaoGeralMiro = function () {
    this.LoteEscrituracao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracaoMiro();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}

function BuscarHTMLINtegracaoLoteEscrituracaoMiro() {
    $.get("Content/Static/Escrituracao/LoteEscrituracaoIntegracao.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoLoteEscrituracaoMiro = data;
    });
}

function BuscarDadosIntegracoesLoteEscrituracaoMiro(sender) {
    executarReST("LoteEscrituracaoIntegracao/ObterDadosIntegracoes", { LoteEscrituracao: _selecaoDocumentos.Codigo.val() }, function (r) {
    });
}

function FinalizarEtapaIntegracaoMiro() {
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
