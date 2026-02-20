
var _integracaoGeral;

//*******EVENTOS*******

var IntegracaoGeral = function () {
    this.Transbordo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true)
    });
}
var _HTMLIntegracoesTransbordo;
function LoadIntegracoes() {
    $.get("Content/Static/Transbordo/TransbordoIntegracoes.html?dyn=" + guid(), function (data) {
        _HTMLIntegracoesTransbordo = data;
    });
}

function BuscarDadosIntegracoesTransbordo(e, sender) {
    executarReST("TransbordoIntegracao/ObterDadosIntegracoes", { Transbordo: _transbordo.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                $("#DivIntegracaoTransbordo").html(_HTMLIntegracoesTransbordo);
                _integracaoGeral = new IntegracaoGeral();
                KoBindings(_integracaoGeral, "divIntegracao");
                _integracaoGeral.Transbordo.val(_transbordo.Codigo.val());

                if (r.Data.TiposIntegracoesTransbordo.length > 0) {
                    LoadIntegracaoTransbordo(_transbordo, "knoutIntegracaoTransbordo");
                    $("#liIntegracaoTransbordo a").tab('show');
                } else {
                    $("#knoutIntegracaoTransbordo").html('<p class="alert alert-success">' + Localization.Resources.Cargas.Transbordo.NaoExistemIntegracoesCarga + '</p>');
                    $("#liIntegracaoTransbordo").hide();
                }

                if (/*_transbordo.SituacaoTransbordo.val() == EnumSituacaoTransbordo.RejeicaoTransbordo*/ true)
                    _integracaoGeral.FinalizarEtapa.visible(true);

            } else {
                $("#knoutIntegracaoTransbordo").html('<p class="alert alert-success">' + Localization.Resources.Cargas.Transbordo.NaoExistemIntegracoesCarga + '</p>');
                $("#liIntegracaoTransbordo").hide();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.TransbordoCarga.FinalizarSemConcluirIntegracoes, function () {
        executarReST("TransbordoIntegracao/Finalizar", { Transbordo: _transbordo.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SolicitacaoRealizadaComSucesso);
                    BuscarTransbordoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}
