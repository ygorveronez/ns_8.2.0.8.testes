<%@ Page Title="Relatório de Ocorrências de CT-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioOcorrenciasCTe.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioOcorrenciasCTe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();
            $("#txtNumeroInicial").mask("9?9999999999999");
            $("#txtNumeroFinal").mask("9?9999999999999");
            $("#txtNumeroNF").mask("9?9999999999999");

            $("#btnGerarRelatorio").click(function () {
                var dados = {
                    Remetente: $("body").data("cpfCnpjRemetente"),
                    Expedidor: $("body").data("cpfCnpjExpedidor"),
                    Recebedor: $("body").data("cpfCnpjRecebedor"),
                    Destinatario: $("body").data("cpfCnpjDestinatario"),
                    Tomador: $("body").data("cpfCnpjTomador"),
                    DataInicial: $("#txtDataInicial").val(),
                    DataFinal: $("#txtDataFinal").val(),
                    NumeroInicial: $("#txtNumeroInicial").val(),
                    NumeroFinal: $("#txtNumeroFinal").val(),
                    TipoOcorrencia: $("#selTipoOcorrencia").val(),
                    TipoRelatorio: $("#selTipoRelatorio").val(),
                    TipoArquivo: $("#selTipoArquivo").val(),
                    CodigoLocalidadeInicio: $("body").data("codigoLocalidadeInicio"),
                    CodigoLocalidadeFim: $("body").data("codigoLocalidadeFim"),
                    CodigoOcorrencia: $("body").data("codigoOcorrencia"),
                    NumeroNF: $("#txtNumeroNF").val()
                };

                executarDownload("/RelatorioOcorrenciasCTe/DownloadRelatorio", dados);
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cpfCnpjRemetente", null);
                        $("#txtCPFCNPJRemetente").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJRemetente").focusout(function () {
                BuscarRemetenteFiltro();
            });

            $("#txtExpedidor").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cpfCnpjExpedidor", null);
                        $("#txtCPFCNPJExpedidor").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJExpedidor").focusout(function () {
                BuscarExpedidorFiltro();
            });

            $("#txtRecebedor").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cpfCnpjRecebedor", null);
                        $("#txtCPFCNPJRecebedor").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJRecebedor").focusout(function () {
                BuscarRecebedorFiltro();
            });

            $("#txtDestinatario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cpfCnpjDestinatario", null);
                        $("#txtCPFCNPJDestinatario").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJDestinatario").focusout(function () {
                BuscarDestinatarioFiltro();
            });

            $("#txtTomador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cpfCnpjTomador", null);
                        $("#txtCPFCNPJTomador").val("");
                    }
                    e.preventDefault();
                }
            });

            $("#txtCPFCNPJTomador").focusout(function () {
                BuscarTomadorFiltro();
            });

            $("#txtLocalidadeInicio").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoLocalidadeInicio", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtLocalidadeFim").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoLocalidadeFim", null);
                    }
                    e.preventDefault();
                }
            });

            $("#txtOcorrencia").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoOcorrencia", null);
                    }
                    e.preventDefault();
                }
            });

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
            CarregarConsultadeClientes("btnBuscarExpedidor", "btnBuscarExpedidor", RetornoConsultaExpedidor, true, false);
            CarregarConsultadeClientes("btnBuscarRecebedor", "btnBuscarRecebedor", RetornoConsultaRecebedor, true, false);
            CarregarConsultadeClientes("btnBuscarDestinatario", "btnBuscarDestinatario", RetornoConsultaDestinatario, true, false);
            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false);
            CarregarConsultaDeLocalidades("btnBuscarLocalidadeInicio", "btnBuscarLocalidadeInicio", RetornoConsultaLocalidadeInicio, true, false);
            CarregarConsultaDeLocalidades("btnBuscarLocalidadeFim", "btnBuscarLocalidadeFim", RetornoConsultaLocalidadeFim, true, false);
            CarregarConsultaDeTiposDeOcorrenciasDeCTes("btnBuscarOcorrencia", "btnBuscarOcorrencia", RetornoConsultaOcorrencia, true, false);

            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(date, "dd/MM/yyyy"));
        });

        function RetornoConsultaOcorrencia(ocorrencia) {
            $("body").data("codigoOcorrencia", ocorrencia.Codigo);
            $("#txtOcorrencia").val(ocorrencia.Descricao);
        }

        function RetornoConsultaLocalidadeInicio(localidade) {
            $("body").data("codigoLocalidadeInicio", localidade.Codigo);
            $("#txtLocalidadeInicio").val(localidade.Descricao + " - " + localidade.UF);
        }

        function RetornoConsultaLocalidadeFim(localidade) {
            $("body").data("codigoLocalidadeFim", localidade.Codigo);
            $("#txtLocalidadeFim").val(localidade.Descricao + " - " + localidade.UF);
        }

        function BuscarRemetenteFiltro() {
            if ($("body").data("cpfCnpjRemetente") != $("#txtCPFCNPJRemetente").val()) {
                var cpfCnpj = $("#txtCPFCNPJRemetente").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("body").data("cpfCnpjRemetente", r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJRemetente").val(r.Objeto.CPF_CNPJ);
                                    $("#txtRemetente").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposRemetenteFiltro();
                                    jAlert("Remetente não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposRemetenteFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposRemetenteFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposRemetenteFiltro();
                }
            }
        }
        function LimparCamposRemetenteFiltro() {
            $("body").data("cpfCnpjRemetente", null);
            $("#txtCPFCNPJRemetente").val("");
            $("#txtRemetente").val("");
        }
        function RetornoConsultaRemetente(remetente) {
            $("body").data("cpfCnpjRemetente", remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.Nome);
            $("#txtCPFCNPJRemetente").val(remetente.CPFCNPJ);
        }

        function BuscarExpedidorFiltro() {
            if ($("body").data("cpfCnpjExpedidor") != $("#txtCPFCNPJExpedidor").val()) {
                var cpfCnpj = $("#txtCPFCNPJExpedidor").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("body").data("cpfCnpjExpedidor", r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJExpedidor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtExpedidor").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposExpedidorFiltro();
                                    jAlert("Expedidor não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposExpedidorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposExpedidorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposExpedidorFiltro();
                }
            }
        }
        function LimparCamposExpedidorFiltro() {
            $("body").data("cpfCnpjExpedidor", null);
            $("#txtCPFCNPJExpedidor").val("");
            $("#txtExpedidor").val("");
        }
        function RetornoConsultaExpedidor(expedidor) {
            $("body").data("cpfCnpjExpedidor", expedidor.CPFCNPJ);
            $("#txtExpedidor").val(expedidor.Nome);
            $("#txtCPFCNPJExpedidor").val(expedidor.CPFCNPJ);
        }

        function BuscarRecebedorFiltro() {
            if ($("body").data("cpfCnpjRecebedor") != $("#txtCPFCNPJRecebedor").val()) {
                var cpfCnpj = $("#txtCPFCNPJRecebedor").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("body").data("cpfCnpjRecebedor", r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJRecebedor").val(r.Objeto.CPF_CNPJ);
                                    $("#txtRecebedor").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposRecebedorFiltro();
                                    jAlert("Recebedor não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposRecebedorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposRecebedorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposRecebedorFiltro();
                }
            }
        }
        function LimparCamposRecebedorFiltro() {
            $("body").data("cpfCnpjRecebedor", null);
            $("#txtCPFCNPJRecebedor").val("");
            $("#txtRecebedor").val("");
        }
        function RetornoConsultaRecebedor(recebedor) {
            $("body").data("cpfCnpjRecebedor", recebedor.CPFCNPJ);
            $("#txtRecebedor").val(recebedor.Nome);
            $("#txtCPFCNPJRecebedor").val(recebedor.CPFCNPJ);
        }

        function BuscarDestinatarioFiltro() {
            if ($("#txtCPFCNPJDestinatario").val() != $("body").data("cpfCnpjDestinatario")) {
                var cpfCnpj = $("#txtCPFCNPJDestinatario").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("body").data("cpfCnpjDestinatario", r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJDestinatario").val(r.Objeto.CPF_CNPJ);
                                    $("#txtDestinatario").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposDestinatarioFiltro();
                                    jAlert("Destinatário não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposDestinatarioFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposDestinatarioFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção!");
                    }
                } else {
                    LimparCamposDestinatarioFiltro();
                }
            }
        }
        function LimparCamposDestinatarioFiltro() {
            $("body").data("cpfCnpjDestinatario", null);
            $("#txtCPFCNPJDestinatario").val("");
            $("#txtDestinatario").val("");
        }
        function RetornoConsultaDestinatario(destinatario) {
            $("body").data("cpfCnpjDestinatario", destinatario.CPFCNPJ);
            $("#txtDestinatario").val(destinatario.Nome);
            $("#txtCPFCNPJDestinatario").val(destinatario.CPFCNPJ);
        }

        function BuscarTomadorFiltro() {
            if ($("#txtCPFCNPJTomador").val() != $("body").data("cpfCnpjTomador")) {
                var cpfCnpj = $("#txtCPFCNPJTomador").val().replace(/[^0-9]/g, '');
                if (cpfCnpj != "") {
                    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
                        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
                            if (r.Sucesso) {
                                if (r.Objeto != null) {
                                    $("body").data("cpfCnpjTomador", r.Objeto.CPF_CNPJ);
                                    $("#txtCPFCNPJTomador").val(r.Objeto.CPF_CNPJ);
                                    $("#txtTomador").val(r.Objeto.Nome);
                                } else {
                                    LimparCamposTomadorFiltro();
                                    jAlert("Destinatário não encontrado.", "Atenção!");
                                }
                            } else {
                                LimparCamposTomadorFiltro();
                                jAlert(r.Erro, "Erro!");
                            }
                        });
                    } else {
                        LimparCamposTomadorFiltro();
                        jAlert("O CPF/CNPJ digitado é inválido!", "Atenção!");
                    }
                } else {
                    LimparCamposTomadorFiltro();
                }
            }
        }
        function LimparCamposTomadorFiltro() {
            $("body").data("cpfCnpjTomador", null);
            $("#txtCPFCNPJTomador").val("");
            $("#txtTomador").val("");
        }
        function RetornoConsultaTomador(tomador) {
            $("body").data("cpfCnpjTomador", tomador.CPFCNPJ);
            $("#txtTomador").val(tomador.Nome);
            $("#txtCPFCNPJTomador").val(tomador.CPFCNPJ);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Ocorrências de CT-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. CT-e Ini.:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Núm. CT-e Fin.:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="selTipoRelatorio" class="form-control">
                    <option value="RelatorioOcorrenciasCTeSumarizado">Sumarizado</option>
                    <option value="RelatorioOcorrenciasCTeCompleto">Completo</option>                    
                    <option value="RelatorioOcorrenciasCTeCompletoPaisagem">Completo Paisagem</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros Adicionais
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Tipo Ocorrência:
                                </span>
                                <select id="selTipoOcorrencia" class="form-control">
                                    <option value="">Todos</option>
                                    <option value="P">Pendente</option>
                                    <option value="F">Final</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Ocorrência:
                                </span>
                                <input type="text" id="txtOcorrencia" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarOcorrencia" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Localidade de início da prestação">Origem</abbr>:
                                </span>
                                <input type="text" id="txtLocalidadeInicio" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarLocalidadeInicio" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">
                                    <abbr title="Localidade de término da prestação">Destino</abbr>:
                                </span>
                                <input type="text" id="txtLocalidadeFim" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarLocalidadeFim" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Rem.:
                                </span>
                                <input type="text" id="txtCPFCNPJRemetente" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Rem.:
                                </span>
                                <input type="text" id="txtRemetente" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Exp.:
                                </span>
                                <input type="text" id="txtCPFCNPJExpedidor" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Exp.:
                                </span>
                                <input type="text" id="txtExpedidor" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarExpedidor" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Receb.:
                                </span>
                                <input type="text" id="txtCPFCNPJRecebedor" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Receb.:
                                </span>
                                <input type="text" id="txtRecebedor" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarRecebedor" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Dest.:
                                </span>
                                <input type="text" id="txtCPFCNPJDestinatario" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Dest.:
                                </span>
                                <input type="text" id="txtDestinatario" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarDestinatario" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-5 col-md-5 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">CPF/CNPJ Toma.:
                                </span>
                                <input type="text" id="txtCPFCNPJTomador" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Nome Toma.:
                                </span>
                                <input type="text" id="txtTomador" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Núm. NF:
                                </span>
                                <input type="text" id="txtNumeroNF" class="form-control maskedInput" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary"><span class="glyphicon glyphicon-print"></span>&nbsp;Gerar Relatório</button>
</asp:Content>
