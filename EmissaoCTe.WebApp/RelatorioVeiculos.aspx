<%@ Page Title="Relatório de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioVeiculos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
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
            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });
        });

        function DownloadRelatorio() {
            var dados = {
                TipoPropriedade: $("#selTipoPropriedade").val(),
                TipoVeiculo: $("#selTipoVeiculo").val(),
                TipoRodado: $("#selTipoRodado").val(),
                TipoCarroceria: $("#selCarroceria").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                TipoRelatorio: $("#selTipoRelatorio").val()
            };

            executarDownload("/RelatorioVeiculos/DownloadRelatorio", dados);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Veículos
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Propriedade*:
                </span>
                <select id="selTipoPropriedade" class="form-control">
                    <option value="">Todos</option>
                    <option value="P">Próprio</option>
                    <option value="T">Terceiros</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Veíc.*:
                </span>
                <select id="selTipoVeiculo" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Tração</option>
                    <option value="1">Reboque</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Rodado*:
                </span>
                <select id="selTipoRodado" class="form-control">
                    <option value="">Todos</option>
                    <option value="00">Não Aplicado</option>
                    <option value="01">Truck </option>
                    <option value="02">Toco </option>
                    <option value="03">Cavalo </option>
                    <option value="04">Van </option>
                    <option value="05">Utilitário </option>
                    <option value="06">Outros </option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Carroc.*:
                </span>
                <select id="selCarroceria" class="form-control">
                    <option value="">Todos</option>
                    <option value="00">Não Aplicado</option>
                    <option value="01">Aberta </option>
                    <option value="02">Fechada/Baú </option>
                    <option value="03">Granel </option>
                    <option value="04">Porta Container</option>
                    <option value="05">Sider</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo:
                </span>
                <select id="selTipoRelatorio" class="form-control">
                    <option value="RelatorioVeiculos">Resumido</option>
                    <option value="RelatorioVeiculosCompleto">Completo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
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
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary"><span class="glyphicon glyphicon-print"></span>&nbsp;Gerar Relatório</button>
</asp:Content>
