var errosEnvioNFe, notasFiscaisConsultaSefaz, chavesNFeImportadas;

$(document).ready(function () {
    $("#txtValorFreteImportacaoSefaz").priceFormat({ prefix: '' });
    $("#txtValorPedagioImportacaoSefaz").priceFormat({ prefix: '' });
    $("#txtValorAdcEntregaImportacaoSefaz").priceFormat({ prefix: '' });

    CarregarConsultaDeFretesPorValor("btnBuscarTabelaImportacaoSefaz", "btnBuscarTabelaImportacaoSefaz", "A", RetornoConsultaFreteImportacaoSefaz, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotoristaImportacaoSefaz", "btnBuscarMotoristaImportacaoSefaz", "A", RetornoConsultaMotoristaImportacaoSefaz, true, false)
    CarregarConsultaDeVeiculos("btnBuscarVeiculoImportacaoSefaz", "btnBuscarVeiculoImportacaoSefaz", RetornoConsultaVeiculoImportacaoSefaz, true, false, 0);
    CarregarConsultaDeVeiculos("btnBuscarReboqueImportacaoSefaz", "btnBuscarReboqueImportacaoSefaz", RetornoConsultaReboqueImportacaoSefaz, true, false, 1);
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarSeguroImportacaoSefaz", "btnBuscarSeguroImportacaoSefaz", 0, RetornoConsultaSeguroImportacaoSefaz, true, false);
    CarregarConsultadeClientes("btnBuscarExpedidorImportacaoSefaz", "btnBuscarExpedidorImportacaoSefaz", RetornoConsultaExpedidorImportacaoSefaz, true, false);
    CarregarConsultadeClientes("btnBuscarRecebedorImportacaoSefaz", "btnBuscarRecebedorImportacaoSefaz", RetornoConsultaRecebedorImportacaoSefaz, true, false);

    RemoveConsulta("#txtMotoristaImportacaoSefaz, #txtVeiculoImportacaoSefaz, #txtTabelaImportacaoSefaz, #txtSeguroImportacaoSefaz, #txtExpedidorImportacaoSefaz, #txtRecebedorImportacaoSefaz", function ($this) {
        /**
         * Esse callback é executado quando o campo de busca é limpo (backspace, recortar, etc)
         * O $this é o input do texto
         * Mas como precisamos limpar o input hidden do filtro, buscaremos pelo id do campos
         * hddCodigoCAMPOImportacaoSefaz
         * Porém, o campo de tabela frete possui nome diferente do input hidden, então preciso
         * checar com o if quando é o mesmo
         */
        var campo = $this.attr('id').substr(3).replace("ImportacaoXML", '');
        if (campo == "Tabela") campo = "TabelaFrete";
        var id = "#hddCodigo" + campo + "ImportacaoSefaz";
        $this.val('');
        $(id).val('');
    });

    RemoveConsulta("#txtTabelaImportacaoSefaz", function ($this) {
        /**
         * Apenas para liberar o campo de valor frente
         */
        $("#txtValorFreteImportacaoSefaz").removeAttr('disabled');
    });

    $("#txtValorFreteImportacaoSefaz").keyup(function () {
        var valor = parseFloat($(this).val().replace(',', '.'));

        if (isNaN(valor) || valor == 0)
            $("#txtTabelaImportacaoSefaz, #btnBuscarTabelaImportacaoSefaz").removeAttr('disabled');
        else
            $("#txtTabelaImportacaoSefaz, #btnBuscarTabelaImportacaoSefaz").attr('disabled', 'disabled');
    });

    $("#btnImportarChavesNFes").click(function () {
        AbrirDivImportacaoChavesNFe();
    });

    $("#btnImportarNFeSefaz").click(function () {
        AbrirDivConsultaNFeSefaz();
    });

    $("#btnImportarNFeSefaz").click(function () {
        AbrirDivConsultaNFeSefaz();
    });

    $("#btnConsultaNFeSefaz").click(function () {
        if (ValidarChaveConsultaNFeSefaz()) {
            AbrirDivCaptchaConsultaNFeSefaz();
        }
    });

    document.getElementById("txtChaveConsultaNFeSefaz").addEventListener("keydown", function (e) {
        if (e.keyCode == 13) {
            if (ValidarChaveConsultaNFeSefaz()) {
                AbrirDivCaptchaConsultaNFeSefaz();
            }
        }
    });

    document.getElementById("txtCaptchaNFeSefaz").addEventListener("keydown", function (e) {
        if (e.keyCode == 13) {
            if (ValidarCaptchaNFeSefaz())
                ConsultaNFeSefaz();
        }
    });

    $("#selTipoRateioImportacaoSefaz").change(function () {
        ControlarCampoPedagio();
    });

    $("#btnGerarCTeNFeSefaz").click(function () {
        if (ValidaGerarCTeNFeSefaz()) {

            var cpfCnpjRemetente = notasFiscaisConsultaSefaz[0].Remetente;
            var cpfCnpjDestinatario = notasFiscaisConsultaSefaz[0].Destinatario;
            var ufsRemetentes = [];
            var ufsDestinatarios = [];
            var countDestinatario = 0;
            var countRemetente = 0;
            var notasFiscaisJaUtilizadas = new Array();

            for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
                if (notasFiscaisConsultaSefaz[i].NumeroDosCTesUtilizados.length > 0)
                    notasFiscaisJaUtilizadas.push(notasFiscaisConsultaSefaz[i]);

                if (cpfCnpjRemetente != notasFiscaisConsultaSefaz[i].Remetente)
                    countRemetente += 1;

                if (cpfCnpjDestinatario != notasFiscaisConsultaSefaz[i].Destinatario)
                    countDestinatario += 1;

                if ($.inArray(notasFiscaisConsultaSefaz[i].RemetenteUF, ufsRemetentes) < 0)
                    ufsRemetentes.push(notasFiscaisConsultaSefaz[i].RemetenteUF);

                if ($.inArray(notasFiscaisConsultaSefaz[i].DestinatarioUF, ufsDestinatarios) < 0)
                    ufsDestinatarios.push(notasFiscaisConsultaSefaz[i].DestinatarioUF);
            }

            if (countDestinatario > 0 || countRemetente > 0 || ufsRemetentes.length > 1 || ufsDestinatarios.length > 1) {
                var distintos = [];

                if (countRemetente > 0)
                    distintos.push("<b>" + countRemetente + " emitentes</b>");

                if (countDestinatario > 0)
                    distintos.push("<b>" + countDestinatario + " destinatários</b>");

                if (ufsRemetentes.length > 1)
                    distintos.push("<b>" + (ufsRemetentes.length) + " UFs emitentes</b>");

                if (ufsDestinatarios.length > 1)
                    distintos.push("<b>" + (ufsDestinatarios.length) + " UFs destinatários</b>");

                var msg = "Foram encontrados " + distintos.join(distintos.length > 2 ? ", " : " e ") + " diferentes nas notas fiscais enviadas.<br /><br />Deseja realmente prosseguir com a emissão?";
                jConfirm(msg, "Atenção", function (r) {
                    if (r) {
                        if (notasFiscaisJaUtilizadas.length > 0)
                            ExibirMensagemNotasFiscaisEmUsoHTML(notasFiscaisJaUtilizadas);
                        else
                            GerarCTeNFeSefaz();
                    }
                });
            }
            else if (notasFiscaisJaUtilizadas.length > 0)
                ExibirMensagemNotasFiscaisEmUsoHTML(notasFiscaisJaUtilizadas);
            else
                GerarCTeNFeSefaz();
        }
    });

    $("#btnCaptchaNFeSefaz").click(function () {
        if (ValidarCaptchaNFeSefaz())
            ConsultaNFeSefaz();
    });

    $("#btnAtualizarCaptchaNFeSefaz").click(function () {
        AbrirDivCaptchaConsultaNFeSefaz();
        $("#txtCaptchaNFeSefaz").focus();
    });

    $("#btnFecharConsultaNFeSefaz").click(function () {
        LimparConsultaNFeSefaz();
        $('#divConsultaNFeSefaz').modal("hide");
    });

    $('#divConsultaNFeSefaz').on('shown.bs.modal', function () {
        $("#txtChaveConsultaNFeSefaz").focus();
    });

    $('#divCaptchaNFeSefaz').on('shown.bs.modal', function () {
        $('#txtCaptchaNFeSefaz').focus();
    });

    $("#selAgrupamentoNFeSefaz").change(function () {
        ExibirOcultarTabelaFreteValorImportacaoSefaz();
    });

    InicializarPlUploadCSV();
    InicializarPlUploadHTMLPortalNFe();
    LimparConsultaNFeSefaz();
    LimparTabelaFreteImportacaoSefaz();
});

