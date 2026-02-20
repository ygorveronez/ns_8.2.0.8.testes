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
/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _certificado;

var Certificado = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.CertificadoDigital.getRequiredFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataInicialCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DataInicial.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.DataFinalCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DataFinal.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.SerieCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.Serie.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false) });
    this.SenhaCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.Senha.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    this.Enviar = PropertyEntity({ eventClick: enviarCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Enviar, visible: ko.observable(false) });
    this.Remover = PropertyEntity({ eventClick: removerCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Remover, visible: ko.observable(false) });
    this.Baixar = PropertyEntity({ eventClick: baixarCertificadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Baixar, visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadCertificado() {

    _certificado = new Certificado();
    KoBindings(_certificado, "knockoutCadastroCertificado");

    if (!_filial.PossuiCertificado.val()) {
        _certificado.Arquivo.visible(true);
        _certificado.SenhaCertificado.visible(true);
        _certificado.Enviar.visible(true);
    } else {
        _certificado.Baixar.visible(true);
        _certificado.Remover.visible(true);
        _certificado.DataFinalCertificado.visible(true);
        _certificado.DataInicialCertificado.visible(true);
        _certificado.SerieCertificado.visible(true);
        _certificado.DataFinalCertificado.val(_filial.DataFinalCertificado.val());
        _certificado.DataInicialCertificado.val(_filial.DataInicialCertificado.val());
        _certificado.SerieCertificado.val(_filial.SerieCertificado.val());
    }
}

function baixarCertificadoClick() {
    executarDownload("Filial/DownloadCertificado", { CodigoFilial: _filial.Codigo.val() });
}

function removerCertificadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Filiais.Filial.DesejaRemoverCertificadoDestaFilial, function () {
        executarReST("Filial/RemoverCertificado", { CodigoFilial: _filial.Codigo.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _certificado.Baixar.visible(false);
                _certificado.Remover.visible(false);
                _certificado.DataFinalCertificado.visible(false);
                _certificado.DataInicialCertificado.visible(false);
                _certificado.SerieCertificado.visible(false);
                _certificado.DataFinalCertificado.val("");
                _certificado.DataInicialCertificado.val("");
                _certificado.SerieCertificado.val("");
                _filial.PossuiCertificado.val(false);

                _certificado.SenhaCertificado.val("");
                _certificado.Arquivo.visible(true);
                _certificado.SenhaCertificado.visible(true);
                _certificado.Enviar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function enviarCertificadoClick() {
    var file = document.getElementById(_certificado.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);

    enviarArquivo("Filial/EnviarCertificado?callback=?",
        {
            CodigoFilial: _filial.Codigo.val(),
            SenhaCertificado: _certificado.SenhaCertificado.val(),
            SerieCertificado: _certificado.SerieCertificado.val(),
            DataInicialCertificado: _certificado.DataInicialCertificado.val(),
            DataFinalCertificado: _certificado.DataFinalCertificado.val()
        }, formData, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                _certificado.DataInicialCertificado.val(arg.Data.DataInicialCertificado);
                _certificado.DataFinalCertificado.val(arg.Data.DataFinalCertificado);
                _certificado.SerieCertificado.val(arg.Data.SerieCertificado);
                _certificado.SenhaCertificado.val(arg.Data.SenhaCertificado);

                _certificado.Baixar.visible(true);
                _certificado.Remover.visible(true);
                _certificado.DataFinalCertificado.visible(true);
                _certificado.DataInicialCertificado.visible(true);
                _certificado.SerieCertificado.visible(true);

                _certificado.Arquivo.val("");
                _certificado.Arquivo.visible(false);
                _certificado.SenhaCertificado.visible(false);
                _certificado.Enviar.visible(false);

                _filial.PossuiCertificado.val(true);

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
}

function alterarEstadoCadastroCertificado() {
    if (_filial.Codigo.val() > 0) {

        loadCertificado();

        $("#liTabCertificado").removeClass("d-none");

    } else {
        $("#liTabCertificado").addClass("d-none");
    }
}