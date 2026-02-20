<%@ Page Title="Cadastro de Empresas Emissoras" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmpresasEmissoras.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EmpresasEmissoras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .divPermissoes {
            margin-bottom: 15px;
            display: block;
            width: 100%;
        }

        .divHeaderPermissoes {
            display: block;
            width: 99.1%;
            padding: 5px 5px 5px 7px;
            background-color: #EEEEEE;
            border: 1px solid #CDCDCD;
            border-bottom: 0px;
            cursor: pointer;
        }

            .divHeaderPermissoes span {
                font-size: 12px;
                font-weight: bold;
            }

        .divBodyPermissoes {
            display: block;
            width: 100%;
        }

        .fields .fields-title {
            border-bottom: #cdcdcd solid 1px;
            margin: 0 2px 5px 5px;
        }

            .fields .fields-title h3 {
                font-size: 1.5em;
                padding: 0 0 8px 8px;
                font-family: "Segoe UI", "Frutiger", "Tahoma", "Helvetica", "Helvetica Neue", "Arial", "sans-serif";
                color: #000;
                letter-spacing: -1px;
                font-weight: bold;
            }
    </style>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.filedownload.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var path = "";
        var countArquivos = 0;
        $(document).ready(function () {

            ObterFusosHorarios();

            if (document.location.pathname.split("/").length > 1) {
                var paths = document.location.pathname.split("/");
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") {
                        path += "/" + paths[i];
                    }
                }
            }

            $("#txtRNTRC").mask("99999999");
            $("#txtCNPJ").mask("99.999.999/9999-99");
            $("#txtCEP").mask("99.999-999");
            $("#txtDataInicialCertificado").mask("99/99/9999");
            $("#txtDataInicialCertificado").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataFinalCertificado").mask("99/99/9999");
            $("#txtDataFinalCertificado").datepicker({ changeMonth: true, changeYear: true });

            CarregarConsultaDeEmpresas("default-search", "default-search", "", Editar, true, false);
            CarregarConsultadeClientes("btnBuscarContador", "btnBuscarContador", RetornoConsultaContador, true, false, "F");
            CarregarConsultaDeEmpresas("btnBuscarEmpresaAdmin", "btnBuscarEmpresaAdmin", "Z", RetornoConsultaEmpresaAdmin, true, false);

            $("#txtCNPJ").keydown(function (e) {
                if (e.which == 13) {
                    VerificarEmpresaCadastrada();
                }
            });

            $("#txtContador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("contador", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtEmpresaAdmin").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("empresaAdmin", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            BuscarUFs("selUF");

            $("#selUF").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidade", null);
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnExcluir").click(function () {
                Excluir();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
                $("#btnSelecionarCertificado").show();
            });

            $("#btnSelecionarCertificado").click(function () {
                if (ValidarCampos()) {
                    var codigoEmpresa = $("#hddCodigo").val();
                    //if (isNaN(codigoEmpresa) || codigoEmpresa == "0" || codigoEmpresa == "")
                    SalvarParaEnvioCertificado();

                    InicializarPlUpload();
                    AbrirPlUpload();
                } else {
                    ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
                }
            });

            $("#btnRemoverCertificado").click(function () {
                RemoverCertificado();
            });

            $("#btnDownloadCertificado").click(function () {
                DownloadCertificado();
            });

            $("#btnGerarPercursos").click(function () {
                GerarPercursos();
            });

            $("#btnReenviarOracle").click(function () {
                AtualiarOracle();
            });

            //$("#btnConsultarReceita").click(function () {
            //    AbrirConsultaReceita();
            //});
            //$("#btnConsultarReceita").click(function () {
            //    ConsultarDadosCentralizado();
            //});
            $("#btnConsultarReceita").click(function () {
                ConsultarCNPJReceitaWS();
            });

            $("#btnCaptchaNFeSefaz").click(function () {
                ConsultarDadosReceita();
            });

            $("#btnAtualizarCaptchaNFeSefaz").click(function () {
                AbrirConsultaReceita();
            });

            $("#txtCNPJ").focusout(function () {
                VerificarEmpresaCadastrada();
            });

            VerificaECarregaEmpresaPorParametro();
        });

        var cookies;

        function VerificarEmpresaCadastrada() {
            BuscarDetalhesPorCNPJ($("#txtCNPJ").val());
        }

        function AbrirConsultaReceita() {
            $("#txtCaptcha").val("");
            executarRest("/Cliente/ConsultarClienteReceita?callback=?", { CNPJ: $("#txtCNPJ").val() }, function (r) {
                if (r.Sucesso) {
                    $('#imgCaptcha').prop('src', r.Objeto.chaptcha);
                    cookies = r.Objeto.Cookies;

                    $.fancybox({
                        href: '#divConsultaReceita',
                        width: 300,
                        height: 180,
                        fitToView: false,
                        autoSize: false,
                        closeClick: false,
                        closeBtn: true,
                        openEffect: 'none',
                        closeEffect: 'none',
                        centerOnScroll: true,
                        type: 'inline',
                        padding: 7,
                        scrolling: 'no',
                        helpers: {
                            overlay: {
                                css: {
                                    cursor: 'auto'
                                },
                                closeClick: false
                            }
                        }
                    });

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function VerificaECarregaEmpresaPorParametro() {
            var cnpj = getParm("CNPJ");
            var exibirMensagemDeDuplicidade = false;

            if (cnpj) {
                // Seta o CNPJ para caso não exista
                $("#txtCNPJ").val(cnpj).focus();
                BuscarDetalhesPorCNPJ(cnpj, exibirMensagemDeDuplicidade);
            }
        }


        function ConsultarDadosCentralizado() {
            executarRest("/Cliente/ConsultarClienteSintegraCentralizado?callback=?", { CNPJ: $("#txtCNPJ").val() }, function (r) {
                if (r.Sucesso) {

                    if (r.Objeto.Nome != "")
                        $("#txtRazaoSocial").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#txtNomeFantasia").val(r.Objeto.Fantasia);
                    if (r.Objeto.CEP != "")
                        $("#txtCEP").val(r.Objeto.CEP);
                    if (r.Objeto.Endereco != "")
                        $("#txtLogradouro").val(r.Objeto.Endereco);
                    $("#txtComplemento").val(r.Objeto.Complemento);
                    if (r.Objeto.Numero != "")
                        $("#txtNumero").val(r.Objeto.Numero);
                    if (r.Objeto.Bairro != "")
                        $("#txtBairro").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#txtTelefone1").val(r.Objeto.TelefonePrincipal).change();
                    if (r.Objeto.InscricaoEstadual != "")
                        $("#txtInscricaoEstadual").val(r.Objeto.InscricaoEstadual);
                    if (r.Objeto.Localidade != null) {
                        $("#selUF").val(r.Objeto.Localidade.UF);
                        BuscarLocalidades($("#selUF").val(), "selLocalidade", null);
                        BuscarLocalidadesPorCodigo("selLocalidade", r.Objeto.Localidade.Codigo);
                    }

                    $.fancybox.close();

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function ConsultarCNPJReceitaWS() {
            executarRest("/Cliente/ConsultarCNPJReceitaWS?callback=?", { CNPJ: $("#txtCNPJ").val() }, function (r) {
                if (r.Sucesso) {

                    if (r.Objeto.Nome != "")
                        $("#txtRazaoSocial").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#txtNomeFantasia").val(r.Objeto.Fantasia);
                    if (r.Objeto.CEP != "")
                        $("#txtCEP").val(r.Objeto.CEP);
                    if (r.Objeto.Endereco != "")
                        $("#txtLogradouro").val(r.Objeto.Endereco);
                    $("#txtComplemento").val(r.Objeto.Complemento);
                    if (r.Objeto.Numero != "")
                        $("#txtNumero").val(r.Objeto.Numero);
                    if (r.Objeto.Bairro != "")
                        $("#txtBairro").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#txtTelefone1").val(r.Objeto.TelefonePrincipal).change();
                    if (r.Objeto.InscricaoEstadual != "")
                        $("#txtInscricaoEstadual").val(r.Objeto.InscricaoEstadual);
                    if (r.Objeto.Localidade != null) {
                        $("#selUF").val(r.Objeto.Localidade.UF);
                        BuscarLocalidades($("#selUF").val(), "selLocalidade", null);
                        BuscarLocalidadesPorCodigo("selLocalidade", r.Objeto.Localidade.Codigo);
                    }

                    $.fancybox.close();

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function ConsultarDadosReceita() {
            executarRest("/Cliente/InformarCaptchaReceita?callback=?", { CNPJ: $("#txtCNPJ").val(), Captcha: $("#txtCaptcha").val(), Cookies: JSON.stringify(cookies) }, function (r) {
                if (r.Sucesso) {

                    $("#txtRazaoSocial").val(r.Objeto.Nome);
                    if (r.Objeto.Fantasia != "")
                        $("#txtNomeFantasia").val(r.Objeto.Fantasia);
                    $("#txtCEP").val(r.Objeto.CEP);
                    $("#txtLogradouro").val(r.Objeto.Endereco);
                    $("#txtComplemento").val(r.Objeto.Complemento);
                    $("#txtNumero").val(r.Objeto.Numero);
                    $("#txtBairro").val(r.Objeto.Bairro);
                    if (r.Objeto.TelefonePrincipal != "")
                        $("#txtTelefone1").val(r.Objeto.TelefonePrincipal).change();

                    if (r.Objeto.Localidade != null) {
                        $("#selUF").val(r.Objeto.Localidade.UF);
                        BuscarLocalidades($("#selUF").val(), "selLocalidade", null);
                        BuscarLocalidadesPorCodigo("selLocalidade", r.Objeto.Localidade.Codigo);
                    }

                    $.fancybox.close();

                } else {
                    jAlert(r.Erro, "Consulta de Cliente Receita");
                }
            });
        }

        function RetornoConsultaContador(contador) {
            $("body").data("contador", contador.CPFCNPJ);
            $("#txtContador").val(contador.CPFCNPJ + " - " + contador.Nome);
        }

        function RetornoConsultaEmpresaAdmin(empresaAdmin) {
            $("body").data("empresaAdmin", empresaAdmin.Codigo);
            $("#txtEmpresaAdmin").val(empresaAdmin.CNPJ + " - " + empresaAdmin.RazaoSocial);
        }

        function AbrirPlUpload() {
            $.fancybox({
                href: '#divUploadArquivos',
                width: 500,
                height: 340,
                fitToView: false,
                autoSize: false,
                closeClick: false,
                closeBtn: true,
                openEffect: 'none',
                closeEffect: 'none',
                centerOnScroll: true,
                type: 'inline',
                padding: 0,
                scrolling: 'no',
                helpers: {
                    overlay: {
                        css: {
                            cursor: 'auto'
                        },
                        closeClick: false
                    }
                }
            });
        }

        var erros = "";
        function InicializarPlUpload() {
            countArquivos = 0;
            erros = "";
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/Empresa/SalvarCertificado?callback=?&Codigo=' + $("#hddCodigo").val() + "&SenhaCertificado=" + $("#txtSenhaCertificado").val(),
                max_file_size: '100kb',
                unique_names: true,
                filters: [{ title: 'Arquivos de Certificado', extensions: 'pfx' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    StateChanged: function (up) {
                        if (up.state != plupload.STARTED) {
                            if (erros != "") {
                                jAlert("Não foi possível enviar certificado digital: " + erros, "Atenção");
                            }
                            else {
                                $("#btnSelecionarCertificado").hide();
                                $("#btnRemoverCertificado").show();
                                $("#btnDownloadCertificado").show();
                                BuscarDetalhesEnvioCertificado($("#hddCodigo").val());
                            }
                        }
                    },
                    FilesAdded: function (up, files) {
                        countArquivos += files.length;
                        if (countArquivos > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo de certificado. Remova os demais!', 'Atenção');
                        }
                    },
                    FilesRemoved: function (up, files) {
                        countArquivos -= files.length;
                        if (countArquivos <= 1) {
                            $(".plupload_start").css("display", "");
                        }
                    },
                    FileUploaded: function (up, file, response) {
                        $('#' + file.id + " b").html("   (100%)");

                        var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
                        if (!retorno.Sucesso)
                            erros += retorno.Erro + "<br />"
                    }
                }
            });
        }
        function RemoverCertificado() {
            jConfirm("Deseja realmente remover o certificado digital? <b>Este processo é irreversível!</b>", "Atenção", function (ret) {
                if (ret) {
                    executarRest("/Empresa/DeletarCertificado?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            $("#btnSelecionarCertificado").show();
                            $("#btnRemoverCertificado").hide();
                            $("#btnDownloadCertificado").hide();
                            $("#txtSenhaCertificado").show();
                            ExibirMensagemSucesso("Certificado digital removido com sucesso!", "Sucesso");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }
        function DownloadCertificado() {
            executarDownload("/Empresa/DownloadCertificado", { Codigo: $("#hddCodigo").val() });
        }
        function BuscarUFs(idSelect) {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, idSelect);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }
        function ValidarCampos() {
            var razao = $("#txtRazaoSocial").val().trim();
            var nome = $("#txtNomeFantasia").val().trim();
            var cnpj = $("#txtCNPJ").val().trim();
            var ie = $("#txtInscricaoEstadual").val().trim();
            var cep = $("#txtCEP").val().trim();
            var logradouro = $("#txtLogradouro").val().trim();
            var numero = $("#txtNumero").val().trim();
            var bairro = $("#txtBairro").val().trim();
            var telefone1 = $("#txtTelefone1").val().trim();
            var dataInicialCert = $("#txtDataInicialCertificado").val().trim();
            var dataFinalCert = $("#txtDataFinalCertificado").val().trim();
            var rntrc = $("#txtRNTRC").val().trim();
            var valido = true;
            if (razao == "") {
                CampoComErro("#txtRazaoSocial");
                valido = false;
            } else {
                CampoSemErro("#txtRazaoSocial");
            }
            if (nome == "") {
                CampoComErro("#txtNomeFantasia");
                valido = false;
            } else {
                CampoSemErro("#txtNomeFantasia");
            }
            if (cnpj == "") {
                CampoComErro("#txtCNPJ");
                valido = false;
            } else {
                CampoSemErro("#txtCNPJ");
            }
            if (ie == "") {
                CampoComErro("#txtInscricaoEstadual");
                valido = false;
            } else {
                CampoSemErro("#txtInscricaoEstadual");
            }
            if (cep == "") {
                CampoComErro("#txtCEP");
                valido = false;
            } else {
                CampoSemErro("#txtCEP");
            }
            if (logradouro == "") {
                CampoComErro("#txtLogradouro");
                valido = false;
            } else {
                CampoSemErro("#txtLogradouro");
            }
            if (numero == "") {
                CampoComErro("#txtNumero");
                valido = false;
            } else {
                CampoSemErro("#txtNumero");
            }
            if (bairro == "") {
                CampoComErro("#txtBairro");
                valido = false;
            } else {
                CampoSemErro("#txtBairro");
            }
            if (telefone1 == "") {
                CampoComErro("#txtTelefone1");
                valido = false;
            } else {
                CampoSemErro("#txtTelefone1");
            }
            //if (dataInicialCert == "") {
            //    CampoComErro("#txtDataInicialCertificado");
            //    valido = false;
            //} else {
            //    CampoSemErro("#txtDataInicialCertificado");
            //}
            //if (dataFinalCert == "") {
            //    CampoComErro("#txtDataFinalCertificado");
            //    valido = false;
            //} else {
            //    CampoSemErro("#txtDataFinalCertificado");
            //}
            if (rntrc == "" || rntrc.length != 8) {
                CampoComErro("#txtRNTRC");
                valido = false;
            } else {
                CampoSemErro("#txtRNTRC");
            }
            return valido;
        }
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtRazaoSocial").val('');
            $("#txtNomeFantasia").val('');
            $("#txtCNPJ").val('');
            $("#txtInscricaoEstadual").val('');
            $("#txtInscricaoEstadualSubstituicao").val('');
            $("#txtInscricaoMunicipal").val('');
            $("#txtCNAE").val('');
            $("#txtSUFRAMA").val('');
            $("#txtCEP").val('');
            $("#txtDataCadastro").val('');
            $("#txtLogradouro").val('');
            $("#txtComplemento").val('');
            $("#txtNumero").val('');
            $("#txtBairro").val('');
            $("#txtTelefone1").val('').change();
            $("#txtTelefone2").val('').change();
            $("#selUF").val($("#selUF option:first").val());
            $("#selLocalidade").html('');
            $("#txtContato").val('');
            $("#txtTelefoneContato").val('').change();
            $("#txtDataInicialCertificado").val('');
            $("#txtDataFinalCertificado").val('');
            $("#txtSerieCertificado").val('');
            $("#txtSenhaCertificado").val('');
            $("#txtEmails").val('');
            $("#chkEmailsStatus").attr("checked", false);
            $("#chkPermiteEmissaoDocumentosDestinados").attr("checked", false);
            $("#chkCobrarDocumentosDestinados").attr("checked", false);
            //$("#chkEmitirTodosCTesComoSimples").attr("checked", false);
            $("#txtEmailsAdministrativos").val('');
            $("#chkEmailsAdministrativosStatus").attr("checked", false);
            $("#txtEmailsContador").val('');
            $("#chkEmailsContadorStatus").attr("checked", false);
            $("#txtTelefoneContador").val("").change();
            $("#txtNomeContador").val("");
            $("#btnCancelar").hide();
            $("#selTipoEmissao").val($("#selTipoEmissao option:first").val());
            $("#selStatusEmissao").val($("#selStatusEmissao option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selStatusFinanceiro").val($("#selStatusFinanceiro option:first").val());
            $("#selTipoTransportador").val($("#selTipoTransportador option:first").val());
            $("#txtRNTRC").val('');
            $("#selSimplesNacional").val($("#selSimplesNacional option:first").val());
            $("#selRegimeTributarioCTe").val($("#selRegimeTributarioCTe option:first").val());
            //$("#selFusoHorario").val($("#selFusoHorario option:first").val());
            $("body").data("contador", null);
            $("#txtContador").val('');
            $("body").data("empresaAdmin", null);
            $("#txtEmpresaAdmin").val('');
            $("#txtCRCContador").val('');
            $("#txtObservacao").val('');
            $("#txtSerieCTeFora").val('');
            $("#txtSerieCTeDentro").val('');
            $("#txtSerieMDFe").val('');
            $("#txtTAF").val('');
            $("#txtNroRegEstadual").val('');
            $("#txtCodigoIntegracao").val('');
            $("#txtLogStatus").val('');
            $("#selCertificado").val($("#selCertificado option:first").val());

            //$("#btnSelecionarCertificado").hide();
            $("#btnRemoverCertificado").hide();
            $("#btnDownloadCertificado").hide();
            $("#btnGerarPercursos").hide();
            $("#btnReenviarOracle").hide();
            $("#btnSelecionarLogo").hide();
            $("#btnRemoverLogo").hide();
            $("#btnSelecionarLogoSistema").hide();
            $("#btnRemoverLogoSistema").hide();
            $("#btnReenviarEmail").hide();
            LimparPermissoes();
            LimparDadosFinanceiros();

            $("body").data("filiais", null)
            LimparCamposFilial();
            RenderizarFiliais(new Array());

            $("#txtSerieCTeFora").attr({ disabled: false });
            $("#txtSerieCTeDentro").attr({ disabled: false });
            $("#txtSerieMDFe").attr({ disabled: false });
            $("#txtCNPJ").attr({ disabled: false });

            BuscarSerieEmpresaUsuario();

            TodasPermissoes();
        }
        function BuscarSerieEmpresaUsuario() {
            executarRest("/Empresa/BuscarSerieEmpresaUsuario?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    $("#txtSerieCTeFora").val(r.Objeto.SerieCTeFora);
                    $("#txtSerieCTeDentro").val(r.Objeto.SerieCTeDentro);
                    $("#txtSerieMDFe").val(r.Objeto.SerieMDFe);
                }
            });
        }


        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function BuscarLocalidadesPorCodigo(idSelect, codigo) {
            executarRest("/Localidade/BuscarPorCodigo?callback=?", { Codigo: codigo }, function (r) {
                if (r.Sucesso) {
                    var selLocalidades = document.getElementById(idSelect);
                    var optn = document.createElement("option");
                    optn.text = r.Objeto.Descricao;
                    optn.value = r.Objeto.Codigo;
                    if (codigo != null) {
                        if (codigo == r.Objeto.Codigo) {
                            optn.setAttribute("selected", "selected");
                        }
                    }
                    selLocalidades.options.add(optn);

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarLocalidades(localidades, idSelect, codigo) {
            var selLocalidades = document.getElementById(idSelect);
            selLocalidades.options.length = 0;
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                if (codigo != null) {
                    if (codigo == localidades[i].Codigo) {
                        optn.setAttribute("selected", "selected");
                    }
                }
                selLocalidades.options.add(optn);
            }
        }
        function Salvar() {
            if (ValidarCampos()) {
                var empresa = {
                    Codigo: $("#hddCodigo").val(),
                    RazaoSocial: $("#txtRazaoSocial").val(),
                    NomeFantasia: $("#txtNomeFantasia").val(),
                    CNPJ: $("#txtCNPJ").val(),
                    InscricaoEstadual: $("#txtInscricaoEstadual").val(),
                    InscricaoEstadualSubstituicao: $("#txtInscricaoEstadualSubstituicao").val(),
                    InscricaoMunicipal: $("#txtInscricaoMunicipal").val(),
                    CNAE: $("#txtCNAE").val(),
                    SUFRAMA: $("#txtSUFRAMA").val(),
                    CEP: $("#txtCEP").val(),
                    Logradouro: $("#txtLogradouro").val(),
                    Complemento: $("#txtComplemento").val(),
                    Numero: $("#txtNumero").val(),
                    Bairro: $("#txtBairro").val(),
                    Telefone: $("#txtTelefone1").val(),
                    Telefone2: $("#txtTelefone2").val(),
                    Localidade: $("#selLocalidade").val(),
                    Contato: $("#txtContato").val(),
                    TelefoneContato: $("#txtTelefoneContato").val(),
                    DataInicialCertificado: $("#txtDataInicialCertificado").val(),
                    DataFinalCertificado: $("#txtDataFinalCertificado").val(),
                    SerieCertificado: $("#txtSerieCertificado").val(),
                    SenhaCertificado: $("#txtSenhaCertificado").val(),
                    Emails: $("#txtEmails").val(),
                    EmailsStatus: $("#chkEmailsStatus")[0].checked,
                    PermiteEmissaoDocumentosDestinados: $("#chkPermiteEmissaoDocumentosDestinados")[0].checked,
                    CobrarDocumentosDestinados: $("#chkCobrarDocumentosDestinados")[0].checked,
                    //EmitirTodosCTesComoSimples: $("#chkEmitirTodosCTesComoSimples")[0].checked,
                    EmailsAdministrativos: $("#txtEmailsAdministrativos").val(),
                    EmailsAdministrativosStatus: $("#chkEmailsAdministrativosStatus")[0].checked,
                    EmailsContador: $("#txtEmailsContador").val(),
                    EmailsContadorStatus: $("#chkEmailsContadorStatus")[0].checked,
                    NomeContador: $("#txtNomeContador").val(),
                    TelefoneContador: $("#txtTelefoneContador").val(),
                    Emissao: $("#selTipoEmissao").val(),
                    StatusEmissao: $("#selStatusEmissao").val(),
                    Status: $("#selStatus").val(),
                    StatusFinanceiro: $("#selStatusFinanceiro").val(),
                    TipoTransportador: $("#selTipoTransportador").val(),
                    RNTRC: $("#txtRNTRC").val(),
                    SimplesNacional: $("#selSimplesNacional").val(),
                    RegimeTributarioCTe: $("#selRegimeTributarioCTe").val(),
                    FusoHorario: $("#selFusoHorario").val(),
                    CodigoEmpresaCobradora: $("#hddCodigoEmpresaCobradora").val(),
                    CodigoPlanoEmissao: $("#hddCodigoPlanoEmissao").val(),
                    DiaVencimento: $("#selDiaVencimento").val(),
                    Permissoes: JSON.stringify(ObterPermissoes()),
                    Contador: $("body").data("contador"),
                    CRCContador: $("#txtCRCContador").val(),
                    CodigoEmpresaAdmin: $("body").data("empresaAdmin"),
                    Observacao: $("#txtObservacao").val(),
                    Filiais: $("body").data("filiais") != null ? JSON.stringify($("body").data("filiais")) : "",
                    SerieCTeFora: $("#txtSerieCTeFora").val(),
                    SerieCTeDentro: $("#txtSerieCTeDentro").val(),
                    SerieMDFe: $("#txtSerieMDFe").val(),
                    TAF: $("#txtTAF").val(),
                    NroRegEstadual: $("#txtNroRegEstadual").val(),
                    CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                    CertificadoA3: $("#selCertificado").val()
                };

                executarRest("/Empresa/Salvar?callback=?", empresa, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        if (r.Erro != null && r.Erro != "")
                            jAlert("Mensagem retornada do sistema de integração: <br />" + r.Erro, "Sistema de Integração");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }
        function SalvarParaEnvioCertificado() {
            var empresa = {
                Codigo: $("#hddCodigo").val(),
                RazaoSocial: $("#txtRazaoSocial").val(),
                NomeFantasia: $("#txtNomeFantasia").val(),
                CNPJ: $("#txtCNPJ").val(),
                InscricaoEstadual: $("#txtInscricaoEstadual").val(),
                InscricaoEstadualSubstituicao: $("#txtInscricaoEstadualSubstituicao").val(),
                InscricaoMunicipal: $("#txtInscricaoMunicipal").val(),
                CNAE: $("#txtCNAE").val(),
                SUFRAMA: $("#txtSUFRAMA").val(),
                CEP: $("#txtCEP").val(),
                Logradouro: $("#txtLogradouro").val(),
                Complemento: $("#txtComplemento").val(),
                Numero: $("#txtNumero").val(),
                Bairro: $("#txtBairro").val(),
                Telefone: $("#txtTelefone1").val(),
                Telefone2: $("#txtTelefone2").val(),
                Localidade: $("#selLocalidade").val(),
                Contato: $("#txtContato").val(),
                TelefoneContato: $("#txtTelefoneContato").val(),
                DataInicialCertificado: $("#txtDataInicialCertificado").val(),
                DataFinalCertificado: $("#txtDataFinalCertificado").val(),
                SerieCertificado: $("#txtSerieCertificado").val(),
                SenhaCertificado: $("#txtSenhaCertificado").val(),
                Emails: $("#txtEmails").val(),
                EmailsStatus: $("#chkEmailsStatus")[0].checked,
                PermiteEmissaoDocumentosDestinados: $("#chkPermiteEmissaoDocumentosDestinados")[0].checked,
                CobrarDocumentosDestinados: $("#chkCobrarDocumentosDestinados")[0].checked,
                //EmitirTodosCTesComoSimples: $("#chkEmitirTodosCTesComoSimples")[0].checked,
                EmailsAdministrativos: $("#txtEmailsAdministrativos").val(),
                EmailsAdministrativosStatus: $("#chkEmailsAdministrativosStatus")[0].checked,
                EmailsContador: $("#txtEmailsContador").val(),
                EmailsContadorStatus: $("#chkEmailsContadorStatus")[0].checked,
                NomeContador: $("#txtNomeContador").val(),
                TelefoneContador: $("#txtTelefoneContador").val(),
                Emissao: $("#selTipoEmissao").val(),
                StatusEmissao: $("#selStatusEmissao").val(),
                Status: $("#selStatus").val(),
                StatusFinanceiro: $("#selStatusFinanceiro").val(),
                TipoTransportador: $("#selTipoTransportador").val(),
                RNTRC: $("#txtRNTRC").val(),
                SimplesNacional: $("#selSimplesNacional").val(),
                RegimeTributarioCTe: $("#selRegimeTributarioCTe").val(),
                FusoHorario: $("#selFusoHorario").val(),
                CodigoEmpresaCobradora: $("#hddCodigoEmpresaCobradora").val(),
                CodigoPlanoEmissao: $("#hddCodigoPlanoEmissao").val(),
                DiaVencimento: $("#selDiaVencimento").val(),
                Permissoes: JSON.stringify(ObterPermissoes()),
                Contador: $("body").data("contador"),
                CRCContador: $("#txtCRCContador").val(),
                CodigoEmpresaAdmin: $("body").data("empresaAdmin"),
                Observacao: $("#txtObservacao").val(),
                Filiais: $("body").data("filiais") != null ? JSON.stringify($("body").data("filiais")) : "",
                SerieCTeFora: $("#txtSerieCTeFora").val(),
                SerieCTeDentro: $("#txtSerieCTeDentro").val(),
                SerieMDFe: $("#txtSerieMDFe").val(),
                TAF: $("#txtTAF").val(),
                NroRegEstadual: $("#txtNroRegEstadual").val(),
                CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                CertificadoA3: $("#selCertificado").val()
            };
            executarRest("/Empresa/Salvar?callback=?", empresa, function (r) {
                if (!r.Sucesso) {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
                else
                    //BuscarDetalhesEnvioCertificado(r.Objeto.Codigo)
                    $("#hddCodigo").val(r.Objeto.Codigo);
            });
        }
        function Editar(empresa) {
            BuscarDetalhes(empresa);
            BuscarPermissoes(empresa);
        }
        function BuscarDetalhesPorCNPJ(cnpj, exibirMensagemDeDuplicidade) {
            if (arguments.length < 2)
                exibirMensagemDeDuplicidade = true;

            cnpj = apenasNumeros(cnpj);
            var nome = $("#txtRazaoSocial").val();
            if (cnpj != null && cnpj != "" && nome == "") {
                executarRest("/Empresa/ObterDetalhes?callback=?", { CNPJ: cnpj }, function (r) {
                    if (r.Sucesso) {
                        if (exibirMensagemDeDuplicidade)
                            jAlert("CNPJ já possui cadastro!");
                        LimparCampos();
                        empresa = r.Objeto;
                        $("#hddCodigo").val(empresa.Codigo);
                        $("#txtRazaoSocial").val(empresa.RazaoSocial);
                        $("#txtNomeFantasia").val(empresa.NomeFantasia);
                        $("#txtCNPJ").val(empresa.CNPJ);
                        $("#txtInscricaoEstadual").val(empresa.InscricaoEstadual);
                        $("#txtInscricaoEstadualSubstituicao").val(empresa.InscricaoEstadualSubstituicao);
                        $("#txtInscricaoMunicipal").val(empresa.InscricaoMunicipal);
                        $("#txtCNAE").val(empresa.CNAE);
                        $("#txtSUFRAMA").val(empresa.SUFRAMA);
                        $("#txtCEP").val(empresa.CEP);
                        $("#txtLogradouro").val(empresa.Logradouro);
                        $("#txtComplemento").val(empresa.Complemento);
                        $("#txtNumero").val(empresa.Numero);
                        $("#txtBairro").val(empresa.Bairro);
                        $("#txtTelefone1").val(empresa.Telefone).change();
                        $("#txtTelefone2").val(empresa.Telefone2).change();

                        $("#selUF").val(empresa.SiglaUF);
                        BuscarLocalidades(empresa.SiglaUF, "selLocalidade", empresa.Localidade);

                        $("#txtContato").val(empresa.Contato);
                        $("#txtTelefoneContato").val(empresa.TelefoneContato).change();
                        $("#txtTelefoneContador").val(empresa.TelefoneContador).change();
                        $("#txtNomeContador").val(empresa.NomeContador);
                        $("#txtDataCadastro").val(empresa.DataCadastro);
                        $("#txtDataInicialCertificado").val(empresa.DataInicialCertificado);
                        $("#txtDataFinalCertificado").val(empresa.DataFinalCertificado);
                        $("#txtSerieCertificado").val(empresa.SerieCertificado);
                        $("#txtSenhaCertificado").val(empresa.SenhaCertificado);
                        $("#txtEmails").val(empresa.Emails);
                        $("#chkEmailsStatus").attr("checked", empresa.EmailsStatus);
                        $("#chkPermiteEmissaoDocumentosDestinados").attr("checked", empresa.PermiteEmissaoDocumentosDestinados);
                        $("#chkCobrarDocumentosDestinados").attr("checked", empresa.CobrarDocumentosDestinados);
                        //$("#chkEmitirTodosCTesComoSimples").attr("checked", empresa.EmitirTodosCTesComoSimples);
                        $("#txtEmailsAdministrativos").val(empresa.EmailsAdministrativos);
                        $("#chkEmailsAdministrativosStatus").attr("checked", empresa.EmailsAdministrativosStatus);
                        $("#txtEmailsContador").val(empresa.EmailsContador);
                        $("#chkEmailsContadorStatus").attr("checked", empresa.EmailsContadorStatus);
                        $("#selTipoEmissao").val(empresa.Emissao);
                        $("#selStatusEmissao").val(empresa.StatusEmissao);
                        $("#selStatus").val(empresa.Status);
                        $("#selStatusFinanceiro").val(empresa.StatusFinanceiro);
                        $("#selTipoTransportador").val(empresa.TipoTransportador != 0 ? empresa.TipoTransportador : $("#selTipoTransportador option:first").val());
                        $("#txtRNTRC").val(empresa.RNTRC);
                        $("#selSimplesNacional").val(empresa.SimplesNacional.toString());
                        $("#selRegimeTributarioCTe").val(empresa.RegimeTributarioCTe);
                        $("#selFusoHorario").val(empresa.FusoHorario);
                        $("#selDiaVencimento").val(empresa.DiaVencimentoFatura);
                        $("#hddCodigoEmpresaCobradora").val(empresa.CodigoEmpresaCobradora);
                        $("#txtEmpresaCobradora").val(empresa.DescricaoEmpresaCobradora);
                        $("#hddCodigoPlanoEmissao").val(empresa.CodigoPlanoEmissao);
                        $("#txtPlanoEmissaoCTe").val(empresa.DescricaoPlanoEmissao);
                        $("body").data("contador", empresa.CPFCNPJContador);
                        $("#txtContador").val(empresa.DescricaoContador);
                        $("body").data("empresaAdmin", empresa.CodigoEmpresaAdmin);
                        $("#txtEmpresaAdmin").val(empresa.DescricaoEmpresaAdmin);
                        $("#txtCRCContador").val(empresa.CRCContador);
                        $("#txtObservacao").val(empresa.Observacao);
                        $("#txtSerieCTeFora").val(empresa.SerieCTeFora);
                        $("#txtSerieCTeDentro").val(empresa.SerieCTeDentro);
                        $("#txtSerieMDFe").val(empresa.SerieMDFe);
                        $("#txtTAF").val(empresa.TAF);
                        $("#txtNroRegEstadual").val(empresa.NroRegEstadual);
                        $("#txtCodigoIntegracao").val(empresa.CodigoIntegracao);
                        $("#selCertificado").val(empresa.CertificadoA3.toString());
                        $("#txtLogStatus").val(empresa.LogStatus);

                        if (empresa.seriesCadastradas > 0) {
                            $("#txtSerieCTeFora").attr({ disabled: true });
                            $("#txtSerieCTeDentro").attr({ disabled: true });
                            $("#txtSerieMDFe").attr({ disabled: true });
                        }
                        else {
                            BuscarSerieEmpresaUsuario();
                            $("#txtSerieCTeFora").attr({ disabled: false });
                            $("#txtSerieCTeDentro").attr({ disabled: false });
                            $("#txtSerieMDFe").attr({ disabled: false });
                        }

                        $("body").data("filiais", empresa.Filiais);
                        RenderizarFiliais(empresa.Filiais);

                        $("#btnCancelar").show();
                        $("#btnSalvar").show();
                        $("#btnReenviarEmail").show();
                        $("#btnGerarPercursos").show();
                        $("#btnReenviarOracle").show();

                        if (!empresa.PossuiLogoSistema) {
                            $("#btnSelecionarLogoSistema").show();
                            $("#btnRemoverLogoSistema").hide();
                        } else {
                            $("#btnSelecionarLogoSistema").hide();
                            $("#btnRemoverLogoSistema").show();
                        }

                        if (!empresa.PossuiCertificado) {
                            $("#btnSelecionarCertificado").show();
                            $("#btnRemoverCertificado").hide();
                            $("#btnDownloadCertificado").hide();
                            $("#txtSenhaCertificado").show();
                        } else {
                            $("#btnSelecionarCertificado").hide();
                            $("#btnRemoverCertificado").show();
                            $("#btnDownloadCertificado").show();
                            $("#txtSenhaCertificado").hide();
                            $("#txtSenhaCertificado").val("");
                        }

                        if (!empresa.PossuiLogoDacte) {
                            $("#btnSelecionarLogo").show();
                            $("#btnRemoverLogo").hide();
                        } else {
                            $("#btnSelecionarLogo").hide();
                            $("#btnRemoverLogo").show();
                        }

                        $("#txtCNPJ").attr({ disabled: true });
                        BuscarPermissoes(empresa);

                        InicializarPlUpload();
                    }
                    else {
                        ConsultarCNPJReceitaWS();//ConsultarDadosCentralizado();//AbrirConsultaReceita();
                    }
                });
            }
        }
        function BuscarDetalhes(empresa) {
            executarRest("/Empresa/ObterDetalhes?callback=?", { Codigo: empresa.Codigo }, function (r) {
                if (r.Sucesso) {
                    var empresa = r.Objeto;
                    $("#hddCodigo").val(empresa.Codigo);
                    $("#txtRazaoSocial").val(empresa.RazaoSocial);
                    $("#txtNomeFantasia").val(empresa.NomeFantasia);
                    $("#txtCNPJ").val(empresa.CNPJ);
                    $("#txtInscricaoEstadual").val(empresa.InscricaoEstadual);
                    $("#txtInscricaoEstadualSubstituicao").val(empresa.InscricaoEstadualSubstituicao);
                    $("#txtInscricaoMunicipal").val(empresa.InscricaoMunicipal);
                    $("#txtCNAE").val(empresa.CNAE);
                    $("#txtSUFRAMA").val(empresa.SUFRAMA);
                    $("#txtCEP").val(empresa.CEP);
                    $("#txtLogradouro").val(empresa.Logradouro);
                    $("#txtComplemento").val(empresa.Complemento);
                    $("#txtNumero").val(empresa.Numero);
                    $("#txtBairro").val(empresa.Bairro);
                    $("#txtTelefone1").val(empresa.Telefone).change();
                    $("#txtTelefone2").val(empresa.Telefone2).change();

                    $("#selUF").val(empresa.SiglaUF);
                    BuscarLocalidades(empresa.SiglaUF, "selLocalidade", empresa.Localidade);

                    $("#txtContato").val(empresa.Contato);
                    $("#txtTelefoneContato").val(empresa.TelefoneContato).change();
                    $("#txtTelefoneContador").val(empresa.TelefoneContador).change();
                    $("#txtNomeContador").val(empresa.NomeContador);
                    $("#txtDataCadastro").val(empresa.DataCadastro);
                    $("#txtDataInicialCertificado").val(empresa.DataInicialCertificado);
                    $("#txtDataFinalCertificado").val(empresa.DataFinalCertificado);
                    $("#txtSerieCertificado").val(empresa.SerieCertificado);
                    $("#txtSenhaCertificado").val(empresa.SenhaCertificado);
                    $("#txtEmails").val(empresa.Emails);
                    $("#chkEmailsStatus").attr("checked", empresa.EmailsStatus);
                    $("#chkPermiteEmissaoDocumentosDestinados").attr("checked", empresa.PermiteEmissaoDocumentosDestinados);
                    $("#chkCobrarDocumentosDestinados").attr("checked", empresa.CobrarDocumentosDestinados);
                    //$("#chkEmitirTodosCTesComoSimples").attr("checked", empresa.EmitirTodosCTesComoSimples);
                    $("#txtEmailsAdministrativos").val(empresa.EmailsAdministrativos);
                    $("#chkEmailsAdministrativosStatus").attr("checked", empresa.EmailsAdministrativosStatus);
                    $("#txtEmailsContador").val(empresa.EmailsContador);
                    $("#chkEmailsContadorStatus").attr("checked", empresa.EmailsContadorStatus);
                    $("#selTipoEmissao").val(empresa.Emissao);
                    $("#selStatusEmissao").val(empresa.StatusEmissao);
                    $("#selStatus").val(empresa.Status);
                    $("#selStatusFinanceiro").val(empresa.StatusFinanceiro);
                    $("#selTipoTransportador").val(empresa.TipoTransportador != 0 ? empresa.TipoTransportador : $("#selTipoTransportador option:first").val());
                    $("#txtRNTRC").val(empresa.RNTRC);
                    $("#selSimplesNacional").val(empresa.SimplesNacional.toString());
                    $("#selRegimeTributarioCTe").val(empresa.RegimeTributarioCTe);
                    $("#selFusoHorario").val(empresa.FusoHorario);
                    $("#selDiaVencimento").val(empresa.DiaVencimentoFatura);
                    $("#hddCodigoEmpresaCobradora").val(empresa.CodigoEmpresaCobradora);
                    $("#txtEmpresaCobradora").val(empresa.DescricaoEmpresaCobradora);
                    $("#hddCodigoPlanoEmissao").val(empresa.CodigoPlanoEmissao);
                    $("#txtPlanoEmissaoCTe").val(empresa.DescricaoPlanoEmissao);
                    $("body").data("contador", empresa.CPFCNPJContador);
                    $("#txtContador").val(empresa.DescricaoContador);
                    $("body").data("empresaAdmin", empresa.CodigoEmpresaAdmin);
                    $("#txtEmpresaAdmin").val(empresa.DescricaoEmpresaAdmin);
                    $("#txtCRCContador").val(empresa.CRCContador);
                    $("#txtObservacao").val(empresa.Observacao);
                    $("#txtSerieCTeFora").val(empresa.SerieCTeFora);
                    $("#txtSerieCTeDentro").val(empresa.SerieCTeDentro);
                    $("#txtSerieMDFe").val(empresa.SerieMDFe);
                    $("#txtTAF").val(empresa.TAF);
                    $("#txtNroRegEstadual").val(empresa.NroRegEstadual);
                    $("#txtCodigoIntegracao").val(empresa.CodigoIntegracao);
                    $("#txtLogStatus").val(empresa.LogStatus);
                    $("#selCertificado").val(empresa.CertificadoA3.toString());

                    if (empresa.seriesCadastradas > 0) {
                        $("#txtSerieCTeFora").attr({ disabled: true });
                        $("#txtSerieCTeDentro").attr({ disabled: true });
                        $("#txtSerieMDFe").attr({ disabled: true });
                    }
                    else {
                        BuscarSerieEmpresaUsuario();
                        $("#txtSerieCTeFora").attr({ disabled: false });
                        $("#txtSerieCTeDentro").attr({ disabled: false });
                        $("#txtSerieMDFe").attr({ disabled: false });
                    }

                    $("body").data("filiais", empresa.Filiais);
                    RenderizarFiliais(empresa.Filiais);

                    $("#btnCancelar").show();
                    $("#btnSalvar").show();
                    $("#btnReenviarEmail").show();
                    $("#btnGerarPercursos").show();
                    $("#btnReenviarOracle").show();

                    if (!empresa.PossuiLogoSistema) {
                        $("#btnSelecionarLogoSistema").show();
                        $("#btnRemoverLogoSistema").hide();
                    } else {
                        $("#btnSelecionarLogoSistema").hide();
                        $("#btnRemoverLogoSistema").show();
                    }

                    if (!empresa.PossuiCertificado) {
                        $("#btnSelecionarCertificado").show();
                        $("#btnRemoverCertificado").hide();
                        $("#btnDownloadCertificado").hide();
                        $("#txtSenhaCertificado").show();
                    } else {
                        $("#btnSelecionarCertificado").hide();
                        $("#btnRemoverCertificado").show();
                        $("#btnDownloadCertificado").show();
                        $("#txtSenhaCertificado").hide();
                        $("#txtSenhaCertificado").val("");
                    }

                    if (!empresa.PossuiLogoDacte) {
                        $("#btnSelecionarLogo").show();
                        $("#btnRemoverLogo").hide();
                    } else {
                        $("#btnSelecionarLogo").hide();
                        $("#btnRemoverLogo").show();
                    }

                    InicializarPlUpload();

                    $("#txtCNPJ").attr({ disabled: true });
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function BuscarDetalhesEnvioCertificado(codigoEmpresa) {
            executarRest("/Empresa/ObterDetalhes?callback=?", { Codigo: codigoEmpresa }, function (r) {
                if (r.Sucesso) {
                    empresa = r.Objeto;
                    $("#hddCodigo").val(empresa.Codigo);
                    $("#txtRazaoSocial").val(empresa.RazaoSocial);
                    $("#txtNomeFantasia").val(empresa.NomeFantasia);
                    $("#txtCNPJ").val(empresa.CNPJ);
                    $("#txtInscricaoEstadual").val(empresa.InscricaoEstadual);
                    $("#txtInscricaoEstadualSubstituicao").val(empresa.InscricaoEstadualSubstituicao);
                    $("#txtInscricaoMunicipal").val(empresa.InscricaoMunicipal);
                    $("#txtCNAE").val(empresa.CNAE);
                    $("#txtSUFRAMA").val(empresa.SUFRAMA);
                    $("#txtCEP").val(empresa.CEP);
                    $("#txtLogradouro").val(empresa.Logradouro);
                    $("#txtComplemento").val(empresa.Complemento);
                    $("#txtNumero").val(empresa.Numero);
                    $("#txtBairro").val(empresa.Bairro);
                    $("#txtTelefone1").val(empresa.Telefone).change();
                    $("#txtTelefone2").val(empresa.Telefone2).change();

                    $("#selUF").val(empresa.SiglaUF);
                    BuscarLocalidades(empresa.SiglaUF, "selLocalidade", empresa.Localidade);

                    $("#txtContato").val(empresa.Contato);
                    $("#txtTelefoneContato").val(empresa.TelefoneContato).change();
                    $("#txtTelefoneContador").val(empresa.TelefoneContador).change();
                    $("#txtNomeContador").val(empresa.NomeContador);
                    $("#txtDataInicialCertificado").val(empresa.DataInicialCertificado);
                    $("#txtDataFinalCertificado").val(empresa.DataFinalCertificado);
                    $("#txtSerieCertificado").val(empresa.SerieCertificado);
                    $("#txtSenhaCertificado").val(empresa.SenhaCertificado);
                    $("#txtEmails").val(empresa.Emails);
                    $("#chkEmailsStatus").attr("checked", empresa.EmailsStatus);
                    $("#chkPermiteEmissaoDocumentosDestinados").attr("checked", empresa.PermiteEmissaoDocumentosDestinados);
                    $("#chkCobrarDocumentosDestinados").attr("checked", empresa.CobrarDocumentosDestinados);
                    //$("#chkEmitirTodosCTesComoSimples").attr("checked", empresa.EmitirTodosCTesComoSimples);
                    $("#txtEmailsAdministrativos").val(empresa.EmailsAdministrativos);
                    $("#chkEmailsAdministrativosStatus").attr("checked", empresa.EmailsAdministrativosStatus);
                    $("#txtEmailsContador").val(empresa.EmailsContador);
                    $("#chkEmailsContadorStatus").attr("checked", empresa.EmailsContadorStatus);
                    $("#selTipoEmissao").val(empresa.Emissao);
                    $("#selStatusEmissao").val(empresa.StatusEmissao);
                    $("#selStatus").val(empresa.Status);
                    $("#selStatusFinanceiro").val(empresa.StatusFinanceiro);
                    $("#selTipoTransportador").val(empresa.TipoTransportador != 0 ? empresa.TipoTransportador : $("#selTipoTransportador option:first").val());
                    $("#txtRNTRC").val(empresa.RNTRC);
                    $("#selSimplesNacional").val(empresa.SimplesNacional.toString());
                    $("#selRegimeTributarioCTe").val(empresa.RegimeTributarioCTe);
                    $("#selFusoHorario").val(empresa.FusoHorario);
                    $("#selDiaVencimento").val(empresa.DiaVencimentoFatura);
                    $("#hddCodigoEmpresaCobradora").val(empresa.CodigoEmpresaCobradora);
                    $("#txtEmpresaCobradora").val(empresa.DescricaoEmpresaCobradora);
                    $("#hddCodigoPlanoEmissao").val(empresa.CodigoPlanoEmissao);
                    $("#txtPlanoEmissaoCTe").val(empresa.DescricaoPlanoEmissao);
                    $("body").data("contador", empresa.CPFCNPJContador);
                    $("#txtContador").val(empresa.DescricaoContador);
                    $("body").data("empresaAdmin", empresa.CodigoEmpresaAdmin);
                    $("#txtEmpresaAdmin").val(empresa.DescricaoEmpresaAdmin);
                    $("#txtCRCContador").val(empresa.CRCContador);
                    $("#txtObservacao").val(empresa.Observacao);
                    $("#txtSerieCTeFora").val(empresa.SerieCTeFora);
                    $("#txtSerieCTeDentro").val(empresa.SerieCTeDentro);
                    $("#txtSerieMDFe").val(empresa.SerieMDFe);
                    $("#txtTAF").val(empresa.TAF);
                    $("#txtNroRegEstadual").val(empresa.NroRegEstadual);
                    $("#txtCodigoIntegracao").val(empresa.CodigoIntegracao);
                    $("#txtLogStatus").val(empresa.LogStatus);
                    $("#selCertificado").val(empresa.CertificadoA3.toString());

                    if (empresa.seriesCadastradas > 0) {
                        $("#txtSerieCTeFora").attr({ disabled: true });
                        $("#txtSerieCTeDentro").attr({ disabled: true });
                        $("#txtSerieMDFe").attr({ disabled: true });
                    }
                    else {
                        BuscarSerieEmpresaUsuario();
                        $("#txtSerieCTeFora").attr({ disabled: false });
                        $("#txtSerieCTeDentro").attr({ disabled: false });
                        $("#txtSerieMDFe").attr({ disabled: false });
                    }

                    $("body").data("filiais", empresa.Filiais);
                    RenderizarFiliais(empresa.Filiais);

                    $("#btnCancelar").show();
                    $("#btnSalvar").show();
                    $("#btnReenviarEmail").show();
                    $("#btnGerarPercursos").show();
                    $("#btnReenviarOracle").show();

                    if (!empresa.PossuiLogoSistema) {
                        $("#btnSelecionarLogoSistema").show();
                        $("#btnRemoverLogoSistema").hide();
                    } else {
                        $("#btnSelecionarLogoSistema").hide();
                        $("#btnRemoverLogoSistema").show();
                    }

                    if (!empresa.PossuiCertificado) {
                        $("#btnSelecionarCertificado").show();
                        $("#btnRemoverCertificado").hide();
                        $("#btnDownloadCertificado").hide();
                        $("#txtSenhaCertificado").show();
                    } else {
                        $("#btnSelecionarCertificado").hide();
                        $("#btnRemoverCertificado").show();
                        $("#btnDownloadCertificado").show();
                        $("#txtSenhaCertificado").hide();
                        $("#txtSenhaCertificado").val("");
                    }

                    if (!empresa.PossuiLogoDacte) {
                        $("#btnSelecionarLogo").show();
                        $("#btnRemoverLogo").hide();
                    } else {
                        $("#btnSelecionarLogo").hide();
                        $("#btnRemoverLogo").show();
                    }

                    jAlert("Certificado enviado com sucesso!", "Sucesso", function () { $.fancybox.close(); });
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function GerarPercursos() {
            executarRest("/Empresa/GerarPercursos?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Percursos gerados com Sucesso!", "Sucesso");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function AtualiarOracle() {
            executarRest("/Empresa/AtualizarEmpresasOracle?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Empresas atualizadas com Sucesso!", "Sucesso");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function apenasNumeros(string) {
            var numsStr = string.replace(/[^0-9]/g, '');
            return numsStr;
        }
    </script>
    <script defer="defer" type="text/javascript">
        var countLogo;
        $(document).ready(function () {
            $("#btnSelecionarLogo").click(function () {
                InicializarPlUploadLogo();
                AbrirPlUpload();
            });
            $("#btnSelecionarLogoSistema").click(function () {
                InicializarPlUploadLogoSistema();
                AbrirPlUpload();
            });
            $("#btnRemoverLogo").click(function () {
                RemoverLogo();
            });
            $("#btnRemoverLogoSistema").click(function () {
                RemoverLogoSistema();
            });
        });
        function InicializarPlUploadLogo() {
            countLogo = 0;
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/Empresa/SalvarLogo?callback=?&Codigo=' + $("#hddCodigo").val(),
                max_file_size: '500kb',
                unique_names: true,
                filters: [{ title: 'Imagens BMP', extensions: 'bmp' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    FileUploaded: function (up, file, info) {
                        var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                        if (retorno.Sucesso) {
                            $("#btnSelecionarLogo").hide();
                            $("#btnRemoverLogo").show();
                            jAlert("Logo salva com sucesso.", "Sucesso", function () { $.fancybox.close(); });
                        } else {
                            jAlert(retorno.Erro, "Atenção");
                        }
                    },
                    FilesAdded: function (up, files) {
                        countLogo += files.length;
                        if (countLogo > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo de logo. Remova os demais!', 'Atenção');
                        }
                    },
                    FilesRemoved: function (up, files) {
                        countLogo -= files.length;
                        if (countLogo <= 1) {
                            $(".plupload_start").css("display", "");
                        }
                    }
                }
            });
        }
        function RemoverLogo() {
            jConfirm("Deseja realmente remover a logo da DACTE? <b>Este processo é irreversível!</b>", "Atenção", function (retorno) {
                if (retorno) {
                    executarRest("/Empresa/DeletarLogo?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            $("#btnSelecionarLogo").show();
                            $("#btnRemoverLogo").hide();
                            ExibirMensagemSucesso("Logo da DACTE removida com sucesso!", "Sucesso");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }
        function InicializarPlUploadLogoSistema() {
            countLogo = 0;
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/Empresa/SalvarLogoDoSistema?callback=?&Codigo=' + $("#hddCodigo").val(),
                max_file_size: '200kb',
                unique_names: true,
                filters: [{ title: 'Imagens PNG', extensions: 'png' }],
                resize: { width: 200, height: 30, quality: 90 },
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    FileUploaded: function (up, file, info) {
                        var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                        if (retorno.Sucesso) {
                            $("#btnSelecionarLogoSistema").hide();
                            $("#btnRemoverLogoSistema").show();
                            jAlert("Logo salva com sucesso.", "Sucesso", function () { $.fancybox.close(); });
                        } else {
                            jAlert(retorno.Erro, "Atenção");
                        }
                    },
                    FilesAdded: function (up, files) {
                        countLogo += files.length;
                        if (countLogo > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo de logo. Remova os demais!', 'Atenção');
                        }
                    },
                    FilesRemoved: function (up, files) {
                        countLogo -= files.length;
                        if (countLogo <= 1) {
                            $(".plupload_start").css("display", "");
                        }
                    }
                }
            });
        }
        function RemoverLogoSistema() {
            jConfirm("Deseja realmente remover a logo do sistema? <b>Este processo é irreversível!</b>", "Atenção", function (retorno) {
                if (retorno) {
                    executarRest("/Empresa/DeletarLogoSistema?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            $("#btnSelecionarLogoSistema").show();
                            $("#btnRemoverLogoSistema").hide();
                            ExibirMensagemSucesso("Logo do sistema removida com sucesso!", "Sucesso");
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
                }
            });
        }
    </script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            ObterFormularios();
            BuscarSerieEmpresaUsuario();
        });
        function BuscarPermissoes(empresa) {
            executarRest("/ConfiguracaoEmpresa/ObterDetalhes?callback=?", { Codigo: empresa.CodigoCriptografado }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto) {
                        PreencherPermissoes(r.Objeto.Permissoes);
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function ObterFormularios() {
            executarRest("/Pagina/BuscarPorTipoAcesso?callback=?", { TipoAcesso: $("#hddCodigo").val() }, function (r) {
                if (r.Sucesso) {
                    RenderizarFormularios(r.Objeto);
                    TodasPermissoes();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarFormularios(formularios) {
            $("#divPermissoesPaginas").html("");
            for (var i = 0; i < formularios.length; i++) {
                var html = "<div class='divPermissoes'>";
                html += "<div id='divHeaderPermissoes_" + formularios[i].Grupo.replace(/\s/g, "") + "' class='divHeaderPermissoes'>";
                html += "<input type='checkbox' id='chkCheckUncheck_" + formularios[i].Grupo.replace(/\s/g, "") + "' onclick='SelecionarTodosDoGrupo(this);' /><span>" + formularios[i].Grupo + "</span>";
                html += "</div><div class='divBodyPermissoes' id='divBodyPermissoes_" + formularios[i].Grupo.replace(/\s/g, "") + "' style='display: none;'><table><theader><tr>";
                html += "<th>Página</th>";
                html += "<th style='width: 60px; text-align: center;'>Acesso</th>";
                html += "<th style='width: 60px; text-align: center;'>Incluir</th>";
                html += "<th style='width: 60px; text-align: center;'>Alterar</th>";
                html += "<th style='width: 60px; text-align: center;'>Excluir</th></tr></theader><tbody>";
                for (var j = 0; j < formularios[i].Paginas.length; j++) {
                    html += "<tr class='tr_pagina_permissao' id='tr_" + formularios[i].Paginas[j].Codigo + "'>";
                    html += "<td>" + formularios[i].Paginas[j].Descricao + "</td>";
                    html += "<td style='text-align: center;'><input type='checkbox' class='chkPermissaoAcesso' onclick='AlterarEstadoAcesso(this);' id='chkAcesso_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkIncluir_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkAlterar_" + formularios[i].Paginas[j].Codigo + "' /></td>";
                    html += "<td style='text-align: center;'><input type='checkbox' disabled='disabled' id='chkExcluir_" + formularios[i].Paginas[j].Codigo + "' /></td></tr>";
                }
                html += "</tbody></div></div>";
                $("#divPermissoesPaginas").append(html);
                $("#divHeaderPermissoes_" + formularios[i].Grupo.replace(/\s/g, "")).click(function (e) {
                    AlterarEstado(this);
                    e.stopPropagation();
                });
                $("#chkCheckUncheck_" + formularios[i].Grupo.replace(/\s/g, "")).click(function (e) {
                    SelecionarTodosDoGrupo(this);
                    e.stopPropagation();
                });
            }
        }
        function SelecionarTodosDoGrupo(chk) {
            var grupo = chk.id.split("_")[1];
            $('#divBodyPermissoes_' + grupo + ' input[type=checkbox]').each(function () {
                if (this.id.indexOf('chkIncluir_') > -1 || this.id.indexOf('chkAlterar_') > -1 || this.id.indexOf('chkExcluir_') > -1)
                    $(this).attr('disabled', !chk.checked);
                $(this).attr('checked', chk.checked);
            });
        }
        function AlterarEstado(div) {
            var id = div.id.split('_')[1];
            if ($("#divBodyPermissoes_" + id).css("display") == "none") {
                $("#divBodyPermissoes_" + id).slideDown();
            } else {
                $("#divBodyPermissoes_" + id).slideUp();
            }
        }
        function ObterPermissoes() {
            var permissoes = new Array();
            $("#divPermissoesPaginas .tr_pagina_permissao").each(function () {
                var id = this.id.split('_')[1];
                permissoes.push({
                    Codigo: id,
                    Acesso: $("#chkAcesso_" + id)[0].checked,
                    Incluir: $("#chkIncluir_" + id)[0].checked,
                    Alterar: $("#chkAlterar_" + id)[0].checked,
                    Excluir: $("#chkExcluir_" + id)[0].checked
                });
            });
            return permissoes;
        }
        function PreencherPermissoes(permissoes) {
            for (var i = 0; i < permissoes.length; i++) {
                $("#chkAcesso_" + permissoes[i].Codigo).attr("checked", permissoes[i].Acesso);
                $("#chkIncluir_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Incluir, disabled: !permissoes[i].Acesso });
                $("#chkAlterar_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Alterar, disabled: !permissoes[i].Acesso });
                $("#chkExcluir_" + permissoes[i].Codigo).attr({ checked: permissoes[i].Excluir, disabled: !permissoes[i].Acesso });
            }
        }
        function LimparPermissoes() {
            $(".chkPermissaoAcesso").each(function () {
                $(this).attr("checked", false);
                AlterarEstadoAcesso(this);
            });
            $(".divBodyPermissoes").each(function () {
                $(this).slideUp();
            });
        }
        function TodasPermissoes() {
            $(".chkPermissaoAcesso").each(function () {
                $(this).attr("checked", true);
                AlterarEstadoAcesso(this);
            });
            $(".divBodyPermissoes").each(function () {
                $(this).slideUp();
            });
        }

        function AlterarEstadoAcesso(chk) {
            var id = chk.id.split('_')[1];
            if (!chk.checked) {
                $("#chkIncluir_" + id).attr({ disabled: true, checked: false });
                $("#chkAlterar_" + id).attr({ disabled: true, checked: false });
                $("#chkExcluir_" + id).attr({ disabled: true, checked: false });
            } else {
                $("#chkIncluir_" + id).attr({ disabled: false, checked: true });
                $("#chkAlterar_" + id).attr({ disabled: false, checked: true });
                $("#chkExcluir_" + id).attr({ disabled: false, checked: true });
            }
        }
    </script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#btnReenviarEmail").click(function () {
                ReenviarDadosAcesso();
            });
        });
        function ReenviarDadosAcesso() {
            jConfirm("Deseja realmente reenviar os dados de acesso para esta empresa?", "Atenção", function (resp) {
                if (resp) {
                    executarRest("/Empresa/ReenviarInformacoesDeLogin?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Dados de acesso enviados com sucesso.", "Sucesso");
                        } else {
                            ExibirMensagemErro(r.Erro);
                        }
                    });
                }
            });
        }
    </script>
    <script type="text/javascript" id="ScriptFusoHorario">
        function ObterFusosHorarios() {
            executarRest("/FusoHorario/ObterListaDeFusos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarFusosHorarios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarFusosHorarios(fusos) {
            var select = document.getElementById("selFusoHorario");
            for (var i = 0; i < fusos.length; i++) {
                var option = document.createElement("option");
                option.text = fusos[i].DisplayName;
                option.value = fusos[i].Id;
                select.appendChild(option);
            }
        }
    </script>
    <script id="DadosFinanceiros" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDePlanosDeEmissao("btnBuscarPlanoDeEmissao", "btnBuscarPlanoDeEmissao", "A", RetornoConsultaPlanosEmissao, true, false);
            CarregarConsultaDeEmpresas("btnBuscarEmpresaCobradora", "btnBuscarEmpresaCobradora", "A", RetornoConsultaEmpresasCobradoras, true, false);

            $("#txtPlanoEmissaoCTe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPlanoEmissao").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtEmpresaCobradora").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmpresaCobradora").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
        });

        function RetornoConsultaEmpresasCobradoras(empresa) {
            $("#hddCodigoEmpresaCobradora").val(empresa.Codigo);
            $("#txtEmpresaCobradora").val(empresa.CNPJ + " - " + empresa.NomeFantasia);
        }

        function RetornoConsultaPlanosEmissao(plano) {
            $("#txtPlanoEmissaoCTe").val(plano.Descricao);
            $("#hddCodigoPlanoEmissao").val(plano.Codigo);
        }

        function LimparDadosFinanceiros() {
            $("#hddCodigoEmpresaCobradora").val('0');
            $("#txtEmpresaCobradora").val('');
            $("#hddCodigoPlanoEmissao").val('0');
            $("#txtPlanoEmissaoCTe").val('');
            $("#selDiaVencimento").val($("#selDiaVencimento option:first").val());
        }
    </script>
    <script id="ScriptFiliais" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeEmpresas("btnBuscarFilial", "btnBuscarFilial", "A", RetornoConsultaFilial, true, false);

            $("#btnSalvarFilial").click(function () {
                SalvarFilial();
            });

            $("#btnCancelarFilial").click(function () {
                LimparCamposFilial();
            });

            $("#btnExcluirFilial").click(function () {
                ExcluirFilial();
            });
        });

        function ValidarDadosFilial() {
            var filial = $("body").data("filial");
            var valido = true;

            if (filial == null || filial.Codigo <= 0) {
                CampoComErro("#txtFilial");
                valido = false;
            } else {
                valido = true;
            }

            return valido;
        }

        function SalvarFilial() {
            if (ValidarDadosFilial()) {
                var filial = $("body").data("filial");

                var dados = {
                    Codigo: filial.Codigo,
                    CNPJ: filial.CNPJ,
                    NomeFantasia: filial.NomeFantasia,
                    RazaoSocial: filial.RazaoSocial
                };

                var filiais = $("body").data("filiais") == null ? new Array() : $("body").data("filiais");

                for (var i = 0; i < filiais.length; i++)
                    if (filiais[i].Codigo == dados.Codigo) {
                        ExibirMensagemErro("A filial já está vinculada a esta empresa!", "Atenção!");
                        return;
                    }

                filiais.push(dados);

                $("body").data("filiais", filiais);

                RenderizarFiliais(filiais);

                LimparCamposFilial();
            } else {
                ExibirMensagemAlerta("", "Atenção!");
            }
        }

        function RenderizarFiliais(filiais) {
            $("#tblFiliais tbody").html("");

            if (filiais != null && filiais.length > 0)
                for (var i = 0; i < filiais.length; i++)
                    $("#tblFiliais tbody").append("<tr><td>" + filiais[i].CNPJ + "</td><td>" + filiais[i].NomeFantasia + "</td><td>" + filiais[i].RazaoSocial + "</td><td><a href='javascript:void(0);' onclick='EditarFilial(" + JSON.stringify(filiais[i]) + ");'>Editar</a></td></tr>");

            if ($("#tblFiliais tbody").html() == "")
                $("#tblFiliais tbody").append("<td colspan='4'>Nenhum registro encontrado!</td>");
        }

        function EditarFilial(filial) {
            $("#txtFilial").val(filial.CNPJ + " - " + filial.RazaoSocial);
            $("body").data("filial", filial);

            $("#btnExcluirFilial").show();
        }

        function ExcluirFilial() {
            var filial = $("body").data("filial");
            var filiais = $("body").data("filiais");

            for (var i = 0; i < filiais.length; i++) {
                if (filiais[i].Codigo == filial.Codigo) {
                    filiais.splice(i, 1);
                    break;
                }
            }

            $("body").data("filiais", filiais);

            RenderizarFiliais(filiais);
            LimparCamposFilial();
            ExibirMensagemSucesso("Exclusão realizada com sucesso!", "Sucesso!");
        }

        function LimparCamposFilial() {
            $("#txtFilial").val("");
            $("body").data("filial", null);
            $("#btnExcluirFilial").hide();
        }

        function RetornoConsultaFilial(filial) {
            $("#txtFilial").val(filial.CNPJ + " - " + filial.RazaoSocial);
            $("body").data("filial", filial);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoPlanoEmissao" value="0" />
        <input type="hidden" id="hddCodigoEmpresaCobradora" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Empresas Emissoras
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            CNPJ*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCNPJ" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="buttons">
                                        <input type="button" id="btnConsultarReceita" value="Consultar CNPJ" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Inscrição Estadual*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtInscricaoEstadual" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Razão Social*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtRazaoSocial" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Data de Cadastro:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataCadastro" disabled="disabled" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Nome Fantasia*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNomeFantasia" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            IE de Subst. Trib.:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtInscricaoEstadualSubstituicao" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Inscrição Municipal:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtInscricaoMunicipal" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            CNAE*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCNAE" maxlength="100" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            SUFRAMA:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSUFRAMA" maxlength="100" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            CEP*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCEP" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Logradouro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtLogradouro" maxlength="100" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Complemento:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtComplemento" maxlength="100" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Número*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNumero" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Bairro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtBairro" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone 1*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefone1" class="maskedInput phone" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone 2:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefone2" class="maskedInput phone" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            UF*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selUF" class="select">
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Município*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selLocalidade" class="select">
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Contato:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtContato" maxlength="80" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Telefone Contato:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefoneContato" class="maskedInput phone" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Nome Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNomeContador" maxlength="200" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Telefone Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTelefoneContador" class="maskedInput phone" />
                                    </div>
                                </div>

                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Data Inicial Cert.*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataInicialCertificado" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Data Final Cert.*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataFinalCertificado" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            CRC Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCRCContador" maxlength="15" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Contador:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtContador" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarContador" value="Buscar" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="checkbox" style="margin-top: 22px;">
                                        <input type="checkbox" id="chkPermiteEmissaoDocumentosDestinados" />
                                        <label>Utiliza NFe Destinada</label>
                                    </div>
                                </div>
                                <div class="field fieldois">
                                    <div class="checkbox" style="margin-top: 22px;">
                                        <input type="checkbox" id="chkCobrarDocumentosDestinados" />
                                        <label>Gerar cobrança NFe Destinada</label>
                                    </div>
                                </div>
                                <%--                                <div class="field fieldois">
                                    <div class="checkbox" style="margin-top: 22px;">
                                        <input type="checkbox" id="chkEmitirTodosCTesComoSimples" />
                                        <label>Emitir todos CTes como Simples Nacional</label>
                                    </div>
                                </div>--%>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Série Certificado*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieCertificado" maxlength="50" value="" />
                                        <input type="text" id="txtRemoverAutoFill2" style="display: none;" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Senha Certificado*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="password" id="txtRemoverAutoFill" value="" style="display: none;" />
                                        <input type="password" id="txtSenhaCertificado" maxlength="50" value="" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Certificado A1:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selCertificado" class="select">
                                            <option value="false">Sim</option>
                                            <option value="true">Não</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldcinco">
                                    <div class="buttons">
                                        <input type="button" id="btnSelecionarCertificado" value="Certificado" />
                                        <input type="button" id="btnRemoverCertificado" value="Remover Certificado" style="display: none;" />
                                        <input type="button" id="btnDownloadCertificado" value="Baixar Certificado" style="display: none;" />
                                        <input type="button" id="btnSelecionarLogo" value="Logo da DACTE" style="display: none;" />
                                        <input type="button" id="btnRemoverLogo" value="Remover Logo da DACTE" style="display: none;" />
                                        <input type="button" id="btnSelecionarLogoSistema" value="Logo do Sistema" style="display: none;" />
                                        <input type="button" id="btnRemoverLogoSistema" value="Remover Logo do Sistema" style="display: none;" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            RNTRC*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtRNTRC" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Simples Nacional*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selSimplesNacional" class="select">
                                            <option value="false">Não</option>
                                            <option value="true">Sim</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Regime Tributário CTe:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selRegimeTributarioCTe" class="select">
                                            <option value="0">Nenhum</option>
                                            <option value="1">Simples Nacional</option>
                                            <option value="2">Simples Nacional, excesso sublimite de Receita Bruta</option>
                                            <option value="3">Regine Normal</option>
                                            <option value="4">Simples Nacional, Microempreendedor Individual MEI</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Fuso Horário*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selFusoHorario" class="select">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Tipo Emissão*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selTipoEmissao" class="select">
                                            <option value="H">Homologação</option>
                                            <option value="P">Produção</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Status Emissão*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selStatusEmissao">
                                            <option value="N">Não Contatou</option>
                                            <option value="P">Pendente</option>
                                            <option value="S">Sistema Web</option>
                                            <option value="C">Call Center</option>
                                            <option value="M">Não Emitente</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Status*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selStatus" class="select">
                                            <option value="A">Ativo</option>
                                            <option value="I">Inativo</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Status Financeiro*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selStatusFinanceiro" class="select">
                                            <option value="N">Normal</option>
                                            <option value="B">Bloqueado</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Tipo do Transportador*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selTipoTransportador" class="select">
                                            <option value="1">ETC</option>
                                            <option value="2">TAC</option>
                                            <option value="3">CTC</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            TAF:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTAF" maxlength="12" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Nº Reg. Estadual:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtNroRegEstadual" maxlength="25" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Cod. Integração:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtCodigoIntegracao" maxlength="50" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmails" maxlength="1000" />
                                    </div>
                                </div>
                                <div class="field fieldquatro" style="margin-top: 22px;">
                                    <div class="checkbox">
                                        <input type="checkbox" id="chkEmailsStatus" />
                                        <label>
                                            Enviar XML Automático
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails Administrativos:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmailsAdministrativos" maxlength="1000" />
                                    </div>
                                </div>
                                <div class="field fieldquatro" style="margin-top: 22px;">
                                    <div class="checkbox">
                                        <input type="checkbox" id="chkEmailsAdministrativosStatus" />
                                        <label>
                                            Enviar XML Automático
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldsete">
                                    <div class="label">
                                        <label>
                                            E-mails do Contador: (Relatório mensal sempre será enviado para este e-mail independente se estiver selecionado "Enviar XML Automático")
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmailsContador" maxlength="1000" />
                                    </div>
                                </div>
                                <div class="field fieldquatro" style="margin-top: 22px;">
                                    <div class="checkbox">
                                        <input type="checkbox" id="chkEmailsContadorStatus" />
                                        <label>
                                            Enviar XML Automático
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Série Padrão CT-e Dentro:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieCTeDentro" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Série Padrão CTe Fora:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieCTeFora" maxlength="20" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Série Padrão MDFe:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtSerieMDFe" maxlength="20" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao" style="margin-bottom: 15px;">
                                <div class="field fieldoito">
                                    <div class="label">
                                        <label>
                                            Observação:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <textarea id="txtObservacao" rows="3" cols="10" style="width: 99.5%" maxlength="2000"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Empresa Admin:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmpresaAdmin" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarEmpresaAdmin" value="Buscar" />
                                    </div>
                                </div>

                            </div>
                            <div class="fieldzao" style="margin-bottom: 15px;">
                                <div class="field fieldoito">
                                    <div class="label">
                                        <label>
                                            Log Status:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <textarea id="txtLogStatus" rows="3" cols="10" style="width: 99.5%" readonly="readonly"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div class="fields-title">
                                <h3>Plano e Informações Financeiras
                                </h3>
                            </div>
                            <div class="fieldzao">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Dia de Vencto.:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <select id="selDiaVencimento" class="select">
                                            <option value="1">1</option>
                                            <option value="2">2</option>
                                            <option value="3">3</option>
                                            <option value="4">4</option>
                                            <option value="5">5</option>
                                            <option value="6">6</option>
                                            <option value="7">7</option>
                                            <option value="8">8</option>
                                            <option value="9">9</option>
                                            <option value="10">10</option>
                                            <option value="11">11</option>
                                            <option value="12">12</option>
                                            <option value="13">13</option>
                                            <option value="14">14</option>
                                            <option value="15">15</option>
                                            <option value="16">16</option>
                                            <option value="17">17</option>
                                            <option value="18">18</option>
                                            <option value="19">19</option>
                                            <option value="20">20</option>
                                            <option value="21">21</option>
                                            <option value="22">22</option>
                                            <option value="23">23</option>
                                            <option value="24">24</option>
                                            <option value="25">25</option>
                                            <option value="26">26</option>
                                            <option value="27">27</option>
                                            <option value="28">28</option>
                                            <option value="29">29</option>
                                            <option value="30">30</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="label">
                                        <label>
                                            Plano de Emissão de CT-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtPlanoEmissaoCTe" />
                                    </div>
                                </div>
                                <div class="field fielddois">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarPlanoDeEmissao" value="Buscar" />
                                    </div>
                                </div>
                            </div>
                            <div class="fieldzao" style="margin-bottom: 15px;">
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            Empresa Cobradora:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmpresaCobradora" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarEmpresaCobradora" value="Buscar" />
                                    </div>
                                </div>
                            </div>
                            <div class="fields-title">
                                <h3>Filiais
                                </h3>
                            </div>
                            <div class="fieldzao">
                                <div class="field fieldseis">
                                    <div class="label">
                                        <label>
                                            Filial:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtFilial" />
                                    </div>
                                </div>
                                <div class="field fieldquatro">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarFilial" value="Buscar" />
                                    </div>
                                </div>
                            </div>
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvarFilial" value="Salvar" />
                                <input type="button" id="btnExcluirFilial" value="Excluir" style="display: none;" />
                                <input type="button" id="btnCancelarFilial" value="Cancelar" />
                            </div>
                            <div class="table" id="divFiliais" style="margin-left: 5px; max-width: 940px; margin-bottom: 15px;">
                                <table id="tblFiliais">
                                    <thead>
                                        <tr>
                                            <th style="width: 15%;">CNPJ
                                            </th>
                                            <th style="width: 35%;">Razão Social
                                            </th>
                                            <th style="width: 35%;">Nome Fantasia
                                            </th>
                                            <th style="width: 15%;">Opções
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td colspan="4">Nenhum registro encontrado!
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="fields-title">
                                <h3>Permissões
                                </h3>
                            </div>
                            <div class="table" id="divPermissoesPaginas" style="margin-left: 5px;">
                            </div>
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvar" value="Salvar" />
                                <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                                <input type="button" id="btnReenviarEmail" value="Reenviar Dados de Acesso" style="display: none;" />
                                <input type="button" id="btnGerarPercursos" value="Gerar Percursos MDFe" style="display: none;" />
                                <input type="button" id="btnReenviarOracle" value="Atualizar Empresas para Emissao" style="display: none;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <div id="divConsultaReceita" style="height: 500px;">
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fieldddois">
                                <div class="label">
                                    <label>
                                        <b>Captcha:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCaptcha" />
                                </div>
                            </div>
                            <div class="field fieldquatro">
                                <div class="buttons">
                                    <input type="button" id="btnCaptchaNFeSefaz" value="Consultar" />
                                </div>
                            </div>
                            <div style="float: left; margin-top: 6px;">
                                <img src="" id="imgCaptcha" style="float: left; margin-top: 6px; border: 1px solid #CCC; width: 260px; height: 80px;" /><a href="javascript:;void(0)" style="float: left; margin-top: 25px; margin-left: 5px;"><i class="fa fa-refresh"></i></a>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="buttons">
                                    <input type="button" id="btnAtualizarCaptchaNFeSefaz" value="Gerar novo Captcha" style="margin-top: -5px;" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <div id="divUploadArquivos">
            Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
        </div>
    </div>
</asp:Content>
