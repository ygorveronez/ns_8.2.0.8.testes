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

//*******MAPEAMENTO KNOUCKOUT*******

var _encerramentoMDFe;

var EncerramentoMDFe = function () {
    this.Numero = PropertyEntity({ text: "Número:" });
    this.Veiculo = PropertyEntity({ text: "Veículo:" });
    this.Motorista = PropertyEntity({ text: "Motorista:" });
    this.UFOrigem = PropertyEntity({ text: "Estado de Origem:" });
    this.UFDestino = PropertyEntity({ text: "Estado de Destino:" });
    this.Data = PropertyEntity({ text: "Data de Autorização:" });
    this.Chave = PropertyEntity({ text: "Chave:" });
    this.Protocolo = PropertyEntity({ text: "Protocolo:" });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DescricaoStatus = PropertyEntity({ text: "Situação:" });
    this.Captcha = PropertyEntity({ text: "Captcha:" });
    this.CaptchaInformado = PropertyEntity({ text: "Captcha:" });

    this.EncerrarMDFe = PropertyEntity({ eventClick: function (e) { EncerrarMDFe(); }, type: types.event, text: "Encerrar o MDF-e", idGrid: guid(), visible: ko.observable(true) });
    this.AbrirTelaEncerramentoMDFe = PropertyEntity({ eventClick: function (e) { AbrirTelaEncerramentoMDFe(); }, type: types.event, text: "Encerrar o MDF-e", idGrid: guid(), visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: function (e) { AtualizarInformacoesMDFe(); }, type: types.event, text: "Atualizar os Dados", idGrid: guid(), visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadEncerramentoTransportador() {
    _encerramentoMDFe = new EncerramentoMDFe();
    KoBindings(_encerramentoMDFe, "knockoutEncerramentoTransportador");

    AtualizarInformacoesMDFe();
}

function ObterPastaURI() {
    var pasta = document.location.href.replace('http://', '').split('/');

    var descricao = pasta[1];

    if (descricao != "MDFe" && descricao != "EncerramentoTransportador")
        return "/" + descricao + "/";
    else
        return "/";
}

function AtualizarInformacoesMDFe() {
    var parametros = /x=([^&]+)/.exec(document.location.href);


    if (parametros != null) {
        executarReST(ObterPastaURI() + "EncerramentoTransportador/ObterDetalhesMDFe", { x: parametros[1] }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_encerramentoMDFe, r);

                    if (r.Data.Status != 3) {
                        _encerramentoMDFe.AbrirTelaEncerramentoMDFe.visible(false);
                        _encerramentoMDFe.Atualizar.visible(true);
                    } else {
                        _encerramentoMDFe.AbrirTelaEncerramentoMDFe.visible(true);
                        _encerramentoMDFe.Atualizar.visible(false);
                    }

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    }
}

function AbrirTelaEncerramentoMDFe() {
    executarReST(ObterPastaURI() + "EncerramentoTransportador/ObterDadosCaptcha", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                $("#imgCaptchaEncerramento").attr("src", "data:image/jpeg;base64," + r.Data.ImagemCaptcha);
                _encerramentoMDFe.Captcha.val(r.Data.Captcha);
                _encerramentoMDFe.CaptchaInformado.val("");
                Global.abrirModal("divModalEncerramentoMDFe");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function EncerrarMDFe() {
    exibirConfirmacao("Atenção!", "Deseja realmente encerrar o MDF-e número " + _encerramentoMDFe.Numero.val() + "?", function () {
        executarReST(ObterPastaURI() + "EncerramentoTransportador/EncerrarMDFe", { MDFe: _encerramentoMDFe.Codigo.val(), Captcha: _encerramentoMDFe.Captcha.val(), CaptchaInformado: _encerramentoMDFe.CaptchaInformado.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    Global.fecharModal('divModalEncerramentoMDFe');
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "MDF-e encerrado com sucesso!");
                    AtualizarInformacoesMDFe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}


