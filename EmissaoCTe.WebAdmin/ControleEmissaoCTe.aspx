<%@ Page Title="Emissão de CT-e" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ControleEmissaoCTe.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ControleEmissaoCTe" %>

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
            CriarGridView("/ConhecimentoDeTransporteEletronico/ConsultarAdmin?callback=?", {
                inicioRegistros: 0,
                Status: $("#selFiltroStatus").val(),
                DataEmissaoInicial: $("#txtFiltroDataInicial").val(),
                DataEmissaoFinal: $("#txtFiltroDataFinal").val(),
                Empresa: $("#txtFiltroEmpresa").data('codigo'),
                NumeroCarga: $("#txtFiltroNumeroCarga").val(),
            }, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", [
                    { Descricao: "Baixar XML", Evento: DownloadXML },
                    { Descricao: "Baixar PDF", Evento: DownloadDACTE },
                    { Descricao: "Reemtir", Evento: Reemitir },
                    { Descricao: "Verificar Digest", Evento: VerificarDigest }
                ], [0, 1, 2, 3], null, true);
        }

        function DownloadDACTE(doc) {
            if (doc.data.Status != "A" && doc.data.Status != "C" && doc.data.Status != "K")
                return jAlert("O status do CT-e não permite a geração do DACTE.", "Atenção!");
            $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadDacte?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.Empresa);
        }

        function DownloadXML(doc) {
            if (doc.data.Status != "A" && doc.data.Status != "C" && doc.data.Status != "K")
                return jAlert("O status do CT-e não permite a geração do XML.", "Atenção!");
            $("#ifrDownload").attr("src", "ConhecimentoDeTransporteEletronico/DownloadXML?CodigoCTe=" + doc.data.Codigo + "&CodigoEmpresa=" + doc.data.Empresa);
        }

        function Reemitir(doc) {
            var cte = {
                CodigoCTe: doc.data.Codigo,
                FormaEmissao: 1
            };

            executarRest("/ConhecimentoDeTransporteEletronico/Emitir?callback=?", cte, function (r) {
                AtualizarGrid();
                if (!r.Sucesso) {
                    jAlert(r.Erro, "Atenção!");
                }
            });
        }

        function VerificarDigest(doc) {
            var cte = {
                CodigoCTe: doc.data.Codigo,
                FormaEmissao: 1
            };

            executarRest("/ConhecimentoDeTransporteEletronico/VerificarDigest?callback=?", cte, function (r) {
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
                <h3>Emissão de CT-e</h3>
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
                                    <option value="A">Autorizado</option>
                                    <option value="C">Cancelado</option>
                                    <option value="D">Denegado</option>
                                    <option value="S">Em Digitação</option>
                                    <option value="E">Enviado</option>
                                    <option value="I">Inutilizado</option>
                                    <option value="P">Pendente</option>
                                    <option value="R">Rejeição</option>
                                    <option value="K">Em Cancelamento</option>
                                    <option value="L">Em Inutilização</option>
                                    <option value="Q">EPEC</option>
                                    <option value="Y">Aguardando Finalizar Carga</option>
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
                        <div class="buttons" style="margin-left: 5px; margin-top: 0px;">
                            <input type="button" id="btnConsultar" value="Consultar" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_ctes" style="margin-bottom: 40px;">
                            </div>
                            <div id="tbl_paginacao_ctes" class="pagination">
                            </div>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="display: none;">
        <iframe id="ifrDownload" src=""></iframe>
    </div>
</asp:Content>
