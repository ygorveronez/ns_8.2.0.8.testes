<%@ Page Title="Exportação de Dados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ExportacaoDados.aspx.cs" Inherits="EmissaoCTe.WebApp.ExportacaoDados" %>
<%@ Import Namespace="System.Web.Optimization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/importacao.css" rel="stylesheet">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/plupload",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/exportacaoDados")%>
        <style>
            .boxes {
                margin-top: 20px;
            }
            .boxes li {
                width: 15%;
            }
            .boxes .box {
                padding: 35px 0;
            }
            .boxes .box:hover,
            .boxes .box:focus {
                text-decoration: none;
            }
        </style>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Exportação de Dados</h2>
    </div>
    <div class="importador">
		<div class="header-container">				
            <ul id="opcoesExportacoes" class="boxes tipos-importacao">
                <li>
                    <a href="#" class="box tipo-importacao" data-tipo="veiculos">
                        <div class="box-icon">
                            <span class="glyphicon glyphicon-road"></span>
                        </div>
                        <div class="box-title">Veículos</div>
                    </a>
                </li>
                            
                <li>
                    <a href="#" class="box tipo-importacao" data-tipo="motoristas">
                        <div class="box-icon">
                            <span class="glyphicon glyphicon-user"></span>
                        </div>
                        <div class="box-title">Motoristas</div>
                     </a>
                </li>
            </ul>
		</div>

        <div id="messages-placeholder"></div>
    </div>
</asp:Content>
