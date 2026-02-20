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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoCancelamento;

var ResumoCancelamento = function () {
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Carga.getFieldDescription(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription()});
    this.Origem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription()});
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription()});
    this.Destino = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription() });
this.DataCancelamento = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataCancelamento.getFieldDescription() });
this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()});
this.Tipo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription()});
this.MensagemRejeicaoCancelamento = PropertyEntity({ text: Localization.Resources.Gerais.Geral.MotivoRejeicao.getFieldDescription(), visible: ko.observable(false) });

}

//*******EVENTOS*******

function LoadResumoCancelamento() {
    _resumoCancelamento = new ResumoCancelamento();
    KoBindings(_resumoCancelamento, "knockoutResumoCancelamento");
}

//*******MÉTODOS*******

function PreecherResumoCancelamento(dados) {
    _resumoCancelamento.NumeroCarga.visible(true);

    _resumoCancelamento.NumeroCarga.val(dados.Carga.CodigoCargaEmbarcador);
    _resumoCancelamento.Remetente.val(dados.Carga.Remetente);
    _resumoCancelamento.Origem.val(dados.Carga.Origem);
    _resumoCancelamento.Destinatario.val(dados.Carga.Destinatario);
    _resumoCancelamento.Destino.val(dados.Carga.Destino);
    _resumoCancelamento.DataCancelamento.val(dados.DataCancelamento);
    _resumoCancelamento.Situacao.val(dados.DescricaoSituacao);
    _resumoCancelamento.Tipo.val(dados.DescricaoTipo);
    _resumoCancelamento.MensagemRejeicaoCancelamento.val(dados.MensagemRejeicaoCancelamento);
    if (dados.Situacao == EnumSituacaoCancelamentoCarga.RejeicaoCancelamento) {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(true);
    } else {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(false);
    }
}

function LimparResumoCancelamento() {
    _resumoCancelamento.NumeroCarga.visible(false);
    LimparCampos(_resumoCancelamento);
}