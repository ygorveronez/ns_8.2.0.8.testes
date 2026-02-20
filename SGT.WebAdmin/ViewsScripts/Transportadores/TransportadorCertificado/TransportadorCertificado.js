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

//*******MAPEAMENTO KNOUCKOUT*******

var _transportadorCertificado;

var TransportadorCertificado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.CertificadoDigital.getRequiredFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataInicialCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataInicial.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.DataFinalCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataFinal.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.SerieCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Serie.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false) });
    this.SenhaCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Senha.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.PossuiCertificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.Enviar = PropertyEntity({ eventClick: enviarTransportadorCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Enviar, visible: ko.observable(false) });
    this.Remover = PropertyEntity({ eventClick: removerTransportadorCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Remover, visible: ko.observable(false) });
    this.Baixar = PropertyEntity({ eventClick: baixarTransportadorCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Baixar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTransportadorCertificado() {
    _transportadorCertificado = new TransportadorCertificado();
    KoBindings(_transportadorCertificado, "knockoutTransportadorCertificado");

    BuscarDadosCertificadoTransportador();
}

function estadoTransportadorCertificado() {
    if (_transportadorCertificado.PossuiCertificado.val()) {
        _transportadorCertificado.Baixar.visible(true);
        _transportadorCertificado.Remover.visible(true);
        _transportadorCertificado.DataFinalCertificado.visible(true);
        _transportadorCertificado.DataInicialCertificado.visible(true);
        _transportadorCertificado.SerieCertificado.visible(true);

        _transportadorCertificado.Arquivo.visible(false);
        _transportadorCertificado.SenhaCertificado.visible(false);
        _transportadorCertificado.Enviar.visible(false);
    } else {
        _transportadorCertificado.Arquivo.visible(true);
        _transportadorCertificado.SenhaCertificado.visible(true);
        _transportadorCertificado.Enviar.visible(true);

        _transportadorCertificado.Baixar.visible(false);
        _transportadorCertificado.Remover.visible(false);
        _transportadorCertificado.DataFinalCertificado.visible(false);
        _transportadorCertificado.DataInicialCertificado.visible(false);
        _transportadorCertificado.SerieCertificado.visible(false);
        _transportadorCertificado.DataFinalCertificado.val("");
        _transportadorCertificado.DataInicialCertificado.val("");
        _transportadorCertificado.SerieCertificado.val("");
        _transportadorCertificado.SenhaCertificado.val("");
    }
}

function baixarTransportadorCertificadoClick() {
    executarDownload("TransportadorCertificado/DownloadCertificado");
}

function removerTransportadorCertificadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.DesejaRealmenteRemoverCertificadoDesteTransportador + "<b>" + Localization.Resources.Transportadores.Transportador.EsteProcessoIrreversivel + "</b>", function () {
        executarReST("TransportadorCertificado/RemoverCertificado", null, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.CertificadoRemovidoComSucesso);
                _transportadorCertificado.Baixar.visible(false);
                _transportadorCertificado.Remover.visible(false);
                _transportadorCertificado.DataFinalCertificado.visible(false);
                _transportadorCertificado.DataInicialCertificado.visible(false);
                _transportadorCertificado.SerieCertificado.visible(false);
                _transportadorCertificado.DataFinalCertificado.val("");
                _transportadorCertificado.DataInicialCertificado.val("");
                _transportadorCertificado.SerieCertificado.val("");

                _transportadorCertificado.SenhaCertificado.val("");
                _transportadorCertificado.Arquivo.visible(true);
                _transportadorCertificado.SenhaCertificado.visible(true);
                _transportadorCertificado.Enviar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function enviarTransportadorCertificadoClick() {
    var file = document.getElementById(_transportadorCertificado.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("TransportadorCertificado/EnviarCertificado?callback=?",
        {
            SenhaCertificado: _transportadorCertificado.SenhaCertificado.val(),
            SerieCertificado: _transportadorCertificado.SerieCertificado.val(),
            DataInicialCertificado: _transportadorCertificado.DataInicialCertificado.val(),
            DataFinalCertificado: _transportadorCertificado.DataFinalCertificado.val()
        }, formData, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.CertificadoValidadoSalvoCucesso);

                _transportadorCertificado.DataInicialCertificado.val(arg.Data.DataInicialCertificado);
                _transportadorCertificado.DataFinalCertificado.val(arg.Data.DataFinalCertificado);
                _transportadorCertificado.SerieCertificado.val(arg.Data.SerieCertificado);
                _transportadorCertificado.SenhaCertificado.val(arg.Data.SenhaCertificado);

                _transportadorCertificado.Baixar.visible(true);
                _transportadorCertificado.Remover.visible(true);
                _transportadorCertificado.DataFinalCertificado.visible(true);
                _transportadorCertificado.DataInicialCertificado.visible(true);
                _transportadorCertificado.SerieCertificado.visible(true);

                _transportadorCertificado.Arquivo.val("");
                _transportadorCertificado.Arquivo.visible(false);
                _transportadorCertificado.SenhaCertificado.visible(false);
                _transportadorCertificado.Enviar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
}

function BuscarDadosCertificadoTransportador() {
    BuscarPorCodigo(_transportadorCertificado, "TransportadorCertificado/BuscarPorCodigo", function () {
        estadoTransportadorCertificado();
    });
}