function ExibirMensagemNotasFiscaisEmUsoHTML(notas) {
    var msg = "Estas NF-es já foram utilizadas no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
    for (var i = 0; i < notas.length; i++) {
        msg += "<br/><b>" + notas[i].Numero + " - " + notas[i].Chave + "</b>:<br/>";
        for (var j = 0; j < notas[i].NumeroDosCTesUtilizados.length; j++)
            msg += " &bull; " + notas[i].NumeroDosCTesUtilizados[j] + "<br/>";
    }
    msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
    jConfirm(msg, "Atenção", function (ret) {
        if (ret) {
            GerarCTeNFeSefaz();
        }
    });
}

function ControlarCampoPedagio() {
    var tipo = $("#selTipoRateioImportacaoSefaz").val();

    //if (tipo == "6" || tipo == "7")
    //{
    $("#idPedagioImportacaoSefaz").show();
    $("#idAdcEntregaImportacaoSefaz").show();
    //}
    //else {
    //    $("#txtValorPedagioImportacaoSefaz").val("0,00")
    //    $("#idPedagioImportacaoSefaz").hide();

    //    $("#txtValorAdcEntregaImportacaoSefaz").val("0,00")
    //    $("#idAdcEntregaImportacaoSefaz").hide();
    //}
}

function RenderizarRetornoImportacaoChavesNFe() {
    $("#tblChavesNFeImportadas tbody").html("");

    if (chavesNFeImportadas != null) {
        for (var i = 0; i < chavesNFeImportadas.length; i++) {
            if (!chavesNFeImportadas[i].Adicionada)
                $("#tblChavesNFeImportadas tbody").append("<tr><td>" + chavesNFeImportadas[i].Chave + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ConsultarChavesNFeImportadasSefaz(" + JSON.stringify(chavesNFeImportadas[i]) + ")'>Consultar</button></td></tr>");
        }
    }

    if ($("#tblChavesNFeImportadas tbody").html() == "")
        $("#tblChavesNFeImportadas tbody").html("<tr><td colspan='4'>Nenhuma NF-e Carregada.</td></tr>");
}

function AbrirDivImportacaoChavesNFe() {
    $('#divImportarChavesNFe').modal('show');
    InicializarPlUploadNOTFIS();
}

function AbrirDivConsultaNFeSefaz() {
    $("#tituloConsultaNFeSefaz").text("Consulta de NF-e Sefaz");
    $('#divConsultaNFeSefaz').modal("show");
    LimparTabelaFreteImportacaoSefaz();
}

function ConsultarChavesNFeImportadasSefaz(nfe) {
    $("#txtCaptchaNFeSefaz").val("");
    $("#txtChaveConsultaNFeSefaz").val(nfe.Chave);

    executarRest("/ConhecimentoDeTransporteEletronico/ConsultarNFeSefazSalva?callback=?", { ChaveNFe: $("#txtChaveConsultaNFeSefaz").val() }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.NumeroDosCTesUtilizados != null && r.Objeto.NumeroDosCTesUtilizados != "") {
                var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
                msg += " &bull; " + r.Objeto.NumeroDosCTesUtilizados + "<br/>";
                msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
                jConfirm(msg, "Atenção", function (ret) {
                    if (ret) {

                        if (chavesNFeImportadas != null)
                            for (var i = 0; i < chavesNFeImportadas.length; i++) {
                                if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                                    chavesNFeImportadas[i].Adicionada = true;
                                    RenderizarRetornoImportacaoChavesNFe();
                                    break;
                                }
                            }

                        LimparCamposConsultaNFeSefaz();
                        notasFiscaisConsultaSefaz.push(r.Objeto);
                        RenderizarNFesConsultaSefaz();
                        FecharDivCaptchaConsultaNFeSefaz();
                    }
                    else FecharDivCaptchaConsultaNFeSefaz();
                });
            } else {

                if (chavesNFeImportadas != null)
                    for (var i = 0; i < chavesNFeImportadas.length; i++) {
                        if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                            chavesNFeImportadas[i].Adicionada = true;
                            RenderizarRetornoImportacaoChavesNFe();
                            break;
                        }
                    }

                LimparCamposConsultaNFeSefaz();
                notasFiscaisConsultaSefaz.push(r.Objeto);
                RenderizarNFesConsultaSefaz();
                FecharDivCaptchaConsultaNFeSefaz();
            }
        } else {
            executarRest("/ConhecimentoDeTransporteEletronico/ConsultarNFeSefaz?callback=?", { ChaveNFe: $("#txtChaveConsultaNFeSefaz").val() }, function (r) {
                if (r.Sucesso) {
                    $('#imgCaptcha').prop('src', r.Objeto.DadosConsultar.imgCaptcha);
                    $("#hddConsultaNFeSefazViewState").val(r.Objeto.DadosConsultar.VIEWSTATE);
                    $("#hddConsultaNFeSefazEventValidation").val(r.Objeto.DadosConsultar.EVENTVALIDATION);
                    $("#hddConsultaNFeSefazToken").val(r.Objeto.DadosConsultar.token);
                    $("#hddConsultaNFeSefazSessionId").val(r.Objeto.DadosConsultar.SessionID);
                    $('#divCaptchaNFeSefaz').modal('show');
                } else {
                    jAlert(r.Erro, "Consulta de NF-e Sefaz");
                }
            });
        }
    });
}

