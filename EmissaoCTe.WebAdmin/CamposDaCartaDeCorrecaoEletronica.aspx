<%@ Page Title="Campos da Carta de Correção Eletrônica" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CamposDaCartaDeCorrecaoEletronica.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CamposDaCartaDeCorrecaoEletronica" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LimparCampos();

            CarregarConsultaDeCamposDaCartaDeCorrecaoEletronica("default-search", "default-search", RetornoConsultaCampoCCe, true, false);

            $("#txtQuantidadeCaracteres").priceFormat({ limit: 3, centsLimit: 0, centsSeparator: '' });
            $("#txtQuantidadeInteiros").priceFormat({ limit: 2, centsLimit: 0, centsSeparator: '' });
            $("#txtQuantidadeDecimais").priceFormat({ limit: 1, centsLimit: 0, centsSeparator: '' });

            $("#selTipoCampo").change(function () {
                TrocarTipoDeCampo();
            });

            $("#btnSalvar").click(function () {
                SalvarDados();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function TrocarTipoDeCampo() {
            switch ($("#selTipoCampo").val()) {
                case "0":
                    $("#divQuantidadeCaracteres").show();
                    $("#divQuantidadeInteiros").hide();
                    $("#divQuantidadeDecimais").hide();
                    break;
                case "1":
                    $("#divQuantidadeCaracteres").hide();
                    $("#divQuantidadeInteiros").show();
                    $("#divQuantidadeDecimais").hide();
                    break;
                case "2":
                    $("#divQuantidadeCaracteres").hide();
                    $("#divQuantidadeInteiros").show();
                    $("#divQuantidadeDecimais").show();
                    break;
                default:
                    $("#divQuantidadeCaracteres").hide();
                    $("#divQuantidadeInteiros").hide();
                    $("#divQuantidadeDecimais").hide();
                    break;
            }
        }

        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#txtDescricao").val("");
            $("#txtNomeCampo").val("");
            $("#txtGrupoCampo").val("");
            $("#selIndicadorRepeticao").val($("#selIndicadorRepeticao option:first").val());
            $("#selTipoCampo").val($("#selTipoCampo option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#txtQuantidadeCaracteres").val("0");
            $("#txtQuantidadeDecimais").val("0");
            $("#txtQuantidadeInteiros").val("0");
            TrocarTipoDeCampo();
        }

        function ValidarCampos() {
            var descricao = $("#txtDescricao").val();
            var nomeCampo = $("#txtNomeCampo").val();
            var quantidadeCaracteres = Globalize.parseInt($("#txtQuantidadeCaracteres").val());
            var quantidadeDecimais = Globalize.parseInt($("#txtQuantidadeDecimais").val());
            var quantidadeInteiros = Globalize.parseInt($("#txtQuantidadeInteiros").val());
            var valido = true, validarInteiros = false, validarDecimais = false, validarCaracteres = false;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if (nomeCampo != "") {
                CampoSemErro("#txtNomeCampo");
            } else {
                CampoComErro("#txtNomeCampo");
                valido = false;
            }

            switch ($("#selTipoCampo").val()) {
                case "0":
                    validarCaracteres = true;
                    break;
                case "1":
                    validarInteiros = true;
                    break;
                case "2":
                    validarInteiros = true;
                    validarDecimais = true;
                    break;
                default:
                    break;
            }

            if (validarCaracteres) {
                if (quantidadeCaracteres > 0) {
                    CampoSemErro("#txtQuantidadeCaracteres");
                } else {
                    CampoComErro("#txtQuantidadeCaracteres");
                    valido = false;
                }
            }

            if (validarInteiros) {
                if (quantidadeInteiros > 0) {
                    CampoSemErro("#txtQuantidadeInteiros");
                } else {
                    CampoComErro("#txtQuantidadeInteiros");
                    valido = false;
                }
            }

            if (validarDecimais) {
                if (quantidadeDecimais > 0) {
                    CampoSemErro("#txtQuantidadeDecimais");
                } else {
                    CampoComErro("#txtQuantidadeDecimais");
                    valido = false;
                }
            }

            return valido;
        }

        function SalvarDados() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(),
                    NomeCampo: $("#txtNomeCampo").val(),
                    GrupoCampo: $("#txtGrupoCampo").val(),
                    IndicadorRepeticao: $("#selIndicadorRepeticao").val(),
                    TipoCampo: $("#selTipoCampo").val(),
                    Status: $("#selStatus").val(),
                    QuantidadeCaracteres: $("#txtQuantidadeCaracteres").val(),
                    QuantidadeDecimais: $("#txtQuantidadeDecimais").val(),
                    QuantidadeInteiros: $("#txtQuantidadeInteiros").val()
                };
                executarRest("/CampoDaCartaDeCorrecaoEletronica/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco(*) são obrigatórios!", "Atenção");
            }
        }

        function RetornoConsultaCampoCCe(campo) {
            executarRest("/CampoDaCartaDeCorrecaoEletronica/ObterDetalhes?callback=?", { CodigoCampoCCe: campo.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#txtNomeCampo").val(r.Objeto.NomeCampo);
                    $("#txtGrupoCampo").val(r.Objeto.GrupoCampo);
                    $("#selIndicadorRepeticao").val(r.Objeto.IndicadorRepeticao ? "true" : "false");
                    $("#selTipoCampo").val(r.Objeto.TipoCampo);
                    $("#selStatus").val(r.Objeto.Status);
                    $("#txtQuantidadeCaracteres").val(r.Objeto.QuantidadeCaracteres);
                    $("#txtQuantidadeDecimais").val(r.Objeto.QuantidadeDecimais);
                    $("#txtQuantidadeInteiros").val(r.Objeto.QuantidadeInteiros);
                    TrocarTipoDeCampo();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Campos da Carta de Correção Eletrônica
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
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" maxlength="50" />
                                </div>
                            </div>
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Nome do Campo*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNomeCampo" maxlength="20" />
                                </div>
                            </div>
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Grupo do Campo:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtGrupoCampo" maxlength="20" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Campo se Repete*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selIndicadorRepeticao" class="select">
                                        <option value="false">Não</option>
                                        <option value="true">Sim</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Tipo do Campo*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoCampo" class="select">
                                        <option value="0">Texto</option>
                                        <option value="1">Inteiro</option>
                                        <option value="2">Decimal</option>
                                        <option value="4">Data</option>
                                        <option value="3">Seleção</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldtres" id="divQuantidadeCaracteres">
                                <div class="label">
                                    <label>
                                        Qtd. de Caracteres*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtQuantidadeCaracteres" value="0" />
                                </div>
                            </div>
                            <div class="field fieldtres" id="divQuantidadeInteiros">
                                <div class="label">
                                    <label>
                                        Qtd. de Inteiros*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtQuantidadeInteiros" value="0" />
                                </div>
                            </div>
                            <div class="field fieldtres" id="divQuantidadeDecimais">
                                <div class="label">
                                    <label>
                                        Qtd. de Decimais*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtQuantidadeDecimais" value="0" />
                                </div>
                            </div>
                            <div class="field fieldtres">
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
                        <div class="buttons">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
