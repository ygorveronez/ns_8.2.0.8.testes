<%@ Page Title="CT-e Admin - Empresas Emissoras" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Empresas.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Empresas" %>

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
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#btnFiltrarEmpresas").click(function () {
                AtualizarGridEmpresas();
            });
            $("#txtRazaoSocial_NomeFantasia, #txtCNPJ, #txtPlacaVeiculo").keydown(function (e) {
                if (e.which == 13) {
                    AtualizarGridEmpresas();
                    e.stopPropagation();
                }
            });
            AtualizarGridEmpresas();
        });
        function AtualizarGridEmpresas() {
            BuscarSolicitacoesPendentes();
            CriarGridView("/Empresa/Consultar?callback=?", { inicioRegistros: 0, Nome: $("#txtRazaoSocial_NomeFantasia").val(), CNPJ: $("#txtCNPJ").val(), PlacaVeiculo: $("#txtPlacaVeiculo").val(), Status: "A" }, "tbl_empresas_emissoras_table", "tbl_empresas_emissoras", "tbl_paginacao_empresas_emissoras", [{ Descricao: "Acessar Sistema", Evento: AcessarSistema }, { Descricao: "Usuários", Evento: EditarUsuarios }], [0,1]);
        }
        function AcessarSistema(empresa) {
            if (empresa.data.StatusEmissao === "S") {
                jConfirm("Transportadora configurada como WEB, deseja acessar?", "Atenção", function (ret) {
                    if (ret) {
                        AcessarMultiCTe(empresa.data.Codigo);
                    }
                });
            }
            else
                AcessarMultiCTe(empresa.data.Codigo);
        }
        function AcessarMultiCTe(codigoEmpresa) {
            executarRest("/Empresa/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: codigoEmpresa }, function (r) {
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

        function EditarUsuarios(empresa) {
            location.href = "UsuariosEmpresa.aspx?x=" + empresa.data.CodigoCriptografado;
        }
        function BuscarSolicitacoesPendentes() {
            var id = "divSolicitacoesEmissoes";
            $("#" + id).slideUp();
            executarRest("/Empresa/ObterSolicitacoesEmissaoPendentes?callback=?", { Codigo: 0 }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.MensagemAviso != "") {
                        $("#" + id + " label").text(r.Objeto.MensagemAviso);
                        $("#" + id + " span").text("");
                        $("#" + id).slideDown();
                    }
                }
            });
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            executarRest("/Empresa/ObterEmpresasComCertificadoAVencer?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.length > 0) {
                        for (var i = 0; i < r.Objeto.length; i++) {
                            $("#tblCertificadosVencidos tbody").append("<tr><td>" + r.Objeto[i].CNPJ + "</td><td>" + r.Objeto[i].NomeFantasia + "</td><td>" + r.Objeto[i].Data + "</td><td>" + r.Objeto[i].EmpresaAdmin + "</td> </tr>");
                        }

                        $("#divCertificadosVencidos").slideDown();
                    }
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Empresas Emissoras
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div class="fields">
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
                        <div class="response-msg notice ui-corner-all" id="divSolicitacoesEmissoes" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Razão Social / Nome Fantasia:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtRazaoSocial_NomeFantasia" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        CNPJ:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtCNPJ" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Placa do Veículo:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtPlacaVeiculo" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="buttons">
                                    <input type="button" id="btnFiltrarEmpresas" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_empresas_emissoras">
                            </div>
                            <div id="tbl_paginacao_empresas_emissoras" class="pagination">
                            </div>
                        </div>
                        <div id="divCertificadosVencidos" style="display: none; margin-left: 5px;">
                            <h3>Empresas com Certificado Vencido ou à Vencer (em até 10 dias)</h3>
                            <div class="table">
                                <table id="tblCertificadosVencidos" style="width: 800px;">
                                    <thead>
                                        <tr>
                                            <th style="width: 15%;">CNPJ</th>
                                            <th style="width: 45%;">Nome</th>
                                            <th style="width: 15%;">Vencto.</th>
                                            <th style="width: 25%;">Emp. Admin</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
