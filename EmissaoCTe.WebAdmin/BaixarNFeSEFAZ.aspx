<%@ Page Title="Baixar NF-e SEFAZ" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="BaixarNFeSEFAZ.aspx.cs" Inherits="EmissaoCTe.WebAdmin.BaixarNFeSEFAZ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
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
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            LimparCampos();

            $("#btnBaixarNFe").click(function () {
                BaixarNFe();
            });

            $("#btnLimpar").click(function () {
                LimparCampos();
            });

        });

        function BaixarNFe() {
            if (ValidarDados()) {
                var dados = {
                    ChaveNFe: $('#txtChaves').val()
                };
                executarRest("/XMLNotaFiscalEletronica/BaixarNFeSEFAZ?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        if (r.Objeto.ChavesErros != "") {
                            $('#txtChaves').val("");
                            $('#txtChaves').val(r.Objeto.ChavesErros);
                            ExibirMensagemAlerta("Algumas chaves não foram possíveis baixar a sua NF-e!", "Atenção");
                        } else {
                            ExibirMensagemSucesso("Processo realizado com sucesso!", "Sucesso");
                            LimparCampos();
                        }
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function LimparCampos() {
            $("#txtChaves").val("");
        }

        function ValidarDados() {
            var chaves = $("#txtChaves").val();
            var valido = true;

            if (chaves == null || chaves == "") {
                CampoComErro("#txtChaves");
                valido = false;
            } else {
                CampoSemErro("#txtChaves");
            }

            return valido;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Baixar NF-e SEFAZ
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
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
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fieldoito">
                                    <div class="label">
                                        <label>
                                            *Chaves de NF-e:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <textarea id="txtChaves" rows="20" cols="10" style="width: 99.5%"></textarea>
                                    </div>
                                </div>
                            </div>
                            <div id="divArquivosSelecionados" class="fieldzao">
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnBaixarNFe" value="Baixar NF-e" />
                                </div>
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnLimpar" value="Limpar Chaves" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
