<%@ Page Title="Relatório de Cartas de Correções" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCCs.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioCCs" %>
<%@ Import Namespace="System.Web.Optimization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/filedownload",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            FormatarCampoDate("txtDataInicialCCe");
            FormatarCampoDate("txtDataFinalCCe");
            FormatarCampoDate("txtDataInicialCTe");
            FormatarCampoDate("txtDataFinalCTe");

            $("#txtNumeroInicialCTe").mask("9?9999");
            $("#txtNumeroFinalCTe").mask("9?9999");
            $("#txtSerieCTe").mask("9?9999");

            var date = new Date();
            $("#txtDataInicialCCe").val(Globalize.format(date, "dd/MM/yyyy"));
            $("#txtDataFinalCCe").val(Globalize.format(date, "dd/MM/yyyy"));

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });
        });

        function DownloadRelatorio() {
            var dados = {
                StatusCCe: $("#selStatusCCe").val(),
                SerieCTe: $("#txtSerieCTe").val(),
                DataInicialCCe: $("#txtDataInicialCCe").val(),
                DataFinalCCe: $("#txtDataFinalCCe").val(),
                DataInicialCTe: $("#txtDataInicialCTe").val(),
                DataFinalCTe: $("#txtDataFinalCTe").val(),
                NumeroInicialCTe: $("#txtNumeroInicialCTe").val(),
                NumeroFinalCTe: $("#txtNumeroFinalCTe").val(),
                TipoArquivo: "PDF"
            };

            executarDownload("/CartaDeCorrecaoEletronica/DownloadRelatorio", dados);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Cartas de Correções</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Status CC-e:
                </span>
                <select id="selStatusCCe" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Em Digitação</option>
                    <option value="1">Pendente</option>
                    <option value="2">Enviado</option>
                    <option value="3">Autorizado</option>
                    <option value="9">Rejeição</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Série CT-e</span>
                <input type="text" id="txtSerieCTe" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data inicial da emissão da CC-e">Dt. Inicial CC-e</abbr>:
                </span>
                <input type="text" id="txtDataInicialCCe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data final da emissão da CC-e">Dt. Final CC-e</abbr>:
                </span>
                <input type="text" id="txtDataFinalCCe" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data inicial da emissão do CT-e">Dt. Inicial CT-e</abbr>:
                </span>
                <input type="text" id="txtDataInicialCTe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data final da emissão do CT-e">Dt. Final CT-e</abbr>:
                </span>
                <input type="text" id="txtDataFinalCTe" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Numero inicial do CT-e">Número Inicial CT-e</abbr>:
                </span>
                <input type="text" id="txtNumeroInicialCTe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Numero final do CT-e">Número Final CT-e</abbr>:
                </span>
                <input type="text" id="txtNumeroFinalCTe" class="form-control" />
            </div>
        </div>
    </div>

    <button type="button" id="btnGerarRelatorio" style="margin-top: 15px;" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>

