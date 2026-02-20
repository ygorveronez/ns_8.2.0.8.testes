<%@ Page Title="Historico Sefaz" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricoSefaz.aspx.cs" Inherits="EmissaoCTe.WebAdmin.HistoricoSefaz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />

    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var IdEmEdicao = 0;
        $(document).ready(function () {
            CarregarConsultaDeHistoricoSefaz("default-search", "default-search", RetornoConsulta, true)

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
            HeaderAuditoria("HistoricoSefaz");
        });

        function RetornoConsulta(historico) {
            LimparCampos();
            executarRest("/SefazHistorico/ObterDetalhes?callback=?", historico, function (r) {
                if (r.Sucesso) {
                    var dados = r.Objeto;

                    IdEmEdicao = dados.Codigo;

                    $("#txtData").val(dados.Data);                    
                    $("#txtUsuario").val(dados.Usuario);
                    $("#txtObservacao").val(dados.Observacao);
                    $("#selSefaz").val(dados.Sefaz);

                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function LimparCampos() {
            IdEmEdicao = 0;

            $("#txtData").val("Automático");
            $("#txtUsuario").val("Automático");
            $("#txtEmpresa").data("codigo", 0).val("");

            $("#selSefaz").val("RS");
            $("#txtObservacao").val("");
        }
        function ValidarCampos() {
            var valido = true;

            if ($("#txtObservacao").val() != "") {
                CampoSemErro("#txtObservacao");
            } else {
                CampoComErro("#txtObservacao");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: IdEmEdicao,
                    Sefaz: $("#selSefaz").val(),
                    Observacao: $("#txtObservacao").val(),
                };

                executarRest("/SefazHistorico/Salvar?callback=?", dados, function (r) {
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
                    executarRest("/SefazHistorico/Excluir?callback=?", Impressora, function (r) {
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
                <h3>Historico Sefaz</h3>
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
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Data:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" disabled id="txtData" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Usuário:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" disabled id="txtUsuario" />
                                </div>
                            </div>
                        </div>

                        <div class="fieldzao">
                            <div class="field fieldquatro">
                                <div class="label">
                                    <label>
                                        Sefaz*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSefaz">
                                        <option value="RS">Sefaz RS (AC, AL, AM, BA, CE, DF, ES, GO, MA, PA, PB, PI, RJ, RN, RO, RS, SC, SE, TO)</option>
                                        <option value="SP">Sefaz SP (AP, PE, RR, SP) </option>
                                        <option value="PR">Sefaz PR</option>
                                        <option value="MT">Sefaz MT</option>
                                        <option value="MS">Sefaz MS</option>
                                        <option value="NG">Sefaz MG</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Observação:
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea rows="6" id="txtObservacao" cols="" style="width: 99.5%"></textarea>
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

    <div style="display: none;">
        <iframe id="ifrDownload" src=""></iframe>
    </div>
</asp:Content>
