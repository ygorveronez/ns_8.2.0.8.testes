<%@ Page Title="SINTEGRA" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SINTEGRA.aspx.cs" Inherits="EmissaoCTe.WebApp.SINTEGRA" %>

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

            $("#btnGerarSINTEGRA").click(function () {
                GerarSINTEGRA();
            });

            SetarDadosPadrao();
        });

        function GerarSINTEGRA() {
            executarDownload("/SINTEGRA/Gerar", {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                CodigoEstruturaArquivo: $("#selCodigoEstruturaArquivo").val(),
                CodigoNaturezaOperacoes: $("#selCodigoNaturezaOperacoes").val(),
                CodigoFinalidadeArquivo: $("#selFinalidadeArquivo").val()
            });
        }

        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>SINTEGRA
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
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código da identificação da estrutura do arquivo magnético entregue">Estrutura</abbr>*:
                </span>
                <select id="selCodigoEstruturaArquivo" class="form-control">
                    <option value="1" selected="selected">1 - Estrutura conforme Convênio ICMS 57/95, na versão estabelecida pelo Convênio ICMS 31/99 e com as alterações promovidas até o Convênio ICMS 30/02</option>
                    <option value="2">2 - Estrutura conforme Convênio ICMS 57/95, na versão estabelecida pelo Convênio ICMS 69/02 e com as alterações promovidas pelo Convênio ICMS 142/02</option>
                    <option value="3" selected="selected">3 - Estrutura conforme Convênio ICMS 57/95, com as alterações promovidas pelo Convênio ICMS 76/03</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código da identificação da natureza das operações informadas">Natureza</abbr>*:
                </span>
                <select id="selCodigoNaturezaOperacoes" class="form-control">
                    <option value="1" selected="selected">1 - Interestaduais somente operações sujeitas ao regime de Substituição Tributária</option>
                    <option value="2">2 - Interestaduais – operações com ou sem Substituição Tributária</option>
                    <option value="3" selected="selected">3 - Totalidade das operações do informante</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Código da finalidade do arquivo magnético">Finalidade</abbr>*:
                </span>
                <select id="selFinalidadeArquivo" class="form-control">
                    <option value="1" selected="selected">1 - Normal</option>
                    <option value="2">2 - Retificação total de arquivo: substituição total de informações prestadas pelo contribuinte referentes a este período</option>
                    <option value="3">3 - Retificação aditiva de arquivo: acréscimo de informação não incluída em arquivos já apresentados</option>
                    <option value="4">4 - Desfazimento: arquivo de informação referente a operações/prestações não efetivadas</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarSINTEGRA" class="btn btn-primary" style="margin-top: 10px;">Gerar SINTEGRA</button>
</asp:Content>
