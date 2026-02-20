/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Home.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoCertificadoHome;

var ConfiguracaoCertificado = function () {
    this.CodigoEmpresa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PossuiCertificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Possui Certificado" });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Certificado Digital A1:", val: ko.observable(""), visible: ko.observable(false), required: true });
    this.DataInicialCertificado = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.DataFinalCertificado = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false), getType: typesKnockout.date });
    this.SerieCertificado = PropertyEntity({ text: "Série: ", val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(false) });
    this.SenhaCertificado = PropertyEntity({ text: "*Senha: ", val: ko.observable(""), visible: ko.observable(false), required: true });

    this.Enviar = PropertyEntity({ eventClick: enviarCertificadoClick, type: types.event, text: "Enviar", visible: ko.observable(false) });
    this.Remover = PropertyEntity({ eventClick: removerCertificadoClick, type: types.event, text: "Remover", visible: ko.observable(false) });
    this.Baixar = PropertyEntity({ eventClick: baixarCertificadoClick, type: types.event, text: "Baixar", visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadConfiguracaoCertificado() {
    _configuracaoCertificadoHome = new ConfiguracaoCertificado();
    KoBindings(_configuracaoCertificadoHome, "knockoutConfiguracaoCertificadoHome");

    executarReST("Home/CarregaDadosCertificadoDigital", null, function (arg) {
        if (arg.Success) {
            if (arg.Data != null && arg.Data) {
                var dadosCertificado = arg.Data;

                _configuracaoCertificadoHome.CodigoEmpresa.val(dadosCertificado.Codigo);
                _configuracaoCertificadoHome.PossuiCertificado.val(dadosCertificado.PossuiCertificado);
                if (!_configuracaoCertificadoHome.PossuiCertificado.val()) {
                    _configuracaoCertificadoHome.Arquivo.visible(true);
                    _configuracaoCertificadoHome.SenhaCertificado.visible(true);
                    _configuracaoCertificadoHome.Enviar.visible(true);
                } else {
                    _configuracaoCertificadoHome.Baixar.visible(true);
                    _configuracaoCertificadoHome.Remover.visible(true);
                    _configuracaoCertificadoHome.DataFinalCertificado.visible(true);
                    _configuracaoCertificadoHome.DataInicialCertificado.visible(true);
                    _configuracaoCertificadoHome.SerieCertificado.visible(true);
                    _configuracaoCertificadoHome.DataFinalCertificado.val(dadosCertificado.DataFinalCertificado);
                    _configuracaoCertificadoHome.DataInicialCertificado.val(dadosCertificado.DataInicialCertificado);
                    _configuracaoCertificadoHome.SerieCertificado.val(dadosCertificado.SerieCertificado);
                }
                Global.abrirModal('divModalConfigCertificadoDigital');
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
        }
    });
}

function baixarCertificadoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente baixar o certificado da sua empresa?", function () {
        executarDownload("Transportador/DownloadCertificado", { CodigoTransportador: _configuracaoCertificadoHome.CodigoEmpresa.val() });
    });
}

function removerCertificadoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o certificado da sua empresa? <b>Este processo é irreversível!</b>", function () {
        executarReST("Transportador/RemoverCertificado", { CodigoTransportador: _configuracaoCertificadoHome.CodigoEmpresa.val() }, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Certificado removido com sucesso.");
                _configuracaoCertificadoHome.Baixar.visible(false);
                _configuracaoCertificadoHome.Remover.visible(false);
                _configuracaoCertificadoHome.DataFinalCertificado.visible(false);
                _configuracaoCertificadoHome.DataInicialCertificado.visible(false);
                _configuracaoCertificadoHome.SerieCertificado.visible(false);
                _configuracaoCertificadoHome.DataFinalCertificado.val("");
                _configuracaoCertificadoHome.DataInicialCertificado.val("");
                _configuracaoCertificadoHome.SerieCertificado.val("");
                _configuracaoCertificadoHome.PossuiCertificado.val(false);

                _configuracaoCertificadoHome.SenhaCertificado.val("");
                _configuracaoCertificadoHome.Arquivo.visible(true);
                _configuracaoCertificadoHome.SenhaCertificado.visible(true);
                _configuracaoCertificadoHome.Enviar.visible(true);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function enviarCertificadoClick() {
    var valido = ValidarCamposObrigatorios(_configuracaoCertificadoHome);
    var file = document.getElementById(_configuracaoCertificadoHome.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var formData = new FormData();
        formData.append("upload", file.files[0]);

        enviarArquivo("Transportador/EnviarCertificado?callback=?",
            {
                CodigoTransportador: _configuracaoCertificadoHome.CodigoEmpresa.val(),
                SenhaCertificado: _configuracaoCertificadoHome.SenhaCertificado.val(),
                SerieCertificado: _configuracaoCertificadoHome.SerieCertificado.val(),
                DataInicialCertificado: _configuracaoCertificadoHome.DataInicialCertificado.val(),
                DataFinalCertificado: _configuracaoCertificadoHome.DataFinalCertificado.val()
            }, formData, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Certificado validado e salvo com sucesso.");

                    _configuracaoCertificadoHome.DataInicialCertificado.val(arg.Data.DataInicialCertificado);
                    _configuracaoCertificadoHome.DataFinalCertificado.val(arg.Data.DataFinalCertificado);
                    _configuracaoCertificadoHome.SerieCertificado.val(arg.Data.SerieCertificado);
                    _configuracaoCertificadoHome.SenhaCertificado.val(arg.Data.SenhaCertificado);

                    _configuracaoCertificadoHome.Baixar.visible(true);
                    _configuracaoCertificadoHome.Remover.visible(true);
                    _configuracaoCertificadoHome.DataFinalCertificado.visible(true);
                    _configuracaoCertificadoHome.DataInicialCertificado.visible(true);
                    _configuracaoCertificadoHome.SerieCertificado.visible(true);

                    _configuracaoCertificadoHome.Arquivo.val("");
                    _configuracaoCertificadoHome.Arquivo.visible(false);
                    _configuracaoCertificadoHome.SenhaCertificado.visible(false);
                    _configuracaoCertificadoHome.Enviar.visible(false);

                    _configuracaoCertificadoHome.PossuiCertificado.val(true);

                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}