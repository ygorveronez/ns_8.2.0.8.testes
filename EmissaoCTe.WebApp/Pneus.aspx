<%@ Page Title="Cadastro de Pneus" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Pneus.aspx.cs" Inherits="EmissaoCTe.WebApp.Pneus" %>

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
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtValorCompra").priceFormat({ prefix: '' });
            $("#txtReceitas").priceFormat({ prefix: '' });
            $("#txtCustos").priceFormat({ prefix: '' });
            $("#txtDataCompra").mask("99/99/9999");
            $("#txtDataCompra").datepicker();
            $("#txtDataVenda").mask("99/99/9999");
            $("#txtDataVenda").datepicker();
            CarregarConsultaDePneus("default-search", "default-search", "", RetornoConsultaPneu, true, false);
            CarregarConsultaDeMarcasDePneus("btnBuscarMarca", "btnBuscarMarca", "A", RetornoConsultaMarcaPneu, true, false);
            CarregarConsultaDeModelosDePneus("btnBuscarModelo", "btnBuscarModelo", "A", RetornoConsultaModeloPneu, true, false);
            CarregarConsultaDeStatusDePneus("btnBuscarStatus", "btnBuscarStatus", "A", "A", RetornoConsultaStatusPneu, true, false);
            CarregarConsultaDeDimensoesDePneus("btnBuscarDimensao", "btnBuscarDimensao", "A", RetornoConsultaDimensaoPneu, true, false);
            $("#txtDimensao").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoDimensao").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtStatus").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoStatus").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtModelo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoModelo").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtMarca").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoMarca").val("0");
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
        function RetornoConsultaDimensaoPneu(dimensao) {
            $("#hddCodigoDimensao").val(dimensao.Codigo);
            $("#txtDimensao").val(dimensao.Descricao);
        }
        function RetornoConsultaStatusPneu(status) {
            $("#hddCodigoStatus").val(status.Codigo);
            $("#txtStatus").val(status.Descricao);
        }
        function RetornoConsultaModeloPneu(modelo) {
            $("#hddCodigoModelo").val(modelo.Codigo);
            $("#txtModelo").val(modelo.Descricao);
        }
        function RetornoConsultaMarcaPneu(marca) {
            $("#hddCodigoMarca").val(marca.Codigo);
            $("#txtMarca").val(marca.Descricao);
        }
        function RetornoConsultaPneu(pneu) {
            executarRest("/Pneu/ObterDetalhes?callback=?", { Codigo: pneu.Codigo }, function (r) {
                if (r.Sucesso) {
                    pneu = r.Objeto;
                    $("#txtMarca").val(pneu.DescricaoMarcaPneu);
                    $("#hddCodigoMarca").val(pneu.CodigoMarcaoPneu);
                    $("#txtModelo").val(pneu.DescricaoModeloPneu);
                    $("#hddCodigoModelo").val(pneu.CodigoModeloPneu);
                    $("#txtStatus").val(pneu.DescricaoStatusPneu);
                    $("#txtStatus").attr("disabled", true);
                    $("#btnBuscarStatus").attr("disabled", true);
                    $("#hddCodigoStatus").val(pneu.CodigoStatusPneu);
                    $("#txtDimensao").val(pneu.DescricaoDimensaoPneu);
                    $("#hddCodigoDimensao").val(pneu.CodigoDimensaoPneu);
                    $("#txtSerie").val(pneu.Serie);
                    $("#txtDataCompra").val(Globalize.format(new Date(), "dd/MM/yyyy"));
                    $("#txtDataVenda").val(Globalize.format(new Date(), "dd/MM/yyyy"));
                    $("#txtValorCompra").val(Globalize.format(pneu.ValorCompra, "n2"));
                    $("#txtCustos").val(Globalize.format(pneu.Custos, "n2"));
                    $("#txtReceitas").val(Globalize.format(pneu.Receitas, "n2"));
                    $("#selStatus").val(pneu.Status);
                    $("#hddCodigo").val(pneu.Codigo);
                    $("#txtObservacao").val(pneu.Observacao);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function LimparCampos() {
            $("#txtMarca").val('');
            $("#hddCodigoMarca").val('0');
            $("#txtModelo").val('');
            $("#hddCodigoModelo").val('0');
            $("#txtStatus").val('');
            $("#hddCodigoStatus").val('0');
            $("#txtDimensao").val('');
            $("#hddCodigoDimensao").val('0');
            $("#txtSerie").val('');
            $("#txtDataCompra").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataVenda").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtValorCompra").val("0,00");
            $("#txtCustos").val("0,00");
            $("#txtReceitas").val("0,00");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#txtObservacao").val('');
            $("#hddCodigo").val('0');
            $("#txtStatus").attr("disabled", false);
            $("#btnBuscarStatus").attr("disabled", false);
        }
        function ValidarCampos() {
            var codigoMarca = Globalize.parseInt($("#hddCodigoMarca").val());
            var codigoModelo = Globalize.parseInt($("#hddCodigoModelo").val());
            var codigoStatus = Globalize.parseInt($("#hddCodigoStatus").val());
            var codigoDimensao = Globalize.parseInt($("#hddCodigoDimensao").val());
            if (isNaN(codigoMarca)) codigoMarca = 0;
            if (isNaN(codigoDimensao)) codigoDimensao = 0;
            if (isNaN(codigoModelo)) codigoModelo = 0;
            if (isNaN(codigoStatus)) codigoStatus = 0;
            var valido = true;
            if (codigoDimensao != 0) {
                CampoSemErro("#txtDimensao");
            } else {
                CampoComErro("#txtDimensao");
                valido = false;
            }
            if (codigoMarca != 0) {
                CampoSemErro("#txtMarca");
            } else {
                CampoComErro("#txtMarca");
                valido = false;
            }
            if (codigoModelo != 0) {
                CampoSemErro("#txtModelo");
            } else {
                CampoComErro("#txtModelo");
                valido = false;
            }
            if (codigoStatus != 0) {
                CampoSemErro("#txtStatus");
            } else {
                CampoComErro("#txtStatus");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: Globalize.parseInt($("#hddCodigo").val()),
                    Status: $("#selStatus").val(),
                    CodigoMarca: Globalize.parseInt($("#hddCodigoMarca").val()),
                    CodigoModelo: Globalize.parseInt($("#hddCodigoModelo").val()),
                    CodigoStatus: Globalize.parseInt($("#hddCodigoStatus").val()),
                    CodigoDimensao: Globalize.parseInt($("#hddCodigoDimensao").val()),
                    Serie: $("#txtSerie").val(),
                    DataCompra: $("#txtDataCompra").val(),
                    DataVenda: $("#txtDataVenda").val(),
                    ValorCompra: $("#txtValorCompra").val(),
                    Custos: $("#txtCustos").val(),
                    Receitas: $("#txtReceitas").val(),
                    Observacao: $("#txtObservacao").val()
                };
                executarRest("/Pneu/Salvar?callback=?", dados, function (r) {
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
        <input type="hidden" id="hddCodigoMarca" value="0" />
        <input type="hidden" id="hddCodigoModelo" value="0" />
        <input type="hidden" id="hddCodigoStatus" value="0" />
        <input type="hidden" id="hddCodigoDimensao" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Pneus
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Marca*:
                </span>
                <input type="text" id="txtMarca" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMarca" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Modelo*:
                </span>
                <input type="text" id="txtModelo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarModelo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <input type="text" id="txtStatus" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarStatus" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Dimensão*:
                </span>
                <input type="text" id="txtDimensao" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDimensao" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Série:
                </span>
                <input type="text" id="txtSerie" class="form-control" />
            </div>
        </div>
        <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Compra:
                </span>
                <input type="text" id="txtValorCompra" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Compra:
                </span>
                <input type="text" id="txtDataCompra" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Venda:
                </span>
                <input type="text" id="txtDataVenda" class="form-control" />
            </div>
        </div>

        <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Receitas:
                </span>
                <input type="text" id="txtReceitas" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-6 col-sm-4 col-md-4 col-lg-3">
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
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" rows="3" class="form-control"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
