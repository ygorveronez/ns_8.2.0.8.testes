<%@ Page Title="Relatório de Mapa de Pneus do Veículo" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioMapaPneusVeiculo.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioMapaPneusVeiculo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                                               "~/bundle/scripts/maskedinput",
                                               "~/bundle/scripts/datatables",
                                               "~/bundle/scripts/ajax",
                                               "~/bundle/scripts/gridview",
                                               "~/bundle/scripts/consulta",
                                               "~/bundle/scripts/baseConsultas",
                                               "~/bundle/scripts/mensagens",
                                               "~/bundle/scripts/validaCampos",
                                               "~/bundle/scripts/fileDownload") %>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("veiculo", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtPneu").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("pneu", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDePneus("btnBuscarPneu", "btnBuscarPneu", "A", RetornoConsultaPneus, true, false);

        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("body").data("veiculo"),
                Pneu: $("body").data("pneu"),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioMapaPneusVeiculo/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("veiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaPneus(pneu) {
            $("body").data("pneu", pneu.Codigo);
            $("#txtPneu").val(pneu.Serie + " - " + pneu.ModeloPneu);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Mapa de Pneus do Veículo
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Pneu:
                </span>
                <input type="text" id="txtPneu" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPneu" class="btn btn-primary">Buscar</button>
                </span>
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
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
