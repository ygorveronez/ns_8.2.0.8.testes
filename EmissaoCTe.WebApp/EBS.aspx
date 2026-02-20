<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EBS.aspx.cs" Inherits="EmissaoCTe.WebApp.EBS" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#btnGerarEBS").click(function () {
                GerarEBS();
            });

            $("#txtRemetente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data('cpfCnpj', null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            CarregarLayoutsEDI();
            CarregarSeries();

            SetarDadosPadrao();
        });

        function GerarEBS() {
            executarDownload("/EBS/Gerar", {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Versao: $("#selVersao").val(),
                CPFCNPJRemetente: $("#txtRemetente").data('cpfCnpj'),
                Serie: $("#selSerie").val()
            });
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
            executarRest("/LayoutEDI/BuscarTodosPorTipo?callback=?", { TipoLayout: 5 }, function (r) {
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

        function CarregarSeries() {
            executarRest("/EmpresaSerie/BuscarSeriesEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {

                    var selSerie = document.getElementById("selSerie");

                    for (var i = 0; i < r.Objeto.length; i++) {

                        var option = document.createElement("option");

                        option.text = r.Objeto[i].Numero;
                        option.value = r.Objeto[i].Codigo;

                        selSerie.add(option);
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
        <h2>Geração de Arquivos de Integração - EBS
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Versão*:
                </span>
                <select id="selVersao" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Série:
                </span>
                <select id="selSerie" class="form-control">
                    <option value="">Todas</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarEBS" class="btn btn-primary">Gerar EBS</button>
</asp:Content>
