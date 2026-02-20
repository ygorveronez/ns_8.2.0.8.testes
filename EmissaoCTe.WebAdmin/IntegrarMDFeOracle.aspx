<%@ Page Title="Integrar MDFe Oracle" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegrarMDFeOracle.aspx.cs" Inherits="EmissaoCTe.WebAdmin.IntegrarMDFeOracle" %>

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
            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data("codigo", null);
                    }

                    e.preventDefault();
                }
            });

            $("#txtDataInicio").datepicker();
            $("#txtDataInicio").mask("99/99/9999");

            $("#txtDataFim").datepicker();
            $("#txtDataFim").mask("99/99/9999");

            LimparCampos();

            $("#btnEnviar").click(function () {
                EnviarMDFe();
            });

            $("#btnEnviarCTe").click(function () {
                EnviarCTe();
            });

            $("#btnEnviarCTeCancelado").click(function () {
                EnviarCTeCancelado();
            });

            $("#btnConsultarCTeOracle").click(function () {
                ConsultarCTeOracle();
            });            

            $("#btnConsultarNotasDestinadas").click(function () {
                ConsultarNotasDestinadas();
            });

            $("#btnExtrairXMLParaDiretorio").click(function () {
                ExtrairXMLParaDiretorio();
            });

            $("#btnBuscarXMLOracle").click(function () {
                BuscarXMLOracle();
            });

            $("#btnCancelarCTesPorCSV").click(function () {
                InicializarPlUploadCSV();
                AbrirPlUploadCSV();
            });
        });

        function AbrirPlUploadCSV() {
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
        function InicializarPlUploadCSV() {
            countArquivos = 0;
            erros = "";
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + 'ConhecimentoDeTransporteEletronico/CancelarCTesPorCSV?callback=?&CodigoEmpresa=0',
                max_file_size: '2000kb',
                unique_names: true,
                filters: [{ title: 'Arquivos CSV', extensions: 'csv' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    StateChanged: function (up) {
                        if (up.state != plupload.STARTED) {
                            if (erros != "") {
                                jAlert("Não foi possível enviar arquivo: " + erros, "Atenção");
                            }
                        }
                    },
                    FilesAdded: function (up, files) {
                        countArquivos += files.length;
                        if (countArquivos > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo. Remova os demais!', 'Atenção');
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

        function RetornoConsultaEmpresa(empresa) {
            $("#txtEmpresa").val(empresa.NomeFantasia);
            $("#txtEmpresa").data("codigo", empresa.Codigo);
        }

        function ConsultarNotasDestinadas() {
            var dados = {
                Empresa: $("#txtEmpresa").data("codigo"),
            };

            if (dados.Empresa == null || dados.Empresa == 0)
                return ExibirMensagemAlerta("Informe a Empresa", "Validação");

            executarRest("/DestinadosNFes/ConsultarNFesDestinadasAdmin?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                    LimparCampos();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function EnviarMDFe() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ManifestoEletronicoDeDocumentosFiscais/IntegrareMDFeOracle?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function EnviarCTe() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ConhecimentoDeTransporteEletronico/IntegrareCTeOracle?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function ConsultarCTeOracle() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ConhecimentoDeTransporteEletronico/ConsultarCTeOracle?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function EnviarCTeCancelado() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ConhecimentoDeTransporteEletronico/IntegrareCTeCanceladoOracle?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function ExtrairXMLParaDiretorio() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ConhecimentoDeTransporteEletronico/ExtrairXMLParaDiretorio?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function BuscarXMLOracle() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    DataInicio: $("#txtDataInicio").val(),
                    DataFim: $("#txtDataFim").val()
                };
                executarRest("/ConhecimentoDeTransporteEletronico/BuscarXMLOracle?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function LimparCampos() {
            $("#txtEmpresa").data("codigo", null);
            $("#txtEmpresa").val("");
            $("#txtDataInicio").val('');
            $("#txtDataInicio").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataFim").val('');
            $("#txtDataFim").val(Globalize.format(new Date(), "dd/MM/yyyy"));
        }

        function ValidarDados() {
            var codigoEmpresa = $("#txtEmpresa").data("codigo");
            var dataInicio = $("#txtDataInicio").val();
            var dataFim = $("#txtDataFim").val();
            var valido = true;

            //if (codigoEmpresa == null || codigoEmpresa <= 0) {
            //    CampoComErro("#txtEmpresa");
            //    valido = false;
            //} else {
            //    CampoSemErro("#txtEmpresa");
            //}

            if (dataInicio == null || dataInicio.length <= 0) {
                CampoComErro("#txtDataInicio");
                valido = false;
            } else {
                CampoSemErro("#txtDataInicio");
            }

            if (dataFim == null || dataFim.length <= 0) {
                CampoComErro("#txtDataFim");
                valido = false;
            } else {
                CampoSemErro("#txtDataFim");
            }

            return valido;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Integrar MDFe Oracle
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
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
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Empresa*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmpresa" />
                                    </div>
                                </div>
                                <div class="field fieldum" style="width: 65px;">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data Inicio*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataInicio" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data Fim*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataFim" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div id="divArquivosSelecionados" class="fieldzao">
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnEnviar" value="Enviar MDFe" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnEnviarCTe" value="Enviar CTe" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnConsultarCTeOracle" value="Consultar CTes Enviados" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnEnviarCTeCancelado" value="Enviar CTe Cancelado" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnConsultarNotasDestinadas" value="Consultar Notas Destinadas" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnExtrairXMLParaDiretorio" value="Extrair XML para diretório" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnBuscarXMLOracle" value="Buscar XML CTe Oracle" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnCancelarCTesPorCSV" value="Cancelar CTes por CSV" />
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
