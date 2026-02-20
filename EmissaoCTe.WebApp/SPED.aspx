<%@ Page Title="SPED Fiscal" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SPED.aspx.cs" Inherits="EmissaoCTe.WebApp.SPED" %>

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
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();
            $("#txtDataFinal").on('change', CarregarValorAtoCOTEPE);
            $("#btnGerarSPED").click(function () {
                GerarSPED();
            });
            SetarDadosPadrao();
        });
        function CarregarValorAtoCOTEPE() {
            const dateValue = $("#txtDataFinal").val();

            if (!dateValue) return;

            const dateParts = dateValue.split(/[-/]/);

            if (dateParts.length !== 3) return;

            let dateObj;

            if (dateParts[0].length === 4) {
                dateObj = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
            } else {
                dateObj = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
            }

            if (isNaN(dateObj.getTime())) return;

            const year = dateObj.getFullYear();

            if (isNaN(year)) return;

            const selectElement = $('#selAtoCOTEPE');

            switch (year) {
                case 2012: selectElement.val('006'); break;
                case 2013: selectElement.val('007'); break;
                case 2014: selectElement.val('008'); break;
                case 2015: selectElement.val('009'); break;
                case 2016: selectElement.val('010'); break;
                case 2017: selectElement.val('011'); break;
                case 2018: selectElement.val('012'); break;
                case 2019: selectElement.val('013'); break;
                case 2020: selectElement.val('014'); break;
                case 2021: selectElement.val('015'); break;
                case 2022: selectElement.val('016'); break;
                case 2023: selectElement.val('017'); break;
                case 2024: selectElement.val('018'); break;
                case 2025: selectElement.val('019'); break;
                default: selectElement.val('018'); break;
            }
        };
        function GerarSPED() {
            executarDownload("/SPED/GerarSPED", { DataInicial: $("#txtDataInicial").val(), DataFinal: $("#txtDataFinal").val(), AtoCOTEPE: $("#selAtoCOTEPE").val(), GerarD160: $("#chkGerarD160")[0].checked });
        }
        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
            CarregarValorAtoCOTEPE();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>SPED Fiscal
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Ato COTEPE:
                </span>
                <select id="selAtoCOTEPE" class="form-control">
                    <option value="006">Versão 006 - Ato COTEPE de 2012</option>
                    <option value="007">Versão 007 - Ato COTEPE de 2013</option>
                    <option value="008">Versão 008 - Ato COTEPE de 2014</option>
                    <option value="009" selected="selected">Versão 009 - Ato COTEPE de 2015</option>
                    <option value="010">Versão 010 - Ato COTEPE de 2016</option>
                    <option value="011">Versão 011 - Ato COTEPE de 2017</option>
                    <option value="012">Versão 012 - Ato COTEPE de 2018</option>
                    <option value="013">Versão 013 - Ato COTEPE de 2019</option>
                    <option value="014">Versão 014 - Ato COTEPE de 2020</option>
                    <option value="015">Versão 015 - Ato COTEPE de 2021</option>
                    <option value="016">Versão 016 - Ato COTEPE de 2022</option>
                    <option value="017">Versão 017 - Ato COTEPE de 2023</option>
                    <option value="018">Versão 018 - Ato COTEPE de 2024</option>
                    <option value="019">Versão 019 - Ato COTEPE de 2025</option>
                </select>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="checkbox-inline">
                <label>
                    <input type="checkbox" id="chkGerarD160" />
                    Gerar D160 e derivados
                </label>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarSPED" class="btn btn-primary" style="margin-top: 10px;">Gerar SPED</button>
</asp:Content>
