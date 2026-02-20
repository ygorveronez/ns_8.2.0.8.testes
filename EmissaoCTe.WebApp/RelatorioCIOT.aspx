<%@ Page Title="Relatório de CIOT" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCIOT.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioCIOT" %>
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
            FormatarCampoDate("txtDataInicialCIOT");
            FormatarCampoDate("txtDataFinalCIOT");

            CarregarConsultadeClientes("txtTransportador", "btnBuscarTransportador", CallBackConsulta("#txtTransportador", "CPFCNPJ", "Nome"), true, false);
            CarregarConsultaDeMotoristas("txtMotorista", "btnBuscarMotorista", CallBackConsulta("#txtMotorista", "Codigo", "Nome"), true, false);
            CarregarConsultaDeVeiculos("txtVeiculo", "btnBuscarVeiculo", CallBackConsulta("#txtVeiculo", "Codigo", "Placa"), true, false);

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });
        });

        function DownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicialCIOT").val(),
                DataFinal: $("#txtDataFinalCIOT").val(),
                Transportador: ($("#txtTransportador").data('codigo') || '').replace(/[^0-9]/g, ''),
                Motorista: $("#txtMotorista").data('codigo'),
                Veiculo: $("#txtVeiculo").data('codigo'),
                TipoArquivo: "PDF"
            };

            executarDownload("/RelatorioCIOT/DownloadRelatorio", dados);
        }

        function CallBackConsulta(el, cod, desc) {
            var $el = $(el);

            RemoveConsulta($el, function ($this) {
                $this.val('').data('codigo', 0);
            });

            return function (data) {
                console.log(arguments);
                $el.val(data[desc]).data('codigo', data[cod]);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de CIOT</h2>
    </div>
    <div id="messages-placeholder">
    </div>
    
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data inicial da emissão do CIOT">Dt. Inicial CIOT</abbr>:
                </span>
                <input type="text" id="txtDataInicialCIOT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data final da emissão do CIOT">Dt. Final CIOT</abbr>:
                </span>
                <input type="text" id="txtDataFinalCIOT" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Transportador:
                </span>
                <input type="text" id="txtTransportador" class="form-control" autocomplete="off">
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTransportador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" autocomplete="off">
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veiculo
                </span>
                <input type="text" id="txtVeiculo" class="form-control" autocomplete="off">
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>

    <button type="button" id="btnGerarRelatorio" style="margin-top: 15px;" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>


