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
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCTeSemCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoCancelamentoCTeSemCarga;

var ResumoCancelamentoCTeSemCarga = function () {
    this.Status = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.DataInclusao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataCancelamento.getFieldDescription() });
    this.UsuarioInclusao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.UsuarioInclusao.getFieldDescription() });
    this.MotivoCancelamento = PropertyEntity({ text: Localization.Resources.Gerais.Geral.MotivoCancelamento.getFieldDescription() });
    this.MotivoRejeicao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.MotivoRejeicao.getFieldDescription(), visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadResumoCancelamentoCTeSemCarga() {
    _resumoCancelamento = new ResumoCancelamentoCTeSemCarga();
    KoBindings(_resumoCancelamento, "knockoutResumoCancelamentoCTeSemCarga");
}

//*******MÉTODOS*******

function PreecherResumoCancelamentoCTeSemCarga(dados) {

    _resumoCancelamento.Status.val(dados.Status);
    _resumoCancelamento.DataInclusao.val(dados.DataInclusao);
    _resumoCancelamento.UsuarioInclusao.val(dados.UsuarioInclusao);
    _resumoCancelamento.MotivoCancelamento.val(dados.MotivoCancelamento);
    _resumoCancelamento.MotivoRejeicao.val(dados.MotivoRejeicao);
    if (dados.Status == EnumSituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento) {
        _resumoCancelamento.MotivoRejeicao.visible(true);
    } else {
        _resumoCancelamento.MotivoRejeicao.visible(false);
    }
}

function LimparResumoCancelamentoCTeSemCarga() {
    _resumoCancelamento.NumeroCarga.visible(false);
    LimparCampos(_resumoCancelamento);
}