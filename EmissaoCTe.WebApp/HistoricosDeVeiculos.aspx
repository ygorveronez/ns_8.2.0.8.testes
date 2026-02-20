<%@ Page Title="Cadastro de Históricos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="HistoricosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.HistoricosDeVeiculos" %>

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
            $("#txtData").mask("99/99/9999");
            $("#txtData").datepicker();
            $("#txtDataGarantia").mask("99/99/9999");
            $("#txtDataGarantia").datepicker();
            $("#txtData").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtValor").priceFormat({ prefix: '' });
            $("#txtKilometragemVeiculo").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtQuantidade").priceFormat({ prefix: '' });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoVeiculo").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtServico").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoServico").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtFornecedor").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoFornecedor").val("");
                        $("#txtFornecedorNaoCadastrado").attr("disabled", false);
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtFornecedorNaoCadastrado").blur(function () {
                if ($(this).val().trim() == "") {
                    $(this).val('');
                    $("#btnBuscarFornecedor").attr("disabled", false);
                } else {
                    $("#btnBuscarFornecedor").attr("disabled", true);
                }
            });
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            CarregarConsultaDeServicosDeVeiculos("btnBuscarServico", "btnBuscarServico", "A", RetornoConsultaServico, true, false);
            CarregarConsultadeClientes("btnBuscarFornecedor", "btnBuscarFornecedor", RetornoConsultaFornecedor, true, false);
            CarregarConsultaDeHistoricosDeVeiculos("default-search", "default-search", "", RetornoConsultaHistoricoVeiculo, true, false);
        });
        function RetornoConsultaHistoricoVeiculo(historico) {
            executarRest("/HistoricoDeVeiculo/ObterDetalhes?callback=?", { Codigo: historico.Codigo }, function (r) {
                if (r.Sucesso) {
                    LimparCampos();
                    $("#txtData").val(r.Objeto.Data);
                    $("#txtDataGarantia").val(r.Objeto.DataGarantia);
                    if (r.Objeto.CodigoFornecedor != "") {
                        $("#txtFornecedor").val(r.Objeto.DescricaoFornecedor);
                        $("#hddCodigoFornecedor").val(r.Objeto.CodigoFornecedor);
                        $("#txtFornecedorNaoCadastrado").attr("disabled", true);
                    } else {
                        if (r.Objeto.DescricaoFornecedor != "") {
                            $("#txtFornecedorNaoCadastrado").val(r.Objeto.DescricaoFornecedor);
                            $("#btnBuscarFornecedor").attr("disabled", true);
                        }
                    }
                    $("#txtServico").val(r.Objeto.DescricaoServico);
                    $("#hddCodigoServico").val(r.Objeto.CodigoServico);
                    $("#txtVeiculo").val(r.Objeto.DescricaoVeiculo);
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("#txtValor").val(Globalize.format(r.Objeto.Valor, "n2"));
                    $("#txtKilometragemVeiculo").val(Globalize.format(r.Objeto.KMVeiculo, "n0"));
                    $("#txtQuantidade").val(Globalize.format(r.Objeto.Quantidade, "n2"));
                    $("#selStatus").val(r.Objeto.Status);
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaFornecedor(fornecedor) {
            $("#txtFornecedor").val(fornecedor.CPFCNPJ + " - " + fornecedor.Nome);
            $("#hddCodigoFornecedor").val(fornecedor.CPFCNPJ);
            $("#txtFornecedorNaoCadastrado").attr("disabled", true);
        }
        function RetornoConsultaServico(servico) {
            $("#txtServico").val(servico.Descricao);
            $("#hddCodigoServico").val(servico.Codigo);
        }
        function RetornoConsultaVeiculo(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
            executarRest("/Veiculo/BuscarKilometragemPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
                if (r.Sucesso) {
                    $("#txtKilometragemVeiculo").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function LimparCampos() {
            $("#btnBuscarFornecedor").attr("disabled", false);
            $("#txtFornecedorNaoCadastrado").attr("disabled", false);
            $("#txtData").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataGarantia").val('');
            $("#txtFornecedor").val('');
            $("#hddCodigoFornecedor").val('');
            $("#txtFornecedorNaoCadastrado").val('');
            $("#txtServico").val('');
            $("#hddCodigoServico").val('0');
            $("#txtVeiculo").val('');
            $("#hddCodigoVeiculo").val('0');
            $("#txtValor").val('0,00');
            $("#txtKilometragemVeiculo").val('0');
            $("#txtQuantidade").val('0,00');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddCodigo").val('0');
            $("#txtObservacao").val('');
        }
        function ValidarCampos() {
            var veiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            var servico = Globalize.parseInt($("#hddCodigoServico").val());
            var valido = true;
            if (veiculo == 0 || isNaN(veiculo)) {
                CampoComErro("#txtVeiculo");
                valido = false;
            } else {
                CampoSemErro("#txtVeiculo");
            }
            if (servico == 0 || isNaN(servico)) {
                CampoComErro("#txtServico");
                valido = false;
            } else {
                CampoSemErro("#txtServico");
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: Globalize.parseInt($("#hddCodigo").val()),
                    Data: $("#txtData").val(),
                    DataGarantia: $("#txtDataGarantia").val(),
                    CodigoFornecedor: $("#hddCodigoFornecedor").val(),
                    DescricaoFornecedor: $("#txtFornecedorNaoCadastrado").val(),
                    CodigoServico: Globalize.parseInt($("#hddCodigoServico").val()),
                    CodigoVeiculo: Globalize.parseInt($("#hddCodigoVeiculo").val()),
                    Valor: $("#txtValor").val(),
                    KilometragemVeiculo: Globalize.parseInt($("#txtKilometragemVeiculo").val()),
                    Quantidade: $("#txtQuantidade").val(),
                    Observacao: $("#txtObservacao").val(),
                    Status: $("#selStatus").val()
                };
                executarRest("/HistoricoDeVeiculo/Salvar?callback=?", dados, function (r) {
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
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoServico" value="0" />
        <input type="hidden" id="hddCodigoFornecedor" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Históricos de Veículos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data*:
                </span>
                <input type="text" id="txtData" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento da Garantia">Vencto. Garantia</abbr>:
                </span>
                <input type="text" id="txtDataGarantia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Serviço*:
                </span>
                <input type="text" id="txtServico" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarServico" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Fornecedor:
                </span>
                <input type="text" id="txtFornecedor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFornecedor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Nome do Fornecedor Não Cadastrado">Forn. Não Cad.</abbr>:
                </span>
                <input type="text" id="txtFornecedorNaoCadastrado" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">KM Veículo:
                </span>
                <input type="text" id="txtKilometragemVeiculo" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Quantidade:
                </span>
                <input type="text" id="txtQuantidade" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor:
                </span>
                <input type="text" id="txtValor" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
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
