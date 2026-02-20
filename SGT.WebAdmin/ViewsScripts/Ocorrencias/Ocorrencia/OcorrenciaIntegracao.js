/// <reference path="OcorrenciaIntegracaoCarga.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CargaFrete.js" />
/// <reference path="CargaFreteRota.js" />
/// <reference path="CargaTransportador.js" />
/// <reference path="CargaFrete.js" />
/// <reference path="CargaFreteRota.js" />
/// <reference path="CargaImpressao.js" />
/// <reference path="CargaMDFe.js" />
/// <reference path="CargaNotasFiscais.js" />
/// <reference path="CargaTipo.js" />
/// <reference path="OcorrenciaIntegracaoCTe.js" />
/// <reference path="OcorrenciaIntegracaoEDI.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />

var _integracaoGeralOcorrencia;
var _HTMLIntegracaoOcorrencia;

var IntegracaoGeralOcorrencia = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracao();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.FinalizarEtapa, idGrid: guid(), visible: ko.observable(false)
    });
}

//*******EVENTOS*******

function loadOcorrenciaIntegracao() {
    $.get("Content/Static/Ocorrencia/IntegracaoOcorrencia.html?dyn=" + guid(), function (data) {
        _HTMLIntegracaoOcorrencia = data;
    });
}


function BuscarDadosIntegracoesOcorrencia(filialEmissora) {
    executarReST("OcorrenciaIntegracao/ObterDadosIntegracoes", { Ocorrencia: _ocorrencia.Codigo.val(), FilialEmissora: filialEmissora }, function (r) {
        if (r.Success) {

            if (r.Data.TiposIntegracoesCTe.length > 0 || r.Data.TiposIntegracoesEDI.length > 0) {
                $("#divConteudoIntegracao").html(_HTMLIntegracaoOcorrencia.replace(/\b#divIntegracao\b/g, _etapaOcorrencia.Etapa4.idGrid));

                $("#divConteudoIntegracao").show();
                $("#divNaoPossuiIntegracao").hide();


                _integracaoGeralOcorrencia = new IntegracaoGeralOcorrencia();
                _integracaoGeralOcorrencia.Ocorrencia.val(_ocorrencia.Codigo.val());

                KoBindings(_integracaoGeralOcorrencia, "divIntegracao_" + _etapaOcorrencia.Etapa4.idGrid);

                if (r.Data.TiposIntegracoesEDI.length > 0) {
                    LoadIntegracaoEDIOcorrencia(_ocorrencia, "divIntegracaoEDI_" + _etapaOcorrencia.Etapa4.idGrid, filialEmissora);
                } else {
                    $("#" + "divIntegracaoEDI_" + _etapaOcorrencia.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoEDI_" + _etapaOcorrencia.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaOcorrencia.Etapa4.idGrid + " a").tab('show');
                }

                if (r.Data.TiposIntegracoesCTe.length > 0) {
                    LoadIntegracaoCTeOcorrencia(_ocorrencia, "divIntegracaoCTe_" + _etapaOcorrencia.Etapa4.idGrid);

                    if (r.Data.TiposIntegracoesCTe.length > 1) {
                        $("#" + "divIntegracaoCTe_" + _etapaOcorrencia.Etapa4.idGrid + " .divBotoesIntegracaoCTe").removeClass("col-md-6 col-lg-8").addClass("col-md-12 col-lg-4");
                        //$("#" + _integracaoCTe.Pesquisar.id).removeClass("input-margin-top-24-md");
                        //$("#" + _integracaoCTe.ReenviarTodos.id).removeClass("input-margin-top-24-md");
                        //_integracaoCTe.Tipo.visible(true);
                    }
                } else {
                    $("#" + "divIntegracaoCTe_" + _etapaOcorrencia.Etapa4.idGrid).hide();
                    $("#" + "liIntegracaoCTe_" + _etapaOcorrencia.Etapa4.idGrid).hide();

                    if (r.Data.TiposIntegracoesEDI.length <= 0)
                        $("#" + "liIntegracaoCarga_" + _etapaOcorrencia.Etapa4.idGrid + " a").tab('show');
                }

                if ((_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgIntegracao || _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.FalhaIntegracao) && !filialEmissora)
                    _integracaoGeralOcorrencia.FinalizarEtapa.visible(true);

            } else {
                $("#divConteudoIntegracao").hide();
                $("#divNaoPossuiIntegracao").show();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function FinalizarEtapaIntegracao() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("OcorrenciaIntegracao/Finalizar", { Ocorrencia: _ocorrencia.Codigo.val() }, function (r) {
            if (r.Data != null) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.Ocorrencia.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.EtapaFinalizadaComSucesso);
                    Etapa4Aprovada();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}
