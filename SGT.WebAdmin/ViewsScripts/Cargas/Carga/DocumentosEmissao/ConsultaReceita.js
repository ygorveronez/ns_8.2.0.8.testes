/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../../../../js/plugin/dropzone/dropzone-amd-module.min.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="CargaPedidoDocumentoCTe.js" />
/// <reference path="CTe.js" />
/// <reference path="Documentos.js" />
/// <reference path="DropZone.js" />
/// <reference path="EtapaDocumentos.js" />
/// <reference path="NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoConsultaPortalFazenda.js" />
/// <reference path="../../../Enumeradores/EnumPermissoesEdicaoCTe.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoMinutaAvon.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _captcha;
var captchaClicado = false; //criardo essa flag por um bug no teclado de alguns PC onde ficava disparando varios clique e chamando varias vezes o metodo de consulta.


var CaptchaReceita = function () {

    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Chave = PropertyEntity({ val: ko.observable(""), def: "" });
    this.VIEWSTATE = PropertyEntity({ val: ko.observable(""), def: "" });
    this.EVENTVALIDATION = PropertyEntity({ val: ko.observable(""), def: "" });
    this.captchaSom = PropertyEntity({ val: ko.observable(""), def: "" });
    this.token = PropertyEntity({ val: ko.observable(""), def: "" });
    this.SessionID = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Captcha = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DigiteCodigoDaImagemAoLado.getFieldDescription(), src: ko.observable(""), maxlength: 6, required: true, visible : true });

    this.TipoConsultaPortalFazenda = PropertyEntity({ val: ko.observable(EnumTipoConsultaPortalFazenda.NFe), def: EnumTipoConsultaPortalFazenda.NFe });

    this.ReCaptcha = PropertyEntity({ src: ko.observable(""), maxlength: 6, required: false, visible: false  });

    this.Continuar = PropertyEntity({ eventClick: continuarCaptchaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Continuar, visible: ko.observable(true) });
    this.BuscarNovoCaptcha = PropertyEntity({ eventClick: verificarReceitaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Continuar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadConsultaReceita() {

    //$("#" + _documentoEmissao.Chave.id).keypress(function (e) {
    //    var keyCode = e.keyCode || e.which;
    //    if (keyCode == 13) {
    //        $('#' + _documentoEmissao.Chave.idBtnSearch).trigger('click');
    //    }
    //});
    _documentoEmissao.Chave.get$()
        .on("change", ChangeChaveDocumentoEmissao)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == 13) ChangeChaveDocumentoEmissao();
        });

    _documentoEmissao.MultiplaChave.get$()
        .on("change", ChangeMultiplaChaveDocumentoEmissao)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == 13) ChangeMultiplaChaveDocumentoEmissao();
        });

    $("#divModalInformaCaptchaReceita").keypress(function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 13) {
            $('#' + _captcha.Continuar.id).trigger('click');
        }
    });

    _captcha = new CaptchaReceita();
    KoBindings(_captcha, "knoutCaptchaReceita", _captcha.Continuar.id);

    $('#divModalInformaCaptchaReceita').on('shown.bs.modal', function () {
        _captcha.Captcha.get$().focus();
    });

    $('#divModalInformaCaptchaReceita').on('hidden.bs.modal', function () {
        _documentoEmissao.Chave.get$().focus();
    });
}

function ChangeMultiplaChaveDocumentoEmissao() {
    setTimeout(function () {
        if (_documentoEmissao.MultiplaChave.val() != "")
            $('#' + _documentoEmissao.MultiplaChave.idBtnSearch).trigger('click');
    }, 100);
}

function ChangeChaveDocumentoEmissao() {
    setTimeout(function () {
        if (_documentoEmissao.Chave.val() != "")
            $('#' + _documentoEmissao.Chave.idBtnSearch).trigger('click');
    }, 100);
}

