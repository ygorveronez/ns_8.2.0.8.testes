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
    this.MDFe = PropertyEntity({ text: "MDF-e: ", visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ text: ko.observable("Transportador: "), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veiculo: " });
    this.Motorista = PropertyEntity({ text: "Motorista: " });
    this.Origem = PropertyEntity({ text: "Origem: " });
    this.Destino = PropertyEntity({ text: "Destino: " });
    this.DataCancelamento = PropertyEntity({ text: "Data do Cancelamento: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.MensagemRejeicaoCancelamento = PropertyEntity({ text: "Motivo Rejeição: ", visible: ko.observable(false) });

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
    
    _resumoCancelamento.MDFe.visible(true);
    _resumoCancelamento.MDFe.val(dados.CargaMDFeManual.MDFe);
    _resumoCancelamento.Empresa.val(dados.CargaMDFeManual.Empresa);
    _resumoCancelamento.Origem.val(dados.CargaMDFeManual.Origem);
    _resumoCancelamento.Veiculo.val(dados.CargaMDFeManual.Veiculo);
    _resumoCancelamento.Motorista.val(dados.CargaMDFeManual.Motorista);
    _resumoCancelamento.Destino.val(dados.CargaMDFeManual.Destino);
    _resumoCancelamento.DataCancelamento.val(dados.DataCancelamento);
    _resumoCancelamento.Situacao.val(dados.DescricaoSituacao);

    _resumoCancelamento.MensagemRejeicaoCancelamento.val(dados.MensagemRejeicaoCancelamento);
    if (dados.Situacao == EnumSituacaoMDFeManualCancelamento.CancelamentoRejeitado) {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(true)
    } else {
        _resumoCancelamento.MensagemRejeicaoCancelamento.visible(false)
    }
}

function LimparResumoCancelamento() {
    LimparCampos(_resumoCancelamento);
    _resumoCancelamento.MDFe.visible(false);
}