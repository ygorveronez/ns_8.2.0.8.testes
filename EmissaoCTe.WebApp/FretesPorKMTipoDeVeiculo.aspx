<%@ Page Title="Frete por KM por Tipo de Veículo" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="FretesPorKMTipoDeVeiculo.aspx.cs" Inherits="EmissaoCTe.WebApp.FretesPorKMTipoDeVeiculo" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        var CodigoEdicao = 0;
        $(function () {
            CarregarConsultaDeTiposDeVeiculos("btnBuscarTipoVeiculo", "btnBuscarTipoVeiculo", "A", RetornoConsultaTipoDeVeiculo, true, false);
            CarregarConsultaDeFretesPorKMTipoDeVeiculo("default-search", "default-search", "", RetornoConsultaFrete, true, false);

            RemoveConsulta("#txtTipoVeiculo", function ($this) {
                $this.val("");
                $this.data("Codigo", 0);
            });

            $("#txtKMFranquia").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtValor, #txtExcedentePorKM").priceFormat();

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaTipoDeVeiculo(tipo) {
            $("#txtTipoVeiculo").val(tipo.Descricao);
            $("#txtTipoVeiculo").data("Codigo", tipo.Codigo);
        }

        function LimparCampos() {
            CodigoEdicao = 0;

            $("#txtTipoVeiculo").val('');
            $("#txtTipoVeiculo").data("Codigo", 0);


            $("#txtKMFranquia").val("0");
            $("#txtValor, #txtExcedentePorKM").val("0,00");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipoCalculo").val($("#selTipoCalculo option:first").val());

            $(".has-error").removeClass("has-error");
        }

        function ValidarCampos() {
            var valido = true;

            if ($("#txtTipoVeiculo").data("Codigo") == 0) {
                CampoComErro("#txtTipoVeiculo");
                valido = false;
            } else {
                CampoSemErro("#txtTipoVeiculo");
            }

            if (Globalize.parseFloat($("#txtValor").val()) <= 0) {
                CampoComErro("#txtValor");
                valido = false;
            } else {
                CampoSemErro("#txtValor");
            }

            if (Globalize.parseInt($("#txtKMFranquia").val()) <= 0) {
                CampoComErro("#txtKMFranquia");
                valido = false;
            } else {
                CampoSemErro("#txtKMFranquia");
            }

            if (Globalize.parseFloat($("#txtExcedentePorKM").val()) <= 0) {
                CampoComErro("#txtExcedentePorKM");
                valido = false;
            } else {
                CampoSemErro("#txtExcedentePorKM");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {

                var dados = {
                    Codigo: CodigoEdicao,
                    TipoVeiculo: $("#txtTipoVeiculo").data("Codigo"),
                    Valor: $("#txtValor").val(),
                    KMFranquia: $("#txtKMFranquia").val(),
                    ExcedentePorKM: $("#txtExcedentePorKM").val(),
                    TipoCalculo: $("#selTipoCalculo").val(),
                    Status: $("#selStatus").val(),
                };

                executarRest("/FretePorKMTipoDeVeiculo/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function RetornoConsultaFrete(frete) {
            executarRest("/FretePorKMTipoDeVeiculo/ObterDetalhes?callback=?", { Codigo: frete.Codigo }, function (r) {
                if (r.Sucesso) {
                    CodigoEdicao = r.Objeto.Codigo;

                    $("#txtTipoVeiculo").data("Codigo", r.Objeto.TipoVeiculo.Codigo).val(r.Objeto.TipoVeiculo.Descricao);
                    $("#txtValor").val(r.Objeto.Valor);
                    $("#txtKMFranquia").val(r.Objeto.KMFranquia);
                    $("#txtExcedentePorKM").val(r.Objeto.ExcedentePorKM);
                    $("#selTipoCalculo").val(r.Objeto.TipoCalculo);
                    $("#selStatus").val(r.Objeto.Status);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Frete por KM por Tipo de Veículo
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Tipo Veículo*:
                </span>
                <input type="text" id="txtTipoVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 hide">
            <div class="input-group">
                <span class="input-group-addon">Tipo Cálculo*:
                </span>
                <select id="selTipoCalculo" class="form-control">
                    <option value="1">Acerto</option>
                    <option value="2">CT-e</option>
                </select>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Valor*:
                </span>
                <input type="text" id="txtValor" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">KM da Franquia*:
                </span>
                <input type="text" id="txtKMFranquia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4">
            <div class="input-group">
                <span class="input-group-addon">Excedente Por KM:
                </span>
                <input type="text" id="txtExcedentePorKM" class="form-control" />
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
