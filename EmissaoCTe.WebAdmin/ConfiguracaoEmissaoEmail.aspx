<%@ Page Title="Configuração Emissão E-mail" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ConfiguracaoEmissaoEmail.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ConfiguracaoEmissaoEmail" %>

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
            CarregarConsultaDeEmail("btnBuscarEmail", "btnBuscarEmail", RetornoConsultaEmail, true, false);
            CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false, "");
            CarregarConsultaDeConfiguracaoEmissaoEmail("default-search", "default-search", "A", RetornoConsultaConfiguracaoEmissaoEmail, true, false);

            $("#txtEmail").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmail").val('0');
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmpresa").val('0');
                    } else {
                        e.preventDefault();
                    }
                }
            });

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

            $("#selAgrupar").change(function () {
                ConfigurarAgrupamento(this.value);
            }).trigger('change');

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaConfiguracaoEmissaoEmail(config) {
            executarRest("/ConfiguracaoEmissaoEmail/ObterDetalhes?callback=?", { Codigo: config.Codigo }, function (r) {
                if (r.Sucesso) {
                    LimparCampos();
                    $("#hddCodigo").val(r.Objeto.Codigo);

                    $("#hddCodigoEmail").val(r.Objeto.CodigoEmail);
                    $("#txtEmail").val(r.Objeto.Email);

                    $("#hddCodigoEmpresa").val(r.Objeto.CodigoEmpresa);
                    $("#txtEmpresa").val(r.Objeto.CNPJEmpresa + " - " + r.Objeto.RazaoEmpresa);

                    if (r.Objeto.RazaoCliente != "") {
                        $("#hddCodigoCliente").val(r.Objeto.CNPJCliente);
                        $("#txtCliente").val(r.Objeto.CNPJCliente + " - " + r.Objeto.RazaoCliente);
                    }

                    $("#txtTempo").val(r.Objeto.TempoEmitir);
                    $("#selTipoDocumento").val(r.Objeto.TipoDocumento);
                    $("#selTipo").val(r.Objeto.Tipo);
                    $("#selEmitir").val(r.Objeto.Emitir);
                    $("#selGerarMDFe").val(r.Objeto.GerarMDFe);

                    $("#selAgrupar").val(r.Objeto.Agrupar).trigger('change');;;
                    $("#selStatus").val(r.Objeto.Status);

                    $("#txtPalavraChave").val(r.Objeto.PalavraChave);
                    $("#txtTamanhoPalavra").val(r.Objeto.TamanhoPalavra);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function RetornoConsultaEmpresas(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.CNPJ + " - " + empresa.NomeFantasia)
        }

        function RetornoConsultaEmail(email) {
            $("#hddCodigoEmail").val(email.Codigo);
            $("#txtEmail").val(email.Email)
        }

        function RetornoConsultaCliente(cliente) {
            $("#hddCodigoCliente").val(cliente.CPFCNPJ);
            $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome)
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoEmpresa: $("#hddCodigoEmpresa").val(),
                    Cliente: $("#hddCodigoCliente").val(),
                    CodigoEmail: $("#hddCodigoEmail").val(),
                    Agrupar: $("#selAgrupar").val(),
                    Emitir: $("#selEmitir").val(),
                    GerarMDFe: $("#selGerarMDFe").val(),
                    TipoDocumento: $("#selTipoDocumento").val(),
                    Tipo: $("#selTipo").val(),
                    TempoEmitir: $("#txtTempo").val(),
                    Status: $("#selStatus").val(),
                    PalavraChave: $("#txtPalavraChave").val(),
                    TamanhoPalavra: $("#txtTamanhoPalavra").val()
                };

                executarRest("/ConfiguracaoEmissaoEmail/Salvar?callback=?", dados, function (r) {
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
            $("#hddCodigoEmpresa").val('0');
            $("#hddCodigoCliente").val('0');
            $("#hddCodigoEmail").val('0');
            $("#txtEmpresa").val('');
            $("#txtCliente").val('');
            $("#txtEmail").val('');
            $("#txtTempo").val('0');
            $("#selAgrupar").val($("#selAgrupar option:first").val()).trigger('change');;
            $("#selTipoDocumento").val($("#selTipoDocumento option:first").val());
            $("#selTipo").val($("#selTipo option:first").val());
            $("#selEmitir").val($("#selEmitir option:first").val());
            $("#selGerarMDFe").val($("#selGerarMDFe option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());

            // Limpa agrupamentos
            $("#txtPalavraChave").val('');
            $("#txtTamanhoPalavra").val('');
        }

        function ValidarDados() {
            var codigoEmpresa = Globalize.parseInt($("#hddCodigoEmpresa").val());
            var codigoEmail = Globalize.parseInt($("#hddCodigoEmail").val());
            var valido = true;

            if (!isNaN(codigoEmpresa) && codigoEmpresa > 0) {
                CampoSemErro("#txtEmpresa");
            } else {
                CampoComErro("#txtEmpresa");
                valido = false;
            }

            if (!isNaN(codigoEmail) && codigoEmail > 0) {
                CampoSemErro("#txtEmail");
            } else {
                CampoComErro("#txtEmail");
                valido = false;
            }

            return valido;
        }

        function ConfigurarAgrupamento(selecao) {
            // Esconde todos campos
            $(".configuracao-agrupamento").hide();

            // Define a classe para exibir
            var classSelecao;
            if (selecao == 0) classSelecao = ''; // Não agrupa
            if (selecao == 1) classSelecao = ''; // Por Destinatario
            if (selecao == 2) classSelecao = ''; // Por Veiculo
            if (selecao == 3) classSelecao = 'observacao'; // Por Observacao

            if (classSelecao != "")
                $(".configuracao-agrupamento-" + classSelecao).show();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoEmail" value="0" />
        <input type="hidden" id="hddCodigoEmpresa" value="0" />
        <input type="hidden" id="hddCodigoCliente" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Configuração Emissão E-mail
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
                                        E-mail recebimento*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmail" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmail" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldquatro">
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
                                        Cliente:
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
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Tipo Emissão:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipo" class="select">
                                        <option value="A">Automática</option>
                                        <option value="F">Frimesa</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Tipo Documento:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoDocumento" class="select">
                                        <option value="99">Todos</option>
                                        <option value="0">CT-e</option>
                                        <option value="1">NFS-e</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Emitir:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selEmitir" class="select">
                                        <option value="0">Em Digitação</option>
                                        <option value="1">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Gerar MDFe:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selGerarMDFe" class="select">
                                        <option value="0">Não</option>
                                        <option value="1">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddum" hidden>
                                <div class="label">
                                    <label>
                                        Tempo(Minutos) para Emitir:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTempo" class="maskedInput" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Agrupamento:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selAgrupar" class="select">
                                        <option value="0">Não agrupar</option>
                                        <option value="1">Por destinatário</option>
                                        <option value="2">Por veículo</option>
                                        <option value="3">Por Observacao (XMLs enviados em um mesmo e-mail)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddum">
                                <div class="label">
                                    <label>
                                        Status:
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

                        <div class="fieldzao">
                            <div class="configuracao-agrupamento configuracao-agrupamento-observacao" style="display: none">
                                <div class="field fielddois">
                                    <div class="label">
                                        <label>
                                            Palavra Chave:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtPalavraChave" />
                                    </div>
                                </div>
                                <div class="field fieldum">
                                    <div class="label">
                                        <label>
                                            Tamanho:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtTamanhoPalavra" />
                                    </div>
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
