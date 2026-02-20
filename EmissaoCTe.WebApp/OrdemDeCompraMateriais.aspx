<%@ Page Title="Ordem De Compra" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OrdemDeCompraMateriais.aspx.cs" Inherits="EmissaoCTe.WebApp.OrdemDeCompraMateriais" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: System.Web.Optimization.Styles.Render("~/bundle/styles/datetimepicker") %>
        <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/fileDownload",
                           "~/bundle/scripts/ordemDeCompraMateriais") %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Requisição de Materiais</h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Número para controle interno">Número</abbr>:
                </span>
                <input type="text" id="txtNumero" class="form-control" maxlength="100" />
            </div>
        </div>               
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data da ordem de compra">Data</abbr>*:
                </span>
                <input type="text" id="txtData" class="form-control" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Selecione um solicitante ou digite o nome">Solicitante</abbr>*:
                </span>
                <input type="text" id="txtNomeSolicitante" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarSolicitate" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Setor:</span>
                <input type="text" id="txtSetor" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Selecione um fornecedor">Fornecedor</abbr>*:
                </span>
                <input type="text" id="txtFornecedor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFornecedor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6">
            <div class="input-group">
                <span class="input-group-addon">Itens:</span>
                <input type="text" id="txtServico" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Descrição da compra ou serviço">Descrição</abbr>*:
                </span>
                <textarea id="txtDescricao" class="form-control" rows="3" maxlength="1000"></textarea>
            </div>
        </div>
    </div>
    
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnDownloadOrdem" class="btn btn-primary" style="display: none;"><span class="glyphicon glyphicon-download"></span>&nbsp;Visualizar Ordem</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>