<%@ Page Title="Cadastro de Impressora" Language="C#" MasterPageFile="Site.Master"
    AutoEventWireup="true" CodeBehind="Impressoras.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CadastroImpressora" %>

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
        var IdImpressoraEmEdicao = 0;
        $(document).ready(function () {
            $("#txtNumeroDaUnidade").mask("9?9999999999");

            CarregarConsultaImpressoras("default-search", "default-search", RetornoConsultaImpressora, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnExcluir").click(function () {
                Excluir();
            });

            LimparCampos();
        });
        function RetornoConsultaImpressora(impressora) {
            IdImpressoraEmEdicao = impressora.Codigo;

            $("#txtNumeroDaUnidade").val(impressora.NumeroDaUnidade);
            $("#txtNomeImpressora").val(impressora.NomeImpressora);
            $("#txtStatus").val(impressora.Status);
            $("#txtLog").val(impressora.Log);

            // Scroll pro fim da area
            $("#txtLog").scrollTop($("#txtLog")[0].scrollHeight);

            $("#btnExcluir").show();
        }
        function LimparCampos() {
            IdImpressoraEmEdicao = 0;

            $("#txtNumeroDaUnidade").val("");
            $("#txtNomeImpressora").val("");
            $("#txtStatus").val($("#txtStatus option:first").val());
            $("#txtLog").val("");

            $("#btnExcluir").hide();
        }
        function ValidarCampos() {
            var valido = true;

            if ($("#txtNumeroDaUnidade").val() != "") {
                CampoSemErro("#txtNumeroDaUnidade");
            } else {
                CampoComErro("#txtNumeroDaUnidade");
                valido = false;
            }

            if ($("#txtNomeImpressora").val() != "") {
                CampoSemErro("#txtNomeImpressora");
            } else {
                CampoComErro("#txtNomeImpressora");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: IdImpressoraEmEdicao,
                    Unidade: $("#txtNumeroDaUnidade").val(),
                    Impressora: $("#txtNomeImpressora").val(),
                    Status: $("#txtStatus").val(),
                    Log: $("#txtLog").val(),
                };

                executarRest("/Impressoras/Salvar?callback=?", dados, function (r) {
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
                    executarRest("/Impressoras/Excluir?callback=?", Impressora, function (r) {
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
                <h3>Cadastro de Impressoras</h3>
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
                                        Unidade*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNumeroDaUnidade"  />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Impressora*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNomeImpressora"  />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Status*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="txtStatus">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Log*:
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea rows="6" readonly id="txtLog" cols="" style="width: 99.5%"></textarea>
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
