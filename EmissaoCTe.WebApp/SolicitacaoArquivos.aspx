<%@ Page Title="Solicitação Arquivos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SolicitacaoArquivos.aspx.cs" Inherits="EmissaoCTe.WebApp.SolicitacaoArquivos" %>

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
        $(document).ready(function () {
            $("#txtMes").datepicker( {
                format: "mm-yyyy",
                viewMode: "months", 
                minViewMode: "months"
            });

            var date = new Date();
            $("#txtMes").val(Globalize.format(date, "MM-yyyy"));

            $("#btnSolicitar").click(function () {
                GerarSolicitacao();
            });
        });

        function GerarSolicitacao() {
            if (ValidarSolicitacao()) {
                executarRest("/SolicitacaoArquivos/Solicitar?callback=?", { MesAno: $("#txtMes").val(), Tipo: $("#selTipo").val(), Emails: $("#txtEmails").val() }, function (r) {
                    if (r.Sucesso) {
                        jAlert("Solicitação feita com sucesso!", "Sucesso");
                    } else {
                        jAlert(r.Erro, "Atenção");
                    }
                });
            }
        }

        function ValidarSolicitacao() {
            var emails = $("#txtEmails").val().split(';');
            var mes = $("#txtMes").val();

            var valido = true;

            for (var i = 0; i < emails.length; i++) {
                if (!ValidarEmail(emails[i].trim())) {
                    valido = false;
                    break;
                }
            }
            if (!valido) {
                CampoComErro("#txtEmails");
            } else {
                CampoSemErro("#txtEmails");
            }

            if (mes == "") {
                CampoComErro("#txtMes");
                valido = false;
            }
            else
                CampoSemErro("#txtMes");

            if (!valido)
                jAlert("Campos obrigatórios não informados corretamente.", "Atenção!");

            return valido;
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Solicitação Arquivos
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Mês*:
                </span>
                <input type="text" id="txtMes" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo do arquivo a ser solicitado">Tipo do Arquivo</abbr>*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="3" selected="selected">XMLs CT-e</option>
                    <option value="5">XMLs MDF-e</option>
                    <option value="1">Relatório em PDF CT-e</option>
                    <option value="2">Relatório em Excel CT-e</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-7 col-md-7 col-lg-7">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="E-mails para recebimento, separados por ponto e virgula (;)">E-mails</abbr>:
                </span>
                <asp:TextBox ID="txtEmails" runat="server" MaxLength="1000" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
            </div>
        </div>
    </div>
    <button type="button" id="btnSolicitar" class="btn btn-primary" style="margin-top: 10px;">Solicitar</button>
</asp:Content>
