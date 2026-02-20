<%@ Page Title="Cadastro de Serviços de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ServicosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.ServicosDeVeiculos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtDiasAvisoManutencao").mask("9?99");
            $("#txtKMAvisoManutencao").mask("9?99999");
            $("#txtKMTroca").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtDiasTroca").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            CarregarConsultaDeServicosDeVeiculos("default-search", "default-search", "", RetornoConsultaServicoDeVeiculo, true, false);
            CarregarConsultaDePlanosDeContas("btnBuscarPlanoDeConta", "btnBuscarPlanoDeConta", "A", "A", RetornoConsultaPlanoConta, true, false);
            $("#txtPlanoDeConta").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPlanoDeConta").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            LimparCampos();
        });
        function RetornoConsultaPlanoConta(plano) {
            $("#hddCodigoPlanoDeConta").val(plano.Codigo);
            $("#txtPlanoDeConta").val(plano.Conta + " - " + plano.Descricao);
        }
        function RetornoConsultaServicoDeVeiculo(tipo) {
            executarRest("/ServicoDeVeiculo/ObterDetalhes?callback=?", { Codigo: tipo.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigoPlanoDeConta").val(r.Objeto.CodigoPlanoDeConta);
                    $("#txtPlanoDeConta").val(r.Objeto.DescricaoPlanoDeConta);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#selStatus").val(r.Objeto.Status);
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtKMTroca").val(r.Objeto.KMTroca);
                    $("#txtDiasTroca").val(r.Objeto.DiasTroca);
                    $("#txtDiasAvisoManutencao").val(r.Objeto.DiasAvisoManutencao);
                    $("#txtKMAvisoManutencao").val(r.Objeto.KMAvisoManutencao);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function LimparCampos() {
            $("#hddCodigoPlanoDeConta").val("0");
            $("#txtPlanoDeConta").val("");
            $("#txtDescricao").val('');
            $("#txtDiasTroca").val('0');
            $("#txtKMTroca").val('0');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddCodigo").val('0');
            $("#txtDiasAvisoManutencao").val('');
            $("#txtKMAvisoManutencao").val('');
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var valido = true;
            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(),
                    DiasTroca: Globalize.parseInt($("#txtDiasTroca").val()),
                    KMTroca: Globalize.parseInt($("#txtKMTroca").val()),
                    Status: $("#selStatus").val(),
                    DiasAvisoManutencao: $("#txtDiasAvisoManutencao").val(),
                    KMAvisoManutencao: $("#txtKMAvisoManutencao").val(),
                    CodigoPlanoDeConta: $("#hddCodigoPlanoDeConta").val()
                };
                executarRest("/ServicoDeVeiculo/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoPlanoDeConta" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Serviços de Veículos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">KM p/ Troca:
                </span>
                <input type="text" id="txtKMTroca" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dias p/ Troca:
                </span>
                <input type="text" id="txtDiasTroca" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Dias para Aviso de Manutenção">Dias Av. Man.</abbr>:
                </span>
                <input type="text" id="txtDiasAvisoManutencao" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Quilômetros para Aviso de Manutenção">KM Av. Man.</abbr>:
                </span>
                <input type="text" id="txtKMAvisoManutencao" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Plano de Conta:
                </span>
                <input type="text" id="txtPlanoDeConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoDeConta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
