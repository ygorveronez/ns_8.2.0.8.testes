<%@ Page Title="Relatório de Documentos de Transporte" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioDocumentosTransporte.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioDocumentosTransporte" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
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

            $("#txtDataFinal").datepicker();
            $("#txtDataInicial").datepicker();

            RemoveConsulta($("#txtVeiculo, #txtMotorista"), function ($this) {
                $this.val("");
                $this.data("Codigo", 0);
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false, "J");
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("#txtVeiculo").data("Codigo"),
                Motorista: $("#txtMotorista").data("Codigo"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                NumeroDT: $("#txtNumeroDT").val(),
                NumeroCTe: $("#txtNumeroCTe").val(),
                NumeroNFSe: $("#txtNumeroNFSe").val(),
                NumeroNFe: $("#txtNumeroNFe").val()
            };
            
            executarDownload("/RelatorioDocumentosTransporte/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("#txtVeiculo")
                .val(veiculo.Placa)
                .data("Codigo", veiculo.Codigo);
        }

        function RetornoConsultaMotorista(motorista) {
            $("#txtMotorista")
                .val(motorista.Nome)
                .data("Codigo", motorista.Codigo);
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Documentos de Transporte
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Número DT:
                </span>
                <input type="text" id="txtNumeroDT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Nº CT-e:
                </span>
                <input type="text" id="txtNumeroCTe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Nº NFS-e:
                </span>
                <input type="text" id="txtNumeroNFSe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Nº NF-e:
                </span>
                <input type="text" id="txtNumeroNFe" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
