<%@ Page Title="Geração de Arquivos de Integração - OCOREN NF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OCORENNFe.aspx.cs" Inherits="EmissaoCTe.WebApp.OCORENNFe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
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
            $("#txtDataInicial, #txtDataFinal")
                .mask("99/99/9999")
                .datepicker();

            $("#btnGerarOCOREN").click(function () {
                GerarOCOREN();
            });

            RemoveConsulta("#txtRemetente", function ($this) {
                $this.data('codigo', '').val('');
            });

            CarregarLayoutsEDI();
            SetarDadosPadrao();

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
        });

        function GerarOCOREN() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Versao: $("#selVersao").val(),
                CPFCNPJRemetente: $("#txtRemetente").data('codigo')
            };

            executarDownload("/OCOREN/GerarNFe", dados);
        }

        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
        }

        function RetornoConsultaRemetente(remetente) {
            $("#txtRemetente").data('codigo', remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.CPFCNPJ + " - " + remetente.Nome);
        }

        function CarregarLayoutsEDI() {
            var OCOREN_NFS = 18; //Enumerador do layout
            executarRest("/LayoutEDI/BuscarTodosPorTipo?callback=?", { TipoLayout: OCOREN_NFS }, function (r) {
                if (r.Sucesso) {
                    var versoes = [];

                    for (var i = 0; i < r.Objeto.length; i++) {
                        versoes.push(
                            $("<option></option>", {value: r.Objeto[i].Codigo})
                            .text(r.Objeto[i].Descricao)
                        );
                    }

                    $("#selVersao").append(versoes);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Geração de Arquivos de Integração - OCOREN NFS-e
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
                <span class="input-group-addon">Versão*:
                </span>
                <select id="selVersao" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Emitente:
                </span>
                <input type="text" id="txtRemetente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarOCOREN" class="btn btn-primary">Gerar OCOREN</button>
</asp:Content>