<%@ Page Title="Entregas do Motorista" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EntregaMotorista.aspx.cs" Inherits="EmissaoCTe.WebApp.EntregaMotorista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                                   "~/bundle/scripts/blockui",
                                                   "~/bundle/scripts/maskedinput",
                                                   "~/bundle/scripts/datatables",
                                                   "~/bundle/scripts/ajax",
                                                   "~/bundle/scripts/gridview",
                                                   "~/bundle/scripts/consulta",
                                                   "~/bundle/scripts/baseConsultas",
                                                   "~/bundle/scripts/mensagens",
                                                   "~/bundle/scripts/validaCampos",
                                                   "~/bundle/scripts/datepicker") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeTiposDeOcorrenciasDeCTes("btnBuscarOcorrencia", "btnBuscarOcorrencia", RetornoConsultaOcorrencia, true, false);

            $("#txtOcorrencia").datepicker();
            $("#txtOcorrencia").mask("99/99/9999");

            $("#txtOcorrencia").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoOcorrencia", null);
                    }
                    e.preventDefault();
                }
            });

            $("#btnAtualizarGridEntregas").click(function () {
                AtualizarGridEntregas();
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                $("#divOcorrenciaEntrega").modal("hide");
            });

            AtualizarGridEntregas();
        });

        function RetornoConsultaOcorrencia(ocorrencia) {
            $("body").data("codigoOcorrencia", ocorrencia.Codigo);
            $("#txtOcorrencia").val(ocorrencia.Descricao);

            $("#txtObservacao").focus();
        }

        function AtualizarGridEntregas() {
            var colunas = new Array();
            colunas[0] = { Descricao: "Ocorrência", Evento: RegistrarOcorrencia };

            CriarGridView("/EntregaMotorista/Consultar?callback=?", { inicioRegistros: 0, Status: $("#selStatus").val() }, "tbl_entregas_table", "tbl_entregas", "tbl_paginacao_entregas", colunas, [0], null);
        }

        function LimparCampos() {
            $("#txtDataOcorrencia").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("body").data("codigoCTe", null);
            $("#txtTituloOcorrenciaEntrega").text("");
            $("body").data("codigoOcorrencia", "");
            $("#txtOcorrencia").val("");
            $("#txtObservacao").val("");
        }

        function RegistrarOcorrencia(entrega) {
            LimparCampos();

            $("body").data("codigoCTe", entrega.data.Codigo);
            $("#txtTituloOcorrenciaEntrega").text("Ocorrência da Entrega Nº " + entrega.data.Numero + " / CT-e Nº " + entrega.data.NumeroCTe);

            $("#divOcorrenciaEntrega").modal({ keyboard: false, backdrop: 'static' });
        }

        function ValidarCampos() {
            var codigoOcorrencia = $("body").data("codigoOcorrencia");
            var dataOcorrencia = $("#txtDataOcorrencia").val();

            var valido = true;

            if (codigoOcorrencia == null || codigoOcorrencia <= 0) {
                CampoComErro("#txtOcorrencia");
                valido = false;
            } else {
                CampoSemErro("#txtOcorrencia");
            }

            if (dataOcorrencia == "") {
                CampoComErro("#txtDataOcorrencia");
                valido = false;
            } else {
                CampoSemErro("#txtDataOcorrencia");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    CodigoCTe: $("body").data("codigoCTe"),
                    CodigoOcorrencia: $("body").data("codigoOcorrencia"),
                    DataOcorrencia: $("#txtDataOcorrencia").val(),
                    Observacao: $("#txtObservacao").val()
                };

                executarRest("/EntregaMotorista/RegistrarOcorrencia?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        AtualizarGridEntregas();
                        $("#divOcorrenciaEntrega").modal("hide");
                        ExibirMensagemSucesso("Ocorrência registrada com sucesso!", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgOcorrenciaEntrega");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!", "placeholder-msgOcorrenciaEntrega");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Entregas do Motorista
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="false">Pendentes</option>
                    <option value="true">Finalizados</option>
                </select>
            </div>
        </div>
    </div>
    <div class="clearfix"></div>
    <button type="button" id="btnAtualizarGridEntregas" class="btn btn-primary"><span class="glyphicon glyphicon-refresh"></span>&nbsp;Atualizar</button>
    <div id="tbl_entregas" class="table-responsive" style="margin-top: 10px;">
    </div>
    <div id="tbl_paginacao_entregas">
    </div>
    <div class="modal fade" id="divOcorrenciaEntrega" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 id="txtTituloOcorrenciaEntrega" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <div id="placeholder-msgOcorrenciaEntrega"></div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
                            <div class="input-group">
                                <span class="input-group-addon">Data*:
                                </span>
                                <input type="text" id="txtDataOcorrencia" class="form-control" />
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                            <div class="input-group">
                                <span class="input-group-addon">Ocor.*:
                                </span>
                                <input type="text" id="txtOcorrencia" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarOcorrencia" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                            <div class="input-group">
                                <span class="input-group-addon">Obs:
                                </span>
                                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSalvar" class="btn btn-primary"><span class="glyphicon glyphicon-ok"></span>&nbsp;Salvar a Ocorrência</button>
                    <button type="button" id="btnCancelar" class="btn btn-default"><span class="glyphicon glyphicon-remove"></span>&nbsp;Voltar à Tela Principal</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