function InicializarPlUploadCSV() {
    documentos = new Array();
    erros = "";
    uploader = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus',
        browse_button: 'btnImportarCSV',
        max_file_size: '10000kb',
        multi_selection: true,
        filters: [{ title: "Arquivos CSV", extensions: "csv" }],
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
    });

    uploader.init();

    uploader.bind('FilesAdded', function (up, files) {

        $.each(uploader.files, function (i, file) {
            up.setOption('url', 'ConhecimentoDeTransporteEletronico/ImportarNFeCSV?callback=?');
            uploader.start();
            //$.fancybox.close();
        });

        up.refresh();
    });

    uploader.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html("   (" + file.percent + "%)");
    });

    uploader.bind('FileUploaded', function (up, file, response) {
        $('#' + file.id + " b").html("   (100%)");

        var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
        if (!retorno.Sucesso)
            jAlert(retorno.Erro);
        else {
            chavesNFeImportadas = retorno.Objeto;
            RenderizarRetornoImportacaoChavesNFe();
        }
    });
}

function InicializarPlUploadHTMLPortalNFe() {
    documentos = new Array();
    erros = "";
    uploader = new plupload.Uploader({
        runtimes: 'gears,html5,flash,silverlight,browserplus',
        browse_button: 'btnImportarHTMLPortalNFe',
        max_file_size: '10000kb',
        multi_selection: true,
        filters: [{ title: "Arquivos HTML", extensions: "html" }],
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
    });

    uploader.init();

    uploader.bind('FilesAdded', function (up, files) {

        $.each(uploader.files, function (i, file) {
            up.setOption('url', 'ConhecimentoDeTransporteEletronico/ImportarHTMLPortalNFe?callback=?');
            uploader.start();

            //$.fancybox.close();
        });

        up.refresh();
    });

    uploader.bind('UploadProgress', function (up, file) {
        $('#' + file.id + " b").html("   (" + file.percent + "%)");
    });

    uploader.bind('FileUploaded', function (up, file, response) {
        $('#' + file.id + " b").html("   (100%)");

        //var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
        var dado = response.response.replace("?(", "").replace(/\);/g, "");
        var retorno = JSON.parse(dado);

        if (!retorno.Sucesso)
            jAlert(retorno.Erro);
        else {
            var notaImportada = false;
            if (notasFiscaisConsultaSefaz != null)
                for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
                    if (notasFiscaisConsultaSefaz[i].Chave == retorno.Objeto.Chave) {
                        notaImportada = true;
                        break;
                    }
                }

            if (notaImportada == false) {
                notasFiscaisConsultaSefaz.push(retorno.Objeto);
                RenderizarNFesConsultaSefaz();
            }
        }
    });
}

