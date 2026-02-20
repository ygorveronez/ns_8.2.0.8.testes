var remetente = false;
var destinatario = false;
var tomador = false;
var expedidor = false;
var recebedor = false;

$(document).ready(function () {
    $("#btnConsultarRemetenteReceita").click(function () {
        remetente = true;
        destinatario = false;
        tomador = false;
        expedidor = false;
        recebedor = false;
        AbrirConsultaReceita();
    });

    $("#btnConsultarDestinatarioReceita").click(function () {
        remetente = false;
        destinatario = true;
        tomador = false;
        expedidor = false;
        recebedor = false;
        AbrirConsultaReceita();
    });

    $("#btnConsultarTomadorReceita").click(function () {
        remetente = false;
        destinatario = false;
        tomador = true;
        expedidor = false;
        recebedor = false;
        AbrirConsultaReceita();
    });

    $("#btnConsultarExpedidorReceita").click(function () {
        remetente = false;
        destinatario = false;
        tomador = false;
        expedidor = true;
        recebedor = false;
        AbrirConsultaReceita();
    });

    $("#btnConsultarRecebedorReceita").click(function () {
        remetente = false;
        destinatario = false;
        tomador = false;
        expedidor = false;
        recebedor = true;
        AbrirConsultaReceita();
    });

    $("#btnCaptchaReceita").click(function () {
        ConsultarDadosReceita();
    });

    $("#btnAtualizarCaptchaReceita").click(function () {
        AbrirConsultaReceita();
    });
});

function AbrirConsultaReceita() {
    var cnpj = "";
    if (remetente)
        cnpj = $("#txtCPFCNPJRemetente").val().replace(/[^0-9]/g, '');
    else if (destinatario)
        cnpj = $("#txtCPFCNPJDestinatario").val().replace(/[^0-9]/g, '');
    else if (tomador)
        cnpj = $("#txtCPFCNPJTomador").val().replace(/[^0-9]/g, '');
    else if (expedidor)
        cnpj = $("#txtCPFCNPJExpedidor").val().replace(/[^0-9]/g, '');
    else if (recebedor)
        cnpj = $("#txtCPFCNPJRecebedor").val().replace(/[^0-9]/g, '');

    executarRest("/Cliente/ConsultarClienteReceita?callback=?", { CNPJ: cnpj }, function (r) {
        if (r.Sucesso) {

            $('#imgCaptchaReceita').prop('src', r.Objeto.chaptcha);
            cookies = r.Objeto.Cookies;

            $("#divCaptchaReceita")
                .modal({ keyboard: false, backdrop: 'static' })
                .on("shown.bs.modal", function () {
                    // Foca o input do CAPTCHA quando abrir o modal
                    $("#txtCaptchaReceita").val("").focus();
                });

        } else {
            jAlert(r.Erro, "Consulta de Cliente Receita");
        }
    });
}

