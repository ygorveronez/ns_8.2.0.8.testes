<%@ Page Title="Relatório de Clientes" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioClientes.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioClientes" %>

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
    <script defer="defer" type="text/javascript">
        var path = "";
        if (document.location.pathname.split("/").length > 1) {
            var paths = document.location.pathname.split("/");
            for (var i = 0; (paths.length - 1) > i; i++) {
                if (paths[i] != "") {
                    path += "/" + paths[i];
                }
            }
        }

        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtDataFinal").datepicker();
            $("#txtDataInicial").datepicker();

            $("#txtPessoa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("pessoa", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });
            
            CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false, "");

            var today = new Date();
            var date = new Date(today);
            date.setDate(today.getDate() - 30);
            $("#txtDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(today, "dd/MM/yyyy"));
        });

        function DownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Cliente: $("body").data("pessoa"),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/Cliente/DownloadRelatorio", dados);
        }

        function RetornoConsultaPessoa(pessoa) {
            $("body").data("pessoa", pessoa.CPFCNPJ);
            $("#txtPessoa").val(pessoa.CPFCNPJ + " - " + pessoa.Nome);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Clientes
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
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">                    
                    <option value="Excel">Excel</option>
                    <option value="PDF">PDF</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