function AbrirDivCaptchaConsultaNFeSefaz() {
    $("#txtCaptchaNFeSefaz").val("");

    executarRest("/ConhecimentoDeTransporteEletronico/ConsultarNFeSefazSalva?callback=?", { ChaveNFe: $("#txtChaveConsultaNFeSefaz").val() }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.NumeroDosCTesUtilizados != null && r.Objeto.NumeroDosCTesUtilizados != "") {
                var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
                msg += " &bull; " + r.Objeto.NumeroDosCTesUtilizados + "<br/>";
                msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
                jConfirm(msg, "Atenção", function (ret) {
                    if (ret) {

                        if (chavesNFeImportadas != null)
                            for (var i = 0; i < chavesNFeImportadas.length; i++) {
                                if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                                    chavesNFeImportadas[i].Adicionada = true;
                                    RenderizarRetornoImportacaoChavesNFe();
                                    break;
                                }
                            }

                        LimparCamposConsultaNFeSefaz();
                        notasFiscaisConsultaSefaz.push(r.Objeto);
                        RenderizarNFesConsultaSefaz();
                        FecharDivCaptchaConsultaNFeSefaz();
                    }
                    else FecharDivCaptchaConsultaNFeSefaz();
                });
            } else {

                if (chavesNFeImportadas != null)
                    for (var i = 0; i < chavesNFeImportadas.length; i++) {
                        if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                            chavesNFeImportadas[i].Adicionada = true;
                            RenderizarRetornoImportacaoChavesNFe();
                            break;
                        }
                    }

                LimparCamposConsultaNFeSefaz();
                notasFiscaisConsultaSefaz.push(r.Objeto);
                RenderizarNFesConsultaSefaz();
                FecharDivCaptchaConsultaNFeSefaz();
            }
        } else {
            executarRest("/ConhecimentoDeTransporteEletronico/ConsultarNFeSefaz?callback=?", { ChaveNFe: $("#txtChaveConsultaNFeSefaz").val() }, function (r) {
                if (r.Sucesso && r.Objeto != null && r.Objeto.DadosConsultar != null) {
                    $('#imgCaptcha').prop('src', r.Objeto.DadosConsultar.imgCaptcha);
                    $("#hddConsultaNFeSefazViewState").val(r.Objeto.DadosConsultar.VIEWSTATE);
                    $("#hddConsultaNFeSefazEventValidation").val(r.Objeto.DadosConsultar.EVENTVALIDATION);
                    $("#hddConsultaNFeSefazToken").val(r.Objeto.DadosConsultar.token);
                    $("#hddConsultaNFeSefazSessionId").val(r.Objeto.DadosConsultar.SessionID);
                    $('#divCaptchaNFeSefaz').modal('show');
                } else {
                    if (r != null && r.Erro != null)
                        jAlert(r.Erro, "Consulta de NF-e Sefaz");
                    else
                        jAlert("Não foi possível consultar NF-e, favor tentar novamente.", "Consulta de NF-e Sefaz");
                }
            });
        }
    });
}

