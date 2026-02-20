<%@ Page Title="Cadastro da Empresa" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmpresasEmissoras.aspx.cs" Inherits="EmissaoCTe.WebApp.EmpresasEmissoras" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker",
                          "~/bundle/styles/plupload") %>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        var path = "";
        var countArquivos = 0;
        $(document).ready(function () {
            if (document.location.pathname.split("/").length > 1) {
                var paths = document.location.pathname.split("/");
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") {
                        path += "/" + paths[i];
                    }
                }
            }
            HeaderAuditoria("Empresa");
            CarregarConsultadeClientes("btnBuscarContador", "btnBuscarContador", RetornoConsultaContador, true, false, "F");

            $("#txtRNTRC").mask("99999999");
            $("#txtCNPJ").mask("99.999.999/9999-99");
            $("#txtCEP").mask("99.999-999");
            $("#txtDataInicialCertificado").mask("99/99/9999");
            $("#txtDataInicialCertificado").datepicker();
            $("#txtDataFinalCertificado").mask("99/99/9999");
            $("#txtDataFinalCertificado").datepicker();
            $("#txtPercentualCredito").priceFormat({ prefix: '', limit: 5 });

            BuscarUFs("selUF");

            $("#selUF").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidade", null);
            });

            $("#selCertificado").change(function () {
                if ($("#selCertificado").val() == "true") {
                    $("#txtSenhaCertificado").val("");
                    $("#txtSenhaCertificado").prop("disabled", true);
                    $("#txtSerieCertificado").val("");
                    $("#txtSerieCertificado").prop("disabled", true);
                }
                else {
                    $("#txtSenhaCertificado").prop("disabled", false);
                    $("#txtSerieCertificado").prop("disabled", false);
                }

            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnExcluir").click(function () {
                Excluir();
            });

            $("#btnSelecionarCertificado").click(function () {
                InicializarPlUpload();
                AbrirPlUpload();
            });

            $("#btnAcessarConfiguracao").click(function () {
                AbrirConfiguracoes();
            });

            $("#btnRemoverCertificado").click(function () {
                RemoverCertificado();
            });

            $("#btnDownloadCertificado").click(function () {
                DownloadCertificado();
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

            ObterDados();
        });

        function RetornoConsultaContador(contador) {
            $("body").data("contador", contador.CPFCNPJ);
            $("#txtContador").val(contador.CPFCNPJ + " - " + contador.Nome);
        }

        function AbrirConfiguracoes() {
            document.location = "ConfiguracoesEmpresasEmissoras.aspx?x=" + $("#hddCodigoCriptografado").val();
        }

        function AbrirPlUpload() {
            $("#modalUploadArquivos").modal("show");
        }

        var erros = "";
        function InicializarPlUpload() {
            countArquivos = 0;
            url = path + '/Empresa/SalvarCertificado?callback=?&Codigo=' + $("#hddCodigo").val() + "&SenhaCertificado=" + $("#txtSenhaCertificado").val();
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: url.replace(/#/g, "%23"),
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
                                ObterDados($("#hddCodigo").val());
                                jAlert("Certificado enviado com sucesso!", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); });
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

        function DownloadCertificado() {
            executarDownload("/Empresa/DownloadCertificado", { Codigo: $("#hddCodigo").val() });
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
            var taf = $("#txtTAF").val().trim();
            var nroRegEstadual = $("#txtNroRegEstadual").val().trim();

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
            if (dataInicialCert == "") {
                CampoComErro("#txtDataInicialCertificado");
                valido = false;
            } else {
                CampoSemErro("#txtDataInicialCertificado");
            }
            if (dataFinalCert == "") {
                CampoComErro("#txtDataFinalCertificado");
                valido = false;
            } else {
                CampoSemErro("#txtDataFinalCertificado");
            }
            if (rntrc == "" || rntrc.length != 8) {
                CampoComErro("#txtRNTRC");
                valido = false;
            } else {
                CampoSemErro("#txtRNTRC");
            }
            if (taf != "" && taf.length != 12) {
                CampoComErro("#txtTAF");
                valido = false;
                ExibirMensagemAlerta("Campo TAF deve conter 12 caracteres.", "Atenção!");
            } else {
                CampoSemErro("#txtTAF");
            }
            if (nroRegEstadual != "" && nroRegEstadual.length != 25) {
                CampoComErro("#txtNroRegEstadual");
                valido = false;
                ExibirMensagemAlerta("Campo Nº Reg. Estadual deve conter 25 caracteres.", "Atenção!");
            } else {
                CampoSemErro("#txtNroRegEstadual");
            }

            return valido;
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
                    InscricaoMunicipal: $("#txtInscricaoMunicipal").val(),
                    InscricaoEstadualSubstituicao: $("#txtInscricaoEstadualSubstituicao").val(),
                    CNAE: $("#txtCNAE").val(),
                    CodigoIntegracao: $("#txtCodigoIntegracao").val(),
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
                    EmailsAdministrativos: $("#txtEmailsAdministrativos").val(),
                    EmailsAdministrativosStatus: $("#chkEmailsAdministrativosStatus")[0].checked,
                    EmailsContador: $("#txtEmailsContador").val(),
                    EmailsContadorStatus: $("#chkEmailsContadorStatus")[0].checked,
                    NomeContador: $("#txtNomeContador").val(),
                    TelefoneContador: $("#txtTelefoneContador").val(),
                    Emissao: $("#selTipoEmissao").val(),
                    TipoTransportador: $("#selTipoTransportador").val(),
                    StatusEmissao: $("#selStatusEmissao").val(),
                    Status: $("#selStatus").val(),
                    RNTRC: $("#txtRNTRC").val(),
                    TAF: $("#txtTAF").val(),
                    NroRegEstadual: $("#txtNroRegEstadual").val(),
                    SimplesNacional: $("#selSimplesNacional").val(),
                    RegimeEspecial: $("#selRegimeEspecial").val(),
                    RegimeTributarioCTe: $("#selRegimeTributarioCTe").val(),
                    PercentualCredito: $("#txtPercentualCredito").val(),
                    FusoHorario: $("#selFusoHorario").val(),
                    Contador: $("body").data("contador"),
                    CRCContador: $("#txtCRCContador").val(),
                    CertificadoA3: $("#selCertificado").val(),
                    COTM: $("#txtCOTM").val()
                };
                executarRest("/Empresa/Salvar?callback=?", empresa, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        if (r.Erro != null && r.Erro != "")
                            jAlert("Mensagem retornada do sistema de integração: <br />" + r.Erro, "Sistema de Integração");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }

        function ObterDados() {
            executarRest("/Empresa/ObterDetalhesDaEmpresaDoUsuario?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    empresa = r.Objeto;
                    HeaderAuditoriaCodigo(empresa.Codigo);
                    $("#hddCodigoCriptografado").val(empresa.CodigoCriptografado);
                    $("#hddCodigo").val(empresa.Codigo);
                    $("#txtRazaoSocial").val(empresa.RazaoSocial);
                    $("#txtNomeFantasia").val(empresa.NomeFantasia);
                    $("#txtCNPJ").val(empresa.CNPJ);
                    $("#txtInscricaoEstadual").val(empresa.InscricaoEstadual);
                    $("#txtInscricaoMunicipal").val(empresa.InscricaoMunicipal);
                    $("#txtInscricaoEstadualSubstituicao").val(empresa.InscricaoEstadualSubstituicao);
                    $("#txtCNAE").val(empresa.CNAE);
                    $("#txtCodigoIntegracao").val(empresa.CodigoIntegracao);
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
                    $("#txtEmailsAdministrativos").val(empresa.EmailsAdministrativos);
                    $("#chkEmailsAdministrativosStatus").attr("checked", empresa.EmailsAdministrativosStatus);
                    $("#txtEmailsContador").val(empresa.EmailsContador);
                    $("#chkEmailsContadorStatus").attr("checked", empresa.EmailsContadorStatus);
                    $("#selTipoEmissao").val(empresa.Emissao);
                    $("#selTipoTransportador").val(empresa.TipoTransportador != 0 ? empresa.TipoTransportador : $("#selTipoTransportador option:first").val());
                    $("#selStatusEmissao").val(empresa.StatusEmissao);
                    $("#selStatus").val(empresa.Status);
                    $("#txtRNTRC").val(empresa.RNTRC);
                    $("#txtTAF").val(empresa.TAF);
                    $("#txtNroRegEstadual").val(empresa.NroRegEstadual);
                    $("#selSimplesNacional").val(empresa.SimplesNacional.toString());
                    $("#selRegimeEspecial").val(empresa.RegimeEspecial);
                    $("#selRegimeTributarioCTe").val(empresa.RegimeTributarioCTe);
                    $("#txtPercentualCredito").val(empresa.PercentualCredito);
                    $("body").data("contador", empresa.CPFCNPJContador);
                    $("#txtContador").val(empresa.DescricaoContador);
                    $("#txtCRCContador").val(empresa.CRCContador);
                    $("#selCertificado").val(empresa.CertificadoA3.toString());
                    $("#txtCOTM").val(empresa.COTM);
                    ObterFusosHorarios(empresa.FusoHorario);
                    $("#btnCancelar").show();

                    if (!empresa.PossuiCertificado) {
                        $("#btnSelecionarCertificado").show();
                        $("#btnRemoverCertificado").hide();
                        $("#btnDownloadCertificado").hide();
                        $("#txtSenhaCertificadoLabel").show();
                        $("#txtSenhaCertificado").show();
                    }
                    else {
                        $("#btnSelecionarCertificado").hide();
                        $("#btnRemoverCertificado").show();
                        $("#btnDownloadCertificado").show();
                        $("#txtSenhaCertificadoLabel").hide();
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
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

    </script>
    <script defer="defer" type="text/javascript">
        var errosEnvioXMLCTe, ctesImportados, countLogo, errosEnvioXMLMDFe, mdfesImportados;
        $(document).ready(function () {
            $("#btnImportarCTe").click(function () {
                InicializarPlUploadCTe();
                AbrirPlUpload();
            });
            $("#btnImportarMDFe").click(function () {
                InicializarPlUploadMDFe();
                AbrirPlUpload();
            });
            $("#btnSelecionarLogo").click(function () {
                InicializarPlUploadLogo();
                AbrirPlUpload();
            });
            $("#btnRemoverLogo").click(function () {
                RemoverLogo();
            });
        });
        function InicializarPlUploadCTe() {
            errosEnvioXMLCTe = "";
            ctesImportados = new Array();
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/ConhecimentoDeTransporteEletronico/GerarCTeAnterior?callback=?',
                max_file_size: '500kb',
                unique_names: true,
                filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    FileUploaded: function (up, file, info) {
                        var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                        if (retorno.Sucesso) {
                            ctesImportados.push(retorno.Objeto);
                        } else {
                            errosEnvioXMLCTe += retorno.Erro + "<br />";
                        }
                    },
                    StateChanged: function (up) {
                        if (up.state != plupload.STARTED) {
                            if (errosEnvioXMLCTe.trim() != "") {
                                jAlert("Ocorreram as seguintes falhas na importação dos arquivos: <br /><br />" + errosEnvioXMLCTe + "<br />", "Atenção");
                            } else {
                                jAlert("CT-es importados com sucesso!", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); });
                            }
                        }
                    }
                }
            });
        }
        function InicializarPlUploadMDFe() {
            errosEnvioXMLMDFe = "";
            mdfesImportados = new Array();
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/ManifestoEletronicoDeDocumentosFiscais/GerarMDFeAnterior?callback=?',
                max_file_size: '500kb',
                unique_names: true,
                filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    FileUploaded: function (up, file, info) {
                        var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                        if (retorno.Sucesso) {
                            mdfesImportados.push(retorno.Objeto);
                        } else {
                            errosEnvioXMLMDFe += retorno.Erro + "<br />";
                        }
                    },
                    StateChanged: function (up) {
                        if (up.state != plupload.STARTED) {
                            if (errosEnvioXMLMDFe.trim() != "") {
                                jAlert("Ocorreram as seguintes falhas na importação dos arquivos: <br /><br />" + errosEnvioXMLMDFe + "<br />", "Atenção");
                            } else {
                                jAlert("MDF-es importados com sucesso!", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); });
                            }
                        }
                    }
                }
            });
        }
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
                            jAlert("Logo salva com sucesso.", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); });
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
    </script>
    <script type="text/javascript" id="ScriptFusoHorario">
        function ObterFusosHorarios(fuso) {
            executarRest("/FusoHorario/ObterListaDeFusos?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    RenderizarFusosHorarios(r.Objeto);

                    if (fuso != null)
                        $("#selFusoHorario").val(empresa.FusoHorario);

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoCriptografado" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro da Empresa
        </h2>
    </div>
    <div class="btn-group" style="margin-bottom: 15px;">
        <button data-toggle="dropdown" class="btn btn-default dropdown-toggle">Opções da Empresa <span class="caret"></span></button>
        <ul class="dropdown-menu" role="menu">
            <li><a id="btnAcessarConfiguracao" tabindex="-1" href="javascript:void(0);">Configurações Gerais</a></li>
            <li><a id="btnImportarCTe" tabindex="-1" href="javascript:void(0);">Importar CT-es</a></li>
            <li><a id="btnImportarMDFe" tabindex="-1" href="javascript:void(0);">Importar MDF-es</a></li>
            <li id="btnSelecionarCertificado" style="display: none;"><a tabindex="-1" href="javascript:void(0);">Enviar Certificado Digital</a></li>
            <li id="btnRemoverCertificado" style="display: none;"><a tabindex="-1" href="javascript:void(0);">Remover Certificado Digital</a></li>
            <li id="btnDownloadCertificado" style="display: none;"><a tabindex="-1" href="javascript:void(0);">Baixar Certificado Digital</a></li>
            <li id="btnSelecionarLogo" style="display: none;"><a tabindex="-1" href="javascript:void(0);">Enviar Logo da DACTE</a></li>
            <li id="btnRemoverLogo" style="display: none;"><a tabindex="-1" href="javascript:void(0);">Remover Logo da DACTE</a></li>
        </ul>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Razão Social*:
                </span>
                <input type="text" id="txtRazaoSocial" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Nome Fantasia*:
                </span>
                <input type="text" id="txtNomeFantasia" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CNPJ*:
                </span>
                <input type="text" id="txtCNPJ" class="form-control maskedInput" maxlength="80" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Inscrição Estadual">IE*</abbr>:
                </span>
                <input type="text" id="txtInscricaoEstadual" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Inscrição Municipal">IM*</abbr>:
                </span>
                <input type="text" id="txtInscricaoMunicipal" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Inscrição Estadual de Substituição Tributária">IE de ST</abbr>:
                </span>
                <input type="text" id="txtInscricaoEstadualSubstituicao" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Classificação Nacional de Atividades Econômicas">CNAE</abbr>:
                </span>
                <input type="text" id="txtCNAE" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código de integração para controle gerencial">Cód. Integração</abbr>:
                </span>
                <input type="text" id="txtCodigoIntegracao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Superintendência da Zona Franca de Manaus">SUFRAMA</abbr>:
                </span>
                <input type="text" id="txtSUFRAMA" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CEP:
                </span>
                <input type="text" id="txtCEP" class="form-control maskedInput" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Logradouro*:
                </span>
                <input type="text" id="txtLogradouro" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Complemento:
                </span>
                <input type="text" id="txtComplemento" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número*:
                </span>
                <input type="text" id="txtNumero" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Bairro*:
                </span>
                <input type="text" id="txtBairro" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone 1*:
                </span>
                <input type="text" id="txtTelefone1" class="form-control maskedInput phone" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone 2:
                </span>
                <input type="text" id="txtTelefone2" class="form-control maskedInput phone" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">UF*:
                </span>
                <select id="selUF" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Município*:
                </span>
                <select id="selLocalidade" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Contato:
                </span>
                <input type="text" id="txtContato" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tel. Contato:
                </span>
                <input type="text" id="txtTelefoneContato" class="form-control maskedInput phone" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Contador:
                </span>
                <input type="text" id="txtNomeContador" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tel. Contador:
                </span>
                <input type="text" id="txtTelefoneContador" class="form-control maskedInput phone" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">CRC Contador:
                </span>
                <input type="text" id="txtCRCContador" class="form-control" maxlength="15" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Contador:
                </span>
                <input type="text" id="txtContador" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarContador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data Inicial de Validade do Certificado">Dt. Ini. Cert.*</abbr>:
                </span>
                <input type="text" id="txtDataInicialCertificado" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data Final de Validade do Certificado">Dt. Fin. Cert.*</abbr>:
                </span>
                <input type="text" id="txtDataFinalCertificado" class="form-control maskedInput" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Série Cert.*:
                </span>
                <input type="text" id="txtSerieCertificado" class="form-control" maxlength="50" />
                <input type="text" id="txtRemoverAutoFill" style="height: 0px; width: 0px; border: 0px; display: block;" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Senha Cert.*:
                </span>
                <input type="password" id="txtRemoverAutoFill2" style="height: 0px; width: 0px; border: 0px; display: block;" />
                <input type="password" id="txtSenhaCertificado" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Certificado A1:
                </span>
                <select id="selCertificado" class="form-control">
                    <option value="false">Sim</option>
                    <option value="true">Não</option>
                </select>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Registro Nacional de Transportadores Rodoviários de Cargas">RNTRC*</abbr>:
                </span>
                <input type="text" id="txtRNTRC" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Termo de Autorização de Fretamento (12 caracteres)">TAF</abbr>:
                </span>
                <input type="text" id="txtTAF" class="form-control" maxlength="12" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número do Registro junto a Administração Estadual (25 caracteres)">Nº Reg. Estadual</abbr>:
                </span>
                <input type="text" id="txtNroRegEstadual" class="form-control" maxlength="25" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número do Certificado do Operador de Transporte Multimodal">COTM</abbr>:
                </span>
                <input type="text" id="txtCOTM" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Simples Nacional*:
                </span>
                <select id="selSimplesNacional" class="form-control">
                    <option value="false">Não</option>
                    <option value="true">Sim</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Regime Especial*:
                </span>
                <select id="selRegimeEspecial" class="form-control">
                    <option value="0">Nenhum</option>
                    <option value="1">Microempresa Municipal</option>
                    <option value="2">Estimativa</option>
                    <option value="3">Sociedade Profissionais</option>
                    <option value="4">Cooperativa</option>
                    <option value="5">Microempresario Individual</option>
                    <option value="6">Microempresario Empresa P. P.</option>
                    <option value="7">Lucro Real</option>
                    <option value="8">Lucro Presumido</option>
                    <option value="9">Simples Nacional</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Regime Tributário CTe:
                </span>
                <select id="selRegimeTributarioCTe" class="form-control">
                    <option value="0">Nenhum</option>
                    <option value="1">Simples Nacional</option>
                    <option value="2">Simples Nacional, excesso sublimite de Receita Bruta</option>
                    <option value="3">Regine Normal</option>
                    <option value="4">Simples Nacional, Microempreendedor Individual MEI</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Percentual Crédito">Perc. Crédito</abbr>:
                </span>
                <input type="text" id="txtPercentualCredito" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Fuso Horário*:
                </span>
                <select id="selFusoHorario" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Emissão*:
                </span>
                <select id="selTipoEmissao" class="form-control">
                    <option value="H">Homologação</option>
                    <option value="P">Produção</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Do Transportador*:
                </span>
                <select id="selTipoTransportador" class="form-control">
                    <option value="1">ETC</option>
                    <option value="2">TAC</option>
                    <option value="3">CTC</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status Emissão*:
                </span>
                <select id="selStatusEmissao" class="form-control" disabled="disabled">
                    <option value="N">Não Contatou</option>
                    <option value="P">Pendente</option>
                    <option value="S">Sistema Web</option>
                    <option value="C">Call Center</option>
                    <option value="M">Não Emitente</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control" disabled="disabled">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="E-mails da Empresa">@ Empresa</abbr>:
                </span>
                <input type="text" id="txtEmails" class="form-control" maxlength="1000" />
            </div>
        </div>
        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkEmailsStatus" />
                        Enviar XML Automático
                    </label>
                </div>
            </div>
        </div>
        <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="E-mails Administrativos">@ Admin</abbr>:
                </span>
                <input type="text" id="txtEmailsAdministrativos" class="form-control" maxlength="1000" />
            </div>
        </div>
        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkEmailsAdministrativosStatus" />
                        Enviar XML Automático
                    </label>
                </div>
            </div>
        </div>
        <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="E-mails do Contador (Relatório mensal sempre será enviado para este e-mail independente se estiver selecionado 'Enviar XML Automático')">@ Contador</abbr>:
                </span>
                <input type="text" id="txtEmailsContador" class="form-control" maxlength="1000" />
            </div>
        </div>
        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkEmailsContadorStatus" />
                        Enviar XML Automático
                    </label>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>

    <div class="modal fade" id="modalUploadArquivos" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Upload de Arquivos</h4>
                </div>
                <div class="modal-body">
                    <div id="divUploadArquivos">
                        Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
