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
/// <reference path="../../Enumeradores/EnumSituacaoAjusteTabelaFrete.js" />
/// <reference path="AjusteTabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracao;

var Integracao = function () {
    // Mensagem
    this.Mensagem = PropertyEntity({ text: ko.observable(""), cssClass: ko.observable(""), visible: ko.observable(false) });
    this.Integrar = PropertyEntity({ eventClick: integrarClick, type: types.event, text: "Integrar", visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadIntegracao() {
    _integracao = new Integracao();
    KoBindings(_integracao, "knockoutIntegracao");
}


function integrarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Você realmente deseja integrar o reajuste?", function () {
        var dados = {
            Codigo: _ajusteTabelaFrete.Codigo.val()
        }
        executarReST("AjusteTabelaFrete/IntegrarAjuste", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ajuste integrado com sucesso.");
                    EditarAjusteTabelaFrete({ Codigo: _ajusteTabelaFrete.Codigo.val() });
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}


//*******MÉTODOS*******
function MensagemIntegracao() {
    var situacao = _ajusteTabelaFrete.Situacao.val();

    if (situacao == EnumSituacaoAjusteTabelaFrete.AgIntegracao) {
        _integracao.Mensagem.visible(true);
        _integracao.Integrar.visible(true);
        _integracao.Mensagem.text("Aguardando a integração do reajuste.");
        _integracao.Mensagem.cssClass("warning");
    }
    else if (situacao == EnumSituacaoAjusteTabelaFrete.Finalizado) {
        _integracao.Mensagem.visible(true);
        _integracao.Integrar.visible(false);
        _integracao.Mensagem.text("Reajuste integrado com sucesso.");
        _integracao.Mensagem.cssClass("success");
    }
    else {
        _integracao.Mensagem.visible(false);
        _integracao.Integrar.visible(false);
    }
}