function continuarCaptchaClick(e, sender) {
    if (!captchaClicado) {
        captchaClicado = true;
        if (!ValidarCamposObrigatorios(e))
            captchaClicado = false;
        Salvar(e, "ConsultaDocumentosReceita/InformarCaptchaReceita", function (arg) {
            captchaClicado = false;
            if (arg.Success) {
                if (arg.Data != false) {
                    veririficarSeCargaMudouTipoContratacao(arg.Data);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                    carregarGridDocumentosParaEmissao();

                    limparCamposDocumentosParaEmissao();

                    Global.fecharModal("divModalInformaCaptchaReceita");

                    _captcha.Captcha.val("");
                    _captcha.Captcha.src("");

                    $("#" + _documentoEmissao.Chave.id).focus();

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
                    _captcha.Captcha.val("");
                    verificarReceitaClick();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    }
}

function verificarReceitaClick(e, sender) {
    if (ValidarCampoObrigatorioMap(_documentoEmissao.Chave)) {
        if (_documentoEmissao.Chave.val().length >= 44) {
            _captcha.Captcha.val("");
            var data = { Chave: _documentoEmissao.Chave.val(), CargaPedido: _documentoEmissao.CargaPedido.val() };
            executarReST("ConsultaDocumentosReceita/ConsultarReceita", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data.NotaAdicionada == true) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                        carregarGridDocumentosParaEmissao();
                        limparCamposDocumentosParaEmissao();
                    } else {
                        if (arg.Data.DadosConsultar != null) {
                            _captcha.Captcha.src(arg.Data.DadosConsultar.imgCaptcha);
                            _captcha.VIEWSTATE.val(arg.Data.DadosConsultar.VIEWSTATE);
                            _captcha.EVENTVALIDATION.val(arg.Data.DadosConsultar.EVENTVALIDATION);
                            _captcha.captchaSom.val(arg.Data.DadosConsultar.captchaSom);
                            _captcha.token.val(arg.Data.DadosConsultar.token);
                            _captcha.Chave.val(_documentoEmissao.Chave.val());
                            _captcha.CargaPedido.val(_documentoEmissao.CargaPedido.val());
                            _captcha.SessionID.val(arg.Data.DadosConsultar.SessionID);
                            _captcha.TipoConsultaPortalFazenda.val(arg.Data.TipoConsultaPortalFazenda);
                            if (_captcha.TipoConsultaPortalFazenda.val() == EnumTipoConsultaPortalFazenda.NFe)
                                $("#myModalLabel_TipoConsulta").text(Localization.Resources.Cargas.Carga.ConsultarNFePelaChave);
                            else
                                $("#myModalLabel_TipoConsulta").text(Localization.Resources.Cargas.Carga.ConsultarCTePelaChave);

                            Global.abrirModal("divModalInformaCaptchaReceita");
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                        }
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.ChaveInvalida, Localization.Resources.Cargas.Carga.PorFavorInformeUmaChaveValidaParaConsulta);
        }
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.ChaveObrigatoria);
    }
}


//function verificarReceitaClick(e, sender) {
//    if (ValidarCampoObrigatorioMap(_documentoEmissao.Chave)) {
//        if (_documentoEmissao.Chave.val().length >= 44) {
//            _captcha.Captcha.val("");
//            var data = { Chave: _documentoEmissao.Chave.val(), CargaPedido: _documentoEmissao.CargaPedido.val() };
//            executarReST("ConsultaDocumentosReceita/ConsultarReceita", data, function (arg) {
//                if (arg.Success) {
//                    if (arg.Data.NotaAdicionada == true) {
//                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
//                        carregarGridDocumentosParaEmissao();
//                        limparCamposDocumentosParaEmissao();
//                    } else {
//                        if (arg.Data.DadosConsultar != null) {
//                            //_captcha.Captcha.src(arg.Data.DadosConsultar.imgCaptcha);
//                            _captcha.VIEWSTATE.val(arg.Data.DadosConsultar.VIEWSTATE);
//                            _captcha.EVENTVALIDATION.val(arg.Data.DadosConsultar.EVENTVALIDATION);
//                            //_captcha.captchaSom.val(arg.Data.DadosConsultar.captchaSom);
//                            //_captcha.token.val(arg.Data.DadosConsultar.token);
//                            _captcha.Chave.val(_documentoEmissao.Chave.val());
//                            _captcha.CargaPedido.val(_documentoEmissao.CargaPedido.val());
//                            _captcha.SessionID.val(arg.Data.DadosConsultar.SessionID);
//                            _captcha.TipoConsultaPortalFazenda.val(arg.Data.TipoConsultaPortalFazenda);

//                            var tes = '<div class="g-recaptcha" style="transform:scale(1.07);transform-origin:0 0" data-sitekey="' + arg.Data.DadosConsultar.token + '"></div>'

//                            $("#reca").html(tes);
//                            $("#" + _captcha.ReCaptcha.id).html(arg.Data.DadosConsultar.imgCaptcha);

//                            if (_captcha.TipoConsultaPortalFazenda.val() == EnumTipoConsultaPortalFazenda.NFe)
//                                $("#myModalLabel_TipoConsulta").text("Consultar NF-e pela Chave");
//                            else
//                                $("#myModalLabel_TipoConsulta").text("Consultar CT-e pela Chave");

//                            $("#divModalInformaCaptchaReceita").modal({ keyboard: true, backdrop: 'static' });
//                        } else {
//                            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
//                        }
//                    }
//                } else {
//                    exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
//                }
//            });
//        } else {
//            exibirMensagem(tipoMensagem.aviso, "Chave inválida", "Por favor, informe uma chave válida para a consulta");
//        }
//    } else {
//        exibirMensagem(tipoMensagem.aviso, "Campo obrigatório", "A chave é obrigatória");
//    }
//}