<%@ Page Title="Despesas Adicionais das Empresas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="DespesasEmpresa.aspx.cs" Inherits="EmissaoCTe.WebAdmin.DespesasEmpresa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresas, true, false);

            CarregarConsultaDeDespesasAdicionaisDaEmpresa("default-search", "default-search", "", RetornoConsultaDespesas, true, false);

            $("#txtValor").priceFormat({ prefix: '' });

            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker({ changeMonth: true, changeYear: true });

            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataInicial").datepicker({ changeMonth: true, changeYear: true });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function RetornoConsultaDespesas(despesa) {
            executarRest("/DespesaAdicionalEmpresa/ObterDetalhes?callback=?", { CodigoDespesa: despesa.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoEmpresa").val(r.Objeto.CodigoEmpresa);
                    $("#txtEmpresa").val(r.Objeto.DescricaoEmpresa);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#txtValor").val(r.Objeto.Valor);
                    $("#txtDataInicial").val(r.Objeto.DataInicial);
                    $("#txtDataFinal").val(r.Objeto.DataFinal);
                    $("#selStatus").val(r.Objeto.Status);
                    $("#selTipo").val(r.Objeto.Tipo);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaEmpresas(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.CNPJ + " - " + empresa.NomeFantasia)
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoEmpresa: $("#hddCodigoEmpresa").val(),
                    Descricao: $("#txtDescricao").val(),
                    DataInicial: $("#txtDataFinal").val(),
                    DataFinal: $("#txtDataFinal").val(),
                    Valor: $("#txtValor").val(),
                    Status: $("#selStatus").val(),
                    Tipo: $("#selTipo").val(),
                };

                executarRest("/DespesaAdicionalEmpresa/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtDescricao").val('');
            $("#txtValor").val('0,00');
            $("#hddCodigoEmpresa").val('0');
            $("#txtEmpresa").val('');
            $("#txtDataInicial").val('');
            $("#txtDataFinal").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipo").val($("#selTipo option:first").val());
        }

        function ValidarDados() {
            var descricao = $("#txtDescricao").val();
            var valor = Globalize.parseFloat($("#txtValor").val());
            var codigoEmpresa = Globalize.parseInt($("#hddCodigoEmpresa").val());
            var dataInicial = $("#txtDataInicial").val();
            var dataFinal = $("#txtDataFinal").val();
            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtEmpresa");
            } else {
                CampoComErro("#txtEmpresa");
                valido = false;
            }

            if (!isNaN(valor) && valor > 0) {
                CampoSemErro("#txtValor");
            } else {
                CampoComErro("#txtValor");
                valido = false;
            }

            if (!isNaN(codigoEmpresa) && codigoEmpresa > 0) {
                CampoSemErro("#txtEmpresa");
            } else {
                CampoComErro("#txtEmpresa");
                valido = false;
            }

            if (dataInicial != "") {
                CampoSemErro("#txtDataInicial");
            } else {
                CampoComErro("#txtDataInicial");
                valido = false;
            }

            if (dataFinal != "") {
                CampoSemErro("#txtDataFinal");
            } else {
                CampoComErro("#txtDataFinal");
                valido = false;
            }

            return valido;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoEmpresa" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Despesas Adicionais das Empresas
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Empresa*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresa" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" maxlength="200" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Valor*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtValor" class="maskedInput" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Inicial*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataInicial" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Final*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataFinal" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Tipo*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipo" class="select">
                                        <option value="A">Acréscimo</option>
                                        <option value="D">Desconto</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Status*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selStatus" class="select">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