function FecharDivCaptchaConsultaNFeSefaz() {
    $('#divCaptchaNFeSefaz').modal("hide");
    $("#txtChaveConsultaNFeSefaz").focus();
}

function ValidaGerarCTeNFeSefaz() {
    var valido = true;

    // Valida seleção de veículos
    var reboque = $("#hddCodigoReboqueImportacaoSefaz").val();
    var veiculo = $("#hddCodigoVeiculoImportacaoSefaz").val();

    if (reboque > 0 && veiculo == 0) {
        jAlert("É obrigatório ter uma tração quando um reboque for selecionado.");
        return false;
    }

    if (notasFiscaisConsultaSefaz == null || notasFiscaisConsultaSefaz.length <= 0) {
        valido = false;
        jAlert('Nenhuma NF-e foi consultada.', "Consulta de NF-e Sefaz");
    }

    if ($("#selTipoTomadorImportacaoSefaz").val() == "-1" && $("#selAgrupamentoNFeSefaz").val() != "") {
        valido = false;
        CampoComErro("#selTipoTomadorImportacaoSefaz");
        jAlert('Não foi informado um tomador.', "Consulta de NF-e Sefaz");
    }
    else
        CampoSemErro("#selTipoTomadorImportacaoSefaz");

    return valido;
}

function ValidarCaptchaNFeSefaz() {
    var captcha = $("#txtCaptchaNFeSefaz").val();
    var valido = true;

    if (captcha == null || captcha == "") {
        CampoComErro("#txtCaptchaNFeSefaz");
        valido = false;
    } else {
        CampoSemErro("#txtCaptchaNFeSefaz");
    }

    if (!valido)
        jAlert('Captcha não informado.', "Consulta de NF-e Sefaz");

    return valido;
}

function ValidarChaveConsultaNFeSefaz() {
    var chaveNFe = apenasNumeros($("#txtChaveConsultaNFeSefaz").val());
    $("#txtChaveConsultaNFeSefaz").val(chaveNFe)
    var valido = true;

    if (chaveNFe == null) {
        jAlert('Chave da NF-e não informada.', "Consulta de NF-e Sefaz");
        valido = false;
    }
    else if (chaveNFe == "") {
        jAlert('Chave da NF-e não informada.', "Consulta de NF-e Sefaz");
        valido = false;
    }
    else if (chaveNFe.length != 44) {
        jAlert('Chave da NFe deve possuir 44 digitos.', "Consulta de NF-e Sefaz");
        CampoComErro("#txtChaveConsultaNFeSefaz");
        valido = false;
    }

    for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
        if (!notasFiscaisConsultaSefaz[i].Excluir) {
            if (notasFiscaisConsultaSefaz[i].Chave == chaveNFe) {
                jAlert("NF-e já foi adicionada!", "Consulta de NF-e Sefaz");
                CampoComErro("#txtChaveConsultaNFeSefaz");
                valido = false;
                break;
            } else {
                CampoSemErro("#txtChaveConsultaNFeSefaz");
            }
        }
    }

    return valido;
}

function ConsultaNFeSefaz() {
    CampoSemErro("#txtCaptchaNFeSefaz");
    var dados = {
        VIEWSTATE: $("#hddConsultaNFeSefazViewState").val(),
        EVENTVALIDATION: $("#hddConsultaNFeSefazEventValidation").val(),
        token: $("#hddConsultaNFeSefazToken").val(),
        SessionID: $("#hddConsultaNFeSefazSessionId").val(),
        ChaveNFe: $("#txtChaveConsultaNFeSefaz").val(),
        Captcha: $("#txtCaptchaNFeSefaz").val()
    }

    executarRest("/ConhecimentoDeTransporteEletronico/InformarCaptchaNFeSefaz?callback=?", dados, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.NumeroDosCTesUtilizados != null && r.Objeto.NumeroDosCTesUtilizados != "") {
                var msg = "Esta NF-e já foi utilizada no(s) seguinte(s) conhecimento(s) de transporte: <br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden;'>";
                msg += " &bull; " + r.Objeto.NumeroDosCTesUtilizados + "<br/>";
                msg += "</div><br/>Deseja utilizá-las assim mesmo?<br/>";
                jConfirm(msg, "Atenção", function (ret) {
                    if (ret) {

                        if (chavesNFeImportadas != null)
                            for (var i = 0; i < chavesNFeImportadas.length; i++) {
                                if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                                    chavesNFeImportadas[i].Adicionada = true;
                                    RenderizarRetornoImportacaoChavesNFe();
                                    break;
                                }
                            }

                        LimparCamposConsultaNFeSefaz();
                        notasFiscaisConsultaSefaz.push(r.Objeto);
                        RenderizarNFesConsultaSefaz();
                        FecharDivCaptchaConsultaNFeSefaz();
                    }
                    else FecharDivCaptchaConsultaNFeSefaz();
                });
            } else {

                if (chavesNFeImportadas != null)
                    for (var i = 0; i < chavesNFeImportadas.length; i++) {
                        if (chavesNFeImportadas[i].Chave == $("#txtChaveConsultaNFeSefaz").val()) {
                            chavesNFeImportadas[i].Adicionada = true;
                            RenderizarRetornoImportacaoChavesNFe();
                            break;
                        }
                    }

                LimparCamposConsultaNFeSefaz();
                notasFiscaisConsultaSefaz.push(r.Objeto);
                RenderizarNFesConsultaSefaz();
                FecharDivCaptchaConsultaNFeSefaz();
            }
        } else {
            jAlert(r.Erro, "Retorno Consulta de NF-e Sefaz", function () {
                AbrirDivCaptchaConsultaNFeSefaz();
                setarFocoCaptcha();
            });
        }
    });
}