function ConsultarDadosReceita() {

    var cnpj = "";
    if (remetente)
        cnpj = $("#txtCPFCNPJRemetente").val().replace(/[^0-9]/g, '');
    else if (destinatario)
        cnpj = $("#txtCPFCNPJDestinatario").val().replace(/[^0-9]/g, '');
    else if (tomador)
        cnpj = $("#txtCPFCNPJTomador").val().replace(/[^0-9]/g, '');
    else if (expedidor)
        cnpj = $("#txtCPFCNPJExpedidor").val().replace(/[^0-9]/g, '');
    else if (recebedor)
        cnpj = $("#txtCPFCNPJRecebedor").val().replace(/[^0-9]/g, '');

    executarRest("/Cliente/InformarCaptchaReceita?callback=?", { CNPJ: cnpj, Captcha: $("#txtCaptchaReceita").val(), Cookies: JSON.stringify(cookies) }, function (r) {
        if (r.Sucesso) {

            if (remetente)
            {
                $("#txtRazaoSocialRemetente").val(r.Objeto.Nome);
                if (r.Objeto.Fantasia != "")
                    $("#txtNomeFantasiaRemetente").val(r.Objeto.Fantasia);
                $("#txtCEPRemetente").val(r.Objeto.CEP);
                $("#txtEnderecoRemetente").val(r.Objeto.Endereco);
                $("#txtComplementoRemetente").val(r.Objeto.Complemento);
                $("#txtNumeroRemetente").val(r.Objeto.Numero);
                $("#txtBairroRemetente").val(r.Objeto.Bairro);
                if (r.Objeto.TelefonePrincipal != "")
                    $("#txtTelefone1Remetente").val(r.Objeto.TelefonePrincipal).change();

                if (r.Objeto.Localidade != null) {
                    // Coloca o estado no ATUACAO na aba Remetente
                    $("#ddlEstadoRemetente").val(r.Objeto.Localidade.UF);

                    // Coloca o estado no INICIO DA PRESTACAO na aba Dados
                    $("#ddlUFInicioPrestacao").val(r.Objeto.Localidade.UF).change();
                    
                    BuscarLocalidades($("#ddlEstadoRemetente").val(), 'selCidadeRemetente', r.Objeto.Localidade.Codigo);
                }
            }
            else if (destinatario)
            {
                $("#txtRazaoSocialDestinatario").val(r.Objeto.Nome);
                if (r.Objeto.Fantasia != "")
                    $("#txtNomeFantasiaDestinatario").val(r.Objeto.Fantasia);
                $("#txtCEPDestinatario").val(r.Objeto.CEP);
                $("#txtEnderecoDestinatario").val(r.Objeto.Endereco);
                $("#txtComplementoDestinatario").val(r.Objeto.Complemento);
                $("#txtNumeroDestinatario").val(r.Objeto.Numero);
                $("#txtBairroDestinatario").val(r.Objeto.Bairro);
                if (r.Objeto.TelefonePrincipal != "")
                    $("#txtTelefone1Destinatario").val(r.Objeto.TelefonePrincipal).change();

                if (r.Objeto.Localidade != null) {
                    // Coloca o estado no ATUACAO na aba Remetente
                    $("#ddlEstadoDestinatario").val(r.Objeto.Localidade.UF);

                    // Coloca o estado no INICIO DA PRESTACAO na aba Dados
                    $("#ddlUFTerminoPrestacao").val(r.Objeto.Localidade.UF).change();

                    BuscarLocalidades($("#ddlEstadoDestinatario").val(), 'selCidadeDestinatario', r.Objeto.Localidade.Codigo);
                }
            }
            else if (tomador) {
                $("#txtRazaoSocialTomador").val(r.Objeto.Nome);
                if (r.Objeto.Fantasia != "")
                    $("#txtNomeFantasiaTomador").val(r.Objeto.Fantasia);
                $("#txtCEPTomador").val(r.Objeto.CEP);
                $("#txtEnderecoTomador").val(r.Objeto.Endereco);
                $("#txtComplementoTomador").val(r.Objeto.Complemento);
                $("#txtNumeroTomador").val(r.Objeto.Numero);
                $("#txtBairroTomador").val(r.Objeto.Bairro);
                if (r.Objeto.TelefonePrincipal != "")
                    $("#txtTelefone1Tomador").val(r.Objeto.TelefonePrincipal).change();

                if (r.Objeto.Localidade != null) {
                    $("#ddlEstadoTomador").val(r.Objeto.Localidade.UF);
                    BuscarLocalidades($("#ddlEstadoTomador").val(), 'selCidadeTomador', r.Objeto.Localidade.Codigo);
                }
            }
            else if (expedidor) {
                $("#txtRazaoSocialExpedidor").val(r.Objeto.Nome);
                if (r.Objeto.Fantasia != "")
                    $("#txtNomeFantasiaExpedidor").val(r.Objeto.Fantasia);
                $("#txtCEPExpedidor").val(r.Objeto.CEP);
                $("#txtEnderecoExpedidor").val(r.Objeto.Endereco);
                $("#txtComplementoExpedidor").val(r.Objeto.Complemento);
                $("#txtNumeroExpedidor").val(r.Objeto.Numero);
                $("#txtBairroExpedidor").val(r.Objeto.Bairro);
                if (r.Objeto.TelefonePrincipal != "")
                    $("#txtTelefone1Expedidor").val(r.Objeto.TelefonePrincipal).change();

                if (r.Objeto.Localidade != null) {
                    $("#ddlEstadoExpedidor").val(r.Objeto.Localidade.UF);
                    BuscarLocalidades($("#ddlEstadoExpedidor").val(), 'selCidadeExpedidor', r.Objeto.Localidade.Codigo);
                }
            }
            else if (recebedor) {
                $("#txtRazaoSocialRecebedor").val(r.Objeto.Nome);
                if (r.Objeto.Fantasia != "")
                    $("#txtNomeFantasiaRecebedor").val(r.Objeto.Fantasia);
                $("#txtCEPRecebedor").val(r.Objeto.CEP);
                $("#txtEnderecoRecebedor").val(r.Objeto.Endereco);
                $("#txtComplementoRecebedor").val(r.Objeto.Complemento);
                $("#txtNumeroRecebedor").val(r.Objeto.Numero);
                $("#txtBairroRecebedor").val(r.Objeto.Bairro);
                if (r.Objeto.TelefonePrincipal != "")
                    $("#txtTelefone1Recebedor").val(r.Objeto.TelefonePrincipal).change();

                if (r.Objeto.Localidade != null) {
                    $("#ddlEstadoRecebedor").val(r.Objeto.Localidade.UF);
                    BuscarLocalidades($("#ddlEstadoRecebedor").val(), 'selCidadeRecebedor', r.Objeto.Localidade.Codigo);
                }
            }

            $('#txtCaptchaReceita').val();
            $("#divCaptchaReceita").modal('hide');

        } else {
            jAlert(r.Erro, "Consulta de Cliente Receita");
        }
    });
}

