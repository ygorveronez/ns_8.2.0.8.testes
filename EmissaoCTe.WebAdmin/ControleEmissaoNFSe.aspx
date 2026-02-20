<%@ Page Title="Emissão de NFS-e" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ControleEmissaoNFSe.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ControleEmissaoNFSe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />

    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(function () {
            $("#txtFiltroDataInicial, #txtFiltroDataFinal").mask("99/99/9999").datepicker({ changeMonth: true, changeYear: true });

            var date = new Date();
            $("#txtFiltroDataFinal").val(Globalize.format(date, "dd/MM/yyyy"));
            $("#txtFiltroDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));

            CarregarConsultaDeEmpresas("btnFiltroEmpresa", "btnFiltroEmpresa", "A", RetornoConsultaTransportador, true);
            RemoveConsulta($("#txtFiltroEmpresa"), function ($this) {
                $this.val('');
                $this.data('codigo', 0);
            });

            AtualizarGrid();

            $("#btnConsultar").click(function () {
                AtualizarGrid();
            });
        });

        function RetornoConsultaTransportador(transportador) {
            $("#txtFiltroEmpresa").data('codigo', transportador.Codigo).val(transportador.RazaoSocial);
        }

        function AtualizarGrid() {
            CriarGridView("/NotaFiscalDeServicosEletronica/ConsultarAdmin?callback=?", {
                inicioRegistros: 0,
                Status: $("#selFiltroStatus").val(),
                DataEmissaoInicial: $("#txtFiltroDataInicial").val(),
                DataEmissaoFinal: $("#txtFiltroDataFinal").val(),
                Empresa: $("#txtFiltroEmpresa").data('codigo'),
                NumeroCarga: $("#txtFiltroNumeroCarga").val(),
            }, "tbl_nfses_table", "tbl_nfses", "tbl_paginacao_nfses", [
                    { Descricao: "Baixar XML", Evento: DownloadXML },
                    { Descricao: "Baixar PDF", Evento: DownloadDANFSE },
                    { Descricao: "Reemtir", Evento: Reemitir },
                ], [0, 1], null, true);
        }

        function DownloadDANFSE(doc) {
            if (!PodeDownalodNFSe(doc.data.Status))
                return jAlert("O status do NFS-e não permite a geração do DANFSE.", "Atenção!");
            $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadDANFSE?CodigoNFSe=" + doc.data.Codigo);
        }

        function DownloadXML(doc) {
            if (!PodeDownalodNFSe(doc.data.Status))
                return jAlert("O status do NFS-e não permite a geração do XML.", "Atenção!");
            $("#ifrDownload").attr("src", "NotaFiscalDeServicosEletronica/DownloadXMLAutorizacao?CodigoNFSe=" + doc.data.Codigo);
        }

        function PodeDownalodNFSe(status) {
            var _arrayStatus = [
                3, // Autorizado
                5, // Cancelado
                4, // EmCancelamento
            ];
            return $.inArray(status, _arrayStatus) >= 0;
        }

        function Reemitir(doc) {
            var NFSe = {
                Codigo: doc.data.Codigo
            };

            executarRest("/NotaFiscalDeServicosEletronica/Emitir?callback=?", NFSe, function (r) {
                AtualizarGrid();
                if (!r.Sucesso) {
                    jAlert(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Emissão de NFS-e</h3>
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
                                    Data Inicial:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtFiltroDataInicial" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Data Final:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtFiltroDataFinal" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Número carga:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtFiltroNumeroCarga" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Status:
                                </label>
                            </div>
                            <div class="input">
                                <select id="selFiltroStatus" class="select">
                                    <option value="">Todos</option>
                                    <option value="3">Autorizado</option>
                                    <option value="5">Cancelado</option>
                                    <option value="4">Em Cancelamento</option>
                                    <option value="0">Em Digitação</option>
                                    <option value="2">Enviado</option>
                                    <option value="1">Pendente</option>
                                    <option value="9">Rejeição</option>
                                </select>
                            </div>
                        </div>
                        <div class="field fielddois">
                            <div class="label">
                                <label>
                                    Empresa:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtFiltroEmpresa" />
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
                            <input type="button" id="btnFiltroEmpresa" value="Buscar" />
                        </div>
                    </div>
                    
                    <div class="fields">
                        <div class="buttons" style="margin-left: 5px;margin-top: 0px;">
                            <input type="button" id="btnConsultar" value="Consultar" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_nfses"style="margin-bottom: 20px;">
                            </div>
                            <div id="tbl_paginacao_nfses" class="pagination">
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