function setarFocoCaptcha() {
    setTimeout(function () {
        //CampoComErro("#txtCaptchaNFeSefaz");
        $('#txtCaptchaNFeSefaz').focus();
    }, 2000);
}

function RenderizarNFesConsultaSefaz() {
    $("#tblNFesSefaz tbody").html("");

    if (notasFiscaisConsultaSefaz != null) {
        for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
            if (!notasFiscaisConsultaSefaz[i].Excluir)
                $("#tblNFesSefaz tbody").append("<tr><td>" + notasFiscaisConsultaSefaz[i].Numero + "</td><td>" + notasFiscaisConsultaSefaz[i].Chave + "</td><td>" + notasFiscaisConsultaSefaz[i].DestinatarioNomeCidadeUF + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirNFeConsultaSefaz(" + JSON.stringify(notasFiscaisConsultaSefaz[i]) + ")'>Excluir</button></td></tr>");
        }
    }

    if ($("#tblNFesSefaz tbody").html() == "")
        $("#tblNFesSefaz tbody").html("<tr><td colspan='4'>Nenhuma NF-e importada.</td></tr>");
}

function ExcluirNFeConsultaSefaz(nfe) {
    for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
        if (notasFiscaisConsultaSefaz[i].Chave == nfe.Chave) {
            notasFiscaisConsultaSefaz.splice(i, 1);
            break;
        }
    }
    RenderizarNFesConsultaSefaz();
}

var EnumAgrupamentoNFeSefaz = {
    PorRemetenteEDestinatario: 0,
    PorRemetente: 1,
    PorDestinatario: 2,
    CTePorNFe: 3,
    PorUFDestino: 4
};

