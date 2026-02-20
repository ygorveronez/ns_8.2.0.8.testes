<%@ Page Title="Relatório de Permissão Usuários" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioPermissaoUsuario.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioPermissaoUsuario" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            //$("#txtUsuario").keydown(function (e) {
            //    if (e.which != 9 && e.which != 16) {
            //        if (e.which == 8 || e.which == 46) {
            //            $(this).val("");
            //            $("body").data("usuario", 0);
            //        } else {
            //            e.preventDefault();
            //        }
            //    }
            //});

            $("#txtPerfil").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("perfil", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeUsuarios("btnBuscarUsuario", "btnBuscarUsuario", RetornoConsultaUsuario, true, false);
            CarregarConsultaDePerfil("btnBuscarPerfil", "btnBuscarPerfil", RetornoConsultaPerfil, true, false);
        });

        function DownloadRelatorio() {
            var dados = {
                //Codigo: $("body").data("usuario"),
                Nome: $("#txtUsuario").val(),
                Login: $("#txtLogin").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                TipoRelatorio: $("#selTipo").val(),
                Perfil: $("body").data("perfil")
            };

            executarDownload("/Usuario/DownloadRelatorioPermissoes", dados);
        }

        function RetornoConsultaUsuario(usuario) {
            //$("body").data("usuario", usuario.Codigo);
            $("#txtUsuario").val(usuario.Nome);
        }

        function RetornoConsultaPerfil(perfil) {
            $("body").data("perfil", perfil.Codigo);
            $("#txtPerfil").val(perfil.Descricao);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Permissão Usuários
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Nome:
                </span>
                <input type="text" id="txtUsuario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarUsuario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Login:
                </span>
                <input type="text" id="txtLogin" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Perfil:
                </span>
                <input type="text" id="txtPerfil" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPerfil" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">Permissão</option>
                    <option value="1">Perfil</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativos</option>
                    <option value="I">Inativos</option>
                    <option value="">Todos</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
