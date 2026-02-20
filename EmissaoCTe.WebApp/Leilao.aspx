<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" ClientIDMode="Static" AutoEventWireup="true" CodeBehind="Leilao.aspx.cs" Inherits="EmissaoCTe.WebApp.Leilao" %>
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
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnAtualizarGrid").click(function () {
                AtualizarGridLeilao();
            });
            $("#btnAtualizarLance").click(function () {
                AtualizarLance();
            });
            AtualizarGridLeilao();
            $("#txtValoLance").priceFormat();

            $("#txtValoLance").blur(function () {
                validarValor();
            });
        });

        var ValorInicial;
        var CodigoParticipante;
        function OfertarClick(e) {
            CodigoParticipante = e.data.Codigo;
            ValorInicial = e.data.valorIncial;
            $("#lblDescricaoCarga").text(e.data.OrigemDestino);
            $("#lblValorInicial").text(e.data.valorIncial);

            AbrirTelaLance();
        }

        function AtualizarGridLeilao() {
            var dados = {
                inicioRegistros: 0
            };
            CriarGridView("/Leilao/Consultar?callback=?", dados, "tbl_Leilao_table", "tbl_leilao", "tbl_paginacao_leilao", [{ Descricao: "Ofertar", Evento: OfertarClick }], [0]);
        }

        function validarValor() {
            if (Globalize.parseFloat(ValorInicial) < Globalize.parseFloat($("#txtValoLance").val())) {
                $("#txtValoLance").val(ValorInicial);
            }
        }

        function AbrirTelaLance() {
            $("#divLance").modal({ keyboard: false, backdrop: 'static' });
        }

        function FecharTelaLance() {
            $("#divLance").modal('hide');
        }

        function AtualizarLance() {
          
            if ($("#txtValoLance").val() != "") {
                jConfirm("Deseja realmente dar o lance no valor de " + $("#txtValoLance").val() + "?", "Confirmar Lance", function (confirm) {
                    if (confirm) {
                        iniciarRequisicao();
                        executarRest("/Leilao/AtualizarOferta?callback=?", { ValorOferta: $("#txtValoLance").val(), Codigo: CodigoParticipante }, function (r) {
                            finalizarRequisicao();
                            if (r.Sucesso) {
                                if (r.Objeto) {
                                    ExibirMensagemSucesso("Lance ofertado com sucesso em caso de vitória do seu lance o embarcador entra-rá em contato!", "Sucesso!");
                                    AtualizarGridLeilao();
                                    $("#txtValoLance").val("");
                                    FecharTelaLance();
                                } else {
                                    ExibirMensagemErro(r.Erro, "Problema!");
                                }

                            } else {
                                ExibirMensagemErro(r.Erro, "Atenção!");
                            }
                        }, false);
                    }
                });
            } else {
                ExibirMensagemErro("É obrigatório informar um valor para seu lance", "Campo Obrigatório");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Fretes disponíveis para leilão
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>

    <button type="button" id="btnAtualizarGrid" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_leilao" class="table-responsive" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_leilao">
    </div>


      <div class="modal fade" id="divLance" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Ofertar lance para o leilão</h4>
                </div>
                <div class="modal-body">
                    <div id="messages-placeholderLance">
                    </div>
                     <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <span id="lblDescricaoCarga"></span>
                        </div>
                      </div>  
                     <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            Valor Mínimo: <b><span id="lblValorInicial"></span></b>
                        </div>
                      </div>  
                    <br />
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Valor do Lance:
                                </span>
                                <input type="text" id="txtValoLance" class="form-control" maxlength="20" />
                            </div>
                        </div>
                      </div>  
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnAtualizarLance" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Dar Lance</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
