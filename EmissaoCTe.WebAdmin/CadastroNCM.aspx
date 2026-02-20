<%@ Page Title="Cadastro de Campos NCM" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="CadastroNCM.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CadastroNCM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var IdCampoNCMEmEdicao = 0;
        $(document).ready(function () {
            $("#txtNumero").mask("?99999999");

            CarregarConsultaCamposNCM("default-search", "default-search", RetornoConsultaCamposNCM, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnExcluir").click(function () {
                Excluir();
            });
        });

        function RetornoConsultaCamposNCM(campo) {
            IdCampoNCMEmEdicao = campo.Codigo;

            $("#txtNumero").val(campo.Numero);
            $("#txtDescricao").val(campo.Descricao);

            $("#btnExcluir").show();
        }
        function LimparCampos() {
            IdCampoNCMEmEdicao = 0;

            $("#txtDescricao").val('');
            $("#txtNumero").val('');

            $("#btnExcluir").hide();
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var numero = parseInt($("#txtNumero").val());

            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if (numero > 0) {
                CampoSemErro("#txtNumero");
            } else {
                CampoComErro("#txtNumero");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: IdCampoNCMEmEdicao,
                    Numero: $("#txtNumero").val(),
                    Descricao: $("#txtDescricao").val(),
                };

                executarRest("/NCM/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }
        function Excluir() {
            jConfirm("Tem certeza que deseja excluir", "Confirmar exclusão", function (r) {
                if (r) {
                    executarRest("/NCM/Excluir?callback=?", { Codigo: IdCampoNCMEmEdicao }, function (r) {
                        if (r.Sucesso) {
                            ExibirMensagemSucesso("Excluído com sucesso.", "Sucesso");
                            LimparCampos();
                        } else {
                            ExibirMensagemErro(r.Erro, "Atenção");
                        }
                    });
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
                <h3>Cadastro de Campos NCM</h3>
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
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Número*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNumero" data-model="Numero" maxlength="8" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Descrição*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" data-model="Descricao" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                            <input type="button" id="btnExcluir" value="Excluir" style="display: none;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
