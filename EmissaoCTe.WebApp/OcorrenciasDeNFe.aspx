<%@ Page Title="Cadastros de Ocorrências de NF-es" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OcorrenciasDeNFe.aspx.cs" Inherits="EmissaoCTe.WebApp.OcorrenciasDeNFe" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker",
                           "~/bundle/styles/plupload") %>

        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/statecreator",
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            FormatarCampoDateTime("txtDataDaOcorrencia");

            CarregarConsultaDeTiposDeOcorrenciasDeCTes("btnBuscarOcorrencia", "btnBuscarOcorrencia", RetornoConsultaTipoOcorrenciaCTe, true, false);
            CarregarConsultaDeNFe("btnBuscarNFe", "btnBuscarNFe", "", RetornoConsultaNFe, true, false);

            CarregarConsultaDeOcorrenciasDeNFes("default-search", "default-search", RetornoConsultaOcorrenciaNFe, true, false);

            RemoveConsulta($("#txtOcorrencia, #txtNFe"), function ($this) {
                $this.val("");
                $this.data("Codigo", 0);
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });


            LimparCampos();
        });

        var IdOcorrenciaNFe;

        function RetornoConsultaOcorrenciaNFe(ocorrencia) {
            executarRest("/OcorrenciaDeNFe/ObterDetalhes?callback=?", { Codigo: ocorrencia.Codigo }, function (r) {
                if (r.Sucesso) {
                    IdOcorrenciaNFe = ocorrencia.Codigo;
                    $("#txtOcorrencia").data("Codigo", r.Objeto.CodigoTipoOcorrencia);
                    $("#txtOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);

                    $("#txtNFe").data("Codigo", r.Objeto.CodigoNFe);
                    $("#txtNFe").val(r.Objeto.DescricaoNFe);

                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#btnSalvar").attr("disabled", true);


                    $("#btnEmail").removeClass("disabled");
                    
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaTipoOcorrenciaCTe(tipoOcorrencia) {
            $("#txtOcorrencia").data("Codigo", tipoOcorrencia.Codigo);
            $("#txtOcorrencia").val(tipoOcorrencia.Descricao);
        }

        function RetornoConsultaNFe(nfe) {
            $("#txtNFe").data("Codigo", nfe.Codigo);
            $("#txtNFe").val(nfe.Numero);
        }

        function SetarDadosPadrao() {
            $("#txtDataDaOcorrencia").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
        }

        function LimparCampos() {
            $("#txtOcorrencia").data("Codigo", 0);
            $("#txtOcorrencia").val("");

            $("#txtNFe").data("Codigo", 0);
            $("#txtNFe").val("");

            $("#txtDataDaOcorrencia").val("");
            $("#txtObservacao").val("");
            $("#btnSalvar").attr("disabled", false);
            $("#btnEmail").addClass("disabled");            

            SetarDadosPadrao();
            IdOcorrenciaNFe = 0;
        }

        function ValidarCampos() {
            var codigoNFe = $("#txtNFe").data("Codigo");
            var codigoTipoOcorrencia = $("#txtOcorrencia").data("Codigo");
            var dataOcorrencia = $("#txtDataDaOcorrencia").val();

            var valido = true;
            if (dataOcorrencia != "") {
                CampoSemErro("#txtDataDaOcorrencia");
            } else {
                CampoComErro("#txtDataDaOcorrencia");
                valido = false;
            }
            if (codigoNFe > 0) {
                CampoSemErro("#txtNFe");
            } else {
                CampoComErro("#txtNFe");
                valido = false;
            }
            if (codigoTipoOcorrencia > 0) {
                CampoSemErro("#txtOcorrencia");
            } else {
                CampoComErro("#txtOcorrencia");
                valido = false;
            }
            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    CodigoTipoOcorrencia: $("#txtOcorrencia").data("Codigo"),
                    CodigoNFe: $("#txtNFe").data("Codigo"),
                    DataDaOcorrencia: $("#txtDataDaOcorrencia").val(),
                    Observacao: $("#txtObservacao").val()
                };
                executarRest("/OcorrenciaDeNFe/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        BuscarOcorrencia(r.Objeto.Codigo);//LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }

        function BuscarOcorrencia(codigo) {
            executarRest("/OcorrenciaDeNFe/ObterDetalhes?callback=?", { Codigo: codigo }, function (r) {
                if (r.Sucesso) {
                    IdOcorrenciaNFe = codigo;
                    $("#txtOcorrencia").data("Codigo", r.Objeto.CodigoTipoOcorrencia);
                    $("#txtOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);

                    $("#txtNFe").data("Codigo", r.Objeto.CodigoNFe);
                    $("#txtNFe").val(r.Objeto.DescricaoNFe);

                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#btnSalvar").attr("disabled", true);

                    $("#btnEmail").removeClass("disabled");

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Ocorrências de NF-es
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Data Ocorrência*:
                </span>
                <input type="text" id="txtDataDaOcorrencia" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">NF-e*:
                </span>
                <input type="text" id="txtNFe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarNFe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Ocorrência*:
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
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>

    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>

</asp:Content>
