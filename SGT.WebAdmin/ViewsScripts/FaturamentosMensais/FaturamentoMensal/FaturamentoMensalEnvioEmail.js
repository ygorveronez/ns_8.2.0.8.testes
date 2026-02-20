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
/// <reference path="FaturamentoMensalEtapa.js" />
/// <reference path="FaturamentoMensalDocumento.js" />
/// <reference path="FaturamentoMensal.js" />
/// <reference path="FaturamentoMensalBoleto.js" />
/// <reference path="../../Enumeradores/EnumEtapaFaturamentoMensal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _envioEmail;
var _gridEnvioEmail;

var EnvioEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaFaturamentoMensal.Etapa4), def: EnumEtapaFaturamentoMensal.Etapa4, getType: typesKnockout.int });

    this.FaturamentoParaEnvioEmail = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(false), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Finalizar = PropertyEntity({ eventClick: FinalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadEnvioEmail() {
    _envioEmail = new EnvioEmail();
    KoBindings(_envioEmail, "knockoutEnvioEmail");
}

function AtualizarProgressEmails(percentual, codigoFaturamentoMensal) {
    if (_envioEmail.Codigo.val() == codigoFaturamentoMensal) {
        SetarPercentualProcessamentoEmails(percentual);

        $("#fdsEmails").hide();
        $("#fdsPercentualEmails").show();

        _envioEmail.Finalizar.enable(false);
    }
}

function SetarPercentualProcessamentoEmails(percentual) {
    finalizarRequisicao();
    var strPercentual = parseInt(percentual) + "%";
    _envioEmail.PercentualProcessado.val(strPercentual);
    $("#" + _envioEmail.PercentualProcessado.id).css("width", strPercentual)
}

function VerificarSeEmailsNotificadaEstaSelecionada(codigoFaturamentoMensal) {
    if (_envioEmail.Codigo.val() == codigoFaturamentoMensal) {

        $("#fdsEmails").show();
        $("#fdsPercentualEmails").hide();

        _envioEmail.Finalizar.enable(true);

        SetarPercentualProcessamentoEmails(0);

        buscarTitulosParaEnvioEmail();
    }
}

function FinalizarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de geração do faturamento mensal?", function () {

        var data = { Codigo: _geracaoBoletos.Codigo.val() };
        executarReST("FaturamentoMensal/FinalizarFaturamentoMensal", data, function (arg) {
            if (arg.Success) {
                _selecaoFaturamento.Status.val(EnumStatusFaturamentoMensal.Finalizado);
                limparCamposFaturamentoMensal();
                buscarFaturamentoMensal();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo concluído com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    });
}

function EnviarEmailClick(e, sender) {
    if (_gridEnvioEmail == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Envio de E-mail", "Por favor selecione ao menos um faturamento para enviar os e-mail.");
        return;
    }
    if (_envioEmail.Codigo.val() > 0) {

        SetarPercentualProcessamentoEmails(0);
        AtualizarProgressEmails(0, _envioEmail.Codigo.val());

        var data = { Codigo: _envioEmail.Codigo.val() };
        executarReST("FaturamentoMensal/EnviarEmailFaturamento", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Processo de envio de e-mail iniciado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    }

}

//*******MÉTODOS*******

function buscarTitulosParaEnvioEmail() {
    if (_envioEmail.Codigo.val() > 0) {

        _envioEmail.EnviarEmail.visible(true);
        _envioEmail.SelecionarTodos.visible(false);
        _envioEmail.SelecionarTodos.val(false);

        _gridEnvioEmail = new GridView(_envioEmail.FaturamentoParaEnvioEmail.idGrid, "FaturamentoMensal/PesquisaFaturamentoMensal", _envioEmail, null, null, 50, null, null, null, null);
        _gridEnvioEmail.CarregarGrid();

    } else
        $("#knockoutEnvioEmail").hide();

}

function limparCamposEnvioEmail() {
    LimparCampos(_envioEmail);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}