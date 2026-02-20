/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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
/// <reference path="PagamentoMotoristaTMS.js" />
/// <reference path="EtapasPagamentoMotorista.js" />
/// <reference path="IntegracaoEnvioPagamentoMotorista.js" />
/// <reference path="IntegracaoRetornoPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />

var _integracaoPagamentoMotoristaGeral;

var IntegracaoPagamentoMotoristaGeral = function () {
    this.PagamentoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FinalizarEtapa = PropertyEntity({
        eventClick: function (e) {
            FinalizarEtapaIntegracaoPagamentoMotorista();
        }, type: types.event, text: "Finalizar Etapa", idGrid: guid(), visible: ko.observable(false)
    });
};

//*******EVENTOS*******

function BuscarDadosIntegracoesPagamentoMotorista(e, sender) {
    executarReST("PagamentoMotoristaTMS/ObterDadosIntegracoes", { PagamentoMotorista: _pagamentoMotorista.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data.ComIntegracao) {
                $("#divConteudoIntegracao").show();
                $("#divNaoPossuiIntegracao").hide();

                _integracaoPagamentoMotoristaGeral = new IntegracaoPagamentoMotoristaGeral();
                _integracaoPagamentoMotoristaGeral.PagamentoMotorista.val(_pagamentoMotorista.Codigo.val());

                KoBindings(_integracaoPagamentoMotoristaGeral, "divIntegracao");

                LoadIntegracaoEnvio(_pagamentoMotorista, "divIntegracaoEnvio");
                LoadIntegracaoRetorno(_pagamentoMotorista, "divIntegracaoRetorno");

                if (_pagamentoMotorista.Situacao.val() == EnumSituacaoPagamentoMotorista.AgIntegracao)
                    _integracaoPagamentoMotoristaGeral.FinalizarEtapa.visible(true);

            } else {
                $("#divConteudoIntegracao").hide();
                $("#divNaoPossuiIntegracao").show();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function FinalizarEtapaIntegracaoPagamentoMotorista() {
    exibirConfirmacao("Atenção!", "Deseja realmente finalizar a etapa de integração sem concluir as integrações?", function () {
        executarReST("PagamentoMotoristaTMS/Finalizar", { PagamentoMotorista: _pagamentoMotorista.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa finalizada com sucesso!");
                    Etapa3Aprovada();
                    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(true);
                    _integracaoPagamentoMotoristaGeral.FinalizarEtapa.visible(false);

                    if (!string.IsNullOrWhiteSpace(r.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", r.Data.MensagemRetorno);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}
