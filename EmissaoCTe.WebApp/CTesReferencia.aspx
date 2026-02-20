<%@ Page Title="CTe(s) Referência para emissão de CT-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CTesReferencia.aspx.cs" Inherits="EmissaoCTe.WebApp.CTesReferencia" %>

<%@ Import Namespace="System.Web.Optimization" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeCTesParaReferencia("btnBuscarCTe", "btnBuscarCTe", "", RetornoConsultaCTe, true, false);

            $("#txtCTe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoCTe").val("0");
                    }
                    e.preventDefault();
                }
            });

            $("#btnConsultarCTe").click(function () {
                AtualizarGridCTes();
            });

            $("#btnSalvar").click(function () {
                AdicionarCTe();
            });
            
            LimparCampos();
            AtualizarGridCTes();
        });

        function AtualizarGridCTes() {

            var dados = {
                CodigoCTe: 0,
            }

            var opcoes = new Array();
            opcoes.push({ Descricao: "Remover", Evento: RemoverCTe });

            CriarGridView("/CTesReferencia/Consultar?callback=?", dados, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", opcoes, [0], null, [0], 20);
        }

        function AdicionarCTe() {
            if (ValidarCampos()) {

                var dados = {
                    CodigoCTe: $("#hddCodigoCTe").val()
                }

                executarRest("/CTesReferencia/AdicionarCTeReferencia?callback=?", dados, function (r) {
                    if (r.Sucesso) {

                        ExibirMensagemSucesso("CT-e referência salvo com sucesso!", "Sucesso!");
                        LimparCampos();
                        AtualizarGridCTes();                        
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function RemoverCTe(cte) {

                var dados = {
                    CodigoCTe: cte.data.Codigo
                }

                executarRest("/CTesReferencia/RemoverCTeReferencia?callback=?", dados, function (r) {
                    if (r.Sucesso) {

                        ExibirMensagemSucesso("CT-e referência removido com sucesso!", "Sucesso!");
                        LimparCampos();
                        AtualizarGridCTes();                        
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });

        }

        function RetornoConsultaCTe(cte) {
            $("#hddCodigoCTe").val(cte.Codigo);
            $("#txtCTe").val(cte.Numero + "-" + cte.Serie);
        }

        function ValidarCampos() {
            var codigoCTe = $("#hddCodigoCTe").val();
            var valido = true;

            if ($("#hddCodigoCTe").val() == "0" || $("#hddCodigoCTe").val() == "") {
                CampoComErro("#txtCTe");
                valido = false;
            } else {
                CampoSemErro("#txtCTe");
            }

            return valido;
        }

        function LimparCampos() {
            $("body").data("codigoVeiculo", null);
            $("#txtCTe").val("");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigoCTe" value="0" />
    </div>
    <div class="page-header">
        <h2>CT-e(s) Referência para emissão de CT-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>

    <div class="panel-group" id="adicionarCTe" style="margin-bottom: 10px; margin-top: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle btn-block" data-toggle="collapse" data-parent="#adicionarCTe" href="#adicionar">Adicionar CT-e Referência
                    </a>
                </h4>
            </div>
            <div id="adicionar" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">CT-e:
                                </span>
                                <input type="text" id="txtCTe" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <button type="button" id="btnSalvar" class="btn btn-success">Salvar </button>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <button type="button" id="btnConsultarCTe" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_ctes" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_ctes">
    </div>

</asp:Content>
