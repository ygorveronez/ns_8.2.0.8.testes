<%@ Page Title="Relatório de Acertos de Viagens" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioAcertosViagens.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioAcertosViagens" %>

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
            $("#txtDataInicialLancamento").mask("99/99/9999");
            $("#txtDataFinalLancamento").mask("99/99/9999");

            $("#txtDataFinalLancamento").datepicker();
            $("#txtDataInicialLancamento").datepicker();

            RemoveConsulta("#txtVeiculo", function ($this) {
                $this.val("");
                $("body").data("veiculo", 0);
            });
            $("body").data("veiculo", 0);

            RemoveConsulta("#txtMotorista", function ($this) {
                $this.val("");
                $("body").data("motorista", 0);
            });
            $("body").data("motorista", 0);

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("body").data("veiculo"),
                Motorista: $("body").data("motorista"),
                DataInicial: $("#txtDataInicialLancamento").val(),
                DataFinal: $("#txtDataFinalLancamento").val(),
                Situacao: $("#ddlSituacao").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                TipoRelatorio: $("#selTipoRelatorio").val(),
            };

            if (dados.Motorista <= 0 && dados.TipoRelatorio != "AcertosViagens")
                return ExibirMensagemAlerta("É obrigatório selecionar um motorista para esse tipo de relatório.", "Filtro Obrigatório");

            executarDownload("/RelatorioAcertosViagens/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("veiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("motorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Acertos de Viagens
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicialLancamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinalLancamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Situação:
                </span>
                <select id="ddlSituacao" class="form-control">
                    <option value="">Todas</option>
                    <option value="A">Aberto</option>
                    <option value="P">Pendente Pagamento</option>
                    <option value="F">Fechado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="selTipoRelatorio" class="form-control">
                    <option value="AcertosViagens">Agrupado por Acerto</option>
                    <option value="AcertoTotalizadores">Totalizadores</option>
                    <option value="AcertoDetalhadoTotais">Detalhado com Totais</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4">
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
