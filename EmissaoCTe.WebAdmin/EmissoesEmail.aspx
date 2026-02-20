<%@ Page Title="Emissões por E-mail" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EmissoesEmail.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EmissoesEmail" %>

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
        $(function () {
            $("#txtDataEmissaoFiltro").mask("99/99/9999");
            $("#txtDataEmissaoFiltro").datepicker({ changeMonth: true, changeYear: true });

            var date = new Date();
            $("#txtDataEmissaoFiltro").val(Globalize.format(date, "dd/MM/yyyy"));

            Carregar();

            $("#btnFiltrar").click(function () {
                Carregar();
            });
        });

        function Carregar() {
            CriarGridView("/EmissaoEmail/Consultar?callback=?", { inicioRegistros: 0, NumeroCTe: $("#txtNumeroFiltro").val(), DataEmissao: $("#txtDataEmissaoFiltro").val(), Empresa: $("#txtEmpresaFiltro").val() }, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", [{ Descricao: "Acessar", Evento: AcessarSistema }, { Descricao: "Emitir", Evento: Reemitir }, { Descricao: "PDF", Evento: DownloadDACTE }, { Descricao: "XML", Evento: DownloadXML }], [0, 1]);
        }

        function AcessarSistema(doc) {
            executarRest("/Empresa/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: doc.data.CodigoEmpresa }, function (r) {
                if (r.Sucesso) {
                    var win = window.open();
                    var uriAcesso = "http://" + window.location.host + "/" + r.Objeto.UriAcesso + "?x=" + r.Objeto.Login + "&y=" + r.Objeto.Senha + "&z=" + r.Objeto.Usuario;
                    win.location = uriAcesso;
                    win.focus();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

        function DownloadDACTE(doc) {
            if (doc.data.Tipo == "CT-e")
                $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadDacte?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
            else if (doc.data.Tipo == "NFs-e")
                $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadDANFSE?CodigoNFSe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
        }

        function DownloadXML(doc) {
            if (doc.data.Tipo == "CT-e")
                $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadXML?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
            else if (doc.data.Tipo == "NFs-e")
                $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadXMLAutorizacao?CodigoNFSe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.CodigoEmpresa);
        }

        function Reemitir(doc) {
            var valor = Globalize.parseFloat(doc.data.ValorFrete);
            if (valor <= 0)
                jAlert("Valor Frete deve ser maior que zero.");
            else
            {
                if (doc.data.Tipo == "CT-e") {
                    if (doc.data.DescricaoStatus != "Rejeição" && doc.data.DescricaoStatus != "Em Digitação")
                        jAlert("Status não permite emitir CT-e.");
                    else {
                        executarRest("/ConhecimentoDeTransporteEletronico/Emitir?callback=?", { CodigoCTe: doc.data.Codigo }, function (r) {
                            if (r.Sucesso) {
                                jAlert("CT-e enviado com sucesso!", "Sucesso");
                                Carregar();
                            } else {
                                jAlert(r.Erro, "Atenção");
                            }
                        });
                    }
                }
                else if (doc.data.Tipo == "NFs-e" && doc.data.DescricaoStatus != "Em Digitação") {
                    if (doc.data.DescricaoStatus != "Rejeição")
                        jAlert("Status não permite emitir NFs-e.");
                    else {
                        executarRest("/NotaFiscalDeServicosEletronica/Emitir?callback=?", { Codigo: doc.data.Codigo }, function (r) {
                            if (r.Sucesso) {
                                jAlert("NFs-e enviada com sucesso!", "Sucesso");
                                Carregar();
                            } else {
                                jAlert(r.Erro, "Atenção");
                            }
                        });
                    }
                }
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Emissões por E-mail
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
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Número:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtNumeroFiltro" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Data Emissão:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtDataEmissaoFiltro" />
                            </div>
                        </div>
                        <div class="field fielddois">
                            <div class="label">
                                <label>
                                    Empresa:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtEmpresaFiltro" />
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
                            <input type="button" id="btnFiltrar" value="Filtrar" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_ctes">
                            </div>
                            <div id="tbl_paginacao_ctes" class="pagination">
                            </div>
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
