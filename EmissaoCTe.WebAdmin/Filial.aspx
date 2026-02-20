<%@ Page Title="Filial" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Filial.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Filial" %>

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
            CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false, "");
            CarregarConsultaDeFilial("default-search", "default-search", RetornaBuscaFilial, true, false);

            $("#txtCliente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoCliente").val('0');
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

            AtualizarVisibilidadeCamposValePedagio();

            $("#selCompraValePedagio, #selIntegradoraValePedagio").on("change", AtualizarVisibilidadeCamposValePedagio);
        });

        function AtualizarVisibilidadeCamposValePedagio() {
            const mostrarCampos = $("#selCompraValePedagio").val() === "1";
            const integradoraValePedagio = $("#selIntegradoraValePedagio").val() === "2";

            $("#selIntegradoraValePedagio").closest(".field").toggle(mostrarCampos);
            $("#txtUrlIntegracaoRest").closest(".field").toggle(mostrarCampos && integradoraValePedagio);
        }

        function RetornaBuscaFilial(filial) {
            executarRest("/Filial/ObterDetalhes?callback=?", { Codigo: filial.Codigo }, function (r) {
                if (r.Sucesso) {
                    LimparCampos();

                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtDescricao").val(r.Objeto.Descricao);

                    if (r.Objeto.RazaoCliente != "") {
                        $("#hddCodigoCliente").val(r.Objeto.CNPJCliente);
                        $("#txtCliente").val(r.Objeto.CNPJCliente + " - " + r.Objeto.RazaoCliente);
                    }

                    $("#selCompraValePedagio").val(r.Objeto.CompraValePedagio);
                    $("#selIntegradoraValePedagio").val(r.Objeto.IntegradoraValePedagio);
                    $("#txtFornecedorValePedagio").val(r.Objeto.FornecedorValePedagio);
                    $("#txtUsuarioValePedagio").val(r.Objeto.UsuarioValePedagio);
                    $("#txtSenhaValePedagio").val(r.Objeto.SenhaValePedagio);
                    $("#txtUrlIntegracaoRest").val(r.Objeto.URLIntegracaoRest);

                    AtualizarVisibilidadeCamposValePedagio();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaCliente(cliente) {
            $("#hddCodigoCliente").val(cliente.CPFCNPJ);
            $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome)
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Cliente: $("#hddCodigoCliente").val(),
                    Descricao: $("#txtDescricao").val(),
                    CompraValePedagio: $("#selCompraValePedagio").val(),
                    IntegradoraValePedagio: $("#selIntegradoraValePedagio").val(),
                    FornecedorValePedagio: $("#txtFornecedorValePedagio").val(),
                    UsuarioValePedagio: $("#txtUsuarioValePedagio").val(),
                    SenhaValePedagio: $("#txtSenhaValePedagio").val(),
                    UrlIntegracaoRest: $("#selIntegradoraValePedagio").val() == 2 ? $("#txtUrlIntegracaoRest").val() : ""
                };

                executarRest("/Filial/Salvar?callback=?", dados, function (r) {
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
            $("#hddCodigoCliente").val('0');
            $("#txtCliente").val('');
            $("#txtDescricao").val('');
            $("#txtFornecedorValePedagio").val('');
            $("#txtUsuarioValePedagio").val('');
            $("#txtSenhaValePedagio").val('');
            $("#selCompraValePedagio").val($("#selStatus option:first").val());
            $("#txtUrlIntegracaoRest").val('');
        }

        function ValidarDados() {
            var cliente = $("#hddCodigoCliente").val();
            var descricao = $("#txtDescricao").val();
            var valido = true;

            if (cliente != "0" && cliente != "")
                CampoSemErro("#txtCliente");
            else {
                CampoComErro("#txtCliente");
                valido = false;
            }

            if (descricao != "0" && descricao != "")
                CampoSemErro("#txtDescricao");
            else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if ($("#selCompraValePedagio").val() == 1) {
                if ($("#txtFornecedorValePedagio").val())
                    CampoSemErro("#txtFornecedorValePedagio");
                else {
                    CampoComErro("#txtFornecedorValePedagio");
                    valido = false;
                }
                if ($("#txtUsuarioValePedagio").val())
                    CampoSemErro("#txtUsuarioValePedagio");
                else {
                    CampoComErro("#txtUsuarioValePedagio");
                    valido = false;
                }
                if ($("#txtSenhaValePedagio").val())
                    CampoSemErro("#txtSenhaValePedagio");
                else {
                    CampoComErro("#txtSenhaValePedagio");
                    valido = false;
                }
            }
            else {
                CampoSemErro("#txtFornecedorValePedagio");
                CampoSemErro("#txtUsuarioValePedagio");
                CampoSemErro("#txtSenhaValePedagio");
            }

            return valido;
        }



    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoCliente" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Filial
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
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Cadastro Filial (Cliente):
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCliente" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarCliente" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Descrição Filial
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Integrar automaticamente Vale Pedágio:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selCompraValePedagio" class="select">
                                        <option value="0">Não</option>
                                        <option value="1">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Integradora Vale Pedágio:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selIntegradoraValePedagio">
                                        <option value="0">Nenhum</option>
                                        <option value="1" selected>Target</option>
                                        <option value="2">Sem Parar</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        URL integração REST
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUrlIntegracaoRest" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        CNPJ Fornecedor Vale Pedágio
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtFornecedorValePedagio" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Usuário Vale Pedágio
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUsuarioValePedagio" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Senha Vale Pedágio
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtSenhaValePedagio" class="maskedInput" />
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
