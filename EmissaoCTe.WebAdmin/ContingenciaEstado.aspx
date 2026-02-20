<%@ Page Title="Contingência Estado" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ContingenciaEstado.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ContingenciaEstado" %>

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

            CarregarConsultaDeContingenciaEstado("default-search", "default-search", RetornoConsultaContingenciaEstado, true, false);

            $("#btnSalvar").click(function () {
                SalvarDados();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });


        function LimparCampos() {
            $("#hddCodigo").val("");
            $("#txtNome").val("");
            $("#selTipoEmissao").val($("#selTipoEmissao option:first").val());
            $("#chkHabilitarContigenciaEpecAutomaticamente").prop('checked', 0);
        }

        function ValidarCampos() {
            return true;
        }

        function SalvarDados() {
            if (ValidarCampos()) {
                var dados = {
                    Sigla: $("#hddCodigo").val(),
                    TipoEmissao: $("#selTipoEmissao").val(),
                    HabilitarContigenciaEpecAutomaticamente: $("#chkHabilitarContigenciaEpecAutomaticamente").prop('checked') ? 1 : 0,
                };
                executarRest("/Estado/SalvarContingencia?callback=?", dados, function (r) {
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

        function RetornoConsultaContingenciaEstado(estado) {
            executarRest("/Estado/ObterDetalhes?callback=?", { Sigla: estado.Sigla }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Sigla);
                    $("#txtNome").val(r.Objeto.Nome);
                    $("#selTipoEmissao").val(r.Objeto.TipoEmissao);
                    $("#chkHabilitarContigenciaEpecAutomaticamente").prop('checked', r.Objeto.HabilitarContigenciaEpecAutomaticamente);
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
                <h3>Contingência Estado
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
                                        Estado*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNome" maxlength="50" disabled />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Tipo emissão*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoEmissao" class="select">
                                        <option value="1">Normal</option>
                                        <option value="7">SVC-RS</option>
                                        <option value="8">SVC-SP</option>
                                        <option value="4">EPEC</option>
                                    </select>
                                </div>
                            </div>
                            <div class="checkbox">
                                <input type="checkbox" id="chkHabilitarContigenciaEpecAutomaticamente" />
                                <label for="chkHabilitarContigenciaEpecAutomaticamente">
                                    Habilitar contigência EPEC automáticamente
                                </label>
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
