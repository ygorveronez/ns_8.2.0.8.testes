<%@ Page Title="Cadastro de Formulários" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CadastroFormularios.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CadastroFormularios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var path = "";
        var Pagina = {
            Codigo: 0
        };
        $(document).ready(function () {
            CarregarConsultaDePaginas("default-search", "default-search", "true", "true", Editar, true, false);
            CarregarConsultaDeMenus("btnSelecionarMenu", "btnSelecionarMenu", RetornoMenus, true, false);

            LimparCampos();
            
            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                Cancelar();
            });
        });

        function RetornoMenus(menu) {
            $("#txtMenu").val(menu.Descricao);
        }

        function Editar(pagina) {
            // Limpa os campos do formulario
            LimparCampos();

            // Habilita o botao
            $("#btnCancelar").show();

            // Vincula ao objeto global
            Pagina = pagina;

            // Seta as ifnormacoes
            $("#txtFormulario").val(Pagina.Formulario);
            $("#txtDescricao").val(Pagina.Descricao);
            $("#txtMenu").val(Pagina.Menu);
            $("#selTipoAcesso").val(Pagina.TipoAcesso);
            $("#selStatus").val(Pagina.Status);
            $("#chkMostrarMenu").attr("checked", Pagina.MostraNoMenu == true);
        }

        function Cancelar() {
            // Limpa os campos do formulario
            LimparCampos();

            // Esconde o botao
            $("#btnCancelar").hide();
        }
        
        function Salvar() {
            // Informacoes do formulario
            var pagina = {
                Formulario: $("#txtFormulario").val(),
                Descricao: $("#txtDescricao").val(),
                Menu: $("#txtMenu").val(),
                TipoAcesso: $("#selTipoAcesso").val(),
                Status: $("#selStatus").val(),
                MostraNoMenu: $("#chkMostrarMenu").is(":checked"),
            };

            // Codigo da pagina (caso seja edicao)
            pagina.Codigo = Pagina.Codigo;

            executarRest("/Pagina/SalvarPagina?callback=?", pagina, function (r) {
                if (r.Sucesso) {
                    // Excibe mensagem
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");

                    // Cancela operacao
                    Cancelar();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function LimparCampos() {
            Pagina = {
                Codigo: 0
            };
            $("#txtFormulario").val("");
            $("#txtDescricao").val("");
            $("#txtMenu").val("");
            $("#selTipoAcesso").val($("#selTipoAcesso option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#chkMostrarMenu").attr("checked", false);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Formulários
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top:15px">
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
                                    <label>Formulário:</label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtFormulario" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Descrição:</label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Menu:</label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtMenu" />
                                </div>
                            </div>
                            <div class="field fieldum">
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnSelecionarMenu" value="Selecione" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label"></div>
                                <div class="checkbox">
                                    <input type="checkbox" id="chkMostrarMenu" /> <label for="chkMostrarMenu">Mostrar no menu</label>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Tipo do Acesso:</label>
                                </div>
                                <div class="input">
                                    <select id="selTipoAcesso">
                                        <option value="1">Admin</option>
                                        <option value="0">Emissão</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Status:</label>
                                </div>
                                <div class="input">
                                    <select id="selStatus">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvar" value="Salvar" />
                                <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
