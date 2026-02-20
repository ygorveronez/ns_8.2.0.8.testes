<%@ Page Title="Produto Fornecedor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProdutoFornecedor.aspx.cs" Inherits="EmissaoCTe.WebApp.ProdutoFornecedor" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker")%>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/produtoFornecedor")%>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Produto Fornecedor
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder"></div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Número ou código que consta na nota enviada pelo fornecedor">Número Fornecedor</abbr>*:
                </span>
                <input type="text" id="txtNumeroFornecedor" class="form-control" value="" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Fornecedor*:
                </span>
                <input type="text" id="txtFornecedor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFornecedor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Produto referente ao número do fornecedor">Produto</abbr>*:
                </span>
                <input type="text" id="txtProduto" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarProduto" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <br />
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
    <button type="button" id="btnExcluir" class="btn btn-danger" style="display: none">Excluir</button>
</asp:Content>
