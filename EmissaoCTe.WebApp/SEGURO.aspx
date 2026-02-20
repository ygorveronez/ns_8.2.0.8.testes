<%@ Page Title="Geração de Arquivos de Integração - SEGURO" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SEGURO.aspx.cs" Inherits="EmissaoCTe.WebApp.SEGURO" %>

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
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#btnGerarSEGURO").click(function () {
                GerarSEGURO();
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data('cpfCnpj', "");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            CarregarLayoutsEDI();

            SetarDadosPadrao();

            CarregarConsultadeClientes("btnBuscarRemetente", "btnBuscarRemetente", RetornoConsultaRemetente, true, false);
        });

        function GerarSEGURO() {
            executarDownload("/SEGURO/Gerar", { DataInicial: $("#txtDataInicial").val(), DataFinal: $("#txtDataFinal").val(), Versao: $("#selVersao").val(), CPFCNPJRemetente: $("#txtRemetente").data('cpfCnpj') });
        }

        function SetarDadosPadrao() {
            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth() - 1, 1), "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 0), "dd/MM/yyyy"));
        }

        function RetornoConsultaRemetente(remetente) {
            $("#txtRemetente").data('cpfCnpj', remetente.CPFCNPJ);
            $("#txtRemetente").val(remetente.CPFCNPJ + " - " + remetente.Nome);
        }

        function CarregarLayoutsEDI() {
            executarRest("/LayoutEDI/BuscarTodosPorTipo?callback=?", { TipoLayout: 4 }, function (r) {
                if (r.Sucesso) {

                    var selVersao = document.getElementById("selVersao");

                    for (var i = 0; i < r.Objeto.length; i++) {

                        var option = document.createElement("option");

                        option.text = r.Objeto[i].Descricao;
                        option.value = r.Objeto[i].Codigo;

                        selVersao.add(option);
                    }

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Geração de Arquivos de Integração - SEGURO
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
                <span class="input-group-addon">Remetente:
                </span>
                <input type="text" id="txtRemetente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarRemetente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarSEGURO" class="btn btn-primary">Gerar SEGURO</button>
</asp:Content>