function GerarCTeNFeSefaz() {
    var agruparCTe = false;
    var agruparRemetente = false;
    var agruparDestinatario = false;
    var agruparUFDestino = false;
    var notasGeracao = new Array();

    agrupamentoNFeSefaz = $("#selAgrupamentoNFeSefaz").val();

    if (agrupamentoNFeSefaz != "")
        agruparCTe = true;
    if (agrupamentoNFeSefaz == EnumAgrupamentoNFeSefaz.PorRemetenteEDestinatario) {
        agruparRemetente = true;
        agruparDestinatario = true;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoNFeSefaz.PorRemetente) {
        agruparRemetente = true;
        agruparDestinatario = false;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoNFeSefaz.PorDestinatario) {
        agruparRemetente = false;
        agruparDestinatario = true;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoNFeSefaz.CTePorNFe) {
        agruparRemetente = false;
        agruparDestinatario = false;
    }
    if (agrupamentoNFeSefaz == EnumAgrupamentoNFeSefaz.PorUFDestino) {
        agruparUFDestino = true;
    }

    var j = 0;
    for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
        if (!notasFiscaisConsultaSefaz[i].Excluir) {
            notasGeracao[j] = notasFiscaisConsultaSefaz[i];
            j += j + 1;
        }
    }

    if (notasGeracao == null || notasGeracao.length == 0) {
        jAlert("Nenhuma NF-e importada", "Consulta de NF-e Sefaz");
    }
    else {
        if (!agruparCTe) {
            NovoCTe(false);

            var pesoTotal = 0;
            var volumeTotal = 0;
            var listaNotasFiscais = new Array();

            for (var i = 0; i < notasFiscaisConsultaSefaz.length; i++) {
                if (ValidarChaveNFe(notasFiscaisConsultaSefaz[i].Chave, listaNotasFiscais)) {
                    var notaFiscal = {
                        Codigo: -(i + 1),
                        Numero: notasFiscaisConsultaSefaz[i].Numero,
                        Chave: notasFiscaisConsultaSefaz[i].Chave,
                        ValorTotal: notasFiscaisConsultaSefaz[i].ValorTotal,
                        DataEmissao: notasFiscaisConsultaSefaz[i].DataEmissao,
                        RemetenteUF: notasFiscaisConsultaSefaz[i].RemetenteUF,
                        DestinatarioUF: notasFiscaisConsultaSefaz[i].DestinatarioUF
                    };
                    pesoTotal += notasFiscaisConsultaSefaz[i].UtilizarPesoLiquido == "1" ? notasFiscaisConsultaSefaz[i].PesoLiquido : notasFiscaisConsultaSefaz[i].Peso;
                    volumeTotal += notasFiscaisConsultaSefaz[i].Volume;
                    listaNotasFiscais.push(notaFiscal);
                }
            }

            $("#hddNotasFiscaisEletronicasRemetente").val(JSON.stringify(listaNotasFiscais));

            RenderizarNFesRemetente();
            AdicionarPesoNFeRemetente(pesoTotal);
            AdicionarVolumeNFeRemetente(volumeTotal);
            AtualizarValorTotalDaCarga();

            if ($('#ddlSerie option[value=' + notasFiscaisConsultaSefaz[0].Serie + ']').length > 0)
                $("#ddlSerie").val(notasFiscaisConsultaSefaz[0].Serie);

            if (notasFiscaisConsultaSefaz[0].Placa != null && notasFiscaisConsultaSefaz[0].Placa != "") {
                $("#txtPlacaVeiculo").val(notasFiscaisConsultaSefaz[0].Placa);
                AdicionarVeiculo();
            }

            BuscarRemetente(true, notasFiscaisConsultaSefaz[0].Remetente, null, null, true);

            if (notasFiscaisConsultaSefaz[0].Destinatario != null && notasFiscaisConsultaSefaz[0].Destinatario != "")
                BuscarDestinatario(true, notasFiscaisConsultaSefaz[0].Destinatario, null, null, true);
            else
                PreencherCamposDestinatarioExportacao(notasFiscaisConsultaSefaz[0].DestinatarioExportacao);

            if (notasFiscaisConsultaSefaz[0].FormaPagamento != null && notasFiscaisConsultaSefaz[0].FormaPagamento != "")
                SetarTomadorXML(notasFiscaisConsultaSefaz[0].FormaPagamento);

            BuscaPalavrasChavesConsultaNFeSefaz();

        }
        else {
            var dados = {
                NFes: JSON.stringify(notasFiscaisConsultaSefaz),
                AgruparRemetente: agruparRemetente,
                AgruparDestinatario: agruparDestinatario,
                AgruparUFDestino: agruparUFDestino,
                TipoRateio: $("#selTipoRateioImportacaoSefaz").val(),
                ValorFrete: $("#txtValorFreteImportacaoSefaz").val(),
                CodigoTabelaFreteValor: $("#hddCodigoTabelaFreteImportacaoSefaz").val(),
                CodigoVeiculo: $("#hddCodigoVeiculoImportacaoSefaz").val(),
                CodigoReboque: $("#hddCodigoReboqueImportacaoSefaz").val(),
                CodigoMotorista: $("#hddCodigoMotoristaImportacaoSefaz").val(),
                CodigoSeguro: $("#hddCodigoSeguroImportacaoSefaz").val(),
                TipoTomador: $("#selTipoTomadorImportacaoSefaz").val(),
                ObservacaoCTe: $("#txtObservacaoImportacaoSefaz").val(),
                ExpedidorCTe: $("#hddCodigoExpedidorImportacaoSefaz").val(),
                RecebedorCTe: $("#hddCodigoRecebedorImportacaoSefaz").val(),
                ValorPedagio: $("#txtValorPedagioImportacaoSefaz").val(),
                ValorAdicionalEntrega: $("#txtValorAdcEntregaImportacaoSefaz").val(),
                ManterDigitacao: true
            };

            executarRest("/ConhecimentoDeTransporteEletronico/SalvarCTePorXMLNFe?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    LimparConsultaNFeSefaz();
                    $('#divConsultaNFeSefaz').modal("hide");
                    AtualizarGridCTes();
                    ExibirMensagemSucesso("CTes salvos com sucesso!", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    }
}

function apenasNumeros(string) {
    var numsStr = string.replace(/[^0-9]/g, '');
    return numsStr;
}

function LimparConsultaNFeSefaz() {
    LimparCamposConsultaNFeSefaz();
    ControlarCampoPedagio();

    errosEnvioXMLNFe = "";
    notasFiscaisConsultaSefaz = null;
    notasFiscaisConsultaSefaz = new Array();
    RenderizarNFesConsultaSefaz();

    LimparImportacaoChaves();
}

function LimparImportacaoChaves() {
    chavesNFeImportadas = null;
    chavesNFeImportadas = new Array();
    RenderizarRetornoImportacaoChavesNFe();
}

function LimparCamposConsultaNFeSefaz() {
    $("#txtChaveConsultaNFeSefaz").val("");
    $("#txtCaptchaNFeSefaz").val("");
    $("#hddConsultaNFeSefazViewState").val("");
    $("#hddConsultaNFeSefazEventValidation").val("");
    $("#hddConsultaNFeSefazToken").val("");
    $("#hddConsultaNFeSefazSessionId").val("");
    CampoSemErro("#txtChaveConsultaNFeSefaz");
}

function BuscaPalavrasChavesConsultaNFeSefaz() {
    executarRest("/XMLNotaFiscalEletronica/ObterPalavrasChaves?callback=?", "", function (r) {
        if (r.Sucesso) {
            MostrarObservacaoNFeConsultaSefaz(r.Objeto);
            LimparConsultaNFeSefaz();
            $('#divConsultaNFeSefaz').modal("hide");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function MostrarObservacaoNFeConsultaSefaz(palavrasChave) {
    if (notasFiscaisConsultaSefaz != null && notasFiscaisConsultaSefaz[0].Observacao != null) {
        var palavra = "";
        var indexBusca = 0;
        var observacao = notasFiscaisConsultaSefaz[0].Observacao.toUpperCase();
        var mostrarMensagem = false;

        if (observacao != "" && palavrasChave.length > 0) {
            for (var i = 0; i < palavrasChave.length; i++) {
                palavra = palavrasChave[i].Palavra.toUpperCase();
                indexBusca = observacao.indexOf(palavra);
                if (indexBusca > 0) {
                    observacao = observacao.replace(new RegExp(palavra, 'g'), "<FONT COLOR=red><b>" + palavra + "</b></FONT>");
                    mostrarMensagem = true;
                }
            }

            if (mostrarMensagem) {
                var msg = "<b>Atenção para as palavras destacadas na observação da nota fiscal importada:</b><br/><br/><div style='max-height: 400px; width: 100%; overflow-y: scroll; overflow-x: hidden; '>";
                msg += " &bull; " + observacao + "<br/>";
                msg += "</div><br/>";
                jAlert(msg, "Observação Nota Fiscal");
            }
        }
    }
}

function ExibirOcultarTabelaFreteValorImportacaoSefaz() {
    if ($("#selAgrupamentoNFeSefaz").val() != "") {
        $("#divOpcoesAvancadasImportacaoSefaz").show();
    } else {
        $("#divOpcoesAvancadasImportacaoSefaz").hide();
    }
}

function RetornoConsultaFreteImportacaoSefaz(frete) {
    $("#hddCodigoTabelaFreteImportacaoSefaz").val(frete.Codigo);
    $("#txtTabelaImportacaoSefaz").val(frete.Destino + " - " + frete.ValorMinimoFrete);

    // Bloqueia campo de valor frete
    $("#txtValorFreteImportacaoSefaz").val("0,00").attr('disabled', 'disabled');
}

function RetornoConsultaMotoristaImportacaoSefaz(motorista) {
    $("#hddCodigoMotoristaImportacaoSefaz").val(motorista.Codigo);
    $("#txtMotoristaImportacaoSefaz").val(motorista.Nome + " - " + motorista.CPFCNPJ);
}

function RetornoConsultaVeiculoImportacaoSefaz(veiculo) {
    $("#hddCodigoVeiculoImportacaoSefaz").val(veiculo.Codigo);
    $("#txtVeiculoImportacaoSefaz").val(veiculo.Placa);
}

function RetornoConsultaReboqueImportacaoSefaz(veiculo) {
    $("#hddCodigoReboqueImportacaoSefaz").val(veiculo.Codigo);
    $("#txtReboqueImportacaoSefaz").val(veiculo.Placa);
}

function RetornoConsultaSeguroImportacaoSefaz(seguro) {
    $("#hddCodigoSeguroImportacaoSefaz").val(seguro.Codigo);
    $("#txtSeguroImportacaoSefaz").val(seguro.NomeSeguradora);
}

function RetornoConsultaExpedidorImportacaoSefaz(cliente) {
    $("#hddCodigoExpedidorImportacaoSefaz").val(cliente.CPFCNPJ);
    $("#txtExpedidorImportacaoSefaz").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaRecebedorImportacaoSefaz(cliente) {
    $("#hddCodigoRecebedorImportacaoSefaz").val(cliente.CPFCNPJ);
    $("#txtRecebedorImportacaoSefaz").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function LimparTabelaFreteImportacaoSefaz() {
    $("#hddCodigoTabelaFreteImportacaoSefaz").val("");
    $("#txtTabelaImportacaoSefaz").val("");

    $("#hddCodigoMotoristaImportacaoSefaz").val("");
    $("#txtMotoristaImportacaoSefaz").val("");

    $("#hddCodigoVeiculoImportacaoSefaz").val("");
    $("#txtVeiculoImportacaoSefaz").val("");

    $("#hddCodigoReboqueImportacaoSefaz").val("");
    $("#txtReboqueImportacaoSefaz").val("");

    $("#hddCodigoSeguroImportacaoSefaz").val("");
    $("#txtSeguroImportacaoSefaz").val("");

    $("#hddCodigoExpedidorImportacaoSefaz").val("");
    $("#txtRecebedorImportacaoSefaz").val("");

    $("#hddCodigoRecebedorImportacaoSefaz").val("");
    $("#txtExpedidorImportacaoSefaz").val("");
}