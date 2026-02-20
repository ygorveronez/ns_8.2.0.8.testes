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
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="Ocorrencia.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoCancelamento;

var ResumoCancelamento = function () {
    this.NFS = PropertyEntity({ text: "NFS: ", visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: ko.observable("Transportador: "), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ text: "Local de Prestação: " });
    this.Tomador = PropertyEntity({ text: "Tomador: " });
    this.DataCancelamento = PropertyEntity({ text: "Data do Cancelamento: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.MensagemRejeicaoCancelamento = PropertyEntity({ text: "Motivo Rejeição: ", visible: ko.observable(false) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadResumoCancelamento() {
    _resumoCancelamento = new ResumoCancelamento();
    KoBindings(_resumoCancelamento, "knockoutResumoCancelamento");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _resumoCancelamento.Empresa.text("Empresa/Filial:")
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _resumoCancelamento.Empresa.visible(false);
    } 
}

//*******MÉTODOS*******

function PreecherResumoCancelamento(dados) {

    _resumoCancelamento.NFS.visible(true);
    _resumoCancelamento.NFS.val(dados.LancamentoNFSManual.NFS);
    _resumoCancelamento.Empresa.val(dados.LancamentoNFSManual.Empresa);
    _resumoCancelamento.LocalidadePrestacao.val(dados.LancamentoNFSManual.LocalidadePrestacao);
    _resumoCancelamento.Filial.val(dados.LancamentoNFSManual.Filial);
    _resumoCancelamento.DataCancelamento.val(dados.DataCancelamento);
    _resumoCancelamento.Situacao.val(dados.DescricaoSituacao);
    _resumoCancelamento.MensagemRejeicaoCancelamento.val(dados.MensagemRejeicaoCancelamento);

    if (dados.Situacao == EnumSituacaoMDFeManualCancelamento.CancelamentoRejeitado) {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(true)
    } else {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(false)
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        _resumoCancelamento.Filial.visible(true);
    }
}

function LimparResumoCancelamento() {
    LimparCampos(_resumoCancelamento);
    _resumoCancelamento.NFS.visible(false);
}